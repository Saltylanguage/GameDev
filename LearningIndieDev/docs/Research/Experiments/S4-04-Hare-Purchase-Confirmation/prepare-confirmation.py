"""Freeze the approved confirmation from the validated purchase screen, without rebuilding."""
import hashlib
import json
import shutil
import subprocess
import sys
from pathlib import Path

project = Path(__file__).resolve().parents[4]
source = project / 'artifacts/cellsim-hare-purchase-20261008-132317'
root = Path(sys.argv[1]).resolve()
assert root.parent == project / 'artifacts', 'Use a new project artifact directory'
root.mkdir(exist_ok=False)

def load(path):
    return json.loads(path.read_text(encoding='utf-8-sig'))

def digest(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()

def run(*args):
    result = subprocess.run(list(map(str, args)), capture_output=True, text=True)
    if result.returncode:
        raise RuntimeError(result.stdout + result.stderr)
    return result.stdout

assert load(source / 'pipeline-state.json')['state'] == 'Completed'
assert load(source / 'validation/checks.json')['state'] == 'Passed'
summary = load(source / 'sweep/batch/summary.json')
assert summary['state'] == 'Completed' and summary['runs'] == 12800
assert digest(source / 'sweep/batch/runs.jsonl') == summary['outputHash']
shutil.copy2(source / 'snapshot.json', root / 'snapshot.json')
shutil.copytree(source / 'tool/bin', root / 'tool/bin')
shutil.copy2(source / 'tool/sweep.py', root / 'tool/sweep.py')
shutil.copy2(project / 'tools/CellSim.Batch/purchase_report.py', root / 'tool/purchase_report.py')
spec = load(source / 'spec.json')
policies = ['none', 'late-five', 'each-one', 'each-three', 'each-five', 'restore-toward-start']
purchase = next(a for a in spec['axes'] if a['id'] == 'purchase')
purchase['values'] = [v for v in purchase['values'] if v['id'] in policies]
assert [v['id'] for v in purchase['values']] == policies
spec.update(seedStart=60000, seedCount=1000, workers=16, chunkSize=50, maxRuns=96000)
(root / 'spec.json').write_text(json.dumps(spec, indent=2) + '\n', encoding='utf-8')
compile_output = run(sys.executable, root / 'tool/sweep.py', 'compile', root / 'spec.json', root / 'sweep')
matrix = load(root / 'sweep/sweep.json')
old_cases = {c['id']:c for c in load(source / 'sweep/sweep.json')['cases']}
assert matrix['runs'] == 96000 and matrix['conditions'] == 96
assert matrix['seedStart'] == 60000 and matrix['seedCount'] == 1000
for case in matrix['cases']:
    old = old_cases[case['id']]
    assert case['arguments'] == old['arguments'], 'Scientific arguments drifted'
    assert case['axes'] == old['axes'] and case['path'] == old['path']
assert digest(root / 'snapshot.json') == digest(source / 'snapshot.json')
binary = Path('tool/bin/Release/net8.0/CellSim.Batch.dll')
old_manifest = load(source / 'sweep/batch/batch.json')
assert digest(root / binary) == old_manifest['assemblyHash']
for old in (source / 'tool/bin').rglob('*'):
    if old.is_file():
        assert digest(old) == digest(root / old.relative_to(source))
self_test = run('dotnet', root / binary, 'self-test')
dry_run = run('dotnet', root / binary, 'batch', root / 'sweep/plan.json', '--dry-run')
validation = root / 'validation'
validation.mkdir()
shutil.copy2(source / 'validation/EditMode-results.xml', validation / 'EditMode-results.xml')
frozen = [root / 'snapshot.json', root / 'spec.json', root / 'sweep/plan.json',
          root / 'sweep/sweep.json', root / 'tool/sweep.py', root / 'tool/purchase_report.py']
frozen += [p for p in (root / 'tool/bin').rglob('*') if p.is_file()]
checks = dict(state='Passed', runs=96000, conditions=96, seedStart=60000, seedCount=1000,
    workers=16, sourceScreen=str(source), sourceBatchIdentity=old_manifest['identity'],
    reusedValidation='Eight Unity references, sixteen historical controls, 128-arm smoke and two EditMode tests are retained evidence for the identical frozen simulation build, not rerun tests.',
    hashes={str(p.relative_to(root)):digest(p) for p in frozen},
    evidence=[compile_output, self_test, dry_run, 'All 96 case arguments match the screen; snapshot and all runner files are byte-identical.'])
(validation / 'checks.json').write_text(json.dumps(checks, indent=2) + '\n', encoding='utf-8')
launcher = (project / 'docs/Research/Experiments/S4-03-Hare-Reinforcement-Candidate-Panel/run-panel.ps1').read_text(encoding='utf-8-sig')
for old, new in [('runs=12800','runs=96000'), ('seedStart=50000','seedStart=60000'),
                 ('seedCount=100 }','seedCount=1000 }'), ('-ne 50000','-ne 60000'),
                 ('seedCount -ne 100 -or','seedCount -ne 1000 -or'), ('conditions.Count -ne 128','conditions.Count -ne 96')]:
    assert old in launcher
    launcher = launcher.replace(old,new)
guard = '''    foreach ($entry in $checks.hashes.PSObject.Properties) {
        $actual = (Get-FileHash -LiteralPath (Join-Path $runDirectory $entry.Name) -Algorithm SHA256).Hash.ToLowerInvariant()
        if ($actual -ne $entry.Value) { throw "Frozen input changed: $($entry.Name)" }
    }
'''
launcher = launcher.replace("    Set-PipelineState 'Running'", guard + "    Set-PipelineState 'Running'")
(root / 'run.ps1').write_text(launcher, encoding='utf-8')
print(json.dumps(dict(root=str(root), checks=checks['state'], dryRun=dry_run), indent=2))
