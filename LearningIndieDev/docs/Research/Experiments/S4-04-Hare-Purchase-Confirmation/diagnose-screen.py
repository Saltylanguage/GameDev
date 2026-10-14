"""Decompose selected screen scores and compare phases only on identical observed windows."""
import csv
import json
import math
import statistics
import sys
from pathlib import Path

project = Path(__file__).resolve().parents[4]
source = project / 'artifacts/cellsim-hare-purchase-20261008-132317'
destination = Path(sys.argv[1]).resolve()
destination.mkdir(exist_ok=False)
matrix = json.loads((source / 'sweep/sweep.json').read_text())
policies = ['none', 'late-five', 'restore-toward-start', 'each-five']
cases = {c['id']:c for c in matrix['cases'] if c['path'] == 'gardeners'
         and c['axes']['candidate'] in ['D5-S25','C2-S14'] and c['axes']['purchase'] in policies}
observations = {}

def decompose(stat, score, average):
    assert stat[score+'Status'] == 'Valid'
    def term(key, penalty=False):
        status = stat[key+'Status']
        assert status in ['Valid','N/A']
        return (-(1-stat[key]) if penalty else stat[key]) if status == 'Valid' else 0.
    parts = [term('RFS'), term(average), term('sAVI',True), term('cAVI',True)]
    assert math.isclose(sum(parts),stat[score],rel_tol=1e-6,abs_tol=1e-6)
    return dict(zip([score+'_RFS',score+'_interaction',score+'_starvation',score+'_crowding'],parts), **{score:stat[score]})

with (source / 'sweep/batch/runs.jsonl').open(encoding='utf-8-sig') as stream:
    for line in stream:
        row = json.loads(line)
        if row['condition'] not in cases:
            continue
        case, run = cases[row['condition']], row['run']
        candidate, policy = case['axes']['candidate'], case['axes']['purchase']
        for phase in [run]+run['phaseResults']:
            index = phase.get('phaseIndex',0)
            hare = next(s for s in phase['herbivoreStatLines'] if s['speciesId']=='hare')
            fox = next(s for s in phase['predatorStatLines'] if s['speciesId']=='fox')
            values = decompose(hare,'APS','predAVG') | decompose(fox,'AHS','huntAVG')
            values.update(hareBirths=hare['BIR'], harePredation=hare['PREY'], hareStarvation=hare['STRV'],
                          hareCrowding=hare['CRWD'], hareFinal=hare['FPO'], hareAdded=hare['ADD'],
                          foxBirths=fox['BIR'], foxStarvation=fox['STRV'], foxFinal=fox['FPO'])
            window = (phase.get('windowStartTickExclusive',0),phase.get('windowEndTickInclusive',run['ticks']))
            observations[(candidate,policy,run['seed'],index)] = dict(window=window,values=values)
assert sum(k[3]==0 for k in observations) == 800
fields = list(next(iter(observations.values()))['values'])

def save(name, rows):
    with (destination / name).open('x',newline='',encoding='utf-8') as stream:
        writer=csv.DictWriter(stream,fieldnames=list(rows[0]));writer.writeheader();writer.writerows(rows)

means=[];pairs=[]
for candidate in ['D5-S25','C2-S14']:
    for policy in policies:
        for phase in range(7):
            group=[v for k,v in observations.items() if k[0]==candidate and k[1]==policy and k[3]==phase]
            means.append(dict(candidate=candidate,policy=policy,phase=phase,n=len(group),
                **{f:statistics.mean(v['values'][f] for v in group) if group else '' for f in fields}))
            if policy=='none':continue
            matched=[];different=0;unreached=0
            for seed in range(50000,50100):
                treatment=observations.get((candidate,policy,seed,phase))
                control=observations.get((candidate,'none',seed,phase))
                if not treatment or not control:unreached+=1;continue
                if treatment['window']!=control['window']:
                    different+=1
                    if phase:continue
                matched.append({f:treatment['values'][f]-control['values'][f] for f in fields})
            pairs.append(dict(candidate=candidate,policy=policy,phase=phase,n=len(matched),
                differentWindows=different,oneOrBothUnreached=unreached,
                **{f:statistics.mean(v[f] for v in matched) if matched else '' for f in fields}))
assert all(r['n']==100 for r in means if r['phase']==0)
assert all(r['n']==100 for r in pairs if r['phase']==0)
save('means.csv',means);save('paired-components.csv',pairs)
text = ['# Purchase screen score diagnostics','',
    'Gardeners only: D5/S25 and C2/S14; four policies; 100 assigned matched seeds per arm. Phase 0 uses all seeds and each run\'s actual observed duration. Phase 1-6 deltas include only matching observed start/end ticks; they are conditional diagnostics.', '',
    'Terms sum to APS/AHS within 1e-6 for every included whole-run and phase observation. Purchased ADD is excluded from RFS by the production score formula; N/A terms contribute zero, matching production. Interaction means predAVG for APS and huntAVG for AHS. These decompositions explain the arithmetic, not a causal mechanism or enjoyment.', '',
    '| Candidate | Policy | ΔAPS | ΔRFS | ΔpredAVG | Δstarvation term | Δcrowding term | ΔAHS | Different whole-run windows |',
    '|---|---|---:|---:|---:|---:|---:|---:|---:|']
for r in pairs:
    if r['phase']!=0:continue
    text.append('| '+' | '.join([r['candidate'],r['policy']]+[f"{r[f]:+.4f}" for f in ['APS','APS_RFS','APS_interaction','APS_starvation','APS_crowding','AHS']]+[str(r['differentWindows'])])+' |')
text += ['', 'Full component means, ecological counters and phase sample sizes are in means.csv and paired-components.csv. No purchased-entity lineage was tracked, so post-purchase population changes cannot identify which individual purchased Hares survived.', '',
         'Source: '+str(source)+'. Source raw hash was verified when preparing the confirmation; these are earlier screen observations, not confirmation results.']
(destination / 'report.md').write_text('\n'.join(text)+'\n',encoding='utf-8')
print('PASS: 800 whole-run records; component reconciliation and matched window assertions; '+str(destination))
