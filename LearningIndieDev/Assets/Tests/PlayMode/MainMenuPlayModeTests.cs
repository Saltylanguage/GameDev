using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SaltyGame.PlayModeTests
{
    [Category("UI")]
    public sealed class MainMenuPlayModeTests
    {
        const string MainMenuScene = "MainMenu";
        const string DesktopScene = "GalapagOSDesktopTest";
        const string ViewModelType = "SaltyGame.VM_MainMenu";
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
        public IEnumerator ContinueIsDisabledWithoutAProfile()
        {
            ClearProfiles();
            yield return LoadMainMenu();

            var viewModel = FindViewModel();
            Assert.That(viewModel, Is.Not.Null, "MainMenu.unity must contain VM_MainMenu.");
            Assert.That(GetBool(viewModel, "ContinueEnabled"), Is.False);

            var continueCommand = GetCommand(viewModel, "ContinueCommand");
            Assert.That(CanExecute(continueCommand), Is.False);
            Execute(continueCommand);
            yield return null;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(MainMenuScene));
        }

        [UnityTest]
        public IEnumerator ProfileCreationEnablesContinueAndLoadsDesktop()
        {
            ClearProfiles();
            yield return LoadMainMenu();

            var viewModel = FindViewModel();
            SetProperty(viewModel, "ProfileNameInput", "Darwin Station");
            ExecuteCommand(viewModel, "OpenProfileSelectionCommand");
            ExecuteCommand(viewModel, "CreateInitialProfileCommand");

            Assert.That(GetBool(viewModel, "ContinueEnabled"), Is.True);
            Assert.That(GetString(viewModel, "CurrentProfileName"), Is.EqualTo("Darwin Station"));
            var menuProfile = GameObject.Find("Main Camera")?.GetComponent<Helper_ProfileSession>();
            Assert.That(menuProfile, Is.Not.Null);
            Assert.That(menuProfile.TrySetGenomeConfiguration(
                new SpeciesId("hare"),
                new[] { "hare.guard" },
                new[] { "hare.guard" },
                out var validationMessage), Is.True, validationMessage);
            var profileId = menuProfile.Current.ProfileId;

            ExecuteCommand(viewModel, "ContinueCommand");
            var transitionDeadline = Time.realtimeSinceStartup + 3f;
            while (SceneManager.GetActiveScene().name != DesktopScene
                && Time.realtimeSinceStartup < transitionDeadline)
            {
                yield return null;
            }

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(DesktopScene));
            var desktopRoot = GameObject.Find("GalapagOS Desktop Test Camera");
            var desktopProfile = desktopRoot?.GetComponent<Helper_ProfileSession>();
            var preview = Object.FindAnyObjectByType<SpeciesSimulationPreview>();
            Assert.That(desktopProfile?.Current.ProfileId, Is.EqualTo(profileId));
            Assert.That(preview, Is.Not.Null);
            Assert.That(preview.BaseSeed, Is.EqualTo(10100));
            Assert.That(preview.SelectedScenario?.name, Is.EqualTo("ForestEdge"));
            Assert.That(preview.PlayerSpecies, Is.EqualTo(new SpeciesId("hare")));
            Assert.That(preview.State, Is.EqualTo(SpeciesPreviewState.Ready));
            Assert.That(preview.Run.Status, Is.EqualTo(SimulationRunStatus.Ready));
            Assert.That(preview.Run.Tick, Is.Zero);
            Assert.That(preview.ActiveGenomeSnapshot.TryGetSpeciesGenome(
                new SpeciesId("hare"), out var genome), Is.True);
            Assert.That(genome.ActiveNodeIds, Is.EqualTo(new[] { "hare.guard" }));
            Assert.That(desktopProfile.TrySetGenomeConfiguration(
                new SpeciesId("hare"),
                new[] { "hare.guard" },
                System.Array.Empty<string>(),
                out var updatedProfileMessage), Is.True, updatedProfileMessage);
            Assert.That(preview.ActiveGenomeSnapshot.TryGetSpeciesGenome(
                new SpeciesId("hare"), out var frozenGenome), Is.True);
            Assert.That(frozenGenome.ActiveNodeIds, Is.EqualTo(new[] { "hare.guard" }));
            var desktopViewModel = desktopRoot.GetComponent("SaltyGame.VM_GalapagOS_Desktop");
            var openSimulation = GetCommand(desktopViewModel, "OpenDesktopIconCommand");
            openSimulation.GetType().GetMethod("Execute")?.Invoke(
                openSimulation, new object[] { "Simulation" });
            Assert.That(preview.ChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.ChooseJourneyNode("first-cycle"), Is.True);
            var tickDeadline = Time.realtimeSinceStartup + 3f;
            while (preview.Run.Tick < 2 && Time.realtimeSinceStartup < tickDeadline)
            {
                yield return null;
            }

            desktopProfile.Flush();
            Assert.That(desktopProfile.Current.SimulationTicks, Is.GreaterThan(0));
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator LastLoadedProfileSurvivesMainMenuReload()
        {
            ClearProfiles();
            yield return LoadMainMenu();

            var firstViewModel = FindViewModel();
            SetProperty(firstViewModel, "ProfileNameInput", "Returning Researcher");
            ExecuteCommand(firstViewModel, "OpenProfileSelectionCommand");
            ExecuteCommand(firstViewModel, "CreateInitialProfileCommand");

            yield return LoadMainMenu();
            var reloadedViewModel = FindViewModel();
            Assert.That(GetString(reloadedViewModel, "CurrentProfileName"), Is.EqualTo("Returning Researcher"));
            Assert.That(GetBool(reloadedViewModel, "ContinueEnabled"), Is.True);
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator ActiveProfilePlaytimeSurvivesMainMenuReload()
        {
            ClearProfiles();
            yield return LoadMainMenu();

            var viewModel = FindViewModel();
            SetProperty(viewModel, "ProfileNameInput", "Timed Researcher");
            ExecuteCommand(viewModel, "OpenProfileSelectionCommand");
            ExecuteCommand(viewModel, "CreateInitialProfileCommand");
            var profile = GameObject.Find("Main Camera")?.GetComponent<Helper_ProfileSession>();
            Assert.That(profile, Is.Not.Null);

            yield return new WaitForSecondsRealtime(0.05f);
            profile.Flush();
            var savedMilliseconds = profile.Current.PlayTimeMilliseconds;
            Assert.That(savedMilliseconds, Is.GreaterThan(0));

            yield return LoadMainMenu();
            var reloaded = GameObject.Find("Main Camera")?.GetComponent<Helper_ProfileSession>();
            Assert.That(reloaded?.Current.PlayTimeMilliseconds, Is.GreaterThanOrEqualTo(savedMilliseconds));
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator SimulationLaunchWithoutItsProfileReturnsToProfileSelection()
        {
            ClearProfiles();
            yield return LoadMainMenu();

            var transition = GameObject.Find("Main Camera")?.GetComponent<Helper_SceneTransition>();
            Assert.That(transition, Is.Not.Null);
            LogAssert.Expect(LogType.Error,
                "The selected profile could not be loaded for Simulation launch.");
            Assert.That(transition.LoadSimulation(new SimulationLaunchRequest(
                "missing-profile", "ForestEdge", "hare", 10100)), Is.True);

            var deadline = Time.realtimeSinceStartup + 3f;
            Component recoveredViewModel = null;
            while (Time.realtimeSinceStartup < deadline)
            {
                yield return null;
                if (SceneManager.GetActiveScene().name == MainMenuScene)
                {
                    recoveredViewModel = FindViewModel();
                    if (recoveredViewModel != null
                        && GetVisibility(recoveredViewModel, "ProfileSelectionVisibility") == "Visible")
                    {
                        break;
                    }
                }
            }

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(MainMenuScene));
            Assert.That(recoveredViewModel, Is.Not.Null);
            Assert.That(GetVisibility(recoveredViewModel, "ProfileSelectionVisibility"), Is.EqualTo("Visible"));
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator PartialSaveRecoveryIsVisibleWithAUsableProfile()
        {
            ClearProfiles();
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey,
                "{\"version\":1,\"profiles\":[{\"id\":\"valid\",\"name\":\"Kept\"},"
                + "{\"id\":\"invalid\",\"currency\":-1}],\"lastLoadedProfileId\":\"valid\"}");
            PlayerPrefs.Save();
            yield return LoadMainMenu();

            var viewModel = FindViewModel();
            Assert.That(GetString(viewModel, "CurrentProfileName"),
                Is.EqualTo("PROFILE SAVE NEEDS RECOVERY"));
            Assert.That(GetBool(viewModel, "ContinueEnabled"), Is.True);
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator VersionTwoProfilesMigrateCurrencyAndPreserveProgressionOnce()
        {
            ClearProfiles();
            PlayerPrefs.DeleteKey(Helper_ProfileSession.RecoveryStoreKey);
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey,
                "{\"version\":2,\"profiles\":[{\"id\":\"legacy\",\"name\":\"Legacy Researcher\","
                + "\"genomeProfiles\":[{\"speciesId\":\"hare\",\"unlockedNodeIds\":[\"hare.guard\"],"
                + "\"activeNodeIds\":[\"hare.guard\"]}],\"currency\":73,\"simulationTicks\":42,"
                + "\"playTimeMilliseconds\":1234,\"unlockedSpeciesIds\":[\"hare\",\"fox\"],"
                + "\"unlockedGenomeSpeciesIds\":[\"hare\"],\"unlockedFeatureIds\":[\"gene-lab\"]}],"
                + "\"lastLoadedProfileId\":\"legacy\"}");
            PlayerPrefs.Save();
            yield return LoadMainMenu();

            var session = GameObject.Find("Main Camera")?.GetComponent<Helper_ProfileSession>();
            Assert.That(session, Is.Not.Null);
            Assert.That(session.Current.ProfileId, Is.EqualTo("legacy"));
            Assert.That(session.Current.ResearchData, Is.EqualTo(73));
            Assert.That(session.Current.Currency, Is.EqualTo(73));
            Assert.That(session.Current.SpeciesDataWallets, Is.Empty);
            Assert.That(session.Current.SimulationTicks, Is.EqualTo(42));
            Assert.That(session.Current.PlayTimeMilliseconds, Is.EqualTo(1234));
            CollectionAssert.AreEqual(
                new[] { new SpeciesId("hare"), new SpeciesId("fox") },
                session.Current.UnlockedSpeciesIds);
            CollectionAssert.AreEqual(new[] { new SpeciesId("hare") },
                session.Current.UnlockedGenomeSpeciesIds);
            CollectionAssert.AreEqual(new[] { "gene-lab" }, session.Current.UnlockedFeatureIds);
            Assert.That(session.Current.TryGetGenomeProfile(new SpeciesId("hare"), out var genome), Is.True);
            CollectionAssert.AreEqual(new[] { "hare.guard" }, genome.UnlockedNodeIds);
            CollectionAssert.AreEqual(new[] { "hare.guard" }, genome.ActiveNodeIds);

            var migratedStore = PlayerPrefs.GetString(Helper_ProfileSession.StoreKey);
            StringAssert.Contains("\"version\":7", migratedStore);
            StringAssert.Contains("\"researchData\":73", migratedStore);
            StringAssert.Contains("\"currency\":73",
                PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey));
            session.Reload();
            Assert.That(session.Current.ResearchData, Is.EqualTo(73));
            Assert.That(session.Current.SpeciesDataWallets, Is.Empty);

            var freshProfile = session.CreateProfile("Fresh Researcher");
            Assert.That(freshProfile.ResearchData, Is.Zero);
            Assert.That(freshProfile.SpeciesDataWallets, Is.Empty);
            Assert.That(freshProfile.UnlockedGenomeSpeciesIds, Is.Empty);
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator VersionThreeSpeciesWalletsSurviveProfileCopiesAndReloads()
        {
            ClearProfiles();
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey,
                "{\"version\":3,\"profiles\":[{\"id\":\"wallets\",\"name\":\"Wallets\","
                + "\"researchData\":42,\"speciesDataWallets\":[{\"speciesId\":\"hare\",\"balance\":19},"
                + "{\"speciesId\":\"fox\",\"balance\":0}]}],\"lastLoadedProfileId\":\"wallets\"}");
            PlayerPrefs.Save();
            yield return LoadMainMenu();

            var session = GameObject.Find("Main Camera")?.GetComponent<Helper_ProfileSession>();
            Assert.That(session, Is.Not.Null);
            Assert.That(session.Current.ResearchData, Is.EqualTo(42));
            Assert.That(session.Current.SpeciesDataWallets[new SpeciesId("hare")], Is.EqualTo(19));
            Assert.That(session.Current.SpeciesDataWallets[new SpeciesId("fox")], Is.Zero);

            session.UnlockFeature("snapshot-copy-check");
            session.Reload();
            Assert.That(session.Current.ResearchData, Is.EqualTo(42));
            Assert.That(session.Current.SpeciesDataWallets[new SpeciesId("hare")], Is.EqualTo(19));
            Assert.That(session.Current.SpeciesDataWallets[new SpeciesId("fox")], Is.Zero);
            StringAssert.Contains("\"speciesId\":\"hare\",\"balance\":19",
                PlayerPrefs.GetString(Helper_ProfileSession.StoreKey));
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator FieldDataSettlementSurvivesSceneReloadWithoutPayingTheJourneyAgain()
        {
            ClearProfiles();
            yield return LoadMainMenu();
            var session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var profileId = session.CreateProfile("FD2 settlement").ProfileId;
            session.AddRabbitData(7);
            session.AddResearchData(40);
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out var purchaseError), Is.True, purchaseError);
            Assert.That(session.TrySettleFieldDataJourney(profileId, "scene-reload", FieldDataJourneyFlowState.Succeeded,
                new SpeciesId("hare"), 100, 1, out _, out var error), Is.True, error);
            Assert.That(session.Current.RabbitData, Is.EqualTo(18));
            Assert.That(session.Current.ResearchData, Is.EqualTo(40));

            yield return LoadMainMenu();
            session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.Current.ProfileId, Is.EqualTo(profileId));
            Assert.That(session.Current.SettlementReceipts.Count, Is.EqualTo(1));
            Assert.That(session.TrySettleFieldDataJourney(profileId, "scene-reload", FieldDataJourneyFlowState.Succeeded,
                new SpeciesId("hare"), 9999, 3, out var prior, out error), Is.True, error);
            Assert.That(prior.LeftoverFieldData, Is.EqualTo(100));
            Assert.That(prior.ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(session.Current.RabbitData, Is.EqualTo(18));
            Assert.That(session.Current.ResearchData, Is.EqualTo(40));
            Assert.That(session.Current.CompletedRuns, Is.EqualTo(1));
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator ConversionResearchSurvivesSceneReloadAndOnlyBoostsFutureSettlements()
        {
            ClearProfiles();
            yield return LoadMainMenu();
            var session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var profileId = session.CreateProfile("Conversion research").ProfileId;
            session.AddResearchData(60);
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out var error), Is.True, error);
            Assert.That(session.TrySettleFieldDataJourney(profileId, "level-one", FieldDataJourneyFlowState.Succeeded,
                new SpeciesId("hare"), 100, out _, out error), Is.True, error);
            yield return LoadMainMenu();
            session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(session.Current.RabbitData, Is.EqualTo(11));
            Assert.That(session.Current.ResearchData, Is.EqualTo(60));
            Assert.That(session.TryPurchaseConversionResearch(profileId, 0, out _), Is.False);
            Assert.That(session.TryPurchaseConversionResearch(profileId, 1, out error), Is.True, error);
            Assert.That(session.TrySettleFieldDataJourney(profileId, "level-two", FieldDataJourneyFlowState.Succeeded,
                new SpeciesId("hare"), 100, out _, out error), Is.True, error);
            yield return LoadMainMenu();
            session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.Current.ConversionResearchLevel, Is.EqualTo(2));
            Assert.That(session.Current.RabbitData, Is.EqualTo(23));
            Assert.That(session.Current.ResearchData, Is.EqualTo(50));
            Assert.That(session.Current.SettlementReceipts[0].ConversionResearchLevel, Is.EqualTo(1));
            Assert.That(session.Current.SettlementReceipts[0].SpeciesDataDeposit, Is.EqualTo(11));
            Assert.That(session.Current.SettlementReceipts[1].SpeciesDataDeposit, Is.EqualTo(12));
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator HareGenomeLicenseSurvivesSceneReloadWithoutAnotherChargeOrNodeEffects()
        {
            ClearProfiles();
            yield return LoadMainMenu();
            var session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var profileId = session.CreateProfile("Hare license").ProfileId;
            session.AddRabbitData(10);
            session.AddResearchData(37);
            Assert.That(session.Current.CompletedRuns, Is.Zero);
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out var error), Is.True, error);
            Assert.That(session.Current.RabbitData, Is.Zero);
            Assert.That(session.Current.ResearchData, Is.EqualTo(37));
            Assert.That(session.Current.GenomeProfiles, Is.Empty);
            yield return LoadMainMenu();
            session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.Current.ProfileId, Is.EqualTo(profileId));
            Assert.That(session.Current.UnlockedGenomeSpeciesIds, Does.Contain(new SpeciesId("hare")));
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out _), Is.False);
            Assert.That(session.Current.RabbitData, Is.Zero);
            Assert.That(session.Current.ResearchData, Is.EqualTo(37));
            Assert.That(session.Current.GenomeProfiles, Is.Empty);
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator PermanentHareNodesSurviveSceneReloadWithTheirPriceCapacityAndCatalogEffects()
        {
            ClearProfiles();
            yield return LoadMainMenu();
            var session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var profileId = session.CreateProfile("Permanent Hare nodes").ProfileId;
            session.AddRabbitData(40);
            session.AddResearchData(37);
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out var error), Is.True, error);
            var active = new[] { "hare.guarded-burrow", "hare.keen-hearing", "hare.swift-digging", "hare.safe-foraging" };
            foreach (var id in active)
                Assert.That(session.TryPurchaseHareGenomeNode(profileId, id, out error), Is.True, error);
            Assert.That(session.Current.RabbitData, Is.EqualTo(10));
            yield return LoadMainMenu();
            session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.Current.ProfileId, Is.EqualTo(profileId));
            Assert.That(session.Current.TryGetGenomeProfile(new SpeciesId("hare"), out var genome), Is.True);
            Assert.That(genome.UnlockedNodeIds, Is.EqualTo(active));
            Assert.That(genome.ActiveNodeIds, Is.EqualTo(active));
            Assert.That(genome.Allocations, Is.Empty);
            var frozen = FieldDataPermanentProgressionDefinition.CreateStarter().FreezeOwnedLoadout(genome);
            var natural = SpeciesRuleDefaults.Create()[SpeciesIds.Herbivore];
            var effective = frozen.GetSpeciesGenome(new SpeciesId("hare")).ApplyToNaturalRules(natural);
            Assert.That(effective.BlockAmount, Is.EqualTo(natural.BlockAmount + 1));
            Assert.That(effective.Awareness.VisionRange, Is.EqualTo(natural.Awareness.VisionRange + 2));
            Assert.That(effective.MovementSpeed, Is.EqualTo(natural.MovementSpeed + 0.25f));
            Assert.That(effective.DigestionEnergyBonus, Is.EqualTo(natural.DigestionEnergyBonus + 0.25f));
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, active[0], out _), Is.False);
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.warren-network", out error), Is.False);
            StringAssert.Contains("capacity", error);
            Assert.That(session.Current.RabbitData, Is.EqualTo(10));
            Assert.That(session.Current.ResearchData, Is.EqualTo(37));
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator PermanentHareResetAndReplacementSurviveSceneReloadWithoutRefundOrDuplicateFee()
        {
            ClearProfiles();
            yield return LoadMainMenu();
            var session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            var profileId = session.CreateProfile("Permanent replacement").ProfileId;
            session.AddRabbitData(50);
            session.AddResearchData(60);
            var hare = new SpeciesId("hare");
            Assert.That(session.TryPurchaseHareGenomeLicense(profileId, out var error), Is.True, error);
            foreach (var id in new[] { "hare.guarded-burrow", "hare.keen-hearing", "hare.swift-digging", "hare.safe-foraging" })
                Assert.That(session.TryPurchaseHareGenomeNode(profileId, id, out error), Is.True, error);
            Assert.That(session.TryResetGenomeLoadout(profileId, hare, 0, out error), Is.True, error);
            Assert.That(session.Current.ResearchData, Is.EqualTo(40));
            Assert.That(session.Current.RabbitData, Is.EqualTo(20));
            yield return LoadMainMenu();
            session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.TryResetGenomeLoadout(profileId, hare, 0, out _), Is.False);
            Assert.That(session.TryResetGenomeLoadout(profileId, hare, 1, out _), Is.False);
            Assert.That(session.TryPurchaseHareGenomeNode(profileId, "hare.warren-network", out error), Is.True, error);
            Assert.That(session.TryResetGenomeLoadout(profileId, hare, 1, out error), Is.True, error);
            var selected = new[] { "hare.warren-network", "hare.keen-hearing", "hare.swift-digging", "hare.safe-foraging" };
            Assert.That(session.TryArrangeGenomeLoadout(profileId, hare, 2, selected, out error), Is.True, error);
            Assert.That(session.Current.ResearchData, Is.EqualTo(20));
            Assert.That(session.Current.RabbitData, Is.EqualTo(15));
            yield return LoadMainMenu();
            session = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.Current.ProfileId, Is.EqualTo(profileId));
            Assert.That(session.Current.TryGetGenomeProfile(hare, out var genome), Is.True);
            Assert.That(genome.UnlockedNodeIds.Count, Is.EqualTo(5));
            Assert.That(genome.ActiveNodeIds, Is.EqualTo(selected));
            Assert.That(genome.ResetGeneration, Is.EqualTo(2));
            var frozen = FieldDataPermanentProgressionDefinition.CreateStarter().FreezeOwnedLoadout(genome);
            Assert.That(frozen.GetPopulationPoolCapacity(hare), Is.EqualTo(60));
            Assert.That(session.TryArrangeGenomeLoadout(profileId, hare, 2, selected, out error), Is.True, error);
            Assert.That(session.TryArrangeGenomeLoadout(profileId, hare, 2, new[] { selected[0] }, out _), Is.False);
            Assert.That(session.TryResetGenomeLoadout(profileId, hare, 0, out _), Is.False);
            Assert.That(session.TryResetGenomeLoadout(profileId, hare, 1, out _), Is.False);
            Assert.That(session.Current.ResearchData, Is.EqualTo(20));
            Assert.That(session.Current.RabbitData, Is.EqualTo(15));
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator HareJourneyCanLaunchWithoutAPathLicense()
        {
            ClearProfiles();
            yield return LoadMainMenu();
            var viewModel = FindViewModel();
            SetProperty(viewModel, "ProfileNameInput", "Unlicensed launch");
            ExecuteCommand(viewModel, "OpenProfileSelectionCommand");
            ExecuteCommand(viewModel, "CreateInitialProfileCommand");
            var menuSession = GameObject.Find("Main Camera").GetComponent<Helper_ProfileSession>();
            menuSession.SetPlaytimeTracking(false);
            Assert.That(menuSession.Current.UnlockedGenomeSpeciesIds, Is.Empty);
            Assert.That(menuSession.Current.GenomeProfiles, Is.Empty);
            var profileId = menuSession.Current.ProfileId;
            ExecuteCommand(viewModel, "ContinueCommand");
            var deadline = Time.realtimeSinceStartup + 3f;
            while (SceneManager.GetActiveScene().name != DesktopScene && Time.realtimeSinceStartup < deadline)
                yield return null;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(DesktopScene));
            var desktop = GameObject.Find("GalapagOS Desktop Test Camera");
            var session = desktop.GetComponent<Helper_ProfileSession>();
            session.SetPlaytimeTracking(false);
            Assert.That(session.Current.ProfileId, Is.EqualTo(profileId));
            Assert.That(session.Current.UnlockedGenomeSpeciesIds, Is.Empty);
            var preview = Object.FindAnyObjectByType<SpeciesSimulationPreview>();
            Assert.That(preview, Is.Not.Null);
            var openSimulation = GetCommand(desktop.GetComponent("SaltyGame.VM_GalapagOS_Desktop"), "OpenDesktopIconCommand");
            openSimulation.GetType().GetMethod("Execute")?.Invoke(openSimulation, new object[] { "Simulation" });
            Assert.That(preview.ChooseJourneyNode("forest-edge-start"), Is.True);
            Assert.That(preview.ClaimJourneyNodeReward(0), Is.True);
            Assert.That(preview.ChooseJourneyNode("first-cycle"), Is.True);
            deadline = Time.realtimeSinceStartup + 3f;
            while (preview.Run.Tick < 2 && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(preview.Run.Tick, Is.GreaterThanOrEqualTo(2));
            Assert.That(session.Current.UnlockedGenomeSpeciesIds, Is.Empty);
            ClearProfiles();
        }

        [UnityTest]
        public IEnumerator QuitConfirmationIsAVisualState()
        {
            ClearProfiles();
            yield return LoadMainMenu();

            var viewModel = FindViewModel();
            ExecuteCommand(viewModel, "RequestQuitCommand");
            Assert.That(GetVisibility(viewModel, "QuitConfirmationVisibility"), Is.EqualTo("Visible"));

            ExecuteCommand(viewModel, "CancelQuitCommand");
            Assert.That(GetVisibility(viewModel, "QuitConfirmationVisibility"), Is.EqualTo("Collapsed"));
            ClearProfiles();
        }

        static IEnumerator LoadMainMenu()
        {
            yield return SceneManager.LoadSceneAsync(MainMenuScene);
            yield return null;
            yield return null;
        }

        static void ClearProfiles()
        {
            foreach (var session in Object.FindObjectsByType<Helper_ProfileSession>(FindObjectsSortMode.None))
            {
                session.SetPlaytimeTracking(false);
                session.Flush();
            }

            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
        }

        static Component FindViewModel()
        {
            var camera = GameObject.Find("Main Camera");
            return camera == null ? null : camera.GetComponent(ViewModelType);
        }

        static void ExecuteCommand(Component viewModel, string commandProperty)
        {
            var command = GetCommand(viewModel, commandProperty);
            Assert.That(CanExecute(command), Is.True, $"Expected command '{commandProperty}' to be executable.");
            Execute(command);
        }

        static object GetCommand(Component viewModel, string propertyName)
        {
            var value = viewModel.GetType().GetProperty(propertyName)?.GetValue(viewModel);
            Assert.That(value, Is.Not.Null, $"Expected command property '{propertyName}'.");
            return value;
        }

        static bool CanExecute(object command)
        {
            return (bool)command.GetType().GetMethod("CanExecute")?.Invoke(command, new object[] { null });
        }

        static void Execute(object command)
        {
            command.GetType().GetMethod("Execute")?.Invoke(command, new object[] { null });
        }

        static bool GetBool(Component viewModel, string propertyName)
        {
            var value = viewModel.GetType().GetProperty(propertyName)?.GetValue(viewModel);
            Assert.That(value, Is.Not.Null, $"Expected boolean property '{propertyName}'.");
            return (bool)value;
        }

        static string GetString(Component viewModel, string propertyName)
        {
            var value = viewModel.GetType().GetProperty(propertyName)?.GetValue(viewModel) as string;
            Assert.That(value, Is.Not.Null, $"Expected string property '{propertyName}'.");
            return value;
        }

        static string GetVisibility(Component viewModel, string propertyName)
        {
            var value = viewModel.GetType().GetProperty(propertyName)?.GetValue(viewModel);
            Assert.That(value, Is.Not.Null, $"Expected visibility property '{propertyName}'.");
            return value.ToString();
        }

        static void SetProperty(Component viewModel, string propertyName, object value)
        {
            viewModel.GetType().GetProperty(propertyName)?.SetValue(viewModel, value);
        }
    }

    internal sealed class ProfilePlayerPrefsBackup
    {
        readonly bool hasStore;
        readonly string store;
        readonly bool hasRecovery;
        readonly string recovery;

        ProfilePlayerPrefsBackup(bool hasStore, string store, bool hasRecovery, string recovery)
        {
            this.hasStore = hasStore;
            this.store = store;
            this.hasRecovery = hasRecovery;
            this.recovery = recovery;
        }

        public static ProfilePlayerPrefsBackup Capture()
        {
            return new ProfilePlayerPrefsBackup(
                PlayerPrefs.HasKey(Helper_ProfileSession.StoreKey),
                PlayerPrefs.GetString(Helper_ProfileSession.StoreKey),
                PlayerPrefs.HasKey(Helper_ProfileSession.RecoveryStoreKey),
                PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey));
        }

        public void Restore()
        {
            RestoreKey(Helper_ProfileSession.StoreKey, hasStore, store);
            RestoreKey(Helper_ProfileSession.RecoveryStoreKey, hasRecovery, recovery);
            PlayerPrefs.Save();
        }

        static void RestoreKey(string key, bool existed, string value)
        {
            if (existed)
            {
                PlayerPrefs.SetString(key, value);
            }
            else
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
    }
}
