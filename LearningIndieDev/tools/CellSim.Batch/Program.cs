using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using SaltyGame;
using static SaltyGame.EditorTools.CellularSimulationExperimentRunner;

internal static class Program
{
    static readonly JsonSerializerOptions Json = new JsonSerializerOptions
    { IncludeFields = true, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow };
    static readonly HashSet<string> DetailFields = new HashSet<string>
    { "behaviorTransitions", "trackedBehavior", "deathEvents", "combatRolls", "combatCooldownSuppressions" };
    static readonly HashSet<string> NullableArrays = new HashSet<string>
    { "mutationChoices", "purchaseWindows", "opportunityAudit", "pairedOpportunityIds" };
    static readonly HashSet<string> DerivedMetrics = new HashSet<string>
    { "pAVI", "eAVI", "predAVG", "sAVI", "cAVI", "bAVG", "RFS", "APS", "hAVG", "aAVG", "huntAVG", "AHS" };
    static volatile bool cancelled;
    static string stopFile;
    static void CheckCancelled()
    {
        if (stopFile != null && File.Exists(stopFile)) cancelled = true;
        if (cancelled) throw new OperationCanceledException("Stopped. Validated completed chunks are retained for --resume.");
    }

    static int Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cancelled = true; };
        try
        {
            if (args.Length == 0 || args[0] == "help")
            {
                Console.WriteLine("CellSim.Batch batch <plan.json> [--dry-run] [--resume] [--stop-file <path>]\n"
                    + "CellSim.Batch verify <reference.jsonl> <runs.jsonl>\n"
                    + "CellSim.Batch replay <snapshot.json> <seed> <new-output.json>\n"
                    + "CellSim.Batch replay-batch <batch-directory> <condition> <seed> <new-output.json>\n"
                    + "CellSim.Batch header <snapshot.json> <arguments.json> <seedStart> <seedCount> <new-output.json>\n"
                    + "CellSim.Batch status <batch-directory>\nCellSim.Batch self-test");
                return 0;
            }
            switch (args[0])
            {
                case "batch": Require(args, 2, 6); Batch(args[1], args.Skip(2).ToArray()); break;
                case "worker": Require(args, 3, 3); Worker(args[1], args[2]); break;
                case "verify": Require(args, 3, 3); Verify(args[1], args[2]); break;
                case "replay":
                    Require(args, 4, 4);
                    var snapshot = LoadSnapshot(args[1]);
                    using (var writer = NewWriter(args[3])) writer.Write(RunPortableSeed(snapshot, snapshot.arguments, int.Parse(args[2]), true));
                    break;
                case "status":
                    Require(args, 2, 2);
                    foreach (var name in new[] { "status.json", "summary.json" })
                        if (File.Exists(Path.Combine(args[1], name))) Console.WriteLine(File.ReadAllText(Path.Combine(args[1], name)));
                    break;
                case "replay-batch":
                    Require(args, 5, 5);
                    using (var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(args[1], "batch.json"))))
                    {
                        var plan = document.RootElement.GetProperty("plan").Deserialize<Plan>(Json);
                        if (Identity(plan) != document.RootElement.GetProperty("identity").GetString())
                            throw new InvalidDataException("Replay inputs, source or runtime differ from the frozen batch.");
                        var condition = plan.conditions.Single(c => c.id == args[2]);
                        var seed = int.Parse(args[3]);
                        if (seed < plan.seedStart || (long)seed >= (long)plan.seedStart + plan.seedCount)
                            throw new ArgumentOutOfRangeException("seed", "Seed is outside this batch.");
                        var input = LoadSnapshot(plan.snapshot);
                        using var output = NewWriter(args[4]);
                        output.Write(RunPortableSeed(input, condition.arguments, seed, true));
                    }
                    break;
                case "self-test": Require(args, 1, 1); SelfTest(); break;
                case "header":
                    Require(args, 6, 6);
                    var inputSnapshot = LoadSnapshot(args[1]); var scientificArguments = Read<string[]>(args[2]);
                    var header = PortableReportHeader(inputSnapshot, scientificArguments, int.Parse(args[3]), int.Parse(args[4]));
                    using (var writer = NewWriter(args[5])) writer.Write(header);
                    break;
                default: throw new ArgumentException("Unknown command. Use help.");
            }
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e.ToString()); return cancelled ? 130 : 1; }
    }

    static void Require(string[] args, int min, int max)
    { if (args.Length < min || args.Length > max) throw new ArgumentException("Invalid command arguments. Use help."); }
    static T Read<T>(string path) => JsonSerializer.Deserialize<T>(File.ReadAllText(path), Json)
        ?? throw new InvalidDataException("Empty JSON: " + path);
    static string HashFile(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }
    static string HashText(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();
    static string AssemblyHash => HashFile(Assembly.GetExecutingAssembly().Location);
    static StreamWriter NewWriter(string path) => new StreamWriter(new FileStream(path, FileMode.CreateNew, FileAccess.Write), new UTF8Encoding(false));
    static void WriteAtomic(string path, object value)
    {
        var temporary = path + ".tmp-" + Guid.NewGuid().ToString("N");
        File.WriteAllText(temporary, JsonSerializer.Serialize(value, Json));
        File.Move(temporary, path, true);
    }
    static PortableSnapshot LoadSnapshot(string path)
    {
        var snapshot = Read<PortableSnapshot>(path);
        var assembly = Assembly.GetExecutingAssembly();
        var sourceHash = PortableSourceHash(relative =>
        {
            using var stream = assembly.GetManifestResourceStream("CellSim.Source." + Path.GetFileName(relative))
                ?? throw new InvalidDataException("Missing embedded source: " + relative);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        });
        if (snapshot.sourceHash != sourceHash) throw new InvalidDataException("Worker source differs from export. Rebuild and re-export together.");
        snapshot.Restore(); ValidatePortableArguments(snapshot.arguments); return snapshot;
    }
    static string[] Merge(string[] baseline, string[] changes)
    {
        ValidatePortableArguments(baseline); ValidatePortableArguments(changes);
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var list in new[] { baseline, changes })
            for (var i = 0; i < list.Length; i += 2) values[list[i]] = list[i + 1];
        return values.OrderBy(pair => pair.Key, StringComparer.Ordinal).SelectMany(pair => new[] { pair.Key, pair.Value }).ToArray();
    }
    static string Identity(Plan plan) => HashText(JsonSerializer.Serialize(new { snapshotHash = HashFile(plan.snapshot), assemblyHash = AssemblyHash,
        runtime = Environment.Version.ToString(), plan.seedStart, plan.seedCount, plan.chunkSize, plan.conditions }, Json));

    static void Batch(string planPath, string[] flags)
    {
        var stopIndex = Array.IndexOf(flags, "--stop-file");
        if (stopIndex >= 0)
        {
            if (stopIndex + 1 >= flags.Length || flags[stopIndex + 1].StartsWith("--")) throw new ArgumentException("--stop-file requires a path.");
            stopFile = Path.GetFullPath(flags[stopIndex + 1]);
            flags = flags.Where((_, index) => index != stopIndex && index != stopIndex + 1).ToArray();
        }
        if (flags.Any(flag => flag != "--dry-run" && flag != "--resume") || flags.Distinct().Count() != flags.Length)
            throw new ArgumentException("Unknown/repeated batch flag.");
        var plan = Read<Plan>(planPath); var basePath = Path.GetDirectoryName(Path.GetFullPath(planPath));
        if (string.IsNullOrWhiteSpace(plan.snapshot) || string.IsNullOrWhiteSpace(plan.output)) throw new ArgumentException("Missing paths.");
        plan.snapshot = Path.GetFullPath(plan.snapshot, basePath); plan.output = Path.GetFullPath(plan.output, basePath);
        if (plan.schemaVersion != 1 || plan.seedStart < 0 || plan.seedCount <= 0 || plan.chunkSize <= 0
            || plan.workers < 1 || plan.workers > 64 || plan.timeoutSeconds < 1 || plan.timeoutSeconds > 86400
            || plan.conditions == null || plan.conditions.Length == 0)
            throw new ArgumentException("Invalid plan version, seed range, worker/chunk limits or conditions.");
        checked { _ = plan.seedStart + plan.seedCount - 1; }
        if (plan.conditions.Any(c => !Regex.IsMatch(c.id ?? "", "^[A-Za-z0-9_-]{1,80}$"))
            || plan.conditions.Select(c => c.id).Distinct(StringComparer.Ordinal).Count() != plan.conditions.Length)
            throw new ArgumentException("Condition IDs must be unique letters/digits/underscore/hyphen.");
        var snapshot = LoadSnapshot(plan.snapshot);
        foreach (var c in plan.conditions)
        {
            c.arguments = Merge(snapshot.arguments, c.arguments);
            try { ValidatePortableConfiguration(snapshot, c.arguments); }
            catch (Exception e) { throw new ArgumentException("Invalid condition: " + c.id, e); }
        }
        var identity = Identity(plan);
        var jobs = new List<Job>();
        // ponytail: one host scans an in-memory job list; use larger chunks if
        // scheduling overhead at very high chunk counts becomes measurable.
        foreach (var c in plan.conditions.OrderBy(c => c.id, StringComparer.Ordinal))
            for (long offset = 0; offset < plan.seedCount; offset += plan.chunkSize)
                jobs.Add(new Job { identity = identity, snapshot = plan.snapshot, condition = c.id, arguments = c.arguments,
                    seedStart = checked(plan.seedStart + (int)offset), seedCount = (int)Math.Min(plan.chunkSize, plan.seedCount - offset),
                    timeoutSeconds = plan.timeoutSeconds,
                    directory = Path.Combine(plan.output, "chunks", c.id + "-" + offset.ToString("D10")) });
        Console.WriteLine($"{(long)plan.seedCount * plan.conditions.Length:N0} runs; {jobs.Count} chunks; {plan.workers} workers; identity {identity}");
        if (flags.Contains("--dry-run")) return;
        Directory.CreateDirectory(plan.output);
        using var controllerLock = new FileStream(Path.Combine(plan.output, ".controller.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        var manifest = Path.Combine(plan.output, "batch.json");
        if (File.Exists(manifest))
        {
            if (!flags.Contains("--resume")) throw new InvalidOperationException("Existing batch. Use --resume or a new output directory.");
            using var existing = JsonDocument.Parse(File.ReadAllText(manifest));
            if (existing.RootElement.GetProperty("identity").GetString() != identity) throw new InvalidOperationException("Resume identity differs.");
        }
        else if (Directory.EnumerateFileSystemEntries(plan.output).Any(path => Path.GetFileName(path) != ".controller.lock"))
            throw new InvalidOperationException("New batch output must be empty.");
        WriteAtomic(manifest, new { schemaVersion = 1, identity, plan, sourceHash = snapshot.sourceHash, assemblyHash = AssemblyHash });
        var stopwatch = Stopwatch.StartNew(); var active = new Dictionary<Process, (Job job, DateTime started)>();
        var completed = new HashSet<string>(); var failures = new Dictionary<string, int>(); var lastProgress = DateTime.MinValue;
        var inheritedClaims = jobs.Where(job => Directory.Exists(job.directory) && Claimed(job.directory))
            .ToDictionary(job => job.directory, job => job);
        try
        {
            while (completed.Count != jobs.Count)
            {
                CheckCancelled();
                foreach (var process in active.Keys.ToArray())
                {
                    var attempt = active[process];
                    if (!process.HasExited && (DateTime.UtcNow - attempt.started).TotalSeconds > plan.timeoutSeconds) process.Kill(true);
                    if (!process.HasExited) continue;
                    if (process.ExitCode != 0)
                    {
                        failures.TryGetValue(attempt.job.directory, out var n); failures[attempt.job.directory] = n + 1;
                        if (n >= 1) throw new InvalidOperationException("Chunk failed twice: " + attempt.job.directory);
                    }
                    active.Remove(process); process.Dispose();
                }
                var occupied = active.Count;
                foreach (var job in inheritedClaims.Values.ToArray())
                {
                    if (Claimed(job.directory)) { occupied++; continue; }
                    inheritedClaims.Remove(job.directory);
                    if (TryCompleted(job, out _)) completed.Add(job.directory);
                }
                foreach (var job in jobs)
                {
                    CheckCancelled();
                    if (completed.Contains(job.directory)) continue;
                    if (occupied >= plan.workers) break;
                    if (active.Values.Any(a => a.job.directory == job.directory)) continue;
                    if (inheritedClaims.ContainsKey(job.directory)) continue;
                    if (TryCompleted(job, out _)) { completed.Add(job.directory); continue; }
                    Directory.CreateDirectory(job.directory);
                    if (Claimed(job.directory)) { occupied++; continue; }
                    var jobPath = Path.Combine(job.directory, "job.json");
                    if (!File.Exists(jobPath)) WriteAtomic(jobPath, job);
                    else if (Read<Job>(jobPath).identity != identity) throw new InvalidDataException("Chunk identity mismatch.");
                    var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true };
                    start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("worker");
                    start.ArgumentList.Add(jobPath); start.ArgumentList.Add(AssemblyHash);
                    var process = Process.Start(start) ?? throw new InvalidOperationException("Worker could not start.");
                    active.Add(process, (job, DateTime.UtcNow)); occupied++;
                }
                if ((DateTime.UtcNow - lastProgress).TotalSeconds >= 5)
                {
                    WriteAtomic(Path.Combine(plan.output, "status.json"), new { state = "Running", completedChunks = completed.Count,
                        totalChunks = jobs.Count, activeWorkers = occupied, elapsedSeconds = stopwatch.Elapsed.TotalSeconds });
                    Console.WriteLine($"{completed.Count}/{jobs.Count} validated chunks; {occupied} active; {stopwatch.Elapsed.TotalSeconds:F1}s");
                    lastProgress = DateTime.UtcNow;
                }
                if (completed.Count != jobs.Count) Thread.Sleep(100);
            }
            Aggregate(plan, jobs, identity, stopwatch);
        }
        catch
        {
            WriteAtomic(Path.Combine(plan.output, "status.json"), new { state = cancelled ? "Cancelled" : "Failed", completedChunks = completed.Count, totalChunks = jobs.Count });
            throw;
        }
        finally
        {
            foreach (var process in active.Keys)
            { if (!process.HasExited) process.Kill(true); process.WaitForExit(); process.Dispose(); }
        }
    }
    static bool Claimed(string directory)
    {
        try { using var claim = new FileStream(Path.Combine(directory, ".worker.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None); return false; }
        catch (IOException) { return true; }
    }
    static void Worker(string jobPath, string assemblyHash)
    {
        if (assemblyHash != AssemblyHash) throw new InvalidDataException("Assembly changed after dispatch.");
        var job = Read<Job>(jobPath);
        using var claim = new FileStream(Path.Combine(job.directory, ".worker.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        // A worker outliving its controller still exits at its own chunk deadline.
        using var deadline = new Timer(_ => Environment.Exit(124), null, TimeSpan.FromSeconds(job.timeoutSeconds), Timeout.InfiniteTimeSpan);
        if (TryCompleted(job, out _)) return;
        var number = Directory.EnumerateDirectories(job.directory, "attempt-*").Count() + 1;
        var attempt = Path.Combine(job.directory, "attempt-" + number.ToString("D4")); Directory.CreateDirectory(attempt);
        var started = DateTime.UtcNow;
        WriteAtomic(Path.Combine(attempt, "status.json"), new { state = "Running", pid = Environment.ProcessId, startedUtc = started });
        try
        {
            var snapshot = LoadSnapshot(job.snapshot); var data = snapshot.Restore(); var output = Path.Combine(attempt, "runs.jsonl");
            var stopwatch = Stopwatch.StartNew();
            using (var writer = NewWriter(output))
                for (var offset = 0; offset < job.seedCount; offset++)
                {
                    if (cancelled) throw new OperationCanceledException("Worker cancelled.");
                    var seed = checked(job.seedStart + offset); var text = RunPortableSeed(snapshot, job.arguments, seed, false, data);
                    using var document = JsonDocument.Parse(text); ValidateRun(document.RootElement, seed, snapshot);
                    writer.WriteLine("{\"condition\":" + JsonSerializer.Serialize(job.condition) + ",\"run\":" + text + "}");
                }
            var done = new Completion { identity = job.identity, condition = job.condition, seedStart = job.seedStart, seedCount = job.seedCount,
                output = output, outputHash = HashFile(output), elapsedSeconds = stopwatch.Elapsed.TotalSeconds,
                startedUtc = started, endedUtc = DateTime.UtcNow, peakWorkingSetBytes = Process.GetCurrentProcess().PeakWorkingSet64 };
            ValidateChunk(job, done); WriteAtomic(Path.Combine(attempt, "completion.json"), done);
            WriteAtomic(Path.Combine(job.directory, "completion.json"), done);
            WriteAtomic(Path.Combine(attempt, "status.json"), new { state = "Completed" });
        }
        catch (Exception e) { WriteAtomic(Path.Combine(attempt, "status.json"), new { state = "Failed", error = e.ToString() }); throw; }
    }
    static bool TryCompleted(Job job, out Completion done)
    {
        var path = Path.Combine(job.directory, "completion.json"); done = null;
        if (!File.Exists(path)) return false;
        done = Read<Completion>(path); ValidateChunk(job, done); return true;
    }
    static void ValidateChunk(Job job, Completion done)
    {
        if (done.identity != job.identity || done.condition != job.condition || done.seedStart != job.seedStart || done.seedCount != job.seedCount
            || !Path.GetFullPath(done.output).StartsWith(Path.GetFullPath(job.directory) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
            || !File.Exists(done.output) || HashFile(done.output) != done.outputHash) throw new InvalidDataException("Chunk provenance/integrity mismatch: " + job.directory);
        var offset = 0;
        var snapshot = Read<PortableSnapshot>(job.snapshot);
        foreach (var line in File.ReadLines(done.output))
        {
            using var document = JsonDocument.Parse(line);
            if (offset >= job.seedCount || document.RootElement.GetProperty("condition").GetString() != job.condition) throw new InvalidDataException("Incorrect/duplicate coverage.");
            ValidateRun(document.RootElement.GetProperty("run"), checked(job.seedStart + offset), snapshot); offset++;
        }
        if (offset != job.seedCount) throw new InvalidDataException("Missing seeds.");
    }
    static void ValidateRun(JsonElement run, int seed, PortableSnapshot snapshot)
    {
        if (run.GetProperty("seed").GetInt32() != seed || run.GetProperty("ticks").GetInt32() < 0
            || !Regex.IsMatch(run.GetProperty("finalStateDigest").GetString() ?? "", "^[a-f0-9]{64}$")) throw new InvalidDataException("Invalid run/state digest.");
        foreach (var name in new[] { "herbivoreStatLine", "predatorStatLine" })
            if (run.TryGetProperty(name, out var stat) && stat.ValueKind == JsonValueKind.Object)
            {
                var species = snapshot.species.FirstOrDefault(s => s.id == stat.GetProperty("speciesId").GetString());
                var role = name == "herbivoreStatLine" ? SpeciesRole.Herbivore : SpeciesRole.Carnivore;
                // The legacy policy report also emits a predator stat line for a
                // Hare. Its denominators are inapplicable; validate the real role.
                if (species != null && species.rules.role == role && !stat.GetProperty("fpoReconciled").GetBoolean())
                    throw new InvalidDataException("Population accounting failed: " + name);
            }
        foreach (var activity in run.GetProperty("activity").EnumerateArray())
            if (!activity.GetProperty("reproductionReconciled").GetBoolean()) throw new InvalidDataException("Reproduction accounting failed.");
        foreach (var window in new[] { run }.Concat(run.GetProperty("phaseResults").EnumerateArray()))
            foreach (var name in new[] { "herbivoreStatLines", "predatorStatLines" })
                foreach (var stat in window.GetProperty(name).EnumerateArray())
                    if (!stat.GetProperty("fpoReconciled").GetBoolean()) throw new InvalidDataException("Species/phase population accounting failed.");
        if (run.TryGetProperty("purchaseWindows", out var purchases) && purchases.ValueKind == JsonValueKind.Array)
        {
            var balance = 0; var added = 0; var phase = 0;
            foreach (var purchase in purchases.EnumerateArray())
            {
                int N(string key) => purchase.GetProperty(key).GetInt32();
                if (N("phase") != ++phase || phase > 5 || N("decisionTick") >= run.GetProperty("ticks").GetInt32()
                    || N("price") != SpeciesProgression.HarePurchaseCost || N("balanceBefore") != balance
                    || N("earned") != N("populationBefore") || N("earned") <= 0
                    || N("added") < 0 || N("added") > N("requested") || N("requested") > 5
                    || N("spent") != N("added") * N("price")
                    || N("balanceAfter") != balance + N("earned") - N("spent") || N("balanceAfter") < 0
                    || N("populationAfter") != N("populationBefore") + N("added"))
                    throw new InvalidDataException("Purchase ledger failed to reconcile.");
                balance = N("balanceAfter"); added += N("added");
            }
            var stat = run.GetProperty("herbivoreStatLines").EnumerateArray().Single(s => s.GetProperty("speciesId").GetString() == "hare");
            if (stat.GetProperty("ADD").GetInt32() != added)
                throw new InvalidDataException("Purchase additions differ from population accounting.");
        }
    }
    static void Aggregate(Plan plan, List<Job> jobs, string identity, Stopwatch stopwatch)
    {
        var summaryPath = Path.Combine(plan.output, "summary.json"); var output = Path.Combine(plan.output, "runs.jsonl");
        if (File.Exists(summaryPath))
        {
            using var old = JsonDocument.Parse(File.ReadAllText(summaryPath));
            if (old.RootElement.GetProperty("identity").GetString() != identity || old.RootElement.GetProperty("outputHash").GetString() != HashFile(output))
                throw new InvalidDataException("Existing aggregate integrity mismatch.");
            WriteAtomic(Path.Combine(plan.output, "status.json"), new { state = "Completed", completedChunks = jobs.Count, totalChunks = jobs.Count });
            Console.WriteLine("Already complete and revalidated: " + summaryPath); return;
        }
        var staging = Path.Combine(plan.output, "runs.partial.jsonl"); var rows = 0L; var peak = 0L; var counts = new Dictionary<string, long>();
        var diagnostics = new Dictionary<string, Diagnostics>();
        using (var writer = new StreamWriter(new FileStream(staging, FileMode.Create, FileAccess.Write), new UTF8Encoding(false)))
            foreach (var job in jobs)
            {
                CheckCancelled();
                if (!TryCompleted(job, out var done)) throw new InvalidDataException("Missing chunk.");
                peak = Math.Max(peak, done.peakWorkingSetBytes);
                foreach (var line in File.ReadLines(done.output))
                {
                    if (rows % 256 == 0) CheckCancelled();
                    writer.WriteLine(line); rows++; counts.TryGetValue(job.condition, out var n); counts[job.condition] = n + 1;
                    if (!diagnostics.TryGetValue(job.condition, out var stats)) diagnostics[job.condition] = stats = new Diagnostics();
                    using var document = JsonDocument.Parse(line); var run = document.RootElement.GetProperty("run");
                    stats.totalTicks += run.GetProperty("ticks").GetInt32();
                    if (run.GetProperty("playerPopulation").GetInt32() == 0) stats.playerExtinctions++;
                    var final = run.GetProperty("populationHistory").EnumerateArray().Last().GetProperty("species").EnumerateArray().ToArray();
                    if (final.All(s => s.GetProperty("population").GetInt32() > 0)) stats.allSpeciesPresentAtEnd++;
                    foreach (var species in final)
                    {
                        var id = species.GetProperty("speciesId").GetString(); stats.totalFinalPopulation.TryGetValue(id, out var population);
                        stats.totalFinalPopulation[id] = population + species.GetProperty("population").GetInt32();
                    }
                }
            }
        if (rows != (long)plan.seedCount * plan.conditions.Length) throw new InvalidDataException("Aggregate coverage mismatch.");
        File.Move(staging, output, true); stopwatch.Stop();
        WriteAtomic(summaryPath, new { schemaVersion = 1, identity, state = "Completed", runs = rows, counts, diagnostics,
            elapsedSeconds = stopwatch.Elapsed.TotalSeconds, runsPerSecond = rows / stopwatch.Elapsed.TotalSeconds,
            peakWorkerWorkingSetBytes = peak, outputBytes = new FileInfo(output).Length, outputHash = HashFile(output),
            note = "Elapsed covers this invocation; resume is not a fresh throughput benchmark." });
        WriteAtomic(Path.Combine(plan.output, "status.json"), new { state = "Completed", completedChunks = jobs.Count, totalChunks = jobs.Count });
        Console.WriteLine($"Completed {rows:N0} validated runs in {stopwatch.Elapsed.TotalSeconds:F2}s: {summaryPath}");
    }
    static JsonElement RunElement(JsonElement value) => value.TryGetProperty("run", out var run) ? run : value;
    static string Canonical(JsonElement value, string name = "")
    {
        if (value.ValueKind == JsonValueKind.Null && NullableArrays.Contains(name)) return "[]";
        if ((name == "herbivoreStatLine" || name == "predatorStatLine") && value.ValueKind == JsonValueKind.Object
            && value.GetProperty("speciesId").GetString() == "") return "null";
        switch (value.ValueKind)
        {
            case JsonValueKind.Object: return "{" + string.Join(",", value.EnumerateObject().Where(p => !DetailFields.Contains(p.Name))
                .OrderBy(p => p.Name, StringComparer.Ordinal).Select(p => JsonSerializer.Serialize(p.Name) + ":" + Canonical(p.Value, p.Name))) + "}";
            case JsonValueKind.Array:
                var array = value.EnumerateArray().ToArray();
                if (name == "populationHistory" && array.Length > 1) array = new[] { array[0], array[array.Length - 1] };
                return "[" + string.Join(",", array.Select(e => Canonical(e))) + "]";
            case JsonValueKind.Number: return value.TryGetInt64(out var integer) ? integer.ToString(CultureInfo.InvariantCulture)
                : value.GetSingle().ToString("R", CultureInfo.InvariantCulture);
            default: return value.GetRawText();
        }
    }
    static void Compare(JsonElement left, JsonElement right, string path = "run", string name = "")
    {
        if (Canonical(left, name) == Canonical(right, name)) return;
        // Mono can retain intermediate precision in report-only rate arithmetic.
        // State digests, counters, timings, choices and all other floats stay exact.
        if (left.ValueKind == JsonValueKind.Number && right.ValueKind == JsonValueKind.Number
            && DerivedMetrics.Contains(name) && (path.Contains(".herbivoreStatLine.") || path.Contains(".predatorStatLine.")
                || path.Contains(".herbivoreStatLines[") || path.Contains(".predatorStatLines[")))
        {
            var a = left.GetDouble(); var b = right.GetDouble();
            if (Math.Abs(a - b) <= 1e-6 * Math.Max(1, Math.Max(Math.Abs(a), Math.Abs(b)))) return;
        }
        if (left.ValueKind == JsonValueKind.Object && right.ValueKind == JsonValueKind.Object)
        {
            var a = left.EnumerateObject().Where(p => !DetailFields.Contains(p.Name)).ToDictionary(p => p.Name, p => p.Value);
            var b = right.EnumerateObject().Where(p => !DetailFields.Contains(p.Name)).ToDictionary(p => p.Name, p => p.Value);
            if (!a.Keys.OrderBy(k => k).SequenceEqual(b.Keys.OrderBy(k => k))) throw new InvalidDataException("Field mismatch: " + path);
            foreach (var key in a.Keys) Compare(a[key], b[key], path + "." + key, key);
            return;
        }
        if (left.ValueKind == JsonValueKind.Array && right.ValueKind == JsonValueKind.Array)
        {
            var a = left.EnumerateArray().ToArray(); var b = right.EnumerateArray().ToArray();
            if (name == "populationHistory")
            {
                if (a.Length > 1) a = new[] { a[0], a[a.Length - 1] };
                if (b.Length > 1) b = new[] { b[0], b[b.Length - 1] };
            }
            if (a.Length == b.Length) { for (var i = 0; i < a.Length; i++) Compare(a[i], b[i], path + "[" + i + "]"); return; }
        }
        throw new InvalidDataException("Scientific mismatch: " + path + " reference=" + left.GetRawText() + " actual=" + right.GetRawText());
    }
    static void Verify(string referencePath, string actualPath)
    {
        using var reference = File.ReadLines(referencePath).GetEnumerator(); using var actual = File.ReadLines(actualPath).GetEnumerator(); var count = 0;
        while (reference.MoveNext())
        {
            if (!actual.MoveNext()) throw new InvalidDataException("Actual has missing runs.");
            using var left = JsonDocument.Parse(reference.Current); using var right = JsonDocument.Parse(actual.Current);
            Compare(RunElement(left.RootElement), RunElement(right.RootElement), "run[" + count + "]");
            count++;
        }
        if (actual.MoveNext() || count == 0) throw new InvalidDataException("Extra runs or empty reference.");
        Console.WriteLine($"PASS: {count} corresponding runs; exact state/counters/choices/timing; derived report rates within 1e-6 relative/absolute tolerance.");
    }
    static void SelfTest()
    {
        var data = new CellularSimData(8, 8, new Dictionary<SpeciesId, float>
        { [SpeciesIds.Plant] = .3f, [SpeciesIds.Herbivore] = .1f, [SpeciesIds.Carnivore] = .03f }, SpeciesRuleDefaults.Create(), 4f, .1f);
        var snapshot = PortableSnapshot.Create(data);
        snapshot.arguments = new[] { "-playerSpeciesId", "herbivore", "-runTicks", "40", "-experimentalFeatures", "bev-experimental" };
        var restored = UnityEngine.JsonUtility.FromJson<PortableSnapshot>(UnityEngine.JsonUtility.ToJson(snapshot));
        if (restored.Restore().Fingerprint != data.Fingerprint) throw new Exception("Snapshot round-trip failed.");
        using var a = JsonDocument.Parse(RunPortableSeed(restored, snapshot.arguments, 42, false));
        using var b = JsonDocument.Parse(RunPortableSeed(restored, snapshot.arguments, 42, true)); ValidateRun(a.RootElement, 42, restored);
        if (Canonical(a.RootElement) != Canonical(b.RootElement)) throw new Exception("Compact/detailed or repeat determinism failed.");
        var rejected = false;
        try { ValidatePortableArguments(new[] { "-typo", "true" }); } catch (ArgumentException) { rejected = true; }
        if (!rejected) throw new Exception("Unknown option accepted.");
        var statData = ApplyPortableStats(data, new[] { "-speciesStats", "herbivore:awareness.vision-range=9" });
        if (statData.SpeciesRules[SpeciesIds.Herbivore].Awareness.VisionRange != 9
            || statData.Fingerprint == data.Fingerprint) throw new Exception("Absolute stat override failed.");
        foreach (var invalid in new[] { "herbivore:typo=1", "herbivore:energy.starting=9.5", "herbivore:resource.seed-drop-chance=2",
            "herbivore:awareness.vision-range=9,herbivore:awareness.vision-range=10" })
        {
            rejected = false;
            try { ApplyPortableStats(data, new[] { "-speciesStats", invalid }); } catch (ArgumentException) { rejected = true; } catch (FormatException) { rejected = true; }
            if (!rejected) throw new Exception("Invalid stat override accepted: " + invalid);
        }
        var trajectory = a.RootElement.GetProperty("populationTrajectory").EnumerateArray().First(t => t.GetProperty("speciesId").GetString() == "herbivore");
        var integral = b.RootElement.GetProperty("populationHistory").EnumerateArray().Skip(1)
            .Sum(sample => sample.GetProperty("species").EnumerateArray().First(s => s.GetProperty("speciesId").GetString() == "herbivore").GetProperty("population").GetInt64());
        if (trajectory.GetProperty("populationTickIntegral").GetInt64() != integral) throw new Exception("Trajectory exposure failed.");
        var hare = new SpeciesId("hare"); var rules = new Dictionary<SpeciesId, SpeciesRules>(SpeciesRuleDefaults.Create());
        rules[hare] = rules[SpeciesIds.Herbivore];
        var policyData = new CellularSimData(8, 8, new Dictionary<SpeciesId, float>
        { [SpeciesIds.Plant] = .3f, [hare] = .1f, [SpeciesIds.Carnivore] = .03f },
            rules, 4f, .1f);
        var policySnapshot = PortableSnapshot.Create(policyData);
        using var policy = JsonDocument.Parse(RunPortableSeed(policySnapshot,
            new[] { "-playerSpeciesId", "hare", "-phaseLengthTicks", "3", "-mutationPolicy", "trailblazer", "-experimentalFeatures", "bev-experimental" }, 42, false));
        ValidateRun(policy.RootElement, 42, policySnapshot);
        var starting = new Dictionary<SpeciesId, int> { [hare] = 30, [SpeciesIds.Plant] = 0,
            [SpeciesIds.Herbivore] = 0, [SpeciesIds.Carnivore] = 0 };
        var purchaseData = new CellularSimData(16, 16, policyData.StartingProbabilities, rules, 4f, .1f,
            startingPopulations: starting);
        var purchaseSnapshot = PortableSnapshot.Create(purchaseData);
        var purchaseArguments = new[] { "-playerSpeciesId", "hare", "-phaseLengthTicks", "1",
            "-phaseUpgradeSchedule", "none;none;none;none;none;none", "-experimentalFeatures", "bev-experimental" };
        using var control = JsonDocument.Parse(RunPortableSeed(purchaseSnapshot, purchaseArguments, 42, false));
        foreach (var purchasePolicy in new[] { "none", "early-five", "middle-five", "late-five", "each-one",
            "each-three", "each-five", "restore-toward-start" })
        {
            var input = purchaseArguments.Concat(new[] { "-harePurchasePolicy", purchasePolicy }).ToArray();
            using var bought = JsonDocument.Parse(RunPortableSeed(purchaseSnapshot, input, 42, false));
            ValidateRun(bought.RootElement, 42, purchaseSnapshot);
            using var repeated = JsonDocument.Parse(RunPortableSeed(purchaseSnapshot, input, 42, true));
            if (Canonical(bought.RootElement) != Canonical(repeated.RootElement))
                throw new Exception("Purchase determinism failed: " + purchasePolicy);
            var windows = bought.RootElement.GetProperty("purchaseWindows").EnumerateArray().ToArray();
            if (windows.Length != 5) throw new Exception("Expected five reached purchase windows.");
            if (purchasePolicy == "early-five" && (windows[0].GetProperty("added").GetInt32() != 3
                || windows[0].GetProperty("stopReason").GetString() != "insufficient-currency"))
                throw new Exception("Unaffordable purchases were granted.");
            if (purchasePolicy == "none")
            {
                string Strip(JsonElement root) => JsonSerializer.Serialize(root.EnumerateObject()
                    .Where(p => p.Name != "purchaseWindows").ToDictionary(p => p.Name, p => p.Value));
                using var original = JsonDocument.Parse(Strip(control.RootElement));
                using var audited = JsonDocument.Parse(Strip(bought.RootElement));
                if (Canonical(original.RootElement) != Canonical(audited.RootElement))
                    throw new Exception("No-purchase audit changed the simulation.");
            }
        }
        var cappedData = new CellularSimData(16, 16, policyData.StartingProbabilities, rules, 4f, .1f,
            maxPopulation: 30, startingPopulations: starting);
        var cappedSnapshot = PortableSnapshot.Create(cappedData);
        using var capped = JsonDocument.Parse(RunPortableSeed(cappedSnapshot,
            purchaseArguments.Concat(new[] { "-harePurchasePolicy", "each-five" }).ToArray(), 42, false));
        ValidateRun(capped.RootElement, 42, cappedSnapshot);
        var firstWindow = capped.RootElement.GetProperty("purchaseWindows")[0];
        if (firstWindow.GetProperty("added").GetInt32() != 0 || firstWindow.GetProperty("spent").GetInt32() != 0
            || firstWindow.GetProperty("stopReason").GetString() != "placement-or-capacity")
            throw new Exception("Failed placement was charged or exceeded capacity.");
        foreach (var invalid in new[] { "unknown", "" })
        {
            rejected = false;
            try { RunPortableSeed(purchaseSnapshot, purchaseArguments.Concat(new[] { "-harePurchasePolicy", invalid }).ToArray(), 42, false); }
            catch (ArgumentException) { rejected = true; }
            if (!rejected) throw new Exception("Invalid purchase policy accepted.");
        }
        restored.width++; rejected = false;
        try { restored.Restore(); } catch (InvalidOperationException) { rejected = true; }
        if (!rejected) throw new Exception("Corrupt snapshot accepted.");
        Console.WriteLine("PASS: snapshot round-trip; compact/detailed parity; repeat determinism; invalid input rejection; eight purchase policies; wallet carryover; affordability; placement/capacity; no-purchase parity.");
    }
    public sealed class Plan
    {
        public int schemaVersion = 1; public string snapshot, output;
        public int seedStart = 1, seedCount, chunkSize = 100, workers = 2, timeoutSeconds = 3600;
        public Condition[] conditions;
    }
    public sealed class Condition { public string id; public string[] arguments = Array.Empty<string>(); }
    public sealed class Job { public string identity, snapshot, condition, directory; public string[] arguments; public int seedStart, seedCount, timeoutSeconds = 3600; }
    public sealed class Completion
    {
        public string identity, condition, output, outputHash; public int seedStart, seedCount;
        public double elapsedSeconds; public DateTime startedUtc, endedUtc; public long peakWorkingSetBytes;
    }
    public sealed class Diagnostics
    {
        public long totalTicks, playerExtinctions, allSpeciesPresentAtEnd;
        public Dictionary<string, long> totalFinalPopulation = new Dictionary<string, long>();
    }
}
