using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    [Category("UI")]
    public sealed class LabPlayModeTests
    {
        const string LabScene = "Lab";
        const string MainMenuScene = "MainMenu";
        const string ViewModelType = "SaltyGame.VM_Lab";
        ProfilePlayerPrefsBackup originalProfileSave;

        [SetUp]
        public void PreserveProfileSave()
        {
            originalProfileSave = ProfilePlayerPrefsBackup.Capture();
        }

        [UnityTearDown]
        public IEnumerator RestoreProfileSave()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            foreach (var session in Object.FindObjectsByType<Helper_ProfileSession>(FindObjectsSortMode.None))
            {
                session.SetPlaytimeTracking(false);
                session.enabled = false;
            }

            originalProfileSave.Restore();
        }

        [UnityTest]
        public IEnumerator LabSceneHasOneComposedRoot()
        {
            yield return LoadLab();

            var root = FindRoot();
            Assert.That(root, Is.Not.Null);
            Assert.That(root.GetComponent("NoesisView"), Is.Not.Null);
            Assert.That(root.GetComponent(ViewModelType), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.LabNoesisHost"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.VM_Overview"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.VM_Research"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.VM_SpeciesArchive"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.VM_ExpeditionSetup"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.VM_Settings"), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator LabFeatureNavigationUpdatesTheUiState()
        {
            yield return LoadLab();

            var viewModel = FindViewModel();
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(GetVisibility(viewModel, "OverviewVisibility"), Is.EqualTo("Visible"));

            var features = (IEnumerable)GetProperty(viewModel, "Features");
            var openedResearch = false;
            foreach (var feature in features)
            {
                if ((string)GetProperty(feature, "FeatureId") != "Research")
                {
                    continue;
                }

                Execute(GetProperty(feature, "OpenCommand"));
                openedResearch = true;
                break;
            }

            Assert.That(openedResearch, Is.True);
            Assert.That(GetVisibility(viewModel, "ResearchVisibility"), Is.EqualTo("Visible"));
            Assert.That(GetVisibility(viewModel, "OverviewVisibility"), Is.EqualTo("Collapsed"));

            Execute(GetProperty(viewModel, "BackToOverviewCommand"));
            Assert.That(GetVisibility(viewModel, "OverviewVisibility"), Is.EqualTo("Visible"));
        }

        [UnityTest]
        public IEnumerator ExpeditionLaunchStaysDisabledWithoutProfile()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
            yield return LoadLab();

            var viewModel = FindViewModel();
            var launch = GetProperty(viewModel, "LaunchExpeditionCommand");
            Assert.That(CanExecute(launch), Is.False);
            Assert.That((bool)GetProperty(viewModel, "CanLaunchExpedition"), Is.False);
        }

        [UnityTest]
        public IEnumerator ExpeditionLaunchLoadsSimulationWithImmutableRequest()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
            yield return LoadLab();

            var root = FindRoot();
            var profile = root.GetComponent<Helper_ProfileSession>();
            profile.CreateInitialProfile("T6 Test Profile");
            Assert.That(profile.TrySetGenomeConfiguration(
                SpeciesIds.Herbivore,
                new[] { "hare.guard" },
                new[] { "hare.guard" },
                out var genomeValidationMessage), Is.True, genomeValidationMessage);

            var viewModel = FindViewModel();
            var features = (IEnumerable)GetProperty(viewModel, "Features");
            foreach (var feature in features)
            {
                if ((string)GetProperty(feature, "FeatureId") == "ExpeditionSetup")
                {
                    Execute(GetProperty(feature, "OpenCommand"));
                    break;
                }
            }

            var launch = GetProperty(viewModel, "LaunchExpeditionCommand");
            Assert.That(CanExecute(launch), Is.True);
            Execute(launch);
            yield return null;
            yield return null;

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("CellularAutomataPrototype"));
            Component preview = null;
            var previewObjects = Object.FindObjectsByType<Component>(FindObjectsSortMode.None);
            foreach (var candidate in previewObjects)
            {
                if (candidate.GetType().FullName == "SaltyGame.SpeciesSimulationPreview")
                {
                    preview = candidate;
                    break;
                }
            }
            Assert.That(preview, Is.Not.Null);
            Assert.That(GetProperty(preview, "SelectedScenario"), Is.Not.Null);
            Assert.That(((UnityEngine.Object)GetProperty(preview, "SelectedScenario")).name, Is.EqualTo("ForestEdge"));
            Assert.That(GetProperty(GetProperty(preview, "PlayerSpecies"), "Value"), Is.EqualTo("hare"));
            Assert.That((int)GetProperty(preview, "BaseSeed"), Is.EqualTo(10100));
            Assert.That((bool)GetProperty(preview, "RandomizeSeedOnStart"), Is.False);
            var activeGenomeSnapshot = GetProperty(preview, "ActiveGenomeSnapshot");
            Assert.That(GetProperty(activeGenomeSnapshot, "Fingerprint"), Is.Not.EqualTo(string.Empty));
            var speciesGenomes = (IDictionary)GetProperty(activeGenomeSnapshot, "SpeciesGenomes");
            Assert.That(speciesGenomes.Count, Is.GreaterThanOrEqualTo(1));

            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
        }

        [UnityTest]
        public IEnumerator SimulationWindowCloseIsDisabledDuringRunAndReturnsToLabAfterResults()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
            yield return LoadLab();

            var root = FindRoot();
            var profile = root.GetComponent<Helper_ProfileSession>();
            profile.CreateInitialProfile("Close Policy Test Profile");

            var viewModel = FindViewModel();
            foreach (var feature in (IEnumerable)GetProperty(viewModel, "Features"))
            {
                if ((string)GetProperty(feature, "FeatureId") == "ExpeditionSetup")
                {
                    Execute(GetProperty(feature, "OpenCommand"));
                    break;
                }
            }

            Execute(GetProperty(viewModel, "LaunchExpeditionCommand"));
            yield return null;
            yield return null;

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("CellularAutomataPrototype"));
            var runtime = Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);
            var preview = runtime.SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var phaseMessage), Is.True, phaseMessage);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "8",
                "8",
                preview.BaseSeed.ToString(),
                preview.MaximumPopulation.ToString(),
                preview.MinimumPopulation.ToString(),
                "4",
                "0.01",
                preview.PlantProbability.ToString(),
                preview.HerbivoreProbability.ToString(),
                preview.CarnivoreProbability.ToString(),
                randomizeSeed: false,
                "20",
                "10",
                "0",
                out var settingsMessage), Is.True, settingsMessage);

            var simulationViewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(simulationViewModel, Is.Not.Null);
            var closeCommand = GetProperty(simulationViewModel, "CloseWindowCommand");
            preview.StartSimulation();
            yield return null;

            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.Results
                   && Time.realtimeSinceStartup < timeout)
            {
                Assert.That(GetProperty(simulationViewModel, "CanCloseWindow"), Is.EqualTo(false));
                Assert.That(CanExecute(closeCommand), Is.False);
                if (preview.State == SpeciesPreviewState.PhaseDecision)
                {
                    preview.ContinueWithoutUpgrade();
                }

                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            yield return null;
            Assert.That(GetProperty(simulationViewModel, "CanCloseWindow"), Is.EqualTo(true));
            Assert.That(CanExecute(closeCommand), Is.True);

            Execute(closeCommand);
            timeout = Time.realtimeSinceStartup + 5f;
            while (SceneManager.GetActiveScene().name != "Lab"
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("Lab"));
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
        }

        [UnityTest]
        public IEnumerator GalapagOSSettingsAppUsesDedicatedSettingsSurface()
        {
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var viewModel = GameObject.Find("GalapagOS Desktop Test Camera")
                ?.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(GetProperty(viewModel, "SettingsSurfaceVisibility").ToString(), Is.EqualTo("Collapsed"));

            var openCommand = GetProperty(viewModel, "OpenDesktopIconCommand");
            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Settings" });

            Assert.That(GetProperty(viewModel, "ActiveDesktopAppTitle"), Is.EqualTo("Settings"));
            Assert.That(GetProperty(viewModel, "SettingsSurfaceVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(GetProperty(viewModel, "GenericDesktopAppVisibility").ToString(), Is.EqualTo("Collapsed"));

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Field Notes" });

            Assert.That(GetProperty(viewModel, "SettingsSurfaceVisibility").ToString(), Is.EqualTo("Collapsed"));
            Assert.That(GetProperty(viewModel, "GenericDesktopAppVisibility").ToString(), Is.EqualTo("Visible"));
        }

        [UnityTest]
        public IEnumerator GalapagOSMyCollectionAppUsesDedicatedCollectionSurface()
        {
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var viewModel = GameObject.Find("GalapagOS Desktop Test Camera")
                ?.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            Assert.That(viewModel, Is.Not.Null);

            var openCommand = GetProperty(viewModel, "OpenDesktopIconCommand");
            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "My Collection" });

            var openWindows = (IList)GetProperty(viewModel, "OpenDesktopWindows");
            Assert.That(openWindows.Count, Is.EqualTo(1));
            Assert.That(GetProperty(openWindows[0], "Title"), Is.EqualTo("My Collection"));
            Assert.That(GetProperty(openWindows[0], "SpeciesCollectionSurfaceVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(GetProperty(openWindows[0], "GenericSurfaceVisibility").ToString(), Is.EqualTo("Collapsed"));
        }

        [UnityTest]
        public IEnumerator GalapagOSDesktopAppsRemainOpenTogether()
        {
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var viewModel = GameObject.Find("GalapagOS Desktop Test Camera")
                ?.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            Assert.That(viewModel, Is.Not.Null);

            var openCommand = GetProperty(viewModel, "OpenDesktopIconCommand");
            var openWindows = (IList)GetProperty(viewModel, "OpenDesktopWindows");
            Assert.That(openWindows.Count, Is.EqualTo(0));

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Field Notes" });
            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Gene Lab" });

            Assert.That(openWindows.Count, Is.EqualTo(2));
            Assert.That(GetProperty(openWindows[0], "Title"), Is.EqualTo("Field Notes"));
            Assert.That(GetProperty(openWindows[1], "Title"), Is.EqualTo("Gene Lab"));
            Assert.That(GetProperty(viewModel, "ActiveDesktopAppTitle"), Is.EqualTo("Gene Lab"));

            var fieldNotesWindow = openWindows[0];
            Assert.That(GetProperty(fieldNotesWindow, "FieldGuideSurfaceVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That((float)GetProperty(fieldNotesWindow, "Width"), Is.InRange(1040f, 1510f));
            Assert.That((float)GetProperty(fieldNotesWindow, "Height"), Is.InRange(560f, 820f));

            var geneLabWindow = openWindows[1];
            Assert.That((float)GetProperty(geneLabWindow, "Width"), Is.InRange(960f, 1450f));
            Assert.That((float)GetProperty(geneLabWindow, "Height"), Is.InRange(560f, 820f));
            Assert.That(GetProperty(geneLabWindow, "GenomeSpeciesNameText"), Is.EqualTo("HARE"));
            var profileSession = GameObject.Find("GalapagOS Desktop Test Camera")
                ?.GetComponent<Helper_ProfileSession>();
            Assert.That(profileSession, Is.Not.Null);
            profileSession.Current.TryGetGenomeProfile(new SpeciesId("hare"), out var activeGenome);
            Assert.That(GetProperty(geneLabWindow, "ActiveGenomeCapacityText"),
                Is.EqualTo($"{activeGenome?.CapacityUsed ?? 0} / 8"));
            Assert.That(GetProperty(geneLabWindow, "GenomeCatalogStateText"), Is.EqualTo("7 GENOME OPTIONS"));

            var genomeNodes = (IList)GetProperty(geneLabWindow, "GenomeNodes");
            Assert.That(genomeNodes.Count, Is.EqualTo(2));
            Assert.That(GetProperty(genomeNodes[0], "NodeId"), Is.EqualTo("fertile-droppings"));
            var genomeTreeSegments = (IList)GetProperty(geneLabWindow, "GenomeTreeSegments");
            Assert.That(genomeTreeSegments.Count, Is.EqualTo(3));
            Assert.That((float)GetProperty(geneLabWindow, "GenomeTreeWidth"), Is.GreaterThan(0f));
            Assert.That((float)GetProperty(geneLabWindow, "GenomeTreeHeight"), Is.GreaterThan(0f));

            var replacementNode = new SaltyGame.GenomeUpgradeNodeSnapshot(
                "hare.updated",
                "Updated Node",
                "Updated metadata.",
                new SaltyGame.SpeciesId("hare"));
            var replacementMap = new SaltyGame.SpeciesGenomeMapSnapshot(
                new SaltyGame.SpeciesId("hare"),
                new[] { replacementNode });
            var replacementCatalog = new SaltyGame.GenomeCatalogSnapshot(new[] { replacementMap });
            var refresh = geneLabWindow.GetType().GetMethod("RefreshGenomeCatalog");
            Assert.That(refresh, Is.Not.Null);
            refresh.Invoke(geneLabWindow, new object[] { replacementCatalog });

            genomeNodes = (IList)GetProperty(geneLabWindow, "GenomeNodes");
            Assert.That(genomeNodes.Count, Is.EqualTo(1));
            Assert.That(GetProperty(genomeNodes[0], "NodeId"), Is.EqualTo("hare.updated"));
            Assert.That(GetProperty(genomeNodes[0], "DisplayName"), Is.EqualTo("Updated Node"));
            Assert.That(GetProperty(geneLabWindow, "GenomeCatalogStateText"), Is.EqualTo("1 GENOME OPTION"));

            var preservedWindow = openWindows[0];
            Assert.That(preservedWindow, Is.Not.Null);
            var closeCommand = GetProperty(preservedWindow, "CloseCommand");
            closeCommand.GetType().GetMethod("Execute")?.Invoke(closeCommand, new object[] { null });

            Assert.That(openWindows.Count, Is.EqualTo(1));
            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Gene Lab" });
            Assert.That(openWindows.Count, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator GeneLabPlaytestOfferPurchasesPersistsAndFreezesForNextJourney()
        {
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            Assert.That(root, Is.Not.Null);
            var session = root.GetComponent<Helper_ProfileSession>();
            Assert.That(session, Is.Not.Null);
            session.SetPlaytimeTracking(false);
            session.CreateProfile("Gene Lab playtest");
            session.AddRabbitData(2);
            session.AddResearchData(1);
            for (var index = 1; index <= 3; index++)
                Assert.That(session.RecordCompletedRun($"genome-playtest-{index}"), Is.True);

            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var open = GetProperty(desktop, "OpenDesktopIconCommand");
            open.GetType().GetMethod("Execute")?.Invoke(open, new object[] { "Gene Lab" });
            var windows = (IList)GetProperty(desktop, "OpenDesktopWindows");
            Assert.That(windows.Count, Is.EqualTo(1));
            var lab = GetProperty(windows[0], "GenomeLab");
            Assert.That(GetProperty(lab, "CanPurchaseLicense"), Is.True);
            ExecuteCommand(lab, "PurchaseLicenseCommand");
            Assert.That(session.Current.RabbitData, Is.EqualTo(1));
            Assert.That(GetProperty(lab, "CanPurchaseNode"), Is.True);
            ExecuteCommand(lab, "PurchaseNodeCommand");
            Assert.That(session.Current.RabbitData, Is.Zero);
            Assert.That(GetProperty(windows[0], "ActiveGenomeCapacityText"), Is.EqualTo("1 / 8"));
            Assert.That(session.Current.CreateGenomeSnapshot()
                .GetSpeciesGenome(new SpeciesId("hare")).HasActiveNode("fertile-droppings"), Is.True);

            session.Reload();
            Assert.That(session.Current.CreateGenomeSnapshot()
                .GetSpeciesGenome(new SpeciesId("hare")).HasActiveNode("fertile-droppings"), Is.True);
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            Assert.That(preview, Is.Not.Null);
            Assert.That(preview.TrySetActiveGenomeBeforeJourney(
                session.Current.CreateGenomeSnapshot(), out var reason), Is.True, reason);
            Assert.That(preview.ActiveGenomeSnapshot
                .GetSpeciesGenome(new SpeciesId("hare")).HasActiveNode("fertile-droppings"), Is.True);

            Assert.That(GetProperty(lab, "CanResetGenome"), Is.True);
            ExecuteCommand(lab, "ResetGenomeCommand");
            Assert.That(session.Current.RabbitData, Is.EqualTo(1));
            Assert.That(session.Current.ResearchData, Is.Zero);
            Assert.That(session.Current.CreateGenomeSnapshot()
                .GetSpeciesGenome(new SpeciesId("hare")).HasActiveNode("fertile-droppings"), Is.False);
            Assert.That(preview.ActiveGenomeSnapshot
                .GetSpeciesGenome(new SpeciesId("hare")).HasActiveNode("fertile-droppings"), Is.True);
        }

        [UnityTest]
        public IEnumerator GeneLabPocSelectionAllocatesPersistsAndLaunchesTheChosenBuild()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            yield return null;
            var menu = GameObject.Find("Main Camera");
            var session = menu.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var profileId = session.CreateProfile("Gene selection POC").ProfileId;
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            var naturalDigestion = root.GetComponent<SpeciesSimulationPreview>()
                .ActiveSpeciesRules[new SpeciesId("hare")].DigestionEnergyBonus;
            session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            // Preserve the explicitly legacy POC economy regression; FD2 UI has its own scene checks.
            desktop.GetType().GetMethod("BindProfileSession").Invoke(desktop, new object[] { session, false });
            OpenDesktopApp(desktop, "Gene Lab");
            yield return null;
            yield return null;
            var windows = (IList)GetProperty(desktop, "OpenDesktopWindows");
            var lab = GetProperty(windows[0], "GenomeLab");
            var view = root.GetComponent<NoesisView>();
            var content = view.Content;
            var camera = root.GetComponent<Camera>();
            var output = GalapagOSVisualAcceptanceTests.TryGetVisualOutputDirectory();
            Assert.That(GetProperty(lab, "CanPurchaseLicense"), Is.False);
            if (output != null)
            {
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "gene-selection-01-locked", camera);
            }
            for (var index = 0; index < 3; index++)
            {
                session.RecordCompletedRun($"selection-poc-{index}");
            }
            session.AddRabbitData(1);
            session.AddResearchData(2);
            ExecuteCommand(lab, "PurchaseLicenseCommand");
            var branches = FindGeneControl<Noesis.ComboBox>(content, combo => combo.Items.Count == 4);
            Assert.That(branches, Is.Not.Null, "The branch selector must be present in the rendered XAML.");
            branches.SelectedItem = "Trailblazer";
            yield return null;
            Assert.That(GetProperty(lab, "SelectedBranch"), Is.EqualTo("Trailblazer"));
            Assert.That(((IList)GetProperty(lab, "Nodes")).Count, Is.EqualTo(4));
            SelectGeneButton(content, "poc.hare.long-stride");
            Assert.That(GetProperty(lab, "CanPurchaseNode"), Is.False);
            Assert.That(GetProperty(lab, "ActionStatusText"), Does.Contain("Rabbit Data"));
            session.AddRabbitData(10);
            yield return null;
            SelectGeneButton(content, "poc.hare.long-vision");
            Assert.That(GetProperty(lab, "CanPurchaseNode"), Is.False);
            Assert.That(GetProperty(lab, "ActionStatusText"), Does.Contain("Long Stride"));
            Assert.That(GetProperty(GetProperty(lab, "SelectedNode"), "StateText"), Is.EqualTo("REQUIRES NODE"));
            if (output != null)
            {
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "gene-selection-02-prerequisite", camera);
            }
            SelectGeneButton(content, "poc.hare.long-stride");
            ExecuteCommand(lab, "PurchaseNodeCommand");
            Assert.That(GetProperty(GetProperty(lab, "SelectedNode"), "NodeId"), Is.EqualTo("poc.hare.long-stride"));
            Assert.That(GetProperty(GetProperty(lab, "SelectedNode"), "StateText"), Is.EqualTo("ACTIVE"));
            Assert.That(GetProperty(lab, "ActiveGenomeCapacityText"), Is.EqualTo("2 / 8"));
            Assert.That(session.Current.RabbitData, Is.EqualTo(9));
            yield return null;
            var strideButton = FindGeneNodeButton(content, "poc.hare.long-stride");
            Assert.That(strideButton.BorderThickness.Left, Is.EqualTo(4), "The selected node must have its visual highlight.");
            SelectGeneButton(content, "poc.hare.long-vision");
            ExecuteCommand(lab, "PurchaseNodeCommand");
            branches.SelectedItem = "Warren";
            yield return null;
            SelectGeneButton(content, "poc.hare.burrows-preview");
            Assert.That(GetProperty(lab, "CanPurchaseNode"), Is.False);
            Assert.That(GetProperty(lab, "NodeOfferText"), Is.EqualTo("PREVIEW ONLY"));
            Assert.That(GetProperty(lab, "ActionStatusText"), Does.Contain("not implemented"));
            if (output != null)
            {
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "gene-selection-02b-preview", camera);
            }
            SelectGeneButton(content, "poc.hare.tough-hide");
            ExecuteCommand(lab, "PurchaseNodeCommand");
            yield return null;
            SelectGeneButton(content, "poc.hare.larger-litters");
            ExecuteCommand(lab, "PurchaseNodeCommand");
            Assert.That(GetProperty(lab, "ActiveGenomeCapacityText"), Is.EqualTo("8 / 8"));
            foreach (var slot in (IList)GetProperty(lab, "CapacitySlots"))
            {
                Assert.That(slot, Is.EqualTo(true));
            }
            branches.SelectedItem = "Gardener";
            yield return null;
            SelectGeneButton(content, "fertile-droppings");
            Assert.That(GetProperty(lab, "CanPurchaseNode"), Is.False);
            Assert.That(GetProperty(lab, "ActionStatusText"), Does.Contain("capacity"));
            var beforeBlockedPurchase = session.Current.RabbitData;
            ExecuteCommand(lab, "PurchaseNodeCommand");
            Assert.That(session.Current.RabbitData, Is.EqualTo(beforeBlockedPurchase));
            if (output != null)
            {
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "gene-selection-03-capacity", camera);
            }
            ExecuteCommand(lab, "ResetGenomeCommand");
            Assert.That(session.Current.RabbitData, Is.EqualTo(10));
            Assert.That(session.Current.ResearchData, Is.EqualTo(1));
            Assert.That(GetProperty(lab, "ActiveGenomeCapacityText"), Is.EqualTo("0 / 8"));
            ExecuteCommand(lab, "PurchaseNodeCommand");
            yield return null;
            SelectGeneButton(content, "poc.hare.efficient-digestion");
            ExecuteCommand(lab, "PurchaseNodeCommand");
            session.Reload();
            Assert.That(GetProperty(GetProperty(lab, "SelectedNode"), "NodeId"), Is.EqualTo("poc.hare.efficient-digestion"));
            Assert.That(GetProperty(lab, "ActiveGenomeCapacityText"), Is.EqualTo("3 / 8"));
            Assert.That(session.Current.RabbitData, Is.EqualTo(8));
            if (output != null)
            {
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "gene-selection-04-active-build", camera);
            }
            // Rebuild the scene as well as the session: saved POC IDs must resolve at a fresh Desktop launch.
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            yield return null;
            menu = GameObject.Find("Main Camera");
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            root = GameObject.Find("GalapagOS Desktop Test Camera");
            session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            OpenDesktopApp(desktop, "Gene Lab");
            windows = (IList)GetProperty(desktop, "OpenDesktopWindows");
            lab = GetProperty(windows[0], "GenomeLab");
            Assert.That(session.Current.ProfileId, Is.EqualTo(profileId));
            Assert.That(session.Current.RabbitData, Is.EqualTo(8));
            Assert.That(GetProperty(lab, "ActiveGenomeCapacityText"), Is.EqualTo("3 / 8"));
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            Assert.That(preview.ActiveSpeciesRules[new SpeciesId("hare")].DigestionEnergyBonus, Is.EqualTo(naturalDigestion + 0.25f));
            ConfigureShortRewardJourney(preview);
            StartRewardJourney(desktop, preview);
            Assert.That(preview.ActiveSpeciesRules[new SpeciesId("hare")].DigestionEnergyBonus, Is.EqualTo(naturalDigestion + 0.25f));
            var frozen = preview.ActiveGenomeSnapshot.Fingerprint;
            ExecuteCommand(lab, "ResetGenomeCommand");
            Assert.That(preview.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(frozen));
            preview.EndSimulation();
            OpenDesktopApp(desktop, "Simulation");
            Assert.That(preview.ActiveSpeciesRules[new SpeciesId("hare")].DigestionEnergyBonus, Is.EqualTo(naturalDigestion));
            Assert.That(session.Current.ProfileId, Is.EqualTo(profileId));
        }

        static Noesis.Button FindGeneNodeButton(Noesis.DependencyObject content, string nodeId)
        {
            return FindGeneControl<Noesis.Button>(content, button =>
                button.DataContext?.GetType().GetProperty("NodeId")?.GetValue(button.DataContext) as string == nodeId);
        }

        static void SelectGeneButton(Noesis.DependencyObject content, string nodeId)
        {
            var button = FindGeneNodeButton(content, nodeId);
            Assert.That(button, Is.Not.Null, $"No rendered node button for '{nodeId}'.");
            Assert.That(button.Command, Is.Not.Null, "The XAML node command must be bound.");
            button.Command.Execute(button.CommandParameter);
        }

        static T FindGeneControl<T>(Noesis.DependencyObject element, System.Func<T, bool> matches) where T : Noesis.DependencyObject
        {
            if (element is T candidate && matches(candidate))
            {
                return candidate;
            }
            for (var index = 0; index < Noesis.VisualTreeHelper.GetChildrenCount(element); index++)
            {
                var child = FindGeneControl(Noesis.VisualTreeHelper.GetChild(element, index), matches);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        [UnityTest]
        public IEnumerator GeneLabPopulationUpgradesPersistLaunchOnceAndLimitRefills()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            yield return null;
            var menu = GameObject.Find("Main Camera");
            var session = menu.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            session.CreateProfile("Population Genome automation");
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            for (var i = 0; i < 3; i++) session.RecordCompletedRun($"population-fixture-{i}");
            session.AddRabbitData(3);
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            // Preserve the explicitly legacy POC economy regression; FD2 UI has its own scene checks.
            desktop.GetType().GetMethod("BindProfileSession").Invoke(desktop, new object[] { session, false });
            OpenDesktopApp(desktop, "Gene Lab");
            yield return null;
            var windows = (IList)GetProperty(desktop, "OpenDesktopWindows");
            var lab = GetProperty(windows[0], "GenomeLab");
            Assert.That(GetProperty(lab, "CanPurchaseLicense"), Is.True);
            ExecuteCommand(lab, "PurchaseLicenseCommand");
            var content = root.GetComponent<NoesisView>().Content;
            var branches = FindGeneControl<Noesis.ComboBox>(content, combo => combo.Items.Count == 4);
            branches.SelectedItem = "Warren";
            yield return null;
            SelectGeneButton(content, "poc.hare.starting-population");
            Assert.That(GetProperty(lab, "CanPurchaseNode"), Is.True);
            ExecuteCommand(lab, "PurchaseNodeCommand");
            yield return null;
            SelectGeneButton(content, "poc.hare.population-pool");
            Assert.That(GetProperty(lab, "CanPurchaseNode"), Is.True);
            ExecuteCommand(lab, "PurchaseNodeCommand");
            yield return null;
            Assert.That(GetProperty(lab, "ActiveGenomeCapacityText"), Is.EqualTo("4 / 8"));
            session.Reload();
            var output = GalapagOSVisualAcceptanceTests.TryGetVisualOutputDirectory();
            if (output != null)
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "population-01-genome", root.GetComponent<Camera>());
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out var reason), Is.True, reason);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "12", "8", "10100", "0", "0", "12", "0.1", "0", "0", "0",
                false, "20", "4", "0", out reason), Is.True, reason);
            StartRewardJourney(desktop, preview);
            Assert.That(preview.Run.PopulationHistory[0].GetCount(new SpeciesId("hare")), Is.EqualTo(9));
            Assert.That(preview.Run.PopulationPoolCapacity, Is.EqualTo(60));
            Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            preview.Progression.AddCurrency(1000);
            var purchases = 0;
            while (preview.CanBuyHare && purchases++ < 100) Assert.That(preview.BuyHare(), Is.True);
            Assert.That(preview.Run.PopulationHistory[preview.Run.PopulationHistory.Count - 1].GetCount(new SpeciesId("hare")), Is.EqualTo(60));
            var currency = preview.Progression.Currency;
            Assert.That(preview.BuyHare(), Is.False);
            Assert.That(preview.Progression.Currency, Is.EqualTo(currency));
            yield return null;
            var simulationCamera = GameObject.Find("GalapagOS Simulation View").GetComponent<Camera>();
            var shell = root.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(shell, Is.Not.Null);
            ExecuteCommand(shell, "CloseJourneyMapCommand");
            yield return null;
            Assert.That(GetProperty(shell, "HarePurchaseText"), Does.Contain("POOL FULL"));
            var populationItems = (System.Array)GetProperty(shell, "PopulationControlItems");
            Assert.That(GetProperty(populationItems.GetValue(0), "CanChoose"), Is.EqualTo(false));
            Assert.That(GetProperty(populationItems.GetValue(0), "Description"), Does.Contain("pool full"));
            if (output != null)
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "population-02-full-pool", simulationCamera);
            preview.ContinueWithoutUpgrade();
            Assert.That(preview.Run.PopulationHistory[preview.Run.PopulationHistory.Count - 1].GetCount(new SpeciesId("hare")), Is.EqualTo(60));
        }

        static void ExecuteGeneAction(Noesis.DependencyObject content, object lab, string command)
        {
            var intent = GetProperty(lab, command);
            var button = FindGeneControl<Noesis.Button>(content, candidate => object.ReferenceEquals(candidate.Command, intent));
            Assert.That(button, Is.Not.Null, "Missing bound Gene Lab control: " + command);
            Assert.That(button.IsEnabled, Is.True, "Disabled Gene Lab control: " + command);
            button.Command.Execute(button.CommandParameter);
        }

        [UnityTest]
        public IEnumerator FD2GeneLabPurchasesArrangesAndLaunchesTheSavedPermanentBuild()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            var session = menu.GetComponent<Helper_ProfileSession>();
            var profileId = session.CreateProfile("FD2 Gene Lab UI").ProfileId;
            session.AddRabbitData(60);
            session.AddResearchData(200);
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            OpenDesktopApp(desktop, "Gene Lab");
            yield return null;
            yield return null;
            var window = ((IList)GetProperty(desktop, "OpenDesktopWindows"))[0];
            var lab = GetProperty(window, "GenomeLab");
            var content = root.GetComponent<NoesisView>().Content;
            Assert.That(((IList)GetProperty(lab, "Nodes")).Count, Is.EqualTo(9));
            Assert.That(GetProperty(lab, "CanPurchaseLicense"), Is.True, "FD2 has no three-journey license gate.");
            Assert.That(GetProperty(lab, "LicenseReasonText").ToString(), Does.Contain("60 -> 50"));
            ExecuteGeneAction(content, lab, "PurchaseResearchCommand");
            ExecuteGeneAction(content, lab, "PurchaseLicenseCommand");
            yield return null;
            var concept = "fast-gestation";
            SelectGeneButton(content, concept);
            var conceptNode = GetProperty(lab, "SelectedNode");
            Assert.That(GetProperty(conceptNode, "IsConcept"), Is.True);
            Assert.That(GetProperty(conceptNode, "StateText"), Is.EqualTo("CONCEPT · LOCKED"));
            Assert.That(GetProperty(conceptNode, "CapacityText"), Does.Contain("RULES TBD"));
            Assert.That(GetProperty(lab, "NodeOfferText"), Is.EqualTo("CONCEPT · LOCKED"));
            Assert.That(GetProperty(lab, "NodeReasonText"), Does.Contain("preview only"));
            Assert.That(GetProperty(lab, "CanPurchaseNode"), Is.False);
            var dataBeforeConceptPurchase = session.Current.RabbitData;
            ExecuteCommand(lab, "PurchaseNodeCommand");
            Assert.That(session.Current.RabbitData, Is.EqualTo(dataBeforeConceptPurchase));
            var conceptOutput = GalapagOSVisualAcceptanceTests.TryGetVisualOutputDirectory();
            if (conceptOutput != null)
            {
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(conceptOutput,
                    "genome-permanent-concept-locked", root.GetComponent<Camera>());
            }
            window.GetType().GetMethod("RefreshGenomeCatalog").Invoke(window, new object[] { GenomeCatalogSnapshot.Empty });
            Assert.That(((IList)GetProperty(lab, "Nodes")).Count, Is.EqualTo(5));
            yield return null;
            Assert.That(session.Current.ResearchData, Is.EqualTo(190));
            Assert.That(session.Current.RabbitData, Is.EqualTo(50));
            var ids = FieldDataPermanentProgressionDefinition.CreateStarter().HareMap.Nodes.Select(node => node.Id).ToArray();
            SelectGeneButton(content, ids[3]);
            Assert.That(GetProperty(lab, "CanPurchaseNode"), Is.False);
            Assert.That(GetProperty(lab, "NodeReasonText").ToString(), Does.Contain("Own first"));
            foreach (var id in ids.Take(4))
            {
                SelectGeneButton(content, id);
                yield return null;
                ExecuteGeneAction(content, lab, "PurchaseNodeCommand");
                yield return null;
            }
            Assert.That(GetProperty(lab, "ActiveGenomeCapacityText"), Is.EqualTo("8 / 8"));
            SelectGeneButton(content, ids[4]);
            Assert.That(GetProperty(lab, "CanPurchaseNode"), Is.False);
            Assert.That(GetProperty(lab, "NodeReasonText").ToString(), Does.Contain("capacity"));
            var output = GalapagOSVisualAcceptanceTests.TryGetVisualOutputDirectory();
            if (output != null && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "t11-owned-full-capacity", root.GetComponent<Camera>());
            ExecuteGeneAction(content, lab, "ResetGenomeCommand");
            yield return null;
            Assert.That(session.Current.RabbitData, Is.EqualTo(30), "Reset never refunds permanent purchases.");
            Assert.That(session.Current.ResearchData, Is.EqualTo(170));
            Assert.That(GetProperty(lab, "CanPurchaseNode"), Is.False);
            Assert.That(GetProperty(lab, "NodeReasonText").ToString(), Does.Contain("Apply your replacement"));
            var blockedPurchase = GetProperty(lab, "PurchaseNodeCommand");
            var blockedButton = FindGeneControl<Noesis.Button>(content, button => object.ReferenceEquals(button.Command, blockedPurchase));
            Assert.That(blockedButton.IsEnabled, Is.False, "Buying a new node must not consume an unfinished reset.");
            if (output != null && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "t12-reset-protects-free-selection", root.GetComponent<Camera>());
            SelectGeneButton(content, ids[0]);
            ExecuteGeneAction(content, lab, "ToggleLoadoutNodeCommand");
            yield return null;
            ExecuteGeneAction(content, lab, "ApplyLoadoutCommand");
            yield return null;
            SelectGeneButton(content, ids[4]);
            yield return null;
            ExecuteGeneAction(content, lab, "PurchaseNodeCommand");
            yield return null;
            ExecuteGeneAction(content, lab, "ResetGenomeCommand");
            yield return null;
            foreach (var id in ids)
            {
                SelectGeneButton(content, id);
                yield return null;
                ExecuteGeneAction(content, lab, "ToggleLoadoutNodeCommand");
                yield return null;
            }
            Assert.That(GetProperty(lab, "CanApplyLoadout"), Is.False);
            Assert.That(GetProperty(lab, "ArrangementReasonText").ToString(), Does.Contain("exceeds"));
            if (output != null && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
            {
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "t11-selection-over-capacity", root.GetComponent<Camera>());
                var apply = GetProperty(lab, "ApplyLoadoutCommand");
                var inspector = FindGeneControl<Noesis.ScrollViewer>(content, viewer =>
                    FindGeneControl<Noesis.Button>(viewer, button => object.ReferenceEquals(button.Command, apply)) != null);
                Assert.That(inspector, Is.Not.Null);
                inspector.ScrollToVerticalOffset(inspector.ScrollableHeight);
                yield return null;
                if (inspector.ScrollableHeight > 0) Assert.That(inspector.VerticalOffset, Is.GreaterThan(0));
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "t11-capacity-explanation-scrolled", root.GetComponent<Camera>());
                inspector.ScrollToVerticalOffset(0);
                yield return null;
            }
            SelectGeneButton(content, ids[0]);
            ExecuteGeneAction(content, lab, "ToggleLoadoutNodeCommand");
            yield return null;
            session.AddResearchData(1);
            yield return null;
            Assert.That(GetProperty(lab, "SelectionText").ToString(), Does.Contain("8 / 8"), "Wallet refresh must retain the draft.");
            if (output != null && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "t11-ready-to-apply", root.GetComponent<Camera>());
            ExecuteGeneAction(content, lab, "ApplyLoadoutCommand");
            yield return null;
            Assert.That(session.Current.RabbitData, Is.EqualTo(25));
            Assert.That(session.Current.ResearchData, Is.EqualTo(151));
            Assert.That(GetProperty(lab, "CanApplyLoadout"), Is.False);
            Assert.That(GetProperty(GetProperty(lab, "SelectedNode"), "StateText").ToString(), Does.Contain("INACTIVE"));
            session.Reload();
            yield return null;
            Assert.That(GetProperty(lab, "ActiveGenomeCapacityText"), Is.EqualTo("8 / 8"));
            if (output != null && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "t11-saved-loadout", root.GetComponent<Camera>());
            ExecuteCommand(window, "CloseCommand");
            OpenDesktopApp(desktop, "Gene Lab");
            yield return null;
            window = ((IList)GetProperty(desktop, "OpenDesktopWindows"))[0];
            lab = GetProperty(window, "GenomeLab");
            Assert.That(GetProperty(lab, "ActiveGenomeCapacityText"), Is.EqualTo("8 / 8"));
            OpenDesktopApp(desktop, "Simulation");
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            SpeciesPresentationPlayModeTests.ConfigureAuthoredFieldDataScenario(preview);
            Assert.That(preview.TryConfigureFieldDataTestJourney(out var error), Is.True, error);
            preview.StartSimulation();
            preview.PauseSimulation();
            var hare = new SpeciesId("hare");
            Assert.That(preview.Run.ActiveGenomeSnapshot.GetSpeciesGenome(hare).ActiveNodeIds, Is.EqualTo(ids.Skip(1)));
            var frozen = preview.Run.ActiveGenomeSnapshot.Fingerprint;
            ExecuteCommand(lab, "ResetGenomeCommand");
            Assert.That(preview.Run.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(frozen));
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            Assert.That(session.Current.ProfileId, Is.EqualTo(profileId));
            Assert.That(session.Current.RabbitData, Is.EqualTo(25));
            Assert.That(session.Current.TryGetGenomeProfile(hare, out var genome), Is.True);
            Assert.That(genome.UnlockedNodeIds.Count, Is.EqualTo(5));
            Assert.That(genome.ActiveNodeIds, Is.Empty);
        }

        [UnityTest]
        public IEnumerator FD2GeneLabExplainsInsufficientFundsAndRejectsOldProfileCommands()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            menu.GetComponent<Helper_ProfileSession>().CreateProfile("Empty permanent UI");
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            var session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            OpenDesktopApp(desktop, "Gene Lab");
            yield return null;
            yield return null;
            var lab = GetProperty(((IList)GetProperty(desktop, "OpenDesktopWindows"))[0], "GenomeLab");
            Assert.That(GetProperty(lab, "CanPurchaseLicense"), Is.False);
            Assert.That(GetProperty(lab, "LicenseReasonText").ToString(), Does.Contain("Needs 10 Hare Data"));
            Assert.That(GetProperty(lab, "ResearchReasonText").ToString(), Does.Contain("Needs 10 Research Data"));
            ExecuteCommand(lab, "PurchaseResearchCommand");
            Assert.That(GetProperty(lab, "ActionStatusText").ToString(), Does.Contain("Needs 10 Research Data"));
            var output = GalapagOSVisualAcceptanceTests.TryGetVisualOutputDirectory();
            if (output != null && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "t11-insufficient-funds", root.GetComponent<Camera>());
            session.CreateProfile("New owner");
            session.AddRabbitData(100);
            session.AddResearchData(100);
            yield return null;
            ExecuteCommand(lab, "PurchaseLicenseCommand");
            ExecuteCommand(lab, "PurchaseResearchCommand");
            Assert.That(session.Current.RabbitData, Is.EqualTo(100));
            Assert.That(session.Current.ResearchData, Is.EqualTo(100));
            Assert.That(GetProperty(lab, "ActionStatusText").ToString(), Does.Contain("Reopen Gene Lab"));
        }

        [UnityTest]
        public IEnumerator FD2GeneLabSaveFailureKeepsWalletAndSelectionUntilRetrySaves()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            menu.GetComponent<Helper_ProfileSession>().CreateProfile("Real UI host");
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var storage = new ResultsStorage();
            var isolated = NewResultsProfile(storage, out var owner);
            desktop.GetType().GetMethod("BindProfileSession").Invoke(desktop, new object[] { isolated, true });
            try
            {
                OpenDesktopApp(desktop, "Gene Lab");
                yield return null;
                yield return null;
                var lab = GetProperty(((IList)GetProperty(desktop, "OpenDesktopWindows"))[0], "GenomeLab");
                var content = root.GetComponent<NoesisView>().Content;
                var before = isolated.Current;
                storage.FailOnSave = storage.Saves + 1;
                ExecuteGeneAction(content, lab, "ResetGenomeCommand");
                Assert.That(isolated.Current, Is.SameAs(before));
                Assert.That(GetProperty(lab, "ActionStatusText").ToString(), Does.Contain("persistence test failure"));
                var output = GalapagOSVisualAcceptanceTests.TryGetVisualOutputDirectory();
                if (output != null && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                    yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "t11-save-failure", root.GetComponent<Camera>());
                storage.FailOnSave = -1;
                ExecuteGeneAction(content, lab, "ResetGenomeCommand");
                yield return null;
                SelectGeneButton(content, "hare.warren-network");
                ExecuteGeneAction(content, lab, "ToggleLoadoutNodeCommand");
                yield return null;
                before = isolated.Current;
                storage.FailOnSave = storage.Saves + 1;
                ExecuteGeneAction(content, lab, "ApplyLoadoutCommand");
                Assert.That(isolated.Current, Is.SameAs(before));
                Assert.That(GetProperty(lab, "SelectionText").ToString(), Does.Contain("2 / 8"));
                storage.FailOnSave = -1;
                ExecuteGeneAction(content, lab, "ApplyLoadoutCommand");
                Assert.That(isolated.Current.ResearchData, Is.EqualTo(before.ResearchData));
                isolated.Reload();
                Assert.That(isolated.Current.TryGetGenomeProfile(new SpeciesId("hare"), out var genome), Is.True);
                Assert.That(genome.ActiveNodeIds, Is.EqualTo(new[] { "hare.warren-network" }));
            }
            finally
            {
                desktop.GetType().GetMethod("BindProfileSession").Invoke(desktop, new object[] { root.GetComponent<Helper_ProfileSession>(), true });
                Object.Destroy(owner);
            }
        }

        [UnityTest]
        public IEnumerator FD2GeneLabRetainsLegacyRefundableAllocationsAndCanReopenPermanentControls()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            menu.GetComponent<Helper_ProfileSession>().CreateProfile("Migrated POC UI");
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            var session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            for (var i = 0; i < 3; i++) session.RecordCompletedRun("legacy-ui-" + i);
            session.AddRabbitData(4);
            session.AddResearchData(3);
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var map = ((GenomeCatalogSnapshot)GetProperty(desktop, "GenomeCatalog")).GetSpeciesMap(new SpeciesId("hare"));
            var node = map.GetNode("fertile-droppings");
            Assert.That(session.TryPurchaseRabbitGenomeLicense(GenomeContract.RabbitTreeLicensePrice, out var error), Is.True, error);
            Assert.That(session.TryPurchaseRabbitGenomeNode(map, node.Id, node.RabbitDataPrice, node.CapacityPoints, out error), Is.True, error);
            OpenDesktopApp(desktop, "Gene Lab");
            yield return null;
            yield return null;
            var window = ((IList)GetProperty(desktop, "OpenDesktopWindows"))[0];
            var lab = GetProperty(window, "GenomeLab");
            Assert.That(GetProperty(lab, "EconomyModeText").ToString(), Does.Contain("LEGACY POC"));
            Assert.That(GetProperty(lab, "PermanentControlsVisibility").ToString(), Is.EqualTo("Collapsed"));
            var balance = session.Current.RabbitData;
            ExecuteGeneAction(root.GetComponent<NoesisView>().Content, lab, "ResetGenomeCommand");
            Assert.That(session.Current.RabbitData, Is.EqualTo(balance + node.RabbitDataPrice));
            Assert.That(session.Current.ResearchData, Is.EqualTo(3 - GenomeContract.RabbitResetResearchDataFee));
            ExecuteCommand(window, "CloseCommand");
            OpenDesktopApp(desktop, "Gene Lab");
            yield return null;
            lab = GetProperty(((IList)GetProperty(desktop, "OpenDesktopWindows"))[0], "GenomeLab");
            Assert.That(GetProperty(lab, "PermanentControlsVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(((IList)GetProperty(lab, "Nodes")).Count, Is.EqualTo(9));
        }

        static void ConfigurePermanentAcceptanceJourney(GameObject root)
        {
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            OpenDesktopApp(root.GetComponent("SaltyGame.VM_GalapagOS_Desktop"), "Simulation");
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready), "Opening the next Field Data journey must wait at the map for an explicit start.");
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "36", "20", "10100", "0", "0", "1800", "0.1", "0", "0", "0", false,
                "200", "20", "0", out var error), Is.True, error);
            var hare = new SpeciesId("hare");
            var rules = preview.GetSpeciesRuleEditValues(hare);
            rules.MovementEnabled = false;
            rules.ReproductionEnabled = false;
            rules.Metabolism = "0";
            rules.StartingEnergy = "100";
            rules.MaximumEnergy = "100";
            Assert.That(preview.TryApplySpeciesRuleEditValues(hare, rules, out error), Is.True, error);
            Assert.That(preview.TryConfigureFieldDataTestJourney(out error), Is.True, error);
            ExecuteCommand(root.GetComponent("SaltyGame.VM_SimulationShell"), "StartFieldDataTestJourneyCommand");
            preview.PauseSimulation();
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Paused));
            Assert.That(preview.Run.PopulationHistory[0].GetCount(hare), Is.EqualTo(20));
        }

        static IEnumerator CompletePermanentAcceptanceJourney(GameObject root)
        {
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            var shell = (MonoBehaviour)root.GetComponent("SaltyGame.VM_SimulationShell");
            for (var round = 1; round <= 18; round++)
            {
                for (var tick = 0; tick < 100; tick++)
                    Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True, $"Acceptance round {round}, tick {preview.Run.Tick}");
                Assert.That(preview.Run.PopulationHistory.Last().GetCount(new SpeciesId("hare")), Is.EqualTo(20));
                if (round == 18) break;
                ExecuteCommand(shell, "ContinueWithoutUpgradeCommand");
                ExecuteCommand(shell, "ContinueJourneyCommand");
                if (round % 6 == 0)
                {
                    SpeciesPresentationPlayModeTests.EnterAvailableFieldDataNode(shell, round / 6 * 2 - 1);
                    Assert.That(GetProperty(shell, "FieldDataShopVisibility").ToString(), Is.EqualTo("Visible"));
                    ExecuteCommand(shell, "ContinueJourneyCommand");
                    SpeciesPresentationPlayModeTests.EnterAvailableFieldDataNode(shell, round / 6 * 2);
                }
                preview.PauseSimulation();
                yield return null;
            }
            yield return null;
            Assert.That(preview.FieldDataSession.State, Is.EqualTo(FieldDataJourneyFlowState.Succeeded));
            Assert.That(preview.PermanentResults.IsPending, Is.False, preview.PermanentResults.Error);
            Assert.That(preview.PermanentResults.Receipt.LeftoverFieldData, Is.EqualTo(390));
            Assert.That(preview.PermanentResults.Receipt.ResearchDataBonus, Is.EqualTo(10));
        }

        static IEnumerator CapturePermanentAcceptance(GameObject root, string name, bool simulation = false)
        {
            var output = GalapagOSVisualAcceptanceTests.TryGetVisualOutputDirectory();
            if (output != null && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, name,
                    simulation ? GameObject.Find("GalapagOS Simulation View").GetComponent<Camera>() : root.GetComponent<Camera>());
        }

        static void AssertAcceptanceReceiptCannotPayAgain(Helper_ProfileSession session, SpeciesSimulationPreview preview)
        {
            var receipt = preview.PermanentResults.Receipt;
            var before = session.Current;
            Assert.That(session.TrySettleFieldDataJourney(before.ProfileId, receipt.JourneyId,
                FieldDataJourneyFlowState.Succeeded, new SpeciesId("hare"), 999999, out var duplicate, out var error), Is.True, error);
            Assert.That(duplicate.LeftoverFieldData, Is.EqualTo(receipt.LeftoverFieldData));
            Assert.That(session.Current, Is.SameAs(before));
            session.Reload();
            Assert.That(session.Current.RabbitData, Is.EqualTo(before.RabbitData));
            Assert.That(session.Current.ResearchData, Is.EqualTo(before.ResearchData));
            Assert.That(session.Current.CompletedRuns, Is.EqualTo(before.CompletedRuns));
        }

        [UnityTest]
        public IEnumerator FD2AcceptanceNewProfileEarnsPurchasesResetsReloadsLaunchesAndRetainsProgressOnFailure()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            var session = menu.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var profileId = session.CreateProfile("FD2 zero-wallet acceptance").ProfileId;
            Assert.That(session.Current.RabbitData, Is.Zero);
            Assert.That(session.Current.ResearchData, Is.Zero);
            Assert.That(session.Current.GenomeProfiles, Is.Empty);
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            var shell = root.GetComponent("SaltyGame.VM_SimulationShell");
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var nodes = FieldDataPermanentProgressionDefinition.CreateStarter().HareMap.Nodes;
            ConfigurePermanentAcceptanceJourney(root);
            Assert.That(preview.Run.ActiveGenomeSnapshot.GetSpeciesGenome(new SpeciesId("hare")).ActiveNodeIds, Is.Empty);
            yield return CompletePermanentAcceptanceJourney(root);
            Assert.That(session.Current.RabbitData, Is.EqualTo(39));
            Assert.That(session.Current.ResearchData, Is.EqualTo(10));
            yield return CapturePermanentAcceptance(root, "t12-01-first-earned-success", true);
            AssertAcceptanceReceiptCannotPayAgain(session, preview);
            ExecuteCommand(shell, "ReturnToLabCommand");
            OpenDesktopApp(desktop, "Gene Lab");
            yield return null;
            yield return null;
            var window = ((IList)GetProperty(desktop, "OpenDesktopWindows"))[0];
            var lab = GetProperty(window, "GenomeLab");
            var content = root.GetComponent<NoesisView>().Content;
            ExecuteGeneAction(content, lab, "PurchaseResearchCommand");
            ExecuteGeneAction(content, lab, "PurchaseLicenseCommand");
            yield return null;
            foreach (var node in nodes.Take(4))
            {
                SelectGeneButton(content, node.Id);
                yield return null;
                ExecuteGeneAction(content, lab, "PurchaseNodeCommand");
                yield return null;
            }
            Assert.That(session.Current.RabbitData, Is.EqualTo(9));
            Assert.That(session.Current.ResearchData, Is.Zero);
            Assert.That(GetProperty(lab, "CanResetGenome"), Is.False);
            yield return CapturePermanentAcceptance(root, "t12-02-earned-genome-purchases");
            ExecuteCommand(window, "CloseCommand");
            for (var journey = 2; journey <= 3; journey++)
            {
                ConfigurePermanentAcceptanceJourney(root);
                yield return CompletePermanentAcceptanceJourney(root);
                Assert.That(preview.PermanentResults.Receipt.ConversionResearchLevel, Is.EqualTo(1));
                Assert.That(preview.PermanentResults.Receipt.SpeciesDataDeposit, Is.EqualTo(42));
                Assert.That(session.Current.CompletedRuns, Is.EqualTo(journey));
                AssertAcceptanceReceiptCannotPayAgain(session, preview);
                ExecuteCommand(shell, "ReturnToLabCommand");
            }
            Assert.That(session.Current.RabbitData, Is.EqualTo(93));
            Assert.That(session.Current.ResearchData, Is.EqualTo(20));
            OpenDesktopApp(desktop, "Gene Lab");
            yield return null;
            window = ((IList)GetProperty(desktop, "OpenDesktopWindows"))[0];
            lab = GetProperty(window, "GenomeLab");
            ExecuteGeneAction(content, lab, "ResetGenomeCommand");
            yield return null;
            SelectGeneButton(content, nodes[1].Id);
            ExecuteGeneAction(content, lab, "ToggleLoadoutNodeCommand");
            yield return null;
            ExecuteGeneAction(content, lab, "ApplyLoadoutCommand");
            Assert.That(session.Current.RabbitData, Is.EqualTo(93));
            Assert.That(session.Current.ResearchData, Is.Zero);
            yield return null;
            yield return CapturePermanentAcceptance(root, "t12-03-earned-reset-and-saved-loadout");
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            menu = GameObject.Find("Main Camera");
            session = menu.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.Current.ProfileId, Is.EqualTo(profileId));
            Assert.That(session.Current.RabbitData, Is.EqualTo(93));
            Assert.That(session.Current.ResearchData, Is.Zero);
            Assert.That(session.Current.SettlementReceipts.Count, Is.EqualTo(3));
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            root = GameObject.Find("GalapagOS Desktop Test Camera");
            session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            preview = root.GetComponent<SpeciesSimulationPreview>();
            shell = root.GetComponent("SaltyGame.VM_SimulationShell");
            ConfigurePermanentAcceptanceJourney(root);
            Assert.That(preview.Run.ActiveGenomeSnapshot.GetSpeciesGenome(new SpeciesId("hare")).ActiveNodeIds,
                Is.EqualTo(new[] { nodes[1].Id }));
            var beforeFailure = session.Current;
            for (var tick = 0; tick < 100; tick++) Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            ExecuteCommand(shell, "ContinueWithoutUpgradeCommand");
            ExecuteCommand(shell, "ContinueJourneyCommand");
            preview.PauseSimulation();
            ExecuteCommand(shell, "EndCommand");
            Assert.That(GetProperty(shell, "EndConfirmationVisibility").ToString(), Is.EqualTo("Visible"));
            ExecuteCommand(shell, "ConfirmEndCommand");
            yield return null;
            Assert.That(preview.FieldDataSession.State, Is.EqualTo(FieldDataJourneyFlowState.Failed));
            Assert.That(preview.FieldDataSession.FieldDataLostOnFailure, Is.EqualTo(20));
            Assert.That(session.Current.RabbitData, Is.EqualTo(beforeFailure.RabbitData));
            Assert.That(session.Current.ResearchData, Is.EqualTo(beforeFailure.ResearchData));
            Assert.That(session.Current.CompletedRuns, Is.EqualTo(3));
            Assert.That(session.Current.CreateGenomeSnapshot().Fingerprint, Is.EqualTo(beforeFailure.CreateGenomeSnapshot().Fingerprint));
            yield return CapturePermanentAcceptance(root, "t12-04-failure-keeps-earned-profile", true);
            ExecuteCommand(shell, "ReturnToLabCommand");
            session.Reload();
            Assert.That(session.Current.SettlementReceipts.Count, Is.EqualTo(3));
            Assert.That(session.Current.RabbitData, Is.EqualTo(93));
        }

        [UnityTest]
        public IEnumerator FD2AcceptanceVersionTwoMigrationCompletesThePermanentLoop() => PermanentMigrationAcceptance("v2");

        [UnityTest]
        public IEnumerator FD2AcceptanceLocalVersionThreeMigrationCompletesThePermanentLoop() => PermanentMigrationAcceptance("local-v3");

        [UnityTest]
        public IEnumerator FD2AcceptanceIncomingRabbitVersionThreeMigrationCompletesThePermanentLoop() => PermanentMigrationAcceptance("incoming-v3");

        static IEnumerator PermanentMigrationAcceptance(string shape)
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            foreach (var profile in Object.FindObjectsByType<Helper_ProfileSession>(FindObjectsSortMode.None))
            {
                profile.SetPlaytimeTracking(false);
                profile.enabled = false;
            }
            var balances = shape == "v2" ? @",""currency"":73"
                : shape == "local-v3" ? @",""researchData"":42,""speciesDataWallets"":[{""speciesId"":""hare"",""balance"":19},{""speciesId"":""fox"",""balance"":7}]"
                : @",""currency"":5,""researchData"":70,""rabbitData"":9";
            var source = @"{""version"":" + (shape == "v2" ? "2" : "3")
                + @",""profiles"":[{""id"":""migrated-acceptance"",""name"":""Migrated permanent acceptance""" + balances
                + @",""genomeProfiles"":[{""speciesId"":""hare"",""unlockedNodeIds"":[""hare.guarded-burrow""],""activeNodeIds"":[""hare.guarded-burrow""]},{""speciesId"":""fox"",""unlockedNodeIds"":[""fox.retained-unlock""],""activeNodeIds"":[]}],""simulationTicks"":42,""playTimeMilliseconds"":1234,""unlockedSpeciesIds"":[""hare"",""fox""],""unlockedGenomeSpeciesIds"":[""hare""],""unlockedFeatureIds"":[""gene-lab"",""retained-feature""],""completedRuns"":2,""completedRunIds"":[""legacy-one"",""legacy-two""]}],""lastLoadedProfileId"":""migrated-acceptance""}";
            if (shape == "local-v3") source = source.Replace(@",""completedRuns"":2,""completedRunIds"":[""legacy-one"",""legacy-two""]", string.Empty);
            PlayerPrefs.DeleteKey(Helper_ProfileSession.RecoveryStoreKey);
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey, source);
            PlayerPrefs.Save();
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            var session = menu.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var original = session.Current;
            Assert.That(original.ResearchData, Is.EqualTo(shape == "v2" ? 73 : shape == "local-v3" ? 42 : 75));
            Assert.That(original.RabbitData, Is.EqualTo(shape == "v2" ? 0 : shape == "local-v3" ? 19 : 9));
            Assert.That(original.SimulationTicks, Is.EqualTo(42));
            Assert.That(original.PlayTimeMilliseconds, Is.GreaterThanOrEqualTo(1234));
            Assert.That(original.CompletedRuns, Is.EqualTo(shape == "local-v3" ? 0 : 2));
            StringAssert.Contains("\"version\":7", PlayerPrefs.GetString(Helper_ProfileSession.StoreKey));
            var recovery = PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey);
            Assert.That(recovery, Is.EqualTo(source));
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            ConfigurePermanentAcceptanceJourney(root);
            yield return CompletePermanentAcceptanceJourney(root);
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            Assert.That(session.Current.RabbitData, Is.EqualTo(original.RabbitData + 39));
            Assert.That(session.Current.ResearchData, Is.EqualTo(original.ResearchData + 10));
            Assert.That(session.Current.CompletedRuns, Is.EqualTo(original.CompletedRuns + 1));
            AssertAcceptanceReceiptCannotPayAgain(session, preview);
            yield return CapturePermanentAcceptance(root, "t12-" + shape + "-migrated-success", true);
            ExecuteCommand(root.GetComponent("SaltyGame.VM_SimulationShell"), "ReturnToLabCommand");
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            OpenDesktopApp(desktop, "Gene Lab");
            yield return null;
            yield return null;
            var lab = GetProperty(((IList)GetProperty(desktop, "OpenDesktopWindows"))[0], "GenomeLab");
            var content = root.GetComponent<NoesisView>().Content;
            Assert.That(((IList)GetProperty(lab, "Nodes")).Count, Is.EqualTo(9));
            ExecuteGeneAction(content, lab, "PurchaseResearchCommand");
            yield return null;
            SelectGeneButton(content, "hare.keen-hearing");
            yield return null;
            ExecuteGeneAction(content, lab, "PurchaseNodeCommand");
            yield return null;
            ExecuteGeneAction(content, lab, "ResetGenomeCommand");
            yield return null;
            ExecuteGeneAction(content, lab, "ToggleLoadoutNodeCommand");
            yield return null;
            ExecuteGeneAction(content, lab, "ApplyLoadoutCommand");
            yield return null;
            yield return CapturePermanentAcceptance(root, "t12-" + shape + "-migrated-loadout");
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            menu = GameObject.Find("Main Camera");
            session = menu.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.Current.RabbitData, Is.EqualTo(original.RabbitData + 34));
            Assert.That(session.Current.ResearchData, Is.EqualTo(original.ResearchData - 20));
            Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(1));
            if (shape != "local-v3") Assert.That(session.Current.CompletedRunIds, Does.Contain("legacy-one").And.Contain("legacy-two"));
            Assert.That(session.Current.SettlementReceipts.Count, Is.EqualTo(1));
            CollectionAssert.AreEqual(original.UnlockedSpeciesIds, session.Current.UnlockedSpeciesIds);
            CollectionAssert.AreEqual(original.UnlockedFeatureIds, session.Current.UnlockedFeatureIds);
            Assert.That(session.Current.TryGetGenomeProfile(new SpeciesId("fox"), out var fox), Is.True);
            CollectionAssert.AreEqual(new[] { "fox.retained-unlock" }, fox.UnlockedNodeIds);
            session.Current.SpeciesDataWallets.TryGetValue(new SpeciesId("fox"), out var foxData);
            Assert.That(foxData, Is.EqualTo(shape == "local-v3" ? 7 : 0));
            Assert.That(PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey), Is.EqualTo(recovery));
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            root = GameObject.Find("GalapagOS Desktop Test Camera");
            ConfigurePermanentAcceptanceJourney(root);
            Assert.That(root.GetComponent<SpeciesSimulationPreview>().Run.ActiveGenomeSnapshot
                .GetSpeciesGenome(new SpeciesId("hare")).ActiveNodeIds, Is.EqualTo(new[] { "hare.keen-hearing" }));
        }

        sealed class ResultsStorage : IProfileSessionStorage
        {
            public readonly Dictionary<string, string> Values = new Dictionary<string, string>();
            public int Saves;
            public int FailOnSave = -1;
            public bool HasKey(string key) => Values.ContainsKey(key);
            public string GetString(string key) => Values.TryGetValue(key, out var value) ? value : string.Empty;
            public void SetString(string key, string value) => Values[key] = value;
            public void DeleteKey(string key) => Values.Remove(key);
            public void Save()
            {
                Saves++;
                if (Saves == FailOnSave) throw new System.IO.IOException("Results persistence test failure.");
            }
        }

        static Helper_ProfileSession NewResultsProfile(ResultsStorage storage, out GameObject owner)
        {
            owner = new GameObject("Isolated permanent results profile");
            owner.SetActive(false);
            var session = owner.AddComponent<Helper_ProfileSession>();
            session.InitializeStorage(storage);
            session.SetPlaytimeTracking(false);
            if (!session.Current.HasLoadedProfile) SeedFD2LaunchProfile(session);
            return session;
        }

        static void RunPermanentResultsJourney(SpeciesSimulationPreview preview)
        {
            preview.PauseSimulation();
            for (var round = 1; round <= 18; round++)
            {
                for (var tick = 0; tick < FieldDataJourneyDefinition.RoundLengthTicks; tick++)
                    Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True, $"Round {round}, tick {preview.Run.Tick}, state {preview.State}");
                if (round == 18) break;
                var session = preview.FieldDataSession;
                var choices = session.GetCurrentRoundRewardChoices() ?? session.GetCurrentNodeRewardChoices();
                if (choices != null && !choices.Resolved)
                    Assert.That(session.ResolveRewardChoice(choices.SourceId,
                        choices.Choices.Count > 0 ? choices.Choices[0].Id : "skip", out var error), Is.True, error);
                Assert.That(session.Continue(), Is.True);
                preview.PauseSimulation();
            }
            Assert.That(preview.FieldDataSession.State, Is.EqualTo(FieldDataJourneyFlowState.Succeeded));
        }

        [UnityTest]
        public IEnumerator FD2PermanentResultsShowSavedReceiptAndReturnToDesktopWithoutAnotherPayment()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            var profile = menu.GetComponent<Helper_ProfileSession>();
            SeedFD2LaunchProfile(profile);
            Assert.That(profile.TryPurchaseConversionResearch(profile.Current.ProfileId, 0, out var error), Is.True, error);
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            profile = root.GetComponent<Helper_ProfileSession>();
            profile.SetPlaytimeTracking(false);
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            var shell = root.GetComponent("SaltyGame.VM_SimulationShell");
            OpenDesktopApp(root.GetComponent("SaltyGame.VM_GalapagOS_Desktop"), "Simulation");
            SpeciesPresentationPlayModeTests.ConfigureAuthoredFieldDataScenario(preview);
            Assert.That(preview.TryConfigureFieldDataTestJourney(out error), Is.True, error);
            ExecuteCommand(shell, "CloseJourneyMapCommand");
            var before = profile.Current;
            preview.StartSimulation();
            RunPermanentResultsJourney(preview);
            yield return null;
            var results = preview.PermanentResults;
            var receipt = profile.Current.SettlementReceipts.Single();
            Assert.That(results.Receipt, Is.SameAs(receipt));
            Assert.That(receipt.LeftoverFieldData, Is.EqualTo(preview.FieldDataSession.Ledger.Balance));
            Assert.That(receipt.SpeciesDataBefore, Is.EqualTo(before.RabbitData));
            Assert.That(receipt.ResearchDataBefore, Is.EqualTo(before.ResearchData));
            Assert.That(GetProperty(shell, "ResultsTitleText").ToString(), Does.Contain("settlement saved"));
            Assert.That(GetProperty(shell, "ResultsMessageText").ToString(), Does.Not.Contain("discarded"));
            Assert.That(GetProperty(shell, "PermanentSettlementDetailsText"), Is.EqualTo(results.Details));
            Assert.That(GetProperty(shell, "CanReturnToLab"), Is.EqualTo(true));
            var output = GalapagOSVisualAcceptanceTests.TryGetVisualOutputDirectory();
            if (output != null && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "t10-saved-results", GameObject.Find("GalapagOS Simulation View").GetComponent<Camera>());
            ExecuteCommand(shell, "RetryPermanentSettlementCommand");
            Assert.That(profile.Current.SettlementReceipts.Count, Is.EqualTo(1));
            var wallets = profile.Current;
            ExecuteCommand(shell, "ReturnToLabCommand");
            Assert.That(root.GetComponent<Camera>().enabled, Is.True);
            Assert.That(profile.Current.RabbitData, Is.EqualTo(wallets.RabbitData));
            Assert.That(profile.Current.ResearchData, Is.EqualTo(wallets.ResearchData));
            profile.Reload();
            Assert.That(profile.Current.SettlementReceipts.Count, Is.EqualTo(1));
            Assert.That(profile.Current.RabbitData, Is.EqualTo(receipt.SpeciesDataAfter));
        }

        [UnityTest]
        public IEnumerator FD2PermanentResultsPendingSaveBlocksEveryExitAndRetryPaysOnlyOnce()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            SeedFD2LaunchProfile(menu.GetComponent<Helper_ProfileSession>());
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            var shell = root.GetComponent("SaltyGame.VM_SimulationShell");
            OpenDesktopApp(root.GetComponent("SaltyGame.VM_GalapagOS_Desktop"), "Simulation");
            var storage = new ResultsStorage();
            var profile = NewResultsProfile(storage, out var owner);
            try
            {
                preview.BindGenomeLaunchSource(() => profile.Current.CreateLaunchGenomeSnapshot(GenomeCatalogSnapshot.Empty));
                preview.BindPermanentResults(new FieldDataPermanentResults(profile));
                SpeciesPresentationPlayModeTests.ConfigureAuthoredFieldDataScenario(preview);
                Assert.That(preview.TryConfigureFieldDataTestJourney(out var error), Is.True, error);
                ExecuteCommand(shell, "CloseJourneyMapCommand");
                var before = profile.Current;
                storage.FailOnSave = storage.Saves + 2;
                preview.StartSimulation();
                RunPermanentResultsJourney(preview);
                yield return null;
                Assert.That(preview.PermanentResults.IsPending, Is.True);
                Assert.That(GetProperty(shell, "CanReturnToLab"), Is.EqualTo(false));
                Assert.That(GetProperty(shell, "CanCloseWindow"), Is.EqualTo(false));
                Assert.That(GetProperty(shell, "CanPlayNextSimulation"), Is.EqualTo(false));
                Assert.That(GetProperty(shell, "PermanentSettlementDetailsText").ToString(), Does.Not.Contain("Research Data: +"));
                ExecuteCommand(shell, "ReturnToLabCommand");
                ExecuteCommand(shell, "CloseWindowCommand");
                ExecuteCommand(shell, "PlayNextSimulationCommand");
                preview.PlayNextSimulation();
                Assert.That(root.GetComponent<Camera>().enabled, Is.False);
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
                Assert.That(profile.Current.RabbitData, Is.EqualTo(before.RabbitData));
                Assert.That(profile.Current.ResearchData, Is.EqualTo(before.ResearchData));
                var output = GalapagOSVisualAcceptanceTests.TryGetVisualOutputDirectory();
                if (output != null && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                    yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "t10-pending-results", GameObject.Find("GalapagOS Simulation View").GetComponent<Camera>());
                storage.FailOnSave = -1;
                ExecuteCommand(shell, "RetryPermanentSettlementCommand");
                Assert.That(preview.PermanentResults.IsPending, Is.False);
                Assert.That(GetProperty(shell, "CanReturnToLab"), Is.EqualTo(true));
                Assert.That(profile.Current.SettlementReceipts.Count, Is.EqualTo(1));
                var saves = storage.Saves;
                ExecuteCommand(shell, "RetryPermanentSettlementCommand");
                Assert.That(storage.Saves, Is.EqualTo(saves));
                ExecuteCommand(shell, "ReturnToLabCommand");
                Assert.That(root.GetComponent<Camera>().enabled, Is.True);
            }
            finally { UnityEngine.Object.Destroy(owner); }
        }

        [UnityTest]
        public IEnumerator FD2PermanentResultsFailureRetainsWalletsAndPendingJournalCanReopenForRetry()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            SeedFD2LaunchProfile(menu.GetComponent<Helper_ProfileSession>());
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            var shell = root.GetComponent("SaltyGame.VM_SimulationShell");
            var profile = root.GetComponent<Helper_ProfileSession>();
            profile.SetPlaytimeTracking(false);
            OpenDesktopApp(root.GetComponent("SaltyGame.VM_GalapagOS_Desktop"), "Simulation");
            SpeciesPresentationPlayModeTests.ConfigureAuthoredFieldDataScenario(preview);
            Assert.That(preview.TryConfigureFieldDataTestJourney(out var error), Is.True, error);
            ExecuteCommand(shell, "CloseJourneyMapCommand");
            var before = profile.Current;
            preview.StartSimulation();
            preview.PauseSimulation();
            for (var tick = 0; tick < 100; tick++) Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            var lost = preview.FieldDataSession.Ledger.Balance;
            Assert.That(preview.AbandonFieldDataJourney(), Is.True);
            yield return null;
            Assert.That(preview.PermanentResults.Summary, Does.Contain($"{lost} Field Data forfeited"));
            Assert.That(GetProperty(shell, "ResultsMessageText").ToString(), Does.Contain("No permanent data awarded"));
            Assert.That(profile.Current.RabbitData, Is.EqualTo(before.RabbitData));
            Assert.That(profile.Current.ResearchData, Is.EqualTo(before.ResearchData));
            Assert.That(profile.Current.CreateGenomeSnapshot().Fingerprint, Is.EqualTo(before.CreateGenomeSnapshot().Fingerprint));
            var output = GalapagOSVisualAcceptanceTests.TryGetVisualOutputDirectory();
            if (output != null && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "t10-failure-results", GameObject.Find("GalapagOS Simulation View").GetComponent<Camera>());
            ExecuteCommand(shell, "ReturnToLabCommand");
            Assert.That(root.GetComponent<Camera>().enabled, Is.True);
            var storage = new ResultsStorage();
            var isolated = NewResultsProfile(storage, out var owner);
            try
            {
                storage.FailOnSave = storage.Saves + 2;
                Assert.That(isolated.TrySettleFieldDataJourney(isolated.Current.ProfileId, "recovered-ui-success",
                    FieldDataJourneyFlowState.Succeeded, new SpeciesId("hare"), 100, out _, out _), Is.False);
                storage.FailOnSave = -1;
                isolated.Reload();
                preview.BindPermanentResults(new FieldDataPermanentResults(isolated));
                OpenDesktopApp(root.GetComponent("SaltyGame.VM_GalapagOS_Desktop"), "Simulation");
                yield return null;
                Assert.That(GetProperty(shell, "RetryPermanentSettlementVisibility").ToString(), Is.EqualTo("Visible"));
                Assert.That(GetProperty(shell, "CanReturnToLab"), Is.EqualTo(false));
                ExecuteCommand(shell, "RetryPermanentSettlementCommand");
                Assert.That(GetProperty(shell, "ResultsTitleText").ToString(), Does.Contain("saved"));
                Assert.That(isolated.Current.SettlementReceipts.Count, Is.EqualTo(1));
            }
            finally { UnityEngine.Object.Destroy(owner); }
        }

        static void SeedFD2LaunchProfile(Helper_ProfileSession session)
        {
            session.SetPlaytimeTracking(false);
            var id = session.CreateProfile("FD2 launch automation").ProfileId;
            session.AddRabbitData(60);
            session.AddResearchData(200);
            Assert.That(session.TryPurchaseHareGenomeLicense(id, out var error), Is.True, error);
            var nodes = FieldDataPermanentProgressionDefinition.CreateStarter().HareMap.Nodes;
            foreach (var node in nodes.Take(4))
                Assert.That(session.TryPurchaseHareGenomeNode(id, node.Id, out error), Is.True, error);
            var hare = new SpeciesId("hare");
            Assert.That(session.TryResetGenomeLoadout(id, hare, 0, out error), Is.True, error);
            Assert.That(session.TryPurchaseHareGenomeNode(id, nodes[4].Id, out error), Is.True, error);
            Assert.That(session.TryResetGenomeLoadout(id, hare, 1, out error), Is.True, error);
            Assert.That(session.TryArrangeGenomeLoadout(id, hare, 2, nodes.Take(4).Select(node => node.Id), out error), Is.True, error);
        }

        [UnityTest]
        public IEnumerator FD2GenomeLaunchRefreshesAtFirstTickPreservesOpeningMutationAndFreezesThroughTheJourney()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            SeedFD2LaunchProfile(menu.GetComponent<Helper_ProfileSession>());
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            var session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var id = session.Current.ProfileId;
            var hare = new SpeciesId("hare");
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            ConfigureShortRewardJourney(preview);
            Assert.That(preview.TrySetActiveGenomeBeforeJourney(GenomeSimulationSnapshot.Empty, out var error), Is.True, error);
            var natural = preview.ActiveSpeciesRules[hare];
            OpenDesktopApp(desktop, "Simulation");
            Assert.That(preview.ChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            var opening = preview.Progression.AppliedRunUpgrades.Single();
            Assert.That(session.TryResetGenomeLoadout(id, hare, 2, out error), Is.True, error);
            var selected = new[] { "hare.warren-network", "hare.keen-hearing", "hare.swift-digging", "hare.safe-foraging" };
            Assert.That(session.TryArrangeGenomeLoadout(id, hare, 3, selected, out error), Is.True, error);
            Assert.That(preview.ChooseJourneyNode("first-cycle"), Is.True, preview.SettingsMessage);
            preview.PauseSimulation();
            Assert.That(preview.Run.ActiveGenomeSnapshot.GetSpeciesGenome(hare).ActiveNodeIds, Is.EqualTo(selected));
            Assert.That(preview.Run.ActiveGenomeSnapshot.GetPopulationPoolCapacity(hare), Is.EqualTo(60));
            Assert.That(preview.Progression.AppliedRunUpgrades.Single().Fingerprint, Is.EqualTo(opening.Fingerprint));
            var expectedBase = preview.Run.ActiveGenomeSnapshot.GetSpeciesGenome(hare).ApplyToNaturalRules(natural);
            var expected = new SpeciesProgression(new SpeciesDefinition(hare, expectedBase));
            Assert.That(expected.TryApplyRunUpgrade(opening), Is.True);
            Assert.That(preview.ActiveSpeciesRules[hare].BlockAmount, Is.EqualTo(expected.CurrentRules.BlockAmount));
            Assert.That(preview.ActiveSpeciesRules[hare].MovementSpeed, Is.EqualTo(expected.CurrentRules.MovementSpeed));
            Assert.That(preview.ActiveSpeciesRules[hare].DigestionEnergyBonus, Is.EqualTo(expected.CurrentRules.DigestionEnergyBonus));
            var frozen = preview.Run.ActiveGenomeSnapshot.Fingerprint;
            Assert.That(session.TryResetGenomeLoadout(id, hare, 3, out error), Is.True, error);
            Assert.That(session.TryArrangeGenomeLoadout(id, hare, 4, new[] { "hare.guarded-burrow" }, out error), Is.True, error);
            Assert.That(preview.TrySetActiveGenomeBeforeJourney(GenomeSimulationSnapshot.Empty, out _), Is.False);
            Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.AwaitingDecision));
            Assert.That(preview.Run.CreateCheckpoint().Restore().ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(frozen));
            preview.ContinueWithoutUpgrade();
            Assert.That(preview.Run.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(frozen));
            preview.PauseSimulation();
            for (var phase = 2; phase <= 6; phase++)
            {
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                Assert.That(preview.Run.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(frozen));
                if (phase < 6)
                {
                    preview.ContinueWithoutUpgrade();
                    preview.PauseSimulation();
                }
            }
            Assert.That(preview.ChooseJourneyNode("seedfall"), Is.True);
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            preview.EndSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            preview.PlayNextSimulation();
            preview.PauseSimulation();
            Assert.That(preview.Run.ActiveGenomeSnapshot.Fingerprint, Is.Not.EqualTo(frozen));
            Assert.That(preview.Run.ActiveGenomeSnapshot.GetSpeciesGenome(hare).ActiveNodeIds, Is.EqualTo(new[] { "hare.guarded-burrow" }));
            Assert.That(preview.Run.ActiveGenomeSnapshot.GetPopulationPoolCapacity(hare), Is.EqualTo(50));
            Assert.That(preview.ActiveSpeciesRules[hare].BlockAmount, Is.EqualTo(natural.BlockAmount + 1));
            Assert.That(preview.ActiveSpeciesRules[hare].MovementSpeed, Is.EqualTo(natural.MovementSpeed));
        }

        [UnityTest]
        public IEnumerator FD2GenomeLaunchAppliesToBackgroundHaresWhenFoxIsThePlayer()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            SeedFD2LaunchProfile(menu.GetComponent<Helper_ProfileSession>());
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            root.GetComponent<Helper_ProfileSession>().SetPlaytimeTracking(false);
            ConfigureShortRewardJourney(preview);
            Assert.That(preview.TrySetActiveGenomeBeforeJourney(GenomeSimulationSnapshot.Empty, out var error), Is.True, error);
            Assert.That(preview.TrySetPlayerSpecies("fox", out error), Is.True, error);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations("8", "8", "10100", "64", "0", "12", "0.1",
                "0", "0", "0", false, "20", "4", "2", out error), Is.True, error);
            var hare = new SpeciesId("hare");
            var natural = preview.ActiveSpeciesRules[hare];
            OpenDesktopApp(root.GetComponent("SaltyGame.VM_GalapagOS_Desktop"), "Simulation");
            preview.PauseSimulation();
            Assert.That(preview.Run.PlayerSpeciesId, Is.EqualTo(new SpeciesId("fox")));
            Assert.That(preview.Run.ActiveGenomeSnapshot.GetSpeciesGenome(hare).ActiveNodeIds.Count, Is.EqualTo(4));
            Assert.That(preview.ActiveSpeciesRules[hare].BlockAmount, Is.EqualTo(natural.BlockAmount + 1));
            Assert.That(preview.ActiveSpeciesRules[hare].Awareness.VisionRange, Is.EqualTo(natural.Awareness.VisionRange + 2));
            Assert.That(preview.ActiveSpeciesRules[hare].MovementSpeed, Is.EqualTo(natural.MovementSpeed + 0.25f));
            var backgroundHares = 0;
            for (var y = 0; y < preview.Run.Cells.Height; y++)
                for (var x = 0; x < preview.Run.Cells.Width; x++)
                    if (preview.Run.Cells.GetCell(x, y).SpeciesId == hare) backgroundHares++;
            Assert.That(backgroundHares, Is.GreaterThan(0));
        }

        [UnityTest]
        public IEnumerator FD2GenomeLaunchSurvivesTestRouteConfigurationAndNextJourneyReset()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            SeedFD2LaunchProfile(menu.GetComponent<Helper_ProfileSession>());
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            var session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            OpenDesktopApp(root.GetComponent("SaltyGame.VM_GalapagOS_Desktop"), "Simulation");
            var prepared = preview.ActiveGenomeSnapshot.Fingerprint;
            Assert.That(preview.TryConfigureFieldDataTestJourney(out var error), Is.True, error);
            Assert.That(preview.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(prepared));
            preview.StartSimulation();
            preview.PauseSimulation();
            Assert.That(preview.FieldDataSession, Is.Not.Null);
            Assert.That(preview.FieldDataSession.Run.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(prepared));
            var hare = new SpeciesId("hare");
            Assert.That(session.TryResetGenomeLoadout(session.Current.ProfileId, hare, 2, out error), Is.True, error);
            Assert.That(preview.Run.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(prepared));
            Assert.That(preview.AbandonFieldDataJourney(), Is.True);
            preview.PlayNextSimulation();
            preview.PauseSimulation();
            Assert.That(preview.FieldDataSession.Run.ActiveGenomeSnapshot.GetSpeciesGenome(hare).ActiveNodeIds, Is.Empty);
            Assert.That(preview.Run.ActiveGenomeSnapshot.Fingerprint, Is.Not.EqualTo(prepared));
        }

        [UnityTest]
        public IEnumerator FD2GenomeLaunchRejectsAChangedProfileBeforeTheFirstSimulationStarts()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            var menu = GameObject.Find("Main Camera");
            SeedFD2LaunchProfile(menu.GetComponent<Helper_ProfileSession>());
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            var session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            OpenDesktopApp(root.GetComponent("SaltyGame.VM_GalapagOS_Desktop"), "Simulation");
            var prepared = preview.Run.ActiveGenomeSnapshot.Fingerprint;
            session.CreateProfile("Different launch owner");
            preview.StartSimulation();
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Ready));
            Assert.That(preview.SettingsMessage, Does.Contain("original profile"));
            Assert.That(preview.Run.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(prepared));
        }

        [UnityTest]
        public IEnumerator GenomeStatsFreezeForAllPopulationsAndRefreshWithoutStackingOnNextJourney()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            yield return null;
            var menu = GameObject.Find("Main Camera");
            var session = menu.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            session.CreateProfile("Genome stat automation");
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var provider = root.GetComponent<GenomeCatalogProvider>();
            Assert.That(provider, Is.Not.Null);
            var mapsField = typeof(GenomeCatalogProvider).GetField("speciesMaps", BindingFlags.Instance | BindingFlags.NonPublic);
            var originalMaps = mapsField.GetValue(provider);
            var hareNode = ScriptableObject.CreateInstance<GenomeUpgradeAsset>();
            var foxNode = ScriptableObject.CreateInstance<GenomeUpgradeAsset>();
            var hareMap = ScriptableObject.CreateInstance<SpeciesGenomeMapAsset>();
            var foxMap = ScriptableObject.CreateInstance<SpeciesGenomeMapAsset>();
            try
            {
                // In-memory fixtures exercise delivery; these are not production offers or proposed balance values.
                JsonUtility.FromJsonOverwrite("{\"upgradeId\":\"hare.fixture\",\"displayName\":\"Hare fixture\",\"targetSpeciesId\":\"hare\",\"modifiers\":[{\"attributeId\":\"energy.starting\",\"signedValue\":3}]}", hareNode);
                JsonUtility.FromJsonOverwrite("{\"upgradeId\":\"fox.fixture\",\"displayName\":\"Fox fixture\",\"targetSpeciesId\":\"fox\",\"modifiers\":[{\"attributeId\":\"energy.starting\",\"signedValue\":4}]}", foxNode);
                var speciesField = typeof(SpeciesGenomeMapAsset).GetField("speciesId", BindingFlags.Instance | BindingFlags.NonPublic);
                var nodesField = typeof(SpeciesGenomeMapAsset).GetField("upgrades", BindingFlags.Instance | BindingFlags.NonPublic);
                speciesField.SetValue(hareMap, "hare");
                nodesField.SetValue(hareMap, new[] { hareNode });
                speciesField.SetValue(foxMap, "fox");
                nodesField.SetValue(foxMap, new[] { foxNode });
                mapsField.SetValue(provider, new[] { hareMap, foxMap });
                Assert.That(provider.TryCapture(out var reason), Is.True, reason);
                Assert.That(session.TrySetGenomeConfiguration(new SpeciesId("hare"), new[] { "hare.fixture" }, new[] { "hare.fixture" }, out reason), Is.True, reason);
                Assert.That(session.TrySetGenomeConfiguration(new SpeciesId("fox"), new[] { "fox.fixture" }, new[] { "fox.fixture" }, out reason), Is.True, reason);
                ConfigureShortRewardJourney(preview);
                Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                    "8", "8", "10100", "64", "0", "12", "0.1", "0", "0", "0", false, "20", "4", "2", out reason), Is.True, reason);
                var naturalHare = preview.ActiveSpeciesRules[new SpeciesId("hare")].StartingEnergy;
                var naturalFox = preview.ActiveSpeciesRules[new SpeciesId("fox")].StartingEnergy;
                OpenDesktopApp(desktop, "Simulation");
                AssertInitialSpeciesEnergy(preview, "hare", naturalHare + 3);
                AssertInitialSpeciesEnergy(preview, "fox", naturalFox + 4);
                // Reopening a ready journey must start from natural rules again.
                OpenDesktopApp(desktop, "Simulation");
                AssertInitialSpeciesEnergy(preview, "hare", naturalHare + 3);
                StartRewardJourney(desktop, preview);
                var frozen = preview.ActiveGenomeSnapshot.Fingerprint;
                JsonUtility.FromJsonOverwrite("{\"modifiers\":[{\"attributeId\":\"energy.starting\",\"signedValue\":7}]}", hareNode);
                Assert.That(provider.TryCapture(out reason), Is.True, reason);
                Assert.That(preview.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(frozen));
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.AwaitingDecision));
                preview.ContinueWithoutUpgrade();
                Assert.That(preview.ActiveSpeciesRules[new SpeciesId("fox")].StartingEnergy, Is.EqualTo(naturalFox + 4));
                Assert.That(preview.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(frozen));
                preview.EndSimulation();
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
                OpenDesktopApp(desktop, "Simulation");
                Assert.That(preview.SettingsEditable, Is.True);
                Assert.That(preview.ActiveGenomeSnapshot.Fingerprint, Is.Not.EqualTo(frozen));
                AssertInitialSpeciesEnergy(preview, "hare", naturalHare + 7);
                AssertInitialSpeciesEnergy(preview, "fox", naturalFox + 4);
                Assert.That(session.TrySetGenomeConfiguration(new SpeciesId("hare"), new[] { "hare.fixture" }, new string[0], out reason), Is.True, reason);
                OpenDesktopApp(desktop, "Simulation");
                AssertInitialSpeciesEnergy(preview, "hare", naturalHare);
                AssertInitialSpeciesEnergy(preview, "fox", naturalFox + 4);
            }
            finally
            {
                mapsField.SetValue(provider, originalMaps);
                provider.TryCapture(out _);
                Object.Destroy(hareMap);
                Object.Destroy(foxMap);
                Object.Destroy(hareNode);
                Object.Destroy(foxNode);
            }
        }

        static void AssertInitialSpeciesEnergy(SpeciesSimulationPreview preview, string species, int energy)
        {
            Assert.That(preview.ActiveSpeciesRules[new SpeciesId(species)].StartingEnergy, Is.EqualTo(energy));
            var found = 0;
            var cells = preview.Run.Cells;
            for (var y = 0; y < cells.Height; y++)
            {
                for (var x = 0; x < cells.Width; x++)
                {
                    var cell = cells.GetCell(x, y);
                    if (cell.SpeciesId != new SpeciesId(species))
                    {
                        continue;
                    }
                    Assert.That(cell.Energy, Is.EqualTo(energy));
                    found++;
                }
            }
            Assert.That(found, Is.GreaterThan(0));
        }

        [UnityTest]
        public IEnumerator ProfileJourneyRewardsPayOncePersistAndUnlockGeneLab()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            yield return null;

            var menu = GameObject.Find("Main Camera");
            var session = menu.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var profileId = session.CreateProfile("Journey reward automation").ProfileId;
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("GalapagOSDesktopTest"));
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.Current.ProfileId, Is.EqualTo(profileId));
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            // Preserve the explicitly legacy POC economy regression; FD2 UI has its own scene checks.
            desktop.GetType().GetMethod("BindProfileSession").Invoke(desktop, new object[] { session, false });
            var shell = root.GetComponent("SaltyGame.VM_SimulationShell");
            ConfigureShortRewardJourney(preview);
            OpenDesktopApp(desktop, "Gene Lab");
            var windows = (IList)GetProperty(desktop, "OpenDesktopWindows");
            var lab = GetProperty(windows[0], "GenomeLab");
            AssertJourneyBalances(session, 0, 0, 0);
            Assert.That(GetProperty(lab, "CanPurchaseLicense"), Is.False);

            StartRewardJourney(desktop, preview);
            Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            preview.EndSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.LastRunEndedEarly, Is.True);
            Assert.That(preview.LastRunRewardsEligible, Is.False);
            Assert.That(preview.Progression.Currency, Is.Zero);
            Assert.That(preview.UsePopulationControl(0), Is.False, "Early end is not a population-control window.");
            AssertJourneyBalances(session, 0, 0, 0);
            yield return null;
            Execute(GetProperty(shell, "CloseWindowCommand"));

            StartRewardJourney(desktop, preview);
            RemovePlayerPopulation(preview);
            Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.LastExpeditionFailed, Is.True);
            Assert.That(preview.LastRunRewardsEligible, Is.False);
            Assert.That(preview.Progression.Currency, Is.Zero);
            Assert.That(preview.UsePopulationControl(0), Is.False, "Journey failure is not a population-control window.");
            AssertJourneyBalances(session, 0, 0, 0);
            yield return null;
            Execute(GetProperty(shell, "CloseWindowCommand"));

            var completionIds = new HashSet<string>();
            for (var completed = 1; completed <= 3; completed++)
            {
                StartRewardJourney(desktop, preview);
                Assert.That(preview.Run.TargetTicks, Is.EqualTo(12));
                Assert.That(completionIds.Add(preview.RunCompletionId), Is.True,
                    "Each new journey must have a distinct settlement identity.");
                for (var phase = 1; phase <= 12; phase++)
                {
                    Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                    if (phase == 12) break;

                    var latestPhase = preview.Run.PhaseResults[preview.Run.PhaseResults.Count - 1];
                    if (phase < 6)
                    {
                        Assert.That(latestPhase.PhaseIndex, Is.EqualTo(phase));
                        Assert.That(preview.Progression.Currency, Is.Zero,
                            "Field Data stays unavailable until the first six-phase simulation finishes.");
                    }

                    AssertJourneyBalances(session, completed - 1, completed - 1, completed - 1);
                    if (phase == 6)
                    {
                        Assert.That(preview.Progression.Currency, Is.GreaterThan(0),
                            "The first simulation releases its banked Field Data at the cycle boundary.");
                        Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyDecision));
                        Assert.That(preview.ChooseJourneyNode("seedfall"), Is.True);
                        Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
                        Assert.That(preview.ChooseJourneyNode("burrow-study"), Is.True);
                    }
                    else
                    {
                        Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                        preview.ContinueWithoutUpgrade();
                    }
                    preview.PauseSimulation();
                }

                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
                Assert.That(preview.UsePopulationControl(0), Is.False, "Terminal success is not a population-control window.");
                Assert.That(preview.LastRunRewardsEligible, Is.True);
                Assert.That(preview.Run.Tick, Is.EqualTo(preview.Run.TargetTicks));
                AssertJourneyBalances(session, completed, completed, completed);
                Assert.That(session.Current.CompletedRunIds, Does.Contain(preview.RunCompletionId));
                session.Reload();
                AssertJourneyBalances(session, completed, completed, completed);
                Assert.That(session.RecordSuccessfulJourney(preview.RunCompletionId), Is.False,
                    "Replaying an already-paid completion after reload must not pay again.");
                AssertJourneyBalances(session, completed, completed, completed);
                Assert.That(GetProperty(lab, "CompletedRunsText"), Is.EqualTo(completed.ToString()));
                Assert.That(GetProperty(lab, "CanPurchaseLicense"), Is.EqualTo(completed == 3));
                yield return null;
                Execute(GetProperty(shell, "CloseWindowCommand"));
            }

            Execute(GetProperty(lab, "PurchaseLicenseCommand"));
            Execute(GetProperty(lab, "PurchaseNodeCommand"));
            AssertJourneyBalances(session, 3, 1, 3);
            var hare = new SpeciesId("hare");
            Assert.That(session.Current.CreateGenomeSnapshot().GetSpeciesGenome(hare)
                .HasActiveNode("fertile-droppings"), Is.True);

            // Recreate the scene components to verify the actual saved profile, not only its in-memory snapshot.
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            yield return null;
            menu = GameObject.Find("Main Camera");
            session = menu.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.Current.ProfileId, Is.EqualTo(profileId));
            AssertJourneyBalances(session, 3, 1, 3);
            ExecuteCommand(menu.GetComponent("SaltyGame.VM_MainMenu"), "ContinueCommand");
            yield return WaitForProfileDesktop();

            root = GameObject.Find("GalapagOS Desktop Test Camera");
            session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            preview = root.GetComponent<SpeciesSimulationPreview>();
            desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            ConfigureShortRewardJourney(preview);
            StartRewardJourney(desktop, preview);
            Assert.That(preview.Run.ActiveGenomeSnapshot.GetSpeciesGenome(hare)
                .HasActiveNode("fertile-droppings"), Is.True);
            RemovePlayerPopulation(preview);
            Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            Assert.That(preview.LastExpeditionFailed, Is.True);
            session.Reload();
            AssertJourneyBalances(session, 3, 1, 3);
            Assert.That(session.Current.UnlockedGenomeSpeciesIds, Does.Contain(hare));
            Assert.That(session.Current.CreateGenomeSnapshot().GetSpeciesGenome(hare)
                .HasActiveNode("fertile-droppings"), Is.True);
        }

        [UnityTest]
        public IEnumerator PopulationControlsRefreshAndSeedCacheUseTheLiveJourney()
        {
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;
            var root = GameObject.Find("GalapagOS Desktop Test Camera");
            var session = root.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            session.CreateProfile("Population decisions automation");
            var preview = root.GetComponent<SpeciesSimulationPreview>();
            var desktop = root.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var shell = root.GetComponent("SaltyGame.VM_SimulationShell");
            ConfigureShortRewardJourney(preview);
            StartRewardJourney(desktop, preview);
            Assert.That(preview.UsePopulationControl(0), Is.False, "An ordinary pause is not a decision boundary.");
            Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            preview.Progression.AddCurrency(100);
            ExecuteCommand(shell, "CloseJourneyMapCommand");
            yield return null;
            var simulation = GameObject.Find("GalapagOS Simulation View");
            var content = simulation.GetComponent<NoesisView>().Content;
            var camera = simulation.GetComponent<Camera>();
            var output = GalapagOSVisualAcceptanceTests.TryGetVisualOutputDirectory();
            var initialData = preview.Progression.Currency;
            var tick = preview.Run.Tick;
            var initial = SpeciesPopulationSnapshot.Create(preview.Run.Cells, tick);
            foreach (var title in new[] { "+1 HARE", "-1 HARE", "+1 FOX", "-1 FOX", "+5 PLANTS", "-5 PLANTS" })
            {
                var button = FindGeneControl<Noesis.Button>(content, candidate =>
                    candidate.DataContext?.GetType().GetProperty("Title")?.GetValue(candidate.DataContext) is string text
                    && text.StartsWith(title, System.StringComparison.Ordinal));
                Assert.That(button, Is.Not.Null, "Missing bound control: " + title);
                Assert.That(button.IsEnabled, Is.True, "Disabled control: " + title);
                Assert.That(button.Command, Is.Not.Null);
                button.Command.Execute(button.CommandParameter);
                yield return null;
            }
            Assert.That(preview.Progression.Currency, Is.EqualTo(initialData - 60));
            Assert.That(preview.Run.Tick, Is.EqualTo(tick));
            var current = SpeciesPopulationSnapshot.Create(preview.Run.Cells, tick);
            foreach (var entry in initial.Counts) Assert.That(current.GetCount(entry.Key), Is.EqualTo(entry.Value));
            Assert.That(preview.Run.PopulationInterventions, Has.Count.EqualTo(6));
            // Expose the lower, scrollable controls at 720p as well as 1080p.
            var scroll = FindGeneControl<Noesis.ScrollViewer>(content, viewer => viewer.ScrollableHeight > 100);
            if (scroll != null) scroll.ScrollToVerticalOffset(scroll.ScrollableHeight);
            if (output != null) yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "decisions-01-controls", camera);
            preview.Progression.AddCurrency(100);
            while (preview.CanUsePopulationControl(5, out _))
            {
                var removePlants = FindGeneControl<Noesis.Button>(content, candidate =>
                    candidate.DataContext?.GetType().GetProperty("Title")?.GetValue(candidate.DataContext) is string text
                    && text.StartsWith("-5 PLANTS", System.StringComparison.Ordinal));
                Assert.That(removePlants, Is.Not.Null);
                Assert.That(removePlants.IsEnabled, Is.True);
                removePlants.Command.Execute(removePlants.CommandParameter);
                yield return null;
            }
            Assert.That(SpeciesPopulationSnapshot.Create(preview.Run.Cells, tick).Plants, Is.Zero);
            yield return null;
            Assert.That(GetProperty(shell, "FoodPressureText").ToString(), Does.Contain("FOOD EXHAUSTED"));
            if (output != null) yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "decisions-01b-food-exhausted", camera);
            // Restore this feature fixture so the route assertions stay independent of balance.
            var missingPlants = initial.Plants;
            while (missingPlants > 0)
            {
                Assert.That(preview.UsePopulationControl(4), Is.True);
                missingPlants -= 5;
            }
            preview.Progression.TrySpend(preview.Progression.Currency);
            Assert.That(preview.UsePopulationControl(0), Is.False);
            Assert.That(preview.Progression.Currency, Is.Zero);
            preview.ContinueWithoutUpgrade();
            preview.PauseSimulation();
            if (output != null) yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "decisions-02-food-ledger", camera);
            while (preview.Run.PhaseIndex <= 6 && preview.State != SpeciesPreviewState.JourneyDecision)
            {
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                if (preview.State == SpeciesPreviewState.PhaseDecision)
                {
                    preview.ContinueWithoutUpgrade();
                    preview.PauseSimulation();
                }
            }
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyDecision));
            Assert.That(preview.IsPopulationControlWindowOpen, Is.True,
                "The first cycle result remains an uncommitted decision boundary.");
            preview.Progression.AddCurrency(100);
            var settledPhase = preview.Run.PhaseResults[preview.Run.PhaseResults.Count - 1];
            var settledTick = preview.Run.Tick;
            var settledIncomeMessage = preview.PhaseRewardMessage;
            var settledHares = settledPhase.ClosingPopulation.GetCount(new SpeciesId("hare"));
            var settledPlants = settledPhase.ClosingPopulation.GetCount(SpeciesIds.Plant);
            var resultPlants = SpeciesPopulationSnapshot.Create(preview.Run.Cells, settledTick).Plants;
            var resultBalance = preview.Progression.Currency;
            Assert.That(settledPhase.PhaseIndex, Is.EqualTo(6));
            Assert.That(GetProperty(shell, "HarePurchaseVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(preview.Progression.TrySpend(preview.Progression.Currency), Is.True);
            Assert.That(preview.UsePopulationControl(4), Is.False, "A cycle result still requires Field Data.");
            preview.Progression.AddCurrency(100);
            var cyclePlantButton = FindGeneControl<Noesis.Button>(content, candidate =>
                candidate.DataContext?.GetType().GetProperty("Title")?.GetValue(candidate.DataContext) is string text
                && text.StartsWith("+5 PLANTS", System.StringComparison.Ordinal));
            Assert.That(cyclePlantButton, Is.Not.Null);
            Assert.That(cyclePlantButton.IsEnabled, Is.True);
            cyclePlantButton.Command.Execute(cyclePlantButton.CommandParameter);
            yield return null;
            Assert.That(preview.Run.Tick, Is.EqualTo(settledTick));
            Assert.That(SpeciesPopulationSnapshot.Create(preview.Run.Cells, settledTick).Plants, Is.EqualTo(resultPlants + 5));
            Assert.That(preview.Progression.Currency, Is.EqualTo(resultBalance - SpeciesSimulationPreview.HareCost));
            Assert.That(preview.PhaseRewardMessage, Is.EqualTo(settledIncomeMessage),
                "The income summary remains the amount settled before the control action.");
            Assert.That(preview.Run.PhaseResults[preview.Run.PhaseResults.Count - 1].ClosingPopulation.GetCount(new SpeciesId("hare")),
                Is.EqualTo(settledHares));
            Assert.That(preview.Run.PhaseResults[preview.Run.PhaseResults.Count - 1].ClosingPopulation.GetCount(SpeciesIds.Plant),
                Is.EqualTo(settledPlants));
            if (output != null) yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "decisions-05-cycle-controls", camera);

            Assert.That(preview.ChooseJourneyNode("rest-stop"), Is.True);
            Assert.That(preview.IsPopulationControlWindowOpen, Is.False,
                "Population controls close while the encounter choice is pending.");
            var pendingBalance = preview.Progression.Currency;
            Assert.That(preview.UsePopulationControl(4), Is.False);
            Assert.That(preview.Progression.Currency, Is.EqualTo(pendingBalance));
            Assert.That(preview.JourneyRewardOptionCount, Is.EqualTo(3));
            yield return null;
            if (output != null) yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "decisions-03-refresh", camera);
            var beforeCancel = preview.Run.Cells;
            preview.CancelJourneyNodeReward();
            Assert.That(preview.Run.Cells, Is.SameAs(beforeCancel));
            Assert.That(preview.ChooseJourneyNode("rest-stop"), Is.True);
            var beforeRefresh = SpeciesPopulationSnapshot.Create(preview.Run.Cells, preview.Run.Tick).GetCount(new SpeciesId("hare"));
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.IsPopulationControlWindowOpen, Is.True,
                "Paid controls reopen after the encounter is claimed and before travel.");
            var postRefreshPlants = SpeciesPopulationSnapshot.Create(preview.Run.Cells, preview.Run.Tick).Plants;
            Assert.That(preview.UsePopulationControl(4), Is.True);
            Assert.That(SpeciesPopulationSnapshot.Create(preview.Run.Cells, preview.Run.Tick).Plants,
                Is.EqualTo(postRefreshPlants + 5));
            Assert.That(SpeciesPopulationSnapshot.Create(preview.Run.Cells, preview.Run.Tick).GetCount(new SpeciesId("hare")),
                Is.EqualTo(System.Math.Min(preview.Run.PopulationPoolCapacity, beforeRefresh + 10)));
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.False, "A refresh can be claimed only once.");
            Assert.That(preview.ChooseJourneyNode("seed-cache"), Is.False, "The other branch is no longer available.");
            Assert.That(preview.ChooseJourneyNode("burrow-study"), Is.True);
            Assert.That(preview.Run.PhaseIndex, Is.EqualTo(7));
            Assert.That(preview.IsPopulationControlWindowOpen, Is.False,
                "Continuing the next simulation commits the cycle result boundary.");
            Assert.That(preview.UsePopulationControl(4), Is.False);
            preview.EndSimulation();
            OpenDesktopApp(desktop, "Simulation");
            ConfigureShortRewardJourney(preview);
            StartRewardJourney(desktop, preview);
            while (preview.State != SpeciesPreviewState.JourneyDecision)
            {
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                if (preview.State == SpeciesPreviewState.PhaseDecision)
                {
                    preview.ContinueWithoutUpgrade();
                    preview.PauseSimulation();
                }
            }
            var eventNode = preview.JourneyMap.GetNode("seed-cache");
            var expected = eventNode.CreatePopulationEncounter().EventAmount(preview.Run.Seed);
            Assert.That(preview.ChooseJourneyNode("seed-cache"), Is.True);
            var label = preview.GetJourneyRewardOptionDisplayName(0);
            Assert.That(label, Does.Contain("50%: +10").And.Contain("50%: -5"));
            preview.CancelJourneyNodeReward();
            Assert.That(preview.ChooseJourneyNode("seed-cache"), Is.True);
            Assert.That(preview.GetJourneyRewardOptionDisplayName(0), Is.EqualTo(label));
            yield return null;
            if (output != null) yield return GalapagOSVisualAcceptanceTests.CaptureCamera(output, "decisions-04-event", camera);
            var plants = SpeciesPopulationSnapshot.Create(preview.Run.Cells, preview.Run.Tick).Plants;
            var balance = preview.Progression.Currency;
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(SpeciesPopulationSnapshot.Create(preview.Run.Cells, preview.Run.Tick).Plants, Is.EqualTo(plants + expected));
            Assert.That(preview.Progression.Currency, Is.EqualTo(balance));
            Assert.That(preview.Run.PopulationInterventions[0].Source, Is.EqualTo("journey:seed-cache"));
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.False);
            Assert.That(preview.ChooseJourneyNode(eventNode.NextNodes[0].NodeId), Is.True);
            Assert.That(preview.Run.PhaseIndex, Is.EqualTo(7));
        }

        static IEnumerator WaitForProfileDesktop()
        {
            var deadline = Time.realtimeSinceStartup + 5f;
            while (SceneManager.GetActiveScene().name != "GalapagOSDesktopTest"
                && Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("GalapagOSDesktopTest"));
            yield return null;
            yield return null;
        }

        static void ConfigureShortRewardJourney(SpeciesSimulationPreview preview)
        {
            // Keep the production route and events; use short phases and no predators to isolate reward wiring from balance.
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out var reason), Is.True, reason);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "8", "8", "10100", "64", "0", "12", "0.1", "0", "0", "0",
                false, "20", "4", "0", out reason), Is.True, reason);
        }

        static void StartRewardJourney(Component desktop, SpeciesSimulationPreview preview)
        {
            OpenDesktopApp(desktop, "Simulation");
            Assert.That(preview.JourneyActive, Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(preview.ChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero, "The opening upgrade is offered with no Field Data.");
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero, "The opening upgrade stays free.");
            Assert.That(preview.ChooseJourneyNode("first-cycle"), Is.True);
            preview.PauseSimulation();
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Paused));
        }

        static void RemovePlayerPopulation(SpeciesSimulationPreview preview)
        {
            var cells = preview.Run.Cells;
            var removed = 0;
            for (var y = 0; y < cells.Height; y++)
            {
                for (var x = 0; x < cells.Width; x++)
                {
                    var cell = cells.GetCell(x, y);
                    if (!cell.IsCreature || cell.SpeciesId != preview.Run.PlayerSpeciesId) continue;
                    cells.SetCell(x, y, cell.WithoutEntity());
                    removed++;
                }
            }
            Assert.That(removed, Is.GreaterThan(0), "The extinction fixture must remove a living player population.");
        }

        static void AssertJourneyBalances(Helper_ProfileSession session, int completed, long rabbitData, long researchData)
        {
            Assert.That(session.Current.CompletedRuns, Is.EqualTo(completed));
            Assert.That(session.Current.CompletedRunIds.Count, Is.EqualTo(completed));
            Assert.That(session.Current.RabbitData, Is.EqualTo(rabbitData));
            Assert.That(session.Current.ResearchData, Is.EqualTo(researchData));
            Assert.That(session.Current.GeneLabUnlocked, Is.EqualTo(completed >= 3));
        }

        static void OpenDesktopApp(Component desktop, string appName)
        {
            var command = GetProperty(desktop, "OpenDesktopIconCommand");
            command.GetType().GetMethod("Execute")?.Invoke(command, new object[] { appName });
        }

        static void ExecuteCommand(object target, string propertyName)
        {
            var command = GetProperty(target, propertyName);
            command.GetType().GetMethod("Execute")?.Invoke(command, new object[] { null });
        }

        static IEnumerator LoadLab()
        {
            yield return SceneManager.LoadSceneAsync(LabScene);
            yield return null;
            yield return null;
        }

        static GameObject FindRoot()
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            return roots.Length == 0 ? null : roots[0];
        }

        static Component FindViewModel()
        {
            var root = FindRoot();
            return root == null ? null : root.GetComponent(ViewModelType);
        }

        static object GetProperty(object target, string propertyName)
        {
            var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            Assert.That(property, Is.Not.Null, $"Expected property '{propertyName}'.");
            var value = property.GetValue(target);
            Assert.That(value, Is.Not.Null, $"Expected non-null property '{propertyName}'.");
            return value;
        }

        static string GetVisibility(Component viewModel, string propertyName)
        {
            return GetProperty(viewModel, propertyName).ToString();
        }

        static bool CanExecute(object command)
        {
            return (bool)command.GetType().GetMethod("CanExecute")?.Invoke(command, new object[] { null });
        }

        static void Execute(object command)
        {
            Assert.That(CanExecute(command), Is.True);
            command.GetType().GetMethod("Execute")?.Invoke(command, new object[] { null });
        }

    }
}
