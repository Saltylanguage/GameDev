using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Noesis;
using UnityEngine;

namespace SaltyGame
{
    public sealed class VM_GalapagOS_Desktop : MonoBehaviour, INotifyPropertyChanged
    {
        const string VolumePreference = "GalapagOS.MasterVolume";
        const string MutePreference = "GalapagOS.Muted";
        const string ReducedMotionPreference = "GalapagOS.ReducedMotion";
        [SerializeField] GenomeCatalogProvider genomeCatalogProvider;
        [SerializeField] string genomeSpeciesId = "rabbit";
        Helper_ProfileSession profileSession;
        bool permanentGenomeEconomy;
        public GenomeCatalogSnapshot GenomeCatalog => genomeCatalogProvider == null
            ? GenomeCatalogSnapshot.Empty : genomeCatalogProvider.Snapshot;

        public DelegateCommand OpenDesktopIconCommand { get; private set; }
        public DelegateCommand CloseDesktopAppCommand { get; private set; }
        public DelegateCommand CloseLabWindowCommand { get; private set; }
        public DelegateCommand ToggleStartMenuCommand { get; private set; }
        public DelegateCommand CloseStartMenuCommand { get; private set; }
        public DelegateCommand ToggleVolumeCommand { get; private set; }
        public DelegateCommand ToggleNotificationsCommand { get; private set; }
        public DelegateCommand ToggleGameSpeedCommand { get; private set; }
        public DelegateCommand VolumeDownCommand { get; private set; }
        public DelegateCommand VolumeUpCommand { get; private set; }
        public DelegateCommand ToggleMuteCommand { get; private set; }
        public DelegateCommand ToggleReducedMotionCommand { get; private set; }
        public DelegateCommand SetGameSpeedCommand { get; private set; }
        public DelegateCommand SelectPlannerScenarioCommand { get; private set; }
        public DelegateCommand SelectPlannerSpeciesCommand { get; private set; }
        public DelegateCommand ContinuePlannerCommand { get; private set; }
        public DelegateCommand BackPlannerCommand { get; private set; }
        public DelegateCommand LaunchPlannerCommand { get; private set; }

        public string DesktopTitle => "GALAPAGOS DESKTOP";
        public string WindowTitle => "GALAPAGOS TEST WINDOW";
        public string StatusText => "VIEW MODEL DATA CONTEXT RESOLVED";
        public string ViewModelType => nameof(VM_GalapagOS_Desktop);
        public string DateText => "JUNE 14 · DAY 03";
        public string ClockText => "10:32 AM";
        public string DayPhaseText => "DAYLIGHT";
        public string NotificationCountText => "3";
        public string VolumeText => muted ? "MUTED" : $"{Math.Round(volume * 100f):0}%";
        public string ReducedMotionText => ReducedMotionEnabled ? "ON" : "OFF";
        public static bool ReducedMotionEnabled => PlayerPrefs.GetInt(ReducedMotionPreference, 0) == 1;
        public static float SavedAudioVolume => PlayerPrefs.GetInt(MutePreference, 0) == 1
            ? 0f : ReadSavedVolume();

        static float ReadSavedVolume()
        {
            var saved = PlayerPrefs.GetFloat(VolumePreference, 0.8f);
            return float.IsNaN(saved) || float.IsInfinity(saved) ? 0.8f : Mathf.Clamp01(saved);
        }
        public string GameSpeedText => $"{gameSpeed:0.#}x";
        public string ActiveDesktopAppTitle => activeDesktopAppTitle ?? "GALAPAGOS APP";
        public string ActiveDesktopAppDescription => activeDesktopAppDescription ?? "Select a GalapagOS application to begin.";
        public ObservableCollection<GalapagOSDesktopWindow> OpenDesktopWindows => openDesktopWindows;

