using System.Globalization;

namespace CellSim.Workbench;

internal sealed record ComparisonAxis(string Key, string Values);
internal sealed record SetupParameter(string Key, string Label, string Description, string Argument, string Component, string SnapshotField, bool Integer, decimal Minimum = 0, decimal Maximum = int.MaxValue)
{
    public override string ToString() => Label;
}

// These are runner-supported absolute overrides. The pinned runner still validates resolved rules.
internal static class SetupParameters
{
    internal static readonly SetupParameter[] All = Build();
    static SetupParameter[] Build()
    {
        var result = new List<SetupParameter>();
        foreach (var species in new[] { "plant", "hare", "fox" })
            result.Add(new(species + "-population", Title(species) + " starting population", "Number placed at tick 0. Every combination must fit the map; Hares must start above zero.", "-startingPopulations", species, "population", true, species == "hare" ? 1 : 0));
        result.Add(new("map-width", "Map width", "Number of columns. Changing area changes density even when populations stay fixed.", "-gridWidth", "", "width", true, 1, 4096));
        result.Add(new("map-height", "Map height", "Number of rows. Each width and height value is crossed with all other selected values.", "-gridHeight", "", "height", true, 1, 4096));
        foreach (var species in new[] { "hare", "fox" })
        {
            void Stat(string id, string label, string field, string help, bool integer = true, decimal min = 0, decimal max = int.MaxValue) =>
                result.Add(new(species + ":" + id, Title(species) + " " + label, help, "-speciesStats", species + ":" + id, field, integer, min, max));
            Stat("awareness.vision-range", "vision range", "visionRange", "Perception range in cells. Detection also depends on the species' existing awareness and behavior rules.");
            Stat("energy.starting", "starting energy", "startingEnergy", "Energy assigned at the start. Values must remain consistent with the species' other energy rules.");
            Stat("energy.maximum", "maximum energy", "maximumEnergy", "Energy storage ceiling. The runner rejects values that would be normalized by existing rules.");
            Stat("energy.metabolism", "metabolism", "metabolism", "Base energy cost at each metabolism interval. The interval comes from the frozen scenario.");
            Stat("movement.speed", "movement speed", "movementSpeed", "Base movement speed. Movement and behavior still follow the existing simulation rules.", false);
            Stat("reproduction.chance", "reproduction chance", "reproductionChance", "Probability from 0 to 1 when the existing reproduction requirements are met.", false, 0, 1);
            Stat("reproduction.food-required", "reproduction food requirement", "reproductionFoodRequired", "Food requirement used by the reproduction rules; energy and partner requirements still apply.");
            Stat("reproduction.litter-minimum", "minimum litter size", "litterMinimum", "Smallest litter. It must be at least 1 and no larger than the maximum litter size.", true, 1);
            Stat("reproduction.litter-maximum", "maximum litter size", "litterMaximum", "Largest litter. It must be at least the minimum litter size.", true, 1);
            Stat("combat.attack-modifier", "attack modifier", "attackModifier", "Modifier used by the opposed combat roll. It does not guarantee a hit.", true, int.MinValue);
            Stat("combat.block", "defense", "blockAmount", "Defensive block value used by the combat rules.");
        }
        result.Add(new("plant:resource.wilt-chance", "Plant wilt chance", "Probability from 0 to 1 used by the existing plant wilt rule.", "-speciesStats", "plant:resource.wilt-chance", "wiltChance", false, 0, 1));
        result.Add(new("plant:energy.value", "Plant food energy", "Energy value of a Plant when consumed, under the existing feeding rules.", "-speciesStats", "plant:energy.value", "energyValue", true));
        result.Add(new("hare:resource.seed-drop-chance", "Hare seed drop chance", "Probability from 0 to 1 for the existing seed-dispersal rule; placement constraints still apply.", "-speciesStats", "hare:resource.seed-drop-chance", "seedDropChance", false, 0, 1));
        return result.ToArray();
    }
    static string Title(string text) => char.ToUpperInvariant(text[0]) + text[1..];
    internal static SetupParameter Find(string key) => All.SingleOrDefault(p => p.Key == key) ?? throw new ArgumentException("Unsupported comparison setting: " + key);
    internal static decimal[] Parse(ComparisonAxis axis)
    {
        var parameter = Find(axis.Key);
        if (string.IsNullOrWhiteSpace(axis.Values)) throw new ArgumentException(parameter.Label + ": enter at least one value.");
        var parts = axis.Values.Split(new[] { ',', ';', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length is < 1 or > 20) throw new ArgumentException(parameter.Label + ": enter 1–20 distinct values, separated by commas. Use a decimal point for fractions.");
        var values = parts.Select(text => decimal.TryParse(text, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value) ? value : throw new ArgumentException(parameter.Label + ": invalid number '" + text + "'. Use a decimal point.")).ToArray();
        if (values.Distinct().Count() != values.Length || values.Any(v => v < parameter.Minimum || v > parameter.Maximum || parameter.Integer && decimal.Truncate(v) != v))
            throw new ArgumentException($"{parameter.Label}: use distinct {(parameter.Integer ? "whole-number " : "")}values from {parameter.Minimum} to {parameter.Maximum}.");
        return values;
    }
    internal static string Value(decimal value) => value.ToString(CultureInfo.InvariantCulture);
}
