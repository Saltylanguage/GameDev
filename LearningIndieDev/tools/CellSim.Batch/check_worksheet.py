"""Focused checks for checkpoint timing, truthful summaries and safe cohort selection."""
import json
import tempfile
import unittest
import zipfile
import xml.etree.ElementTree as ET
from pathlib import Path
from unittest.mock import patch

import worksheet as w


def snapshot(tick, hares, foxes=15, plants=100):
    return dict(tick=tick, species=[dict(speciesId=s, population=p)
                for s, p in zip(w.SPECIES, (hares, foxes, plants)) if p is not None])


def fixture(seed=1, tick=600, final=12):
    return dict(seed=seed, ticks=tick, populationHistory=[snapshot(0, 30), snapshot(400, 28),
                snapshot(tick, final)],
        phaseResults=[dict(phaseIndex=4, windowStartTickExclusive=300, windowEndTickInclusive=400,
                          closingPopulation=[snapshot(400, 23)]),
                      dict(phaseIndex=5, windowStartTickExclusive=400, windowEndTickInclusive=tick,
                          closingPopulation=[snapshot(tick, final)])],
        purchaseWindows=[dict(decisionTick=400, populationBefore=23, populationAfter=28,
                              requested=5, added=5, spent=50, stopReason='policy-cap', policy='restoration')],
        mutationChoices=[dict(decisionTick=400, chosenUpgradeId='efficient-digestion')],
        herbivoreStatLines=[dict(speciesId='hare', BIR=10, PREY=20, STRV=13, CRWD=0)])


