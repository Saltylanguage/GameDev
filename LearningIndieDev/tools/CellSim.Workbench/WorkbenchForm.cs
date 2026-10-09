namespace CellSim.Workbench;

internal sealed class WorkbenchForm : Form
{
    readonly string project;
    readonly ListBox reports = new() { Dock = DockStyle.Fill, HorizontalScrollbar = true, SelectionMode = SelectionMode.MultiExtended, Height = 82, AccessibleName = "Selected source reports" };
    readonly TextBox match = new() { Dock = DockStyle.Fill, PlaceholderText = "All conditions (example: D5-S25 gardeners)", AccessibleName = "Condition filter" };
    readonly TextBox seeds = new() { Dock = DockStyle.Fill, PlaceholderText = "All seeds (example: 60000, 60001)", AccessibleName = "Seed filter" };
    readonly NumericUpDown rows = Number(200, 1, 1000, "Maximum example rows");
    readonly NumericUpDown first = Number(400, 0, int.MaxValue, "First checkpoint tick");
    readonly NumericUpDown second = Number(500, 0, int.MaxValue, "Second checkpoint tick");
    readonly CheckBox detailedObservations = new() { Text = "Include detailed purchase-by-purchase observations", AutoSize = true, AccessibleName = "Detailed observations" };
    readonly TextBox output = new() { Dock = DockStyle.Fill, ReadOnly = true, AccessibleName = "New workbook output path" };
    readonly TextBox log = new() { Dock = DockStyle.Fill, Height = 105, Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true, BackColor = Color.FromArgb(247, 249, 247), BorderStyle = BorderStyle.FixedSingle, AccessibleName = "Export activity log" };
    readonly Label state = TextLabel("Ready. Set up an experiment, review saved progress, or export a workbook.");
    readonly ProgressBar working = new() { Dock = DockStyle.Top, Height = 5, Style = ProgressBarStyle.Marquee, Visible = false };
    readonly Button export = Button("Create Excel worksheet");
    readonly Button openWorkbook = Button("Open workbook");
    readonly Button openOutput = Button("Open output folder");
    readonly CheckBox openWhenDone = new() { Text = "Open workbook when ready", Checked = true, AutoSize = true, Padding = new(6, 7, 0, 0) };
    readonly Panel exportSettings = new() { Dock = DockStyle.Fill, AutoSize = true };
    readonly TextBox batch = new() { Dock = DockStyle.Fill, ReadOnly = true, AccessibleName = "Selected batch folder" };
    readonly Label batchDetails = TextLabel("Choose an experiment or batch folder. Progress refreshes every 5 seconds.");
    readonly ProgressBar batchBar = new() { Dock = DockStyle.Top, Height = 20 };
    readonly System.Windows.Forms.Timer timer = new() { Interval = 5000 };
    bool busy;
    string? lastOutput;
    readonly RunPage runPage;
    bool closeAfterRun;

