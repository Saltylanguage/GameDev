"""Full-cohort statistics and matched controls, independent of Excel layout."""
import collections
import json
import math
import sqlite3
import tempfile
from pathlib import Path


def wilson(successes, total):
    """Two-sided 95% Wilson interval; NIST prc241, independent seed trials."""
    if not total:
        return None, None
    z = 1.959963984540054
    p, z2 = successes / total, z * z
    center = (p + z2 / (2 * total)) / (1 + z2 / total)
    half = z * math.sqrt(p * (1 - p) / total + z2 / (4 * total * total)) / (1 + z2 / total)
    return max(0., center - half), min(1., center + half)


def strategy(context):
    args = context.get('arguments', {})
    if args.get('-phaseUpgradeSchedule') == 'none;none;none;none;none;none':
        return 'skip-all'
    return args.get('-mutationPolicy') or context.get('strategy')


def facts(run, context, final, goals):
    horizon = context['horizon']
    known = horizon is not None and all(v is not None for v in final)
    met = int(run['ticks'] >= horizon and all(v >= g for v, g in zip(final, goals))) if known else None
    windows, choices = run.get('purchaseWindows'), run.get('mutationChoices')
    added = sum(p['added'] for p in windows) if isinstance(windows, list) else None
    recorded = isinstance(windows, list) or isinstance(choices, list)
    ticks = {p['decisionTick'] for p in (windows or []) + (choices or []) if p['decisionTick'] > 0}
    decisions = len(ticks) if recorded else None
    selected = sum(bool(c.get('chosenUpgradeId')) for c in choices) if isinstance(choices, list) else None
    no_purchase = int(met and added == 0) if met is not None and added is not None else None
    return met, added, decisions, selected, no_purchase


