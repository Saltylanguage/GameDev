using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    [Category("Graphics")]
    public sealed class GalapagOSVisualAcceptanceTests
    {
        const string SceneName = "GalapagOSDesktopTest";
        const string DesktopCameraName = "GalapagOS Desktop Test Camera";
        const string SimulationCameraName = "GalapagOS Simulation View";
        const string CurrentDefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v5";
        const string PreviousDefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v4";
        const string LegacyDefaultSettingsKey = "SaltyGame.SpeciesSimulationPreview.DefaultSettings.v3";

        [UnityTest]
        public IEnumerator FieldDataAuthoredRouteCapturesRoundShopReorderAndSuccessEvidence()
        {
            var outputDirectory = TryGetVisualOutputDirectory();
            if (outputDirectory == null)
                Assert.Ignore("Use the visual runner to retain Field Data screenshots.");
            var profileBackup = ProfilePlayerPrefsBackup.Capture();
            try
            {
                Screen.SetResolution(GetVisualDimension("CELLSIM_VISUAL_WIDTH", 1280),
                    GetVisualDimension("CELLSIM_VISUAL_HEIGHT", 720), false);
                yield return SceneManager.LoadSceneAsync(SceneName);
                yield return null;
                yield return null;
                var desktopRoot = GameObject.Find(DesktopCameraName);
                var preview = desktopRoot.GetComponent<SpeciesSimulationPreview>();
                var shell = desktopRoot.GetComponent("SaltyGame.VM_SimulationShell");
                var desktop = desktopRoot.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
                Assert.That(preview, Is.Not.Null);
                Assert.That(shell, Is.Not.Null);
                var profile = UnityEngine.Object.FindAnyObjectByType<Helper_ProfileSession>();
                if (profile == null) profile = new GameObject("Field Data visual profile").AddComponent<Helper_ProfileSession>();
                profile.SetPlaytimeTracking(false);
                profile.CreateProfile("Field Data authored visual route");
                typeof(SpeciesSimulationPreview).GetField("randomizeSeedOnStart", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(preview, false);
                typeof(SpeciesSimulationPreview).GetField("seed", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(preview, 10100);
                SpeciesPresentationPlayModeTests.ConfigureAuthoredFieldDataScenario(preview);
                ExecuteCommand(GetProperty(desktop, "OpenDesktopIconCommand"), "Simulation");
                yield return null;
                yield return null;
                var camera = GameObject.Find(SimulationCameraName).GetComponent<Camera>();
                yield return CaptureCamera(outputDirectory, "fd-01-map-entry", camera);
                ExecuteCommand(GetProperty(shell, "StartFieldDataTestJourneyCommand"));
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
                Assert.That(preview.FieldDataSession, Is.Null);
                yield return CaptureCamera(outputDirectory, "fd-01-selected-route-preview", camera);
                ExecuteCommand(GetProperty(shell, "CloseJourneyMapCommand"));
                yield return CaptureCamera(outputDirectory, "fd-01-return-to-setup", camera);
                ExecuteCommand(GetProperty(shell, "OpenJourneyMapCommand"));
                ExecuteCommand(GetProperty(shell, "StartFieldDataTestJourneyCommand"));
                preview.PauseSimulation();
                var retainedRun = preview.Run;
                yield return CaptureCamera(outputDirectory, "fd-02-running", camera);

                for (var round = 1; round <= 18; round++)
                {
                    for (var tick = 0; tick < 100; tick++)
                        Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True,
                            "Authored seed 10100 must reach round " + round);
                    Assert.That(preview.Run, Is.SameAs(retainedRun));
                    if (round == 18) break;
                    var session = preview.FieldDataSession;
                    var choices = session.GetCurrentRoundRewardChoices() ?? session.GetCurrentNodeRewardChoices();
                    Assert.That(choices, Is.Not.Null);
                    if (round == 1 || round == 2 || round == 6 || round == 12)
                        yield return CaptureCamera(outputDirectory, "fd-" + round.ToString("00") + "-reward", camera);
                    Assert.That(choices.Choices.All(choice => !choice.IncomeOffer.HasValue
                        && choice.OrdinaryUpgrade != null
                        && choice.OrdinaryUpgrade.Id != SpeciesUpgradeCatalog.StrongerAttackId), Is.True);
                    if (round == 1 || round == 6 || round == 12)
                    {
                        var stateBeforeMap = preview.State;
                        OpenFieldDataMapFromRenderedButton(camera, "FieldDataDecisionMapButton");
                        yield return CaptureCamera(outputDirectory, "fd-round-" + round + "-map", camera);
                        ExecuteCommand(GetProperty(shell, "CloseJourneyMapCommand"));
                        Assert.That(preview.State, Is.EqualTo(stateBeforeMap));
                        Assert.That(preview.Run.Tick, Is.EqualTo(round * 100));
                    }
                    ExecuteCommand(GetProperty(shell, "ContinueWithoutUpgradeCommand"));
                    if (round == 1)
                    {
                        yield return BringFieldDataElementIntoView(camera, "FieldDataIncomePanel");
                        yield return CaptureCamera(outputDirectory, "fd-03-round-actions", camera);
                    }
                    if (round == 6 || round == 12)
                    {
                        Assert.That(GetProperty(shell, "FieldDataPopulationControlsVisibility").ToString(), Is.EqualTo("Collapsed"));
                        Assert.That(GetProperty(shell, "FieldDataShopVisibility").ToString(), Is.EqualTo("Collapsed"));
                        yield return CaptureCamera(outputDirectory, "fd-node-" + (round / 6) + "-settlement", camera);
                        ExecuteCommand(GetProperty(shell, "ContinueJourneyCommand"));
                        yield return CaptureCamera(outputDirectory, "fd-node-" + (round / 6) + "-map-to-shop", camera);
                        SpeciesPresentationPlayModeTests.EnterAvailableFieldDataNode((MonoBehaviour)shell, round / 6 * 2 - 1);
                        yield return BringFieldDataElementIntoView(camera, "FieldDataShopPanel");
                        yield return CaptureCamera(outputDirectory, "fd-shop-" + (round / 6) + "-offers", camera);
                        var visit = session.GetCurrentShopVisit();
                        var incomeShopIndex = visit.Offers.ToList().FindIndex(offer => offer.Kind == FieldDataShopOfferKind.IncomeModifier);
                        if (incomeShopIndex >= 0 && session.Ledger.Balance >= visit.Offers[incomeShopIndex].Cost)
                            ExecuteCommand(GetProperty(shell, "BuyFieldDataShopOffer" + (incomeShopIndex + 1) + "Command"));
                        yield return CaptureCamera(outputDirectory, "fd-shop-" + (round / 6) + "-purchased", camera);
                        if (session.CurrentIncomeExecutionOrder.Count >= 2)
                        {
                            var walletBeforeDraft = session.Ledger.Balance;
                            ExecuteCommand(GetProperty(shell, "StartChosenIncomeReorderCommand"));
                            Assert.That(session.ChosenReorderDraft, Is.Not.Null);
                            var rows = (IList)GetProperty(shell, "ChosenIncomeOrderItems");
                            var multiplierRow = Enumerable.Range(1, rows.Count - 1)
                                .FirstOrDefault(index => ((string)GetProperty(rows[index], "Label")).Contains("×"));
                            if (multiplierRow == 0) multiplierRow = rows.Count - 1;
                            ExecuteCommand(GetProperty(rows[multiplierRow], "MoveUpCommand"));
                            yield return BringFieldDataElementIntoView(camera, "FieldDataReorderDraftPanel");
                            yield return CaptureCamera(outputDirectory, "fd-shop-" + (round / 6) + "-chosen-preview", camera);
                            if (round == 6)
                            {
                                ExecuteCommand(GetProperty(shell, "CancelChosenIncomeReorderCommand"));
                                Assert.That(session.Ledger.Balance, Is.EqualTo(walletBeforeDraft));
                            }
                            else
                            {
                                var chosenCost = session.ChosenReorderDraft.Cost;
                                ExecuteCommand(GetProperty(shell, "ConfirmChosenIncomeReorderCommand"));
                                if (walletBeforeDraft < chosenCost)
                                {
                                    Assert.That(session.Ledger.Balance, Is.EqualTo(walletBeforeDraft));
                                    Assert.That(session.ChosenReorderDraft, Is.Not.Null);
                                    ExecuteCommand(GetProperty(shell, "CancelChosenIncomeReorderCommand"));
                                }
                                else
                                    Assert.That(session.Ledger.Balance, Is.EqualTo(walletBeforeDraft - chosenCost));
                                Assert.That(session.ChosenReorderDraft, Is.Null);
                                yield return BringFieldDataElementIntoView(camera, "FieldDataShopPanel");
                                yield return CaptureCamera(outputDirectory, "fd-shop-2-chosen-applied", camera);
                            }
                        }
                    }
                    ExecuteCommand(GetProperty(shell, "ContinueJourneyCommand"));
                    if (round == 6 || round == 12)
                    {
                        yield return CaptureCamera(outputDirectory, "fd-shop-" + (round / 6) + "-map-to-simulation", camera);
                        SpeciesPresentationPlayModeTests.EnterAvailableFieldDataNode((MonoBehaviour)shell, round / 6 * 2);
                    }
                    preview.PauseSimulation();
                }
                Assert.That(preview.FieldDataSession.State, Is.EqualTo(FieldDataJourneyFlowState.Succeeded));
                Assert.That(preview.Run.Tick, Is.EqualTo(1800));
                yield return CaptureCamera(outputDirectory, "fd-18-success", camera);
                OpenFieldDataMapFromRenderedButton(camera, "JourneyResultsMapButton");
                yield return CaptureCamera(outputDirectory, "fd-18-cleared-map", camera);
                ExecuteCommand(GetProperty(shell, "CloseJourneyMapCommand"));
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
                Assert.That((string)GetProperty(shell, "SelectedUpgradeSummaryText"), Does.Contain("income modifiers"));
                Assert.That((string)GetProperty(shell, "ResultsMessageText"), Does.Contain("Field Data remains"));
                var completedSession = preview.FieldDataSession;
                var nextRunNumber = (int)typeof(SpeciesSimulationPreview).GetField("runNumber", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(preview);
                typeof(SpeciesSimulationPreview).GetField("seed", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(preview, 10108 - nextRunNumber);
                ExecuteCommand(GetProperty(shell, "PlayNextSimulationCommand"));
                Assert.That(preview.FieldDataSession, Is.SameAs(completedSession));
                yield return CaptureCamera(outputDirectory, "fd-final-node-map", camera);
                ExecuteCommand(GetProperty(shell, "StartNewJourneyCommand"));
                SpeciesPresentationPlayModeTests.EnterAvailableFieldDataNode((MonoBehaviour)shell, 0);
                preview.PauseSimulation();
                Assert.That(completedSession.Ledger.Balance, Is.Zero);
                Assert.That(preview.FieldDataSession.Ledger.Balance, Is.Zero);
                Assert.That(preview.Run.Seed, Is.EqualTo(10108));
                while (preview.FieldDataSession.State != FieldDataJourneyFlowState.Failed && preview.Run.Tick < 600)
                {
                    if (preview.FieldDataSession.State == FieldDataJourneyFlowState.Running)
                        Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                    else
                    {
                        ExecuteCommand(GetProperty(shell, "ContinueWithoutUpgradeCommand"));
                        ExecuteCommand(GetProperty(shell, "ContinueJourneyCommand"));
                        preview.PauseSimulation();
                    }
                }
                Assert.That(preview.FieldDataSession.State, Is.EqualTo(FieldDataJourneyFlowState.Failed));
                yield return CaptureCamera(outputDirectory, "fd-natural-failure", camera);
            }
            finally
            {
                profileBackup.Restore();
            }
        }

        static void OpenFieldDataMapFromRenderedButton(Camera camera, string name)
        {
            var content = GetProperty(camera.GetComponent("NoesisView"), "Content");
            var button = content.GetType().GetMethod("FindName").Invoke(content, new object[] { name }) as Noesis.Button;
            Assert.That(button, Is.Not.Null);
            Assert.That(button.IsVisible && button.IsEnabled && button.ActualWidth > 0 && button.ActualHeight > 0, Is.True,
                "The map action must be visible and usable on the active panel.");
            ExecuteCommand(GetProperty(button, "Command"));
        }

        static IEnumerator BringFieldDataElementIntoView(Camera camera, string name)
        {
            yield return null;
            yield return null;
            var content = GetProperty(camera.GetComponent("NoesisView"), "Content");
            var element = content.GetType().GetMethod("FindName").Invoke(content, new object[] { name }) as Noesis.FrameworkElement;
            Assert.That(element, Is.Not.Null, "The Field Data view must expose " + name);
            element.BringIntoView();
        }

        [UnityTest]
        public IEnumerator FieldDataShopOwnershipFixtureCapturesCapsRerollsAndPaidReorders()
        {
            var outputDirectory = TryGetVisualOutputDirectory();
            if (outputDirectory == null) Assert.Ignore("Use the visual runner to retain Field Data screenshots.");
            var backup = ProfilePlayerPrefsBackup.Capture();
            try
            {
                Screen.SetResolution(GetVisualDimension("CELLSIM_VISUAL_WIDTH", 1280),
                    GetVisualDimension("CELLSIM_VISUAL_HEIGHT", 720), false);
                yield return SceneManager.LoadSceneAsync(SceneName);
                yield return null;
                yield return null;
                var root = GameObject.Find(DesktopCameraName);
                var preview = root.GetComponent<SpeciesSimulationPreview>();
                var shell = root.GetComponent("SaltyGame.VM_SimulationShell");
                var profile = UnityEngine.Object.FindAnyObjectByType<Helper_ProfileSession>();
                if (profile == null) profile = new GameObject("Field Data fixture profile").AddComponent<Helper_ProfileSession>();
                profile.SetPlaytimeTracking(false);
                profile.CreateProfile("Field Data synthetic ownership fixture");
                ExecuteCommand(GetProperty(root.GetComponent("SaltyGame.VM_GalapagOS_Desktop"), "OpenDesktopIconCommand"), "Simulation");
                yield return null;
                yield return null;
                Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                    "36", "20", "10100", "2000", "0", "1800", "0.2", "0", "0", "0", false,
                    "300", "200", "0", out var error), Is.True, error);
                var hare = new SpeciesId("hare");
                var rules = preview.GetSpeciesRuleEditValues(hare);
                rules.MovementEnabled = false;
                rules.ReproductionEnabled = false;
                rules.Metabolism = "0";
                ExecuteCommand(GetProperty(shell, "StartFieldDataTestJourneyCommand"));
                Assert.That(preview.TryApplySpeciesRuleEditValues(hare, rules, out error), Is.True, error);
                ExecuteCommand(GetProperty(shell, "StartFieldDataTestJourneyCommand"));
                preview.PauseSimulation();
                var session = preview.FieldDataSession;
                // Preloaded ownership reaches a future-shop state the two-shop starter route cannot naturally reach.
                var owned = (IList)typeof(FieldDataJourneySession).GetField("speciesEffects", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(session);
                foreach (var offer in session.Definition.SignedSpeciesOffers.Where(item => item.SignedAmount > 0))
                    for (var copy = 0; copy < 3; copy++)
                        owned.Add(new FieldDataSpeciesEffectInstance("fixture-owned-" + offer.Id + "-" + copy, offer));
                for (var round = 1; round <= 6; round++)
                {
                    for (var tick = 0; tick < 100; tick++) Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                    if (round == 1)
                    {
                        Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance(
                            "fixture-plus", "income-plus-one-per-hare", FieldDataIncomeOperation.AddPerHare, 1m), out error), Is.True, error);
                        Assert.That(session.TryAddIncomeModifier(new FieldDataIncomeModifierInstance(
                            "fixture-times", "income-multiply-one-point-five", FieldDataIncomeOperation.MultiplyPopulationPayout, 1.5m), out error), Is.True, error);
                    }
                    ExecuteCommand(GetProperty(shell, "PurchaseRewardOption1Command"));
                    if (round == 6) break;
                    ExecuteCommand(GetProperty(shell, "ContinueJourneyCommand"));
                    preview.PauseSimulation();
                }
                ExecuteCommand(GetProperty(shell, "ContinueJourneyCommand"));
                SpeciesPresentationPlayModeTests.EnterAvailableFieldDataNode((MonoBehaviour)shell, 1);
                var camera = GameObject.Find(SimulationCameraName).GetComponent<Camera>();
                var visit = session.GetCurrentShopVisit();
                Assert.That(visit.Offers.Any(offer => offer.Locked), Is.True);
                yield return BringFieldDataElementIntoView(camera, "FieldDataShopPanel");
                yield return CaptureCamera(outputDirectory, "fd-fixture-capped-offers", camera);
                var beforeReroll = session.Ledger.Balance;
                while (visit.Offers.Any(offer => offer.Locked))
                {
                    var index = visit.Offers.ToList().FindIndex(offer => offer.Locked);
                    Assert.That(GetProperty(shell, "CanRerollFieldDataShopOffer" + (index + 1)), Is.EqualTo(true));
                    Assert.That(GetProperty(shell, "CanBuyFieldDataShopOffer" + (index + 1)), Is.EqualTo(false));
                    var original = visit.Offers[index].Id;
                    ExecuteCommand(GetProperty(shell, "RerollFieldDataShopOffer" + (index + 1) + "Command"));
                    Assert.That(session.TryRerollCappedShopOffer(original, out _, out _), Is.False);
                    Assert.That(session.Ledger.Balance, Is.EqualTo(beforeReroll));
                }
                Assert.That(visit.Offers.Count, Is.EqualTo(2));
                yield return CaptureCamera(outputDirectory, "fd-fixture-reroll-exhaustion", camera);
                ExecuteCommand(GetProperty(shell, "RandomIncomeReorderCommand"));
                Assert.That(session.Ledger.Balance, Is.EqualTo(beforeReroll - 10));
                Assert.That(session.ChosenReorderDraft, Is.Null);
                ExecuteCommand(GetProperty(shell, "StartChosenIncomeReorderCommand"));
                var rows = (IList)GetProperty(shell, "ChosenIncomeOrderItems");
                ExecuteCommand(GetProperty(rows[0], "MoveDownCommand"));
                var beforeChosen = session.Ledger.Balance;
                yield return BringFieldDataElementIntoView(camera, "FieldDataReorderDraftPanel");
                yield return CaptureCamera(outputDirectory, "fd-fixture-paid-chosen-preview", camera);
                Assert.That(GetProperty(shell, "CanConfirmChosenIncomeReorder"), Is.EqualTo(true));
                ExecuteCommand(GetProperty(shell, "ConfirmChosenIncomeReorderCommand"));
                Assert.That(session.Ledger.Balance, Is.EqualTo(beforeChosen - 110));
                Assert.That(visit.ReorderPriceStep, Is.EqualTo(2));
                yield return BringFieldDataElementIntoView(camera, "FieldDataShopPanel");
                yield return CaptureCamera(outputDirectory, "fd-fixture-paid-reorders-applied", camera);
                ExecuteCommand(GetProperty(shell, "ContinueJourneyCommand"));
                SpeciesPresentationPlayModeTests.EnterAvailableFieldDataNode((MonoBehaviour)shell, 2);
                preview.PauseSimulation();
                for (var tick = 0; tick < 100; tick++) Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                ExecuteCommand(GetProperty(shell, "OpenJourneyMapCommand"));
                ExecuteCommand(GetProperty(shell, "CloseJourneyMapCommand"));
                Assert.That(session.GlobalRound, Is.EqualTo(7));
                Assert.That(session.GetCurrentRoundRewardChoices().Choices, Has.Count.EqualTo(3));
                Assert.That(GetProperty(shell, "CanPurchaseRewardOption1"), Is.EqualTo(true));
                yield return CaptureCamera(outputDirectory, "fd-round-7-after-shop-mutations", camera);
                var beforeReward = session.Ledger.Balance;
                ExecuteCommand(GetProperty(shell, "PurchaseRewardOption1Command"));
                Assert.That(session.GetCurrentRoundRewardChoices().Resolved, Is.True);
                Assert.That(session.Ledger.Balance, Is.EqualTo(beforeReward));
                Assert.That(GetProperty(shell, "CanAddHares"), Is.EqualTo(true));
                ExecuteCommand(GetProperty(shell, "AddHaresCommand"));
                Assert.That(session.Ledger.Balance, Is.EqualTo(beforeReward - 10));
                yield return BringFieldDataElementIntoView(camera, "FieldDataIncomePanel");
                yield return CaptureCamera(outputDirectory, "fd-round-7-after-shop-population-purchase", camera);
            }
            finally { backup.Restore(); }
        }

        [UnityTest]
        public IEnumerator LegacySquareGridSettingsMigrateOnStartup()
        {
            var keys = new[]
            {
                CurrentDefaultSettingsKey,
                PreviousDefaultSettingsKey,
                LegacyDefaultSettingsKey,
            };
            var hadValues = new bool[keys.Length];
            var originalValues = new string[keys.Length];
            for (var i = 0; i < keys.Length; i++)
            {
                hadValues[i] = PlayerPrefs.HasKey(keys[i]);
                originalValues[i] = PlayerPrefs.GetString(keys[i]);
            }

            try
            {
                yield return SceneManager.LoadSceneAsync(SceneName);
                yield return null;

                var desktopRoot = GameObject.Find(DesktopCameraName);
                Assert.That(desktopRoot, Is.Not.Null);
                var preview = desktopRoot.GetComponent<SpeciesSimulationPreview>();
                Assert.That(preview, Is.Not.Null);
                Assert.That(((UnityEngine.Object)GetProperty(preview, "SelectedScenario")).name, Is.EqualTo("ForestEdge"));

                preview.SaveCurrentSettingsAsDefault();
                var authoredSettings = PlayerPrefs.GetString(CurrentDefaultSettingsKey);
                var legacySettings = authoredSettings
                    .Replace($"\"width\":{(int)GetProperty(preview, "GridWidth")}", "\"width\":64")
                    .Replace($"\"height\":{(int)GetProperty(preview, "GridHeight")}", "\"height\":64");
                StringAssert.Contains("\"width\":64", legacySettings);
                StringAssert.Contains("\"height\":64", legacySettings);

                PlayerPrefs.SetString(PreviousDefaultSettingsKey, legacySettings);
                PlayerPrefs.DeleteKey(CurrentDefaultSettingsKey);
                PlayerPrefs.Save();

                yield return SceneManager.LoadSceneAsync(SceneName);
                yield return null;

                desktopRoot = GameObject.Find(DesktopCameraName);
                preview = desktopRoot.GetComponent<SpeciesSimulationPreview>();
                Assert.That(GetProperty(preview, "GridWidth"), Is.EqualTo(36));
                Assert.That(GetProperty(preview, "GridHeight"), Is.EqualTo(20));
                var migratedSettings = PlayerPrefs.GetString(CurrentDefaultSettingsKey);
                StringAssert.Contains("\"width\":36", migratedSettings);
                StringAssert.Contains("\"height\":20", migratedSettings);
            }
            finally
            {
                for (var i = 0; i < keys.Length; i++)
                {
                    if (hadValues[i])
                    {
                        PlayerPrefs.SetString(keys[i], originalValues[i]);
                    }
                    else
                    {
                        PlayerPrefs.DeleteKey(keys[i]);
                    }
                }

                PlayerPrefs.Save();
            }
        }

        [UnityTest]
        public IEnumerator GalapagOSDesktopHomeCapturesVisualEvidenceWithoutOpeningWindows()
        {
            Screen.SetResolution(
                GetVisualDimension("CELLSIM_VISUAL_WIDTH", 1280),
                GetVisualDimension("CELLSIM_VISUAL_HEIGHT", 720),
                false);
            yield return null;
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            var desktopRoot = GameObject.Find(DesktopCameraName);
            Assert.That(desktopRoot, Is.Not.Null);
            var desktopCamera = desktopRoot.GetComponent<Camera>();
            Assert.That(desktopCamera, Is.Not.Null);
            Assert.That(desktopCamera.enabled, Is.True);

            var viewModel = desktopRoot.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(
                GetProperty(viewModel, "LabWindowVisibility").ToString(),
                Is.EqualTo("Collapsed"),
                "The desktop home screen must start without the legacy Lab placeholder window.");
            var openWindows = (System.Collections.IList)GetProperty(viewModel, "OpenDesktopWindows");
            Assert.That(openWindows.Count, Is.Zero, "Home capture must start before any desktop app is opened.");

            var outputDirectory = TryGetVisualOutputDirectory();
            if (outputDirectory == null)
            {
                Assert.Pass("Set CELLSIM_VISUAL_OUTPUT to capture GalapagOS desktop-home evidence.");
            }

            yield return CaptureCamera(outputDirectory, "01-galapagos-desktop-home", desktopCamera);
        }

        [UnityTest]
        public IEnumerator GalapagOSDesktopAndSimulationCaptureGameViewEvidence()
        {
            Screen.SetResolution(
                GetVisualDimension("CELLSIM_VISUAL_WIDTH", 1280),
                GetVisualDimension("CELLSIM_VISUAL_HEIGHT", 720),
                false);
            yield return null;
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            var desktopRoot = GameObject.Find(DesktopCameraName);
            Assert.That(desktopRoot, Is.Not.Null);
            var viewModel = desktopRoot.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            Assert.That(viewModel, Is.Not.Null);

            var outputDirectory = TryGetVisualOutputDirectory();
            if (outputDirectory == null)
            {
                Assert.Pass("Set CELLSIM_VISUAL_OUTPUT to capture GalapagOS Game-view evidence.");
            }

            var openCommand = GetProperty(viewModel, "OpenDesktopIconCommand");
            yield return CaptureCamera(outputDirectory, "01-galapagos-desktop-home", desktopRoot.GetComponent<Camera>());

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Field Notes" });
            var fieldNotesWindows = (System.Collections.IList)GetProperty(viewModel, "OpenDesktopWindows");
            Assert.That(fieldNotesWindows.Count, Is.EqualTo(1));
            Assert.That(GetProperty(fieldNotesWindows[0], "FieldGuideSurfaceVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That((float)GetProperty(fieldNotesWindows[0], "Width"), Is.InRange(1040f, 1510f));
            Assert.That((float)GetProperty(fieldNotesWindows[0], "Height"), Is.InRange(560f, 820f));
            yield return CaptureCamera(outputDirectory, "02-galapagos-field-notes", desktopRoot.GetComponent<Camera>());

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Settings" });
            var openWindows = (System.Collections.IList)GetProperty(viewModel, "OpenDesktopWindows");
            Assert.That(openWindows.Count, Is.EqualTo(2));
            Assert.That(GetProperty(openWindows[1], "SettingsSurfaceVisibility").ToString(), Is.EqualTo("Visible"));
            yield return null;
            yield return null;
            yield return null;

            openCommand.GetType().GetMethod("Execute")?.Invoke(openCommand, new object[] { "Simulation" });
            yield return null;
            yield return null;
            yield return null;
            var simulationCamera = GameObject.Find(SimulationCameraName)?.GetComponent<Camera>();
            Assert.That(simulationCamera, Is.Not.Null);
            var preview = desktopRoot.GetComponent("SaltyGame.SpeciesSimulationPreview");
            var simulationViewModel = desktopRoot.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(simulationViewModel, Is.Not.Null);
            Assert.That(GetProperty(preview, "State").ToString(), Is.EqualTo("Ready"));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Visible"));
            for (var frame = 0; frame < 10; frame++) yield return null;
            ResetJourneyHover(simulationViewModel, ((SpeciesSimulationPreview)preview).JourneyMap.StartNode.NodeId);
            yield return null;
            yield return CaptureCamera(outputDirectory, "03a-galapagos-journey-map", simulationCamera, settleFrames: 0);
            var journeyContent = GetProperty(simulationCamera.GetComponent("NoesisView"), "Content");
            var mapScroll = journeyContent.GetType().GetMethod("FindName")
                ?.Invoke(journeyContent, new object[] { "JourneyMapScroll" });
            Assert.That(mapScroll, Is.Not.Null);
            Assert.That((float)GetProperty(mapScroll, "ScrollableWidth"), Is.GreaterThan(1800f));
            var scrollViewer = (Noesis.ScrollViewer)mapScroll;
            var wheel = new Noesis.MouseWheelEventArgs(scrollViewer, Noesis.UIElement.PreviewMouseWheelEvent, -120);
            scrollViewer.RaiseEvent(wheel);
            yield return null;
            yield return null;
            Assert.That(wheel.Handled, Is.True);
            Assert.That(scrollViewer.HorizontalOffset, Is.GreaterThan(0f));
            mapScroll.GetType().GetMethod("ScrollToHorizontalOffset")?.Invoke(mapScroll, new object[] { 926.6667f });
            yield return null;
            yield return null;
            Assert.That((float)GetProperty(mapScroll, "HorizontalOffset"), Is.EqualTo(926.6667f).Within(1f));
            yield return CaptureCamera(outputDirectory, "03b-galapagos-journey-middle", simulationCamera, settleFrames: 0);
            mapScroll.GetType().GetMethod("ScrollToHorizontalOffset")?.Invoke(mapScroll,
                new[] { GetProperty(mapScroll, "ScrollableWidth") });
            yield return null;
            yield return null;
            yield return CaptureCamera(outputDirectory, "03c-galapagos-journey-right", simulationCamera, settleFrames: 0);
            ExecuteCommand(GetProperty(simulationViewModel, "CloseJourneyMapCommand"));
            ExecuteCommand(openCommand, "Simulation");
            yield return null;
            yield return null;
            Assert.That(scrollViewer.HorizontalOffset, Is.EqualTo(0f).Within(1f),
                "Reopening the journey should return to the available trailheads.");
            var seedfallHover = FindJourneyNode(simulationViewModel, "seedfall");
            ExecuteCommand(GetProperty(seedfallHover, "HoverCommand"));
            Assert.That(GetProperty(seedfallHover, "LabelVisibility").ToString(), Is.EqualTo("Visible"));
            yield return null;
            yield return null;
            yield return CaptureCamera(outputDirectory, "03d-galapagos-journey-node-hover", simulationCamera, settleFrames: 0);
            ExecuteCommand(GetProperty(seedfallHover, "LeaveCommand"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "forest-edge-start"), "ChooseCommand"));
            yield return null;
            yield return CaptureCamera(outputDirectory, "03e-galapagos-first-reward", simulationCamera);
            var firstRewards = (Array)GetProperty(simulationViewModel, "JourneyRewardItems");
            ExecuteCommand(GetProperty(firstRewards.GetValue(0), "ChooseCommand"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "first-cycle"), "ChooseCommand"));
            yield return null;
            yield return null;
            yield return null;
            yield return CaptureCamera(
                outputDirectory,
                "03-galapagos-simulation",
                simulationCamera);

            Assert.That(simulationCamera.enabled, Is.True);
            Assert.That(desktopRoot.GetComponent<Camera>().enabled, Is.False);

            Assert.That(GetProperty(preview, "State").ToString(), Is.EqualTo("Running"));
            Assert.That(GetProperty(simulationViewModel, "SettingsVisibility").ToString(), Is.EqualTo("Collapsed"));
            Assert.That(GetProperty(simulationViewModel, "RunningVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(((UnityEngine.Object)GetProperty(preview, "SelectedScenario")).name, Is.EqualTo("ForestEdge"));
            Assert.That(GetProperty(GetProperty(preview, "PlayerSpecies"), "Value"), Is.EqualTo("hare"));
            Assert.That(GetProperty(preview, "GridWidth"), Is.EqualTo(36));
            Assert.That(GetProperty(preview, "GridHeight"), Is.EqualTo(20));

            var boardViewModel = desktopRoot.GetComponent("SaltyGame.VM_SimulationBoard");
            Assert.That(boardViewModel, Is.Not.Null);
            var snapshot = GetProperty(boardViewModel, "Snapshot");
            Assert.That(snapshot, Is.Not.Null);
            Assert.That(GetProperty(snapshot, "Width"), Is.EqualTo(36));
            Assert.That(GetProperty(snapshot, "Height"), Is.EqualTo(20));

            Assert.That(GetProperty(simulationViewModel, "HerbivorePopulation"), Is.GreaterThan(0));
            Assert.That(GetProperty(simulationViewModel, "CarnivorePopulation"), Is.GreaterThan(0));

            ((SpeciesSimulationPreview)preview).PauseSimulation();
            var clearEventReaction = simulationViewModel.GetType().GetMethod(
                "ClearEventReaction", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(clearEventReaction, Is.Not.Null);
            clearEventReaction.Invoke(simulationViewModel, null);
            Assert.That(
                GetProperty(simulationViewModel, "EventReactionVisibility").ToString(),
                Is.EqualTo("Collapsed"),
                "The baseline capture should start with the event panel hidden.");
            var host = desktopRoot.GetComponent("SaltyGame.GalapagOSDesktopNoesisHost");
            Assert.That(host, Is.Not.Null);
            var board = host
                .GetType()
                .GetField("simulationBoard", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(host);
            Assert.That(board, Is.Not.Null);
            var presentFoxReaction = simulationViewModel.GetType().GetMethod(
                "PresentFoxHuntReaction",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(presentFoxReaction, Is.Not.Null);
            var setHuntCue = board.GetType().GetMethod("SetFoxHuntCue");
            Assert.That(setHuntCue, Is.Not.Null);
            var boardSnapshot = (SimulationBoardSnapshot)snapshot;
            var foxId = new SpeciesId("fox");
            setHuntCue.Invoke(board, new object[] { 0, 0, -1 });
            yield return null;
            yield return CaptureCamera(outputDirectory, "04-fox-hunt-before", simulationCamera, settleFrames: 1);
            var foxFound = false;
            for (var y = 0; y < boardSnapshot.Height && !foxFound; y++)
            {
                for (var x = 0; x < boardSnapshot.Width; x++)
                {
                    if (boardSnapshot.GetCell(x, y).SpeciesId != foxId)
                    {
                        continue;
                    }

                    setHuntCue.Invoke(board, new object[] { x, y, boardSnapshot.Tick + 1 });
                    foxFound = true;
                    break;
                }
            }

            Assert.That(foxFound, Is.True, "Visual proof needs a Fox on the board.");
            presentFoxReaction.Invoke(simulationViewModel, null);
            Assert.That(GetProperty(simulationViewModel, "EventReactionVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(GetProperty(simulationViewModel, "EventReactionFoxVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(GetProperty(simulationViewModel, "EventReactionTitleText"), Is.EqualTo("SNEAKY PAWS!"));
            yield return CaptureCamera(outputDirectory, "05-fox-hunt-cue", simulationCamera, settleFrames: 1);

            var setMatingCue = board.GetType().GetMethod("SetMatingCue");
            Assert.That(setMatingCue, Is.Not.Null);
            var presentBirthReaction = simulationViewModel.GetType().GetMethod(
                "PresentBirthReaction",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(presentBirthReaction, Is.Not.Null);
            setMatingCue.Invoke(board, new object[] { 0, 0, 0, 0, 0, 0, -1 });
            yield return null;
            yield return CaptureCamera(outputDirectory, "06-mating-before", simulationCamera, settleFrames: 1);

            var hareId = new SpeciesId("hare");
            var hareFound = false;
            for (var y = 0; y < boardSnapshot.Height && !hareFound; y++)
            {
                for (var x = 0; x < boardSnapshot.Width; x++)
                {
                    if (boardSnapshot.GetCell(x, y).SpeciesId != hareId)
                    {
                        continue;
                    }

                    var mateX = x + 1 < boardSnapshot.Width ? x + 1 : x - 1;
                    var childY = y + 1 < boardSnapshot.Height ? y + 1 : y - 1;
                    setMatingCue.Invoke(board, new object[] { x, y, mateX, y, x, childY, boardSnapshot.Tick + 2 });
                    hareFound = true;
                    break;
                }
            }

            Assert.That(hareFound, Is.True, "Visual proof needs a Hare on the board.");
            presentBirthReaction.Invoke(simulationViewModel, new object[] { 1, false });
            Assert.That(GetProperty(simulationViewModel, "EventReactionVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(GetProperty(simulationViewModel, "EventReactionRabbitVisibility").ToString(), Is.EqualTo("Visible"));
            Assert.That(GetProperty(simulationViewModel, "EventReactionTitleText"), Is.EqualTo("NEW LITTLE LIFE!"));
            yield return CaptureCamera(outputDirectory, "07-mating-cue", simulationCamera, settleFrames: 12);
        }

        [UnityTest]
        public IEnumerator JourneyReportReturnsToMapWithAvailableRoutes()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;
            yield return null;

            var desktopRoot = GameObject.Find(DesktopCameraName);
            var preview = desktopRoot?.GetComponent<SpeciesSimulationPreview>();
            var desktopViewModel = desktopRoot?.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var simulationViewModel = desktopRoot?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(desktopViewModel, Is.Not.Null);
            Assert.That(simulationViewModel, Is.Not.Null);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out var message), Is.True, message);

            ExecuteCommand(GetProperty(desktopViewModel, "OpenDesktopIconCommand"), "Simulation");
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Visible"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "forest-edge-start"), "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyNodeReward));
            var popupChoices = (Array)GetProperty(simulationViewModel, "JourneyRewardItems");
            Assert.That(popupChoices.Length, Is.EqualTo(3));
            ExecuteCommand(GetProperty(popupChoices.GetValue(0), "ChooseCommand"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "first-cycle"), "ChooseCommand"));
            for (var phase = 1; phase <= 5; phase++)
            {
                var deadline = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.PhaseDecision
                       && Time.realtimeSinceStartup < deadline)
                {
                    yield return null;
                }
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                ExecuteCommand(GetProperty(simulationViewModel, "ContinueWithoutUpgradeCommand"));
            }

            var boundaryDeadline = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.JourneyDecision
                   && Time.realtimeSinceStartup < boundaryDeadline)
            {
                yield return null;
            }
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyDecision));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Collapsed"));
            Assert.That(GetProperty(simulationViewModel, "ResultsVisibility").ToString(), Is.EqualTo("Visible"));
            var outputDirectory = TryGetVisualOutputDirectory();
            var simulationCamera = GameObject.Find(SimulationCameraName)?.GetComponent<Camera>();
            if (outputDirectory != null && simulationCamera != null)
            {
                yield return CaptureCamera(outputDirectory, "08-cycle-one-report", simulationCamera);
            }
            ExecuteCommand(GetProperty(simulationViewModel, "PlayNextSimulationCommand"));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Visible"));
            if (outputDirectory != null && simulationCamera != null)
            {
                yield return CaptureCamera(outputDirectory, "09-cycle-one-route-choice", simulationCamera);
            }
            var noesisView = GameObject.Find(SimulationCameraName)?.GetComponent("NoesisView");
            Assert.That(noesisView, Is.Not.Null);
            var viewContent = GetProperty(noesisView, "Content");
            var mapPanel = viewContent.GetType().GetMethod("FindName")
                ?.Invoke(viewContent, new object[] { "JourneyMapPanel" });
            Assert.That(mapPanel, Is.Not.Null);
            Assert.That(GetProperty(mapPanel, "IsVisible"), Is.EqualTo(true));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapRouteGeometry"), Is.Not.Null);
            var visibleNodes = (Array)GetProperty(simulationViewModel, "JourneyMapNodes");
            Assert.That(visibleNodes.Length, Is.EqualTo(preview.JourneyMap.Nodes.Count));
            var seedfall = FindJourneyNode(simulationViewModel, "seedfall");
            Assert.That(GetProperty(seedfall, "CanChoose"), Is.EqualTo(true));
            ExecuteCommand(GetProperty(seedfall, "HoverCommand"));
            Assert.That(GetProperty(simulationViewModel, "JourneyDetailTitle"), Is.EqualTo(GetProperty(seedfall, "Title")));
            Assert.That(GetProperty(simulationViewModel, "JourneyDetailDescription"), Is.EqualTo(GetProperty(seedfall, "Description")));
            ExecuteCommand(GetProperty(seedfall, "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyNodeReward));
            Assert.That(GetProperty(simulationViewModel, "JourneyNodePopupVisibility").ToString(), Is.EqualTo("Visible"));
            if (outputDirectory != null && simulationCamera != null)
            {
                yield return CaptureCamera(outputDirectory, "10-habitat-condition-popup", simulationCamera);
            }
            var conditionChoices = (Array)GetProperty(simulationViewModel, "JourneyRewardItems");
            Assert.That(conditionChoices.Length, Is.EqualTo(1));
            ExecuteCommand(GetProperty(conditionChoices.GetValue(0), "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyDecision));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Visible"));
            var secondSimulation = FindJourneyNode(simulationViewModel, "burrow-study");
            Assert.That(GetProperty(secondSimulation, "CanChoose"), Is.EqualTo(true));
            if (outputDirectory != null && simulationCamera != null)
            {
                yield return CaptureCamera(outputDirectory, "11-next-simulation-available", simulationCamera);
            }
            ExecuteCommand(GetProperty(secondSimulation, "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(GetProperty(simulationViewModel, "JourneyMapVisibility").ToString(), Is.EqualTo("Collapsed"));
        }

        [UnityTest]
        public IEnumerator ReopeningSimulationAfterResultsStartsANewRun()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;
            yield return null;

            var desktopRoot = GameObject.Find(DesktopCameraName);
            Assert.That(desktopRoot, Is.Not.Null);
            var desktopViewModel = desktopRoot.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var openCommand = GetProperty(desktopViewModel, "OpenDesktopIconCommand");
            var preview = desktopRoot.GetComponent<SpeciesSimulationPreview>();
            var simulationViewModel = desktopRoot.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(simulationViewModel, Is.Not.Null);

            Assert.That(preview.TryApplyContinuousPhases(true, "1", out var phaseMessage), Is.True, phaseMessage);
            ExecuteCommand(openCommand, "Simulation");
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "forest-edge-start"), "ChooseCommand"));
            var rewards = (Array)GetProperty(simulationViewModel, "JourneyRewardItems");
            ExecuteCommand(GetProperty(rewards.GetValue(0), "ChooseCommand"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "first-cycle"), "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));

            var completedRun = preview.Run;
            preview.EndSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            yield return null;
            ExecuteCommand(GetProperty(simulationViewModel, "ReturnToLabCommand"));

            ExecuteCommand(openCommand, "Simulation");
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(preview.Run, Is.Not.SameAs(completedRun));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "forest-edge-start"), "ChooseCommand"));
            var secondRewards = (Array)GetProperty(simulationViewModel, "JourneyRewardItems");
            ExecuteCommand(GetProperty(secondRewards.GetValue(0), "ChooseCommand"));
            ExecuteCommand(GetProperty(FindJourneyNode(simulationViewModel, "first-cycle"), "ChooseCommand"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            var reopenedRun = preview.Run;
            ExecuteCommand(openCommand, "Simulation");
            Assert.That(preview.Run, Is.SameAs(reopenedRun), "Opening an already running simulation must not start another run.");
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.EqualTo(1));
            Assert.That(preview.Progression.Currency, Is.Zero);
            yield return null;
            Assert.That(GetProperty(simulationViewModel, "ResultsVisibility").ToString(), Is.EqualTo("Collapsed"));

            preview.EndSimulation();
            yield return null;
            ExecuteCommand(GetProperty(simulationViewModel, "OpenJourneyMapCommand"));
            var content = GetProperty(GameObject.Find(SimulationCameraName).GetComponent("NoesisView"), "Content");
            var navigation = (Noesis.ScrollViewer)content.GetType().GetMethod("FindName")
                ?.Invoke(content, new object[] { "JourneyMapScroll" });
            Assert.That(navigation, Is.Not.Null);
            yield return null;
            yield return null;
            Assert.That(navigation.ScrollableWidth, Is.GreaterThan(1800f));
            navigation.ScrollToHorizontalOffset(navigation.ScrollableWidth);
            yield return null;
            yield return null;
            Assert.That(navigation.HorizontalOffset, Is.GreaterThan(1800f));
            ExecuteCommand(GetProperty(simulationViewModel, "StartNewJourneyCommand"));
            yield return null;
            yield return null;
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(navigation.HorizontalOffset, Is.EqualTo(0f).Within(1f));
            ExecuteCommand(GetProperty(simulationViewModel, "CloseJourneyMapCommand"));
        }

        static object GetProperty(object target, string propertyName)
        {
            var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            Assert.That(property, Is.Not.Null, $"Expected property '{propertyName}'.");
            return property.GetValue(target);
        }

        static void ExecuteCommand(object command, params object[] arguments)
        {
            Assert.That(command, Is.Not.Null);
            var parameters = arguments.Length == 0 ? new object[] { null } : arguments;
            Assert.That((bool)command.GetType().GetMethod("CanExecute")?.Invoke(command, parameters), Is.True);
            command.GetType().GetMethod("Execute")?.Invoke(command, parameters);
        }

        static object FindJourneyNode(object simulationViewModel, string nodeId)
        {
            foreach (var node in (IEnumerable)GetProperty(simulationViewModel, "JourneyMapNodes"))
            {
                if (string.Equals((string)GetProperty(node, "NodeId"), nodeId, StringComparison.Ordinal))
                {
                    return node;
                }
            }

            Assert.Fail($"Journey node '{nodeId}' was not rendered.");
            return null;
        }

        static void ResetJourneyHover(object simulationViewModel, string startNodeId)
        {
            foreach (var node in (IEnumerable)GetProperty(simulationViewModel, "JourneyMapNodes"))
            {
                ExecuteCommand(GetProperty(node, "LeaveCommand"));
                Assert.That(GetProperty(node, "LabelVisibility").ToString(), Is.EqualTo("Collapsed"));
            }

            var start = FindJourneyNode(simulationViewModel, startNodeId);
            ExecuteCommand(GetProperty(start, "HoverCommand"));
            ExecuteCommand(GetProperty(start, "LeaveCommand"));
        }

        static string TryGetVisualOutputDirectory()
        {
            var configuredPath = VisualTestConfiguration.GetValue("CELLSIM_VISUAL_OUTPUT");
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                return null;
            }

            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            var directory = Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(projectRoot, configuredPath);
            Directory.CreateDirectory(directory);
            return directory;
        }

        static IEnumerator CaptureCamera(string directory, string name, Camera camera, int settleFrames = 10)
        {
            for (var frame = 0; frame < settleFrames; frame++)
            {
                yield return null;
            }

            Assert.That(camera, Is.Not.Null, $"Visual evidence requires camera '{name}'.");
            var width = GetVisualDimension("CELLSIM_VISUAL_WIDTH", 1280);
            var height = GetVisualDimension("CELLSIM_VISUAL_HEIGHT", 720);
            var path = Path.Combine(directory, name + ".png");
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var image = new Texture2D(width, height, TextureFormat.RGBA32, false);
            try
            {
                renderTexture.Create();
                camera.targetTexture = renderTexture;
                // Let Noesis arrange against the capture target instead of the Editor Game view.
                yield return null;
                yield return null;
                camera.Render();
                RenderTexture.active = renderTexture;
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                image.Apply();
                File.WriteAllBytes(path, image.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                UnityEngine.Object.Destroy(image);
                UnityEngine.Object.Destroy(renderTexture);
            }

            Assert.That(File.Exists(path), Is.True, $"Unity did not write screenshot '{path}'.");
        }

        static int GetVisualDimension(string environmentVariable, int fallback)
        {
            return int.TryParse(VisualTestConfiguration.GetValue(environmentVariable), out var value)
                ? Mathf.Max(1, value)
                : fallback;
        }
    }
}
