"""Small fixture checks for truthful chunk progress and lifecycle states."""
import datetime as dt
import json
import os
import tempfile
from pathlib import Path
from dashboard import Progress


def write(path,value):
    path.parent.mkdir(parents=True,exist_ok=True)
    path.write_text(json.dumps(value))


with tempfile.TemporaryDirectory() as directory:
    root=Path(directory)
    case=dict(id='case',axes=dict(candidate='D5-S25',purchase='none'),path='gardeners')
    write(root/'sweep/sweep.json',dict(cases=[case],runs=75))
    write(root/'sweep/plan.json',dict(seedStart=60000,seedCount=75,chunkSize=50,workers=16))
    write(root/'sweep/batch/batch.json',dict(identity='test'))
    write(root/'sweep/batch/status.json',dict(state='Running',activeWorkers=1,elapsedSeconds=10))
    write(root/'pipeline-state.json',dict(state='Running'))
    write(root/'launch.json',dict(startedUtc=dt.datetime.now(dt.timezone.utc).isoformat()))
    (root/'stderr.log').touch()
    chunks=root/'sweep/batch/chunks'
    chunks.mkdir()
    progress=Progress(root)
    initial=progress.snapshot()
    assert initial['completed']==0 and initial['etaSeconds'] is None and initial['totalChunks']==2
    completed=chunks/'case-0000000000/completion.json'
    write(completed,dict(identity='test',condition='case',seedStart=60000,seedCount=50))
    active=chunks/'case-0000000050'
    write(active/'job.json',dict(identity='test',condition='case',seedStart=60050,seedCount=25))
    write(active/'attempt-0001/status.json',dict(state='Running',pid=123,
        startedUtc=dt.datetime.now(dt.timezone.utc).isoformat()))
    mixed=progress.snapshot()
    assert mixed['completed']==50 and mixed['cells'][0]['active']==1 and len(mixed['workers'])==1
    assert mixed['workers'][0]['seedCount']==25 and mixed['etaSeconds']==5
    old=dt.datetime.now().timestamp()-60
    os.utime(root/'sweep/batch/status.json',(old,old))
    assert progress.snapshot()['stale']
    write(active/'attempt-0001/status.json',dict(state='Failed',error='fixture failure'))
    assert len(progress.snapshot()['errors'])==1 and not progress.snapshot()['workers']
    write(active/'completion.json',dict(identity='test',condition='case',seedStart=60050,seedCount=25))
    write(root/'sweep/batch/status.json',dict(state='Completed'))
    write(root/'sweep/batch/summary.json',dict(elapsedSeconds=20))
    write(root/'pipeline-state.json',dict(state='Analyzing'))
    finished=progress.snapshot()
    assert finished['completed']==75 and finished['simulationDone'] and finished['state']=='Analyzing'
    assert not finished['stale'] and finished['etaSeconds']==0 and finished['elapsedSeconds']==20
    completed.unlink()
    assert progress.snapshot()['completed']==25 and progress.snapshot()['completedChunks']==1
    write(completed,dict(identity='foreign',condition='case',seedStart=60000,seedCount=50))
    try:
        progress.snapshot()
    except AssertionError:
        pass
    else:
        raise AssertionError('Foreign completion accepted')
print('PASS: pending/active/validated/tail chunks, ETA, stale status, worker failure, analysis lifecycle and foreign identity rejection.')