class WorksheetChecks(unittest.TestCase):
    def setUp(self):
        self.context = dict(label='Fixture / restoration', horizon=600, phaseLength=100,
                            condition='fixture', source='fixture/report.json')

    def test_pre_purchase_checkpoint_and_post_purchase_recovery(self):
        row = w.review_row(fixture(), self.context, [400, 500], detailed=True)
        self.assertEqual(row[1], '23 / 15 / 100')
        self.assertIn('28 -> 12', row[6])
        self.assertIn('-16, excluding the immediate purchase', row[6])
        self.assertEqual(row[7], '')
        self.assertIn('Tick 400: Efficient digestion; +5 Hares; 50 Field Data', row[4])
        self.assertIn('Not recorded', row[2])

    def test_extinction_is_zero_not_missing_or_completion(self):
        row = w.review_row(fixture(tick=491, final=0), self.context, [400, 500])
        self.assertEqual(row[2], 'Not reached')
        self.assertEqual(row[3], 'Tick 491\n0 / 15 / 100')
        self.assertIn('Hares became extinct by tick 491', row[6])
        self.assertIn('Tick 500: not reached', row[4])

    def test_missing_population_and_horizon_are_explicit(self):
        run = fixture()
        run['populationHistory'][-1] = snapshot(600, 12, plants=None)
        run['phaseResults'][-1]['closingPopulation'] = [snapshot(600, 12, plants=None)]
        context = dict(self.context, horizon=None)
        row = w.review_row(run, context, [400, 500])
        self.assertIn('Not recorded', row[3])
        self.assertIn('Completion reason not recorded', row[6])
        self.assertNotIn('Reached the', row[6])

    def test_missing_purchases_not_zero_and_all_reasons_visible(self):
        run = fixture()
        del run['purchaseWindows']
        self.assertEqual(w.review_row(run, self.context, [400, 500])[5], 'Purchase ledger not recorded')
        for reason, text in w.REASONS.items():
            run = fixture()
            run['purchaseWindows'][0]['stopReason'] = reason
            self.assertIn(text, w.review_row(run, self.context, [400, 500])[5])

    def test_invalid_populations_rejected(self):
        for value in (-1, True, 2.5):
            with self.assertRaises(ValueError):
                w.population(snapshot(0, value))

    def test_peak_includes_post_purchase_observations(self):
        run = fixture()
        run['populationHistory'][0] = snapshot(0, 10)
        self.assertIn('Highest recorded Hare count: 28 at tick 400',
                      w.review_row(run, self.context, [400, 500])[6])

    def test_native_evidence_links_preserve_content_and_refuse_duplicate_finalization(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'review.xlsx'
            namespace = 'http://schemas.openxmlformats.org/spreadsheetml/2006/main'
            original = f'<worksheet xmlns="{namespace}"><sheetData/><pageMargins/><tableParts/></worksheet>'
            with zipfile.ZipFile(path, 'w') as archive:
                archive.writestr('xl/workbook.xml', f'<workbook xmlns="{namespace}" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets><sheet name="Batch summary" r:id="summary"/><sheet name="Run review" r:id="review"/></sheets></workbook>')
                archive.writestr('xl/_rels/workbook.xml.rels', '<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="summary" Target="worksheets/sheet1.xml"/><Relationship Id="review" Target="worksheets/sheet2.xml"/></Relationships>')
                archive.writestr('xl/worksheets/sheet2.xml', original)
                archive.writestr('unchanged.txt', 'keep this evidence')
            w.link_evidence(path, [directory])
            with zipfile.ZipFile(path) as archive:
                self.assertEqual(archive.read('unchanged.txt'), b'keep this evidence')
                sheet = ET.fromstring(archive.read('xl/worksheets/sheet2.xml'))
                self.assertEqual([child.tag.split('}')[-1] for child in sheet],
                                 ['sheetData', 'hyperlinks', 'pageMargins', 'tableParts'])
                self.assertEqual(sheet.find(f'{{{namespace}}}hyperlinks')[0].get('ref'), 'I6')
                relations = ET.fromstring(archive.read('xl/worksheets/_rels/sheet2.xml.rels'))
                self.assertEqual(relations[0].get('Target'), Path(directory).as_uri() + '/')
            before = path.read_bytes()
            with self.assertRaisesRegex(ValueError, 'already exist'):
                w.link_evidence(path, [directory])
            self.assertEqual(before, path.read_bytes())

    def test_inconsistent_purchase_ledger_rejected(self):
        run = fixture()
        run['purchaseWindows'][0]['populationAfter'] = 30
        with self.assertRaisesRegex(ValueError, 'does not reconcile'):
            w.review_row(run, self.context, [400, 500])
        run = fixture()
        run['purchaseWindows'][0].update(price=10, spent=10)
        with self.assertRaisesRegex(ValueError, 'does not reconcile'):
            w.review_row(run, self.context, [400, 500])

    def test_all_run_summary_and_order_independent_bounded_selection(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'report.json'
            runs = [fixture(seed=i, final=0 if i % 2 else 12) for i in [5, 1, 4, 2, 3]]
            report = dict(runs=runs, runTicks=600, mutationPolicyId='gardeners')
            path.write_text(json.dumps(report))
            result = w.prepare([path], max_runs=2)
            self.assertEqual(result['totalRuns'], 5)
            self.assertEqual(result['exportedRows'], 2)
            self.assertEqual([r[0].splitlines()[0] for r in result['rows']], ['1', '2'])
            self.assertEqual(result['groups'][0]['totals']['extinct'], 3)
            self.assertEqual(result['summaryRows'][0][3:7], [2, .4, 2, .4])
            report['runs'].reverse()
            path.write_text(json.dumps(report))
            self.assertEqual(w.prepare([path], max_runs=2)['rows'], result['rows'])
            selected = w.prepare([path], seeds=[4], match='gardeners')
            self.assertEqual(selected['totalRuns'], 1)
            self.assertEqual(selected['rows'][0][0].splitlines()[0], '4')

    def test_concise_default_preserves_detailed_option(self):
        compact = w.review_row(fixture(), self.context, [400, 500])
        detailed = w.review_row(fixture(), self.context, [400, 500], detailed=True)
        self.assertEqual(len(compact[6].splitlines()), 3)
        self.assertNotIn('excluding the immediate purchase', compact[6])
        self.assertIn('excluding the immediate purchase', detailed[6])
        self.assertEqual(compact[:6], detailed[:6])

    def test_summary_distinguishes_extinction_at_horizon_and_missing_species(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'report.json'
            missing = fixture(seed=2)
            missing['phaseResults'][-1]['closingPopulation'] = [snapshot(600, 12, plants=None)]
            path.write_text(json.dumps(dict(runTicks=600, runs=[fixture(final=0), missing])))
            result = w.prepare([path])
            self.assertEqual(result['summaryRows'][0][3:10], [1, .5, 'Not recorded', 'Not recorded', 2, 1, 1])
            path.write_text(json.dumps(dict(runs=[fixture(final=0)])))
            self.assertEqual(w.prepare([path])['summaryRows'][0][3:8], ['Not recorded'] * 5)

    def test_actual_settings_and_frozen_experiment_context(self):
        context = dict(self.context, setup=w.settings({'-gridWidth':'54', '-gridHeight':'32', '-wrapEdges':'true',
            '-startingPopulations':'fox=15,hare=30,plant=325', '-speciesStats':'fox:energy.starting=160,hare:awareness.vision-range=9'}))
        self.assertEqual(context['setup']['populations'], '30 / 15 / 325')
        self.assertEqual(context['setup']['map'], '54 × 32 (wrapped)')
        self.assertIn('Hare vision 9',context['setup']['overrides'])
        with tempfile.TemporaryDirectory() as directory:
            folder = Path(directory); (folder/'sweep').mkdir()
            notes = folder/'experiment.json'
            notes.write_text(json.dumps(dict(Name='Readable test',Owner='Bevin',Question='What changes?')))
            import hashlib
            (folder/'workbench-run.json').write_text(json.dumps(dict(files={'experiment.json':hashlib.sha256(notes.read_bytes()).hexdigest()})))
            details = w.experiment_details(folder/'sweep/sweep.json', {'test':context}, {'test':{1,3}})
            self.assertEqual(details['name'],'Readable test')
            self.assertIn('(2 selected seeds)', details['seeds'])
            notes.write_text('{}')
            with self.assertRaisesRegex(ValueError,'notes changed'):
                w.experiment_details(folder/'sweep/sweep.json', {'test':context}, {'test':{1}})

    def test_duplicate_seeds_rejected_and_inputs_deduplicated(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'report.json'
            path.write_text(json.dumps(dict(runs=[fixture()])))
            self.assertEqual(w.prepare([path, path])['totalRuns'], 1)
            path.write_text(json.dumps(dict(runs=[fixture(), fixture()])))
            with self.assertRaisesRegex(ValueError, 'Duplicate seed'):
                w.prepare([path])

    def test_run_data_numeric_populations_purchases_and_phase_metrics(self):
        run = fixture()
        run['phaseResults'][0]['herbivoreStatLines'] = [dict(speciesId='hare', APS=.25, APSStatus='Valid')]
        run['predatorStatLines'] = [dict(speciesId='fox', KIL=20, AHS=-.5, AHSStatus='Valid')]
        data = w.run_data(run, self.context, [400, 500])
        self.assertEqual(data['Tick 400 before purchases Hare population'],23)
        self.assertIsNone(data['Tick 500 before purchases Hare population'])
        self.assertEqual(data['Tick 500 population status'],'Not recorded')
        self.assertEqual(data['Run Hare herbivore BIR'],10)
        self.assertEqual(data['Run Fox predator AHS'],-.5)
        self.assertEqual(data['Round 4 Hare herbivore APS'],.25)
        self.assertEqual(data['Decision 4 purchase added'],5)
        self.assertEqual(data['Decision 4 Hare change after immediate purchase'],-16)
        self.assertEqual(data['Decision 4 mutation chosenUpgradeId'],'efficient-digestion')

    def test_run_data_invalid_rates_are_blank_with_explicit_source_status(self):
        run = fixture(final=0)
        run['herbivoreStatLines'][0].update(APS=0, APSStatus='INVALID', cAVI=0, cAVIStatus='N/A', bAVG=0, bAVGStatus='Valid')
        data = w.run_data(run, self.context, [400, 500])
        self.assertEqual(data['Hares alive at horizon (0/1)'],0)
        self.assertEqual(data['Final Hare population'],0)
        self.assertIsNone(data['Run Hare herbivore APS'])
        self.assertEqual(data['Run Hare herbivore APS status'],'INVALID')
        self.assertIsNone(data['Run Hare herbivore cAVI'])
        self.assertEqual(data['Run Hare herbivore bAVG'],0)
        early=w.run_data(fixture(tick=491,final=0),self.context,[400,500])
        self.assertIsNone(early['Tick 500 before purchases Hare population'])
        self.assertEqual(early['Tick 500 population status'],'Not reached')

    def test_run_data_selection_matches_review_and_keeps_scalars(self):
        with tempfile.TemporaryDirectory() as directory:
            path=Path(directory)/'report.json'
            path.write_text(json.dumps(dict(runTicks=600,runs=[fixture(seed=i) for i in (3,1,2)])))
            result=w.prepare([path], max_runs=2)
            table=result['runData']; seed=table['headers'].index('Seed'); review=table['headers'].index('Run review row')
            self.assertEqual([r[seed] for r in table['rows']],[1,2])
            self.assertEqual([r[review] for r in table['rows']],[6,7])
            self.assertEqual(len(table['headers']),len(set(table['headers'])))
            self.assertTrue(all(v is None or isinstance(v,(str,int,float,bool)) for row in table['rows'] for v in row))

    def test_run_data_separates_executed_map_and_stat_overrides(self):
        context = dict(self.context, arguments={'-gridWidth':'54', '-gridHeight':'32', '-wrapEdges':'true',
            '-speciesStats':'fox:energy.starting=160,hare:movement.speed=2.2'})
        data = w.run_data(fixture(), context, [400,500])
        self.assertEqual(data['Map width'],54)
        self.assertEqual(data['Map height'],32)
        self.assertEqual(data['Wrap map edges (0/1)'],1)
        self.assertEqual(data['Stat override Fox energy.starting'],160)
        self.assertEqual(data['Stat override Hare movement.speed'],2.2)
        self.assertNotIn('Stat override Hare energy.starting',data)

    def test_run_data_fallback_and_known_empty_loadout_are_distinct(self):
        run=fixture()
        run.pop('herbivoreStatLines')
        run['herbivoreStatLine']=dict(speciesId='hare',BIR=2)
        run['predatorStatLine']=dict(speciesId='hare',KIL=999)
        run['upgradeLoadout']=[dict(upgradeId='seed-dispersal')]
        first=w.run_data(run,self.context,[400,500])
        self.assertEqual(first['Run Hare herbivore BIR'],2)
        self.assertNotIn('Run Hare predator KIL',first)
        run['upgradeLoadout']=[]
        empty=w.run_data(run,self.context,[400,500])
        del run['upgradeLoadout']
        missing=w.run_data(run,self.context,[400,500])
        table=w.data_table([first,empty,missing]); col=table['headers'].index('Final loadout seed-dispersal count')
        self.assertEqual([r[col] for r in table['rows']],[1,0,None])

    def test_run_data_rejects_duplicate_metrics_and_non_scalar_fields(self):
        run=fixture()
        run['herbivoreStatLines'].append(run['herbivoreStatLines'][0])
        with self.assertRaisesRegex(ValueError,'Duplicate herbivore'):
            w.run_data(run,self.context,[400,500])
        with self.assertRaisesRegex(ValueError,'non-scalar'):
            w.data_table([{'bad':['nested']}])

    def test_sweep_streaming_pairs_and_coverage(self):
        contexts = {key: dict(self.context, label=f'D5 S25 / Gardeners / {key}', condition=key)
                    for key in ['late-five', 'restoration']}
        runs = [(key, fixture(seed=seed)) for key in contexts for seed in [2, 1]]
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'sweep.json'
            path.touch()
            with patch.object(w, 'contexts_and_runs', return_value=(contexts, iter(runs), 2, 1)):
                result = w.prepare([path], max_runs=2, match='D5-S25 gardeners')
            self.assertEqual(result['totalRuns'], 4)
            self.assertEqual([r[0].splitlines()[0] for r in result['rows']], ['1', '1'])
            with patch.object(w, 'contexts_and_runs', return_value=(contexts, iter(runs[:-1]), 2, 1)):
                with self.assertRaisesRegex(ValueError, 'Seed coverage'):
                    w.prepare([path])

    def test_invalid_and_empty_inputs_fail(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'report.json'
            path.write_text(json.dumps(dict(runs=[])))
            with self.assertRaisesRegex(ValueError, 'nonempty'):
                w.prepare([path])
            path.write_text(json.dumps(dict(runs=[fixture()])))
            with self.assertRaisesRegex(ValueError, 'No runs matched'):
                w.prepare([path], match='does-not-exist')
            with self.assertRaises(ValueError):
                w.prepare([path], checkpoints=[100])


if __name__ == '__main__':
    unittest.main()