    internal WorkbenchForm(string project)
    {
        this.project = project;
        AutoScaleDimensions = new(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        Text = "CellSim Workbench";
        Font = new("Segoe UI", 10);
        BackColor = Color.White;
        Size = new(1280, 900);
        MinimumSize = new(1000, 800);
        StartPosition = FormStartPosition.CenterScreen;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new(14, 8, 14, 8) };
        root.ColumnStyles.Add(new(SizeType.Percent, 100));
        root.SizeChanged += (_, _) => state.MaximumSize = new(Math.Max(200, root.ClientSize.Width - 50), 0);
        root.RowStyles.Add(new(SizeType.AutoSize));
        root.RowStyles.Add(new(SizeType.Percent, 100));
        root.RowStyles.Add(new(SizeType.AutoSize));
        var heading = Stack();
        var title = TextLabel("CellSim Workbench");
        title.Font = new("Segoe UI", 16, FontStyle.Bold);
        title.ForeColor = Color.FromArgb(39, 75, 49);
        heading.Controls.Add(title);
        var projectLabel = TextLabel("Project: " + project);
        projectLabel.Font = new("Segoe UI", 9);
        projectLabel.ForeColor = Color.DimGray;
        heading.Controls.Add(projectLabel);
        root.Controls.Add(heading, 0, 0);
        var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new(16, 8), AccessibleName = "Workbench pages" };
        runPage = new RunPage(project, () => !busy, folder => { batch.Text = folder; RefreshBatch(); }, path => { AddSources([path]); tabs.SelectedIndex = 1; });
        tabs.TabPages.Add(runPage);
        tabs.TabPages.Add(ExportPage());
        tabs.TabPages.Add(BatchPage());
        tabs.SelectedIndexChanged += (_, _) => AcceptButton = tabs.SelectedIndex == 1 ? export : null;
        root.Controls.Add(tabs, 0, 1);
        state.Padding = new(0, 10, 0, 0);
        root.Controls.Add(state, 0, 2);
        Controls.Add(root);
        Shown += (_, _) =>
        {
            var area = Screen.FromControl(this).WorkingArea;
            MinimumSize = new(Math.Min((int)(1000 * DeviceDpi / 96f), area.Width), Math.Min((int)(800 * DeviceDpi / 96f), area.Height));
            Size = new(Math.Min((int)(1280 * DeviceDpi / 96f), area.Width), Math.Min((int)(900 * DeviceDpi / 96f), area.Height));
        };
        runPage.BecameIdle += () => { if (closeAfterRun) Close(); };
        openWorkbook.Enabled = openOutput.Enabled = false;
        export.Click += async (_, _) => await ExportWorksheet();
        openWorkbook.Click += (_, _) => OpenSafely(lastOutput!);
        openOutput.Click += (_, _) => OpenSafely(Path.GetDirectoryName(lastOutput!)!);
        FormClosing += (_, e) =>
        {
            if (runPage.IsBusy)
            {
                e.Cancel = true;
                closeAfterRun = true;
                try { runPage.RequestStop(); state.Text = "Waiting for the owned operation to stop safely before closing."; }
                catch (Exception error) { state.Text = "Could not request a safe stop. The window remains open: " + error.Message; }
                return;
            }
            if (!busy) return;
            e.Cancel = true;
            state.Text = "An export is still running. Wait for it to finish before closing the workbench.";
        };
        timer.Tick += (_, _) => RefreshBatch();
        timer.Start();
        FormClosed += (_, _) => timer.Dispose();
    }

    internal static NumericUpDown Number(int value, int minimum, int maximum, string name) => new()
    { Minimum = minimum, Maximum = maximum, Value = value, Width = 105, ThousandsSeparator = true, AccessibleName = name };
    internal static Button Button(string text) => new() { Text = text, AutoSize = true, MinimumSize = new(110, 32), Padding = new(8, 2, 8, 2), AccessibleName = text };
    internal static Label TextLabel(string text) => new() { Text = text, AutoSize = true, Margin = new(3, 3, 3, 6) };
    internal static TableLayoutPanel Stack() => new() { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, GrowStyle = TableLayoutPanelGrowStyle.AddRows };
    internal static FlowLayoutPanel Flow(params Control[] controls)
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true, Margin = new(0, 3, 0, 6) };
        panel.Controls.AddRange(controls);
        return panel;
    }
    static TabPage Page(string title) => new(title) { Padding = new(12), AutoScroll = true, BackColor = Color.White };

    TabPage ExportPage()
    {
        var page = Page("Excel worksheets");
        page.AutoScroll = false;
        exportSettings.AutoSize = false;
        exportSettings.AutoScroll = true;
        var content = Stack();
        content.Controls.Add(TextLabel("1. Choose saved simulation evidence"));
        content.Controls.Add(reports);
        var add = Button("Add reports...");
        var addFolder = Button("Add sweep folder...");
        var remove = Button("Remove selected");
        add.Click += (_, _) =>
        {
            using var picker = new OpenFileDialog { Title = "Select report.json files or a completed sweep.json", Filter = "Simulation evidence (*.json)|*.json", Multiselect = true, InitialDirectory = Path.Combine(project, "artifacts") };
            if (picker.ShowDialog(this) == DialogResult.OK) AddSources(picker.FileNames);
        };
        addFolder.Click += (_, _) =>
        {
            var folder = ChooseFolder("Select a completed sweep or experiment folder");
            if (folder != null) AddSources([folder]);
        };
        remove.Click += (_, _) => { foreach (var item in reports.SelectedItems.Cast<string>().ToArray()) reports.Items.Remove(item); };
        content.Controls.Add(Flow(add, addFolder, remove));
        var customize = new CheckBox { Text = "Customize examples, filters and checkpoint ticks", AutoSize = true, Margin = new(3, 6, 3, 6) };
        content.Controls.Add(customize);
        var fields = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
        fields.ColumnStyles.Add(new(SizeType.Absolute, 145));
        fields.ColumnStyles.Add(new(SizeType.Percent, 100));
        fields.Controls.Add(TextLabel("Conditions"), 0, 0); fields.Controls.Add(match, 1, 0);
        fields.Controls.Add(TextLabel("Seeds"), 0, 1); fields.Controls.Add(seeds, 1, 1);
        fields.Controls.Add(TextLabel("Examples"), 0, 2);
        fields.Controls.Add(Flow(rows, TextLabel("rows     Checkpoints"), first, TextLabel("and"), second), 1, 2);
        fields.Controls.Add(detailedObservations, 1, 3);
        content.Controls.Add(fields);
        fields.Visible = false;
        customize.CheckedChanged += (_, _) => fields.Visible = customize.Checked;
        var hint = TextLabel("The workbook opens on Batch summary: experiment settings, survival counts and percentages for all matching runs.\nRun review shows up to 200 examples. Run data splits those same runs into columns for pivots and charts.\nExamples use the lowest seeds across conditions; they are not a statistical sample.");
        hint.ForeColor = Color.DimGray;
        content.Controls.Add(hint);
        content.Controls.Add(TextLabel("2. Save a new workbook (automatic timestamped filename, or choose Save as...)"));
        var destination = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
        destination.ColumnStyles.Add(new(SizeType.Percent, 100)); destination.ColumnStyles.Add(new(SizeType.AutoSize));
        var chooseOutput = Button("Save as...");
        chooseOutput.Click += (_, _) =>
        {
            using var picker = new SaveFileDialog { Title = "Choose a NEW workbook filename", Filter = "Excel workbook (*.xlsx)|*.xlsx", DefaultExt = "xlsx", FileName = "Simulation review-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".xlsx", InitialDirectory = Directory.Exists(Path.Combine(project, "artifacts", "worksheets")) ? Path.Combine(project, "artifacts", "worksheets") : project };
            if (picker.ShowDialog(this) == DialogResult.OK) output.Text = picker.FileName;
        };
        output.PlaceholderText = "Automatic timestamped file in artifacts/worksheets (or choose Save as...)";
        destination.Controls.Add(output, 0, 0); destination.Controls.Add(chooseOutput, 1, 0);
        content.Controls.Add(destination);
        exportSettings.Controls.Add(content);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
        layout.RowStyles.Add(new(SizeType.Percent, 100));
        layout.RowStyles.Add(new(SizeType.AutoSize));
        layout.RowStyles.Add(new(SizeType.AutoSize));
        layout.RowStyles.Add(new(SizeType.Absolute, 105));
        layout.Controls.Add(exportSettings, 0, 0);
        layout.Controls.Add(Flow(export, openWhenDone, openWorkbook, openOutput), 0, 1);
        layout.Controls.Add(working, 0, 2);
        layout.Controls.Add(log, 0, 3);
        page.Controls.Add(layout);
        return page;
    }

    TabPage BatchPage()
    {
        var page = Page("Batch progress");
        var layout = Stack();
        layout.Controls.Add(TextLabel("Check a saved batch"));
        layout.Controls.Add(batch);
        var browse = Button("Choose batch folder...");
        var refresh = Button("Refresh now");
        var open = Button("Open evidence folder");
        var use = Button("Use for Excel export");
        browse.Click += (_, _) =>
        {
            var folder = ChooseFolder("Select an experiment or batch folder");
            if (folder == null) return;
            try { batch.Text = Tools.BatchFolder(folder); RefreshBatch(); }
            catch (Exception error) { ShowError(error.Message); }
        };
        refresh.Click += (_, _) => RefreshBatch();
        open.Click += (_, _) => { if (Directory.Exists(batch.Text)) OpenSafely(batch.Text); };
        use.Click += (_, _) =>
        {
            if (string.IsNullOrEmpty(batch.Text)) return;
            var sweep = Directory.GetParent(batch.Text)?.FullName;
            if (sweep == null || !File.Exists(Path.Combine(sweep, "sweep.json")))
            { ShowError("This batch has no sweep.json alongside it. Use a legacy report.json or a completed compiled sweep for worksheet export."); return; }
            AddSources([Path.Combine(sweep, "sweep.json")]);
            ((TabControl)page.Parent!).SelectedIndex = 1;
        };
        layout.Controls.Add(Flow(browse, refresh, open, use));
        layout.Controls.Add(batchBar);
        batchDetails.Font = new("Segoe UI", 12);
        batchDetails.Padding = new(0, 16, 0, 16);
        layout.Controls.Add(batchDetails);
        var hint = TextLabel("Progress comes from small saved metadata files and does not read the raw run archive.\nA completed status is a reported state; full source checks happen during worksheet export.\nUse Set up & run for validated new experiments and frozen Workbench resume.");
        hint.ForeColor = Color.DimGray;
        layout.Controls.Add(hint);
        page.Controls.Add(layout);
        return page;
    }

    void AddSources(IEnumerable<string> paths)
    {
        foreach (var path in paths)
            if (!reports.Items.Cast<string>().Contains(path, StringComparer.OrdinalIgnoreCase)) reports.Items.Add(path);
    }

    string? ChooseFolder(string title)
    {
        using var picker = new FolderBrowserDialog { Description = title, UseDescriptionForTitle = true, InitialDirectory = Path.Combine(project, "artifacts") };
        return picker.ShowDialog(this) == DialogResult.OK ? picker.SelectedPath : null;
    }

    async Task ExportWorksheet()
    {
        if (busy) return;
        if (runPage.IsBusy) { ShowError("Wait for the current setup/run operation to finish or stop before exporting."); return; }
        var destination = string.IsNullOrWhiteSpace(output.Text) ? Path.Combine(project, "artifacts", "worksheets", "Simulation review-" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".xlsx") : output.Text;
        var options = new ExportOptions(reports.Items.Cast<string>().ToArray(), destination, match.Text, seeds.Text, (int)rows.Value, (int)first.Value, (int)second.Value, detailedObservations.Checked);
        try { Tools.ValidateExport(options); }
        catch (Exception error) { ShowError(error.Message); return; }
        busy = true;
        export.Enabled = exportSettings.Enabled = openWhenDone.Enabled = openWorkbook.Enabled = openOutput.Enabled = false;
        working.Visible = true;
        log.Clear();
        state.Text = "Exporting... Large batches may spend several minutes verifying the source archive.";
        try
        {
            var code = await Tools.Export(project, options, line =>
            {
                if (log.TextLength > 100000) log.Text = log.Text[^50000..];
                log.AppendText(line + Environment.NewLine);
                log.SelectionStart = log.TextLength;
                log.ScrollToCaret();
            });
            if (code != 0 || !File.Exists(destination)) throw new IOException("Export did not finish. See the activity log for the reason. Your source evidence was not changed.");
            lastOutput = destination;
            output.Clear();
            state.Text = "Worksheet ready. Use Open workbook or Open output folder to review it.";
            if (openWhenDone.Checked) OpenSafely(destination);
        }
        catch (Exception error) { ShowError(error.Message); }
        finally
        {
            busy = false;
            export.Enabled = exportSettings.Enabled = openWhenDone.Enabled = true;
            openWorkbook.Enabled = openOutput.Enabled = lastOutput != null;
            working.Visible = false;
        }
    }

    void RefreshBatch()
    {
        if (string.IsNullOrEmpty(batch.Text)) return;
        try
        {
            var progress = Tools.ReadProgress(batch.Text, DateTime.UtcNow);
            batchBar.Value = progress.TotalChunks == 0 ? 0 : (int)(100L * progress.CompletedChunks / progress.TotalChunks);
            batchDetails.ForeColor = progress.Stale ? Color.DarkOrange : Color.FromArgb(39, 75, 49);
            batchDetails.Text = $"{progress.State}{(progress.Stale ? " — status is stale; worker activity is unconfirmed" : "")}\n\nCompleted chunks: {progress.CompletedChunks:N0} / {progress.TotalChunks:N0}\nReported active workers: {progress.Workers}\nCompleted run total: {progress.Runs}\nStatus updated: {progress.UpdatedUtc.ToLocalTime():g}\nChecked: {DateTime.Now:T}";
        }
        catch (Exception error)
        {
            batchBar.Value = 0;
            batchDetails.ForeColor = Color.Firebrick;
            batchDetails.Text = "Progress unavailable. No files changed.\n" + error.Message + "\nRetrying every 5 seconds.";
        }
    }

    void OpenSafely(string path)
    {
        try { Tools.Open(path); }
        catch (Exception error) { ShowError("Could not open the file or folder. It is still saved on disk. " + error.Message); }
    }
    void ShowError(string message)
    {
        state.Text = message;
        MessageBox.Show(this, message, "CellSim Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
