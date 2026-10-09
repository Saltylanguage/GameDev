using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CellSim.Workbench;

internal sealed record SetupOptions
{
    public int SchemaVersion { get; init; } = 1;
    public string Name { get; init; } = "Forest Edge review";
    public string Owner { get; init; } = Environment.UserName;
    public string Question { get; init; } = "";
    public string Hypothesis { get; init; } = "";
    public string SuccessCriteria { get; init; } = "";
    public string FailureCriteria { get; init; } = "";
    public string Snapshot { get; init; } = "";
    public int SeedStart { get; init; } = 70000;
    public int SeedCount { get; init; } = 4;
    public int Workers { get; init; } = 2;
    public int ChunkSize { get; init; } = 2;
    public int TimeoutSeconds { get; init; } = 3600;
    public int PhaseTicks { get; init; } = 100;
    public int Width { get; init; } = 36;
    public int Height { get; init; } = 20;
    public int Plants { get; init; } = 400;
    public int Hares { get; init; } = 55;
    public int Foxes { get; init; } = 35;
    public bool Wrap { get; init; } = true;
    public bool OverrideStats { get; init; }
    public int HareVision { get; init; } = 9;
    public int FoxEnergy { get; init; } = 160;
    public string[] Strategies { get; init; } = ["skip-all"];
    public string[] Purchases { get; init; } = ["none"];
    public ComparisonAxis[] Comparisons { get; init; } = [];
}

internal sealed record PreparedRun(string Root, SetupOptions Settings, Dictionary<string, string> Files)
{
    internal string Plan => Path.Combine(Root, "sweep", "plan.json");
    internal string Runner => Path.Combine(Root, "runner", "CellSim.Batch.dll");
    internal string Batch => Path.Combine(Root, "sweep", "batch");
}