class EvidenceStats:
    """Temporary compact rows keep seed pairing bounded without retaining raw runs."""
    def __init__(self, goals):
        if len(goals) != 3 or any(type(v) is not int or not 1 <= v <= 2147483647 for v in goals):
            raise ValueError('Success floors must be three positive whole numbers: Hares, Foxes, Plants')
        self.goals = goals
        self.contexts = {}

    def __enter__(self):
        self.temp = tempfile.TemporaryDirectory(prefix='cellsim-statistics-')
        self.db = sqlite3.connect(Path(self.temp.name) / 'outcomes.sqlite')
        self.db.execute('CREATE TABLE outcomes (case_id INTEGER, seed INTEGER, ticks INTEGER, hare INTEGER, fox INTEGER, plant INTEGER, goal INTEGER, added INTEGER, decisions INTEGER, choices INTEGER, no_purchase INTEGER, births INTEGER, PRIMARY KEY(case_id,seed)) WITHOUT ROWID')
        return self

    def __exit__(self, *error):
        self.db.close()
        self.temp.cleanup()

    def register(self, contexts):
        ids = {}
        for key, context in contexts.items():
            case_id = len(self.contexts)
            self.contexts[case_id] = context
            ids[key] = case_id
        return ids

    def add(self, case_id, run, final):
        values = facts(run, self.contexts[case_id], final, self.goals)
        lines = run.get('herbivoreStatLines') or [run.get('herbivoreStatLine') or {}]
        births = next((line.get('BIR') for line in lines if line.get('speciesId') == 'hare'), None)
        if births is not None and (type(births) is not int or births < 0):
            raise ValueError('Recorded Hare births must be a nonnegative whole number')
        self.db.execute('INSERT INTO outcomes VALUES (?,?,?,?,?,?,?,?,?,?,?,?)',
                        (case_id, run['seed'], run['ticks'], *final, *values, births))
        return values

    def summary(self, case_id):
        n, known, met, hare, fox, plant, added, decisions, choices, no_purchase, no_purchase_known, hare_known, fox_known, plant_known = self.db.execute(
            'SELECT COUNT(*),COUNT(goal),SUM(goal),AVG(hare),AVG(fox),AVG(plant),AVG(added),AVG(decisions),AVG(choices),SUM(no_purchase),COUNT(no_purchase),COUNT(hare),COUNT(fox),COUNT(plant) FROM outcomes WHERE case_id=?', (case_id,)).fetchone()
        rate = met / n if n == known and n else None
        lower, upper = wilson(met, n) if rate is not None else (None, None)
        median = self.db.execute('SELECT AVG(ticks) FROM (SELECT ticks FROM outcomes WHERE case_id=? ORDER BY ticks LIMIT ? OFFSET ?)', (case_id, 2 if n % 2 == 0 else 1, (n - 1) // 2)).fetchone()[0]
        context = self.contexts[case_id]
        expected = len(range(context['phaseLength'], context['horizon'], context['phaseLength'])) if context['phaseLength'] and context['horizon'] else None
        complete = self.db.execute('SELECT SUM(decisions>=?),COUNT(decisions),COUNT(added),COUNT(choices) FROM outcomes WHERE case_id=?', (expected, case_id)).fetchone()
        mean_births, no_births, births_known = self.db.execute('SELECT AVG(births),SUM(births=0),COUNT(births) FROM outcomes WHERE case_id=?', (case_id,)).fetchone()
        return {'Goal met': met if n == known else None, 'Goal success rate': rate,
                'Goal 95% lower': lower, 'Goal 95% upper': upper, 'Unknown goal outcomes': n - known,
                'Median endpoint tick': median, 'Mean final Hares': hare if hare_known == n else None,
                'Mean final Foxes': fox if fox_known == n else None, 'Mean final Plants': plant if plant_known == n else None,
                'Goal met with no purchased Hares': no_purchase if n == no_purchase_known else None,
                'Unassisted goal met': no_purchase if n == no_purchase_known and strategy(context) == 'skip-all' and context.get('arguments', {}).get('-harePurchasePolicy') == 'none' else None,
                'Mean purchased Hares': added if complete[2] == n else None,
                'Expected decision windows': expected, 'Mean recorded decision windows': decisions if complete[1] == n else None,
                'Runs reaching every decision window': complete[0] if expected and complete[1] == n else None,
                'Mean non-skip Mutation choices': choices if complete[3] == n else None,
                'Mean Hare births': mean_births if births_known == n else None,
                'Runs with no Hare births': no_births if births_known == n else None}

    def comparisons(self, selected):
        rows = []
        for kind, excluded in [('Mutations', {'-mutationPolicy', '-phaseUpgradeSchedule'}), ('Purchases', {'-harePurchasePolicy'})]:
            def signature(context):
                args = context.get('arguments')
                if not args or strategy(context) not in ('skip-all', 'trailblazer', 'warren', 'gardeners'):
                    return None
                return context['source'], context['horizon'], json.dumps({k: v for k, v in args.items() if k not in excluded}, sort_keys=True)
            controls = collections.defaultdict(list)
            for case_id, context in self.contexts.items():
                baseline = strategy(context) == 'skip-all' if kind == 'Mutations' else context.get('arguments', {}).get('-harePurchasePolicy') == 'none'
                if baseline and signature(context) is not None:
                    controls[signature(context)].append(case_id)
            for case_id in selected:
                context = self.contexts[case_id]
                value = strategy(context) if kind == 'Mutations' else context.get('arguments', {}).get('-harePurchasePolicy')
                if value == ('skip-all' if kind == 'Mutations' else 'none'):
                    continue
                candidates = controls.get(signature(context), [])
                n = self.db.execute('SELECT COUNT(*) FROM outcomes WHERE case_id=?', (case_id,)).fetchone()[0]
                row = {'Comparison': kind, 'Condition': context['label'], 'Control condition': None,
                       'Matching runs': n, 'Matched seeds': 0, 'Missing control seeds': n,
                       'Unknown paired goals': None, 'Goal wins': None, 'Goal losses': None, 'Goal ties': None,
                       'Paired goal rate change': None, 'Mean endpoint tick change': None,
                       'Mean final Hare change': None, 'Pairs with non-skip Mutations': None,
                       'Pairs with purchased Hares': None, 'Status': 'Control not available',
                       'Condition ID': context['condition'], 'Control condition ID': None, 'Source report': context['source']}
                if len(candidates) != 1:
                    if len(candidates) > 1:
                        row['Status'] = 'Ambiguous control'
                    rows.append(row)
                    continue
                control = candidates[0]
                pairs, known, wins, losses, ties, tick_change, hare_change, hare_known, changed, bought, choices_known, purchases_known = self.db.execute('''
                    SELECT COUNT(*),COUNT(a.goal+b.goal),SUM(a.goal=1 AND b.goal=0),SUM(a.goal=0 AND b.goal=1),SUM(a.goal=b.goal),
                           AVG(a.ticks-b.ticks),AVG(a.hare-b.hare),COUNT(a.hare+b.hare),SUM(a.choices>0),SUM(a.added>0),COUNT(a.choices),COUNT(a.added)
                    FROM outcomes a JOIN outcomes b ON a.seed=b.seed AND b.case_id=? WHERE a.case_id=?''', (control, case_id)).fetchone()
                row.update({'Control condition': self.contexts[control]['label'], 'Control condition ID': self.contexts[control]['condition'],
                            'Matched seeds': pairs, 'Missing control seeds': n - pairs, 'Unknown paired goals': pairs - known,
                            'Goal wins': wins if pairs == known and pairs else None, 'Goal losses': losses if pairs == known and pairs else None,
                            'Goal ties': ties if pairs == known and pairs else None,
                            'Paired goal rate change': (wins - losses) / pairs if pairs == known and pairs else None,
                            'Mean endpoint tick change': tick_change, 'Mean final Hare change': hare_change if pairs == hare_known else None,
                            'Pairs with non-skip Mutations': changed if choices_known == pairs else None,
                            'Pairs with purchased Hares': bought if purchases_known == pairs else None,
                            'Status': 'Paired observed seeds' if pairs and not (n - pairs) and pairs == known else 'Incomplete paired evidence'})
                rows.append(row)
        return rows
