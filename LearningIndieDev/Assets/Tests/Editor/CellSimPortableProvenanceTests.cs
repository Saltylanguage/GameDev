using System;
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
    }
}
