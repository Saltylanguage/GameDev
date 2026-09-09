using System;
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

        public Visibility StartMenuVisibility => startMenuOpen ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LabWindowVisibility => labWindowOpen ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DesktopAppVisibility => desktopAppOpen ? Visibility.Visible : Visibility.Collapsed;
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

            activeDesktopAppTitle = appName;
            activeDesktopAppDescription = GetDesktopAppDescription(appName);
            desktopAppOpen = true;
            OnPropertyChanged(nameof(ActiveDesktopAppTitle));
            OnPropertyChanged(nameof(ActiveDesktopAppDescription));
            OnPropertyChanged(nameof(DesktopAppVisibility));
            Debug.Log($"GalapagOS desktop app opened: {appName}", this);
        }

        void CloseDesktopApp()
        {
            desktopAppOpen = false;
            OnPropertyChanged(nameof(DesktopAppVisibility));
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
                    return "Species research and upgrade trees will be organized here.";
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
                desktopAppOpen = false;
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
                desktopAppOpen = false;
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
                desktopAppOpen = false;
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
                desktopAppOpen = false;
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
            desktopAppOpen = false;
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
    }
}
