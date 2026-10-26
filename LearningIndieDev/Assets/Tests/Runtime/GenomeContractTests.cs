using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    [Category("Simulation")]
    public sealed class GenomeContractTests
    {
        static GenomeSimulationSnapshot PopulationGenome(params SpeciesUpgradeModifier[] modifiers)
        {
            var node = new GenomeUpgradeNodeSnapshot("hare.population-test", "Population", "Population fixture",
                GenomeContract.RabbitSpeciesId, modifiers: modifiers);
            return new GenomeSimulationSnapshot(new[] { new SpeciesGenomeSnapshot(GenomeContract.RabbitSpeciesId,
                new[] { node.Id }, new[] { node }) });
        }

        static SpeciesSimulationRunner PopulationPoolRunner(int count, GenomeSimulationSnapshot genome = null)
        {
            var cells = new Grid<SpeciesCell>(80, 1, (x, _) => x < count
                ? new SpeciesCell(GenomeContract.RabbitSpeciesId, health: 1, energy: 24) : SpeciesCell.Empty);
            var run = new SimulationRunState(cells, GenomeContract.RabbitSpeciesId, 871, 0.4f, 4, genome);
            run.ConfigureContinuousPhases(1);
            run.Start();
            run.Advance(cells, 0.1f);
            return new SpeciesSimulationRunner(run, PopulationRules(), 0.1f);
        }

        static Dictionary<SpeciesId, SpeciesRules> PopulationRules()
        {
            var rules = new Dictionary<SpeciesId, SpeciesRules>(SpeciesRuleDefaults.Create());
            rules[GenomeContract.RabbitSpeciesId] = rules[SpeciesIds.Herbivore];
            return rules;
        }

        [Test]
        public void PopulationPoolClampsGrantsRecordsActualAdditionsAndAllowsExplicitExceptions()
        {
            var runner = PopulationPoolRunner(49);
            Assert.That(runner.Run.PopulationPoolCapacity, Is.EqualTo(50));
            Assert.That(runner.TryAddBoundaryPopulation(GenomeContract.RabbitSpeciesId, 10, out var added), Is.True);
            Assert.That(added, Is.EqualTo(1));
            Assert.That(runner.Run.PopulationHistory[1].GetCount(GenomeContract.RabbitSpeciesId), Is.EqualTo(50));
            Assert.That(runner.Run.GetBoundaryPopulationAdded(GenomeContract.RabbitSpeciesId), Is.EqualTo(1));
            Assert.That(runner.TryAddBoundaryPopulation(GenomeContract.RabbitSpeciesId, 1, out added), Is.False);
            Assert.That(added, Is.Zero);
            Assert.That(runner.TryAddBoundaryPopulation(GenomeContract.RabbitSpeciesId, 5, out added, ignorePopulationPool: true), Is.True);
            Assert.That(added, Is.EqualTo(5));
            Assert.That(runner.Run.PopulationPoolSpace, Is.Zero);
            Assert.That(runner.Run.GetBoundaryPopulationAdded(GenomeContract.RabbitSpeciesId), Is.EqualTo(6));
        }

        [Test]
        public void AbovePoolPopulationContinuesWithoutCullingAndNonPlayerAdditionsStayIndependent()
        {
            var runner = PopulationPoolRunner(55);
            Assert.That(runner.TryAddBoundaryPopulation(GenomeContract.RabbitSpeciesId, 1), Is.False);
            Assert.That(runner.TryAddBoundaryPopulation(SpeciesIds.Carnivore, 1), Is.True);
            Assert.That(runner.Run.ContinueWithoutUpgrade(), Is.True);
            Assert.That(runner.Run.PopulationHistory[1].GetCount(GenomeContract.RabbitSpeciesId), Is.EqualTo(55));
        }

        [Test]
        public void GenomePopulationEffectsAreFrozenSeparateFromStatsAndRestoredWithCheckpoint()
        {
            var modifiers = new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.StartingPopulation, 5),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.PopulationPoolCapacity, 10) };
            var genome = PopulationGenome(modifiers);
            var natural = new CellularSimData(80, 1, new Dictionary<SpeciesId, float>(), PopulationRules(),
                0.4f, 0.1f, startingPopulations: new Dictionary<SpeciesId, int> { [GenomeContract.RabbitSpeciesId] = 20 });
            Assert.That(genome.ApplyToStartingPopulations(natural).StartingPopulations[GenomeContract.RabbitSpeciesId], Is.EqualTo(25));
            Assert.That(genome.ApplyToStartingPopulations(natural).StartingPopulations[GenomeContract.RabbitSpeciesId], Is.EqualTo(25));
            Assert.That(natural.StartingPopulations[GenomeContract.RabbitSpeciesId], Is.EqualTo(20));
            Assert.That(genome.ApplyToNaturalRules(natural.SpeciesRules)[GenomeContract.RabbitSpeciesId],
                Is.SameAs(natural.SpeciesRules[GenomeContract.RabbitSpeciesId]));
            modifiers[1] = new SpeciesUpgradeModifier(SpeciesAttributeIds.PopulationPoolCapacity, 30);
            var runner = PopulationPoolRunner(59, genome);
            Assert.That(runner.Run.PopulationPoolCapacity, Is.EqualTo(60));
            Assert.That(runner.Run.CreateCheckpoint().Restore().PopulationPoolCapacity, Is.EqualTo(60));
            Assert.That(runner.TryAddBoundaryPopulation(GenomeContract.RabbitSpeciesId, 10, out var added), Is.True);
            Assert.That(added, Is.EqualTo(1));
        }

        [Test]
        public void MultiplePopulationCapacityNodesAddTenEach()
        {
            var nodes = new[]
            {
                new GenomeUpgradeNodeSnapshot("hare.pool-1", "Pool I", "Capacity", GenomeContract.RabbitSpeciesId,
                    modifiers: new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.PopulationPoolCapacity, 10) }),
                new GenomeUpgradeNodeSnapshot("hare.pool-2", "Pool II", "Capacity", GenomeContract.RabbitSpeciesId,
                    modifiers: new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.PopulationPoolCapacity, 10) }),
            };
            var genome = new GenomeSimulationSnapshot(new[] { new SpeciesGenomeSnapshot(GenomeContract.RabbitSpeciesId,
                new[] { nodes[0].Id, nodes[1].Id }, nodes) });
            Assert.That(genome.GetPopulationPoolCapacity(GenomeContract.RabbitSpeciesId), Is.EqualTo(70));
        }

        [Test]
        public void PopulationModifiersRequireWholeNumbersValidBoundsAndGenomeScope()
        {
            Assert.Throws<System.ArgumentException>(() => PopulationGenome(new SpeciesUpgradeModifier(SpeciesAttributeIds.StartingPopulation, 0.5f)));
            Assert.Throws<System.InvalidOperationException>(() => PopulationGenome(new SpeciesUpgradeModifier(SpeciesAttributeIds.PopulationPoolCapacity, -50))
                .GetPopulationPoolCapacity(GenomeContract.RabbitSpeciesId));
            Assert.Throws<System.InvalidOperationException>(() => PopulationGenome(new SpeciesUpgradeModifier(SpeciesAttributeIds.PopulationPoolCapacity, -50))
                .ApplyToNaturalRules(PopulationRules()));
            Assert.Throws<System.ArgumentException>(() => new SpeciesUpgradeSnapshot("invalid", "Invalid", "Invalid Mutation",
                GenomeContract.RabbitSpeciesId, 0, new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.StartingPopulation, 5) }));
        }

        bool hadProfileSave;
        string originalProfileSave;
        bool hadRecoverySave;
        string originalRecoverySave;

        [SetUp]
        public void PreserveProfileSave()
        {
            hadProfileSave = PlayerPrefs.HasKey(Helper_ProfileSession.StoreKey);
            originalProfileSave = PlayerPrefs.GetString(Helper_ProfileSession.StoreKey);
            hadRecoverySave = PlayerPrefs.HasKey(Helper_ProfileSession.RecoveryStoreKey);
            originalRecoverySave = PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey);
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.DeleteKey(Helper_ProfileSession.RecoveryStoreKey);
            PlayerPrefs.Save();
        }

        [TearDown]
        public void RestoreProfileSave()
        {
            RestoreKey(Helper_ProfileSession.StoreKey, hadProfileSave, originalProfileSave);
            RestoreKey(Helper_ProfileSession.RecoveryStoreKey, hadRecoverySave, originalRecoverySave);
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

        [Test]
        public void GenomeStatsStackAcrossAllSpeciesBeforeMutationsWithoutChangingNaturalRules()
        {
            var hare = SpeciesIds.Herbivore;
            var fox = SpeciesIds.Carnivore;
            var natural = SpeciesRuleDefaults.Create();
            var firstModifiers = new List<SpeciesUpgradeModifier>
            {
                new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, 0.25f),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.DigestionEnergyBonus, 2),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.GestationTicks, -1),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.LitterMaximum, 1),
                new SpeciesUpgradeModifier(SpeciesAttributeIds.LitterMinimum, 1),
            };
            var first = new GenomeUpgradeNodeSnapshot("hare.first", "First", "Fixture", hare, modifiers: firstModifiers);
            var second = StatNode("hare.second", hare, SpeciesAttributeIds.MovementSpeed, 0.5f);
            var inactive = StatNode("hare.inactive", hare, SpeciesAttributeIds.MovementSpeed, 50);
            var predator = StatNode("fox.vision", fox, SpeciesAttributeIds.VisionRange, 4);
            var catalog = new GenomeCatalogSnapshot(new[]
            {
                new SpeciesGenomeMapSnapshot(hare, new[] { first, second, inactive }),
                new SpeciesGenomeMapSnapshot(fox, new[] { predator }),
            });
            var frozen = new GenomeSimulationSnapshot(new[]
            {
                new SpeciesGenomeSnapshot(hare, new[] { first.Id, second.Id }),
                new SpeciesGenomeSnapshot(fox, new[] { predator.Id }),
            }).Resolve(catalog).IncludeSpecies(natural.Keys);
            firstModifiers.Clear();
            var effective = frozen.ApplyToNaturalRules(natural);
            Assert.That(effective[hare].MovementSpeed, Is.EqualTo(natural[hare].MovementSpeed + 0.75f));
            Assert.That(effective[hare].DigestionEnergyBonus, Is.EqualTo(natural[hare].DigestionEnergyBonus + 2));
            Assert.That(effective[hare].GestationTicks, Is.EqualTo(natural[hare].GestationTicks - 1));
            Assert.That(effective[hare].LitterMinimum, Is.EqualTo(natural[hare].LitterMinimum + 1));
            Assert.That(effective[fox].Awareness.VisionRange, Is.EqualTo(natural[fox].Awareness.VisionRange + 4));
            Assert.That(effective[SpeciesIds.Plant], Is.SameAs(natural[SpeciesIds.Plant]));
            Assert.That(natural[hare].MovementSpeed, Is.EqualTo(SpeciesRuleDefaults.Create()[hare].MovementSpeed));

            var progression = new SpeciesProgression(new SpeciesDefinition(hare, effective[hare]));
            var mutation = new SpeciesUpgradeSnapshot("mutation", "Mutation", "Fixture", hare, 0,
                new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.MovementSpeed, -0.25f) });
            Assert.That(progression.TryApplyRunUpgrade(mutation), Is.True);
            Assert.That(progression.Definition.Rules.MovementSpeed, Is.EqualTo(natural[hare].MovementSpeed + 0.75f));
            Assert.That(progression.CurrentRules.MovementSpeed, Is.EqualTo(natural[hare].MovementSpeed + 0.5f));
            Assert.That(progression.AppliedRunUpgrades, Has.Count.EqualTo(1));
            Assert.That(frozen.GetSpeciesGenome(hare).ResolvedNodes[0].Modifiers, Has.Count.EqualTo(5));
        }

        [Test]
        public void ResolvedGenomeFreezesValuesAndExecutionOrderInFingerprint()
        {
            var species = SpeciesIds.Herbivore;
            var first = StatNode("a", species, SpeciesAttributeIds.MovementSpeed, 0.25f);
            var second = StatNode("b", species, SpeciesAttributeIds.VisionRange, 1);
            var catalog = new GenomeCatalogSnapshot(new[] { new SpeciesGenomeMapSnapshot(species, new[] { first, second }) });
            var selection = new GenomeSimulationSnapshot(new[] { new SpeciesGenomeSnapshot(species, new[] { "a", "b" }) });
            var frozen = selection.Resolve(catalog);
            var changed = new GenomeCatalogSnapshot(new[] { new SpeciesGenomeMapSnapshot(species, new[]
            {
                StatNode("a", species, SpeciesAttributeIds.MovementSpeed, 0.75f), second,
            }) });
            var newRun = selection.Resolve(changed);
            var reverse = new GenomeSimulationSnapshot(new[] { new SpeciesGenomeSnapshot(species, new[] { "b", "a" }) }).Resolve(catalog);
            Assert.That(frozen.Fingerprint, Is.Not.EqualTo(newRun.Fingerprint));
            Assert.That(frozen.Fingerprint, Is.Not.EqualTo(reverse.Fingerprint));
            Assert.That(frozen.GetSpeciesGenome(species).ResolvedNodes[0].Modifiers[0].SignedValue, Is.EqualTo(0.25f));
            Assert.That(frozen.GetSpeciesGenome(species).CatalogFingerprint, Is.EqualTo(catalog.Fingerprint));
            var grid = new Grid<SpeciesCell>(4, 4);
            grid.SetCell(1, 1, new SpeciesCell(species, energy: 100));
            var run = new SimulationRunState(grid, species, 1, 1f, targetTicks: 10, activeGenomeSnapshot: frozen);
            run.ConfigureContinuousPhases(1);
            var runner = new SpeciesSimulationRunner(run, frozen.ApplyToNaturalRules(SpeciesRuleDefaults.Create()), 0.1f);
            runner.Start();
            Assert.That(runner.AdvanceOneTick(), Is.True);
            Assert.That(run.CreateCheckpoint().Restore().ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(frozen.Fingerprint));
            Assert.That(SimulationRunResults.Create(run).ActiveGenomeSnapshot, Is.SameAs(frozen));
        }

        [Test]
        public void GenomeRejectsUnknownFractionalDuplicateAndUnresolvedStatModifiers()
        {
            var species = SpeciesIds.Herbivore;
            Assert.Throws<System.ArgumentException>(() => StatNode("bad", species, "missing.stat", 1));
            Assert.Throws<System.ArgumentException>(() => StatNode("bad", species, SpeciesAttributeIds.VisionRange, 0.5f));
            Assert.Throws<System.ArgumentException>(() => new GenomeUpgradeNodeSnapshot("bad", "Bad", "Fixture", species,
                modifiers: new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.VisionRange, 1),
                    new SpeciesUpgradeModifier(SpeciesAttributeIds.VisionRange, 2) }));
            var unresolved = new GenomeSimulationSnapshot(new[] { new SpeciesGenomeSnapshot(species, new[] { "missing" }) });
            Assert.Throws<System.InvalidOperationException>(() => unresolved.Resolve(GenomeCatalogSnapshot.Empty));
            Assert.Throws<System.InvalidOperationException>(() => unresolved.ApplyToNaturalRules(SpeciesRuleDefaults.Create()));
            var parent = StatNode("parent", species, SpeciesAttributeIds.VisionRange, 1);
            var child = new GenomeUpgradeNodeSnapshot("child", "Child", "Fixture", species, new[] { "parent" },
                modifiers: new[] { new SpeciesUpgradeModifier(SpeciesAttributeIds.VisionRange, 1) });
            var catalog = new GenomeCatalogSnapshot(new[] { new SpeciesGenomeMapSnapshot(species, new[] { parent, child }) });
            var missingPrerequisite = new GenomeSimulationSnapshot(new[] { new SpeciesGenomeSnapshot(species, new[] { "child" }) });
            Assert.Throws<System.InvalidOperationException>(() => missingPrerequisite.Resolve(catalog));
            var invalid = StatNode("negative", species, SpeciesAttributeIds.MovementSpeed, -1000);
            Assert.Throws<System.ArgumentOutOfRangeException>(() => invalid.Apply(SpeciesRuleDefaults.Create()[species]));
            var placeholder = new GenomeUpgradeNodeSnapshot("placeholder", "Placeholder", "Metadata only", species);
            Assert.Throws<System.InvalidOperationException>(() => placeholder.Apply(SpeciesRuleDefaults.Create()[species]));
        }

        static GenomeUpgradeNodeSnapshot StatNode(string id, SpeciesId species, string attribute, float amount)
        {
            return new GenomeUpgradeNodeSnapshot(id, id, "Fixture", species,
                modifiers: new[] { new SpeciesUpgradeModifier(attribute, amount) });
        }

        [Test]
        public void ProfileKeepsPermanentUnlocksSeparateFromActiveConfiguration()
        {
            var profile = new SpeciesGenomeProfile(
                SpeciesIds.Herbivore,
                new[] { "hare.guard", "hare.migrate" },
                new[] { "hare.guard" });

            Assert.That(profile.UnlockedNodeIds, Is.EqualTo(new[] { "hare.guard", "hare.migrate" }));
            Assert.That(profile.ActiveNodeIds, Is.EqualTo(new[] { "hare.guard" }));
            Assert.That(GenomeContract.ActiveCapacity, Is.EqualTo(8));
            Assert.That(profile.IsNodeUnlocked("hare.migrate"), Is.True);
            Assert.That(profile.IsNodeActive("hare.migrate"), Is.False);
        }

        [Test]
        public void ProfileRejectsActiveNodeThatIsNotUnlocked()
        {
            Assert.Throws<System.ArgumentException>(() => new SpeciesGenomeProfile(
                SpeciesIds.Herbivore,
                new[] { "hare.guard" },
                new[] { "hare.migrate" }));
        }

        [Test]
        public void ProfileSnapshotLaunchRequestAndRunKeepTheSameGenomeIdentity()
        {
            var profile = new ProfileSessionSnapshot(
                true,
                "profile-1",
                "Test Profile",
                new[]
                {
                    new SpeciesGenomeProfile(
                        SpeciesIds.Herbivore,
                        new[] { "hare.guard", "hare.migrate" },
                        new[] { "hare.migrate" }),
                    new SpeciesGenomeProfile(
                        SpeciesIds.Carnivore,
                        new[] { "fox.stalk" },
                        new[] { "fox.stalk" }),
                });
            var launch = new SimulationLaunchRequest(
                profile.ProfileId,
                "ForestEdge",
                SpeciesIds.Herbivore.Value,
                10100,
                activeGenomeSnapshot: profile.CreateGenomeSnapshot());
            var run = new SimulationRunState(
                new Grid<SpeciesCell>(1, 1),
                SpeciesIds.Herbivore,
                10100,
                1f,
                activeGenomeSnapshot: launch.ActiveGenomeSnapshot);

            Assert.That(launch.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(profile.CreateGenomeSnapshot().Fingerprint));
            Assert.That(run.ActiveGenomeSnapshot.Fingerprint, Is.EqualTo(launch.ActiveGenomeSnapshot.Fingerprint));
            Assert.That(run.ActiveGenomeSnapshot.GetSpeciesGenome(SpeciesIds.Carnivore).ActiveNodeIds,
                Is.EqualTo(new[] { "fox.stalk" }));
            Assert.That(run.ActiveGenomeSnapshot.GetSpeciesGenome(SpeciesIds.Plant).ActiveNodeIds,
                Is.Empty);
            Assert.That(SimulationRunResults.Create(run).ActiveGenomeSnapshot.Fingerprint,
                Is.EqualTo(run.ActiveGenomeSnapshot.Fingerprint));
        }

        [Test]
        public void GenomeSnapshotFingerprintDoesNotDependOnSpeciesOrNodeInputOrder()
        {
            var first = new GenomeSimulationSnapshot(new[]
            {
                new SpeciesGenomeSnapshot(SpeciesIds.Herbivore, new[] { "hare.migrate", "hare.guard" }),
                new SpeciesGenomeSnapshot(SpeciesIds.Carnivore, new[] { "fox.stalk" }),
            });
            var second = new GenomeSimulationSnapshot(new[]
            {
                new SpeciesGenomeSnapshot(SpeciesIds.Carnivore, new[] { "fox.stalk" }),
                new SpeciesGenomeSnapshot(SpeciesIds.Herbivore, new[] { "hare.guard", "hare.migrate" }),
            });

            Assert.That(first.Fingerprint, Is.EqualTo(second.Fingerprint));
        }

        [Test]
        public void GenomeCatalogSnapshotPreservesNodeTopologyWithoutAssetReferences()
        {
            var root = new GenomeUpgradeNodeSnapshot(
                "hare.root",
                "Root",
                "Root node.",
                new SpeciesId("hare"));
            var branch = new GenomeUpgradeNodeSnapshot(
                "hare.branch",
                "Branch",
                "Branch node.",
                new SpeciesId("hare"),
                new[] { "hare.root" });
            var map = new SpeciesGenomeMapSnapshot(
                new SpeciesId("hare"),
                new[] { root, branch });
            var catalog = new GenomeCatalogSnapshot(new[] { map });

            Assert.That(catalog.TryGetSpeciesMap(new SpeciesId("hare"), out var resolvedMap), Is.True);
            Assert.That(resolvedMap.Nodes, Is.EqualTo(new[] { root, branch }));
            Assert.That(resolvedMap.GetNode("hare.branch").PrerequisiteNodeIds,
                Is.EqualTo(new[] { "hare.root" }));
            Assert.That(catalog.Fingerprint, Is.Not.Empty);
        }

        [Test]
        public void GenomeMapRejectsMissingPrerequisitesAndCycles()
        {
            var missingPrerequisite = new GenomeUpgradeNodeSnapshot(
                "hare.branch",
                "Branch",
                "Branch node.",
                new SpeciesId("hare"),
                new[] { "hare.missing" });
            Assert.Throws<System.ArgumentException>(() => new SpeciesGenomeMapSnapshot(
                new SpeciesId("hare"),
                new[] { missingPrerequisite }));

            var first = new GenomeUpgradeNodeSnapshot(
                "hare.first",
                "First",
                "First node.",
                new SpeciesId("hare"),
                new[] { "hare.second" });
            var second = new GenomeUpgradeNodeSnapshot(
                "hare.second",
                "Second",
                "Second node.",
                new SpeciesId("hare"),
                new[] { "hare.first" });
            Assert.Throws<System.ArgumentException>(() => new SpeciesGenomeMapSnapshot(
                new SpeciesId("hare"),
                new[] { first, second }));
        }

        [Test]
        public void ProfileSessionPersistsGenomeConfiguration()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
            GameObject firstObject = null;
            GameObject secondObject = null;
            try
            {
                firstObject = new GameObject("Genome Profile Session");
                var firstSession = firstObject.AddComponent<Helper_ProfileSession>();
                firstSession.CreateInitialProfile("Genome Test");
                Assert.That(firstSession.TrySetGenomeConfiguration(
                    SpeciesIds.Herbivore,
                    new[] { "hare.guard" },
                    new[] { "hare.guard" },
                    out var validationMessage), Is.True, validationMessage);

                secondObject = new GameObject("Reloaded Genome Profile Session");
                var secondSession = secondObject.AddComponent<Helper_ProfileSession>();
                secondSession.Reload();

                Assert.That(secondSession.Current.HasLoadedProfile, Is.True);
                Assert.That(secondSession.Current.TryGetGenomeProfile(
                    SpeciesIds.Herbivore,
                    out var genomeProfile), Is.True);
                Assert.That(genomeProfile.ActiveNodeIds, Is.EqualTo(new[] { "hare.guard" }));
            }
            finally
            {
                if (firstObject != null)
                {
                    Object.DestroyImmediate(firstObject);
                }

                if (secondObject != null)
                {
                    Object.DestroyImmediate(secondObject);
                }

                PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
                PlayerPrefs.Save();
            }
        }

        [Test]
        public void ProfileSessionKeepsCurrencyProgressAndTimeSeparatePerProfile()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
            GameObject firstObject = null;
            GameObject secondObject = null;
            try
            {
                firstObject = new GameObject("First Profile Session");
                var firstSession = firstObject.AddComponent<Helper_ProfileSession>();
                firstSession.SetPlaytimeTracking(false);
                var firstProfileId = firstSession.CreateProfile("First").ProfileId;
                firstSession.AddCurrency(100);
                Assert.That(firstSession.TrySpendCurrency(25), Is.True);
                Assert.That(firstSession.TrySpendCurrency(100), Is.False);
                firstSession.UnlockSpecies(new SpeciesId("fox"));
                firstSession.UnlockGenome(new SpeciesId("hare"));
                firstSession.UnlockFeature("test-feature");
                firstSession.UnlockFeature("test-feature");
                Assert.That(firstSession.TrySetGenomeConfiguration(
                    new SpeciesId("hare"),
                    new[] { "hare.guard" },
                    new[] { "hare.guard" },
                    out var validationMessage), Is.True, validationMessage);
                firstSession.RecordSimulationTicks(600);
                firstSession.RecordPlayedMilliseconds(7200000);
                firstSession.Flush();

                var secondProfileId = firstSession.CreateProfile("Second").ProfileId;
                firstSession.AddCurrency(9);
                firstSession.RecordSimulationTicks(10);
                firstSession.RecordPlayedMilliseconds(1800000);
                firstSession.Flush();

                secondObject = new GameObject("Reloaded Profile Session");
                var reloaded = secondObject.AddComponent<Helper_ProfileSession>();
                reloaded.Reload();
                reloaded.SetPlaytimeTracking(false);
                Assert.That(reloaded.Profiles.Count, Is.EqualTo(2));
                Assert.That(reloaded.Current.ProfileId, Is.EqualTo(secondProfileId));
                Assert.That(reloaded.Current.Currency, Is.EqualTo(9));
                Assert.That(reloaded.Current.SimulationTicks, Is.EqualTo(10));
                Assert.That(reloaded.Current.HoursPlayed, Is.EqualTo(0.5d));
                Assert.That(reloaded.Current.UnlockedFeatureIds, Is.Empty);

                Assert.That(reloaded.SelectProfile(firstProfileId), Is.True);
                Assert.That(reloaded.Current.Currency, Is.EqualTo(75));
                Assert.That(reloaded.Current.SimulationTicks, Is.EqualTo(600));
                Assert.That(reloaded.Current.HoursPlayed, Is.EqualTo(2d));
                Assert.That(reloaded.Current.UnlockedSpeciesIds, Does.Contain(new SpeciesId("hare")));
                Assert.That(reloaded.Current.UnlockedSpeciesIds, Does.Contain(new SpeciesId("fox")));
                Assert.That(reloaded.Current.UnlockedGenomeSpeciesIds, Does.Contain(new SpeciesId("hare")));
                Assert.That(reloaded.Current.UnlockedFeatureIds,
                    Is.EqualTo(new[] { "test-feature" }));
                Assert.That(reloaded.Current.TryGetGenomeProfile(new SpeciesId("hare"),
                    out var genomeProfile), Is.True);
                Assert.That(genomeProfile.UnlockedNodeIds, Is.EqualTo(new[] { "hare.guard" }));
            }
            finally
            {
                if (firstObject != null)
                {
                    Object.DestroyImmediate(firstObject);
                }

                if (secondObject != null)
                {
                    Object.DestroyImmediate(secondObject);
                }

                PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
                PlayerPrefs.Save();
            }
        }

        [Test]
        public void RabbitGenomePurchasesResetAndReloadPreserveTheRightBalances()
        {
            PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
            PlayerPrefs.Save();
            GameObject sessionObject = null;
            GameObject reloadedObject = null;
            try
            {
                sessionObject = new GameObject("Rabbit Genome Economy");
                var session = sessionObject.AddComponent<Helper_ProfileSession>();
                session.SetPlaytimeTracking(false);
                var firstId = session.CreateProfile("Rabbit").ProfileId;
                session.AddRabbitData(100);
                session.AddResearchData(10);
                Assert.That(session.TryPurchaseRabbitGenomeLicense(10, out _), Is.False);
                Assert.That(session.RecordCompletedRun("run-1"), Is.True);
                Assert.That(session.RecordCompletedRun("run-1"), Is.False);
                Assert.That(session.RecordCompletedRun("run-2"), Is.True);
                Assert.That(session.RecordCompletedRun("run-3"), Is.True);
                Assert.That(session.Current.GeneLabUnlocked, Is.True);
                Assert.That(session.TryPurchaseRabbitGenomeLicense(10, out var reason), Is.True, reason);
                Assert.That(session.TryPurchaseRabbitGenomeLicense(10, out _), Is.False);

                var hare = new SpeciesId("hare");
                var root = new GenomeUpgradeNodeSnapshot("hare.root", "Root", "Root", hare,
                    rabbitDataPrice: 20, capacityPoints: 3);
                var branch = new GenomeUpgradeNodeSnapshot("hare.branch", "Branch", "Branch",
                    hare, new[] { "hare.root" }, rabbitDataPrice: 5, capacityPoints: 5);
                var map = new SpeciesGenomeMapSnapshot(hare, new[] { root, branch });
                Assert.That(session.TryPurchaseRabbitGenomeNode(map, branch.Id, 5, 5, out _), Is.False);
                Assert.That(session.TryPurchaseRabbitGenomeNode(map, root.Id, 1, 1, out _), Is.False);
                Assert.That(session.TryPurchaseRabbitGenomeNode(map, root.Id, 20, 3, out reason), Is.True, reason);
                Assert.That(session.TryPurchaseRabbitGenomeNode(map, root.Id, 20, 3, out _), Is.False);
                Assert.That(session.TryPurchaseRabbitGenomeNode(map, branch.Id, 5, 6, out _), Is.False);
                Assert.That(session.TryPurchaseRabbitGenomeNode(map, branch.Id, 5, 5, out reason), Is.True, reason);
                Assert.That(session.Current.RabbitData, Is.EqualTo(65));
                Assert.That(session.Current.TryGetGenomeProfile(hare, out var genome), Is.True);
                Assert.That(genome.CapacityUsed, Is.EqualTo(8));
                Assert.That(genome.Allocations.Count, Is.EqualTo(2));
                Assert.That(session.TrySetGenomeConfiguration(hare, new[] { root.Id },
                    new[] { root.Id }, out _), Is.False);

                var secondId = session.CreateProfile("Second").ProfileId;
                Assert.That(session.Current.RabbitData, Is.Zero);
                Assert.That(session.Current.ResearchData, Is.Zero);
                Assert.That(session.Current.CompletedRuns, Is.Zero);
                Assert.That(session.SelectProfile(firstId), Is.True);

                reloadedObject = new GameObject("Reloaded Rabbit Genome Economy");
                var reloaded = reloadedObject.AddComponent<Helper_ProfileSession>();
                reloaded.Reload();
                reloaded.SetPlaytimeTracking(false);
                Assert.That(reloaded.Current.ProfileId, Is.EqualTo(firstId));
                Assert.That(reloaded.Current.RabbitData, Is.EqualTo(65));
                Assert.That(reloaded.Current.ResearchData, Is.EqualTo(10));
                Assert.That(reloaded.Current.CompletedRuns, Is.EqualTo(3));
                Assert.That(reloaded.RecordCompletedRun("run-1"), Is.False);
                Assert.That(reloaded.Current.CompletedRuns, Is.EqualTo(3));
                Assert.That(reloaded.Current.TryGetGenomeProfile(hare, out genome), Is.True);
                Assert.That(genome.Allocations.Count, Is.EqualTo(2));
                Assert.That(reloaded.TryResetRabbitGenome(11, out _), Is.False);
                Assert.That(reloaded.TryResetRabbitGenome(3, out reason), Is.True, reason);
                Assert.That(reloaded.Current.RabbitData, Is.EqualTo(90));
                Assert.That(reloaded.Current.ResearchData, Is.EqualTo(7));
                Assert.That(reloaded.Current.UnlockedGenomeSpeciesIds, Does.Contain(hare));
                Assert.That(reloaded.Current.TryGetGenomeProfile(hare, out genome), Is.True);
                Assert.That(genome.ActiveNodeIds, Is.Empty);
                Assert.That(reloaded.TryResetRabbitGenome(3, out _), Is.False);
                Assert.That(reloaded.SelectProfile(secondId), Is.True);
                Assert.That(reloaded.Current.RabbitData, Is.Zero);
            }
            finally
            {
                if (sessionObject != null) Object.DestroyImmediate(sessionObject);
                if (reloadedObject != null) Object.DestroyImmediate(reloadedObject);
                PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
                PlayerPrefs.Save();
            }
        }

        [Test]
        public void VersionTwoGenomeSaveKeepsLegacyNodesAndMigratesCurrencyToResearchData()
        {
            const string versionTwo =
                "{\"version\":2,\"profiles\":[{\"id\":\"v2\",\"name\":\"Old\",\"currency\":14,"
                + "\"genomeProfiles\":[{\"speciesId\":\"hare\",\"unlockedNodeIds\":[\"hare.old\"],"
                + "\"activeNodeIds\":[\"hare.old\"]}]}],\"lastLoadedProfileId\":\"v2\"}";
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey, versionTwo);
            PlayerPrefs.Save();
            GameObject sessionObject = null;
            try
            {
                sessionObject = new GameObject("Version Two Genome Profile");
                var session = sessionObject.AddComponent<Helper_ProfileSession>();
                session.Reload();
                session.SetPlaytimeTracking(false);
                Assert.That(session.Current.Currency, Is.EqualTo(14));
                Assert.That(session.Current.RabbitData, Is.Zero);
                Assert.That(session.Current.ResearchData, Is.EqualTo(14));
                Assert.That(session.Current.TryGetGenomeProfile(new SpeciesId("hare"), out var genome), Is.True);
                Assert.That(genome.ActiveNodeIds, Is.EqualTo(new[] { "hare.old" }));
                session.AddRabbitData(2);
                session.Reload();
                Assert.That(session.Current.RabbitData, Is.EqualTo(2));
                Assert.That(session.Current.TryGetGenomeProfile(new SpeciesId("hare"), out genome), Is.True);
                Assert.That(genome.UnlockedNodeIds, Is.EqualTo(new[] { "hare.old" }));
                Assert.That(genome.ActiveNodeIds, Is.EqualTo(new[] { "hare.old" }));
            }
            finally
            {
                if (sessionObject != null) Object.DestroyImmediate(sessionObject);
                PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
                PlayerPrefs.Save();
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void VersionTwoMigrationCanReplayWithoutDuplicatingWalletsOrReplacingRecovery(bool existingRecovery)
        {
            const string legacyJson = "{\"version\":2,\"profiles\":[{\"id\":\"first\",\"name\":\"First\","
                + "\"currency\":73},{\"id\":\"second\",\"name\":\"Second\",\"currency\":19}],"
                + "\"lastLoadedProfileId\":\"second\"}";
            const string priorRecovery = "previous recovery evidence";
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey, legacyJson);
            if (existingRecovery)
            {
                PlayerPrefs.SetString(Helper_ProfileSession.RecoveryStoreKey, priorRecovery);
            }
            else
            {
                PlayerPrefs.DeleteKey(Helper_ProfileSession.RecoveryStoreKey);
            }

            PlayerPrefs.Save();
            var sessionObject = new GameObject("Wallet Migration Session");
            try
            {
                var session = sessionObject.AddComponent<Helper_ProfileSession>();
                session.SetPlaytimeTracking(false);
                session.Reload();
                for (var attempt = 0; attempt < 2; attempt++)
                {
                    Assert.That(session.Profiles.Count, Is.EqualTo(2));
                    Assert.That(session.Current.ProfileId, Is.EqualTo("second"));
                    Assert.That(session.Current.ResearchData, Is.EqualTo(19));
                    Assert.That(session.Current.SpeciesDataWallets, Is.Empty);
                    Assert.That(session.SelectProfile("first"), Is.True);
                    Assert.That(session.Current.ResearchData, Is.EqualTo(73));
                    Assert.That(session.Current.SpeciesDataWallets, Is.Empty);
                    Assert.That(PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey),
                        Is.EqualTo(existingRecovery ? priorRecovery : legacyJson));
                    StringAssert.Contains("\"version\":5", PlayerPrefs.GetString(Helper_ProfileSession.StoreKey));

                    // Replay the old on-disk input as if migration had been interrupted.
                    PlayerPrefs.SetString(Helper_ProfileSession.StoreKey, legacyJson);
                    PlayerPrefs.Save();
                    session.Reload();
                }

                session.Reload();
                Assert.That(session.Current.ResearchData, Is.EqualTo(19));
                var fresh = session.CreateProfile("Fresh");
                Assert.That(fresh.ResearchData, Is.Zero);
                Assert.That(fresh.SpeciesDataWallets, Is.Empty);
                Assert.That(fresh.UnlockedGenomeSpeciesIds, Is.Empty);
                Assert.That(fresh.GenomeProfiles, Is.Empty);
            }
            finally
            {
                Object.DestroyImmediate(sessionObject);
            }
        }

        [Test]
        public void EveryProfileSnapshotCopyPreservesIndependentSpeciesWallets()
        {
            var hare = new SpeciesId("hare");
            var fox = new SpeciesId("fox");
            var wallets = new Dictionary<SpeciesId, long> { { hare, 19 }, { fox, 0 } };
            var original = new ProfileSessionSnapshot(true, "wallets", "Wallets", currency: 42,
                speciesDataWallets: wallets);
            wallets[hare] = 999;
            var genome = new SpeciesGenomeProfile(hare, new[] { "hare.guard" }, new[] { "hare.guard" });
            var copies = new[]
            {
                original.WithLoaded(false),
                original.WithCurrency(43),
                original.WithTime(600, 1234),
                original.WithUnlockedSpecies(fox),
                original.WithUnlockedGenome(hare),
                original.WithUnlockedFeature("gene-lab"),
                original.WithGenomeProfile(genome),
                original.WithGenomeProfile(genome).WithGenomeProfile(genome),
            };
            foreach (var copy in copies)
            {
                Assert.That(copy.SpeciesDataWallets[hare], Is.EqualTo(19));
                Assert.That(copy.SpeciesDataWallets[fox], Is.Zero);
                Assert.That(copy.Currency, Is.EqualTo(copy.ResearchData));
            }

            Assert.That(original.ResearchData, Is.EqualTo(42));
            Assert.Throws<System.NotSupportedException>(() =>
                ((IDictionary<SpeciesId, long>)original.SpeciesDataWallets)[hare] = 100);
        }

        [TestCase("", 0)]
        [TestCase("hare", -1)]
        public void ProfileSnapshotRejectsInvalidSpeciesWallets(string species, long balance)
        {
            var speciesId = string.IsNullOrEmpty(species) ? default : new SpeciesId(species);
            Assert.Throws<System.ArgumentException>(() => new ProfileSessionSnapshot(true, "id", "Name",
                speciesDataWallets: new[] { new KeyValuePair<SpeciesId, long>(speciesId, balance) }));
        }

        [Test]
        public void ProfileSnapshotRejectsDuplicateSpeciesWallets()
        {
            var wallet = new KeyValuePair<SpeciesId, long>(new SpeciesId("hare"), 1);
            Assert.Throws<System.ArgumentException>(() => new ProfileSessionSnapshot(true, "id", "Name",
                speciesDataWallets: new[] { wallet, wallet }));
        }

        [System.Serializable]
        sealed class MigrationArchiveProbe
        {
            public int version;
            public string migrationSourceJson;
        }

        [TestCase(2, 73, 0, 0)]
        [TestCase(30, 73, 19, 11)]
        [TestCase(31, 21, 19, 0)]
        public void CompatibleMigrationPreservesBothV3ShapesAndOriginalJson(
            int shape, long expectedResearch, long expectedHare, long expectedFox)
        {
            var fields = shape == 2 ? "\"currency\":73,"
                : shape == 30 ? "\"researchData\":73,\"speciesDataWallets\":[{\"speciesId\":\"hare\",\"balance\":19},"
                    + "{\"speciesId\":\"fox\",\"balance\":11}],"
                : "\"currency\":14,\"researchData\":7,\"rabbitData\":19,\"completedRuns\":3,\"completedRunIds\":[\"prior-run\"],";
            var allocations = shape == 31
                ? ",\"allocations\":[{\"nodeId\":\"hare.guard\",\"rabbitDataSpent\":1,\"capacityPoints\":2}]" : string.Empty;
            var original = "{\"version\":" + (shape == 2 ? 2 : 3)
                + ",\"profiles\":[{\"id\":\"compat\",\"name\":\"Compatible\"," + fields
                + "\"simulationTicks\":42,\"playTimeMilliseconds\":1234,"
                + "\"unlockedSpeciesIds\":[\"hare\",\"fox\"],\"unlockedGenomeSpeciesIds\":[\"hare\"],"
                + "\"genomeProfiles\":[{\"speciesId\":\"hare\",\"unlockedNodeIds\":[\"hare.guard\"],"
                + "\"activeNodeIds\":[\"hare.guard\"]" + allocations + "}]}],\"lastLoadedProfileId\":\"compat\"}";
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey, original);
            PlayerPrefs.SetString(Helper_ProfileSession.RecoveryStoreKey, "older recovery");
            PlayerPrefs.Save();
            var sessionObject = new GameObject("Compatible Wallet Migration");
            try
            {
                var session = sessionObject.AddComponent<Helper_ProfileSession>();
                session.SetPlaytimeTracking(false);
                session.Reload();
                for (var attempt = 0; attempt < 2; attempt++)
                {
                    Assert.That(session.Current.ResearchData, Is.EqualTo(expectedResearch));
                    Assert.That(session.Current.Currency, Is.EqualTo(expectedResearch));
                    Assert.That(session.Current.RabbitData, Is.EqualTo(expectedHare));
                    Assert.That(session.Current.SpeciesDataWallets.TryGetValue(new SpeciesId("fox"), out var fox)
                        ? fox : 0, Is.EqualTo(expectedFox));
                    Assert.That(session.Current.SimulationTicks, Is.EqualTo(42));
                    Assert.That(session.Current.PlayTimeMilliseconds, Is.EqualTo(1234));
                    Assert.That(session.Current.TryGetGenomeProfile(new SpeciesId("hare"), out var genome), Is.True);
                    CollectionAssert.AreEqual(new[] { "hare.guard" }, genome.UnlockedNodeIds);
                    CollectionAssert.AreEqual(new[] { "hare.guard" }, genome.ActiveNodeIds);
                    Assert.That(genome.Allocations.Count, Is.EqualTo(shape == 31 ? 1 : 0));
                    Assert.That(session.Current.CompletedRuns, Is.EqualTo(shape == 31 ? 3 : 0));
                    if (shape == 31)
                    {
                        CollectionAssert.AreEqual(new[] { "prior-run" }, session.Current.CompletedRunIds);
                        Assert.That(genome.Allocations[0].RabbitDataSpent, Is.EqualTo(1));
                    }

                    var probe = JsonUtility.FromJson<MigrationArchiveProbe>(PlayerPrefs.GetString(Helper_ProfileSession.StoreKey));
                    Assert.That(probe.version, Is.EqualTo(5));
                    Assert.That(probe.migrationSourceJson, Is.EqualTo(original));
                    Assert.That(PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey), Is.EqualTo("older recovery"));
                    session.Reload();
                }

                session.AddCurrency(1);
                session.AddResearchData(2);
                session.AddRabbitData(3);
                session.Reload();
                Assert.That(session.Current.ResearchData, Is.EqualTo(expectedResearch + 3));
                Assert.That(session.Current.RabbitData, Is.EqualTo(expectedHare + 3));
                Assert.That(session.Current.SpeciesDataWallets.TryGetValue(new SpeciesId("fox"), out var finalFox)
                    ? finalFox : 0, Is.EqualTo(expectedFox));
                Assert.That(JsonUtility.FromJson<MigrationArchiveProbe>(PlayerPrefs.GetString(Helper_ProfileSession.StoreKey))
                    .migrationSourceJson, Is.EqualTo(original));
            }
            finally
            {
                Object.DestroyImmediate(sessionObject);
            }
        }

        [TestCase("\"researchData\":2,\"rabbitData\":1,\"speciesDataWallets\":[]")]
        [TestCase("\"currency\":9223372036854775807,\"researchData\":1")]
        [TestCase("\"researchData\":2,\"speciesDataWallets\":[{\"speciesId\":\"hare\",\"balance\":-1}]")]
        public void AmbiguousOverflowingOrInvalidWalletMigrationKeepsOriginalForRecovery(string fields)
        {
            var original = "{\"version\":3,\"profiles\":[{\"id\":\"invalid\"," + fields
                + "}],\"lastLoadedProfileId\":\"invalid\"}";
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey, original);
            PlayerPrefs.SetString(Helper_ProfileSession.RecoveryStoreKey, "older recovery");
            PlayerPrefs.Save();
            var sessionObject = new GameObject("Rejected Wallet Migration");
            try
            {
                var session = sessionObject.AddComponent<Helper_ProfileSession>();
                session.Reload();
                session.SetPlaytimeTracking(false);
                Assert.That(session.HasProfiles, Is.False);
                Assert.That(session.LoadWarning, Is.Not.Empty);
                Assert.That(PlayerPrefs.GetString(Helper_ProfileSession.StoreKey), Is.EqualTo(original));
                Assert.That(PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey), Is.EqualTo("older recovery"));
            }
            finally
            {
                Object.DestroyImmediate(sessionObject);
            }
        }

        [Test]
        public void ProfileSessionLoadsLegacyProfileWithoutLosingItsIdentity()
        {
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey,
                "{\"profiles\":[{\"id\":\"legacy\",\"name\":\"Legacy\",\"genomeProfiles\":[]}],\"lastLoadedProfileId\":\"legacy\"}");
            PlayerPrefs.Save();
            GameObject sessionObject = null;
            try
            {
                sessionObject = new GameObject("Legacy Profile Session");
                var session = sessionObject.AddComponent<Helper_ProfileSession>();
                session.Reload();
                session.SetPlaytimeTracking(false);
                Assert.That(session.Current.ProfileId, Is.EqualTo("legacy"));
                Assert.That(session.Current.Currency, Is.Zero);
                Assert.That(session.Current.SimulationTicks, Is.Zero);
                Assert.That(session.Current.PlayTimeMilliseconds, Is.Zero);
                Assert.That(session.Current.UnlockedSpeciesIds,
                    Does.Contain(new SpeciesId("hare")));
                Assert.That(session.Current.UnlockedFeatureIds, Is.Empty);
            }
            finally
            {
                if (sessionObject != null)
                {
                    Object.DestroyImmediate(sessionObject);
                }

                PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
                PlayerPrefs.Save();
            }
        }

        [Test]
        public void ProfileSessionPreservesUnreadableSaveForRecovery()
        {
            const string invalidJson = "not a profile store";
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey, invalidJson);
            PlayerPrefs.DeleteKey(Helper_ProfileSession.RecoveryStoreKey);
            PlayerPrefs.Save();
            GameObject sessionObject = null;
            try
            {
                sessionObject = new GameObject("Corrupt Profile Session");
                var session = sessionObject.AddComponent<Helper_ProfileSession>();
                session.Reload();
                session.SetPlaytimeTracking(false);
                Assert.That(session.HasProfiles, Is.False);
                Assert.That(session.LoadWarning, Is.Not.Empty);
                Assert.That(PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey),
                    Is.EqualTo(invalidJson));
                Assert.That(session.CreateProfile("New Profile").HasLoadedProfile, Is.True);
                Assert.That(PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey),
                    Is.EqualTo(invalidJson));
            }
            finally
            {
                if (sessionObject != null)
                {
                    Object.DestroyImmediate(sessionObject);
                }

                PlayerPrefs.DeleteKey(Helper_ProfileSession.StoreKey);
                PlayerPrefs.DeleteKey(Helper_ProfileSession.RecoveryStoreKey);
                PlayerPrefs.Save();
            }
        }

        [Test]
        public void ProfileSessionKeepsValidProfileWhenAnotherRecordIsInvalid()
        {
            const string partiallyInvalidJson =
                "{\"version\":1,\"profiles\":[{\"id\":\"valid\",\"name\":\"Kept\",\"currency\":7},"
                + "{\"id\":\"invalid\",\"name\":\"Discarded\",\"currency\":-1}],"
                + "\"lastLoadedProfileId\":\"valid\"}";
            PlayerPrefs.SetString(Helper_ProfileSession.StoreKey, partiallyInvalidJson);
            PlayerPrefs.Save();
            GameObject sessionObject = null;
            try
            {
                sessionObject = new GameObject("Partial Profile Session");
                var session = sessionObject.AddComponent<Helper_ProfileSession>();
                session.Reload();
                session.SetPlaytimeTracking(false);

                Assert.That(session.Current.ProfileId, Is.EqualTo("valid"));
                Assert.That(session.Current.Currency, Is.EqualTo(7));
                Assert.That(session.Profiles.Count, Is.EqualTo(1));
                Assert.That(session.LoadWarning, Is.Not.Empty);
                Assert.That(PlayerPrefs.GetString(Helper_ProfileSession.RecoveryStoreKey),
                    Is.EqualTo(partiallyInvalidJson));
            }
            finally
            {
                if (sessionObject != null)
                {
                    Object.DestroyImmediate(sessionObject);
                }
            }
        }
    }
}
