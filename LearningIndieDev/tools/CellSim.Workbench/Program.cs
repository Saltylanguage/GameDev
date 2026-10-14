namespace CellSim.Workbench;

internal static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        try
        {
            if (args.Length == 3 && args[0] == "--run-check")
            {
                RunChecks.Run(args[1], args[2]);
                return 0;
            }
            if (args.Length is 2 or 3 && args[0] == "--self-test")
            {
                Checks.Run(args[1], args.Length == 3 ? args[2] : null);
                return 0;
            }
            var project = args.Length == 2 && args[0] == "--project" ? args[1] : Tools.FindProject(AppContext.BaseDirectory);
            if (project == null)
            {
                using var picker = new FolderBrowserDialog { Description = "Select your LearningIndieDev project folder", UseDescriptionForTitle = true };
                if (picker.ShowDialog() != DialogResult.OK) return 0;
                project = picker.SelectedPath;
            }
            Tools.ValidateProject(project);
            Application.Run(new WorkbenchForm(Path.GetFullPath(project)));
            return 0;
        }
        catch (Exception error)
        {
            if (args.Length == 3 && args[0] == "--run-check") File.WriteAllText(args[2], error.ToString());
            else if (args.Length is 2 or 3 && args[0] == "--self-test") File.WriteAllText(args[1], error.ToString());
            else MessageBox.Show(error.Message, "CellSim Workbench could not start", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 1;
        }
    }
}
