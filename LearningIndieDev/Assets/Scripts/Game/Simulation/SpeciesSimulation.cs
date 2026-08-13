using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public static class SpeciesSimulation
    {
        public static Grid<SpeciesCell> Step(
            Grid<SpeciesCell> source,
            CellularSimData simulationData,
            int seed,
            SpeciesSimulationMetrics metrics = null)
        {
            if (simulationData == null)
            {
                throw new ArgumentNullException(nameof(simulationData));
            }

            return Step(
                source,
                simulationData.SpeciesRules,
                seed,
                simulationData.MaxPopulation,
                simulationData.TerrainDefinitions,
                simulationData.AlphaOffspringRules,
                metrics);
        }

        public static Grid<SpeciesCell> Step(
            Grid<SpeciesCell> source,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            int seed,
            int maxPopulation = 0,
            IReadOnlyDictionary<TerrainId, TerrainDefinition> terrainDefinitions = null,
            IReadOnlyDictionary<SpeciesId, AlphaOffspringRule> alphaOffspringRules = null,
            SpeciesSimulationMetrics metrics = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            terrainDefinitions ??= TerrainDefaults.Create();
            if (!terrainDefinitions.ContainsKey(TerrainIds.Grass))
            {
                throw new ArgumentException("Terrain definitions must include the grass terrain id.", nameof(terrainDefinitions));
            }

            var next = source.Copy();
            var random = new System.Random(seed);
            ResolveAttacks(source, next, rules, random, metrics);
            ResolveMovement(source, next, rules, random, metrics);
            ResolveMetabolism(next, rules);
            ResolveStarvation(next, rules, metrics);
            ResolveCrowdingStress(next, rules, metrics);
            ResolveSeedDrops(next, rules, terrainDefinitions, random, metrics);
            ResolveWilt(next, rules, random, metrics);
            ResolveReproduction(next, rules, terrainDefinitions, alphaOffspringRules, random, metrics);
            ResolvePopulationLimit(next, maxPopulation, random, metrics);
            return next;
        }

        [Obsolete("Use the SpeciesId overload instead.")]
        public static Grid<SpeciesCell> Step(
            Grid<SpeciesCell> source,
            IReadOnlyDictionary<SpeciesArchetype, SpeciesRules> rules,
            int seed,
            int maxPopulation = 0,
            SpeciesSimulationMetrics metrics = null)
        {
            return Step(source, SpeciesIdConversions.FromLegacy(rules), seed, maxPopulation, metrics: metrics);
        }

        static void ResolvePopulationLimit(
            Grid<SpeciesCell> next,
            int maxPopulation,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            if (maxPopulation <= 0)
            {
                return;
            }

            var occupied = new List<int>();
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    if (next.GetCell(x, y).IsCreature || next.GetCell(x, y).IsPlantResource)
                    {
                        occupied.Add(GetIndex(next, x, y));
                    }
                }
            }

            while (occupied.Count > maxPopulation)
            {
                var removeIndex = random.Next(occupied.Count);
                var cellIndex = occupied[removeIndex];
                occupied.RemoveAt(removeIndex);
                var x = cellIndex % next.Width;
                var y = cellIndex / next.Width;
                var cell = next.GetCell(x, y);
                next.SetCell(x, y, cell.IsPlantResource
                    ? cell.WithoutPlantResource()
                    : cell.WithoutEntity());
                metrics?.Record(cell.SpeciesId, deaths: 1, populationLimitRemovals: 1);
            }
        }

        static void ResolveAttacks(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var attacker = source.GetCell(x, y);
                    if (!attacker.IsCreature
                        || !rules.TryGetValue(attacker.SpeciesId, out var attackerRules)
                        || attackerRules.AttackAmount <= 0
                        || !next.GetCell(x, y).IsCreature)
                    {
                        continue;
                    }

                    var attackPattern = attackerRules.AttackPattern;
                    var startOffset = attackPattern.Count == 0 ? 0 : random.Next(attackPattern.Count);
                    for (var offsetIndex = 0; offsetIndex < attackPattern.Count; offsetIndex++)
                    {
                        var offset = attackPattern.Offsets[(startOffset + offsetIndex) % attackPattern.Count];
                        var targetX = x + offset.x;
                        var targetY = y + offset.y;
                        if (!source.TryGetCell(targetX, targetY, out var target)
                            || !attackerRules.DietTargetId.HasValue
                            || !SpeciesPerception.IsDietTarget(target, attackerRules.DietTargetId.Value))
                        {
                            continue;
                        }

                        var currentTarget = target;
                        if (target.IsCreature
                            && (!next.TryGetCell(targetX, targetY, out currentTarget)
                                || !currentTarget.IsCreature))
                        {
                            continue;
                        }

                        var damage = attackerRules.AttackAmount;
                        if (rules.TryGetValue(target.SpeciesId, out var targetRules)
                            && ContainsOffset(targetRules.BlockPattern, new Vector2Int(-offset.x, -offset.y)))
                        {
                            damage = Math.Max(0, damage - targetRules.BlockAmount);
                        }

                        var currentAttacker = next.GetCell(x, y);
                        if (!currentAttacker.IsCreature || currentAttacker.SpeciesId != attacker.SpeciesId)
                        {
                            break;
                        }

                        if (target.IsPlantResource)
                        {
                            TryFeedOnPlant(
                                next,
                                targetX,
                                targetY,
                                x,
                                y,
                                currentAttacker,
                                attackerRules,
                                rules.TryGetValue(target.SpeciesId, out var foodRules)
                                    ? foodRules.EnergyValue
                                    : 1,
                                metrics);
                            break;
                        }

                        if (damage > 0 && currentTarget.IsCreature)
                        {
                            var remainingHealth = currentTarget.Health - damage;
                            metrics?.Record(
                                attacker.SpeciesId,
                                damageDealt: Math.Min(damage, currentTarget.Health));
                            next.SetCell(targetX, targetY, remainingHealth > 0
                                ? currentTarget.WithEntity(currentTarget.SpeciesId, remainingHealth, currentTarget.Energy, currentTarget.Age, currentTarget.FoodEaten, currentTarget.FoodReserve, currentTarget.IsAlpha)
                                : currentTarget.WithoutEntity());

                            if (remainingHealth <= 0)
                            {
                                metrics?.Record(target.SpeciesId, deaths: 1);
                                metrics?.Record(attacker.SpeciesId, combatKills: 1);
                            }

                            if (remainingHealth <= 0
                                && attackerRules.StartingEnergy > 0
                                && currentAttacker.IsCreature
                                && currentAttacker.SpeciesId == attacker.SpeciesId)
                            {
                                next.SetCell(x, y, CreateFedCell(
                                    currentAttacker,
                                    attackerRules,
                                    rules.TryGetValue(target.SpeciesId, out var foodRules)
                                        ? foodRules.EnergyValue
                                        : 0));
                                metrics?.Record(attacker.SpeciesId, foodConsumed: 1f);
                            }
                        }

                        break;
                    }
                }
            }
        }

        static void ResolveMovement(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            var movementPasses = 1;
            foreach (var speciesRules in rules.Values)
            {
                movementPasses = Math.Max(movementPasses, (int)Math.Ceiling(speciesRules.MovementSpeed));
            }

            for (var pass = 0; pass < movementPasses; pass++)
            {
                var movementSource = pass == 0 ? source : next.Copy();
                ResolveMovementPass(movementSource, next, rules, pass, random, metrics);
            }
        }

        static void ResolveMovementPass(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            int movementPass,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            var moved = new bool[source.Count];
            var claimed = new bool[source.Count];
            var plantEnergyValue = GetFirstPlantEnergyValue(rules);
            var processingOrder = CreateShuffledIndices(source.Count, random);

            for (var orderIndex = 0; orderIndex < processingOrder.Length; orderIndex++)
            {
                var sourceIndex = processingOrder[orderIndex];
                var x = sourceIndex % source.Width;
                var y = sourceIndex / source.Width;
                var sourceCell = source.GetCell(x, y);
                var currentCell = next.GetCell(x, y);
                if (moved[sourceIndex]
                    || !sourceCell.IsCreature
                    || !currentCell.IsCreature
                    || !rules.TryGetValue(sourceCell.SpeciesId, out var speciesRules)
                    || speciesRules.MovementSpeed <= movementPass
                    || currentCell.SpeciesId != sourceCell.SpeciesId)
                {
                    continue;
                }

                if (TryResolveVisionMovement(
                    source,
                    next,
                    x,
                    y,
                    sourceCell,
                    currentCell,
                    speciesRules,
                    plantEnergyValue,
                    moved,
                    claimed,
                    random,
                    metrics))
                {
                    continue;
                }

                if (speciesRules.DietTargetId.HasValue
                    && currentCell.FoodReserve <= sourceCell.FoodReserve
                    && TryMove(
                        source,
                        next,
                        x,
                        y,
                        currentCell,
                        speciesRules,
                        speciesRules.DietPattern,
                        plantEnergyValue,
                        requireDietTarget: true,
                        moved,
                        claimed,
                        random,
                        metrics))
                {
                    continue;
                }

                if (speciesRules.ReproductionNeighborCount > 0
                    && CountPatternSpeciesNeighbors(
                        source,
                        x,
                        y,
                        sourceCell.SpeciesId,
                        speciesRules.ReproductionPattern,
                        excludeX: -1,
                        excludeY: -1) < speciesRules.ReproductionNeighborCount
                    && TryMoveTowardMate(
                        source,
                        next,
                        x,
                        y,
                        currentCell,
                        speciesRules,
                        moved,
                        claimed,
                        random,
                        metrics))
                {
                    continue;
                }

                TryMove(
                    source,
                    next,
                    x,
                    y,
                    currentCell,
                    speciesRules,
                    speciesRules.MovementPattern,
                    plantEnergyValue,
                    requireDietTarget: false,
                    moved,
                    claimed,
                    random,
                    metrics);
            }
        }

        static bool TryMove(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesCell cell,
            SpeciesRules speciesRules,
            GridPattern pattern,
            int plantEnergyValue,
            bool requireDietTarget,
            bool[] moved,
            bool[] claimed,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            var bestX = -1;
            var bestY = -1;
            var bestCrowding = int.MaxValue;

            var startOffset = pattern.Count == 0 ? 0 : random.Next(pattern.Count);
            for (var offsetIndex = 0; offsetIndex < pattern.Count; offsetIndex++)
            {
                var offset = pattern.Offsets[(startOffset + offsetIndex) % pattern.Count];
                var targetX = x + offset.x;
                var targetY = y + offset.y;
                if (!source.IsInBounds(targetX, targetY))
                {
                    continue;
                }

                var targetIndex = GetIndex(source, targetX, targetY);
                if (claimed[targetIndex])
                {
                    continue;
                }

                var sourceTarget = source.GetCell(targetX, targetY);
                if (!sourceTarget.IsPassable)
                {
                    continue;
                }

                var isDietTarget = speciesRules.DietTargetId.HasValue
                    && SpeciesPerception.IsDietTarget(sourceTarget, speciesRules.DietTargetId.Value);
                if (requireDietTarget && !isDietTarget)
                {
                    continue;
                }

                var nextTarget = next.GetCell(targetX, targetY);
                if (nextTarget.IsCreature && !isDietTarget)
                {
                    continue;
                }

                if (isDietTarget && sourceTarget.IsCreature)
                {
                    continue;
                }

                if (requireDietTarget)
                {
                    bestX = targetX;
                    bestY = targetY;
                    break;
                }

                var crowding = CountNearbySpecies(source, targetX, targetY, cell.SpeciesId, x, y);
                if (speciesRules.MaxReproductionGroupSize > 0
                    && crowding + 1 > speciesRules.MaxReproductionGroupSize)
                {
                    continue;
                }

                if (crowding < bestCrowding)
                {
                    bestX = targetX;
                    bestY = targetY;
                    bestCrowding = crowding;
                }
            }

            if (bestX < 0)
            {
                return false;
            }

            return TryMoveTo(
                source,
                next,
                x,
                y,
                cell,
                speciesRules,
                bestX,
                bestY,
                plantEnergyValue,
                moved,
                claimed,
                metrics);
        }

        static bool TryResolveVisionMovement(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesCell sourceCell,
            SpeciesCell currentCell,
            SpeciesRules speciesRules,
            int plantEnergyValue,
            bool[] moved,
            bool[] claimed,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            if (speciesRules.Awareness.VisionRange <= 0)
            {
                return false;
            }

            var foodTarget = default(SpeciesPerceivedTarget);
            var hasFood = speciesRules.DietTargetId.HasValue
                && currentCell.FoodReserve <= sourceCell.FoodReserve
                && SpeciesPerception.TryFindFoodTarget(source, x, y, speciesRules, random, out foodTarget);
            var canSeekMate = speciesRules.ReproductionChance > 0f
                && speciesRules.ReproductionNeighborCount > 0
                && GetReproductionEnergy(currentCell) > speciesRules.ReproductionFoodRequired
                && CountPatternSpeciesNeighbors(
                    source,
                    x,
                    y,
                    currentCell.SpeciesId,
                    speciesRules.ReproductionPattern,
                    excludeX: -1,
                    excludeY: -1) < speciesRules.ReproductionNeighborCount;
            var mateTarget = default(SpeciesPerceivedTarget);
            var hasMate = canSeekMate
                && SpeciesPerception.TryFindMateTarget(
                    source,
                    x,
                    y,
                    currentCell.SpeciesId,
                    speciesRules,
                    random,
                    out mateTarget);
            var prioritizeMate = hasMate
                && speciesRules.Awareness.Intelligence > 0
                && GetReproductionEnergy(currentCell) > speciesRules.ReproductionFoodRequired;

            if (prioritizeMate
                && TryMoveTowardPerceivedTarget(
                    source,
                    next,
                    x,
                    y,
                    currentCell,
                    speciesRules,
                    mateTarget,
                    plantEnergyValue,
                    moved,
                    claimed,
                    random,
                    metrics))
            {
                return true;
            }

            if (hasFood
                && TryMoveTowardPerceivedTarget(
                    source,
                    next,
                    x,
                    y,
                    currentCell,
                    speciesRules,
                    foodTarget,
                    plantEnergyValue,
                    moved,
                    claimed,
                    random,
                    metrics))
            {
                return true;
            }

            return !prioritizeMate
                && hasMate
                && TryMoveTowardPerceivedTarget(
                    source,
                    next,
                    x,
                    y,
                    currentCell,
                    speciesRules,
                    mateTarget,
                    plantEnergyValue,
                    moved,
                    claimed,
                    random,
                    metrics);
        }

        static bool TryMoveTowardPerceivedTarget(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesCell cell,
            SpeciesRules speciesRules,
            SpeciesPerceivedTarget target,
            int plantEnergyValue,
            bool[] moved,
            bool[] claimed,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            var interactionPattern = target.Intent == SpeciesMovementIntent.Mate
                ? speciesRules.ReproductionPattern
                : target.Cell.IsCreature
                    ? speciesRules.AttackPattern
                    : speciesRules.DietPattern;
            if (!SpeciesNavigation.TryFindNextStep(
                source,
                new Vector2Int(x, y),
                target.Location,
                speciesRules.MovementPattern,
                interactionPattern,
                random,
                out var nextStep))
            {
                return false;
            }

            return TryMoveTo(
                source,
                next,
                x,
                y,
                cell,
                speciesRules,
                nextStep.x,
                nextStep.y,
                plantEnergyValue,
                moved,
                claimed,
                metrics);
        }

        static bool TryMoveTo(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesCell cell,
            SpeciesRules speciesRules,
            int targetX,
            int targetY,
            int plantEnergyValue,
            bool[] moved,
            bool[] claimed,
            SpeciesSimulationMetrics metrics)
        {
            if (!source.IsInBounds(targetX, targetY))
            {
                return false;
            }

            var targetIndex = GetIndex(source, targetX, targetY);
            var sourceTarget = source.GetCell(targetX, targetY);
            var currentTarget = next.GetCell(targetX, targetY);
            if (claimed[targetIndex]
                || !sourceTarget.IsPassable
                || sourceTarget.IsCreature
                || currentTarget.IsCreature)
            {
                return false;
            }

            if (speciesRules.DietTargetId.HasValue
                && SpeciesPerception.IsDietTarget(sourceTarget, speciesRules.DietTargetId.Value))
            {
                if (!TryFeedOnPlant(next, targetX, targetY, x, y, cell, speciesRules, plantEnergyValue, metrics))
                {
                    return false;
                }

                moved[GetIndex(source, x, y)] = true;
                return true;
            }

            var crowding = CountNearbySpecies(source, targetX, targetY, cell.SpeciesId, x, y);
            if (speciesRules.MaxReproductionGroupSize > 0
                && crowding + 1 > speciesRules.MaxReproductionGroupSize)
            {
                return false;
            }

            next.SetCell(x, y, source.GetCell(x, y).WithoutEntity());
            next.SetCell(targetX, targetY, currentTarget.WithEntity(
                cell.SpeciesId,
                cell.Health,
                cell.Energy,
                cell.Age,
                cell.FoodEaten,
                cell.FoodReserve,
                cell.IsAlpha));
            moved[GetIndex(source, x, y)] = true;
            moved[targetIndex] = true;
            claimed[targetIndex] = true;
            metrics?.Record(cell.SpeciesId, movementSteps: 1);
            return true;
        }

        static bool TryMoveTowardMate(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesCell cell,
            SpeciesRules speciesRules,
            bool[] moved,
            bool[] claimed,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            var pattern = speciesRules.MovementPattern;
            var startOffset = pattern.Count == 0 ? 0 : random.Next(pattern.Count);
            for (var offsetIndex = 0; offsetIndex < pattern.Count; offsetIndex++)
            {
                var offset = pattern.Offsets[(startOffset + offsetIndex) % pattern.Count];
                var targetX = x + offset.x;
                var targetY = y + offset.y;
                if (!source.IsInBounds(targetX, targetY))
                {
                    continue;
                }

                var targetIndex = GetIndex(source, targetX, targetY);
                if (claimed[targetIndex]
                    || !source.GetCell(targetX, targetY).IsPassable
                    || source.GetCell(targetX, targetY).IsCreature
                    || next.GetCell(targetX, targetY).IsCreature)
                {
                    continue;
                }

                var sameSpeciesNeighbors = CountPatternSpeciesNeighbors(
                    source,
                    targetX,
                    targetY,
                    cell.SpeciesId,
                    speciesRules.ReproductionPattern,
                    excludeX: x,
                    excludeY: y);
                if (sameSpeciesNeighbors < speciesRules.ReproductionNeighborCount
                    || (speciesRules.MaxReproductionGroupSize > 0
                        && sameSpeciesNeighbors + 1 > speciesRules.MaxReproductionGroupSize))
                {
                    continue;
                }

                next.SetCell(x, y, source.GetCell(x, y).WithoutEntity());
                next.SetCell(targetX, targetY, next.GetCell(targetX, targetY).WithEntity(
                    cell.SpeciesId,
                    cell.Health,
                    cell.Energy,
                    cell.Age,
                    cell.FoodEaten,
                    cell.FoodReserve,
                    cell.IsAlpha));
                moved[GetIndex(source, x, y)] = true;
                moved[targetIndex] = true;
                claimed[targetIndex] = true;
                metrics?.Record(cell.SpeciesId, movementSteps: 1);
                return true;
            }

            return false;
        }

        static void ResolveStarvation(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            SpeciesSimulationMetrics metrics)
        {
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (!cell.IsCreature
                        || !rules.TryGetValue(cell.SpeciesId, out var speciesRules)
                        || speciesRules.Metabolism <= 0)
                    {
                        continue;
                    }

                    var remainingEnergy = cell.Energy - speciesRules.Metabolism;
                    next.SetCell(x, y, remainingEnergy > 0
                        ? cell.WithEntity(cell.SpeciesId, cell.Health, remainingEnergy, cell.Age, cell.FoodEaten, cell.FoodReserve, cell.IsAlpha)
                        : cell.WithoutEntity());
                    if (remainingEnergy <= 0)
                    {
                        metrics?.Record(cell.SpeciesId, deaths: 1, starvationDeaths: 1);
                    }
                }
            }
        }

        static void ResolveMetabolism(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules)
        {
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (!cell.IsPlantResource
                        || !rules.TryGetValue(cell.SpeciesId, out var plantRules)
                        || !IsPlantSpecies(cell.SpeciesId, plantRules)
                        || plantRules.Metabolism >= 0)
                    {
                        continue;
                    }

                    var grownEnergy = cell.IsTerrainResource
                        ? cell.TerrainEnergy - plantRules.Metabolism
                        : cell.FoodReserve - plantRules.Metabolism;
                    next.SetCell(x, y, cell.IsTerrainResource
                        ? cell.WithTerrainEnergy(grownEnergy)
                        : new SpeciesCell(
                            cell.SpeciesId,
                            cell.Health,
                            cell.Energy,
                            cell.Age,
                            cell.FoodEaten,
                            grownEnergy));
                }
            }
        }

        static void ResolveCrowdingStress(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            SpeciesSimulationMetrics metrics)
        {
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (!cell.IsCreature
                        || !rules.TryGetValue(cell.SpeciesId, out var speciesRules)
                        || speciesRules.MaxReproductionGroupSize <= 0
                        || speciesRules.CrowdingEnergyPenalty <= 0)
                    {
                        continue;
                    }

                    var groupSize = CountPatternSpeciesNeighbors(
                        next,
                        x,
                        y,
                        cell.SpeciesId,
                        speciesRules.ReproductionPattern,
                        excludeX: -1,
                        excludeY: -1) + 1;
                    var excessMembers = groupSize - speciesRules.MaxReproductionGroupSize;
                    if (excessMembers <= 0)
                    {
                        continue;
                    }

                    var remainingEnergy = cell.Energy - excessMembers * speciesRules.CrowdingEnergyPenalty;
                    next.SetCell(x, y, remainingEnergy > 0
                        ? cell.WithEntity(cell.SpeciesId, cell.Health, remainingEnergy, cell.Age, cell.FoodEaten, cell.FoodReserve, cell.IsAlpha)
                        : cell.WithoutEntity());
                    if (remainingEnergy <= 0)
                    {
                        metrics?.Record(cell.SpeciesId, deaths: 1, crowdingDeaths: 1);
                    }
                }
            }
        }

        static void ResolveWilt(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (!cell.IsPlantResource
                        || !rules.TryGetValue(cell.SpeciesId, out var speciesRules)
                        || speciesRules.WiltChance <= 0f
                        || random.NextDouble() > speciesRules.WiltChance)
                    {
                        continue;
                    }

                    next.SetCell(x, y, cell.WithoutPlantResource());
                    metrics?.Record(cell.SpeciesId, deaths: 1, wiltDeaths: 1);
                }
            }
        }

        static void ResolveSeedDrops(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            IReadOnlyDictionary<TerrainId, TerrainDefinition> terrainDefinitions,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            var plantEntry = GetFirstPlant(rules);
            if (!plantEntry.HasValue || plantEntry.Value.Value.StartingFoodReserve <= 0f)
            {
                return;
            }

            var plantSpecies = plantEntry.Value.Key;
            var plantRules = plantEntry.Value.Value;

            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (!cell.IsCreature
                        || !rules.TryGetValue(cell.SpeciesId, out var speciesRules)
                        || speciesRules.SeedDropChance <= 0f
                        || cell.FoodReserve <= 0f
                        || random.NextDouble() > speciesRules.SeedDropChance
                        || speciesRules.MovementPattern.Count == 0)
                    {
                        continue;
                    }

                    var startOffset = random.Next(speciesRules.MovementPattern.Count);
                    for (var offsetIndex = 0; offsetIndex < speciesRules.MovementPattern.Count; offsetIndex++)
                    {
                        var offset = speciesRules.MovementPattern.Offsets[
                            (startOffset + offsetIndex) % speciesRules.MovementPattern.Count];
                        var seedX = x + offset.x;
                        var seedY = y + offset.y;
                        if (!next.IsInBounds(seedX, seedY)
                            || next.GetCell(seedX, seedY).IsCreature
                            || next.GetCell(seedX, seedY).IsPlantResource)
                        {
                            continue;
                        }

                        next.SetCell(seedX, seedY, SpeciesCell.FromTerrain(
                            terrainDefinitions[TerrainIds.Grass],
                            plantRules.StartingFoodReserve,
                            plantSpecies));
                        metrics?.Record(plantSpecies, births: 1);
                        break;
                    }
                }
            }
        }

        static void ResolveReproduction(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            IReadOnlyDictionary<TerrainId, TerrainDefinition> terrainDefinitions,
            IReadOnlyDictionary<SpeciesId, AlphaOffspringRule> alphaOffspringRules,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            var source = next.Copy();
            var claimed = new bool[source.Count];

            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var parent = source.GetCell(x, y);
                    if ((!parent.IsCreature && !parent.IsPlantResource)
                        || !rules.TryGetValue(parent.SpeciesId, out var speciesRules)
                        || (!next.GetCell(x, y).IsCreature && !next.GetCell(x, y).IsPlantResource))
                    {
                        continue;
                    }

                    var currentParent = next.GetCell(x, y);
                    if (currentParent.SpeciesId != parent.SpeciesId
                        || GetReproductionEnergy(currentParent) <= speciesRules.ReproductionFoodRequired
                        || speciesRules.ReproductionChance <= 0f)
                    {
                        continue;
                    }

                    var sameSpeciesNeighbors = 0;
                    if (speciesRules.ReproductionNeighborCount > 0
                        || speciesRules.MaxReproductionGroupSize > 0)
                    {
                        foreach (var offset in speciesRules.ReproductionPattern.Offsets)
                        {
                            if (source.TryGetCell(x + offset.x, y + offset.y, out var neighbor)
                                && IsSameSpecies(neighbor, parent.SpeciesId))
                            {
                                sameSpeciesNeighbors++;
                            }
                        }
                    }

                    if (sameSpeciesNeighbors < speciesRules.ReproductionNeighborCount
                        || (speciesRules.MaxReproductionGroupSize > 0
                            && sameSpeciesNeighbors + 1 >= speciesRules.MaxReproductionGroupSize))
                    {
                        continue;
                    }

                    var reproductionPattern = speciesRules.ReproductionPattern;
                    var startOffset = reproductionPattern.Count == 0 ? 0 : random.Next(reproductionPattern.Count);
                    for (var offsetIndex = 0; offsetIndex < reproductionPattern.Count; offsetIndex++)
                    {
                        var offset = reproductionPattern.Offsets[(startOffset + offsetIndex) % reproductionPattern.Count];
                        var childX = x + offset.x;
                        var childY = y + offset.y;
                        if (!source.IsInBounds(childX, childY))
                        {
                            continue;
                        }

                        var childIndex = GetIndex(source, childX, childY);
                        if (claimed[childIndex]
                            || next.GetCell(childX, childY).IsCreature
                            || next.GetCell(childX, childY).IsPlantResource)
                        {
                            continue;
                        }

                        if (random.NextDouble() <= speciesRules.ReproductionChance)
                        {
                            var offspring = IsPlantSpecies(parent.SpeciesId, speciesRules)
                                ? SpeciesCell.FromTerrain(
                                    terrainDefinitions[TerrainIds.Grass],
                                    speciesRules.StartingFoodReserve,
                                    parent.SpeciesId)
                                : new SpeciesCell(
                                    parent.SpeciesId,
                                    health: 1,
                                    energy: speciesRules.StartingEnergy);
                            if (alphaOffspringRules != null
                                && alphaOffspringRules.TryGetValue(parent.SpeciesId, out var alphaRule))
                            {
                                offspring = alphaRule.Apply(offspring, random);
                            }

                            next.SetCell(childX, childY, offspring);
                            next.SetCell(x, y, ConsumeReproductionEnergy(
                                currentParent,
                                speciesRules.ReproductionFoodRequired));
                            claimed[childIndex] = true;
                            metrics?.Record(parent.SpeciesId, births: 1);
                        }

                        break;
                    }
                }
            }
        }

        static bool ContainsOffset(GridPattern pattern, Vector2Int offset)
        {
            foreach (var candidate in pattern.Offsets)
            {
                if (candidate == offset)
                {
                    return true;
                }
            }

            return false;
        }

        static SpeciesCell CreateFedCell(
            SpeciesCell cell,
            SpeciesRules rules,
            int energyValue,
            float foodAmount = 1f)
        {
            return cell.WithEntity(
                cell.SpeciesId,
                cell.Health,
                cell.Energy + energyValue,
                cell.Age,
                cell.FoodEaten + 1,
                cell.FoodReserve + foodAmount,
                cell.IsAlpha);
        }

        static bool TryFeedOnPlant(
            Grid<SpeciesCell> next,
            int plantX,
            int plantY,
            int eaterX,
            int eaterY,
            SpeciesCell eater,
            SpeciesRules eaterRules,
            int energyValue,
            SpeciesSimulationMetrics metrics)
        {
            var plant = next.GetCell(plantX, plantY);
            if (!plant.IsPlantResource)
            {
                return false;
            }

            var availableEnergy = plant.IsTerrainResource ? plant.TerrainEnergy : plant.FoodReserve;
            if (availableEnergy <= 0f)
            {
                return false;
            }

            var consumedEnergy = Math.Min(1f, availableEnergy);
            next.SetCell(eaterX, eaterY, CreateFedCell(eater, eaterRules, energyValue, consumedEnergy));
            metrics?.Record(eater.SpeciesId, foodConsumed: consumedEnergy);
            var remainingEnergy = availableEnergy - consumedEnergy;
            next.SetCell(plantX, plantY, plant.IsTerrainResource
                ? remainingEnergy > 0f
                    ? plant.WithTerrainEnergy(remainingEnergy)
                    : plant.WithoutPlantResource()
                : remainingEnergy > 0f
                    ? new SpeciesCell(
                        plant.SpeciesId,
                        plant.Health,
                        plant.Energy,
                        plant.Age,
                        plant.FoodEaten,
                        remainingEnergy)
                    : plant.WithoutEntity());
            if (remainingEnergy <= 0f)
            {
                metrics?.Record(plant.SpeciesId, deaths: 1);
            }
            return true;
        }

        static bool IsSameSpecies(SpeciesCell cell, SpeciesId species)
        {
            return (cell.IsPlantResource || cell.IsCreature) && cell.SpeciesId == species;
        }

        static int GetFirstPlantEnergyValue(IReadOnlyDictionary<SpeciesId, SpeciesRules> rules)
        {
            var entry = GetFirstPlant(rules);
            return entry.HasValue ? entry.Value.Value.EnergyValue : 1;
        }

        static KeyValuePair<SpeciesId, SpeciesRules>? GetFirstPlant(
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules)
        {
            KeyValuePair<SpeciesId, SpeciesRules>? result = null;
            foreach (var entry in rules)
            {
                if (!IsPlantSpecies(entry.Key, entry.Value)
                    || (result.HasValue && string.CompareOrdinal(entry.Key.Value, result.Value.Key.Value) >= 0))
                {
                    continue;
                }

                result = entry;
            }

            return result;
        }

        static bool IsPlantSpecies(SpeciesId species, SpeciesRules rules)
        {
            return rules.IsPlant || species == SpeciesIds.Plant;
        }

        static int GetReproductionEnergy(SpeciesCell cell)
        {
            return cell.IsCreature
                ? cell.Energy
                : (int)(cell.IsTerrainResource ? cell.TerrainEnergy : cell.FoodReserve);
        }

        static SpeciesCell ConsumeReproductionEnergy(SpeciesCell cell, int amount)
        {
            if (cell.IsCreature)
            {
                return cell.WithEntity(
                    cell.SpeciesId,
                    cell.Health,
                    cell.Energy - amount,
                    cell.Age,
                    cell.FoodEaten,
                    cell.FoodReserve,
                    cell.IsAlpha);
            }

            var remaining = Math.Max(0f, (cell.IsTerrainResource ? cell.TerrainEnergy : cell.FoodReserve) - amount);
            return cell.IsTerrainResource
                ? cell.WithTerrainEnergy(remaining)
                : new SpeciesCell(
                    cell.SpeciesId,
                    cell.Health,
                    cell.Energy,
                    cell.Age,
                    cell.FoodEaten,
                    remaining);
        }

        static int CountNearbySpecies(
            Grid<SpeciesCell> grid,
            int x,
            int y,
            SpeciesId species,
            int excludeX,
            int excludeY)
        {
            var count = 0;
            for (var offsetY = -1; offsetY <= 1; offsetY++)
            {
                for (var offsetX = -1; offsetX <= 1; offsetX++)
                {
                    var neighborX = x + offsetX;
                    var neighborY = y + offsetY;
                    if ((neighborX == x && neighborY == y)
                        || (neighborX == excludeX && neighborY == excludeY))
                    {
                        continue;
                    }

                    if (grid.TryGetCell(neighborX, neighborY, out var neighbor)
                        && IsSameSpecies(neighbor, species))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        static int CountPatternSpeciesNeighbors(
            Grid<SpeciesCell> grid,
            int x,
            int y,
            SpeciesId species,
            GridPattern pattern,
            int excludeX,
            int excludeY)
        {
            var count = 0;
            foreach (var offset in pattern.Offsets)
            {
                var neighborX = x + offset.x;
                var neighborY = y + offset.y;
                if (neighborX == excludeX && neighborY == excludeY)
                {
                    continue;
                }

                if (grid.TryGetCell(neighborX, neighborY, out var neighbor)
                    && IsSameSpecies(neighbor, species))
                {
                    count++;
                }
            }

            return count;
        }

        static int GetIndex<T>(Grid<T> grid, int x, int y)
        {
            return x + y * grid.Width;
        }

        static int[] CreateShuffledIndices(int count, System.Random random)
        {
            var indices = new int[count];
            for (var index = 0; index < count; index++)
            {
                indices[index] = index;
            }

            for (var index = count - 1; index > 0; index--)
            {
                var swapIndex = random.Next(index + 1);
                var temporary = indices[index];
                indices[index] = indices[swapIndex];
                indices[swapIndex] = temporary;
            }

            return indices;
        }
    }
}
