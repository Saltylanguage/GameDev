"""Independent small cohorts for success, ranking and paired-control calculations."""
import json
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

import worksheet as w
from worksheet_stats import wilson
from check_worksheet import fixture, snapshot


class StatisticalChecks(unittest.TestCase):
    def report(self, runs, **options):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'report.json'
            path.write_text(json.dumps(dict(runTicks=600, runs=runs)))
            return w.prepare([path], **options)

    def test_wilson_limits_include_uncertainty_at_zero_and_full_success(self):
        self.assertEqual(wilson(0, 0), (None, None))
        self.assertAlmostEqual(wilson(0, 2)[1], .6576197724933469)
        self.assertAlmostEqual(wilson(2, 2)[0], .342380227506653)
        self.assertAlmostEqual(wilson(0, 1000)[1], .00382675848555512)
        for success in (0, 1, 53, 1000):
            low, high = wilson(success, 1000)
            self.assertLessEqual(low, success / 1000 + 1e-12)
            self.assertGreaterEqual(high, success / 1000 - 1e-12)

    def test_full_statistics_do_not_use_the_ranked_shortlist(self):
        runs = [fixture(seed=1, final=0), fixture(seed=2, tick=200, final=0), fixture(seed=90, final=12), fixture(seed=99, final=20)]
        result = self.report(runs, max_runs=1)
        self.assertEqual(result['rows'][0][0].splitlines()[0], '99')
        stats = result['groups'][0]['statistics']
        self.assertEqual(result['totalRuns'], 4)
        self.assertEqual(stats['Goal met'], 2)
        self.assertEqual(stats['Goal success rate'], .5)
        self.assertEqual(stats['Median endpoint tick'], 600)
        self.assertEqual(stats['Mean final Hares'], 8)
        self.assertEqual(stats['Mean purchased Hares'], 5)
        self.assertEqual(stats['Goal met with no purchased Hares'], 0)

    def test_success_floors_and_missing_outcomes_are_not_invented(self):
        run = fixture()
        self.assertEqual(self.report([run], goals=(13,5,100))['groups'][0]['statistics']['Goal met'], 0)
        run['phaseResults'][-1]['closingPopulation'] = [snapshot(600,12,plants=None)]
        result = self.report([run])['groups'][0]['statistics']
        self.assertIsNone(result['Goal success rate'])
        self.assertIsNone(result['Goal 95% lower'])
        self.assertEqual(result['Unknown goal outcomes'], 1)
        self.assertEqual(result['Mean final Hares'], 12)
        self.assertIsNone(result['Mean final Plants'])
        for goals in ((0,1,1), (True,1,1), (1.5,1,1), (1,1), (1,1,2147483648)):
            with self.assertRaises(ValueError):
                self.report([fixture()], goals=goals)

    def test_hare_births_keep_zero_and_missing_evidence_distinct(self):
        first, second = fixture(seed=1), fixture(seed=2)
        first['herbivoreStatLines'] = [{'speciesId': 'hare', 'BIR': 0}]
        second['herbivoreStatLines'] = [{'speciesId': 'hare', 'BIR': 4}]
        stats = self.report([first, second])['groups'][0]['statistics']
        self.assertEqual(stats['Mean Hare births'], 2)
        self.assertEqual(stats['Runs with no Hare births'], 1)
        del second['herbivoreStatLines']
        second.pop('herbivoreStatLine', None)
        stats = self.report([first, second])['groups'][0]['statistics']
        self.assertIsNone(stats['Mean Hare births'])
        self.assertIsNone(stats['Runs with no Hare births'])

    def test_shortlist_is_order_independent_and_prefers_success_without_reinforcement(self):
        purchased, independent, failed = fixture(seed=99,final=50), fixture(seed=90,final=12), fixture(seed=1,final=0)
        independent['purchaseWindows'] = []
        result = self.report([purchased,independent,failed], max_runs=2)
        self.assertEqual([r[0].splitlines()[0] for r in result['rows']], ['90','99'])
        reordered=self.report([failed,independent,purchased],max_runs=2)
        content=lambda rows: [(r[0].splitlines()[0],*r[1:8]) for r in rows]
        self.assertEqual(content(result['rows']),content(reordered['rows']))
        table = result['runData']
        self.assertEqual([r[table['headers'].index('Shortlist rank')] for r in table['rows']], [1,2])
        self.assertEqual([r[table['headers'].index('Goal met without purchased Hares (0/1)')] for r in table['rows']], [1,0])

    def test_absent_purchase_ledger_is_not_unassisted_success(self):
        run = fixture()
        del run['purchaseWindows']
        stats = self.report([run])['groups'][0]['statistics']
        self.assertEqual(stats['Goal met'], 1)
        self.assertIsNone(stats['Goal met with no purchased Hares'])
        self.assertIsNone(stats['Mean purchased Hares'])

    def paired(self, missing=False, ambiguous=False, match='choices', unknown=False):
        contexts = {}
        cases = [('control','skip-all','none','36'), ('choices','gardeners','none','36'),
                 ('other-map','gardeners','none','40'), ('purchases','gardeners','each-one','36')]
        if ambiguous:
            cases.append(('control-copy','skip-all','none','36'))
        rows = []
        for key, strategy, policy, width in cases:
            args = {'-harePurchasePolicy':policy,'-gridWidth':width,'-gridHeight':'20','-phaseLengthTicks':'100'}
            args.update({'-phaseUpgradeSchedule':'none;none;none;none;none;none'} if strategy=='skip-all' else {'-mutationPolicy':strategy})
            contexts[key] = dict(label=key,condition=key,source='one-batch/runs.jsonl',horizon=600,phaseLength=100,arguments=args,identity='same-batch',sourceHash='same-revision')
            finals = [0,12,0,12] if strategy=='skip-all' else [12,0,0,12]
            for seed, final in enumerate(finals, 1):
                if missing and key=='control' and seed==4:
                    continue
                run = fixture(seed=seed,final=final)
                if unknown and key=='choices' and seed==1:
                    run['phaseResults'][-1]['closingPopulation']=[snapshot(600,final,plants=None)]
                if policy=='none':
                    run['purchaseWindows'] = []
                if strategy=='skip-all':
                    run['mutationChoices'] = None
                rows.append((key,run))
        with tempfile.TemporaryDirectory() as directory:
            path=Path(directory)/'report.json'; path.write_text('{}')
            with patch.object(w,'contexts_and_runs',return_value=(contexts,iter(rows),None,None)):
                return w.prepare([path],match=match)

    def test_paired_controls_match_seeds_and_settings_even_outside_text_filter(self):
        result=self.paired()
        self.assertEqual(result['totalRuns'],4)
        table=result['choiceImpact']; rows=[dict(zip(table['headers'],r)) for r in table['rows']]
        row=next(r for r in rows if r['Comparison']=='Mutations')
        self.assertEqual(row['Control condition ID'],'control')
        self.assertEqual(row['Matched seeds'],4)
        self.assertEqual([row['Goal wins'],row['Goal losses'],row['Goal ties']],[1,1,2])
        self.assertEqual(row['Paired goal rate change'],0)
        self.assertEqual(row['Status'],'Paired observed seeds')
        self.assertEqual(row['Pairs with non-skip Mutations'],4)

    def test_missing_and_ambiguous_controls_are_visible(self):
        table=self.paired(missing=True)['choiceImpact']; row=dict(zip(table['headers'],table['rows'][0]))
        self.assertEqual(row['Matched seeds'],3)
        self.assertEqual(row['Missing control seeds'],1)
        self.assertEqual(row['Status'],'Incomplete paired evidence')
        table=self.paired(ambiguous=True)['choiceImpact']; row=dict(zip(table['headers'],table['rows'][0]))
        self.assertEqual(row['Status'],'Ambiguous control')
        self.assertIsNone(row['Paired goal rate change'])

    def test_purchase_comparison_holds_strategy_and_ecology_fixed(self):
        table=self.paired(match='')['choiceImpact']; rows=[dict(zip(table['headers'],r)) for r in table['rows']]
        purchase=next(r for r in rows if r['Comparison']=='Purchases')
        self.assertEqual(purchase['Control condition ID'],'choices')
        self.assertEqual(purchase['Matched seeds'],4)
        self.assertEqual(purchase['Pairs with purchased Hares'],4)
        self.assertEqual([purchase['Goal wins'],purchase['Goal losses'],purchase['Goal ties']],[0,0,4])
        different=next(r for r in rows if r['Condition ID']=='other-map')
        self.assertEqual(different['Status'],'Control not available')

    def test_unknown_paired_goal_is_not_a_loss_or_a_zero_effect(self):
        table=self.paired(unknown=True)['choiceImpact']; row=dict(zip(table['headers'],table['rows'][0]))
        self.assertEqual(row['Unknown paired goals'],1)
        self.assertIsNone(row['Paired goal rate change'])
        self.assertIsNone(row['Goal losses'])


if __name__=='__main__':
    unittest.main()
