using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    [Category("Graphics")]
    public sealed class SpeciesPresentationPlayModeTests
    {
        ProfilePlayerPrefsBackup journeyProfileSave;

        [UnityTearDown]
        public IEnumerator RestoreJourneyProfileSave()
        {
            if (journeyProfileSave == null)
            {
                yield break;
            }

            foreach (var session in UnityEngine.Object.FindObjectsByType<Helper_ProfileSession>(FindObjectsSortMode.None))
            {
                session.SetPlaytimeTracking(false);
                session.enabled = false;
            }

            journeyProfileSave.Restore();
            journeyProfileSave = null;
        }

        [UnityTest]
        public IEnumerator CellularPrototypeUsesSerializedComposition()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var root = GameObject.Find("Cellular Automata Prototype");
            var camera = GameObject.Find("Prototype Camera");
            Assert.That(root, Is.Not.Null);
            Assert.That(camera, Is.Not.Null);

            Assert.That(root.GetComponent("SaltyGame.CellularAutomataPrototypeRuntime"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.SpeciesSimulationNoesisHost"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.Helper_Simulation"), Is.Not.Null);
            Assert.That(root.GetComponent("SaltyGame.SpeciesSimulationPreview"), Is.Not.Null);
            Assert.That(camera.GetComponent<Camera>(), Is.Not.Null);
            Assert.That(camera.GetComponent("NoesisView"), Is.Not.Null);
            var viewModel = camera.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(camera.GetComponent("SaltyGame.VM_SimulationBoard"), Is.Not.Null);

            var canStart = viewModel.GetType().GetProperty("CanStart");
            Assert.That(canStart, Is.Not.Null);
            var preview = root.GetComponent<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(preview.SelectedScenario.name, Is.EqualTo("ForestEdge"));
            Assert.That(preview.PlayerSpecies.Value, Is.EqualTo("hare"));
            Assert.That(canStart.GetValue(viewModel), Is.False);
        }

        [UnityTest]
        public IEnumerator DesktopJourneyLaunchShowsMapBeforeFirstSimulation()
        {
            journeyProfileSave = ProfilePlayerPrefsBackup.Capture();
            var profileSession = UnityEngine.Object.FindAnyObjectByType<Helper_ProfileSession>();
            if (profileSession == null)
            {
                profileSession = new GameObject("Journey test profile").AddComponent<Helper_ProfileSession>();
            }

            var profileId = profileSession.CreateProfile("Journey test").ProfileId;

            var transitionObject = new GameObject("Journey launch test transition");
            var transition = transitionObject.AddComponent<Helper_SceneTransition>();
            var startingUpgrade = new SpeciesUpgradeSnapshot(
                "journey-test-starting-block",
                "Starting Block",
                "A frozen launch upgrade for the journey test.",
                new SpeciesId("hare"),
                0,
                new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.BlockAmount, 1f) });
            var launch = new SimulationLaunchRequest(
                profileId, "ForestEdge", "hare", 10100,
                orderedUpgradeSnapshots: new[] { startingUpgrade });
            Assert.That(transition.LoadSimulation(launch), Is.True);
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>()?.SpeciesPreview;
            var viewModel = GameObject.Find("Prototype Camera")?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(preview.JourneyActive, Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Ready));
            Assert.That(viewModel.GetType().GetProperty("JourneyMapVisibility")
                ?.GetValue(viewModel)?.ToString(), Is.EqualTo("Visible"));
            Assert.That(preview.ChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.ChooseJourneyNode("first-cycle"), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(preview.Run.UpgradeLoadout.Any(
                upgrade => upgrade.Id == startingUpgrade.Id), Is.True);
        }

        [UnityTest]
        public IEnumerator DesktopSimulationIconOpensTheJourneyMap()
        {
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var desktopViewModel = GameObject.Find("GalapagOS Desktop Test Camera")
                ?.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var preview = UnityEngine.Object.FindAnyObjectByType<SpeciesSimulationPreview>();
            var simulationViewModel = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(
                    FindObjectsSortMode.None)
                .FirstOrDefault(component => component.GetType().FullName == "SaltyGame.VM_SimulationShell");
            Assert.That(desktopViewModel, Is.Not.Null);
            Assert.That(preview, Is.Not.Null);
            Assert.That(simulationViewModel, Is.Not.Null);
            Assert.That(preview.JourneyActive, Is.True);

            var open = desktopViewModel.GetType().GetProperty("OpenDesktopIconCommand")
                ?.GetValue(desktopViewModel);
            Assert.That(open, Is.Not.Null);
            open.GetType().GetMethod("Execute")?.Invoke(open, new object[] { "Simulation" });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(preview.FieldDataSession, Is.Null,
                "Opening the map must preserve the existing route until the Field Data test route is explicitly selected.");
            Assert.That(simulationViewModel.GetType().GetProperty("JourneyMapVisibility")
                ?.GetValue(simulationViewModel)?.ToString(), Is.EqualTo("Visible"));
            var back = simulationViewModel.GetType().GetProperty("CloseJourneyMapCommand")
                ?.GetValue(simulationViewModel);
            back?.GetType().GetMethod("Execute")?.Invoke(back, new object[] { null });
            Assert.That(GameObject.Find("GalapagOS Desktop Test Camera")?.GetComponent<Camera>()?.enabled,
                Is.True);
            open.GetType().GetMethod("Execute")?.Invoke(open, new object[] { "Simulation" });
            Assert.That(simulationViewModel.GetType().GetProperty("JourneyMapVisibility")
                ?.GetValue(simulationViewModel)?.ToString(), Is.EqualTo("Visible"));
            Assert.That(preview.ChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.ChooseJourneyNode("first-cycle"), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.FieldDataSession, Is.Null,
                "Selecting a legacy simulation node must continue through the existing route composition.");
        }

        [UnityTest]
        public IEnumerator FieldDataTestRouteCanBeSelectedFromTheJourneyMapBeforeFirstSimulation()
        {
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<SpeciesSimulationPreview>();
            var shell = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .FirstOrDefault(component => component.GetType().FullName == "SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(shell, Is.Not.Null);
            Assert.That(preview.HasFieldDataTestJourney, Is.True);
            Assert.That(preview.JourneyMap, Is.Not.Null, "The test route is offered from the existing journey map entry.");
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            // Keep this UI/ordering fixture independent of ecology tuning and random free rewards.
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "36", "20", "10100", "2000", "0", "1800", "0.2", "0", "0", "0", false,
                "300", "200", "0", out var setupError), Is.True, setupError);

            var shellType = shell.GetType();
            var openMap = shellType.GetProperty("OpenJourneyMapCommand")?.GetValue(shell);
            Assert.That(openMap, Is.Not.Null);
            openMap.GetType().GetMethod("Execute")?.Invoke(openMap, new object[] { null });
            Assert.That(shellType.GetProperty("JourneyMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
            Assert.That(shellType.GetProperty("FieldDataTestRouteVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));

            var startRoute = shellType.GetProperty("StartFieldDataTestJourneyCommand")?.GetValue(shell);
            Assert.That(startRoute, Is.Not.Null);
            startRoute.GetType().GetMethod("Execute")?.Invoke(startRoute, new object[] { null });
            var stableRules = preview.GetSpeciesRuleEditValues(new SpeciesId("hare"));
            stableRules.MovementEnabled = false;
            stableRules.ReproductionEnabled = false;
            stableRules.Metabolism = "0";
            Assert.That(preview.TryApplySpeciesRuleEditValues(new SpeciesId("hare"), stableRules, out setupError), Is.True, setupError);
            Assert.That(preview.ConfiguredFieldDataJourney, Is.Not.Null);
            Assert.That(preview.FieldDataSession, Is.Null, "Selecting the route must not start the simulation.");
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Ready));
            Assert.That(shellType.GetProperty("JourneyMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
            Assert.That(shellType.GetProperty("FieldDataTestRouteButtonText")?.GetValue(shell), Is.EqualTo("START THIS ROUTE"));
            var routeSummary = shellType.GetProperty("JourneyMapSummary")?.GetValue(shell)?.ToString();
            Assert.That(((Array)shellType.GetProperty("FieldDataRouteNodes")?.GetValue(shell)).Length, Is.EqualTo(5));
            Assert.That(shellType.GetProperty("FieldDataRouteMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
            foreach (var node in new[] { "SIMULATION 1", "SHOP 1", "SIMULATION 2", "SHOP 2", "SIMULATION 3" })
                Assert.That(routeSummary, Does.Contain(node));
            ExecuteShellCommand(shell, "CloseJourneyMapCommand");
            Assert.That(shellType.GetProperty("JourneyMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Collapsed"));
            Assert.That(preview.FieldDataSession, Is.Null);
            ExecuteShellCommand(shell, "OpenJourneyMapCommand");
            Assert.That(shellType.GetProperty("JourneyMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
            startRoute.GetType().GetMethod("Execute")?.Invoke(startRoute, new object[] { null });
            Assert.That(preview.FieldDataSession, Is.Not.Null);
            Assert.That(preview.FieldDataSession.State, Is.EqualTo(FieldDataJourneyFlowState.Running));
            Assert.That(preview.FieldDataSession.Ledger.Balance, Is.Zero);
            Assert.That(preview.Run.Tick, Is.Zero);
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Running));

            var runningRun = preview.Run;
            ExecuteShellCommand(shell, "OpenJourneyMapCommand");
            var mapTick = runningRun.Tick;
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
            yield return null;
            yield return null;
            Assert.That(preview.Run.Tick, Is.EqualTo(mapTick), "The map must pause a running simulation.");
            ExecuteShellCommand(shell, "CloseJourneyMapCommand");
            Assert.That(preview.Run, Is.SameAs(runningRun));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            preview.PauseSimulation();
            AssertFieldDataMapRoundTrip(shell, preview);
            for (var tick = 0; tick < 100; tick++)
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.FieldDataRoundDecision));
            AssertFieldDataMapRoundTrip(shell, preview);
            yield return null;
            var breakdown = shellType.GetProperty("FieldDataBreakdownText")?.GetValue(shell)?.ToString();
            Assert.That(breakdown, Does.Contain("All populations at settlement"));
            Assert.That(breakdown, Does.Contain("Selected population income"));
            Assert.That(shellType.GetProperty("FieldDataPopulationControlsVisibility")?.GetValue(shell)?.ToString(),
                Is.EqualTo("Collapsed"), "Population actions wait until the free reward is resolved.");

            ExecuteShellCommand(shell, "PurchaseRewardOption1Command");
            Assert.That(shellType.GetProperty("FieldDataPopulationControlsVisibility")?.GetValue(shell)?.ToString(),
                Is.EqualTo("Visible"));
            Assert.That(preview.FieldDataSession.TryAddIncomeModifier(
                new FieldDataIncomeModifierInstance("ui-income-plus", "income-plus-one-per-hare",
                    FieldDataIncomeOperation.AddPerHare, 1m), out var incomeError, "ui-binding-test"), Is.True, incomeError);
            Assert.That(preview.FieldDataSession.TryAddIncomeModifier(
                new FieldDataIncomeModifierInstance("ui-income-times", "income-multiply-one-point-five",
                    FieldDataIncomeOperation.MultiplyPopulationPayout, 1.5m), out incomeError, "ui-binding-test"), Is.True, incomeError);
            ExecuteShellCommand(shell, "ContinueJourneyCommand");
            preview.PauseSimulation();
            for (var tick = 0; tick < 100; tick++)
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            ExecuteShellCommand(shell, "PurchaseRewardOption1Command");
            ExecuteShellCommand(shell, "ContinueJourneyCommand");
            preview.PauseSimulation();
            for (var tick = 0; tick < 100; tick++)
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            ExecuteShellCommand(shell, "PurchaseRewardOption1Command");
            var retainedRun = preview.Run;

            for (var round = 4; round <= 6; round++)
            {
                if (preview.FieldDataSession.GetCurrentRoundRewardChoices()?.Resolved == false)
                    ExecuteShellCommand(shell, "PurchaseRewardOption1Command");
                ExecuteShellCommand(shell, "ContinueJourneyCommand");
                preview.PauseSimulation();
                for (var tick = 0; tick < 100; tick++)
                    Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True,
                        $"Round {round}, tick {preview.Run.Tick}, preview {preview.State}, run {preview.Run.Status}, session {preview.FieldDataSession.State}: {preview.FieldDataSession.SettlementError}");
                if (round < 6)
                    Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.FieldDataRoundDecision));
            }

            Assert.That(preview.FieldDataSession.State, Is.EqualTo(FieldDataJourneyFlowState.Shop));
            yield return null;
            Assert.That(preview.Run, Is.SameAs(retainedRun));
            Assert.That(shellType.GetProperty("FieldDataShopVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Collapsed"),
                "The node reward is resolved before shop actions become available.");
            var shopWallet = preview.FieldDataSession.Ledger.Balance;
            ExecuteShellCommand(shell, "BuyFieldDataShopOffer1Command");
            ExecuteShellCommand(shell, "StartChosenIncomeReorderCommand");
            Assert.That(preview.FieldDataSession.Ledger.Balance, Is.EqualTo(shopWallet));
            Assert.That(preview.FieldDataSession.ChosenReorderDraft, Is.Null);
            ExecuteShellCommand(shell, "PurchaseRewardOption1Command");
            Assert.That(shellType.GetProperty("FieldDataPopulationControlsVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Collapsed"));
            Assert.That(shellType.GetProperty("FieldDataShopVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Collapsed"),
                "Resolving the node reward must keep the player at settlement until they return to the map.");
            ExecuteShellCommand(shell, "ContinueJourneyCommand");
            EnterAvailableFieldDataNode(shell, 1);
            Assert.That(shellType.GetProperty("FieldDataShopVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
            Assert.That(shellType.GetProperty("FieldDataShopSummaryText")?.GetValue(shell)?.ToString(), Does.Contain("randomized offers"));
            Assert.That(shellType.GetProperty("FieldDataRewardChoicesVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Collapsed"));
            var offers = preview.FieldDataSession.GetCurrentShopVisit().Offers;
            for (var offerIndex = 0; offerIndex < offers.Count; offerIndex++)
            {
                var offer = offers[offerIndex];
                Assert.That(shellType.GetProperty("CanRerollFieldDataShopOffer" + (offerIndex + 1))?.GetValue(shell),
                    Is.EqualTo(offer.Locked && !offer.Purchased));
                Assert.That(shellType.GetProperty("FieldDataShopOffer" + (offerIndex + 1) + "Text")?.GetValue(shell)?.ToString(),
                    Does.Contain(offer.Kind == FieldDataShopOfferKind.SpeciesEffect ? "Family owned" : "No copy cap"));
            }
            ExecuteShellCommand(shell, "StartChosenIncomeReorderCommand");
            Assert.That(shellType.GetProperty("CanConfirmChosenIncomeReorder")?.GetValue(shell), Is.EqualTo(false),
                "An unchanged preview cannot be purchased.");
            var chosenItems = shellType.GetProperty("ChosenIncomeOrderItems")?.GetValue(shell) as System.Collections.IList;
            Assert.That(chosenItems, Is.Not.Null.And.Not.Empty, "The chosen reorder draft should expose each modifier as an editable row.");
            Assert.That(chosenItems.Count, Is.GreaterThanOrEqualTo(2));
            var originalFirst = GetChosenIncomeItemName(chosenItems[0]);
            var originalSecond = GetChosenIncomeItemName(chosenItems[1]);
            var firstRow = chosenItems[0];
            var firstMoveDown = firstRow.GetType().GetProperty("MoveDownCommand")?.GetValue(firstRow);
            Assert.That(firstMoveDown, Is.Not.Null);
            firstMoveDown.GetType().GetMethod("Execute")?.Invoke(firstMoveDown, new object[] { null });
            chosenItems = shellType.GetProperty("ChosenIncomeOrderItems")?.GetValue(shell) as System.Collections.IList;
            Assert.That(GetChosenIncomeItemName(chosenItems[0]), Is.EqualTo(originalSecond));
            Assert.That(GetChosenIncomeItemName(chosenItems[1]), Is.EqualTo(originalFirst));
            ExecuteShellCommand(shell, "CancelChosenIncomeReorderCommand");
            Assert.That(preview.FieldDataSession.Ledger.Balance, Is.EqualTo(shopWallet), "Cancelling an edited preview must not charge the wallet.");
            Assert.That(preview.FieldDataSession.ChosenReorderDraft, Is.Null);
            ExecuteShellCommand(shell, "ContinueWithoutUpgradeCommand");
            Assert.That(shellType.GetProperty("FieldDataPopulationControlsVisibility")?.GetValue(shell)?.ToString(),
                Is.EqualTo("Collapsed"), "Completed-node settlement and shops must never offer population controls.");
            Assert.That(shellType.GetProperty("FieldDataShopOffer1Text")?.GetValue(shell)?.ToString(), Does.Contain("FD"));

            ExecuteShellCommand(shell, "BuyFieldDataShopOffer1Command");
            Assert.That(preview.FieldDataSession.Ledger.Balance, Is.LessThan(shopWallet));
            ExecuteShellCommand(shell, "ContinueJourneyCommand");
            EnterAvailableFieldDataNode(shell, 2);
            preview.PauseSimulation();
            for (var tick = 0; tick < 100; tick++)
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            ExecuteShellCommand(shell, "OpenJourneyMapCommand");
            ExecuteShellCommand(shell, "CloseJourneyMapCommand");
            Assert.That(preview.FieldDataSession.GlobalRound, Is.EqualTo(7));
            var roundSevenChoices = preview.FieldDataSession.GetCurrentRoundRewardChoices();
            Assert.That(roundSevenChoices.Choices, Is.Not.Empty);
            Assert.That(roundSevenChoices.Choices.All(choice => choice.OrdinaryUpgrade != null
                && choice.OrdinaryUpgrade.Id != SpeciesUpgradeCatalog.StrongerAttackId), Is.True);
            Assert.That(shellType.GetProperty("CanPurchaseRewardOption1")?.GetValue(shell), Is.EqualTo(true));
            var beforeFreeChoice = preview.FieldDataSession.Ledger.Balance;
            ExecuteShellCommand(shell, "PurchaseRewardOption1Command");
            Assert.That(roundSevenChoices.Resolved, Is.True);
            Assert.That(preview.FieldDataSession.Ledger.Balance, Is.EqualTo(beforeFreeChoice));
            Assert.That(shellType.GetProperty("CanAddHares")?.GetValue(shell), Is.EqualTo(true));
            var populationBefore = preview.Run.PopulationHistory.Last().GetCount(preview.PlayerSpecies);
            ExecuteShellCommand(shell, "AddHaresCommand");
            Assert.That(preview.Run.PopulationHistory.Last().GetCount(preview.PlayerSpecies), Is.EqualTo(populationBefore + 1));
            Assert.That(preview.FieldDataSession.Ledger.Balance, Is.EqualTo(beforeFreeChoice - 10));

            ExecuteShellCommand(shell, "RemoveHaresCommand");
            Assert.That(preview.Run.PopulationHistory.Last().GetCount(preview.PlayerSpecies), Is.EqualTo(populationBefore));
            var accountingSummary = shellType.GetProperty("ExperimentalHerbivoreStatLineSummary")?.GetValue(shell)?.ToString();
            Assert.That(accountingSummary, Does.Contain("ADD: " + preview.Run.GetBoundaryPopulationAdded(preview.PlayerSpecies)));
            Assert.That(accountingSummary, Does.Contain("REM: " + preview.Run.GetBoundaryPopulationRemoved(preview.PlayerSpecies)));
            Assert.That(accountingSummary, Does.Contain("Reconciled: True").And.Not.Contain("APS: INVALID"));

            // A fully capped future journey must show Skip, never disabled legacy offers.
            foreach (var reward in SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(preview.PlayerSpecies, null))
                while (preview.Progression.TryApplyFieldDataReward(reward)) { }
            ExecuteShellCommand(shell, "ContinueJourneyCommand");
            preview.PauseSimulation();
            for (var tick = 0; tick < 100; tick++) Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            ExecuteShellCommand(shell, "OpenJourneyMapCommand");
            ExecuteShellCommand(shell, "CloseJourneyMapCommand");
            Assert.That(preview.FieldDataSession.GetCurrentRoundRewardChoices().Choices, Is.Empty);
            for (var slot = 1; slot <= 3; slot++)
            {
                Assert.That(shellType.GetProperty("RewardOption" + slot + "Text").GetValue(shell), Is.Empty);
                Assert.That(shellType.GetProperty("RewardOption" + slot + "Visibility").GetValue(shell).ToString(), Is.EqualTo("Collapsed"));
            }
            Assert.That(shellType.GetProperty("FieldDataStatusText").GetValue(shell).ToString(), Does.Contain("No eligible mutations"));
            Assert.That(shellType.GetProperty("CanContinueWithoutUpgrade").GetValue(shell), Is.EqualTo(true));
            ExecuteShellCommand(shell, "ContinueWithoutUpgradeCommand");
            Assert.That(shellType.GetProperty("CanContinueJourney").GetValue(shell), Is.EqualTo(true));

        }

        [UnityTest]
        public IEnumerator FieldDataTestRouteCompletesThroughBothShopsOnFixedSeed()
        {
            const int testSeed = 10100;
            journeyProfileSave = ProfilePlayerPrefsBackup.Capture();
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<SpeciesSimulationPreview>();
            var profileSession = UnityEngine.Object.FindAnyObjectByType<Helper_ProfileSession>();
            if (profileSession == null)
                profileSession = new GameObject("Field Data success test profile").AddComponent<Helper_ProfileSession>();
            var shell = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .FirstOrDefault(component => component.GetType().FullName == "SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(profileSession, Is.Not.Null);
            Assert.That(shell, Is.Not.Null);
            profileSession.SetPlaytimeTracking(false);
            profileSession.CreateProfile("Field Data success boundary test");
            profileSession.AddCurrency(113);
            profileSession.UnlockFeature("field-data-success-preserved-feature");
            profileSession.UnlockSpecies(new SpeciesId("fox"));
            profileSession.UnlockGenome(new SpeciesId("fox"));
            var bankedProfileBefore = profileSession.Current;

            typeof(SpeciesSimulationPreview).GetField("randomizeSeedOnStart", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(preview, false);
            typeof(SpeciesSimulationPreview).GetField("seed", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(preview, testSeed);
            var authoredScenarioData = ConfigureAuthoredFieldDataScenario(preview);
            Assert.That(preview.StepInterval, Is.EqualTo(authoredScenarioData.StepInterval));

            var shellType = shell.GetType();
            ExecuteShellCommand(shell, "OpenJourneyMapCommand");
            ExecuteShellCommand(shell, "StartFieldDataTestJourneyCommand");
            ExecuteShellCommand(shell, "StartFieldDataTestJourneyCommand");
            Assert.That(preview.Run.Seed, Is.EqualTo(testSeed));
            var retainedRun = preview.Run;
            preview.PauseSimulation();

            const int roundsPerSimulation = FieldDataJourneyDefinition.RoundsPerSimulation;
            const int totalRounds = FieldDataJourneyDefinition.RoundsPerSimulation
                * FieldDataJourneyDefinition.SimulationCount;
            for (var round = 1; round <= totalRounds; round++)
            {
                for (var tick = 0; tick < FieldDataJourneyDefinition.RoundLengthTicks; tick++)
                {
                    if (preview.FieldDataSession.State == FieldDataJourneyFlowState.Failed)
                        Assert.Fail($"The approved test route seed {testSeed} failed at tick {preview.Run.Tick}.");
                    Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True,
                        $"The simulation should advance to route tick {round * FieldDataJourneyDefinition.RoundLengthTicks}.");
                }

                Assert.That(preview.Run.Tick, Is.EqualTo(round * FieldDataJourneyDefinition.RoundLengthTicks));
                AssertFieldDataMapRoundTrip(shell, preview);
                Assert.That(preview.Run, Is.SameAs(retainedRun));
                var settledAwardCount = preview.FieldDataSession.Ledger.Awards.Count;
                if (round == roundsPerSimulation || round == 2 * roundsPerSimulation)
                    Assert.That(preview.FieldDataSession.State, Is.EqualTo(FieldDataJourneyFlowState.Shop));
                else if (round < totalRounds)
                    Assert.That(preview.FieldDataSession.State, Is.EqualTo(FieldDataJourneyFlowState.RoundDecision));

                if (round == roundsPerSimulation || round == 2 * roundsPerSimulation)
                {
                    var shopVisit = preview.FieldDataSession.GetCurrentShopVisit();
                    Assert.That(shopVisit, Is.Not.Null);
                    Assert.That(preview.FieldDataSession.GetCurrentShopVisit(), Is.SameAs(shopVisit),
                        "Re-entering the same shop must retain its original offer set.");
                    var nodeChoices = preview.FieldDataSession.GetCurrentNodeRewardChoices();
                    Assert.That(nodeChoices, Is.Not.Null);
                    Assert.That(preview.FieldDataSession.GetCurrentNodeRewardChoices(), Is.SameAs(nodeChoices),
                        "Re-entering the node reward decision must retain its original choices.");
                    Assert.That(preview.FieldDataSession.Ledger.Awards.Count, Is.EqualTo(settledAwardCount),
                        "Reading a shop again must not settle another award.");
                }

                if (round < totalRounds)
                {
                    ExecuteShellCommand(shell, "ContinueWithoutUpgradeCommand");
                    ExecuteShellCommand(shell, "ContinueJourneyCommand");
                    if (round % roundsPerSimulation == 0)
                    {
                        var balanceBeforeMap = preview.FieldDataSession.Ledger.Balance;
                        var boundaryTick = preview.Run.Tick;
                        ExecuteShellCommand(shell, "ContinueJourneyCommand");
                        ExecuteShellCommand(shell, "CloseJourneyMapCommand");
                        Assert.That(shellType.GetProperty("JourneyMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
                        Assert.That(shellType.GetProperty("FieldDataShopVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Collapsed"));
                        ExecuteShellCommand(shell, "BuyFieldDataShopOffer1Command");
                        yield return null;
                        Assert.That(preview.Run.Tick, Is.EqualTo(boundaryTick));
                        Assert.That(preview.FieldDataSession.Ledger.Balance, Is.EqualTo(balanceBeforeMap));
                        EnterAvailableFieldDataNode(shell, round / roundsPerSimulation * 2 - 1);
                        Assert.That(shellType.GetProperty("FieldDataShopVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
                        AssertFieldDataMapRoundTrip(shell, preview);
                        ExecuteShellCommand(shell, "ContinueJourneyCommand");
                        yield return null;
                        Assert.That(preview.Run.Tick, Is.EqualTo(boundaryTick));
                        Assert.That(shellType.GetProperty("JourneyMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
                        EnterAvailableFieldDataNode(shell, round / roundsPerSimulation * 2);
                    }
                    var continuedTick = preview.Run.Tick;
                    var awardsAfterContinue = preview.FieldDataSession.Ledger.Awards.Count;
                    ExecuteShellCommand(shell, "ContinueJourneyCommand");
                    Assert.That(preview.Run.Tick, Is.EqualTo(continuedTick),
                        "A repeated Continue click while the simulation is already running must not advance it.");
                    Assert.That(preview.FieldDataSession.Ledger.Awards.Count, Is.EqualTo(awardsAfterContinue),
                        "A repeated Continue click must not duplicate a settled payout.");
                    preview.PauseSimulation();
                }
            }

            yield return null;
            Assert.That(preview.FieldDataSession.State, Is.EqualTo(FieldDataJourneyFlowState.Succeeded));
            Assert.That(shellType.GetProperty("ResultsMessageText")?.GetValue(shell)?.ToString(),
                Does.Contain("Field Data remains").And.Not.Contain("was discarded"));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.Run.Tick, Is.EqualTo(1800));
            Assert.That(shellType.GetProperty("FieldDataPopulationControlsVisibility")?.GetValue(shell)?.ToString(),
                Is.EqualTo("Collapsed"), "Terminal results must not offer population controls.");
            Assert.That(shellType.GetProperty("PlayNextSimulationText")?.GetValue(shell)?.ToString(),
                Is.EqualTo("RETURN TO MAP"));
            var completedRun = preview.Run;
            var completedAwards = preview.FieldDataSession.Ledger.Awards.Count;
            var successfulWallet = preview.FieldDataSession.Ledger.Balance;
            ExecuteShellCommand(shell, "ContinueJourneyCommand");
            Assert.That(preview.FieldDataSession.State, Is.EqualTo(FieldDataJourneyFlowState.Succeeded),
                "A stale Continue click on successful results must not reopen or advance the completed journey.");
            Assert.That(preview.Run, Is.SameAs(completedRun));
            Assert.That(preview.FieldDataSession.Ledger.Awards.Count, Is.EqualTo(completedAwards));
            Assert.That(preview.FieldDataSession.Ledger.Balance, Is.EqualTo(successfulWallet));
            var permanentCurrencyBeforeRestart = preview.Progression.Currency;
            var successfulSession = preview.FieldDataSession;
            var awardsAtExit = successfulSession.Ledger.Awards.Count;
            ExecuteShellCommand(shell, "PlayNextSimulationCommand");
            Assert.That(shellType.GetProperty("JourneyMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
            Assert.That(preview.Run, Is.SameAs(completedRun));
            Assert.That(successfulSession.Ledger.Balance, Is.EqualTo(successfulWallet));
            Assert.That(shellType.GetProperty("JourneyNewRunVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
            ExecuteShellCommand(shell, "StartNewJourneyCommand");
            Assert.That(successfulSession.Ledger.Balance, Is.Zero);
            Assert.That(successfulSession.Ledger.Awards.Count, Is.EqualTo(awardsAtExit));
            EnterAvailableFieldDataNode(shell, 0);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.Not.SameAs(completedRun));
            Assert.That(preview.FieldDataSession.Ledger.Balance, Is.Zero);
            Assert.That(preview.Progression.Currency, Is.EqualTo(permanentCurrencyBeforeRestart));
            AssertBankedProfilePreserved(bankedProfileBefore, profileSession.Current);
        }

        [UnityTest]
        public IEnumerator FieldDataNaturalFailureForfeitsJourneyWalletAndRestartStartsAtZero()
        {
            const int failingSeed = 10108;
            journeyProfileSave = ProfilePlayerPrefsBackup.Capture();
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<SpeciesSimulationPreview>();
            var profileSession = UnityEngine.Object.FindAnyObjectByType<Helper_ProfileSession>();
            if (profileSession == null)
                profileSession = new GameObject("Field Data failure test profile").AddComponent<Helper_ProfileSession>();
            var shell = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .FirstOrDefault(component => component.GetType().FullName == "SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(profileSession, Is.Not.Null);
            Assert.That(shell, Is.Not.Null);
            profileSession.SetPlaytimeTracking(false);
            profileSession.CreateProfile("Field Data failure boundary test");
            profileSession.AddCurrency(73);
            profileSession.UnlockFeature("field-data-test-preserved-feature");
            profileSession.UnlockSpecies(new SpeciesId("fox"));
            profileSession.UnlockGenome(new SpeciesId("fox"));
            var bankedProfileBefore = profileSession.Current;
            typeof(SpeciesSimulationPreview).GetField("randomizeSeedOnStart", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(preview, false);
            typeof(SpeciesSimulationPreview).GetField("seed", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(preview, failingSeed);
            ConfigureAuthoredFieldDataScenario(preview);

            ExecuteShellCommand(shell, "OpenJourneyMapCommand");
            ExecuteShellCommand(shell, "StartFieldDataTestJourneyCommand");
            ExecuteShellCommand(shell, "StartFieldDataTestJourneyCommand");
            preview.PauseSimulation();
            for (var tick = 0; tick < 100; tick++)
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            ExecuteShellCommand(shell, "ContinueWithoutUpgradeCommand");
            ExecuteShellCommand(shell, "ContinueJourneyCommand");
            preview.PauseSimulation();
            while (preview.FieldDataSession.State != FieldDataJourneyFlowState.Failed
                && preview.Run.Tick < 600)
            {
                if (preview.FieldDataSession.State == FieldDataJourneyFlowState.RoundDecision
                    || preview.FieldDataSession.State == FieldDataJourneyFlowState.Shop)
                {
                    ExecuteShellCommand(shell, "ContinueWithoutUpgradeCommand");
                    ExecuteShellCommand(shell, "ContinueJourneyCommand");
                    preview.PauseSimulation();
                    continue;
                }

                var advanced = preview.AdvanceOneTickWhilePaused();
                if (!advanced && preview.FieldDataSession.State != FieldDataJourneyFlowState.Failed)
                    Assert.Fail("The simulation stopped before the expected natural-extinction failure.");
            }

            var failedSession = preview.FieldDataSession;
            Assert.That(failedSession.State, Is.EqualTo(FieldDataJourneyFlowState.Failed),
                "The approved failing seed should exercise natural extinction before the next node clear.");
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(failedSession.FieldDataLostOnFailure, Is.GreaterThan(0));
            Assert.That(shell.GetType().GetProperty("PlayNextSimulationText")?.GetValue(shell)?.ToString(),
                Is.EqualTo("START NEW JOURNEY"));
            Assert.That(failedSession.Ledger.Balance, Is.Zero);
            Assert.That(failedSession.Ledger.Awards.Any(award => award.GlobalRound == 3), Is.False,
                "Extinction during round 3 must not settle that round's population payout.");
            var permanentCurrencyBeforeRestart = preview.Progression.Currency;
            var failedRun = preview.Run;
            AssertBankedProfilePreserved(bankedProfileBefore, profileSession.Current);

            ExecuteShellCommand(shell, "PlayNextSimulationCommand");
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running),
                "The results screen's Start New Journey action should close the failed session and start a fresh route.");
            Assert.That(preview.FieldDataSession, Is.Not.SameAs(failedSession));
            Assert.That(preview.FieldDataSession.Ledger.Balance, Is.Zero);
            Assert.That(preview.Progression.Currency, Is.EqualTo(permanentCurrencyBeforeRestart));
            Assert.That(preview.Run, Is.Not.SameAs(failedRun));
            AssertBankedProfilePreserved(bankedProfileBefore, profileSession.Current);
        }

        [UnityTest]
        public IEnumerator FieldDataManualAbandonmentUsesEndConfirmationAndPreservesBankedProfileState()
        {
            journeyProfileSave = ProfilePlayerPrefsBackup.Capture();
            yield return SceneManager.LoadSceneAsync("GalapagOSDesktopTest");
            yield return null;
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<SpeciesSimulationPreview>();
            var profileSession = UnityEngine.Object.FindAnyObjectByType<Helper_ProfileSession>();
            if (profileSession == null)
                profileSession = new GameObject("Field Data abandonment test profile").AddComponent<Helper_ProfileSession>();
            var shell = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .FirstOrDefault(component => component.GetType().FullName == "SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(profileSession, Is.Not.Null);
            Assert.That(shell, Is.Not.Null);
            profileSession.SetPlaytimeTracking(false);
            profileSession.CreateProfile("Field Data abandonment boundary test");
            profileSession.AddCurrency(91);
            profileSession.UnlockFeature("field-data-abandonment-preserved-feature");
            profileSession.UnlockSpecies(new SpeciesId("fox"));
            profileSession.UnlockGenome(new SpeciesId("fox"));
            var bankedProfileBefore = profileSession.Current;
            typeof(SpeciesSimulationPreview).GetField("randomizeSeedOnStart", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(preview, false);
            typeof(SpeciesSimulationPreview).GetField("seed", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(preview, 10100);
            ConfigureAuthoredFieldDataScenario(preview);

            ExecuteShellCommand(shell, "OpenJourneyMapCommand");
            ExecuteShellCommand(shell, "StartFieldDataTestJourneyCommand");
            ExecuteShellCommand(shell, "StartFieldDataTestJourneyCommand");
            preview.PauseSimulation();
            for (var tick = 0; tick < 100; tick++)
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            ExecuteShellCommand(shell, "ContinueWithoutUpgradeCommand");
            ExecuteShellCommand(shell, "ContinueJourneyCommand");
            preview.PauseSimulation();
            Assert.That(preview.FieldDataSession.Ledger.Balance, Is.GreaterThan(0));

            ExecuteShellCommand(shell, "EndCommand");
            Assert.That(shell.GetType().GetProperty("EndConfirmationVisibility")?.GetValue(shell)?.ToString(),
                Is.EqualTo("Visible"));
            ExecuteShellCommand(shell, "ConfirmEndCommand");

            var abandonedSession = preview.FieldDataSession;
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(abandonedSession.State, Is.EqualTo(FieldDataJourneyFlowState.Failed));
            Assert.That(abandonedSession.FieldDataLostOnFailure, Is.GreaterThan(0));
            Assert.That(abandonedSession.Ledger.Balance, Is.Zero);
            AssertBankedProfilePreserved(bankedProfileBefore, profileSession.Current);

            ExecuteShellCommand(shell, "PlayNextSimulationCommand");
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.FieldDataSession.Ledger.Balance, Is.Zero);
            AssertBankedProfilePreserved(bankedProfileBefore, profileSession.Current);
        }

        static void AssertBankedProfilePreserved(ProfileSessionSnapshot expected, ProfileSessionSnapshot actual)
        {
            Assert.That(actual.Currency, Is.EqualTo(expected.Currency));
            Assert.That(actual.UnlockedSpeciesIds, Is.EqualTo(expected.UnlockedSpeciesIds));
            Assert.That(actual.UnlockedGenomeSpeciesIds, Is.EqualTo(expected.UnlockedGenomeSpeciesIds));
            Assert.That(actual.UnlockedFeatureIds, Is.EqualTo(expected.UnlockedFeatureIds));
            Assert.That(actual.CreateGenomeSnapshot().Fingerprint,
                Is.EqualTo(expected.CreateGenomeSnapshot().Fingerprint));
        }

        static void AssertFieldDataMapRoundTrip(MonoBehaviour shell, SpeciesSimulationPreview preview)
        {
            var run = preview.Run;
            var session = preview.FieldDataSession;
            var state = preview.State;
            var tick = run.Tick;
            var balance = session.Ledger.Balance;
            var shellType = shell.GetType();
            Assert.That(shellType.GetProperty("JourneyMapHudButtonVisibility")?.GetValue(shell)?.ToString(),
                Is.EqualTo("Visible"));
            ExecuteShellCommand(shell, "OpenJourneyMapCommand");
            Assert.That(shellType.GetProperty("JourneyMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
            ExecuteShellCommand(shell, "CloseJourneyMapCommand");
            Assert.That(shellType.GetProperty("JourneyMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Collapsed"));
            Assert.That(preview.State, Is.EqualTo(state));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(preview.FieldDataSession, Is.SameAs(session));
            Assert.That(run.Tick, Is.EqualTo(tick));
            Assert.That(session.Ledger.Balance, Is.EqualTo(balance));
        }

        internal static CellularSimData ConfigureAuthoredFieldDataScenario(SpeciesSimulationPreview preview)
        {
            var authoredScenarioData = preview.SelectedScenario.CreateRuntimeData();
            typeof(SpeciesSimulationPreview).GetField("width", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(preview, authoredScenarioData.Width);
            typeof(SpeciesSimulationPreview).GetField("height", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(preview, authoredScenarioData.Height);
            typeof(SpeciesSimulationPreview).GetField("stepInterval", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(preview, authoredScenarioData.StepInterval);
            typeof(SpeciesSimulationPreview).GetField("wrapEdges", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(preview, authoredScenarioData.WrapEdges);
            typeof(SpeciesSimulationPreview).GetField("startingPopulationOverrideEnabled", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(preview, false);
            return authoredScenarioData;
        }

        internal static void EnterAvailableFieldDataNode(MonoBehaviour shell, int index)
        {
            var shellType = shell.GetType();
            Assert.That(shellType.GetProperty("JourneyMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Visible"));
            var nodes = (Array)shellType.GetProperty("FieldDataRouteNodes").GetValue(shell);
            for (var i = 0; i < nodes.Length; i++)
                Assert.That(nodes.GetValue(i).GetType().GetProperty("CanEnter").GetValue(nodes.GetValue(i)), Is.EqualTo(i == index));
            var node = nodes.GetValue(index);
            var enter = node.GetType().GetProperty("EnterCommand").GetValue(node);
            enter.GetType().GetMethod("Execute").Invoke(enter, new object[] { null });
            Assert.That(shellType.GetProperty("JourneyMapVisibility")?.GetValue(shell)?.ToString(), Is.EqualTo("Collapsed"));
        }

        static void ExecuteShellCommand(MonoBehaviour shell, string commandName)
        {
            var command = shell.GetType().GetProperty(commandName)?.GetValue(shell);
            Assert.That(command, Is.Not.Null, commandName + " must be bound by the simulation shell.");
            command.GetType().GetMethod("Execute")?.Invoke(command, new object[] { null });
        }

        static string GetChosenIncomeItemName(object item)
        {
            var label = item.GetType().GetProperty("Label")?.GetValue(item)?.ToString() ?? string.Empty;
            var separator = label.IndexOf(". ", StringComparison.Ordinal);
            return separator < 0 ? label : label.Substring(separator + 2);
        }

        [UnityTest]
        public IEnumerator SimulationZoomCommandsChangeTheLiveBoardScale()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var camera = GameObject.Find("Prototype Camera");
            var viewModel = camera?.GetComponent("SaltyGame.VM_SimulationShell");
            var host = GameObject.Find("Cellular Automata Prototype")
                ?.GetComponent("SaltyGame.SpeciesSimulationNoesisHost");
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(host, Is.Not.Null);

            var viewModelType = viewModel.GetType();
            var boardField = host.GetType().GetField("simulationBoard", BindingFlags.Instance | BindingFlags.NonPublic);
            var board = boardField?.GetValue(host);
            Assert.That(board, Is.Not.Null);

            var zoomInCommand = viewModelType.GetProperty("ZoomInCommand")?.GetValue(viewModel);
            zoomInCommand?.GetType().GetMethod("Execute")?.Invoke(zoomInCommand, new object[] { null });
            yield return null;

            Assert.That(viewModelType.GetProperty("BoardZoom")?.GetValue(viewModel), Is.EqualTo(1.25f));
            Assert.That(board.GetType().GetProperty("Zoom")?.GetValue(board), Is.EqualTo(1.25f));

            var resetZoomCommand = viewModelType.GetProperty("ResetZoomCommand")?.GetValue(viewModel);
            resetZoomCommand?.GetType().GetMethod("Execute")?.Invoke(resetZoomCommand, new object[] { null });
            yield return null;

            Assert.That(viewModelType.GetProperty("BoardZoom")?.GetValue(viewModel), Is.EqualTo(1f));
            Assert.That(board.GetType().GetProperty("Zoom")?.GetValue(board), Is.EqualTo(1f));
        }

        [UnityTest]
        public IEnumerator CellularPrototypeInitializesEveryAuthoredAnimalSprite()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                Assert.Ignore("Animal sprite initialization requires a graphics-capable Unity player.");
            }

            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            yield return null;

            var camera = GameObject.Find("Prototype Camera");
            var viewModel = camera?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null, "CellularAutomataPrototype must initialize VM_SimulationShell.");

            var sprites = viewModel.GetType()
                .GetField("animalSprites", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(viewModel) as Array;
            Assert.That(sprites, Is.Not.Null, "The animal atlas must produce presentation sprites at runtime.");
            Assert.That(sprites.Length, Is.EqualTo(8), "The authored animal roster contains eight presentation slots.");

            for (var index = 0; index < sprites.Length; index++)
            {
                Assert.That(sprites.GetValue(index), Is.Not.Null, $"Animal presentation slot {index} must be initialized.");
            }
        }

        [UnityTest]
        public IEnumerator TerrainPreservesStraightBoundariesIslandsAndHoles()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                Assert.Ignore("Terrain topology regression requires a graphics device.");
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            yield return null;
            UnityEngine.Object.FindAnyObjectByType<SpeciesSimulationPreview>().PauseSimulation();
            var host = GameObject.Find("Cellular Automata Prototype").GetComponent("SaltyGame.SpeciesSimulationNoesisHost");
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var board = (Noesis.FrameworkElement)host.GetType().GetField("simulationBoard", flags).GetValue(host);
            var boardType = board.GetType();
            var previousSnapshot = boardType.GetField("snapshot", flags).GetValue(board);
            var previousFlourishes = boardType.GetField("forestEdgeFlourishes", flags).GetValue(board);
            var camera = GameObject.Find("Prototype Camera").GetComponent<Camera>();
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var target = new RenderTexture(Screen.width, Screen.height, 24);
            var capture = new Texture2D(target.width, target.height, TextureFormat.RGBA32, false);
            try
            {
                boardType.GetMethod("SetForestEdgeFlourishes").Invoke(board, new object[] { null });
                for (var pattern = 0; pattern < 3; pattern++)
                {
                    var grid = new Grid<SpeciesCell>(8, 8, (x, y) =>
                    {
                        var grass = pattern == 0 ? x < 4 : pattern == 1 ? x == 3 && y == 3 : x != 3 || y != 3;
                        return grass ? SpeciesCell.Grass(1f) : SpeciesCell.Empty;
                    });
                    var run = new SimulationRunState(grid, SpeciesIds.Herbivore, seed: 17, durationSeconds: 1f);
                    var snapshot = SimulationBoardSnapshot.Create(run, SpeciesRuleDefaults.Create(), SpeciesIds.Herbivore);
                    boardType.GetMethod("SetSnapshot").Invoke(board, new object[] { snapshot });
                    yield return null;
                    yield return null;
                    yield return new WaitForEndOfFrame();
                    camera.targetTexture = target;
                    camera.Render();
                    RenderTexture.active = target;
                    capture.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
                    capture.Apply();
                    var evidence = System.IO.Path.Combine(Application.dataPath, "../artifacts/unity-tests/terrain-topology");
                    System.IO.Directory.CreateDirectory(evidence);
                    System.IO.File.WriteAllBytes(System.IO.Path.Combine(evidence, $"pattern-{pattern}.png"), capture.EncodeToPNG());
                    var layout = new object[] { board.ActualWidth, board.ActualHeight, 0f, 0f, 0f };
                    boardType.GetMethod("GetBoardLayout", flags).Invoke(board, layout);
                    for (var y = 2; y <= 5; y++)
                    for (var x = 2; x <= 5; x++)
                    {
                        var point = board.PointToScreen(new Noesis.Point(
                            (float)layout[3] + (x + 0.5f) * (float)layout[2],
                            (float)layout[4] + (snapshot.Height - y - 0.5f) * (float)layout[2]));
                        var color = capture.GetPixel((int)point.X, target.height - 1 - (int)point.Y);
                        Assert.That(color.g > color.r + 5f / 255f, Is.EqualTo(grid.GetCell(x, y).TerrainId == TerrainIds.Grass),
                            $"Terrain center ({x},{y}) must retain its terrain in pattern {pattern}.");
                    }
                }
            }
            finally
            {
                boardType.GetMethod("SetSnapshot").Invoke(board, new[] { previousSnapshot });
                boardType.GetMethod("SetForestEdgeFlourishes").Invoke(board, new[] { previousFlourishes });
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                UnityEngine.Object.DestroyImmediate(capture);
                UnityEngine.Object.DestroyImmediate(target);
            }
        }

        [UnityTest]
        public IEnumerator TerrainHasNoSeamsAtFractionalZoomPanAndUiScale()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                Assert.Ignore("Terrain seam regression requires a graphics device.");

            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            yield return null;
            UnityEngine.Object.FindAnyObjectByType<SpeciesSimulationPreview>().PauseSimulation();
            var host = GameObject.Find("Cellular Automata Prototype")
                .GetComponent("SaltyGame.SpeciesSimulationNoesisHost");
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var board = (Noesis.FrameworkElement)host.GetType().GetField("simulationBoard", flags).GetValue(host);
            var boardType = board.GetType();
            var camera = GameObject.Find("Prototype Camera").GetComponent<Camera>();
            var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            var target = new RenderTexture(Screen.width, Screen.height, 24);
            var capture = new Texture2D(target.width, target.height, TextureFormat.RGBA32, false);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var parent = (Noesis.FrameworkElement)board.Parent;
            var previousTransform = parent.RenderTransform;
            var previousSprites = new[] { "animalSprites", "grassTerrainTiles", "desertTerrainTiles" }
                .Select(name => boardType.GetField(name, flags).GetValue(board)).ToArray();
            var previousFlourishes = boardType.GetField("forestEdgeFlourishes", flags).GetValue(board);
            try
            {
                texture.SetPixels32(Enumerable.Repeat(new Color32(64, 192, 64, 255), 4096).ToArray());
                texture.Apply();
                var tile = new Noesis.CroppedBitmap(new Noesis.TextureSource(texture), new Noesis.Int32Rect(0, 0, 64, 64));
                var tiles = Enumerable.Repeat(tile, 256).ToArray();
                boardType.GetMethod("SetSpriteVisuals").Invoke(board, new object[] { null, tiles, null });
                boardType.GetMethod("SetForestEdgeFlourishes").Invoke(board, new object[] { null });
                foreach (var zoom in new[] { 0.75f, 1.13704991f, 2f, 4f })
                {
                    boardType.GetProperty("Zoom").SetValue(board, zoom);
                    boardType.GetField("panOffsetX", flags).SetValue(board, -3.25f);
                    boardType.GetField("panOffsetY", flags).SetValue(board, 4.75f);
                    boardType.GetMethod("ApplyPanZoomTransform", flags).Invoke(board, null);
                    parent.RenderTransform = new Noesis.ScaleTransform(0.83f, 0.91f);
                    yield return null;
                    yield return null;
                    yield return new WaitForEndOfFrame();

                    camera.targetTexture = target;
                    camera.Render();
                    RenderTexture.active = target;
                    capture.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
                    capture.Apply();
                    var a = board.PointToScreen(new Noesis.Point(board.ActualWidth * 0.3f, board.ActualHeight * 0.3f));
                    var b = board.PointToScreen(new Noesis.Point(board.ActualWidth * 0.7f, board.ActualHeight * 0.7f));
                    var pixels = capture.GetPixels32();
                    var snapshot = (SimulationBoardSnapshot)boardType.GetField("snapshot", flags).GetValue(board);
                    var layout = new object[] { board.ActualWidth, board.ActualHeight, 0f, 0f, 0f };
                    boardType.GetMethod("GetBoardLayout", flags).Invoke(board, layout);
                    var edgeMethod = boardType.GetMethod("GetPixelAlignedEdges", flags);
                    var columns = (float[])edgeMethod.Invoke(board, new object[] { layout[3], layout[2], snapshot.Width, true });
                    var rows = (float[])edgeMethod.Invoke(board, new object[] { layout[4], layout[2], snapshot.Height, false });
                    var edgeXs = new HashSet<int>();
                    var edgeYs = new HashSet<int>();
                    var detailAreas = new List<Noesis.Rect>();
                    for (var cellY = 0; cellY < snapshot.Height; cellY++)
                    for (var cellX = 0; cellX < snapshot.Width; cellX++)
                    {
                        var cell = snapshot.GetCell(cellX, cellY);
                        var hash = unchecked((uint)cellX * 73856093u ^ (uint)cellY * 19349663u);
                        if (!((cell.IsTerrainResource && hash % 19 == 0) || (!cell.IsTerrainResource && hash % 47 == 0))) continue;
                        var centerX = (float)layout[3] + (cellX + 0.5f) * (float)layout[2];
                        var centerY = (float)layout[4] + (snapshot.Height - cellY - 0.5f) * (float)layout[2];
                        var radius = Math.Max(3f, (float)layout[2] * 0.45f);
                        var p = board.PointToScreen(new Noesis.Point(centerX - radius, centerY - radius));
                        var q = board.PointToScreen(new Noesis.Point(centerX + radius, centerY + radius));
                        detailAreas.Add(new Noesis.Rect(p.X, p.Y, q.X - p.X, q.Y - p.Y));
                    }
                    var terrainColumns = (float[])edgeMethod.Invoke(board, new object[] { (float)layout[3] - (float)layout[2] * 0.5f, layout[2], snapshot.Width + 1, true });
                    var terrainRows = (float[])edgeMethod.Invoke(board, new object[] { (float)layout[4] - (float)layout[2] * 0.5f, layout[2], snapshot.Height + 1, false });
                    foreach (var edge in columns.Concat(terrainColumns))
                    {
                        var screenX = board.PointToScreen(new Noesis.Point(edge, 0f)).X;
                        edgeXs.Add((int)Math.Floor(screenX));
                        edgeXs.Add((int)Math.Ceiling(screenX));
                    }
                    foreach (var edge in rows.Concat(terrainRows))
                    {
                        var screenY = board.PointToScreen(new Noesis.Point(0f, edge)).Y;
                        edgeYs.Add((int)Math.Floor(screenY));
                        edgeYs.Add((int)Math.Ceiling(screenY));
                    }
                    var corner = board.PointToScreen(new Noesis.Point(columns[snapshot.Width / 2], rows[snapshot.Height / 2]));
                    var sample = pixels[(target.height - 1 - (int)corner.Y - 2) * target.width + (int)corner.X + 2];
                    Assert.That(sample.g, Is.GreaterThan(sample.r + 20), "Probe must render opaque green terrain.");
                    for (var y = (int)Math.Ceiling(a.Y); y < (int)b.Y; y++)
                    for (var x = (int)Math.Ceiling(a.X); x < (int)b.X; x++)
                    {
                        // Ground details cross the shifted terrain edges; exclude their known occupied areas.
                        if (!edgeXs.Contains(x) && !edgeYs.Contains(y)) continue;
                        if (detailAreas.Any(area => area.Contains(new Noesis.Point(x, y)))) continue;
                        var pixel = pixels[(target.height - 1 - y) * target.width + x];
                        if (Math.Abs(pixel.r - sample.r) + Math.Abs(pixel.g - sample.g) + Math.Abs(pixel.b - sample.b) > 6)
                        {
                            var evidence = System.IO.Path.Combine(Application.dataPath, "../artifacts/unity-tests/terrain-seams");
                            System.IO.Directory.CreateDirectory(evidence);
                            System.IO.File.WriteAllBytes(System.IO.Path.Combine(evidence, "regression-failure.png"), capture.EncodeToPNG());
                        }
                        Assert.That(Math.Abs(pixel.r - sample.r) + Math.Abs(pixel.g - sample.g) + Math.Abs(pixel.b - sample.b),
                            Is.LessThanOrEqualTo(6), $"Terrain seam at ({x},{y}), zoom {zoom}.");
                    }
                    camera.targetTexture = previousTarget;
                }
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                parent.RenderTransform = previousTransform;
                boardType.GetMethod("SetSpriteVisuals").Invoke(board, previousSprites);
                boardType.GetMethod("SetForestEdgeFlourishes").Invoke(board, new[] { previousFlourishes });
                UnityEngine.Object.DestroyImmediate(texture);
                UnityEngine.Object.DestroyImmediate(target);
                UnityEngine.Object.DestroyImmediate(capture);
            }
        }

        [UnityTest]
        public IEnumerator CellularPrototypeInitializesEveryAuthoredGrassTerrainSprite()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                Assert.Ignore("Terrain sprite initialization requires a graphics-capable Unity player.");
            }

            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            yield return null;

            var camera = GameObject.Find("Prototype Camera");
            var viewModel = camera?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null, "CellularAutomataPrototype must initialize VM_SimulationShell.");

            var grassTiles = viewModel.GetType()
                .GetField("grassTerrainTiles", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(viewModel) as Array;
            Assert.That(grassTiles, Is.Not.Null, "The terrain atlas must produce Grass presentation sprites at runtime.");
            Assert.That(grassTiles.Length, Is.EqualTo(256), "Grass presentation uses direct eight-bit mask slots.");

            foreach (var mask in TerrainTileResolver.AllValidMasks)
            {
                Assert.That(grassTiles.GetValue(mask), Is.Not.Null, $"Grass mask {mask:D3} must be initialized.");
            }

            var flourishes = viewModel.GetType()
                .GetField("forestEdgeFlourishes", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(viewModel) as Array;
            Assert.That(flourishes, Is.Not.Null, "ForestEdge must initialize the authored flourish atlas crops.");
            Assert.That(flourishes.Length, Is.EqualTo(ForestEdgeFlourishResolver.SpriteNames.Count));
            var flourishRects = new HashSet<Noesis.Int32Rect>();
            for (var index = 0; index < flourishes.Length; index++)
            {
                Assert.That(flourishes.GetValue(index), Is.Not.Null,
                    $"Flourish '{ForestEdgeFlourishResolver.SpriteNames[index]}' must be initialized.");
                Assert.That(flourishRects.Add(((Noesis.CroppedBitmap)flourishes.GetValue(index)).SourceRect), Is.True,
                    $"Flourish '{ForestEdgeFlourishResolver.SpriteNames[index]}' must resolve its own atlas crop.");
            }

            var boardViewModel = camera.GetComponent("SaltyGame.VM_SimulationBoard");
            Assert.That(boardViewModel, Is.Not.Null);
            var snapshot = boardViewModel.GetType()
                .GetProperty("Snapshot", BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(boardViewModel) as SimulationBoardSnapshot;
            Assert.That(snapshot, Is.Not.Null, "The prototype scene must provide a terrain snapshot.");
            var grassCellCount = 0;
            foreach (var cell in snapshot.Cells)
            {
                if (cell.TerrainId == TerrainIds.Grass)
                {
                    grassCellCount++;
                }
            }

            Assert.That(grassCellCount, Is.GreaterThan(0), "ForestEdge must place Grass cells for the renderer to tile.");

            var desertTiles = viewModel.GetType()
                .GetField("desertTerrainTiles", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(viewModel) as Array;
            Assert.That(desertTiles, Is.Not.Null, "Missing Desert art must not disable the terrain atlas family slots.");
        }

        [UnityTest]
        public IEnumerator SlashlineIsAvailableWithoutDeveloperModeWhileUpgradeDiagnosticsStayHidden()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);
            runtime.SpeciesPreview.StopSimulation();
            Assert.That(runtime.SpeciesPreview.BevExperimentalFeaturesEnabled, Is.True);
            Assert.That(runtime.SpeciesPreview.FoxAttackCooldownTicks, Is.EqualTo(0));

            var continuousSettingsApplied = runtime.SpeciesPreview.TryApplyContinuousPhases(
                enabled: false,
                phaseLengthValue: string.Empty,
                out var continuousMessage);
            Assert.That(continuousSettingsApplied, Is.True, continuousMessage);

            var applied = runtime.SpeciesPreview.TryApplyExperimentalFeatures(
                true,
                "2",
                out var message);

            Assert.That(applied, Is.True, message);
            Assert.That(runtime.SpeciesPreview.BevExperimentalFeaturesEnabled, Is.True);
            Assert.That(runtime.SpeciesPreview.FoxAttackCooldownTicks, Is.EqualTo(2));
            StringAssert.Contains("species stat lines", message);

            var settingsApplied = runtime.SpeciesPreview.TryApplyGlobalSettings(
                "36",
                "20",
                runtime.SpeciesPreview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "0.05",
                "0.01",
                runtime.SpeciesPreview.PlantProbability.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.HerbivoreProbability.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.CarnivoreProbability.ToString(CultureInfo.InvariantCulture),
                randomizeSeed: false,
                out var settingsMessage);
            Assert.That(settingsApplied, Is.True, settingsMessage);

            runtime.SpeciesPreview.StartSimulation();
            var timeout = Time.realtimeSinceStartup + 5f;
            while (runtime.SpeciesPreview.State != SpeciesPreviewState.Rewards
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(runtime.SpeciesPreview.State, Is.EqualTo(SpeciesPreviewState.Rewards));
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            var summary = viewModel.GetType()
                .GetProperty("ExperimentalHerbivoreStatLineSummary")
                ?.GetValue(viewModel) as string;
            var summaryVisibility = viewModel.GetType()
                .GetProperty("ExperimentalHerbivoreStatLineSummaryVisibility")
                ?.GetValue(viewModel)
                ?.ToString();
            Assert.That(summaryVisibility, Is.EqualTo("Visible"));
            StringAssert.Contains("ADD:", summary);
            StringAssert.Contains("FPO:", summary);
            Assert.That(viewModel.GetType().GetProperty("ExperimentalUpgradeCountVisibility")
                ?.GetValue(viewModel)?.ToString(), Is.EqualTo("Collapsed"));
            var showNotesCommand = viewModel.GetType().GetProperty("ShowFieldNotesCommand")?.GetValue(viewModel);
            showNotesCommand?.GetType().GetMethod("Execute")?.Invoke(showNotesCommand, new object[] { null });
            Assert.That(viewModel.GetType().GetProperty("FieldNotesVisibility")
                ?.GetValue(viewModel)?.ToString(), Is.EqualTo("Visible"));
            var closeNotesCommand = viewModel.GetType().GetProperty("CloseFieldNotesCommand")?.GetValue(viewModel);
            closeNotesCommand?.GetType().GetMethod("Execute")?.Invoke(closeNotesCommand, new object[] { null });
            Assert.That(viewModel.GetType().GetProperty("FieldNotesVisibility")
                ?.GetValue(viewModel)?.ToString(), Is.EqualTo("Collapsed"));

            viewModel.GetType().GetProperty("DeveloperMode")?.SetValue(viewModel, true);
            summary = viewModel.GetType()
                .GetProperty("ExperimentalHerbivoreStatLineSummary")
                ?.GetValue(viewModel) as string;
            summaryVisibility = viewModel.GetType()
                .GetProperty("ExperimentalHerbivoreStatLineSummaryVisibility")
                ?.GetValue(viewModel)
                ?.ToString();
            var expectedStartingPopulation = runtime.SpeciesPreview.Run.PopulationHistory[0]
                .GetCount(runtime.SpeciesPreview.PlayerSpecies);
            StringAssert.Contains($"SPO: {expectedStartingPopulation}", summary);
            StringAssert.Contains("SPO:", summary);
            StringAssert.Contains("HPS:", summary);
            StringAssert.Contains("EHS:", summary);
            StringAssert.Contains("eAVI:", summary);
            StringAssert.Contains("predAVG:", summary);
            StringAssert.Contains("APS:", summary);
            Assert.That(summaryVisibility, Is.EqualTo("Visible"));
            Assert.That(runtime.SpeciesPreview.RewardOptionCount, Is.EqualTo(2));
            Assert.That(runtime.SpeciesPreview.GetRewardOptionId(0), Is.Not.Empty);
            Assert.That(
                runtime.SpeciesPreview.GetRewardOptionId(1),
                Is.Not.EqualTo(runtime.SpeciesPreview.GetRewardOptionId(0)));
            var thirdRewardVisibility = viewModel.GetType()
                .GetProperty("RewardOption3Visibility")
                ?.GetValue(viewModel)
                ?.ToString();
            Assert.That(thirdRewardVisibility, Is.EqualTo("Collapsed"));

            runtime.SpeciesPreview.ContinueWithoutUpgrade();
            yield return null;
        }

        [UnityTest]
        public IEnumerator DeveloperSettingsCanSetExactStartingPopulations()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            var viewModelType = viewModel.GetType();
            viewModelType.GetProperty("DeveloperMode")?.SetValue(viewModel, true);
            viewModelType.GetProperty("PlantStartingPopulationText")?.SetValue(viewModel, "3");
            viewModelType.GetProperty("HerbivoreStartingPopulationText")?.SetValue(viewModel, "2");
            viewModelType.GetProperty("CarnivoreStartingPopulationText")?.SetValue(viewModel, "1");
            var applySettingsCommand = viewModelType.GetProperty("ApplySettingsCommand")?.GetValue(viewModel);
            applySettingsCommand?.GetType().GetMethod("Execute")?.Invoke(applySettingsCommand, new object[] { null });

            Assert.That(preview.PlantStartingPopulation, Is.EqualTo(3));
            Assert.That(preview.HerbivoreStartingPopulation, Is.EqualTo(2));
            Assert.That(preview.CarnivoreStartingPopulation, Is.EqualTo(1));
            var settingsMessage = viewModelType.GetProperty("SettingsMessage")?.GetValue(viewModel) as string;
            StringAssert.Contains("Starting populations: plant 3, herbivore 2, carnivore 1", settingsMessage);

            preview.ResetToStart();
            preview.StartSimulation();
            var openingPopulation = preview.Run.PopulationHistory[0];
            Assert.That(openingPopulation.GetCount(SpeciesIds.Plant), Is.EqualTo(3));
            Assert.That(openingPopulation.GetCount(FindSpeciesId(preview, SpeciesRole.Herbivore)), Is.EqualTo(2));
            Assert.That(openingPopulation.GetCount(FindSpeciesId(preview, SpeciesRole.Carnivore)), Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator GardenersPathCanBeChosenAcrossFivePhaseDecisions()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            preview.ResetToStart();
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
            Assert.That(preview.TrySetPlayerSpecies("hare", out var message), Is.True, message);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations("36", "20", "1", "720", "0", "6", "0.1",
                "0", "0", "0", false, "400", "25", "15", out message), Is.True, message);
            Assert.That(preview.TryApplyExperimentalFeatures(true, false, "0", out message), Is.True, message);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out message), Is.True, message);
            preview.StartSimulation();
            preview.PauseSimulation();
            var baseline = preview.ActiveSpeciesRules[preview.PlayerSpecies];
            var path = new[] { "efficient-digestion", "seed-dispersal", "efficient-digestion", "seed-dispersal", "efficient-digestion" };
            foreach (var desired in path)
            {
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                var index = -1;
                for (var option = 0; option < preview.RewardOptionCount; option++)
                    if (preview.GetRewardOptionId(option) == desired) index = option;
                Assert.That(index, Is.GreaterThanOrEqualTo(0), desired);
                var tick = preview.Run.Tick;
                var currency = preview.Progression.Currency;
                Assert.That(preview.PurchaseReward(index), Is.True);
                Assert.That(preview.Run.Tick, Is.EqualTo(tick));
                Assert.That(preview.Progression.Currency, Is.EqualTo(currency));
                preview.PauseSimulation();
            }
            Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.ActiveSpeciesRules[preview.PlayerSpecies].SeedDropChance,
                Is.EqualTo(baseline.SeedDropChance + 0.02f).Within(0.000001f));
            Assert.That(preview.ActiveSpeciesRules[preview.PlayerSpecies].DigestionEnergyBonus,
                Is.EqualTo(baseline.DigestionEnergyBonus + 0.3f).Within(0.000001f));
            Assert.That(preview.Run.UpgradeAcquisitionTimeline.Count, Is.EqualTo(5));
            var phasePlants = 0;
            foreach (var phase in preview.Run.PhaseResults)
                phasePlants += phase.Metrics.GetActivity(preview.PlayerSpecies).SeedDropSuccesses;
            Assert.That(phasePlants, Is.EqualTo(preview.Run.Metrics.GetActivity(preview.PlayerSpecies).SeedDropSuccesses));
        }

        [UnityTest]
        public IEnumerator TrailblazerPairCanBeChosenThroughPhaseOffers()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            preview.ResetToStart();
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
            Assert.That(preview.TrySetPlayerSpecies("hare", out var message), Is.True, message);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "36", "20", "11100", "720", "0", "6", "0.1",
                "0", "0", "0", false, "400", "20", "10", out message), Is.True, message);
            Assert.That(preview.TryApplyExperimentalFeatures(true, false, "0", out message), Is.True, message);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out message), Is.True, message);
            preview.StartSimulation();
            preview.PauseSimulation();

            var choices = new[]
            {
                SpeciesUpgradeCatalog.FasterMovementId,
                SpeciesUpgradeCatalog.ThreatExposureId,
            };
            var expectedDescriptions = new[]
            {
                "Move faster to escape predators and reach fresh food.",
                "Evade predator attacks more often.",
            };
            for (var choice = 0; choice < choices.Length; choice++)
            {
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                var index = -1;
                for (var option = 0; option < preview.RewardOptionCount; option++)
                    if (preview.GetRewardOptionId(option) == choices[choice]) index = option;
                Assert.That(index, Is.GreaterThanOrEqualTo(0), choices[choice]);
                StringAssert.Contains(expectedDescriptions[choice], preview.GetRewardOptionDisplayName(index));
                var tick = preview.Run.Tick;
                Assert.That(preview.PurchaseReward(index), Is.True);
                Assert.That(preview.Run.Tick, Is.EqualTo(tick));
                preview.PauseSimulation();
            }

            Assert.That(preview.Progression.GetUpgradeLevel(SpeciesUpgradeCatalog.FasterMovementId), Is.EqualTo(1));
            Assert.That(preview.Progression.GetUpgradeLevel(SpeciesUpgradeCatalog.ThreatExposureId), Is.EqualTo(1));
            Assert.That(preview.Run.UpgradeAcquisitionTimeline.Count, Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator DeveloperSettingsRejectingPopulationCapPreservesInputAndRunSeed()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(runtime, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);
            runtime.SpeciesPreview.StopSimulation();

            var viewModelType = viewModel.GetType();
            viewModelType.GetProperty("DeveloperMode")?.SetValue(viewModel, true);
            viewModelType.GetProperty("MaximumPopulationText")?.SetValue(viewModel, "100");
            viewModelType.GetProperty("PlantStartingPopulationText")?.SetValue(viewModel, "40");
            viewModelType.GetProperty("HerbivoreStartingPopulationText")?.SetValue(viewModel, "40");
            viewModelType.GetProperty("CarnivoreStartingPopulationText")?.SetValue(viewModel, "40");

            var runBeforeInvalidSettings = runtime.SpeciesPreview.Run;
            var runSeedBefore = runBeforeInvalidSettings.Seed;
            var applySettingsCommand = viewModelType.GetProperty("ApplySettingsCommand")?.GetValue(viewModel);
            applySettingsCommand?.GetType().GetMethod("Execute")?.Invoke(applySettingsCommand, new object[] { null });

            var settingsMessage = viewModelType.GetProperty("SettingsMessage")?.GetValue(viewModel) as string;
            StringAssert.Contains("total 120 cannot exceed maximum population 100", settingsMessage);
            Assert.That(runtime.SpeciesPreview.Run, Is.SameAs(runBeforeInvalidSettings));
            Assert.That(runtime.SpeciesPreview.Run.Seed, Is.EqualTo(runSeedBefore));
            Assert.That(viewModelType.GetProperty("PlantStartingPopulationText")?.GetValue(viewModel), Is.EqualTo("40"));
            Assert.That(viewModelType.GetProperty("HerbivoreStartingPopulationText")?.GetValue(viewModel), Is.EqualTo("40"));
            Assert.That(viewModelType.GetProperty("CarnivoreStartingPopulationText")?.GetValue(viewModel), Is.EqualTo("40"));
        }

        [UnityTest]
        public IEnumerator AuthoredStartingPopulationRejectsGridThatCannotFitIt()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            var runBefore = preview.Run;

            Assert.That(preview.TryApplyGlobalSettingsForTicks(
                "8", "8", preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4", "0.01", "0", "1", "0", false, out var message), Is.False);
            StringAssert.Contains("Starting populations cannot exceed the grid capacity", message);
            Assert.That(preview.Run, Is.SameAs(runBefore));
        }

        static SpeciesId FindSpeciesId(SpeciesSimulationPreview preview, SpeciesRole role)
        {
            foreach (var entry in preview.ActiveSpeciesRules)
            {
                if (entry.Value.Role == role)
                {
                    return entry.Key;
                }
            }

            Assert.Fail($"No species with role {role} is active.");
            return default;
        }

        [UnityTest]
        public IEnumerator ContinuousPhaseDecisionResumesTheSamePreviewRun()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);

            var preview = runtime.SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            var phaseSettingsApplied = preview.TryApplyContinuousPhases(
                enabled: true,
                phaseLengthValue: "2",
                out var phaseMessage);
            Assert.That(phaseSettingsApplied, Is.True, phaseMessage);

            var settingsApplied = preview.TryApplyGlobalSettingsForTicks(
                "36",
                "20",
                preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4",
                "0.01",
                preview.PlantProbability.ToString(CultureInfo.InvariantCulture),
                preview.HerbivoreProbability.ToString(CultureInfo.InvariantCulture),
                preview.CarnivoreProbability.ToString(CultureInfo.InvariantCulture),
                randomizeSeed: false,
                out var settingsMessage);
            Assert.That(settingsApplied, Is.True, settingsMessage);

            preview.StartSimulation();
            var run = preview.Run;

            // Pausing changes only simulation progression. The same shell and
            // board remain the active presentation while the player decides
            // when to resume.
            preview.PauseSimulation();
            yield return null;

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(
                viewModel.GetType().GetProperty("BoardVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));
            Assert.That(
                viewModel.GetType().GetProperty("CanCloseWindow")?.GetValue(viewModel),
                Is.EqualTo(false));

            preview.ResumeSimulation();

            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.RewardOptionCount, Is.EqualTo(3));
            var offeredIds = Enumerable.Range(0, preview.RewardOptionCount)
                .Select(preview.GetRewardOptionId)
                .ToArray();
            Assert.That(offeredIds.Distinct().Count(), Is.EqualTo(3));
            Assert.That(offeredIds.All(SpeciesUpgradeCatalog.IsExperimentalHerbivoreMutationId), Is.True);
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(2));
            Assert.That(run.TargetTicks, Is.EqualTo(2 * SpeciesSimulationPreview.ContinuousExpeditionPhaseCount));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.AwaitingDecision));

            Assert.That(
                viewModel.GetType().GetProperty("PhaseDecisionVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));
            Assert.That(
                viewModel.GetType().GetProperty("BoardVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));
            Assert.That(
                viewModel.GetType().GetProperty("CanCloseWindow")?.GetValue(viewModel),
                Is.EqualTo(false));

            var currencyBeforeSkip = preview.Progression.Currency;
            preview.ContinueWithoutUpgrade();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(preview.Progression.Currency, Is.EqualTo(currencyBeforeSkip));

            for (var phase = 2; phase < SpeciesSimulationPreview.ContinuousExpeditionPhaseCount; phase++)
            {
                timeout = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.PhaseDecision
                       && Time.realtimeSinceStartup < timeout)
                {
                    yield return null;
                }

                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                Assert.That(run.Tick, Is.EqualTo(phase * 2));
                preview.ContinueWithoutUpgrade();
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
                Assert.That(preview.Run, Is.SameAs(run));
            }

            timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.Results
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(2 * SpeciesSimulationPreview.ContinuousExpeditionPhaseCount));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(
                viewModel.GetType().GetProperty("ResultsTitleText")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Expedition complete"));
            Assert.That(
                viewModel.GetType().GetProperty("PlayNextSimulationText")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("START NEW EXPEDITION"));
            Assert.That(preview.LastExpeditionFailed, Is.False);
            var earnedCurrency = preview.Progression.Currency;
            Assert.That(earnedCurrency, Is.GreaterThan(0));
            var completedRun = preview.Run;
            preview.PlayNextSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.Not.SameAs(completedRun));
            Assert.That(preview.Progression.Currency, Is.EqualTo(earnedCurrency));
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.Zero);
        }

        [UnityTest]
        public IEnumerator EveryOpeningTrailheadCanBeginTheJourney()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            var openings = new[]
            {
                ("fern-glade-start", "canopy-cycle"),
                ("forest-edge-start", "first-cycle"),
                ("pine-verge-start", "pine-cycle"),
            };
            foreach (var (startId, simulationId) in openings)
            {
                preview.ResetToStart();
                Assert.That(preview.JourneyActive, Is.True);
                Assert.That(preview.JourneyMap.Nodes.Count(node => node.Row == 0
                    && preview.CanChooseJourneyNode(node.NodeId)), Is.EqualTo(3));
                Assert.That(preview.ChooseJourneyNode(startId), Is.True);
                Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
                Assert.That(preview.JourneyCurrentNode.NodeId, Is.EqualTo(startId));
                Assert.That(preview.CanChooseJourneyNode(simulationId), Is.True);
                Assert.That(preview.JourneyMap.Nodes.Count(node => node.Row == 0
                    && preview.CanChooseJourneyNode(node.NodeId)), Is.Zero);
                Assert.That(preview.ChooseJourneyNode(simulationId), Is.True);
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            }
        }

        [UnityTest]
        public IEnumerator JourneyNodeRewardsAndSimulationsContinueTheSameBiome()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            var viewModel = GameObject.Find("Prototype Camera").GetComponent("SaltyGame.VM_SimulationShell");
            preview.StopSimulation();
            Assert.That(preview.TryApplyWrapEdges(true, out var wrapMessage), Is.True, wrapMessage);
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var message), Is.True, message);
            Assert.That(preview.JourneyActive, Is.True);
            Assert.That(preview.CanChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.CanChooseJourneyNode("first-cycle"), Is.False);
            Assert.That(preview.ChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyNodeReward));
            Assert.That(preview.JourneyRewardOptionCount, Is.EqualTo(3));
            CollectionAssert.AreEqual(new[] { "faster-movement", "tough-hide", "efficient-digestion" },
                preview.JourneyMap.StartNode.RewardMutationIds);
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(preview.PurchasedUpgradeCount, Is.EqualTo(1));
            Assert.That(preview.HasVisitedJourneyNode(preview.JourneyMap.StartNode), Is.True);
            Assert.That(preview.ChooseJourneyNode("first-cycle"), Is.True);
            var run = preview.Run;
            Assert.That(run.Cells.WrapEdges, Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(run.TargetTicks, Is.EqualTo(24));
            Assert.That(run.UpgradeLoadout.Any(upgrade => upgrade.Id == "faster-movement"), Is.True);

            for (var phase = 1; phase <= 5; phase++)
            {
                var timeout = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
                {
                    yield return null;
                }
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                if (phase == 1)
                {
                    Assert.That(preview.PurchaseReward(0), Is.True);
                }
                else
                {
                    preview.ContinueWithoutUpgrade();
                }
            }

            var boundary = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.JourneyDecision && Time.realtimeSinceStartup < boundary)
            {
                yield return null;
            }
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyDecision));
            Assert.That(run.PhaseResults, Has.Count.EqualTo(6));
            Assert.That(run.Tick, Is.EqualTo(12));
            var mutationCount = preview.PurchasedUpgradeCount;
            Assert.That(mutationCount, Is.GreaterThanOrEqualTo(2));
            Assert.That(viewModel.GetType().GetProperty("ResultsVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));
            Assert.That(viewModel.GetType().GetProperty("JourneyMapVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Collapsed"));
            Assert.That(preview.ChooseJourneyNode("burrow-study"), Is.False);

            var cells = run.Cells;
            var historyCount = run.PopulationHistory.Count;
            var seedDropBefore = preview.ActiveSpeciesRules[new SpeciesId("plant")].SeedDropChance;
            Assert.That(preview.ChooseJourneyNode("seedfall"), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyNodeReward));
            Assert.That(preview.JourneyRewardOptionCount, Is.EqualTo(1));
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.JourneyDecision));
            Assert.That(preview.PurchasedUpgradeCount, Is.EqualTo(mutationCount));
            Assert.That(preview.ActiveSpeciesRules[new SpeciesId("plant")].SeedDropChance,
                Is.EqualTo(seedDropBefore + 0.1f).Within(0.0001f));
            Assert.That(preview.ChooseJourneyNode("burrow-study"), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Cells, Is.SameAs(cells));
            Assert.That(run.Cells.WrapEdges, Is.True);
            Assert.That(run.PopulationHistory.Count, Is.EqualTo(historyCount));
            Assert.That(run.PhaseIndex, Is.EqualTo(7));
            Assert.That(run.UpgradeLoadout.Count(upgrade => upgrade.TargetSpecies == preview.PlayerSpecies),
                Is.GreaterThanOrEqualTo(mutationCount));
            Assert.That(run.UpgradeLoadout.Any(upgrade => upgrade.Id == "journey.seedfall"), Is.True);

            for (var phase = 7; phase <= 11; phase++)
            {
                var timeout = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
                {
                    yield return null;
                }
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                preview.ContinueWithoutUpgrade();
            }
            var finish = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.Results && Time.realtimeSinceStartup < finish)
            {
                yield return null;
            }
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(run.Tick, Is.EqualTo(24));
            Assert.That(run.PhaseResults, Has.Count.EqualTo(12));
            Assert.That(preview.PurchasedUpgradeCount, Is.EqualTo(mutationCount));
            Assert.That(preview.HasVisitedJourneyNode(preview.JourneyCurrentNode), Is.True);
            Assert.That(preview.Progression.Currency, Is.GreaterThan(0));

            var next = viewModel.GetType().GetProperty("PlayNextSimulationCommand")?.GetValue(viewModel);
            next?.GetType().GetMethod("Execute")?.Invoke(next, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(viewModel.GetType().GetProperty("JourneyMapVisibility")?.GetValue(viewModel)?.ToString(),
                Is.EqualTo("Visible"));
            var fresh = viewModel.GetType().GetProperty("StartNewJourneyCommand")?.GetValue(viewModel);
            fresh?.GetType().GetMethod("Execute")?.Invoke(fresh, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(preview.PurchasedUpgradeCount, Is.Zero);
            Assert.That(preview.Progression.Currency, Is.Zero);
        }

        [UnityTest]
        public IEnumerator ResultsActionsStartTheNextExpedition()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(preview, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);

            // The scene host auto-starts on load; reset to the editable Ready
            // state before configuring this focused action test.
            preview.ResetToStart();
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out var phaseMessage), Is.True, phaseMessage);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "8",
                "8",
                preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "10",
                "0.01",
                preview.PlantProbability.ToString(CultureInfo.InvariantCulture),
                preview.HerbivoreProbability.ToString(CultureInfo.InvariantCulture),
                preview.CarnivoreProbability.ToString(CultureInfo.InvariantCulture),
                randomizeSeed: false,
                "4",
                "2",
                "1",
                out var settingsMessage), Is.True, settingsMessage);

            preview.StartSimulation();
            var abandonedRun = preview.Run;
            preview.Progression.AddCurrency(20);
            Assert.That(preview.Progression.TryPurchase(SpeciesUpgradeCatalog.Create("tough-hide")), Is.True);
            preview.EndSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.LastRunEndedEarly, Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);

            var playNextCommand = viewModel.GetType().GetProperty("PlayNextSimulationCommand")?.GetValue(viewModel);
            playNextCommand?.GetType().GetMethod("Execute")?.Invoke(playNextCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(preview.Run, Is.Not.SameAs(abandonedRun));
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.Zero);
            Assert.That(preview.Progression.Currency, Is.Zero);

            preview.EndSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));

            var resetCommand = viewModel.GetType().GetProperty("ResetCommand")?.GetValue(viewModel);
            resetCommand?.GetType().GetMethod("Execute")?.Invoke(resetCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Running));
        }

        [UnityTest]
        public IEnumerator EndConfirmationCanCancelRunningAndForfeitAtThePhaseBoundary()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            var preview = runtime.SpeciesPreview;
            var viewModel = GameObject.Find("Prototype Camera").GetComponent("SaltyGame.VM_SimulationShell");
            var viewModelType = viewModel.GetType();
            var endCommand = viewModelType.GetProperty("EndCommand")?.GetValue(viewModel);
            var confirmEndCommand = viewModelType.GetProperty("ConfirmEndCommand")?.GetValue(viewModel);
            var cancelEndConfirmationCommand = viewModelType.GetProperty("CancelEndConfirmationCommand")?.GetValue(viewModel);
            preview.StopSimulation();
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var phaseMessage), Is.True, phaseMessage);
            preview.StartSimulation();
            yield return null;
            Assert.That(
                endCommand.GetType().GetMethod("CanExecute")?.Invoke(endCommand, new object[] { null }),
                Is.EqualTo(true));
            var run = preview.Run;
            var tickBeforeConfirmation = run.Tick;

            confirmEndCommand.GetType().GetMethod("Execute")?.Invoke(confirmEndCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));
            endCommand.GetType().GetMethod("Execute")?.Invoke(endCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
            Assert.That(viewModelType.GetProperty("EndConfirmationVisibility")?.GetValue(viewModel)?.ToString(), Is.EqualTo("Visible"));
            Assert.That(viewModelType.GetProperty("CanCloseWindow")?.GetValue(viewModel), Is.False);
            cancelEndConfirmationCommand.GetType().GetMethod("Execute")?.Invoke(cancelEndConfirmationCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(tickBeforeConfirmation));

            preview.PauseSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
            var pausedTick = run.Tick;
            endCommand.GetType().GetMethod("Execute")?.Invoke(endCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
            Assert.That(viewModelType.GetProperty("EndConfirmationVisibility")?.GetValue(viewModel)?.ToString(), Is.EqualTo("Visible"));
            cancelEndConfirmationCommand.GetType().GetMethod("Execute")?.Invoke(cancelEndConfirmationCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Paused));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(pausedTick));
            preview.ResumeSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));

            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            var decisionTick = run.Tick;
            preview.Progression.AddCurrency(7);
            var dataAtDecision = preview.Progression.Currency;
            endCommand.GetType().GetMethod("Execute")?.Invoke(endCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(viewModelType.GetProperty("EndConfirmationVisibility")?.GetValue(viewModel)?.ToString(), Is.EqualTo("Visible"));
            cancelEndConfirmationCommand.GetType().GetMethod("Execute")?.Invoke(cancelEndConfirmationCommand, new object[] { null });
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(decisionTick));
            Assert.That(preview.Progression.Currency, Is.EqualTo(dataAtDecision));

            endCommand.GetType().GetMethod("Execute")?.Invoke(endCommand, new object[] { null });
            confirmEndCommand.GetType().GetMethod("Execute")?.Invoke(confirmEndCommand, new object[] { null });

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(decisionTick));
            Assert.That(preview.LastRunEndedEarly, Is.True);
            Assert.That(preview.LastExpeditionFailed, Is.False);
            Assert.That(preview.Progression.Currency, Is.Zero);
            Assert.That(viewModelType.GetProperty("EndConfirmationVisibility")?.GetValue(viewModel)?.ToString(), Is.EqualTo("Collapsed"));
            Assert.That(viewModelType.GetProperty("ResultsTitleText")?.GetValue(viewModel), Is.EqualTo("Expedition ended"));
        }

        [UnityTest]
        public IEnumerator PlayerSpeciesExtinctionFailsImmediatelyWithoutRewards()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var phaseMessage), Is.True, phaseMessage);

            var playerRole = preview.ActiveSpeciesRules[preview.PlayerSpecies].Role;
            var herbivores = playerRole == SpeciesRole.Herbivore ? 0 : 1;
            var carnivores = playerRole == SpeciesRole.Carnivore ? 0 : 1;
            Assert.That(
                playerRole == SpeciesRole.Herbivore || playerRole == SpeciesRole.Carnivore,
                Is.True);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "8",
                "8",
                preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "20",
                "0.01",
                "0",
                "0",
                "0",
                randomizeSeed: false,
                "0",
                herbivores.ToString(CultureInfo.InvariantCulture),
                carnivores.ToString(CultureInfo.InvariantCulture),
                out var settingsMessage), Is.True, settingsMessage);

            preview.StartSimulation();
            preview.Progression.AddCurrency(9);
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.Results && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.Run.Tick, Is.EqualTo(1));
            Assert.That(preview.LastExpeditionFailed, Is.True);
            Assert.That(preview.LastRunEndedEarly, Is.False);
            Assert.That(preview.Progression.Currency, Is.Zero);
            yield return null;
            var viewModel = GameObject.Find("Prototype Camera").GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(
                viewModel.GetType().GetProperty("ResultsTitleText")?.GetValue(viewModel),
                Is.EqualTo("Expedition failed"));
        }

        [UnityTest]
        public IEnumerator PhaseDecisionCanPurchaseAuthoredUpgradeAndResumeSameRun()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            Assert.That(runtime, Is.Not.Null);

            var preview = runtime.SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out var phaseMessage), Is.True, phaseMessage);
            Assert.That(preview.TryApplyGlobalSettingsForTicks(
                "36",
                "20",
                "0",
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4",
                "0.01",
                "0",
                "1",
                "0",
                randomizeSeed: false,
                out var settingsMessage), Is.True, settingsMessage);

            preview.StartSimulation();
            var run = preview.Run;
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            var rewardIndex = Enumerable.Range(0, preview.RewardOptionCount)
                .First(index => preview.GetRewardOptionId(index) == SpeciesUpgradeCatalog.ToughHideId);
            Assert.That(preview.GetRewardOptionId(rewardIndex), Is.EqualTo("tough-hide"));
            StringAssert.Contains("TOUGH HIDE", preview.GetRewardOptionDisplayName(rewardIndex));
            StringAssert.Contains("Block more incoming attacks", preview.GetRewardOptionDisplayName(rewardIndex));
            StringAssert.DoesNotContain("TRAILBLAZER", preview.GetRewardOptionDisplayName(rewardIndex));
            StringAssert.Contains("FREE", preview.GetRewardOptionDisplayName(rewardIndex));
            StringAssert.DoesNotContain("COST", preview.GetRewardOptionDisplayName(rewardIndex));
            Assert.That(preview.Progression.TrySpend(preview.Progression.Currency), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);
            Assert.That(preview.CanPurchaseReward(rewardIndex), Is.True);
            var currencyAtBoundary = preview.Progression.Currency;
            var blockBefore = preview.ActiveSpeciesRules[preview.PlayerSpecies].BlockAmount;

            Assert.That(preview.PurchaseReward(rewardIndex), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Running));
            Assert.That(run.Tick, Is.EqualTo(2));
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.EqualTo(1));
            Assert.That(run.UpgradeLoadout.Count, Is.EqualTo(1));
            Assert.That(
                preview.ActiveSpeciesRules[preview.PlayerSpecies].BlockAmount,
                Is.EqualTo(blockBefore + 2));
            Assert.That(preview.Progression.Currency, Is.EqualTo(currencyAtBoundary));

            // A repeated click cannot purchase or apply the same boundary twice.
            Assert.That(preview.PurchaseReward(rewardIndex), Is.False);
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.EqualTo(1));
            Assert.That(run.UpgradeLoadout.Count, Is.EqualTo(1));

            for (var phase = 2; phase < SpeciesSimulationPreview.ContinuousExpeditionPhaseCount; phase++)
            {
                timeout = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.PhaseDecision
                       && Time.realtimeSinceStartup < timeout)
                {
                    yield return null;
                }

                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                Assert.That(run.Tick, Is.EqualTo(phase * 2));
                preview.ContinueWithoutUpgrade();
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            }

            timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.Results
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(preview.Run, Is.SameAs(run));
            Assert.That(run.Tick, Is.EqualTo(2 * SpeciesSimulationPreview.ContinuousExpeditionPhaseCount));
            Assert.That(run.Status, Is.EqualTo(SimulationRunStatus.Complete));
            Assert.That(preview.CanPurchaseReward(rewardIndex), Is.False);

            var earnedFieldData = preview.Progression.Currency;
            preview.PlayNextSimulation();
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Running));
            Assert.That(preview.Run, Is.Not.SameAs(run));
            Assert.That(preview.Progression.PurchasedUpgradeCount, Is.Zero);
            Assert.That(preview.Progression.Currency, Is.EqualTo(earnedFieldData));
        }

        [UnityTest]
        public IEnumerator CoupledResponseAppliesToTheCounterpartAtTheSameBoundary()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(viewModel, Is.Not.Null);
            var viewModelType = viewModel.GetType();
            viewModelType.GetProperty("CoupledSpeciesResponsesEnabled")?.SetValue(viewModel, true);
            StringAssert.Contains(
                "ON (EXPERIMENTAL)",
                viewModelType.GetProperty("CoupledSpeciesResponsesToggleText")?.GetValue(viewModel) as string);
            var applySettingsCommand = viewModelType.GetProperty("ApplySettingsCommand")?.GetValue(viewModel);
            applySettingsCommand?.GetType().GetMethod("Execute")?.Invoke(applySettingsCommand, new object[] { null });
            Assert.That(preview.CoupledSpeciesResponsesEnabled, Is.True);
            Assert.That(preview.TryApplyExperimentalFeatures(true, true, "0", out var message), Is.True, message);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out message), Is.True, message);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "8", "8", "0", preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "2", "0.01", "0", "1", "0", false, "0", "1", "0", out message), Is.True, message);
            preview.StartSimulation();
            preview.Progression.AddCurrency(10);
            var run = preview.Run;
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            var rewardIndex = Enumerable.Range(0, preview.RewardOptionCount)
                .First(index => preview.GetRewardOptionId(index) == SpeciesUpgradeCatalog.ToughHideId);
            Assert.That(preview.GetRewardOptionId(rewardIndex), Is.EqualTo(SpeciesUpgradeCatalog.ToughHideId));
            StringAssert.Contains("Fox responds: PIERCING BITE", preview.GetRewardOptionDisplayName(rewardIndex));
            Assert.That(SpeciesUpgradeCatalog.TryGetCoupledResponse(
                preview.PlayerSpecies,
                SpeciesUpgradeCatalog.ToughHideId,
                out var carnivore,
                out _), Is.True);
            var attackModifier = preview.ActiveSpeciesRules[carnivore].AttackModifier;

            Assert.That(preview.PurchaseReward(rewardIndex), Is.True);
            Assert.That(run.UpgradeLoadout, Has.Count.EqualTo(2));
            Assert.That(run.UpgradeLoadout[0].TargetSpecies, Is.EqualTo(preview.PlayerSpecies));
            Assert.That(run.UpgradeLoadout[1].TargetSpecies, Is.EqualTo(carnivore));
            Assert.That(run.UpgradeLoadout[1].Id, Is.EqualTo(SpeciesUpgradeCatalog.PiercingBiteId));
            Assert.That(run.UpgradeAcquisitionTimeline[0].Source,
                Is.EqualTo(SimulationUpgradeAcquisition.PlayerChoiceSource));
            Assert.That(run.UpgradeAcquisitionTimeline[1].Source,
                Is.EqualTo(SimulationUpgradeAcquisition.CoupledResponseSource));
            Assert.That(run.UpgradeAcquisitionTimeline[1].TriggeringUpgradeId,
                Is.EqualTo(SpeciesUpgradeCatalog.ToughHideId));
            Assert.That(preview.ActiveSpeciesRules[carnivore].AttackModifier, Is.EqualTo(attackModifier + 1));
            StringAssert.Contains("PIERCING BITE", preview.PhaseRewardMessage);
        }

        [UnityTest]
        public IEnumerator FoxOfferHidesBroodDriveWhenItsHareResponseIsCapped()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TryApplyExperimentalFeatures(true, true, "0", out var message), Is.True, message);
            Assert.That(preview.TrySetPlayerSpecies("fox", out message), Is.True, message);
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out message), Is.True, message);
            Assert.That(preview.TryApplyGlobalSettingsForTicks(
                "36", "20", "3", preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4", "0.01", "0", "1", "0", false, out message), Is.True, message);

            preview.StartSimulation();
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.ActiveSpeciesRules[new SpeciesId("hare")].ReproductionChance, Is.EqualTo(1f));
            Assert.That(preview.RewardOptionCount, Is.EqualTo(3));
            Assert.That(preview.GetRewardOptionId(0), Is.Not.EqualTo(SpeciesUpgradeCatalog.BroodDriveId));
            Assert.That(preview.GetRewardOptionId(1), Is.Not.EqualTo(SpeciesUpgradeCatalog.BroodDriveId));
            Assert.That(preview.CanPurchaseReward(0), Is.True);
            Assert.That(preview.PurchaseReward(0), Is.True);
        }

        [UnityTest]
        public IEnumerator TrailblazerFivePickPathWorksThroughNormalPhaseOffersAndPausedSteps()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;
            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "36", "20", "5", "720", "0", "6", "0.1", "0", "0", "0", false,
                "400", "25", "15", out var message), Is.True, message);
            Assert.That(preview.TryApplyExperimentalFeatures(true, false, "0", out message), Is.True, message);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out message), Is.True, message);
            preview.StartSimulation();
            var run = preview.Run;
            var originalRules = preview.Progression.CurrentRules;
            var picks = new[] { "faster-movement", "threat-exposure", "faster-movement", "threat-exposure", "faster-movement" };
            for (var phase = 0; phase < picks.Length; phase++)
            {
                preview.PauseSimulation();
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                Assert.That(preview.AdvanceOneTickWhilePaused(), Is.False);
                var option = Enumerable.Range(0, preview.RewardOptionCount)
                    .First(index => preview.GetRewardOptionId(index) == picks[phase]);
                var currency = preview.Progression.Currency;
                Assert.That(preview.PurchaseReward(option), Is.True);
                Assert.That(preview.Progression.Currency, Is.EqualTo(currency));
                Assert.That(preview.Run, Is.SameAs(run));
                Assert.That(run.UpgradeAcquisitionTimeline[phase].EffectiveTick, Is.EqualTo(phase + 1));
            }
            Assert.That(preview.Progression.CurrentRules.MovementSpeed, Is.EqualTo(originalRules.MovementSpeed + 1.5f));
            Assert.That(preview.Progression.CurrentRules.FleeMovementSpeedBonus, Is.EqualTo(originalRules.FleeMovementSpeedBonus));
            Assert.That(preview.Progression.PreContactAvoidanceChance, Is.EqualTo(0.16f).Within(0.00001f));
            Assert.That(run.UpgradeLoadout[1].PreContactAvoidanceChanceBonus, Is.EqualTo(0.08f));
            Assert.That(run.UpgradeLoadout[1].Modifiers, Is.Empty);
            preview.PauseSimulation();
            Assert.That(preview.AdvanceOneTickWhilePaused(), Is.True);
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
            Assert.That(run.Tick, Is.EqualTo(6));
            Assert.That(run.PhaseResults, Has.Count.EqualTo(6));
        }

        [UnityTest]
        public IEnumerator ApplicableHerbivoreSkillsRetainLevelsAndAcquisitionsAcrossDecisionBoundaries()
        {
            var ids = new[] { "tough-hide", "efficient-digestion", "crowding-tolerance",
                "threat-exposure" };
            var firstOfferSeeds = new[] { 0, 1, 4, 3 };
            for (var skillIndex = 0; skillIndex < ids.Length; skillIndex++)
            {
                yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
                yield return null;
                var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
                preview.StopSimulation();
                Assert.That(preview.TrySetJourneyPrototypeEnabled(false), Is.True);
                Assert.That(preview.TryApplyExperimentalFeatures(true, "0", out var message), Is.True, message);
                Assert.That(preview.TryApplyContinuousPhases(true, "1", out message), Is.True, message);
                // The seed selects the first experimental offer deterministically.
                Assert.That(preview.TryApplyGlobalSettingsForTicks(
                    "36", "20", firstOfferSeeds[skillIndex].ToString(CultureInfo.InvariantCulture),
                    preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                    preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                    "10", "0.01", "0", "1", "0", false, out message), Is.True, message);
                preview.StartSimulation();
                var run = preview.Run;
                var originalRules = preview.Progression.CurrentRules;
                preview.Progression.AddCurrency(100);
                Assert.That(preview.Progression.CanApplyFreeUpgrade(
                    SpeciesUpgradeCatalog.Create(SpeciesUpgradeCatalog.ReproductiveDriveId)), Is.False);
                var upgrade = SpeciesUpgradeCatalog.Create(ids[skillIndex]);
                // Preload one level, then exercise every in-expedition choice.
                Assert.That(preview.Progression.TryPurchase(upgrade), Is.True);
                for (var phase = 1; phase < SpeciesSimulationPreview.ContinuousExpeditionPhaseCount; phase++)
                {
                    var timeout = Time.realtimeSinceStartup + 5f;
                    while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
                    {
                        yield return null;
                    }
                    Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
                    Assert.That(preview.RewardOptionCount, Is.EqualTo(3));
                    var offeredIds = Enumerable.Range(0, preview.RewardOptionCount)
                        .Select(preview.GetRewardOptionId)
                        .ToArray();
                    Assert.That(offeredIds.Distinct().Count(), Is.EqualTo(3));
                    Assert.That(offeredIds.All(SpeciesUpgradeCatalog.IsExperimentalHerbivoreMutationId), Is.True);
                    var rewardIndex = Enumerable.Range(0, preview.RewardOptionCount)
                        .First(index => preview.GetRewardOptionId(index) == upgrade.Id);
                    Assert.That(preview.GetRewardOptionDisplayName(rewardIndex), Does.Contain($"LV {phase + 1}"));
                    Assert.That(preview.GetRewardOptionDisplayName(rewardIndex), Does.Not.Contain("COST"));
                    Assert.That(preview.GetRewardOptionDisplayName(rewardIndex), Does.Not.Contain("DATA"));
                    Assert.That(preview.GetRewardOptionId(rewardIndex), Is.EqualTo(upgrade.Id));
                    if (upgrade.Id == SpeciesUpgradeCatalog.ToughHideId && phase > 1)
                    {
                        var completedPhase = run.PhaseResults[run.PhaseResults.Count - 1];
                        var blockedIncomingAttacks = completedPhase.Metrics.CombatRollEvents.Count(
                            combat => combat.TargetSpecies == preview.PlayerSpecies && !combat.Hit);
                        StringAssert.Contains(
                            $"Incoming attacks blocked this phase: {blockedIncomingAttacks}",
                            preview.PhaseRewardMessage);
                    }
                    Assert.That(preview.CanPurchaseReward(rewardIndex), Is.True);
                    var currency = preview.Progression.Currency;
                    Assert.That(preview.PurchaseReward(rewardIndex), Is.True);
                    Assert.That(preview.Run, Is.SameAs(run));
                    Assert.That(run.Tick, Is.EqualTo(phase));
                    Assert.That(preview.Progression.GetUpgradeLevel(upgrade.Id), Is.EqualTo(phase + 1));
                    Assert.That(preview.Progression.Currency, Is.EqualTo(currency));
                    Assert.That(run.UpgradeLoadout, Has.Count.EqualTo(phase + 1));
                    Assert.That(run.UpgradeAcquisitionTimeline, Has.Count.EqualTo(phase + 1));
                    Assert.That(run.UpgradeLoadout[phase].Cost, Is.Zero);
                    Assert.That(run.UpgradeAcquisitionTimeline[phase].EffectiveTick, Is.EqualTo(phase));
                    Assert.That(run.UpgradeAcquisitionTimeline[phase].Order, Is.EqualTo(phase));
                    Assert.That(preview.ActiveSpeciesRules[preview.PlayerSpecies], Is.SameAs(preview.Progression.CurrentRules));
                    if (upgrade.Id == "threat-exposure")
                    {
                        Assert.That(preview.Progression.CurrentRules.FleeMovementSpeedBonus,
                            Is.EqualTo(originalRules.FleeMovementSpeedBonus));
                        Assert.That(preview.Progression.PreContactAvoidanceChance,
                            Is.EqualTo((phase + 1) * 0.08f).Within(0.00001f));
                    }
                    Assert.That(preview.PurchaseReward(rewardIndex), Is.False, "A repeated click must not charge again.");
                }
                var finalTimeout = Time.realtimeSinceStartup + 5f;
                while (preview.State != SpeciesPreviewState.Results && Time.realtimeSinceStartup < finalTimeout)
                {
                    yield return null;
                }

                Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Results));
                Assert.That(run.Tick, Is.EqualTo(SpeciesSimulationPreview.ContinuousExpeditionPhaseCount));
                Assert.That(run.PhaseResults, Has.Count.EqualTo(SpeciesSimulationPreview.ContinuousExpeditionPhaseCount));
            }
        }

        [UnityTest]
        public IEnumerator PredatorPhaseRewardsUseAuthoredPredatorSkills()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            Assert.That(preview.TryApplyExperimentalFeatures(true, "0", out var message), Is.True, message);
            Assert.That(preview.TrySetPlayerSpecies("fox", out message), Is.True, message);
            Assert.That(preview.TryApplyContinuousPhases(true, "2", out message), Is.True, message);
            Assert.That(preview.TryApplyGlobalSettingsForTicks(
                "36", "20", preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4", "0.01", "0", "1", "0", false, out message), Is.True, message);

            preview.StartSimulation();
            preview.Progression.AddCurrency(10);
            var run = preview.Run;
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.RewardOptionCount, Is.EqualTo(3));
            Assert.That(
                preview.GetRewardOptionId(2),
                Is.EqualTo(SpeciesUpgradeCatalog.PopulationReinforcementId));
            StringAssert.Contains("+1 FOX", preview.GetRewardOptionDisplayName(2));
            StringAssert.Contains("FREE", preview.GetRewardOptionDisplayName(2));
            StringAssert.DoesNotContain("COST", preview.GetRewardOptionDisplayName(2));
            var predatorSkillIds = new[] { "relentless-pursuit", "piercing-bite", "hunt-urgency", "brood-drive" };
            for (var index = 0; index < 2; index++)
            {
                var optionId = preview.GetRewardOptionId(index);
                Assert.That(Array.IndexOf(predatorSkillIds, optionId), Is.GreaterThanOrEqualTo(0));
                StringAssert.Contains("FREE", preview.GetRewardOptionDisplayName(index));
                StringAssert.DoesNotContain("COST", preview.GetRewardOptionDisplayName(index));
                StringAssert.DoesNotContain("FASTER", preview.GetRewardOptionDisplayName(index));
                StringAssert.DoesNotContain("ATTACK", preview.GetRewardOptionDisplayName(index));
                StringAssert.DoesNotContain("BLOCK", preview.GetRewardOptionDisplayName(index));
            }

            Assert.That(preview.Progression.TrySpend(preview.Progression.Currency), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);
            Assert.That(preview.CanPurchaseReward(2), Is.True);
            Assert.That(preview.PurchaseReward(2), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);
            Assert.That(run.UpgradeLoadout, Has.Count.EqualTo(1));
            Assert.That(run.UpgradeLoadout[0].PopulationToAdd, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator AuthoredMutationIsFreeAtPhaseBoundary()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var preview = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>().SpeciesPreview;
            preview.StopSimulation();
            typeof(SpeciesSimulationPreview)
                .GetField("bevExperimentalFeaturesEnabled", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(preview, false);
            Assert.That(preview.TryApplyContinuousPhases(true, "1", out var message), Is.True, message);
            Assert.That(preview.TryApplyGlobalSettingsForTicksWithStartingPopulations(
                "8", "8", preview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                preview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                preview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4", "0.01", "0", "1", "0", false,
                "20", "10", "0", out message), Is.True, message);

            preview.StartSimulation();
            var timeout = Time.realtimeSinceStartup + 5f;
            while (preview.State != SpeciesPreviewState.PhaseDecision && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            Assert.That(preview.RewardOptionCount, Is.GreaterThan(0));
            StringAssert.Contains("FREE", preview.GetRewardOptionDisplayName(0));
            StringAssert.DoesNotContain("COST", preview.GetRewardOptionDisplayName(0));
            Assert.That(preview.Progression.TrySpend(preview.Progression.Currency), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);
            Assert.That(preview.CanPurchaseReward(0), Is.True);
            Assert.That(preview.PurchaseReward(0), Is.True);
            Assert.That(preview.Progression.Currency, Is.Zero);
        }

        [UnityTest]
        public IEnumerator PlayerModeUsesContinuousPhasesAndDeveloperModeCanSelectSingleRun()
        {
            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            var runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            var viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(runtime, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(runtime.SpeciesPreview.ContinuousPhasesEnabled, Is.True);

            runtime.SpeciesPreview.StopSimulation();

            var continuousSettingsApplied = runtime.SpeciesPreview.TryApplyContinuousPhases(
                enabled: false,
                phaseLengthValue: string.Empty,
                out var continuousMessage);
            Assert.That(continuousSettingsApplied, Is.True, continuousMessage);
            var settingsApplied = runtime.SpeciesPreview.TryApplyGlobalSettingsForTicks(
                "36",
                "20",
                runtime.SpeciesPreview.BaseSeed.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.MaximumPopulation.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.MinimumPopulation.ToString(CultureInfo.InvariantCulture),
                "4",
                "0.01",
                runtime.SpeciesPreview.PlantProbability.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.HerbivoreProbability.ToString(CultureInfo.InvariantCulture),
                runtime.SpeciesPreview.CarnivoreProbability.ToString(CultureInfo.InvariantCulture),
                randomizeSeed: false,
                out var settingsMessage);
            Assert.That(settingsApplied, Is.True, settingsMessage);

            // Player mode reapplies the VM's continuous settings on start. Keep
            // this test's boundary short enough to observe the decision state.
            viewModel.GetType().GetProperty("RunTicksText")?.SetValue(viewModel, "40");
            viewModel.GetType().GetProperty("PhaseLengthTicksText")?.SetValue(viewModel, "4");
            viewModel.GetType().GetProperty("DeveloperMode")?.SetValue(viewModel, false);
            var playerStartCommand = viewModel.GetType().GetProperty("StartCommand")?.GetValue(viewModel);
            playerStartCommand?.GetType().GetMethod("Execute")?.Invoke(playerStartCommand, new object[] { null });

            var timeout = Time.realtimeSinceStartup + 5f;
            while (runtime.SpeciesPreview.State != SpeciesPreviewState.PhaseDecision
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(runtime.SpeciesPreview.State, Is.EqualTo(SpeciesPreviewState.PhaseDecision));
            var phaseDecisionTitle = viewModel.GetType()
                .GetProperty("PhaseDecisionTitleText")
                ?.GetValue(viewModel) as string;
            Assert.That(phaseDecisionTitle, Is.EqualTo("PHASE 01 COMPLETE"));

            yield return SceneManager.LoadSceneAsync("CellularAutomataPrototype");
            yield return null;

            runtime = UnityEngine.Object.FindAnyObjectByType<CellularAutomataPrototypeRuntime>();
            viewModel = GameObject.Find("Prototype Camera")
                ?.GetComponent("SaltyGame.VM_SimulationShell");
            Assert.That(runtime, Is.Not.Null);
            Assert.That(viewModel, Is.Not.Null);

            runtime.SpeciesPreview.StopSimulation();
            var viewModelType = viewModel.GetType();
            viewModelType.GetProperty("DeveloperMode")?.SetValue(viewModel, true);
            viewModelType.GetProperty("RunTicksText")?.SetValue(viewModel, "4");
            viewModelType.GetProperty("PhaseLengthTicksText")?.SetValue(viewModel, "2");
            viewModelType.GetProperty("ContinuousPhasesEnabled")?.SetValue(viewModel, false);
            var applySettingsCommand = viewModelType.GetProperty("ApplySettingsCommand")?.GetValue(viewModel);
            applySettingsCommand?.GetType().GetMethod("Execute")?.Invoke(applySettingsCommand, new object[] { null });
            Assert.That(runtime.SpeciesPreview.ContinuousPhasesEnabled, Is.False);

            var startCommand = viewModelType.GetProperty("StartCommand")?.GetValue(viewModel);
            startCommand?.GetType().GetMethod("Execute")?.Invoke(startCommand, new object[] { null });
            timeout = Time.realtimeSinceStartup + 5f;
            while (runtime.SpeciesPreview.State != SpeciesPreviewState.Rewards
                   && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Assert.That(runtime.SpeciesPreview.State, Is.EqualTo(SpeciesPreviewState.Rewards));
            Assert.That(runtime.SpeciesPreview.Run.Status, Is.EqualTo(SimulationRunStatus.Complete));
        }
    }
}
