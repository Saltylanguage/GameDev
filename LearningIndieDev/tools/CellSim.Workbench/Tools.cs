using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CellSim.Workbench;

internal sealed record ExportOptions(string[] Reports, string Output, string Match, string Seeds, int Rows, int FirstTick, int SecondTick, bool DetailedObservations = false);
internal sealed record BatchProgress(string Folder, string State, int CompletedChunks, int TotalChunks, string Workers, string Runs, DateTime UpdatedUtc, bool Stale);

internal static class Tools
{
    internal static string? FindProject(string start)
    {
        for (var folder = new DirectoryInfo(start); folder != null; folder = folder.Parent)
            if (File.Exists(Path.Combine(folder.FullName, "tools", "Export-CellSimWorksheet.ps1"))) return folder.FullName;
        return null;
    }

    internal static void ValidateProject(string project)
    {
        if (!File.Exists(Path.Combine(project, "tools", "Export-CellSimWorksheet.ps1")))
            throw new InvalidDataException("Select the LearningIndieDev folder containing tools/Export-CellSimWorksheet.ps1.");
    }

    internal static int[] ParseSeeds(string text) => string.IsNullOrWhiteSpace(text) ? [] :
        text.Split([',', ' ', ';', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(value =>
            int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var seed) ? seed :
                throw new ArgumentException("Seeds must be whole nonnegative numbers, separated by commas or spaces.")).Distinct().ToArray();

    internal static void ValidateExport(ExportOptions options)
    {
        if (options.Reports.Length == 0) throw new ArgumentException("Add at least one simulation report or completed sweep first.");
        if (options.Reports.Any(path => !File.Exists(path) && !Directory.Exists(path)))
            throw new FileNotFoundException("A selected source no longer exists. Remove it and choose its current location.");
        if (options.Rows is < 1 or > 1000 || options.FirstTick < 0 || options.SecondTick <= options.FirstTick)
            throw new ArgumentException("Choose 1–1,000 example rows and two increasing checkpoint ticks.");
        ParseSeeds(options.Seeds);
        if (!Path.IsPathFullyQualified(options.Output) || !options.Output.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Choose a full output filename ending in .xlsx.");
        if (File.Exists(options.Output) || Directory.Exists(options.Output))
            throw new IOException("This workbook already exists. Choose a new filename to protect any human notes.");
    }

    // Encode only after quoting each value; report paths and filters are data, never PowerShell code.
    internal static string Quote(string value) => "'" + value.Replace("'", "''") + "'";
    internal static string ExportCommand(string project, ExportOptions options)
    {
        ValidateExport(options);
        var seeds = ParseSeeds(options.Seeds);
        return "$ProgressPreference = 'SilentlyContinue'; $OutputEncoding = [Console]::OutputEncoding = [Text.UTF8Encoding]::new($false); & " +
            Quote(Path.Combine(project, "tools", "Export-CellSimWorksheet.ps1")) +
            " -ProjectPath " + Quote(project) + " -ReportPath @(" + string.Join(",", options.Reports.Select(Quote)) + ")" +
            " -OutputPath " + Quote(options.Output) + " -MaxRuns " + options.Rows +
            " -Checkpoints " + options.FirstTick + "," + options.SecondTick + " -Match " + Quote(options.Match) +
            (seeds.Length == 0 ? "" : " -Seeds " + string.Join(",", seeds)) +
            (options.DetailedObservations ? " -DetailedObservations" : "") + "; exit $LASTEXITCODE";
    }

    internal static async Task<int> Export(string project, ExportOptions options, Action<string> log)
    {
        var start = new ProcessStartInfo("powershell.exe")
        {
            WorkingDirectory = project, UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8, StandardErrorEncoding = Encoding.UTF8
        };
        foreach (var value in new[] { "-NoProfile", "-NonInteractive", "-ExecutionPolicy", "Bypass", "-OutputFormat", "Text", "-EncodedCommand",
            Convert.ToBase64String(Encoding.Unicode.GetBytes(ExportCommand(project, options))) }) start.ArgumentList.Add(value);
        using var process = Process.Start(start) ?? throw new IOException("Windows could not start the exporter.");
        return await Pump(process, log);
    }

    internal static async Task<int> Pump(Process process, Action<string> log)
    {
        async Task Drain(StreamReader reader)
        {
            while (await reader.ReadLineAsync() is { } line) log(line);
        }
        await Task.WhenAll(Drain(process.StandardOutput), Drain(process.StandardError), process.WaitForExitAsync());
        return process.ExitCode;
    }

    internal static string BatchFolder(string selected)
    {
        foreach (var folder in new[] { selected, Path.Combine(selected, "batch"), Path.Combine(selected, "sweep", "batch") })
            if (File.Exists(Path.Combine(folder, "status.json")) && File.Exists(Path.Combine(folder, "batch.json"))) return folder;
        throw new InvalidDataException("Choose a batch folder containing status.json and batch.json, or its parent experiment folder.");
    }

    internal static JsonDocument ReadSmall(string path)
    {
        if (new FileInfo(path).Length > 4 * 1024 * 1024) throw new InvalidDataException("Progress metadata is unexpectedly large: " + path);
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        return JsonDocument.Parse(stream);
    }

    internal static BatchProgress ReadProgress(string selected, DateTime nowUtc)
    {
        var folder = BatchFolder(selected);
        var statusFile = Path.Combine(folder, "status.json");
        using var status = ReadSmall(statusFile);
        var data = status.RootElement;
        var state = data.GetProperty("state").GetString() ?? "Unknown";
        var done = data.GetProperty("completedChunks").GetInt32();
        var total = data.GetProperty("totalChunks").GetInt32();
        if (done < 0 || total < done) throw new InvalidDataException("Batch chunk counts are inconsistent. Check its source files.");
        var workers = data.TryGetProperty("activeWorkers", out var active) ? active.GetInt32().ToString("N0") :
            state == "Completed" ? "0" : "Not recorded";
        var runs = "Available when the batch completes";
        var summaryPath = Path.Combine(folder, "summary.json");
        if (state == "Completed" && File.Exists(summaryPath))
        {
            using var summary = ReadSmall(summaryPath);
            using var batch = ReadSmall(Path.Combine(folder, "batch.json"));
            if (summary.RootElement.GetProperty("identity").GetString() != batch.RootElement.GetProperty("identity").GetString())
                throw new InvalidDataException("Summary identity differs from this batch. Check its source files.");
            runs = summary.RootElement.GetProperty("runs").GetInt64().ToString("N0");
        }
        var updated = File.GetLastWriteTimeUtc(statusFile);
        return new(folder, state, done, total, workers, runs, updated, state == "Running" && nowUtc - updated > TimeSpan.FromSeconds(30));
    }

    internal static void Open(string path) => Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
}