internal static class RunTools
{
    internal static readonly string[] StrategyIds = ["skip-all", "trailblazer", "warren", "gardeners"];
    internal static readonly string[] PurchaseIds = ["none", "late-five", "each-one", "each-three", "each-five", "restore-toward-start"];
    static readonly JsonSerializerOptions Json = new() { WriteIndented = true, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow };
    internal static long VariantCount(SetupOptions options)
    {
        if (options.Comparisons.Length > 8 || options.Comparisons.Select(a => a.Key).Distinct().Count() != options.Comparisons.Length)
            throw new ArgumentException("Use up to eight different comparison settings; combine values for the same setting into one row.");
        long count = 1;
        foreach (var axis in options.Comparisons)
        {
            count = checked(count * SetupParameters.Parse(axis).Length);
            if (count > 10000) throw new ArgumentException("Too many combinations. Reduce comparison values (at most 10,000 conditions).");
        }
        return count;
    }
    internal static long RunCount(SetupOptions options) => checked((long)options.SeedCount * options.Strategies.Length * options.Purchases.Length * VariantCount(options));
    internal static string Hash(string path) { using var input = File.OpenRead(path); return Convert.ToHexString(SHA256.HashData(input)).ToLowerInvariant(); }
    internal static void Validate(SetupOptions options)
    {
        if (options.SchemaVersion != 1 || new[] { options.Name, options.Owner, options.Question, options.Hypothesis, options.SuccessCriteria, options.FailureCriteria }.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Fill in the experiment name, owner, question, expectation, and success/failure criteria before validation.");
        if (!File.Exists(options.Snapshot)) throw new FileNotFoundException("Choose an existing frozen scenario snapshot. This is an exported snapshot, not a Unity .asset or report.json.");
        if (options.SeedStart < 0 || options.SeedCount < 1 || (long)options.SeedStart + options.SeedCount - 1 > int.MaxValue || options.Workers is < 1 or > 64 || options.ChunkSize < 1 || options.TimeoutSeconds is < 1 or > 86400)
            throw new ArgumentException("Check the seed range, worker count (1–64), chunk size and timeout (1–86,400 seconds).");
        if (options.PhaseTicks < 1 || options.PhaseTicks > 166666 || options.Width is < 1 or > 4096 || options.Height is < 1 or > 4096 || options.Plants < 0 || options.Hares < 1 || options.Foxes < 0)
            throw new ArgumentException("Use a positive round length and grid. Starting populations must fit the grid, with at least one Hare.");
        if (options.Strategies.Length == 0 || options.Purchases.Length == 0 || options.Strategies.Except(StrategyIds).Any() || options.Purchases.Except(PurchaseIds).Any() || options.Strategies.Distinct().Count() != options.Strategies.Length || options.Purchases.Distinct().Count() != options.Purchases.Length || RunCount(options) > 1000000 || VariantCount(options) * options.Strategies.Length * options.Purchases.Length > 10000)
            throw new ArgumentException("Choose supported strategies and purchase policies, with a total of at most 1,000,000 runs.");
        decimal Extreme(string key, int fallback, bool maximum) => options.Comparisons.FirstOrDefault(a => a.Key == key) is { } axis ? (maximum ? SetupParameters.Parse(axis).Max() : SetupParameters.Parse(axis).Min()) : fallback;
        if (Extreme("plant-population", options.Plants, true) + Extreme("hare-population", options.Hares, true) + Extreme("fox-population", options.Foxes, true) > Extreme("map-width", options.Width, false) * Extreme("map-height", options.Height, false))
            throw new ArgumentException("Some comparison populations do not fit the smallest requested map. Reduce populations or increase the map size.");
        if (options.HareVision < 0 || options.FoxEnergy < 0) throw new ArgumentException("Stat overrides must be nonnegative.");
        using var snapshot = Tools.ReadSmall(options.Snapshot);
        var data = snapshot.RootElement;
        if (!data.TryGetProperty("sourceHash", out _) || !data.TryGetProperty("species", out var species) || !data.TryGetProperty("arguments", out var arguments)) throw new InvalidDataException("This file is not a frozen scenario snapshot.");
        var ids = species.EnumerateArray().Select(s => s.GetProperty("id").GetString()).Order().ToArray();
        if (!ids.SequenceEqual(new[] { "fox", "hare", "plant" })) throw new InvalidDataException("This first setup screen supports the Forest Edge Plant/Hare/Fox scenario only.");
        var frozen = arguments.EnumerateArray().Select(a => a.GetString()!).ToArray();
        if (frozen.Length % 2 != 0 || frozen.Where((_, i) => i % 2 == 0).Any(key => key is "-mutationPolicy" or "-phaseUpgradeSchedule" or "-phaseUpgradeAssetSchedule" or "-runTicks" or "-runDurationSeconds" or "-speciesStats" or "-startingPopulations"))
            throw new InvalidDataException("Choose a neutral frozen snapshot without a preselected run length, strategy or stat/population overrides.");
    }

    internal static SetupOptions LoadPreset(string path)
    {
        if (new FileInfo(path).Length > 1024 * 1024) throw new InvalidDataException("This is too large to be a Workbench preset.");
        var options = JsonSerializer.Deserialize<SetupOptions>(File.ReadAllText(path), Json) ?? throw new InvalidDataException("Empty preset.");
        if (options.SchemaVersion != 1 || options.Comparisons == null || options.Strategies == null || options.Purchases == null)
            throw new InvalidDataException("Unsupported or incomplete Workbench preset.");
        foreach (var axis in options.Comparisons) SetupParameters.Find(axis.Key);
        return options with { Snapshot = string.IsNullOrWhiteSpace(options.Snapshot) ? "" : Path.GetFullPath(options.Snapshot, Path.GetDirectoryName(Path.GetFullPath(path))!) };
    }
    internal static void SavePreset(string path, SetupOptions options)
    {
        var temporary = path + ".tmp-" + Guid.NewGuid().ToString("N");
        try { File.WriteAllText(temporary, JsonSerializer.Serialize(options, Json)); File.Move(temporary, path, true); }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }

    internal static ProcessStartInfo Command(string executable, string working, params string[] arguments)
    {
        var start = new ProcessStartInfo(executable) { WorkingDirectory = working, UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true, StandardOutputEncoding = Encoding.UTF8, StandardErrorEncoding = Encoding.UTF8 };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        start.Environment["PYTHONIOENCODING"] = "utf-8";
        return start;
    }
    static async Task<int> Invoke(ProcessStartInfo start, Action<string> log)
    {
        using var process = Process.Start(start) ?? throw new IOException("Could not start " + start.FileName);
        return await Tools.Pump(process, log);
    }
    internal static string Python()
    {
        foreach (var folder in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
            if (File.Exists(Path.Combine(folder, "python.exe"))) return Path.Combine(folder, "python.exe");
        var root = Environment.GetEnvironmentVariable("CELLSIM_WORKSPACE_DEPENDENCIES") ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache", "codex-runtimes", "codex-primary-runtime", "dependencies");
        var bundled = Path.Combine(root, "python", "python.exe");
        return File.Exists(bundled) ? bundled : throw new FileNotFoundException("Sweep validation needs Python 3.11+ on PATH or in the configured workspace runtime.");
    }

    internal static async Task<PreparedRun> Prepare(string project, SetupOptions options, Action<string> log, string? runnerDirectory = null)
    {
        Validate(options);
        var runnerSource = runnerDirectory ?? Path.Combine(AppContext.BaseDirectory, "runner");
        if (!File.Exists(Path.Combine(runnerSource, "CellSim.Batch.dll"))) throw new FileNotFoundException("The Workbench runner is missing. Reopen using Open-CellSim-Workbench.cmd to build it.");
        var slug = new string(options.Name.Select(c => char.IsAsciiLetterOrDigit(c) ? c : '-').ToArray()).Trim('-');
        if (slug.Length == 0) slug = "experiment";
        slug = slug[..Math.Min(48, slug.Length)];
        var root = Path.Combine(project, "artifacts", "workbench-runs", slug + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N")[..6]);
        Directory.CreateDirectory(root);
        Directory.CreateDirectory(Path.Combine(root, "runner"));
        foreach (var name in new[] { "CellSim.Batch.dll", "CellSim.Batch.deps.json", "CellSim.Batch.runtimeconfig.json" })
            File.Copy(Path.Combine(runnerSource, name), Path.Combine(root, "runner", name));
        File.Copy(options.Snapshot, Path.Combine(root, "snapshot.json"));
        var frozen = options with { Snapshot = Path.Combine(root, "snapshot.json") };
        SavePreset(Path.Combine(root, "experiment.json"), frozen);
        var spec = Specification(options);
        File.WriteAllText(Path.Combine(root, "spec.json"), JsonSerializer.Serialize(spec, Json));
        log("Preparing a new experiment folder: " + root);
        var runner = Path.Combine(root, "runner", "CellSim.Batch.dll");
        var code = await Invoke(Command(Python(), project, Path.Combine(project, "tools", "CellSim.Batch", "sweep.py"), "compile", Path.Combine(root, "spec.json"), Path.Combine(root, "sweep"), "--runner", runner), log);
        if (code != 0) throw new InvalidDataException("Validation failed. No simulation workers were launched. Diagnostics are retained in " + root);
        var files = new[] { "experiment.json", "snapshot.json", "spec.json", "sweep/plan.json", "sweep/sweep.json", "runner/CellSim.Batch.dll", "runner/CellSim.Batch.deps.json", "runner/CellSim.Batch.runtimeconfig.json" }.ToDictionary(name => name, name => Hash(Path.Combine(root, name)));
        var prepared = new PreparedRun(root, frozen, files);
        File.WriteAllText(Path.Combine(root, "workbench-run.json"), JsonSerializer.Serialize(new { schemaVersion = 1, createdUtc = DateTime.UtcNow, runtime = Environment.Version.ToString(), files }, Json));
        log($"Validated {RunCount(options):N0} runs. Nothing has run yet. Click Run when ready.");
        return prepared;
    }

    internal static object Specification(SetupOptions options)
    {
        var overridden = options.Comparisons.Select(a => SetupParameters.Find(a.Key)).ToArray();
        var baseline = new List<string> { "-phaseLengthTicks", options.PhaseTicks.ToString(), "-wrapEdges", options.Wrap ? "true" : "false" };
        if (!overridden.Any(p => p.Argument == "-gridWidth")) baseline.AddRange(["-gridWidth", options.Width.ToString()]);
        if (!overridden.Any(p => p.Argument == "-gridHeight")) baseline.AddRange(["-gridHeight", options.Height.ToString()]);
        var populations = new Dictionary<string, int> { ["plant"] = options.Plants, ["hare"] = options.Hares, ["fox"] = options.Foxes };
        var fixedPops = populations.Where(p => !overridden.Any(a => a.Argument == "-startingPopulations" && a.Component == p.Key));
        if (fixedPops.Any()) baseline.AddRange(["-startingPopulations", string.Join(",", fixedPops.Select(p => p.Key + "=" + p.Value))]);
        var fixedStats = new Dictionary<string, int>();
        if (options.OverrideStats) { fixedStats["fox:energy.starting"] = options.FoxEnergy; fixedStats["hare:awareness.vision-range"] = options.HareVision; }
        var stats = fixedStats.Where(p => !overridden.Any(a => a.Argument == "-speciesStats" && a.Component == p.Key));
        if (stats.Any()) baseline.AddRange(["-speciesStats", string.Join(",", stats.Select(p => p.Key + "=" + p.Value))]);
        var axes = new List<object> { new { id = "purchase", values = options.Purchases.Select(p => new { id = p, arguments = new[] { "-harePurchasePolicy", p } }).ToArray() } };
        foreach (var axis in options.Comparisons)
        {
            var p = SetupParameters.Find(axis.Key);
            axes.Add(new { id = axis.Key.Replace(':', '-').Replace('.', '-'), values = SetupParameters.Parse(axis).Select(value => new { id = p.Label.Replace(' ', '-') + "-" + SetupParameters.Value(value), arguments = new[] { p.Argument, (p.Component.Length == 0 ? "" : p.Component + "=") + SetupParameters.Value(value) } }).ToArray() });
        }
        var spec = new
        {
            schemaVersion = 1, snapshot = "snapshot.json", seedStart = options.SeedStart, seedCount = options.SeedCount, workers = options.Workers, chunkSize = options.ChunkSize, timeoutSeconds = options.TimeoutSeconds, maxRuns = RunCount(options), horizonTicks = checked(options.PhaseTicks * 6), baseArguments = baseline,
            axes,
            paths = options.Strategies.Select(s => new { id = s, arguments = s == "skip-all" ? new[] { "-phaseUpgradeSchedule", "none;none;none;none;none;none" } : new[] { "-mutationPolicy", s } }).ToArray(), referencePath = options.Strategies[0],
            metrics = new[] { "outcome.playerAliveAtHorizon", "outcome.allSpeciesAliveAtHorizon", "population.hare.final", "population.fox.final", "population.plant.final" }
        };
        return spec;
    }

    internal static PreparedRun LoadRun(string root)
    {
        root = Path.GetFullPath(root);
        using var manifest = Tools.ReadSmall(Path.Combine(root, "workbench-run.json"));
        if (manifest.RootElement.GetProperty("schemaVersion").GetInt32() != 1) throw new InvalidDataException("Unsupported Workbench run version.");
        var files = manifest.RootElement.GetProperty("files").Deserialize<Dictionary<string, string>>()!;
        var required = new[] { "experiment.json", "snapshot.json", "spec.json", "sweep/plan.json", "sweep/sweep.json", "runner/CellSim.Batch.dll", "runner/CellSim.Batch.deps.json", "runner/CellSim.Batch.runtimeconfig.json" };
        if (!required.Order().SequenceEqual(files.Keys.Order())) throw new InvalidDataException("The frozen run file list is incomplete or unexpected.");
        foreach (var (name, hash) in files)
            if (Hash(Path.Combine(root, name)) != hash) throw new InvalidDataException("Frozen run input changed: " + name + ". Resume is refused; prepare a new run.");
        var settings = LoadPreset(Path.Combine(root, "experiment.json"));
        using var plan = Tools.ReadSmall(Path.Combine(root, "sweep", "plan.json"));
        if (!string.Equals(settings.Snapshot, Path.Combine(root, "snapshot.json"), StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(plan.RootElement.GetProperty("snapshot").GetString(), settings.Snapshot, StringComparison.OrdinalIgnoreCase) ||
            plan.RootElement.GetProperty("output").GetString() != "batch")
            throw new InvalidDataException("This frozen run was moved or points outside its folder. Resume it at its original location.");
        return new(root, settings, files);
    }

    internal static async Task<int> Preflight(PreparedRun run, Action<string> log) => await Invoke(Command("dotnet", run.Root, run.Runner, "batch", run.Plan, "--dry-run"), log);
}

internal sealed class RunSession : IDisposable
{
    readonly Process process;
    readonly string stopFile;
    internal RunSession(PreparedRun run, bool resume)
    {
        RunTools.LoadRun(run.Root);
        stopFile = Path.Combine(run.Root, "stop-" + Guid.NewGuid().ToString("N") + ".request");
        var arguments = new List<string> { run.Runner, "batch", run.Plan, "--stop-file", stopFile };
        if (resume) arguments.Add("--resume");
        process = Process.Start(RunTools.Command("dotnet", run.Root, arguments.ToArray())) ?? throw new IOException("Could not start the batch coordinator.");
    }
    internal void RequestStop()
    {
        if (process.HasExited || File.Exists(stopFile)) return;
        using var marker = new FileStream(stopFile, FileMode.CreateNew, FileAccess.Write);
    }
    internal Task<int> Completion(Action<string> log) => Tools.Pump(process, log);
    public void Dispose() => process.Dispose();
}
