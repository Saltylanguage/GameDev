using System;
using System.Collections;
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
        bool desktopTransitionVisible;
        bool desktopTransitionStarted;
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
        public Visibility DesktopTransitionVisibility => desktopTransitionVisible ? Visibility.Visible : Visibility.Collapsed;
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
            if (ContinueEnabled && !desktopTransitionStarted)
            {
                desktopTransitionStarted = true;
                desktopTransitionVisible = true;
                OnPropertyChanged(nameof(DesktopTransitionVisibility));

                if (Application.isBatchMode)
                {
                    sceneTransition.LoadDesktop(profileSession.Current);
                    return;
                }

                PlayDesktopChime();
                StartCoroutine(LoadDesktopAfterTransition());
            }
        }

        IEnumerator LoadDesktopAfterTransition()
        {
            yield return new WaitForSeconds(0.95f);
            sceneTransition.LoadDesktop(profileSession.Current);
        }

        void PlayDesktopChime()
        {
            var audioObject = new GameObject("GalapagOS Desktop Chime");
            DontDestroyOnLoad(audioObject);

            var source = audioObject.AddComponent<AudioSource>();
            source.clip = CreateDesktopChime();
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
            source.volume = 0.65f;
            audioObject.AddComponent<AudioListener>();

            foreach (var listener in FindObjectsByType<AudioListener>())
            {
                if (listener.gameObject != audioObject)
                {
                    listener.enabled = false;
                }
            }

            source.Play();
            Destroy(audioObject, source.clip.length + 0.15f);
        }

        static AudioClip CreateDesktopChime()
        {
            const float echoDelay = 0.24f;
            const int echoCount = 4;
            const float duration = 2.4f;
            var sampleRate = AudioSettings.outputSampleRate;
            var samples = new float[Mathf.CeilToInt(duration * sampleRate)];
            var starts = new[] { 0f, 0.14f, 0.30f, 0.48f, 0.70f };
            var lengths = new[] { 0.40f, 0.40f, 0.42f, 0.44f, 0.50f };
            var frequencies = new[] { 659.25f, 783.99f, 1046.50f, 1318.51f, 1046.50f };
            var echoGains = new[] { 1f, 0.34f, 0.18f, 0.095f, 0.045f };

            for (var sample = 0; sample < samples.Length; sample++)
            {
                var time = sample / (float)sampleRate;
                var value = 0f;

                for (var echo = 0; echo <= echoCount; echo++)
                {
                    var echoedTime = time - echo * echoDelay;
                    if (echoedTime < 0f)
                    {
                        continue;
                    }

                    for (var note = 0; note < frequencies.Length; note++)
                    {
                        var localTime = echoedTime - starts[note];
                        var noteLength = lengths[note];
                        if (localTime < 0f || localTime > noteLength)
                        {
                            continue;
                        }

                        var progress = localTime / noteLength;
                        var attack = 1f - Mathf.Exp(-localTime * 72f);
                        var release = Mathf.Exp(-localTime * 5.4f);
                        var envelope = attack * Mathf.Sin(Mathf.PI * progress) * release;
                        var pitchBend = 1f + 0.055f * Mathf.Exp(-localTime * 13f);
                        var frequency = frequencies[note] * pitchBend * (1f - echo * 0.0025f);
                        var phase = Mathf.PI * 2f * frequency * localTime;
                        var brightness = Mathf.Lerp(0.22f, 0.07f, echo / (float)echoCount);
                        var tone = Mathf.Sin(phase) * 0.78f
                            + Mathf.Sin(phase * 2f) * 0.16f
                            + Mathf.Sin(phase * 3f) * brightness;
                        var chirp = Mathf.Sin(phase * 2.01f) * Mathf.Exp(-localTime * 18f) * 0.08f;
                        value += (tone + chirp) * envelope * 0.14f * echoGains[echo];
                    }
                }

                // A short low body gives the first note some presence without turning it into a thump.
                value += Mathf.Sin(Mathf.PI * 2f * 164f * time) * Mathf.Exp(-time * 22f) * 0.055f;
                samples[sample] = Mathf.Clamp(value, -1f, 1f);
            }

            var clip = AudioClip.Create("GalapagOS_Desktop_Chime", samples.Length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
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
            OnPropertyChanged(nameof(DesktopTransitionVisibility));
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
