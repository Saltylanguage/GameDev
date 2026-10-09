using System.Text.Json;

namespace CellSim.Workbench;

internal static class Checks
{
    internal static void PumpUi()
    {
        Application.DoEvents();
        // Self-checks pump without Application.Run; restore the UI context after each temporary loop.
        SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
    }
    internal static void Run(string resultPath, string? actualReport)
    {
        var passed = new List<string>();
        var root = Path.Combine(Path.GetTempPath(), "cellsim-workbench-check-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        void Check(string name, Action check) { check(); passed.Add(name); }
        void Require(bool value) { if (!value) throw new Exception("Self-check failed."); }
        void Reject(Action action)
        {
            try { action(); }
            catch (Exception error) when (error is ArgumentException or IOException or InvalidDataException) { return; }
            throw new Exception("Invalid input was accepted.");
        }
        try
        {
            var project = Path.Combine(root, "Project O'Brien $value `tick");
            Directory.CreateDirectory(Path.Combine(project, "tools"));
            var script = Path.Combine(project, "tools", "Export-CellSimWorksheet.ps1");
            File.WriteAllText(script, "param([string[]]$ReportPath,[string]$OutputPath,[string]$ProjectPath,[int]$MaxRuns,[int[]]$Checkpoints,[string]$Match,[int[]]$Seeds,[switch]$DetailedObservations)\n[Console]::OutputEncoding=[Text.UTF8Encoding]::new($false)\n@{reports=$ReportPath;match=$Match;seeds=$Seeds;ticks=$Checkpoints;rows=$MaxRuns;detailed=[bool]$DetailedObservations}|ConvertTo-Json -Depth 5|Set-Content -LiteralPath $OutputPath -Encoding UTF8\nWrite-Output 'Readable progress: Hares / Foxes / Plants'\nexit 0\n");
            var report = Path.Combine(root, "report O'Brien $(throw 'unsafe') ` 日本語.json");
            File.WriteAllText(report, "{}");
            var options = new ExportOptions([report], Path.Combine(root, "new workbook.xlsx"), "D5 O'Brien $(throw 'unsafe') ` 日本語", "60000, 60001 60000", 24, 400, 500);
            Check("Project discovery and invalid project refusal", () =>
            {
                Require(Tools.FindProject(Path.Combine(project, "tools")) == project);
                Tools.ValidateProject(project); Reject(() => Tools.ValidateProject(root));
            });
            Check("Seed parsing, deduplication, empty and invalid values", () =>
            {
                Require(Tools.ParseSeeds(options.Seeds).SequenceEqual([60000, 60001]));
                Require(Tools.ParseSeeds(" ").Length == 0);
                foreach (var bad in new[] { "-1", "1.5", "1-3", "2147483648", "hello" }) Reject(() => Tools.ParseSeeds(bad));
            });
            Check("Required evidence, row limits, checkpoint ordering and extension", () =>
            {
                Reject(() => Tools.ValidateExport(options with { Reports = [] }));
                Reject(() => Tools.ValidateExport(options with { Reports = ["missing.json"] }));
                Reject(() => Tools.ValidateExport(options with { Rows = 1001 }));
                Reject(() => Tools.ValidateExport(options with { SecondTick = 400 }));
                Reject(() => Tools.ValidateExport(options with { Output = Path.Combine(root, "wrong.csv") }));
                Reject(() => Tools.ValidateExport(options with { Output = "relative.xlsx" }));
            });
            Check("Actual PowerShell process preserves arrays, quoting and progress", () =>
            {
                var lines = new List<string>();
                Require(Tools.Export(project, options, lines.Add).GetAwaiter().GetResult() == 0);
                using var captured = JsonDocument.Parse(File.ReadAllText(options.Output).TrimStart('\uFEFF'));
                Require(captured.RootElement.GetProperty("reports")[0].GetString() == report);
                Require(captured.RootElement.GetProperty("match").GetString() == options.Match);
                Require(captured.RootElement.GetProperty("seeds").EnumerateArray().Select(x => x.GetInt32()).SequenceEqual([60000, 60001]));
                Require(captured.RootElement.GetProperty("ticks")[1].GetInt32() == 500);
                Require(captured.RootElement.GetProperty("rows").GetInt32() == 24);
                Require(!captured.RootElement.GetProperty("detailed").GetBoolean());
                Require(lines.Any(line => line.Contains("Readable progress")));
            });
            Check("Detailed observations is an explicit export option", () =>
            {
                var detailed = options with { Output = Path.Combine(root, "detailed.xlsx"), DetailedObservations = true };
                Require(Tools.Export(project, detailed, _ => { }).GetAwaiter().GetResult() == 0);
                using var captured = JsonDocument.Parse(File.ReadAllText(detailed.Output).TrimStart('\uFEFF'));
                Require(captured.RootElement.GetProperty("detailed").GetBoolean());
            });
            Check("Existing workbook protection", () => Reject(() => Tools.ValidateExport(options)));
            Check("Child failures and stderr stay visible", () =>
            {
                File.WriteAllText(script, "[Console]::Error.WriteLine('Fixture failure'); exit 7");
                var lines = new List<string>();
                Require(Tools.Export(project, options with { Output = Path.Combine(root, "failed.xlsx") }, lines.Add).GetAwaiter().GetResult() == 7);
                Require(lines.Any(line => line.Contains("Fixture failure")));
            });
            var folder = Path.Combine(root, "experiment", "sweep", "batch");
            Directory.CreateDirectory(folder);
            void Write(string name, object data) => File.WriteAllText(Path.Combine(folder, name), JsonSerializer.Serialize(data));
            Write("batch.json", new { identity = "test" });
            Write("status.json", new { state = "Running", completedChunks = 1, totalChunks = 2, activeWorkers = 3 });
            Check("Batch folder and parent experiment discovery", () =>
            {
                Require(Tools.BatchFolder(Path.Combine(root, "experiment")) == folder);
                Require(Tools.BatchFolder(Directory.GetParent(folder)!.FullName) == folder);
                Reject(() => Tools.BatchFolder(root));
            });
            Check("Running progress and stale status", () =>
            {
                var progress = Tools.ReadProgress(folder, DateTime.UtcNow);
                Require(progress.State == "Running" && progress.CompletedChunks == 1 && progress.Workers == "3" && !progress.Stale);
                File.SetLastWriteTimeUtc(Path.Combine(folder, "status.json"), DateTime.UtcNow.AddMinutes(-2));
                Require(Tools.ReadProgress(folder, DateTime.UtcNow).Stale);
            });
            Check("Completed totals, identity and corrupt progress refusal", () =>
            {
                Write("status.json", new { state = "Completed", completedChunks = 2, totalChunks = 2 });
                Write("summary.json", new { identity = "test", runs = 75 });
                var progress = Tools.ReadProgress(folder, DateTime.UtcNow);
                Require(progress.Workers == "0" && progress.Runs == "75" && !progress.Stale);
                Write("summary.json", new { identity = "foreign", runs = 75 });
                Reject(() => Tools.ReadProgress(folder, DateTime.UtcNow));
                Write("status.json", new { state = "Running", completedChunks = 3, totalChunks = 2 });
                Reject(() => Tools.ReadProgress(folder, DateTime.UtcNow));
            });
            Check("Native form renders both tabs", () =>
            {
                var actualProject = Tools.FindProject(AppContext.BaseDirectory) ?? throw new IOException("Run the check from the project build folder.");
                using var form = new WorkbenchForm(actualProject);
                form.ShowInTaskbar = false;
                form.Opacity = 0;
                form.Show();
                Control.CheckForIllegalCrossThreadCalls = true;
                PumpUi();
                using (var initial = new Bitmap(form.Width, form.Height))
                { form.DrawToBitmap(initial, new Rectangle(Point.Empty, form.Size)); initial.Save(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(resultPath))!, "workbench-setup-initial.png")); }
                var tabs = Find<TabControl>(form).Single(t => t.AccessibleName == "Workbench pages");
                tabs.SelectedIndex = 1;
                PumpUi();
                if (actualReport != null) Check("Native export button, async completion and close guard with real evidence", () =>
                {
                    var actualOutput = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(resultPath))!, "workbench-integration-" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".xlsx");
                    Find<ListBox>(form).Single(control => control.AccessibleName == "Selected source reports").Items.Add(Path.GetFullPath(actualReport));
                    Find<TextBox>(form).Single(control => control.AccessibleName == "New workbook output path").Text = actualOutput;
                    Find<CheckBox>(form).Single(control => control.Text == "Open workbook when ready").Checked = false;
                    Require(!Find<CheckBox>(form).Single(control => control.AccessibleName == "Detailed observations").Checked);
                    var button = Find<Button>(form).Single(control => control.Text == "Create Excel worksheet");
                    button.PerformClick();
                    Require(!button.Enabled);
                    form.Close();
                    Require(!form.IsDisposed);
                    var deadline = DateTime.UtcNow.AddSeconds(60);
                    while (!button.Enabled && DateTime.UtcNow < deadline) { PumpUi(); Thread.Sleep(10); }
                    Require(button.Enabled && new FileInfo(actualOutput).Length > 1000);
                    var activity = Find<TextBox>(form).Single(control => control.AccessibleName == "Export activity log").Text;
                    Require(activity.Contains("Worksheet ready:") && !activity.Contains("<Objs"));
                    File.WriteAllText(Path.ChangeExtension(actualOutput, ".log"), activity);
                });
                form.PerformLayout();
                using var bitmap = new Bitmap(form.Width, form.Height);
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
                bitmap.Save(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(resultPath))!, "workbench-excel.png"));
                tabs.SelectedIndex = 2;
                PumpUi();
                Write("status.json", new { state = "Completed", completedChunks = 2, totalChunks = 2 });
                Write("summary.json", new { identity = "test", runs = 75 });
                Find<TextBox>(form).Single(control => control.AccessibleName == "Selected batch folder").Text = folder;
                Find<Button>(form).Single(control => control.Text == "Refresh now").PerformClick();
                Require(Find<Label>(form).Any(control => control.Text.Contains("Completed run total: 75")));
                form.PerformLayout();
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
                bitmap.Save(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(resultPath))!, "workbench-progress.png"));
            });
            Check("Interactive matrix updates immediately and both previews remain visible", () =>
            {
                var actualProject = Tools.FindProject(AppContext.BaseDirectory)!;
                using var form = new WorkbenchForm(actualProject) { Opacity = 0, ShowInTaskbar = false };
                form.Show(); PumpUi();
                var page = Find<RunPage>(form).Single();
                page.Apply(RunTools.LoadPreset(Path.Combine(actualProject, "tools", "CellSim.Workbench", "presets", "D5-Gardeners-review.json")) with
                { Strategies = RunTools.StrategyIds, Comparisons = [new("hare-population", "20,30"), new("fox:energy.starting", "120,160")] });
                PumpUi();
                var matrix = Find<DataGridView>(page).Single(g => g.AccessibleName == "Live test matrix");
                var conditions = Find<DataGridView>(page).Single(g => g.AccessibleName == "Condition preview");
                Require(matrix.RowCount == 4 && matrix.ColumnCount == 7 && Convert.ToString(matrix[1,0].Value) == "16 runs");
                Require(conditions.RowCount == 96 && matrix.Height >= 90 && conditions.Height >= 100);
                typeof(DataGridView).GetMethod("OnCellClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(matrix, [new DataGridViewCellEventArgs(1, 0)]);
                Require(conditions.RowCount == 4);
                Find<Button>(page).Single(b => b.Text == "Show all combinations").PerformClick(); Require(conditions.RowCount == 96);
                var sections = Find<TabControl>(page).Single(t => t.AccessibleName == "Setup sections");
                for (var section = 0; section < 4; section++)
                {
                    sections.SelectedIndex = section; PumpUi();
                    using var picture = new Bitmap(form.Width, form.Height); form.DrawToBitmap(picture, new Rectangle(Point.Empty, form.Size));
                    picture.Save(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(resultPath))!, $"workbench-matrix-final-{section}.png"));
                }
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(resultPath))!, "matrix-layout.json"), JsonSerializer.Serialize(new { dpi = form.DeviceDpi, width = form.Width, height = form.Height, matrixHeight = matrix.Height, conditionsHeight = conditions.Height }));
                form.Size = form.MinimumSize; PumpUi();
                Require(matrix.Height >= 70 && conditions.Height >= 80);
                sections.SelectedIndex = 2; PumpUi();
                using var compact = new Bitmap(form.Width, form.Height); form.DrawToBitmap(compact, new Rectangle(Point.Empty, form.Size));
                compact.Save(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(resultPath))!, "workbench-matrix-compact.png"));
            });
            File.WriteAllText(resultPath, JsonSerializer.Serialize(new { status = "Passed", checks = passed }, new JsonSerializerOptions { WriteIndented = true }));
        }
        finally { Directory.Delete(root, true); }
    }

    static IEnumerable<T> Find<T>(Control parent) where T : Control
    {
        foreach (Control child in parent.Controls)
        {
            if (child is T found) yield return found;
            foreach (var nested in Find<T>(child)) yield return nested;
        }
    }
}
