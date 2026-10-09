using System.Text.Json;
using static CellSim.Workbench.WorkbenchForm;

namespace CellSim.Workbench;

internal sealed partial class RunPage : TabPage
{
    readonly string project;
    readonly Func<bool> canStart;
    readonly Action<string> monitor;
    readonly Action<string> exportSweep;
    readonly TextBox name = Input("Experiment name", "Forest Edge review");
    readonly TextBox owner = Input("Experiment owner", Environment.UserName);
    readonly TextBox question = Input("Question");
    readonly TextBox hypothesis = Input("Expectation");
    readonly TextBox success = Input("Success criteria");
    readonly TextBox failure = Input("Failure criteria");
    readonly TextBox snapshot = Input("Frozen snapshot");
    readonly NumericUpDown seedStart = Number(70000, 0, int.MaxValue, "First seed");
    readonly NumericUpDown seedCount = Number(4, 1, 1000000, "Seeds per condition");
    readonly NumericUpDown workers = Number(2, 1, 64, "Worker processes");
    readonly NumericUpDown chunks = Number(2, 1, 1000000, "Seeds per chunk");
    readonly NumericUpDown timeout = Number(3600, 1, 86400, "Chunk timeout seconds");
    readonly NumericUpDown phase = Number(100, 1, 166666, "Ticks per round");
    readonly NumericUpDown width = Number(36, 1, 4096, "Grid width");
    readonly NumericUpDown height = Number(20, 1, 4096, "Grid height");
    readonly NumericUpDown plants = Number(400, 0, int.MaxValue, "Starting Plants");
    readonly NumericUpDown hares = Number(55, 1, int.MaxValue, "Starting Hares");
    readonly NumericUpDown foxes = Number(35, 0, int.MaxValue, "Starting Foxes");
    readonly NumericUpDown vision = Number(9, 0, 4096, "Hare vision");
    readonly NumericUpDown energy = Number(160, 0, int.MaxValue, "Fox starting energy");
    readonly CheckBox wrap = new() { Text = "Wrap map edges", Checked = true, AutoSize = true };
    readonly CheckBox statOverrides = new() { Text = "Override Hare vision / Fox starting energy", AutoSize = true };
    readonly Dictionary<string, CheckBox> strategies = ChoiceChecks(RunTools.StrategyIds);
    readonly Dictionary<string, CheckBox> purchases = ChoiceChecks(RunTools.PurchaseIds);
    readonly Button validate = Button("Validate setup");
    readonly Button run = Button("Run");
    readonly Button stop = Button("Stop safely");
    readonly Button resume = Button("Resume / recheck");
    readonly Button export = Button("Export this run");
    readonly Label preview = TextLabel("");
    readonly Label status = TextLabel("Choose a snapshot or load a preset, then validate before running.");
    readonly TextBox log = new() { Dock = DockStyle.Fill, ReadOnly = true, Multiline = true, ScrollBars = ScrollBars.Vertical, AccessibleName = "Simulation activity log" };
    readonly Panel settings = new() { Dock = DockStyle.Fill, AutoScroll = true };
    readonly System.Windows.Forms.Timer timer = new() { Interval = 1000 };
    PreparedRun? prepared;
    RunSession? session;
    string? signature;
    string? logFile;
    bool busy;
    internal bool IsBusy => busy;
    internal event Action? BecameIdle;

