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
        public DelegateCommand SetGameSpeedCommand { get; private set; }

        public string DesktopTitle => "GALAPAGOS DESKTOP";
        public string WindowTitle => "GALAPAGOS TEST WINDOW";
        public string StatusText => "VIEW MODEL DATA CONTEXT RESOLVED";
        public string ViewModelType => nameof(VM_GalapagOS_Desktop);
        public string DateText => "JUNE 14 · DAY 03";
        public string ClockText => "10:32 AM";
        public string DayPhaseText => "DAYLIGHT";
        public string NotificationCountText => "3";
        public string VolumeText => muted ? "MUTED" : $"{Math.Round(volume * 100f):0}%";
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void BindSimulationLauncher(Action launch)
        {
            simulationLauncher = launch;
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
            SetGameSpeedCommand = new DelegateCommand(SetGameSpeed);
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
                    simulationLauncher();
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
                () => OpenRelatedDesktopApp(openedWindow, "History / Data Record"));
            openDesktopWindows.Add(openedWindow);
            OnPropertyChanged(nameof(ActiveDesktopAppTitle));
            OnPropertyChanged(nameof(ActiveDesktopAppDescription));
            OnPropertyChanged(nameof(DesktopAppVisibility));
            OnPropertyChanged(nameof(GenericDesktopAppVisibility));
            OnPropertyChanged(nameof(SettingsSurfaceVisibility));
            OnPropertyChanged(nameof(GeneLabSurfaceVisibility));
            Debug.Log($"GalapagOS desktop app opened: {appName}", this);
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
            OnPropertyChanged(nameof(VolumeText));
        }

        void VolumeUp()
        {
            volume = Math.Min(1f, volume + 0.1f);
            muted = false;
            OnPropertyChanged(nameof(VolumeText));
        }

        void ToggleMute()
        {
            muted = !muted;
            OnPropertyChanged(nameof(VolumeText));
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
        bool labWindowOpen = true;
        bool desktopAppOpen;
        bool volumePanelOpen;
        bool notificationsPanelOpen;
        bool gameSpeedPanelOpen;
        string activeDesktopAppTitle;
        string activeDesktopAppDescription;
        Action simulationLauncher;
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
            Action openHistory = null)
        {
            var isGeneLab = title == "Gene Lab";
            var isFieldGuide = title == "Field Notes" || title == "Field Guide / Journal";
            var isSettings = title == "Settings";
            var isSpeciesCollection = title == "My Collection";
            Title = title;
            Description = description;
            Width = isGeneLab
                ? Mathf.Clamp(Screen.width - 64f, 960f, 1450f)
                : isFieldGuide
                    ? Mathf.Clamp(Screen.width - 96f, 1040f, 1510f)
                    : isSpeciesCollection
                        ? Mathf.Clamp(Screen.width - 120f, 1120f, 1480f)
                        : isSettings
                            ? Mathf.Clamp(Screen.width - 220f, 1000f, 1280f)
                : 620f;
            Height = isGeneLab
                ? Mathf.Clamp(Screen.height - 116f, 560f, 820f)
                : isFieldGuide
                    ? Mathf.Clamp(Screen.height - 128f, 560f, 820f)
                    : isSpeciesCollection
                        ? Mathf.Clamp(Screen.height - 148f, 620f, 840f)
                        : isSettings
                            ? Mathf.Clamp(Screen.height - 180f, 560f, 760f)
                : 300f;
            Left = isGeneLab || isFieldGuide || isSettings || isSpeciesCollection
                ? Mathf.Max(24f, (Screen.width - Width) * 0.5f)
                : left;
            Top = isGeneLab || isFieldGuide || isSettings || isSpeciesCollection
                ? Mathf.Max(24f, (Screen.height - 76f - Height) * 0.5f)
                : top;
            CloseCommand = new DelegateCommand(close);
            OpenGeneLabCommand = new DelegateCommand(openGeneLab ?? (() => { }));
            OpenHistoryCommand = new DelegateCommand(openHistory ?? (() => { }));
            ToggleGuardedBurrowCommand = new DelegateCommand(ToggleGuardedBurrow);
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
        public DelegateCommand ToggleGuardedBurrowCommand { get; }
        public string GuardedBurrowStateText => guardedBurrowActive ? "UNLOCKED · ACTIVE" : "UNLOCKED · INACTIVE";
        public string GuardedBurrowNodeStateText => guardedBurrowActive ? "✓  ACTIVE" : "UNLOCKED";
        public string GuardedBurrowActionText => guardedBurrowActive ? "DEACTIVATE" : "ACTIVATE";
        public string ActiveGenomeCapacityText => guardedBurrowActive ? "6 / 8" : "5 / 8";
        public Visibility GuardedBurrowActiveVisibility => guardedBurrowActive ? Visibility.Visible : Visibility.Collapsed;
        public Visibility GuardedBurrowInactiveVisibility => guardedBurrowActive ? Visibility.Collapsed : Visibility.Visible;
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

        void ToggleGuardedBurrow()
        {
            guardedBurrowActive = !guardedBurrowActive;
            OnPropertyChanged(nameof(GuardedBurrowStateText));
            OnPropertyChanged(nameof(GuardedBurrowNodeStateText));
            OnPropertyChanged(nameof(GuardedBurrowActionText));
            OnPropertyChanged(nameof(ActiveGenomeCapacityText));
            OnPropertyChanged(nameof(GuardedBurrowActiveVisibility));
            OnPropertyChanged(nameof(GuardedBurrowInactiveVisibility));
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

        bool guardedBurrowActive = true;
    }
}