        public Visibility StartMenuVisibility => startMenuOpen ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LabWindowVisibility => labWindowOpen ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DesktopAppVisibility => desktopAppOpen ? Visibility.Visible : Visibility.Collapsed;
        public Visibility GenericDesktopAppVisibility => activeDesktopAppTitle == "Settings" || activeDesktopAppTitle == "Gene Lab"
            ? Visibility.Collapsed
            : Visibility.Visible;
        public Visibility SettingsSurfaceVisibility => activeDesktopAppTitle == "Settings"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility GeneLabSurfaceVisibility => activeDesktopAppTitle == "Gene Lab"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility VolumePanelVisibility => volumePanelOpen ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NotificationsPanelVisibility => notificationsPanelOpen ? Visibility.Visible : Visibility.Collapsed;
        public Visibility GameSpeedPanelVisibility => gameSpeedPanelOpen ? Visibility.Visible : Visibility.Collapsed;
        public string PlannerScenarioId => plannerScenarioId;
        public string PlannerScenarioName => plannerScenarioId == "FiveSpeciesSandbox" ? "FIVE SPECIES SANDBOX" : "FOREST EDGE";
        public string PlannerPlayerSpeciesId => plannerPlayerSpeciesId;
        public string PlannerPlayerSpeciesName => SpeciesDisplayName(plannerPlayerSpeciesId);
        public string PlannerScenarioSummary => plannerScenarioId == "FiveSpeciesSandbox"
            ? "A shared test ecology with Rabbit, Fox, Owl, Deer, and Wolf."
            : "The current Rabbit expedition route, with Rabbit and Fox available.";
        public string PlannerSelectedScenarioText => "Selected: " + PlannerScenarioName + " · " + PlannerScenarioSummary;
        public string PlannerSpeciesAvailableText => "Available in " + PlannerScenarioName;
        public string PlannerPlayerSpeciesSummary => plannerPlayerSpeciesId == "rabbit"
            ? "Herbivore · feeds on plants"
            : plannerPlayerSpeciesId == "deer" ? "Herbivore · feeds on plants"
            : "Carnivore · choose a species to control";
        public string PlannerSelectedSpeciesText => "Selected: " + PlannerPlayerSpeciesName + " · " + PlannerPlayerSpeciesSummary;
        public string PlannerOptionsTitleText => PlannerPlayerSpeciesName + " · " + PlannerScenarioName;
        public Visibility PlannerScenarioStepVisibility => plannerStep == 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility PlannerSpeciesStepVisibility => plannerStep == 1 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility PlannerOptionsStepVisibility => plannerStep == 2 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility PlannerContinueVisibility => plannerStep < 2 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility PlannerLaunchVisibility => plannerStep == 2 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility PlannerOwlVisibility => plannerScenarioId == "FiveSpeciesSandbox" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility PlannerDeerVisibility => PlannerOwlVisibility;
        public Visibility PlannerWolfVisibility => PlannerOwlVisibility;
        public string PlannerStepLabel => plannerStep == 0 ? "1 / 3 · SCENARIO" : plannerStep == 1 ? "2 / 3 · PLAYER SPECIES" : "3 / 3 · TEST OPTIONS";
        public bool UseTestMutation
        {
            get => useTestMutation;
            set
            {
                if (useTestMutation == value) return;
                useTestMutation = value;
                OnPropertyChanged();
            }
        }
        public bool UseTestGenome
        {
            get => useTestGenome;
            set
            {
                if (useTestGenome == value) return;
                useTestGenome = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void BindSimulationLauncher(Action<bool> launch)
        {
            simulationLauncher = launch;
        }

        public void BindProfileSession(Helper_ProfileSession session, bool permanentEconomy = false)
        {
            if (profileSession != null)
                profileSession.SnapshotChanged -= HandleProfileSnapshotChanged;
            profileSession = session;
            permanentGenomeEconomy = permanentEconomy;
            if (profileSession != null)
                profileSession.SnapshotChanged += HandleProfileSnapshotChanged;
            HandleProfileSnapshotChanged(profileSession?.Current);
        }

        void OnDestroy()
        {
            if (profileSession != null)
                profileSession.SnapshotChanged -= HandleProfileSnapshotChanged;
        }

        void Awake()
        {
            OpenDesktopIconCommand = new DelegateCommand(OpenDesktopIcon);
            CloseDesktopAppCommand = new DelegateCommand(CloseDesktopApp);
            CloseLabWindowCommand = new DelegateCommand(CloseLabWindow);
            ToggleStartMenuCommand = new DelegateCommand(ToggleStartMenu);
            CloseStartMenuCommand = new DelegateCommand(CloseStartMenu);
            ToggleVolumeCommand = new DelegateCommand(ToggleVolume);
            ToggleNotificationsCommand = new DelegateCommand(ToggleNotifications);
            ToggleGameSpeedCommand = new DelegateCommand(ToggleGameSpeed);
            VolumeDownCommand = new DelegateCommand(VolumeDown);
            VolumeUpCommand = new DelegateCommand(VolumeUp);
            ToggleMuteCommand = new DelegateCommand(ToggleMute);
            ToggleReducedMotionCommand = new DelegateCommand(ToggleReducedMotion);
            SetGameSpeedCommand = new DelegateCommand(SetGameSpeed);
            SelectPlannerScenarioCommand = new DelegateCommand(SelectPlannerScenario);
            SelectPlannerSpeciesCommand = new DelegateCommand(SelectPlannerSpecies);
            ContinuePlannerCommand = new DelegateCommand(() => SetPlannerStep(Math.Min(2, plannerStep + 1)));
            BackPlannerCommand = new DelegateCommand(() => SetPlannerStep(Math.Max(0, plannerStep - 1)));
            LaunchPlannerCommand = new DelegateCommand(() => simulationLauncher?.Invoke(true));
            volume = ReadSavedVolume();
            muted = PlayerPrefs.GetInt(MutePreference, 0) == 1;
            ApplyAudioVolume();
        }

        void OnEnable()
        {
            if (genomeCatalogProvider != null)
            {
                genomeCatalogProvider.SnapshotChanged += HandleGenomeCatalogChanged;
            }
        }

        void OnDisable()
        {
            if (genomeCatalogProvider != null)
            {
                genomeCatalogProvider.SnapshotChanged -= HandleGenomeCatalogChanged;
            }
        }

        void OpenDesktopIcon(object parameter)
        {
            var appName = parameter?.ToString();
            if (string.IsNullOrWhiteSpace(appName))
            {
                return;
            }

            CloseTransientPanels();
            if (appName == "Simulation")
            {
                if (simulationLauncher != null)
                {
                    simulationLauncher(false);
                }
                else
                {
                    Debug.LogError("GalapagOS Simulation launcher is not bound.", this);
                }

                return;
            }

            if (HasOpenDesktopWindow(appName))
            {
                return;
            }

            activeDesktopAppTitle = appName;
            activeDesktopAppDescription = GetDesktopAppDescription(appName);
            desktopAppOpen = true;
            var slot = openDesktopWindows.Count;
            GalapagOSDesktopWindow openedWindow = null;
            openedWindow = new GalapagOSDesktopWindow(
                appName,
                activeDesktopAppDescription,
                250f + (slot % 4) * 42f,
                96f + (slot % 4) * 34f,
                () => CloseDesktopWindow(openedWindow),
                () => OpenRelatedDesktopApp(openedWindow, "Gene Lab"),
                () => OpenRelatedDesktopApp(openedWindow, "History / Data Record"),
                appName == "Gene Lab" ? CreateGenomeLabViewModel() : null,
                this);
            openDesktopWindows.Add(openedWindow);
            OnPropertyChanged(nameof(ActiveDesktopAppTitle));
            OnPropertyChanged(nameof(ActiveDesktopAppDescription));
            OnPropertyChanged(nameof(DesktopAppVisibility));
            OnPropertyChanged(nameof(GenericDesktopAppVisibility));
            OnPropertyChanged(nameof(SettingsSurfaceVisibility));
            OnPropertyChanged(nameof(GeneLabSurfaceVisibility));
            Debug.Log($"GalapagOS desktop app opened: {appName}", this);
        }

        void SelectPlannerScenario(object parameter)
        {
            var scenarioId = parameter?.ToString();
            if (scenarioId != "ForestEdge" && scenarioId != "FiveSpeciesSandbox") return;
            plannerScenarioId = scenarioId;
            if (scenarioId == "ForestEdge" && plannerPlayerSpeciesId != "rabbit" && plannerPlayerSpeciesId != "fox")
                plannerPlayerSpeciesId = "rabbit";
            NotifyPlannerSelectionChanged();
        }

        void SelectPlannerSpecies(object parameter)
        {
            var speciesId = parameter?.ToString();
            if (speciesId != "rabbit" && speciesId != "fox" && speciesId != "owl" && speciesId != "deer" && speciesId != "wolf") return;
            if (plannerScenarioId == "ForestEdge" && speciesId != "rabbit" && speciesId != "fox") return;
            plannerPlayerSpeciesId = speciesId;
            NotifyPlannerSelectionChanged();
        }

        void SetPlannerStep(int nextStep)
        {
            plannerStep = nextStep;
            OnPropertyChanged(nameof(PlannerStepLabel));
            OnPropertyChanged(nameof(PlannerScenarioStepVisibility));
            OnPropertyChanged(nameof(PlannerSpeciesStepVisibility));
            OnPropertyChanged(nameof(PlannerOptionsStepVisibility));
        }

        void NotifyPlannerSelectionChanged()
        {
            OnPropertyChanged(nameof(PlannerScenarioId));
            OnPropertyChanged(nameof(PlannerScenarioName));
            OnPropertyChanged(nameof(PlannerScenarioSummary));
            OnPropertyChanged(nameof(PlannerSelectedScenarioText));
            OnPropertyChanged(nameof(PlannerSpeciesAvailableText));
            OnPropertyChanged(nameof(PlannerPlayerSpeciesId));
            OnPropertyChanged(nameof(PlannerPlayerSpeciesName));
            OnPropertyChanged(nameof(PlannerPlayerSpeciesSummary));
            OnPropertyChanged(nameof(PlannerSelectedSpeciesText));
            OnPropertyChanged(nameof(PlannerOptionsTitleText));
            OnPropertyChanged(nameof(PlannerOwlVisibility));
            OnPropertyChanged(nameof(PlannerDeerVisibility));
            OnPropertyChanged(nameof(PlannerWolfVisibility));
        }

        static string SpeciesDisplayName(string speciesId)
        {
            if (speciesId == "fox") return "FOX";
            if (speciesId == "owl") return "OWL";
            if (speciesId == "deer") return "DEER";
            if (speciesId == "wolf") return "WOLF";
            return "RABBIT";
        }

        void CloseDesktopApp()
        {
            desktopAppOpen = false;
            OnPropertyChanged(nameof(DesktopAppVisibility));
        }

        bool HasOpenDesktopWindow(string appName)
        {
            foreach (var window in openDesktopWindows)
            {
                if (window.Title == appName)
                {
                    return true;
                }
            }

            return false;
        }

        void CloseDesktopWindow(GalapagOSDesktopWindow window)
        {
            if (window != null)
            {
                openDesktopWindows.Remove(window);
            }
        }

        void OpenRelatedDesktopApp(GalapagOSDesktopWindow sourceWindow, string appName)
        {
            CloseDesktopWindow(sourceWindow);
            OpenDesktopIcon(appName);
        }

        GenomeLabViewModel CreateGenomeLabViewModel()
        {
            var selectedSpecies = GetSelectedGenomeSpecies();
            return new GenomeLabViewModel(
                genomeCatalogProvider == null
                    ? GenomeCatalogSnapshot.Empty
                    : genomeCatalogProvider.Snapshot,
                profile: profileSession?.Current,
                selectedSpecies: selectedSpecies,
                profileSession: profileSession,
                permanentEconomy: permanentGenomeEconomy);
        }

        SpeciesId? GetSelectedGenomeSpecies()
        {
            return string.IsNullOrWhiteSpace(genomeSpeciesId)
                ? (SpeciesId?)null
                : new SpeciesId(genomeSpeciesId);
        }

        void HandleGenomeCatalogChanged(GenomeCatalogSnapshot snapshot)
        {
            foreach (var window in openDesktopWindows)
            {
                if (window.Title == "Gene Lab")
                {
                    window.RefreshGenomeCatalog(snapshot);
                }
            }
        }

        void HandleProfileSnapshotChanged(ProfileSessionSnapshot snapshot)
        {
            foreach (var window in openDesktopWindows)
            {
                if (window.Title == "Gene Lab")
                    window.RefreshGenomeProfile(snapshot);
            }
        }

        void CloseLabWindow()
        {
            labWindowOpen = false;
            OnPropertyChanged(nameof(LabWindowVisibility));
        }

        string GetDesktopAppDescription(string appName)
        {
            switch (appName)
            {
                case "Field Notes":
                    return "A field notebook for observations, tasks, and expedition notes.";
                case "Gene Lab":
                    return "Permanent species research and future expedition choices will be organized here.";
                case "Biome Data":
                    return "Biome conditions, ecology upgrades, and habitat trends will live here.";
                case "My Collection":
                    return "Unlocked species and research specimens will be catalogued here.";
                case "Settings":
                    return "Display, audio, accessibility, and interface preferences will live here.";
                case "Simulation":
                    return "Run the cellular-automata ecosystem and guide an evolving expedition.";
                case "My PC":
                    return "Desktop appearance, wallpapers, icon layout, and personal preferences will live here.";
                case "Music Player":
                    return "Choose desktop and simulation music for the expedition workspace.";
                case "History / Data Record":
                    return "Run metrics, species performance, achievements, and research records will live here.";
                case "Expedition Planner / Launchpad":
                    return "Prepare the next expedition and choose the ecosystem you want to study.";
                case "Field Guide / Journal":
                    return "Reference notes and observations will be collected in this field guide.";
                case "Research Inbox / Bulletin":
                    return "New findings, tasks, and research updates will arrive here.";
                case "Habitat Gallery / Museum":
                    return "A visual record of habitats, discoveries, and memorable expeditions will live here.";
                case "Quick Search / Jump":
                    return "Jump directly to a species, biome, expedition, or research record.";
                default:
                    return "This GalapagOS application is ready for its feature pass.";
            }
        }

        void ToggleStartMenu()
        {
            startMenuOpen = !startMenuOpen;
            if (startMenuOpen)
            {
                volumePanelOpen = false;
                notificationsPanelOpen = false;
                gameSpeedPanelOpen = false;
            }

            NotifyPanelVisibility();
        }

        void CloseStartMenu()
        {
            if (!startMenuOpen)
            {
                return;
            }

            startMenuOpen = false;
            NotifyPanelVisibility();
        }

        void ToggleVolume()
        {
            volumePanelOpen = !volumePanelOpen;
            if (volumePanelOpen)
            {
                startMenuOpen = false;
                notificationsPanelOpen = false;
                gameSpeedPanelOpen = false;
            }

            NotifyPanelVisibility();
        }

        void ToggleNotifications()
        {
            notificationsPanelOpen = !notificationsPanelOpen;
            if (notificationsPanelOpen)
            {
                startMenuOpen = false;
                volumePanelOpen = false;
                gameSpeedPanelOpen = false;
            }

            NotifyPanelVisibility();
        }

        void ToggleGameSpeed()
        {
            gameSpeedPanelOpen = !gameSpeedPanelOpen;
            if (gameSpeedPanelOpen)
            {
                startMenuOpen = false;
                volumePanelOpen = false;
                notificationsPanelOpen = false;
            }

            NotifyPanelVisibility();
        }

        void VolumeDown()
        {
            volume = Math.Max(0f, volume - 0.1f);
            muted = false;
            ApplyAudioVolume();
            OnPropertyChanged(nameof(VolumeText));
            SaveAudioPreferences();
        }

        void VolumeUp()
        {
            volume = Math.Min(1f, volume + 0.1f);
            muted = false;
            ApplyAudioVolume();
            OnPropertyChanged(nameof(VolumeText));
            SaveAudioPreferences();
        }

        void ToggleMute()
        {
            muted = !muted;
            ApplyAudioVolume();
            OnPropertyChanged(nameof(VolumeText));
            SaveAudioPreferences();
        }

        void ToggleReducedMotion()
        {
            PlayerPrefs.SetInt(ReducedMotionPreference, ReducedMotionEnabled ? 0 : 1);
            PlayerPrefs.Save();
            OnPropertyChanged(nameof(ReducedMotionText));
        }

        void SaveAudioPreferences()
        {
            PlayerPrefs.SetFloat(VolumePreference, volume);
            PlayerPrefs.SetInt(MutePreference, muted ? 1 : 0);
            PlayerPrefs.Save();
        }

        void ApplyAudioVolume()
        {
            AudioListener.volume = muted ? 0f : volume;
        }

        void SetGameSpeed(object parameter)
        {
            if (!float.TryParse(parameter?.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var requestedSpeed)
                || requestedSpeed <= 0f)
            {
                return;
            }

            gameSpeed = requestedSpeed;
            gameSpeedPanelOpen = false;
            OnPropertyChanged(nameof(GameSpeedText));
            OnPropertyChanged(nameof(GameSpeedPanelVisibility));
        }

        void CloseTransientPanels()
        {
            startMenuOpen = false;
            volumePanelOpen = false;
            notificationsPanelOpen = false;
            gameSpeedPanelOpen = false;
            NotifyPanelVisibility();
        }

        void NotifyPanelVisibility()
        {
            OnPropertyChanged(nameof(StartMenuVisibility));
            OnPropertyChanged(nameof(LabWindowVisibility));
            OnPropertyChanged(nameof(DesktopAppVisibility));
            OnPropertyChanged(nameof(VolumePanelVisibility));
            OnPropertyChanged(nameof(NotificationsPanelVisibility));
            OnPropertyChanged(nameof(GameSpeedPanelVisibility));
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        float volume = 0.8f;
        float gameSpeed = 1f;
        bool muted;
        bool startMenuOpen;
        bool labWindowOpen;
        bool desktopAppOpen;
        bool volumePanelOpen;
        bool notificationsPanelOpen;
        bool gameSpeedPanelOpen;
        string activeDesktopAppTitle;
        string activeDesktopAppDescription;
        Action<bool> simulationLauncher;
        string plannerScenarioId = "ForestEdge";
        string plannerPlayerSpeciesId = "rabbit";
        int plannerStep;
        bool useTestMutation;
        bool useTestGenome;
        readonly ObservableCollection<GalapagOSDesktopWindow> openDesktopWindows = new ObservableCollection<GalapagOSDesktopWindow>();
    }

    public sealed class GalapagOSDesktopWindow : INotifyPropertyChanged
    {
        public GalapagOSDesktopWindow(
            string title,
            string description,
            float left,
            float top,
            Action close,
            Action openGeneLab = null,
            Action openHistory = null,
            GenomeLabViewModel genomeLab = null,
            VM_GalapagOS_Desktop settings = null)
        {
            var isGeneLab = title == "Gene Lab";
            var isFieldGuide = title == "Field Notes" || title == "Field Guide / Journal";
            var isSettings = title == "Settings";
            var isSpeciesCollection = title == "My Collection";
            Title = title;
            Description = description;
            Width = isGeneLab
                ? 1450f
                : isFieldGuide
                    ? Mathf.Clamp(Screen.width - 96f, 1040f, 1510f)
                    : isSpeciesCollection
                        ? Mathf.Clamp(Screen.width - 120f, 1120f, 1480f)
                        : isSettings
                            ? Mathf.Clamp(Screen.width - 220f, 1000f, 1280f)
                : 620f;
            Height = isGeneLab
                ? 820f
                : isFieldGuide
                    ? Mathf.Clamp(Screen.height - 128f, 560f, 820f)
                    : isSpeciesCollection
                        ? Mathf.Clamp(Screen.height - 148f, 620f, 840f)
                        : isSettings
                            ? Mathf.Clamp(Screen.height - 180f, 560f, 760f)
                : 300f;
            Left = isGeneLab
                ? 235f
                : isFieldGuide || isSettings || isSpeciesCollection
                ? Mathf.Max(24f, (Screen.width - Width) * 0.5f)
                : left;
            Top = isGeneLab
                ? 92f
                : isFieldGuide || isSettings || isSpeciesCollection
                ? Mathf.Max(24f, (Screen.height - 76f - Height) * 0.5f)
                : top;
            CloseCommand = new DelegateCommand(close);
            OpenGeneLabCommand = new DelegateCommand(openGeneLab ?? (() => { }));
            OpenHistoryCommand = new DelegateCommand(openHistory ?? (() => { }));
            GenomeLab = genomeLab ?? new GenomeLabViewModel(GenomeCatalogSnapshot.Empty);
            Settings = settings;
            GenomeLab.PropertyChanged += HandleGenomeLabPropertyChanged;
        }

        public string Title { get; }
        public string Description { get; }
        public float Left { get; }
        public float Top { get; }
        public float Width { get; }
        public float Height { get; }
        public DelegateCommand CloseCommand { get; }
        public DelegateCommand OpenGeneLabCommand { get; private set; }
        public DelegateCommand OpenHistoryCommand { get; private set; }
        public GenomeLabViewModel GenomeLab { get; }
        public VM_GalapagOS_Desktop Settings { get; }
        public ObservableCollection<GenomeLabNodeViewModel> GenomeNodes => GenomeLab.Nodes;
        public ObservableCollection<GenomeLabTreeSegmentViewModel> GenomeTreeSegments => GenomeLab.GenomeTreeSegments;
        public float GenomeTreeWidth => GenomeLab.GenomeTreeWidth;
        public float GenomeTreeHeight => GenomeLab.GenomeTreeHeight;
        public string GenomeSpeciesNameText => GenomeLab.SpeciesNameText;
        public string GenomeSpeciesIdText => GenomeLab.SpeciesIdText;
        public string GenomeNodeCountText => GenomeLab.NodeCountText;
        public string GenomeCatalogStateText => GenomeLab.CatalogStateText;
        public string ActiveGenomeCapacityText => GenomeLab.ActiveGenomeCapacityText;
        public GenomeLabNodeViewModel SelectedGenomeNode => GenomeLab.SelectedNode;
        public Visibility GenericSurfaceVisibility => IsConceptSurface
            ? Visibility.Collapsed
            : Visibility.Visible;
        public Visibility SettingsSurfaceVisibility => Title == "Settings"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility GeneLabSurfaceVisibility => Title == "Gene Lab"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility SpeciesCollectionSurfaceVisibility => Title == "My Collection"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility MyPcSurfaceVisibility => Title == "My PC"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility MusicPlayerSurfaceVisibility => Title == "Music Player"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility HistorySurfaceVisibility => Title == "History / Data Record"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility BiomeSurfaceVisibility => Title == "Biome Data"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility ExpeditionPlannerSurfaceVisibility => Title == "Expedition Planner / Launchpad"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility FieldGuideSurfaceVisibility => Title == "Field Notes" || Title == "Field Guide / Journal"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility ResearchInboxSurfaceVisibility => Title == "Research Inbox / Bulletin"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility HabitatGallerySurfaceVisibility => Title == "Habitat Gallery / Museum"
            ? Visibility.Visible
            : Visibility.Collapsed;
        public Visibility QuickSearchSurfaceVisibility => Title == "Quick Search / Jump"
            ? Visibility.Visible
            : Visibility.Collapsed;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RefreshGenomeCatalog(GenomeCatalogSnapshot catalog)
        {
            GenomeLab.BindCatalog(catalog);
        }

        public void RefreshGenomeProfile(ProfileSessionSnapshot profile)
        {
            GenomeLab.BindProfile(profile);
        }

        void HandleGenomeLabPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            switch (eventArgs.PropertyName)
            {
                case nameof(GenomeLabViewModel.Nodes):
                    OnPropertyChanged(nameof(GenomeNodes));
                    break;
                case nameof(GenomeLabViewModel.GenomeTreeSegments):
                    OnPropertyChanged(nameof(GenomeTreeSegments));
                    break;
                case nameof(GenomeLabViewModel.GenomeTreeWidth):
                    OnPropertyChanged(nameof(GenomeTreeWidth));
                    break;
                case nameof(GenomeLabViewModel.GenomeTreeHeight):
                    OnPropertyChanged(nameof(GenomeTreeHeight));
                    break;
                case nameof(GenomeLabViewModel.SelectedNode):
                    OnPropertyChanged(nameof(SelectedGenomeNode));
                    break;
                case nameof(GenomeLabViewModel.SpeciesNameText):
                    OnPropertyChanged(nameof(GenomeSpeciesNameText));
                    break;
                case nameof(GenomeLabViewModel.SpeciesIdText):
                    OnPropertyChanged(nameof(GenomeSpeciesIdText));
                    break;
                case nameof(GenomeLabViewModel.NodeCountText):
                    OnPropertyChanged(nameof(GenomeNodeCountText));
                    break;
                case nameof(GenomeLabViewModel.CatalogStateText):
                    OnPropertyChanged(nameof(GenomeCatalogStateText));
                    break;
                case nameof(GenomeLabViewModel.ActiveGenomeCapacityText):
                    OnPropertyChanged(nameof(ActiveGenomeCapacityText));
                    break;
            }
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        bool IsConceptSurface => Title == "Settings"
            || Title == "Gene Lab"
            || Title == "My Collection"
            || Title == "My PC"
            || Title == "Music Player"
            || Title == "History / Data Record"
            || Title == "Biome Data"
            || Title == "Expedition Planner / Launchpad"
            || Title == "Field Notes"
            || Title == "Field Guide / Journal"
            || Title == "Research Inbox / Bulletin"
            || Title == "Habitat Gallery / Museum"
            || Title == "Quick Search / Jump";

    }
}
