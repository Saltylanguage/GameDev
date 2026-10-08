"""Read-only localhost progress dashboard; never reads raw simulation reports."""
import argparse
import datetime as dt
import json
import math
import os
import threading
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.parse import urlsplit


def load(path):
    return json.loads(path.read_text(encoding='utf-8-sig'))


class Progress:
    def __init__(self, root):
        self.root = Path(root).resolve()
        self.matrix = load(self.root / 'sweep/sweep.json')
        self.plan = load(self.root / 'sweep/plan.json')
        self.identity = load(self.root / 'sweep/batch/batch.json')['identity']
        self.cases = {c['id']:c for c in self.matrix['cases']}
        self.completed = {}
        self.lock = threading.Lock()

    def snapshot(self):
        with self.lock:
            now = dt.datetime.now(dt.timezone.utc)
            batch = self.root / 'sweep/batch'
            status_path = batch / 'status.json'
            status = load(status_path)
            pipeline = load(self.root / 'pipeline-state.json')
            cells = {key:dict(id=key, candidate=c['axes']['candidate'], path=c['path'],
                        policy=c['axes']['purchase'], completed=0, active=0, total=self.plan['seedCount'])
                     for key,c in self.cases.items()}
            workers, errors, completed_chunks = [], [], 0
            for folder in (batch / 'chunks').iterdir():
                if not folder.is_dir():
                    continue
                completion = folder / 'completion.json'
                if completion.exists():
                    stamp = completion.stat().st_mtime_ns
                    cached = self.completed.get(folder.name)
                    done = cached[1] if cached and cached[0] == stamp else load(completion)
                    assert done['identity'] == self.identity and done['condition'] in cells, 'Foreign completion'
                    offset = done['seedStart'] - self.plan['seedStart']
                    assert 0 <= offset < self.plan['seedCount'] and offset % self.plan['chunkSize'] == 0
                    assert done['seedCount'] == min(self.plan['chunkSize'],self.plan['seedCount']-offset)
                    assert folder.name == done['condition'] + '-' + f'{offset:010d}'
                    self.completed[folder.name] = (stamp,done)
                    cells[done['condition']]['completed'] += done['seedCount']
                    completed_chunks += 1
                    continue
                attempts = sorted(folder.glob('attempt-*/status.json'))
                if not attempts:
                    continue
                worker = load(attempts[-1])
                job = load(folder / 'job.json')
                assert job['identity'] == self.identity and job['condition'] in cells
                cell = cells[job['condition']]
                if worker['state'] == 'Failed':
                    errors.append(dict(chunk=folder.name,error=worker.get('error','Worker failed')))
                elif worker['state'] == 'Running':
                    cell['active'] += 1
                    workers.append(dict(pid=worker['pid'],candidate=cell['candidate'],path=cell['path'],
                        policy=cell['policy'],seedStart=job['seedStart'],seedCount=job['seedCount'],
                        seconds=max(0,(now-dt.datetime.fromisoformat(worker['startedUtc'].replace('Z','+00:00'))).total_seconds())))
            completed = sum(c['completed'] for c in cells.values())
            assert all(0 <= c['completed'] <= c['total'] for c in cells.values())
            summary_path = batch / 'summary.json'
            summary = load(summary_path) if summary_path.exists() else {}
            launch = load(self.root / 'launch.json')
            elapsed = summary.get('elapsedSeconds',status.get('elapsedSeconds'))
            if elapsed is None:
                elapsed = (now-dt.datetime.fromisoformat(launch['startedUtc'].replace('Z','+00:00'))).total_seconds()
            rate = completed / elapsed if elapsed > 0 else 0
            eta = (self.matrix['runs']-completed)/rate if rate else None
            age = max(0,now.timestamp()-status_path.stat().st_mtime)
            simulation_done = status['state'] == 'Completed'
            return dict(run=self.root.name, updatedUtc=now.isoformat(), state=pipeline['state'],
                message=pipeline.get('message',''),batchState=status['state'], completed=completed,
                total=self.matrix['runs'],completedChunks=completed_chunks,
                totalChunks=len(self.cases)*math.ceil(self.plan['seedCount']/self.plan['chunkSize']),
                workersConfigured=self.plan['workers'],workersReported=status.get('activeWorkers',0 if simulation_done else None),
                elapsedSeconds=elapsed, rate=rate, etaSeconds=eta, simulationDone=simulation_done,
                statusAgeSeconds=age, stale=status['state']=='Running' and age>30,
                seedStart=self.plan['seedStart'],seedCount=self.plan['seedCount'],
                stderrBytes=(self.root/'stderr.log').stat().st_size,
                cells=list(cells.values()),workers=sorted(workers,key=lambda w:-w['seconds']),errors=errors)


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('root',type=Path)
    parser.add_argument('--port',type=int,default=0)
    args=parser.parse_args()
    progress=Progress(args.root)
    html=Path(__file__).with_name('dashboard.html').read_bytes()

    class Handler(BaseHTTPRequestHandler):
        def do_GET(self):
            route=urlsplit(self.path).path
            if route not in ['/', '/status']:
                self.send_error(404);return
            try:
                body=html if route=='/' else json.dumps(progress.snapshot(),allow_nan=False).encode()
                self.send_response(200)
                self.send_header('Content-Type','text/html; charset=utf-8' if route=='/' else 'application/json')
                self.send_header('Cache-Control','no-store')
                self.send_header('Content-Length',str(len(body)))
                self.end_headers();self.wfile.write(body)
            except (OSError,ValueError,AssertionError,KeyError) as error:
                self.send_error(503,str(error))

        def log_message(self,*args):
            pass

    server=ThreadingHTTPServer(('127.0.0.1',args.port),Handler)
    url=f'http://127.0.0.1:{server.server_port}/'
    (progress.root/'dashboard-server.json').write_text(json.dumps(dict(url=url,pid=os.getpid()),indent=2)+'\n')
    print(url,flush=True)
    server.serve_forever()


if __name__=='__main__':
    main()
