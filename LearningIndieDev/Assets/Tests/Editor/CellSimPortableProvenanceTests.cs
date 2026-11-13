using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.EditorTests
{
    public sealed class CellSimPortableProvenanceTests
    {
        [Test]
        public void PortableManifestCoversEveryBatchSharedSource()
        {
            var projectRoot = Directory.GetParent(Application.dataPath).FullName;
            var projectDirectory = Path.Combine(projectRoot, "tools", "CellSim.Batch");
            var project = XDocument.Load(Path.Combine(projectDirectory, "CellSim.Batch.csproj"));
            var sharedSources = project.Descendants("SharedSource")
                .Select(element => (string)element.Attribute("Include"))
                .Select(include => Path.GetRelativePath(projectRoot,
                    Path.GetFullPath(Path.Combine(projectDirectory, include))).Replace('\\', '/'));
            var portableSources = GetPortableSourceFiles();

            foreach (var source in sharedSources)
            {
                Assert.That(portableSources, Does.Contain(source),
                    "CellSim.Batch compiles this source, so portable exports must fingerprint it: " + source);
            }
        }

        [Test]
        public void ChangingOnlyPopulationInterventionChangesPortableSourceHash()
        {
            const string target = "Assets/Scripts/Game/Simulation/PopulationIntervention.cs";
            Assert.That(GetPortableSourceFiles(), Does.Contain(target));

            var before = GetPortableSourceHash(path =>
                path == target ? "population source before" : "unchanged source");
            var after = GetPortableSourceHash(path =>
                path == target ? "population source after" : "unchanged source");

            Assert.That(after, Is.Not.EqualTo(before));
        }

        [Test]
        public void PortableRunDigestIncludesHiddenHareAndBurrowState()
        {
            var hare = new SpeciesId("hare");
            var fox = new SpeciesId("fox");
            var noMovement = new GridPattern(Array.Empty<UnityEngine.Vector2Int>());
            var hareRules = new SpeciesRules(movementSpeed: 0f, movementPattern: noMovement,
                attackPattern: noMovement, attackAmount: 0, blockPattern: noMovement, blockAmount: 0,
                dietPattern: noMovement, dietTarget: null, reproductionPattern: noMovement,
                reproductionNeighborCount: 0, reproductionChance: 0f, reproductionFoodRequired: 1,
                metabolism: 0, role: SpeciesRole.Herbivore);
            var foxRules = new SpeciesRules(movementSpeed: 0f, movementPattern: noMovement,
                attackPattern: GridPatternTemplates.CreateMooreRange(1), attackAmount: 100,
                blockPattern: noMovement, blockAmount: 0, dietPattern: noMovement, dietTarget: hare,
                reproductionPattern: noMovement, reproductionNeighborCount: 0,
                reproductionChance: 0f, reproductionFoodRequired: 1, metabolism: 0,
                role: SpeciesRole.Carnivore);
            var cells = new Grid<SpeciesCell>(7, 7);
            cells.SetCell(3, 3, new SpeciesCell(hare, energy: 48));
            var run = new SimulationRunState(cells, hare, seed: 75, durationSeconds: 100f);
            run.ConfigureContinuousPhases(10);
            var rules = new Dictionary<SpeciesId, SpeciesRules> { [hare] = hareRules, [fox] = foxRules };
            var burrowing = SpeciesUpgradeCatalog.ResolveFieldDataOrdinaryRewardPool(hare,
                    Array.Empty<SpeciesUpgradeSnapshot>(), id => id == SpeciesUpgradeCatalog.BurrowingId)
                .Single(upgrade => upgrade.Id == SpeciesUpgradeCatalog.BurrowingId);
            var simulation = new SpeciesSimulationRunner(run, rules, 0.1f,
                upgradeLoadout: new[] { burrowing });

            Assert.That(simulation.AdvanceOneTick(), Is.True);
            run.Cells.SetCell(4, 3, new SpeciesCell(fox, energy: 100));
            Assert.That(simulation.AdvanceOneTick(), Is.True);
            Assert.That(run.HiddenHareCount, Is.EqualTo(1));

            // Keep the visible grid identical while omitting simulation-owned
            // Burrow state; only the portable hidden-state contribution differs.
            var surfaceOnly = new SimulationRunState(run.Cells.Copy(), hare, seed: 75, durationSeconds: 100f);
            var withBurrowDigest = GetPortableRunStateDigest(run);
            var surfaceOnlyDigest = GetPortableRunStateDigest(surfaceOnly);

            Assert.That(withBurrowDigest, Is.Not.EqualTo(surfaceOnlyDigest));
        }

        static Type GetPortableRunnerType()
        {
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(
                    "SaltyGame.EditorTools.CellularSimulationExperimentRunner", false))
                .FirstOrDefault(candidate => candidate != null);
            Assert.That(type, Is.Not.Null, "The Editor portable runner must be loaded for these tests.");
            return type;
        }

        static string[] GetPortableSourceFiles() => (string[])GetPortableRunnerType()
            .GetField("PortableSourceFiles", BindingFlags.Public | BindingFlags.Static)
            .GetValue(null);

        static string GetPortableSourceHash(Func<string, string> readSource) => (string)GetPortableRunnerType()
            .GetMethod("PortableSourceHash", BindingFlags.Public | BindingFlags.Static)
            .Invoke(null, new object[] { readSource });

        static string GetPortableRunStateDigest(SimulationRunState run) => (string)GetPortableRunnerType()
            .GetMethod("PortableRunStateDigest", BindingFlags.Public | BindingFlags.Static)
            .Invoke(null, new object[] { run });
    }
}
