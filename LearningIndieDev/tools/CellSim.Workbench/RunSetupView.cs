using System.Text.Json;
using static CellSim.Workbench.WorkbenchForm;

namespace CellSim.Workbench;

internal sealed partial class RunPage
{
    readonly ToolTip help = new() { AutoPopDelay = 20000, InitialDelay = 350, ReshowDelay = 100 };
    readonly DataGridView matrix = Grid("Live test matrix");
    readonly DataGridView conditionGrid = Grid("Condition preview");
    readonly DataGridView comparisonGrid = Grid("Comparison values");
    readonly Label total = TextLabel("");
    readonly Label matrixCaption = TextLabel("");
    readonly Label conditionCaption = TextLabel("");
    readonly Label parameterHelp = TextLabel("Choose a setting to see its description and frozen snapshot value.");
    readonly ComboBox parameter = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 285, AccessibleName = "Comparison setting" };
    readonly TextBox values = Input("Comparison value list", "");
    readonly TabControl setupTabs = new() { Dock = DockStyle.Fill, AccessibleName = "Setup sections", Padding = new(9, 7) };
    readonly Label snapshotDetails = TextLabel("Choose a frozen scenario to see its starting rules.");
    bool applying, matrixValid;
    string? selectedStrategy, selectedPurchase;

    static DataGridView Grid(string name) => new()
    {
        Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        BackgroundColor = Color.White, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
        EnableHeadersVisualStyles = false, ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
        ColumnHeadersDefaultCellStyle = new() { BackColor = Color.FromArgb(47, 78, 62), ForeColor = Color.White, WrapMode = DataGridViewTriState.True, Padding = new(4) },
        DefaultCellStyle = new() { Padding = new(4), SelectionBackColor = Color.FromArgb(212, 232, 221), SelectionForeColor = Color.FromArgb(30, 55, 40) },
        AlternatingRowsDefaultCellStyle = new() { BackColor = Color.FromArgb(244, 248, 245) },
        AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, AccessibleName = name, MultiSelect = false
    };
    static Dictionary<string, CheckBox> ChoiceChecks(string[] ids) => ids.ToDictionary(id => id, id => new CheckBox { Text = ChoiceLabel(id), Checked = id == ids[0], AutoSize = true, AccessibleName = id });
    internal static string ChoiceLabel(string id) => id switch
    {
        "skip-all" => "Skip all Mutations", "trailblazer" => "Trailblazer", "warren" => "Warren", "gardeners" => "Gardeners",
        "none" => "No purchases", "late-five" => "Late five", "each-one" => "One each decision", "each-three" => "Three each decision", "each-five" => "Five each decision", "restore-toward-start" => "Restore toward start", _ => id
    };
    internal static string ChoiceDescription(string id) => id switch
    {
        "skip-all" => "Take no Mutations. Provides a comparison with the frozen starting rules.",
        "trailblazer" => "Automatic choices between Faster Movement and Threat Exposure, favoring the lower-level path skill.",
        "warren" => "Automatic choices between Tough Hide and Crowding Tolerance, favoring the lower-level path skill.",
        "gardeners" => "Automatic choices between Efficient Digestion and Seed Dispersal, favoring the lower-level path skill.",
        "none" => "Spend no Field Data on Hares.",
        "late-five" => "Request up to five Hares only after round 5 (before the final round).",
        "each-one" => "Request up to one Hare after each of rounds 1–5.",
        "each-three" => "Request up to three Hares after each of rounds 1–5.",
        "each-five" => "Request up to five Hares after each of rounds 1–5.",
        "restore-toward-start" => "Request up to five Hares toward this condition's starting population at each decision. Request zero when already at or above it.", _ => ""
    };
    Label Description(string text)
    {
        var label = TextLabel(text); label.ForeColor = Color.FromArgb(88, 102, 94); label.MaximumSize = new(480, 0);
        return label;
    }
    void Explain(Control control, string text) { help.SetToolTip(control, text); control.AccessibleDescription = text; }
    TableLayoutPanel Section(string title, string description)
    {
        var tab = new TabPage(title) { BackColor = Color.White, Padding = new(12), AutoScroll = true };
        var fields = Stack(); fields.ColumnStyles.Add(new(SizeType.Percent, 100));
        fields.Controls.Add(Description(description)); tab.Controls.Add(fields); setupTabs.TabPages.Add(tab);
        tab.SizeChanged += (_, _) =>
        {
            foreach (var label in Descendants(fields).OfType<Label>()) label.MaximumSize = new(Math.Max(200, tab.ClientSize.Width - 45), 0);
        };
        return fields;
    }
    void Field(TableLayoutPanel fields, string label, Control control, string description)
    {
        if (control is TextBox text)
        {
            fields.Controls.Add(TextLabel(label)); text.Dock = DockStyle.Top; fields.Controls.Add(text);
        }
        else fields.Controls.Add(Row(label, control));
        fields.Controls.Add(Description(description)); Explain(control, description);
    }
    void BuildSetup()
    {
        var split = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        split.ColumnStyles.Add(new(SizeType.Percent, 53)); split.ColumnStyles.Add(new(SizeType.Percent, 47));
        var left = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        left.ColumnStyles.Add(new(SizeType.Percent, 100)); left.RowStyles.Add(new(SizeType.AutoSize)); left.RowStyles.Add(new(SizeType.Percent, 100));
        var example = Button("Load D5 example"); var load = Button("Load preset..."); var save = Button("Save preset..."); var openRun = Button("Open saved run...");
        example.Click += (_, _) => { try { Apply(RunTools.LoadPreset(Path.Combine(project, "tools", "CellSim.Workbench", "presets", "D5-Gardeners-review.json"))); } catch (Exception error) { Error(error); } };
        load.Click += (_, _) => LoadPreset(); save.Click += (_, _) => SavePreset(); openRun.Click += async (_, _) => await OpenRun();
        Explain(example, "Fill in the saved D5 research example. It starts nothing and replaces the current setup fields.");
        Explain(load, "Load editable settings from a saved JSON preset."); Explain(save, "Save the current settings, including comparisons, for later. This does not run or validate them.");
        Explain(openRun, "Open an existing frozen Workbench experiment for safe resume/recheck or export.");
        left.Controls.Add(Flow(example, load, save, openRun), 0, 0); left.Controls.Add(setupTabs, 0, 1);
        var basics = Section("Scenario", "Set the starting world. Values in Compare values replace the corresponding single setting for this experiment.");
        snapshot.ReadOnly = true; snapshot.Width = 320;
        var browse = Button("Choose snapshot..."); browse.Click += (_, _) => ChooseSnapshot();
        basics.Controls.Add(Flow(browse, snapshot)); basics.Controls.Add(snapshotDetails);
        Explain(snapshot, "Neutral frozen Plant/Hare/Fox scenario exported from Unity. This file and its rules are copied into each validated experiment.");
        basics.Controls.Add(Row("Starting Plants", plants, TextLabel("Hares"), hares, TextLabel("Foxes"), foxes));
        basics.Controls.Add(Description("Placed at tick 0. Each animal/Plant occupies a cell; starting totals must fit every requested map."));
        foreach (var control in new[] { plants, hares, foxes }) Explain(control, "Initial population. Use Compare values to test multiple totals with matched seeds.");
        basics.Controls.Add(Row("Map size", width, TextLabel("×"), height, wrap));
        Explain(width, "Map columns (1–4096)."); Explain(height, "Map rows (1–4096)."); Explain(wrap, "When enabled, movement/perception can cross the map boundary to the opposite side.");
        basics.Controls.Add(Description("Width × height determines available cells and population density. Wrapping connects opposite map edges."));
        Field(basics, "Ticks per round", phase, "Six rounds, with five Mutation/purchase decisions after rounds 1–5. Increasing this changes time between decisions and the final horizon.");
        basics.Controls.Add(Flow(statOverrides)); basics.Controls.Add(Row("Hare vision", vision, TextLabel("Fox energy"), energy));
        basics.Controls.Add(Description("Optional D5 overrides. Leave unchecked to retain snapshot rules. Comparison values replace these when the same stat is selected."));
        Explain(statOverrides, "Override only Hare vision and Fox starting energy; other species rules remain from the snapshot unless added in Compare values.");
        Explain(vision, "Hare perception range in cells, applied only when the override checkbox is enabled."); Explain(energy, "Fox starting energy, applied only when the override checkbox is enabled.");
        var choices = Section("Choices", "Every selected strategy is tested against every selected purchase policy. The matrix updates immediately. These are scripted research policies.");
        choices.Controls.Add(TextLabel("MUTATION STRATEGIES"));
        foreach (var (id, check) in strategies) { choices.Controls.Add(check); choices.Controls.Add(Description(ChoiceDescription(id))); Explain(check, ChoiceDescription(id)); }
        choices.Controls.Add(TextLabel("HARE PURCHASE POLICIES"));
        foreach (var (id, check) in purchases) { choices.Controls.Add(check); choices.Controls.Add(Description(ChoiceDescription(id))); Explain(check, ChoiceDescription(id)); }
        choices.Controls.Add(Description("Purchases are limited by the recorded wallet, price and placement capacity. Requests are not guaranteed additions."));
        var comparisons = Section("Compare values", "Add one value to override a setting, or several to compare. All values are crossed with each other, every strategy, every purchase policy and the same seeds.");
        parameter.Items.AddRange(SetupParameters.All); parameter.SelectedIndexChanged += (_, _) => DescribeParameter(); parameter.SelectedIndex = 0;
        values.Width = 250; values.PlaceholderText = "Example: 20, 30, 40";
        Explain(values, "Enter 1–20 distinct values separated by commas. Use a decimal point, e.g. 0.25, 0.5. Up to eight different settings.");
        comparisons.Controls.Add(parameter); comparisons.Controls.Add(parameterHelp); comparisons.Controls.Add(values);
        var add = Button("Add comparison"); var remove = Button("Remove selected");
        add.Click += (_, _) =>
        {
            try
            {
                var p = (SetupParameter)parameter.SelectedItem!; var axis = new ComparisonAxis(p.Key, values.Text); SetupParameters.Parse(axis);
                if (ReadComparisons().Any(a => a.Key == p.Key)) throw new ArgumentException("That setting is already listed. Edit its values in the table.");
                if (comparisonGrid.Rows.Count >= 8) throw new ArgumentException("Use up to eight comparison settings.");
                comparisonGrid.Rows.Add(p.Key, values.Text); Changed();
            }
            catch (Exception error) { Error(error); }
        };
        remove.Click += (_, _) => { if (comparisonGrid.CurrentRow != null) { comparisonGrid.Rows.Remove(comparisonGrid.CurrentRow); Changed(); } };
        comparisons.Controls.Add(Flow(add, remove));
        comparisonGrid.ReadOnly = false; comparisonGrid.Height = 180; comparisonGrid.Dock = DockStyle.Top;
        comparisonGrid.Columns.Add(new DataGridViewComboBoxColumn { Name = "Setting", HeaderText = "Setting", DataSource = SetupParameters.All, DisplayMember = "Label", ValueMember = "Key", ReadOnly = true, FillWeight = 58 });
        comparisonGrid.Columns.Add("Values", "Values (comma separated)"); comparisonGrid.Columns[1].FillWeight = 42;
        comparisonGrid.CellValueChanged += (_, _) => Changed(); comparisonGrid.RowsRemoved += (_, _) => Changed();
        comparisons.Controls.Add(comparisonGrid);
        comparisons.Controls.Add(Description("Example: Hare population 20, 30 × Fox energy 120, 160 creates four variants per strategy/purchase pairing. One value adds no extra conditions. Double-click a values cell to edit it."));
        var execution = Section("Run settings", "Describe the experiment, choose matched seeds, and set how this computer executes the batch. Nothing runs until Validate setup succeeds and you click Run.");
        name.Width = owner.Width = question.Width = hypothesis.Width = success.Width = failure.Width = 330;
        foreach (var text in new[] { question, hypothesis, success, failure }) { text.Multiline = true; text.AutoSize = false; text.Height = 60; text.ScrollBars = ScrollBars.Vertical; }
        Field(execution, "Name", name, "A readable name for the saved experiment and workbook.");
        Field(execution, "Owner", owner, "Person responsible for reviewing the result.");
        Field(execution, "Question", question, "What are you trying to learn from this comparison?");
        Field(execution, "Expectation", hypothesis, "What do you expect to happen, before seeing results?");
        Field(execution, "Success", success, "What evidence would make this experiment useful or support your expectation?");
        Field(execution, "Failure", failure, "What evidence would contradict the expectation or make the run unusable?");
        Field(execution, "First seed", seedStart, "Beginning of the consecutive seed range. Each condition uses the same seeds for comparison.");
        Field(execution, "Seeds per condition", seedCount, "Repeated runs for every condition. More seeds multiply the total work.");
        Field(execution, "Workers", workers, "Maximum concurrent worker processes (1–64). More workers use more CPU and memory; they do not add test conditions.");
        Field(execution, "Seeds per chunk", chunks, "Runs assigned to one work unit. Completed chunks are retained on stop; unfinished chunks may run again on resume.");
        Field(execution, "Timeout (seconds)", timeout, "Time allowed for a chunk before reporting a failure. Larger worlds or chunks may need longer.");
        var right = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 7, Padding = new(12, 0, 0, 0), BackColor = Color.FromArgb(244, 248, 245) };
        right.ColumnStyles.Add(new(SizeType.Percent, 100));
        right.RowStyles.Add(new(SizeType.AutoSize)); right.RowStyles.Add(new(SizeType.AutoSize)); right.RowStyles.Add(new(SizeType.AutoSize));
        right.RowStyles.Add(new(SizeType.Percent, 50)); right.RowStyles.Add(new(SizeType.AutoSize)); right.RowStyles.Add(new(SizeType.AutoSize)); right.RowStyles.Add(new(SizeType.Percent, 50));
        var title = TextLabel("YOUR TEST MATRIX"); title.Font = new("Segoe UI", 13, FontStyle.Bold);
        total.Font = new("Segoe UI", 18, FontStyle.Bold); total.ForeColor = Color.FromArgb(39, 75, 49);
        matrixCaption.Font = conditionCaption.Font = new("Segoe UI", 9);
        right.Controls.Add(title, 0, 0); right.Controls.Add(total, 0, 1); right.Controls.Add(matrixCaption, 0, 2); right.Controls.Add(matrix, 0, 3);
        var showAll = Button("Show all combinations"); showAll.Click += (_, _) => { selectedStrategy = selectedPurchase = null; if (matrixValid) RefreshConditions(Options()); };
        right.Controls.Add(Flow(showAll), 0, 4); right.Controls.Add(conditionCaption, 0, 5); right.Controls.Add(conditionGrid, 0, 6);
        conditionGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        conditionGrid.SizeChanged += (_, _) => { if (conditionGrid.Columns.Count > 0) conditionGrid.AutoResizeRows(); };
        matrix.CellClick += (_, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1 || !matrixValid) return;
            selectedStrategy = (string)matrix.Rows[e.RowIndex].Tag!; selectedPurchase = (string)matrix.Columns[e.ColumnIndex].Tag!; RefreshConditions(Options());
        };
        right.SizeChanged += (_, _) => { matrixCaption.MaximumSize = conditionCaption.MaximumSize = new(Math.Max(200, right.Width - 30), 0); };
        Explain(matrix, "Each cell is one strategy/purchase pairing. Its run count includes every comparison value and seed. Click a cell to inspect its combinations below.");
        split.Controls.Add(left, 0, 0); split.Controls.Add(right, 1, 0); settings.Controls.Add(split);
        run.BackColor = Color.FromArgb(47, 100, 68); run.ForeColor = Color.White; run.FlatStyle = FlatStyle.Flat;
        log.BackColor = Color.FromArgb(244, 248, 245); log.BorderStyle = BorderStyle.FixedSingle;
        Explain(validate, "Check every condition with the existing compiler and pinned runner, then freeze the exact inputs. No simulation workers run yet.");
        Explain(run, "Execute the validated experiment. Editing any setting requires validation again."); Explain(stop, "Stop only this Workbench's coordinator and workers, retaining completed chunks.");
        Explain(resume, "Continue/recheck the same frozen saved experiment. Changed inputs are refused."); Explain(export, "Create the standard three-sheet workbook from this completed run's saved evidence.");
    }
    ComparisonAxis[] ReadComparisons() => comparisonGrid.Rows.Cast<DataGridViewRow>().Select(row => new ComparisonAxis(Convert.ToString(row.Cells[0].Value) ?? "", Convert.ToString(row.Cells[1].Value) ?? "")).ToArray();
    void RefreshSnapshotDetails()
    {
        snapshotDetails.Text = File.Exists(snapshot.Text) ? "Frozen source: " + Path.GetFileName(Path.GetDirectoryName(snapshot.Text)) : "Choose a frozen scenario to inspect its starting rules.";
        DescribeParameter();
    }
    void DescribeParameter()
    {
        if (parameter.SelectedItem is not SetupParameter p) return;
        var current = "Choose a snapshot to inspect its value.";
        if (File.Exists(snapshot.Text))
        {
            try
            {
                using var document = Tools.ReadSmall(snapshot.Text); var data = document.RootElement;
                JsonElement value;
                if (p.Argument is "-gridWidth" or "-gridHeight") value = data.GetProperty(p.SnapshotField);
                else
                {
                    var species = p.Component.Split(':')[0]; var entry = data.GetProperty("species").EnumerateArray().Single(s => s.GetProperty("id").GetString() == species);
                    value = p.Argument == "-startingPopulations" ? entry.GetProperty("population") : entry.GetProperty("rules").GetProperty(p.SnapshotField);
                }
                current = "Frozen snapshot value: " + (value.TryGetDecimal(out var number) ? number.ToString("0.######") : value.ToString()) + ".";
            }
            catch (Exception error) when (error is IOException or InvalidOperationException or KeyNotFoundException or JsonException) { current = "Snapshot value unavailable: " + error.Message; }
        }
        parameterHelp.Text = p.Description + "\n" + current + " Comparison values replace the single setting for this experiment.";
        Explain(parameter, p.Description);
    }
    void RefreshMatrix(SetupOptions options)
    {
        matrixValid = false; matrix.Rows.Clear(); matrix.Columns.Clear(); conditionGrid.Rows.Clear();
        try
        {
            var variants = RunTools.VariantCount(options); var count = RunTools.RunCount(options);
            var conditions = variants * options.Strategies.Length * options.Purchases.Length;
            if (conditions > 10000 || count > 1000000) throw new ArgumentException("Reduce the matrix to at most 10,000 conditions and 1,000,000 runs.");
            total.Text = $"{count:N0} runs";
            matrixCaption.Text = $"{conditions:N0} conditions × {options.SeedCount:N0} matched seeds\n{variants:N0} variants per pairing · up to {6L * options.PhaseTicks:N0} ticks\nClick a cell to filter the list below.";
            preview.Text = $"{options.Strategies.Length} strategies × {options.Purchases.Length} purchase policies × {variants:N0} setting variants × {options.SeedCount:N0} seeds = {count:N0} runs";
            matrix.Columns.Add("Strategy", "Strategy"); matrix.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None; matrix.Columns[0].Width = (int)(125 * DeviceDpi / 96f); matrix.Columns[0].Frozen = true;
            foreach (var id in options.Purchases)
            {
                var label = id switch { "none" => "None", "late-five" => "Late 5", "each-one" => "Each 1", "each-three" => "Each 3", "each-five" => "Each 5", "restore-toward-start" => "Restore", _ => ChoiceLabel(id) };
                var column = matrix.Columns[matrix.Columns.Add(id, label)]; column.Tag = id; column.MinimumWidth = 72; column.ToolTipText = ChoiceDescription(id);
            }
            foreach (var id in options.Strategies)
            {
                var index = matrix.Rows.Add(new[] { ChoiceLabel(id) }.Concat(options.Purchases.Select(_ => $"{variants * options.SeedCount:N0} runs")).Cast<object>().ToArray());
                matrix.Rows[index].Tag = id;
            }
            matrix.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            matrixValid = conditions > 0;
            if (!matrixValid) matrixCaption.Text = "Select at least one strategy and one purchase policy on Choices.";
            if (!options.Strategies.Contains(selectedStrategy)) selectedStrategy = null;
            if (!options.Purchases.Contains(selectedPurchase)) selectedPurchase = null;
            RefreshConditions(options);
        }
        catch (Exception error) when (error is ArgumentException or OverflowException)
        {
            total.Text = "Check values"; matrixCaption.Text = error.Message; preview.Text = "Comparison values need attention."; conditionCaption.Text = "No valid preview until these values are fixed.";
        }
        vision.Enabled = energy.Enabled = options.OverrideStats;
    }
    void RefreshConditions(SetupOptions options)
    {
        conditionGrid.Rows.Clear(); conditionGrid.Columns.Clear();
        conditionGrid.Columns.Add("Strategy", "Strategy"); conditionGrid.Columns.Add("Purchase", "Purchase");
        foreach (var axis in options.Comparisons) conditionGrid.Columns.Add(axis.Key, SetupParameters.Find(axis.Key).Label);
        if (options.Comparisons.Length == 0) conditionGrid.Columns.Add("Values", "Scenario settings");
        foreach (DataGridViewColumn column in conditionGrid.Columns) column.MinimumWidth = (int)(95 * DeviceDpi / 96f);
        var paths = options.Strategies.Where(s => selectedStrategy == null || s == selectedStrategy).ToArray();
        var policies = options.Purchases.Where(s => selectedPurchase == null || s == selectedPurchase).ToArray();
        var count = RunTools.VariantCount(options) * paths.Length * policies.Length;
        IEnumerable<string[]> Combinations(int index, string[] prior)
        {
            if (index == options.Comparisons.Length) { yield return prior; yield break; }
            var axis = options.Comparisons[index];
            foreach (var value in SetupParameters.Parse(axis))
                foreach (var combination in Combinations(index + 1, prior.Append(SetupParameters.Value(value)).ToArray())) yield return combination;
        }
        var combinations = from policy in policies from combination in Combinations(0, []) from strategy in paths select (strategy, policy, combination);
        foreach (var (strategy, policy, combination) in combinations.Take(200))
            conditionGrid.Rows.Add(new[] { ChoiceLabel(strategy), ChoiceLabel(policy) }.Concat(combination.Length == 0 ? ["Single scenario settings"] : combination).Cast<object>().ToArray());
        conditionGrid.AutoResizeRows();
        conditionCaption.Text = $"{(selectedStrategy == null && selectedPurchase == null ? "All combinations" : "Selected pairing")} · {Math.Min(200, count):N0} of {count:N0} conditions shown\nEach uses seeds {options.SeedStart:N0}–{(long)options.SeedStart + options.SeedCount - 1:N0}.";
    }
}