    internal RunPage(string project, Func<bool> canStart, Action<string> monitor, Action<string> exportSweep) : base("Set up & run")
    {
        this.project = project; this.canStart = canStart; this.monitor = monitor; this.exportSweep = exportSweep;
        Padding = new(12); BackColor = Color.White;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5 };
        layout.ColumnStyles.Add(new(SizeType.Percent, 100));
        layout.SizeChanged += (_, _) => { preview.MaximumSize = status.MaximumSize = new(Math.Max(200, layout.Width - 20), 0); };
        layout.RowStyles.Add(new(SizeType.Percent, 100));
        layout.RowStyles.Add(new(SizeType.AutoSize)); layout.RowStyles.Add(new(SizeType.AutoSize)); layout.RowStyles.Add(new(SizeType.AutoSize));
        layout.RowStyles.Add(new(SizeType.Absolute, 55));
        BuildSetup();
        layout.Controls.Add(settings, 0, 0); layout.Controls.Add(preview, 0, 1); layout.Controls.Add(status, 0, 2);
        layout.Controls.Add(Flow(validate, run, stop, resume, export), 0, 3); layout.Controls.Add(log, 0, 4);
        Controls.Add(layout);
        foreach (var input in Descendants(settings))
        {
            if (input is TextBox text && !text.ReadOnly) text.TextChanged += (_, _) => Changed();
            if (input is NumericUpDown number) number.ValueChanged += (_, _) => Changed();
            if (input is CheckBox check) check.CheckedChanged += (_, _) => Changed();
        }
        validate.Click += async (_, _) => await Prepare();
        run.Click += async (_, _) => await Execute(false);
        resume.Click += async (_, _) => await Execute(true);
        stop.Click += (_, _) => { try { RequestStop(); } catch (Exception error) { Error(error); } };
        export.Click += (_, _) => { if (prepared != null) exportSweep(Path.Combine(prepared.Root, "sweep", "sweep.json")); };
        timer.Tick += (_, _) => Progress(); timer.Start();
        Disposed += (_, _) => { timer.Dispose(); help.Dispose(); };
        Changed();
    }

    static TextBox Input(string label, string value = "") => new() { Text = value, Width = 360, AccessibleName = label };
    static FlowLayoutPanel Row(string label, params Control[] controls) => Flow(new[] { TextLabel(label + ":") }.Cast<Control>().Concat(controls).ToArray());
    static IEnumerable<Control> Descendants(Control root)
    {
        foreach (Control child in root.Controls) { yield return child; foreach (var descendant in Descendants(child)) yield return descendant; }
    }
    internal SetupOptions Options() => new()
    {
        Name = name.Text, Owner = owner.Text, Question = question.Text, Hypothesis = hypothesis.Text, SuccessCriteria = success.Text, FailureCriteria = failure.Text, Snapshot = snapshot.Text,
        SeedStart = (int)seedStart.Value, SeedCount = (int)seedCount.Value, Workers = (int)workers.Value, ChunkSize = (int)chunks.Value, TimeoutSeconds = (int)timeout.Value, PhaseTicks = (int)phase.Value,
        Width = (int)width.Value, Height = (int)height.Value, Plants = (int)plants.Value, Hares = (int)hares.Value, Foxes = (int)foxes.Value, Wrap = wrap.Checked,
        OverrideStats = statOverrides.Checked, HareVision = (int)vision.Value, FoxEnergy = (int)energy.Value,
        Strategies = strategies.Where(c => c.Value.Checked).Select(c => c.Key).ToArray(), Purchases = purchases.Where(c => c.Value.Checked).Select(c => c.Key).ToArray(),
        Comparisons = ReadComparisons()
    };
    internal void Apply(SetupOptions options)
    {
        applying = true;
        try
        {
        name.Text = options.Name; owner.Text = options.Owner; question.Text = options.Question; hypothesis.Text = options.Hypothesis; success.Text = options.SuccessCriteria; failure.Text = options.FailureCriteria; snapshot.Text = options.Snapshot;
        seedStart.Value = options.SeedStart; seedCount.Value = options.SeedCount; workers.Value = options.Workers; chunks.Value = options.ChunkSize; timeout.Value = options.TimeoutSeconds; phase.Value = options.PhaseTicks;
        width.Value = options.Width; height.Value = options.Height; plants.Value = options.Plants; hares.Value = options.Hares; foxes.Value = options.Foxes; wrap.Checked = options.Wrap;
        statOverrides.Checked = options.OverrideStats; vision.Value = options.HareVision; energy.Value = options.FoxEnergy;
        foreach (var (id, check) in strategies) check.Checked = options.Strategies.Contains(id);
        foreach (var (id, check) in purchases) check.Checked = options.Purchases.Contains(id);
        comparisonGrid.Rows.Clear();
        foreach (var axis in options.Comparisons) comparisonGrid.Rows.Add(axis.Key, axis.Values);
        }
        finally { applying = false; }
        RefreshSnapshotDetails();
        if (busy) RefreshMatrix(Options()); else Changed();
    }
    void Changed()
    {
        if (busy || applying) return;
        prepared = null; signature = null;
        var options = Options();
        RefreshMatrix(options);
        status.Text = "Setup changed. Validate to prepare a new run.";
        Buttons();
    }
    void Buttons()
    {
        settings.Enabled = !busy;
        validate.Enabled = !busy && matrixValid;
        run.Enabled = !busy && prepared != null && !File.Exists(Path.Combine(prepared.Batch, "batch.json"));
        run.BackColor = run.Enabled ? Color.FromArgb(47, 100, 68) : Color.FromArgb(230, 235, 231);
        run.ForeColor = run.Enabled ? Color.White : Color.DimGray;
        resume.Enabled = !busy && prepared != null && File.Exists(Path.Combine(prepared.Batch, "batch.json"));
        stop.Enabled = busy && session != null;
        export.Enabled = !busy && prepared != null && File.Exists(Path.Combine(prepared.Batch, "summary.json"));
    }
    void Append(string text)
    {
        if (log.TextLength > 100000) log.Text = log.Text[^50000..];
        log.AppendText(text + Environment.NewLine); log.SelectionStart = log.TextLength; log.ScrollToCaret();
        if (logFile != null)
            try { File.AppendAllText(logFile, text + Environment.NewLine); }
            catch (IOException error) { logFile = null; status.Text = "Could not save the activity log: " + error.Message; }
    }
    void Error(Exception error) { status.Text = error.Message; Append(error.Message); }
    void Idle() { busy = false; Buttons(); BecameIdle?.Invoke(); }
    internal async Task Prepare()
    {
        if (busy) return;
        if (!canStart()) { status.Text = "Wait for the worksheet export to finish before validating."; return; }
        prepared = null; signature = null; logFile = null; log.Clear();
        try
        {
            var options = Options(); RunTools.Validate(options);
            busy = true; Buttons(); status.Text = "Validating and freezing inputs. No workers are launched.";
            prepared = await RunTools.Prepare(project, options, Append);
            signature = JsonSerializer.Serialize(options);
            status.Text = "Validated. Click Run to start; changes require validation again.";
        }
        catch (Exception error) { Error(error); }
        finally { Idle(); }
    }
    internal async Task Execute(bool isResume)
    {
        if (busy || prepared == null) return;
        if (!canStart()) { status.Text = "Wait for the worksheet export to finish before running."; return; }
        try
        {
            if (signature != JsonSerializer.Serialize(Options())) throw new InvalidOperationException("Settings changed. Validate again before Run.");
            prepared = RunTools.LoadRun(prepared.Root);
            busy = true; Buttons();
            logFile = Path.Combine(prepared.Root, "session-" + Guid.NewGuid().ToString("N") + ".log");
            session = new RunSession(prepared, isResume); Buttons();
            Append(isResume ? "Resuming/rechecking the frozen batch." : "Starting the validated batch.");
            status.Text = "Running. Stop safely retains completed chunks for resume.";
            monitor(prepared.Batch);
            var code = await session.Completion(Append);
            status.Text = code == 0 ? "Completed. Review progress or export this run." : code == 130 ? "Stopped. Completed chunks are retained; Resume continues this frozen run." : "Run stopped with an error. See the log; evidence is retained.";
        }
        catch (Exception error) { Error(error); }
        finally { if (session != null) Progress(); session?.Dispose(); session = null; logFile = null; Idle(); }
    }
    internal void RequestStop()
    {
        session?.RequestStop();
        if (session != null) status.Text = "Stop requested. Waiting for the coordinator to clean up its owned workers.";
    }
    void Progress()
    {
        if (!busy || prepared == null || session == null) return;
        try
        {
            var progress = Tools.ReadProgress(prepared.Batch, DateTime.UtcNow);
            preview.Text = $"{progress.State}: {progress.CompletedChunks:N0}/{progress.TotalChunks:N0} completed chunks · {progress.Workers} reported workers" + (progress.Stale ? " · status is stale" : "");
        }
        catch (Exception error) { preview.Text = "Waiting for progress metadata: " + error.Message; }
    }
    void ChooseSnapshot()
    {
        using var picker = new OpenFileDialog { Title = "Select a neutral exported scenario snapshot", Filter = "Frozen scenario (*.json)|*.json", InitialDirectory = Path.Combine(project, "artifacts") };
        if (picker.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            using var doc = Tools.ReadSmall(picker.FileName); var data = doc.RootElement;
            var values = data.GetProperty("species").EnumerateArray().ToDictionary(s => s.GetProperty("id").GetString()!, s => s.GetProperty("population").GetInt32());
            Apply(Options() with { Snapshot = picker.FileName, Width = data.GetProperty("width").GetInt32(), Height = data.GetProperty("height").GetInt32(), Wrap = data.GetProperty("wrapEdges").GetBoolean(), Plants = values["plant"], Hares = values["hare"], Foxes = values["fox"], OverrideStats = false, Comparisons = [] });
        }
        catch (Exception error) { Error(new InvalidDataException("Could not load the frozen Plant/Hare/Fox snapshot: " + error.Message)); }
    }
    void LoadPreset()
    {
        using var picker = new OpenFileDialog { Title = "Load a simulation preset", Filter = "Workbench preset (*.json)|*.json", InitialDirectory = Path.Combine(project, "tools", "CellSim.Workbench", "presets") };
        if (picker.ShowDialog(this) != DialogResult.OK) return;
        try { Apply(RunTools.LoadPreset(picker.FileName)); } catch (Exception error) { Error(error); }
    }
    void SavePreset()
    {
        using var picker = new SaveFileDialog { Title = "Save simulation settings as a preset", Filter = "Workbench preset (*.json)|*.json", DefaultExt = "json", FileName = "simulation-preset.json", InitialDirectory = Path.Combine(project, "artifacts") };
        if (picker.ShowDialog(this) != DialogResult.OK) return;
        try { RunTools.SavePreset(picker.FileName, Options()); status.Text = "Preset saved. It does not launch simulations."; } catch (Exception error) { Error(error); }
    }
    async Task OpenRun()
    {
        using var picker = new FolderBrowserDialog { Description = "Choose a Workbench run folder containing workbench-run.json", UseDescriptionForTitle = true, InitialDirectory = Path.Combine(project, "artifacts", "workbench-runs") };
        if (picker.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            var loaded = RunTools.LoadRun(picker.SelectedPath);
            busy = true; Apply(loaded.Settings); Buttons();
            if (await RunTools.Preflight(loaded, Append) != 0) throw new InvalidDataException("Frozen runner/input preflight failed; resume is refused. See the log.");
            prepared = loaded; signature = JsonSerializer.Serialize(Options());
            status.Text = "Frozen run loaded. Use Resume / recheck to continue without changing settings.";
            if (Directory.Exists(loaded.Batch)) monitor(loaded.Batch);
        }
        catch (Exception error) { Error(error); }
        finally { Idle(); }
    }
}
