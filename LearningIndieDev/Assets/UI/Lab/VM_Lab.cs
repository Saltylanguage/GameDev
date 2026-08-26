using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Noesis;
using UnityEngine;

namespace SaltyGame
{
    public sealed class LabFeatureOption
    {
        public string FeatureId { get; }
        public string Label { get; }
        public DelegateCommand OpenCommand { get; }

        public LabFeatureOption(string featureId, string label, Action<string> open)
        {
            FeatureId = featureId;
            Label = label;
            OpenCommand = new DelegateCommand(_ => open(FeatureId));
        }
    }

    public sealed class VM_Lab : MonoBehaviour, INotifyPropertyChanged
    {
        [Header("Serialized Composition")]
        [SerializeField] Helper_ProfileSession profileSession;
        [SerializeField] Helper_SceneTransition sceneTransition;
        [SerializeField] VM_Overview overview;
        [SerializeField] VM_Research research;
        [SerializeField] VM_SpeciesArchive speciesArchive;
        [SerializeField] VM_ExpeditionSetup expeditionSetup;
        [SerializeField] VM_Settings settings;

        readonly ObservableCollection<LabFeatureOption> features = new ObservableCollection<LabFeatureOption>();
        VM_LabFeature activeFeature;

        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand BackToOverviewCommand { get; private set; }
        public DelegateCommand LaunchExpeditionCommand { get; private set; }
        public DelegateCommand ReturnToMainMenuCommand { get; private set; }
        public ObservableCollection<LabFeatureOption> Features => features;
        public VM_Overview Overview => overview;
        public VM_Research Research => research;
        public VM_SpeciesArchive SpeciesArchive => speciesArchive;
        public VM_ExpeditionSetup ExpeditionSetup => expeditionSetup;
        public VM_Settings Settings => settings;
        public VM_LabFeature ActiveFeature => activeFeature;
        public string ProfileName => profileSession?.Current?.HasLoadedProfile == true
            ? profileSession.Current.ProfileName
            : "NO PROFILE LOADED";
        public string ActiveFeatureTitle => activeFeature?.Title ?? "LAB OVERVIEW";
        public Visibility OverviewVisibility => activeFeature == overview ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ResearchVisibility => activeFeature == research ? Visibility.Visible : Visibility.Collapsed;
        public Visibility SpeciesArchiveVisibility => activeFeature == speciesArchive ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ExpeditionSetupVisibility => activeFeature == expeditionSetup ? Visibility.Visible : Visibility.Collapsed;
        public Visibility SettingsVisibility => activeFeature == settings ? Visibility.Visible : Visibility.Collapsed;
        public bool CanLaunchExpedition => activeFeature == expeditionSetup
            && profileSession?.Current?.HasLoadedProfile == true;

        void Awake()
        {
            BackToOverviewCommand = new DelegateCommand(BackToOverview);
            LaunchExpeditionCommand = new DelegateCommand(LaunchExpedition, () => CanLaunchExpedition);
            ReturnToMainMenuCommand = new DelegateCommand(ReturnToMainMenu);

            features.Add(new LabFeatureOption("Overview", "OVERVIEW", OpenFeature));
            features.Add(new LabFeatureOption("Research", "RESEARCH", OpenFeature));
            features.Add(new LabFeatureOption("SpeciesArchive", "SPECIES ARCHIVE", OpenFeature));
            features.Add(new LabFeatureOption("ExpeditionSetup", "EXPEDITION SETUP", OpenFeature));
            features.Add(new LabFeatureOption("Settings", "SETTINGS", OpenFeature));
            activeFeature = overview;
        }

        void Start()
        {
            if (profileSession == null || sceneTransition == null || overview == null || research == null
                || speciesArchive == null || expeditionSetup == null || settings == null)
            {
                Debug.LogError("VM_Lab requires serialized profile, transition, and feature ViewModels.", this);
                enabled = false;
                return;
            }

            profileSession.SnapshotChanged += HandleProfileSnapshotChanged;
            RaiseState();
        }

        void OnDestroy()
        {
            if (profileSession != null)
            {
                profileSession.SnapshotChanged -= HandleProfileSnapshotChanged;
            }
        }

        void OpenFeature(string featureId)
        {
            switch (featureId)
            {
                case "Research": activeFeature = research; break;
                case "SpeciesArchive": activeFeature = speciesArchive; break;
                case "ExpeditionSetup": activeFeature = expeditionSetup; break;
                case "Settings": activeFeature = settings; break;
                default: activeFeature = overview; break;
            }

            RaiseState();
        }

        void BackToOverview()
        {
            activeFeature = overview;
            RaiseState();
        }

        void LaunchExpedition()
        {
            if (!CanLaunchExpedition)
            {
                return;
            }

            var launch = expeditionSetup.CreateLaunchRequest(profileSession.Current);
            if (launch != null)
            {
                sceneTransition.LoadSimulation(launch);
            }
        }

        void ReturnToMainMenu()
        {
            sceneTransition.LoadMainMenu();
        }

        void HandleProfileSnapshotChanged(ProfileSessionSnapshot snapshot)
        {
            OnPropertyChanged(nameof(ProfileName));
        }

        void RaiseState()
        {
            OnPropertyChanged(nameof(ActiveFeature));
            OnPropertyChanged(nameof(ActiveFeatureTitle));
            OnPropertyChanged(nameof(OverviewVisibility));
            OnPropertyChanged(nameof(ResearchVisibility));
            OnPropertyChanged(nameof(SpeciesArchiveVisibility));
            OnPropertyChanged(nameof(ExpeditionSetupVisibility));
            OnPropertyChanged(nameof(SettingsVisibility));
            OnPropertyChanged(nameof(CanLaunchExpedition));
            LaunchExpeditionCommand?.RaiseCanExecuteChanged();
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
