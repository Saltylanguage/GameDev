using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Noesis;
using UnityEngine;

namespace SaltyGame
{
    public sealed class ProfileOption
    {
        public string ProfileId { get; }
        public string ProfileName { get; }
        public DelegateCommand SelectCommand { get; }

        public ProfileOption(ProfileSessionSnapshot profile, Action<string> select)
        {
            ProfileId = profile.ProfileId;
            ProfileName = profile.ProfileName;
            SelectCommand = new DelegateCommand(_ => select(ProfileId));
        }
    }

    public sealed class VM_MainMenu : MonoBehaviour, INotifyPropertyChanged
    {
        enum MenuPage
        {
            MainMenu,
            ProfileSelection,
        }

        [Header("Serialized Composition")]
        [SerializeField] Helper_ProfileSession profileSession;
        [SerializeField] Helper_SceneTransition sceneTransition;

        readonly ObservableCollection<ProfileOption> profiles = new ObservableCollection<ProfileOption>();
        MenuPage page = MenuPage.MainMenu;
        bool quitConfirmationVisible;
        string profileNameInput = "Researcher 01";

        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand OpenProfileSelectionCommand { get; private set; }
        public DelegateCommand CloseProfileSelectionCommand { get; private set; }
        public DelegateCommand CreateInitialProfileCommand { get; private set; }
        public DelegateCommand ContinueCommand { get; private set; }
        public DelegateCommand RequestQuitCommand { get; private set; }
        public DelegateCommand ConfirmQuitCommand { get; private set; }
        public DelegateCommand CancelQuitCommand { get; private set; }

        public Visibility MainMenuVisibility => page == MenuPage.MainMenu ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ProfileSelectionVisibility => page == MenuPage.ProfileSelection ? Visibility.Visible : Visibility.Collapsed;
        public Visibility QuitConfirmationVisibility => quitConfirmationVisible ? Visibility.Visible : Visibility.Collapsed;
        public ObservableCollection<ProfileOption> Profiles => profiles;
        public string ProfileNameInput
        {
            get => profileNameInput;
            set
            {
                var normalized = value ?? string.Empty;
                if (profileNameInput == normalized)
                {
                    return;
                }

                profileNameInput = normalized;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CreateInitialProfileEnabled));
                CreateInitialProfileCommand?.RaiseCanExecuteChanged();
            }
        }

        public string CurrentProfileName => profileSession?.Current?.HasLoadedProfile == true
            ? profileSession.Current.ProfileName
            : "NO PROFILE LOADED";

        public bool ContinueEnabled => profileSession?.Current?.HasLoadedProfile == true;
        public bool CreateInitialProfileEnabled => profileSession != null
            && !profileSession.HasProfiles
            && !string.IsNullOrWhiteSpace(profileNameInput);

        void Awake()
        {
            OpenProfileSelectionCommand = new DelegateCommand(OpenProfileSelection);
            CloseProfileSelectionCommand = new DelegateCommand(CloseProfileSelection);
            CreateInitialProfileCommand = new DelegateCommand(CreateInitialProfile, () => CreateInitialProfileEnabled);
            ContinueCommand = new DelegateCommand(Continue, () => ContinueEnabled);
            RequestQuitCommand = new DelegateCommand(RequestQuit);
            ConfirmQuitCommand = new DelegateCommand(ConfirmQuit);
            CancelQuitCommand = new DelegateCommand(CancelQuit);
        }

        void Start()
        {
            if (profileSession == null || sceneTransition == null)
            {
                Debug.LogError("VM_MainMenu requires serialized profile and scene transition helpers.", this);
                enabled = false;
                return;
            }

            profileSession.SnapshotChanged += HandleProfileSnapshotChanged;
            RefreshProfiles();
            RaiseProfileState();
        }

        void OnDestroy()
        {
            if (profileSession != null)
            {
                profileSession.SnapshotChanged -= HandleProfileSnapshotChanged;
            }
        }

        void OpenProfileSelection()
        {
            page = MenuPage.ProfileSelection;
            quitConfirmationVisible = false;
            RaisePageState();
        }

        void CloseProfileSelection()
        {
            page = MenuPage.MainMenu;
            RaisePageState();
        }

        void CreateInitialProfile()
        {
            if (!CreateInitialProfileEnabled)
            {
                return;
            }

            profileSession.CreateInitialProfile(profileNameInput);
            CloseProfileSelection();
        }

        void SelectProfile(string profileId)
        {
            if (profileSession.SelectProfile(profileId))
            {
                CloseProfileSelection();
            }
        }

        void Continue()
        {
            if (ContinueEnabled)
            {
                sceneTransition.LoadLab(profileSession.Current);
            }
        }

        void RequestQuit()
        {
            quitConfirmationVisible = true;
            OnPropertyChanged(nameof(QuitConfirmationVisibility));
        }

        void ConfirmQuit()
        {
            sceneTransition.QuitApplication();
        }

        void CancelQuit()
        {
            quitConfirmationVisible = false;
            OnPropertyChanged(nameof(QuitConfirmationVisibility));
        }

        void HandleProfileSnapshotChanged(ProfileSessionSnapshot snapshot)
        {
            RefreshProfiles();
            RaiseProfileState();
        }

        void RefreshProfiles()
        {
            profiles.Clear();
            for (var index = 0; index < profileSession.Profiles.Count; index++)
            {
                profiles.Add(new ProfileOption(profileSession.Profiles[index], SelectProfile));
            }

            OnPropertyChanged(nameof(Profiles));
        }

        void RaiseProfileState()
        {
            OnPropertyChanged(nameof(CurrentProfileName));
            OnPropertyChanged(nameof(ContinueEnabled));
            OnPropertyChanged(nameof(CreateInitialProfileEnabled));
            ContinueCommand?.RaiseCanExecuteChanged();
            CreateInitialProfileCommand?.RaiseCanExecuteChanged();
        }

        void RaisePageState()
        {
            OnPropertyChanged(nameof(MainMenuVisibility));
            OnPropertyChanged(nameof(ProfileSelectionVisibility));
            OnPropertyChanged(nameof(QuitConfirmationVisibility));
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
