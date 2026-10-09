using System.Text.Json;
using System.Runtime.CompilerServices;

namespace CellSim.Workbench;

internal static class RunChecks
{
    internal static void Run(string snapshot, string resultPath)
    {
        var project = Tools.FindProject(AppContext.BaseDirectory)!;
        var checks = new List<string>();
        var roots = new List<string>();
        void Require(bool value, [CallerArgumentExpression(nameof(value))] string? expression = null) { if (!value) throw new Exception("Run self-check failed: " + expression); }
        void Check(string name, Action action) { try { action(); checks.Add(name); } catch (Exception error) { throw new Exception(name, error); } }
        void Reject(Action action)
        {
            try { action(); } catch (Exception error) when (error is ArgumentException or IOException or InvalidDataException) { return; }
            throw new Exception("Invalid setup was accepted.");
        }
        var options = new SetupOptions
        {
            Name = "Workbench tooling check", Owner = "Bevin / tooling validation", Question = "Does the UI preserve the requested experiment and execute/export its exact seed coverage?",
            Hypothesis = "The compiler and pinned runner produce the requested condition/seed matrix.", SuccessCriteria = "Exact coverage, reconciled output, safe stop/resume and immutable inputs.", FailureCriteria = "Changed inputs accepted, missing/duplicate seeds, unrelated work stopped or missing workbook.",
            Snapshot = Path.GetFullPath(snapshot), SeedStart = 80000, SeedCount = 2, Width = 16, Height = 12, Plants = 65, Hares = 12, Foxes = 4, PhaseTicks = 3, Workers = 2, ChunkSize = 1,
            Strategies = ["skip-all", "gardeners"], Purchases = ["none", "each-one"]
        };
        var snapshotHash = RunTools.Hash(options.Snapshot);
        PreparedRun Prepare(SetupOptions setup)
        {
            var result = RunTools.Prepare(project, setup, _ => { }).GetAwaiter().GetResult(); roots.Add(result.Root); return result;
        }
        int Execute(PreparedRun prepared, bool resume)
        {
            using var session = new RunSession(prepared, resume); return session.Completion(_ => { }).WaitAsync(TimeSpan.FromSeconds(60)).GetAwaiter().GetResult();
        }
        Check("Preview counts and invalid setup limits", () =>
        {
            Require(RunTools.RunCount(options) == 8); RunTools.Validate(options);
            Reject(() => RunTools.Validate(options with { SeedStart = int.MaxValue }));
            Reject(() => RunTools.Validate(options with { Workers = 65 }));
            Reject(() => RunTools.Validate(options with { SeedCount = 1000000 }));
            Reject(() => RunTools.Validate(options with { Question = "" }));
            Reject(() => RunTools.Validate(options with { Plants = 100000 }));
            Reject(() => RunTools.Validate(options with { Strategies = ["made-up"] }));
        });
        Check("Preset round trip preserves readable experiment settings", () =>
        {
            var path = Path.Combine(Path.GetDirectoryName(resultPath)!, "preset-check.json");
            RunTools.SavePreset(path, options);
            Require(JsonSerializer.Serialize(RunTools.LoadPreset(path)) == JsonSerializer.Serialize(options));
        });
        var comparisonOptions = options with { Name = "Workbench comparison wiring check", SeedCount = 1,
            Comparisons = [new("hare-population", "8, 12"), new("fox:energy.starting", "120, 160")] };
        Check("Comparison values validate numbers, duplicates, bounds and total budget", () =>
        {
            Require(RunTools.VariantCount(comparisonOptions) == 4 && RunTools.RunCount(comparisonOptions) == 16);
            RunTools.Validate(comparisonOptions);
            foreach (var axis in new[] { new ComparisonAxis("hare-population", "1.5"), new("hare-population", "0"), new("hare-population", "8,8"), new("plant:resource.wilt-chance", "1.1"), new("map-width", "nope"), new("missing-key", "1") })
                Reject(() => RunTools.Validate(options with { Comparisons = [axis] }));
            Reject(() => RunTools.Validate(options with { Comparisons = [new("hare-population", "1"), new("hare-population", "2")] }));
            Reject(() => RunTools.Validate(options with { Comparisons = [new("map-width", "1,16")] }));
            Reject(() => RunTools.Validate(comparisonOptions with { SeedCount = 1000000 }));
            var path = Path.Combine(Path.GetDirectoryName(resultPath)!, "comparison-preset-check.json");
            RunTools.SavePreset(path, comparisonOptions);
            Require(JsonSerializer.Serialize(RunTools.LoadPreset(path)) == JsonSerializer.Serialize(comparisonOptions));
        });
        var compared = Prepare(comparisonOptions);
        Check("Every offered comparison control passes pinned-runner preflight", () =>
        {
            using var source = Tools.ReadSmall(options.Snapshot);
            foreach (var group in SetupParameters.All.Chunk(8))
            {
                var axes = group.Select(p =>
                {
                    var data = source.RootElement;
                    if (p.Argument is "-gridWidth" or "-gridHeight") return new ComparisonAxis(p.Key, data.GetProperty(p.SnapshotField).GetRawText());
                    var species = data.GetProperty("species").EnumerateArray().Single(s => s.GetProperty("id").GetString() == p.Component.Split(':')[0]);
                    var value = p.Argument == "-startingPopulations" ? species.GetProperty("population") : species.GetProperty("rules").GetProperty(p.SnapshotField);
                    return new ComparisonAxis(p.Key, value.GetRawText());
                }).ToArray();
                Prepare(options with { Name = "Workbench parameter catalogue preflight", Strategies = ["skip-all"], Purchases = ["none"], SeedCount = 1, Comparisons = axes });
            }
        });
        Check("Comparison compilation crosses axes and replaces overlapping fixed overrides", () =>
        {
            using var plan = Tools.ReadSmall(compared.Plan);
            var conditions = plan.RootElement.GetProperty("conditions").EnumerateArray().ToArray();
            Require(conditions.Length == 16 && !Directory.Exists(compared.Batch));
            var values = conditions.Select(c => c.GetProperty("arguments").EnumerateArray().Select(a => a.GetString()!).ToArray()).ToArray();
            Require(values.All(a => a[a.ToList().IndexOf("-startingPopulations") + 1].Contains("plant=65")));
            Require(values.Count(a => a[a.ToList().IndexOf("-startingPopulations") + 1].Contains("hare=8")) == 8);
            Require(values.Count(a => a[a.ToList().IndexOf("-speciesStats") + 1] == "fox:energy.starting=160") == 8);
            // Exercise the quick-override overlap without launching a second batch.
            using var spec = JsonDocument.Parse(JsonSerializer.Serialize(RunTools.Specification(comparisonOptions with { OverrideStats = true })));
            var baseline = spec.RootElement.GetProperty("baseArguments").EnumerateArray().Select(a => a.GetString()).ToArray();
            Require(baseline[Array.IndexOf(baseline, "-speciesStats") + 1] == "hare:awareness.vision-range=9");
        });
        Check("Comparison runner executes exact per-condition populations and seed coverage", () =>
        {
            Require(Execute(compared, false) == 0);
            var expected = new HashSet<int>(); var found = 0;
            foreach (var line in File.ReadLines(Path.Combine(compared.Batch, "runs.jsonl")))
            {
                using var row = JsonDocument.Parse(line); var result = row.RootElement.GetProperty("run");
                Require(result.GetProperty("seed").GetInt32() == 80000);
                expected.Add(result.GetProperty("populationHistory")[0].GetProperty("species").EnumerateArray().Single(s => s.GetProperty("speciesId").GetString() == "hare").GetProperty("population").GetInt32()); found++;
            }
            Require(found == 16 && expected.SetEquals([8,12]));
        });
        var prepared = Prepare(options);
        Check("Validation freezes a new folder without running workers", () =>
        {
            Require(!Directory.Exists(prepared.Batch));
            Require(RunTools.LoadRun(prepared.Root).Files.Count == 8);
            using var sweep = Tools.ReadSmall(Path.Combine(prepared.Root, "sweep", "sweep.json"));
            Require(sweep.RootElement.GetProperty("runs").GetInt32() == 8 && sweep.RootElement.GetProperty("conditions").GetInt32() == 4);
        });
        Check("Pinned runner produces exact condition and seed coverage", () =>
        {
            Require(Execute(prepared, false) == 0);
            var rows = File.ReadLines(Path.Combine(prepared.Batch, "runs.jsonl")).Select(line => JsonDocument.Parse(line)).ToArray();
            try
            {
                Require(rows.Length == 8);
                Require(rows.GroupBy(row => row.RootElement.GetProperty("condition").GetString()).All(group => group.Select(row => row.RootElement.GetProperty("run").GetProperty("seed").GetInt32()).SequenceEqual([80000, 80001])));
            }
            finally { foreach (var row in rows) row.Dispose(); }
        });
        Check("Completed resume revalidates without changing output", () =>
        {
            var path = Path.Combine(prepared.Batch, "runs.jsonl"); var hash = RunTools.Hash(path);
            Require(Execute(prepared, true) == 0 && RunTools.Hash(path) == hash);
        });
        Check("Changed frozen inputs refuse resume", () =>
        {
            var path = Path.Combine(prepared.Root, "snapshot.json"); var bytes = File.ReadAllBytes(path);
            try { File.AppendAllText(path, " "); Reject(() => RunTools.LoadRun(prepared.Root)); }
            finally { File.WriteAllBytes(path, bytes); }
        });
        var stopped = Prepare(options with { Name = "Workbench stop resume check", Strategies = ["gardeners"], Purchases = ["none"], SeedCount = 16, PhaseTicks = 20 });
        Check("Stop targets its own coordinator and retains completed chunks", () =>
        {
            File.WriteAllText(Path.Combine(stopped.Root, "stop-unrelated.request"), "Unrelated marker must not stop this coordinator");
            using var session = new RunSession(stopped, false);
            var completion = session.Completion(_ => { });
            var deadline = DateTime.UtcNow.AddSeconds(20);
            while (DateTime.UtcNow < deadline && !completion.IsCompleted && (!Directory.Exists(stopped.Batch) || !Directory.EnumerateFiles(stopped.Batch, "completion.json", SearchOption.AllDirectories).Any())) Thread.Sleep(10);
            Require(!completion.IsCompleted);
            session.RequestStop();
            Require(completion.WaitAsync(TimeSpan.FromSeconds(30)).GetAwaiter().GetResult() == 130);
            Require(Directory.EnumerateFiles(stopped.Batch, "completion.json", SearchOption.AllDirectories).Any());
            using var state = Tools.ReadSmall(Path.Combine(stopped.Batch, "status.json"));
            Require(state.RootElement.GetProperty("state").GetString() == "Cancelled");
            Require(!File.Exists(Path.Combine(stopped.Batch, "summary.json")));
            foreach (var workerClaim in Directory.EnumerateFiles(stopped.Batch, ".worker.lock", SearchOption.AllDirectories))
            { using var released = new FileStream(workerClaim, FileMode.Open, FileAccess.ReadWrite, FileShare.None); }
        });
        Check("Resume reuses completed chunk bytes and finishes missing seeds", () =>
        {
            var completed = Directory.EnumerateFiles(stopped.Batch, "completion.json", SearchOption.AllDirectories).ToDictionary(path => path, RunTools.Hash);
            Require(Execute(stopped, true) == 0);
            Require(completed.All(pair => RunTools.Hash(pair.Key) == pair.Value));
            var seeds = File.ReadLines(Path.Combine(stopped.Batch, "runs.jsonl")).Select(line => { using var row = JsonDocument.Parse(line); return row.RootElement.GetProperty("run").GetProperty("seed").GetInt32(); }).ToArray();
            Require(seeds.SequenceEqual(Enumerable.Range(options.SeedStart, 16)));
            using var claim = new FileStream(Path.Combine(stopped.Batch, ".controller.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        });
        Check("Native setup preview, validation gate and deliberate Run button", () =>
        {
            using var form = new WorkbenchForm(project) { Opacity = 0, ShowInTaskbar = false };
            Control.CheckForIllegalCrossThreadCalls = true;
            form.Show(); Checks.PumpUi();
            var page = Find<RunPage>(form).Single();
            page.Apply(comparisonOptions); Checks.PumpUi();
            var matrix = Find<DataGridView>(page).Single(g => g.AccessibleName == "Live test matrix");
            Require(matrix.RowCount == 2 && matrix.ColumnCount == 3 && Convert.ToString(matrix[1,0].Value) == "4 runs");
            Require(Find<DataGridView>(page).Single(g => g.AccessibleName == "Condition preview").RowCount == 16);
            Find<CheckBox>(page).Single(c => c.AccessibleName == "warren").Checked = true;
            Require(matrix.RowCount == 3 && RunTools.RunCount(page.Options()) == 24);
            Find<CheckBox>(page).Single(c => c.AccessibleName == "warren").Checked = false;
            var sections = Find<TabControl>(page).Single(t => t.AccessibleName == "Setup sections");
            foreach (var section in new[] { 0, 1, 2, 3 })
            {
                sections.SelectedIndex = section; Checks.PumpUi();
                using var picture = new Bitmap(form.Width, form.Height); form.DrawToBitmap(picture, new Rectangle(Point.Empty, form.Size));
                picture.Save(Path.Combine(Path.GetDirectoryName(resultPath)!, $"workbench-matrix-section-{section}.png"));
            }
            var comparisonGrid = Find<DataGridView>(page).Single(g => g.AccessibleName == "Comparison values");
            comparisonGrid[1,0].Value = "bad value";
            Require(!Find<Button>(page).Single(b => b.Text == "Validate setup").Enabled);
            comparisonGrid[1,0].Value = "8,12";
            Require(Find<Button>(page).Single(b => b.Text == "Validate setup").Enabled);
            sections.SelectedIndex = 0;
            page.Apply(options with { Name = "Workbench native run check", SeedCount = 1, Strategies = ["skip-all"], Purchases = ["none"] });
            Checks.PumpUi();
            Require(!Find<Button>(page).Single(b => b.Text == "Run").Enabled);
            Find<Button>(page).Single(b => b.Text == "Validate setup").PerformClick(); WaitPage(page);
            var run = Find<Button>(page).Single(b => b.Text == "Run");
            if (!run.Enabled)
            {
                var frozen = (PreparedRun?)typeof(RunPage).GetField("prepared", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(page);
                throw new Exception($"Native validation did not enable Run. Busy={page.IsBusy}; prepared={frozen?.Root}; batchExists={frozen != null && File.Exists(Path.Combine(frozen.Batch, "batch.json"))}; parentEnabled={run.Parent!.Enabled}; pageEnabled={page.Enabled}: " + string.Join(" | ", Find<Label>(page).Select(label => label.Text)) + " LOG: " + Find<TextBox>(page).Single(control => control.AccessibleName == "Simulation activity log").Text);
            }
            Find<NumericUpDown>(page).Single(n => n.AccessibleName == "Seeds per condition").Value = 2;
            Require(!run.Enabled);
            Find<Button>(page).Single(b => b.Text == "Validate setup").PerformClick(); WaitPage(page); Require(run.Enabled);
            run.PerformClick(); WaitPage(page);
            Require(Find<Button>(page).Single(b => b.Text == "Export this run").Enabled);
            Find<Button>(page).Single(b => b.Text == "Export this run").PerformClick();
            Require(Find<TabControl>(form).Single(t => t.AccessibleName == "Workbench pages").SelectedIndex == 1);
            Find<CheckBox>(form).Single(c => c.Text == "Open workbook when ready").Checked = false;
            var export = Find<Button>(form).Single(b => b.Text == "Create Excel worksheet"); export.PerformClick();
            var deadline = DateTime.UtcNow.AddSeconds(60);
            while (!export.Enabled && DateTime.UtcNow < deadline) { Checks.PumpUi(); Thread.Sleep(10); }
            Require(export.Enabled && Find<Button>(form).Single(b => b.Text == "Open workbook").Enabled);
            Find<TabControl>(form).Single(t => t.AccessibleName == "Workbench pages").SelectedIndex = 0;
            using var image = new Bitmap(form.Width, form.Height); form.DrawToBitmap(image, new Rectangle(Point.Empty, form.Size));
            image.Save(Path.Combine(Path.GetDirectoryName(resultPath)!, "workbench-setup.png"));
        });
        Require(snapshotHash == RunTools.Hash(options.Snapshot));
        File.WriteAllText(resultPath, JsonSerializer.Serialize(new { status = "Passed", checks, fixtureRuns = 42, experimentFolders = roots, sourceSnapshotUnchanged = true }, new JsonSerializerOptions { WriteIndented = true }));
    }
    static void WaitPage(RunPage page)
    {
        var deadline = DateTime.UtcNow.AddSeconds(60);
        while (page.IsBusy && DateTime.UtcNow < deadline) { Checks.PumpUi(); Thread.Sleep(10); }
        Checks.PumpUi();
        if (page.IsBusy) { page.RequestStop(); throw new TimeoutException("Native setup/run check timed out."); }
    }
    static IEnumerable<T> Find<T>(Control root) where T : Control
    {
        foreach (Control child in root.Controls) { if (child is T value) yield return value; foreach (var nested in Find<T>(child)) yield return nested; }
    }
}
