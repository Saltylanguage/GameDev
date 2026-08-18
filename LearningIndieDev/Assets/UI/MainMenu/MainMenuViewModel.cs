using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Noesis;
using UnityEngine;

namespace SaltyGame
{
    public sealed class MainMenuViewModel : MonoBehaviour, INotifyPropertyChanged
    {
        enum ShellPage
        {
            MainMenu,
            LabOverview,
            ResearchPreview,
        }

        ShellPage page = ShellPage.MainMenu;
        bool quitConfirmationVisible;
        bool lockedProjectSelected;
        NoesisView view;

        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand EnterLabCommand { get; private set; }
        public DelegateCommand OpenResearchCommand { get; private set; }
        public DelegateCommand BackCommand { get; private set; }
        public DelegateCommand QuitCommand { get; private set; }
        public DelegateCommand CancelQuitCommand { get; private set; }
        public DelegateCommand ConfirmQuitCommand { get; private set; }
        public DelegateCommand SelectAvailableProjectCommand { get; private set; }
        public DelegateCommand SelectLockedProjectCommand { get; private set; }

        public Visibility MainMenuVisibility => page == ShellPage.MainMenu ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LabVisibility => page == ShellPage.LabOverview ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ResearchVisibility => page == ShellPage.ResearchPreview ? Visibility.Visible : Visibility.Collapsed;
        public Visibility QuitConfirmationVisibility => quitConfirmationVisible ? Visibility.Visible : Visibility.Collapsed;

        public bool SettingsEnabled => false;
        public bool CreditsEnabled => false;
        public bool SpeciesArchiveEnabled => false;
        public bool ExpeditionEnabled => false;
        public bool PurchaseEnabled => false;

        public string SelectedProjectTitle => lockedProjectSelected
            ? "Predator Avoidance Field Notes"
            : "Forage Route Mapping";

        public string SelectedProjectState => lockedProjectSelected
            ? "LOCKED · PREREQUISITE AND BALANCE REQUIRED"
            : "AVAILABLE · AFFORDABLE REPRESENTATIVE PROJECT";

        public string SelectedProjectCost => lockedProjectSelected
            ? "Cost: 160 Research + 60 Herbivore Data · Current: 120 Research + 48 Herbivore Data"
            : "Cost: 10 Research + 20 Herbivore Data · Current: 120 Research + 48 Herbivore Data";

        public string SelectedProjectPrerequisite => lockedProjectSelected
            ? "Prerequisite: Forage Route Mapping"
            : "Prerequisite: none";

        public string SelectedProjectBenefit => lockedProjectSelected
            ? "Benefit preview: reveals a predator-pressure observation overlay."
            : "Benefit preview: reveals a food-search route preview for the Hare study.";

        void Awake()
        {
            EnterLabCommand = new DelegateCommand(EnterLab);
            OpenResearchCommand = new DelegateCommand(OpenResearch);
            BackCommand = new DelegateCommand(Back);
            QuitCommand = new DelegateCommand(OpenQuitConfirmation);
            CancelQuitCommand = new DelegateCommand(CancelQuit);
            ConfirmQuitCommand = new DelegateCommand(ConfirmQuit);
            SelectAvailableProjectCommand = new DelegateCommand(SelectAvailableProject);
            SelectLockedProjectCommand = new DelegateCommand(SelectLockedProject);
        }

        void Start()
        {
            view = GetComponent<NoesisView>();
            if (view == null || view.Content == null)
            {
                Debug.LogError("MainMenuViewModel requires a NoesisView with MainMenuShell.xaml.", this);
                return;
            }

            view.Content.DataContext = this;
            FocusElement("EnterLabButton");
        }

        void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape))
            {
                return;
            }

            if (quitConfirmationVisible)
            {
                CancelQuit();
            }
            else if (page == ShellPage.MainMenu)
            {
                OpenQuitConfirmation();
            }
            else
            {
                Back();
            }
        }

        void EnterLab()
        {
            SetPage(ShellPage.LabOverview, "OpenResearchButton");
        }

        void OpenResearch()
        {
            lockedProjectSelected = false;
            OnPropertyChanged(nameof(SelectedProjectTitle));
            OnPropertyChanged(nameof(SelectedProjectState));
            OnPropertyChanged(nameof(SelectedProjectCost));
            OnPropertyChanged(nameof(SelectedProjectPrerequisite));
            OnPropertyChanged(nameof(SelectedProjectBenefit));
            SetPage(ShellPage.ResearchPreview, "AvailableProjectButton");
        }

        void Back()
        {
            switch (page)
            {
                case ShellPage.ResearchPreview:
                    SetPage(ShellPage.LabOverview, "OpenResearchButton");
                    break;
                case ShellPage.LabOverview:
                    SetPage(ShellPage.MainMenu, "EnterLabButton");
                    break;
            }
        }

        void OpenQuitConfirmation()
        {
            quitConfirmationVisible = true;
            OnPropertyChanged(nameof(QuitConfirmationVisibility));
            FocusElement("CancelQuitButton");
        }

        void CancelQuit()
        {
            quitConfirmationVisible = false;
            OnPropertyChanged(nameof(QuitConfirmationVisibility));
            FocusElement(page == ShellPage.MainMenu
                ? "EnterLabButton"
                : page == ShellPage.LabOverview
                    ? "OpenResearchButton"
                    : "AvailableProjectButton");
        }

        void ConfirmQuit()
        {
            Application.Quit();
        }

        void SelectAvailableProject()
        {
            lockedProjectSelected = false;
            RaiseProjectProperties();
        }

        void SelectLockedProject()
        {
            lockedProjectSelected = true;
            RaiseProjectProperties();
        }

        void SetPage(ShellPage nextPage, string focusName)
        {
            page = nextPage;
            OnPropertyChanged(nameof(MainMenuVisibility));
            OnPropertyChanged(nameof(LabVisibility));
            OnPropertyChanged(nameof(ResearchVisibility));
            FocusElement(focusName);
        }

        void RaiseProjectProperties()
        {
            OnPropertyChanged(nameof(SelectedProjectTitle));
            OnPropertyChanged(nameof(SelectedProjectState));
            OnPropertyChanged(nameof(SelectedProjectCost));
            OnPropertyChanged(nameof(SelectedProjectPrerequisite));
            OnPropertyChanged(nameof(SelectedProjectBenefit));
        }

        void FocusElement(string name)
        {
            if (view?.Content is FrameworkElement content)
            {
                (content.FindName(name) as UIElement)?.Focus();
            }
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
