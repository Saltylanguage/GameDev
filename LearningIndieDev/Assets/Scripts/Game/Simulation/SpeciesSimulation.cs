using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public enum SpeciesCombatResolutionMode
    {
        LegacyFixedDamage,
        OpposedRoll,
    }

    public enum SpeciesAttackOpportunityMode
    {
        Natural,
        FixedRateDiagnostic,
        PairedLockstepDiagnostic,
    }

    public static class SpeciesSimulation
    {
        public const int FixedRateDiagnosticPeriodTicks = 3;
        static readonly SpeciesId FoxSpeciesId = new SpeciesId("fox");

        public static bool DoesOpposedRollHit(
            int attackRoll,
            int attackModifier,
            int blockRoll,
            int blockModifier)
        {
            return attackRoll + attackModifier > blockRoll + blockModifier;
        }

        public static float GetOpposedRollHitProbability(
            int attackModifier,
            int blockModifier)
        {
            var winningRolls = 0;
            for (var attackRoll = 1; attackRoll <= 20; attackRoll++)
            {
                for (var blockRoll = 1; blockRoll <= 20; blockRoll++)
                {
                    if (DoesOpposedRollHit(
                        attackRoll,
                        attackModifier,
                        blockRoll,
                        blockModifier))
                    {
                        winningRolls++;
                    }
                }
            }

            return winningRolls / 400f;
        }

        internal static string FormatTrackedEntityId(SpeciesId species, long entityId)
        {
            var value = species.Value;
            return string.IsNullOrEmpty(value)
                ? $"#{entityId}"
                : $"{char.ToUpperInvariant(value[0])}{value.Substring(1)}#{entityId}";
        }

        static SpeciesCell MarkCreatureDead(
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesSimulationMetrics metrics)
        {
            var cell = next.GetCell(x, y);
            if (!cell.IsCreature)
            {
                return cell;
            }

            var dead = cell.WithBehaviorState(SpeciesBehaviorState.Dead, ticks: 1);
            next.SetCell(x, y, dead);
            metrics?.RecordState(cell.SpeciesId, SpeciesBehaviorState.Dead, transitioned: true);
            if (metrics != null && metrics.IsTrackedBehaviorCell(cell.SpeciesId, x, y))
            {
                metrics.RecordTrackedTransition(
                    cell.SpeciesId,
                    cell.EntityId,
                    cell.Age,
                    x,
                    y,
                    cell.BehaviorState,
                    SpeciesBehaviorState.Dead);
                Debug.Log(
                    $"[FSM][Tracked] {FormatTrackedEntityId(cell.SpeciesId, cell.EntityId)} age {cell.Age} at ({x},{y}) "
                    + $"Previous: {cell.BehaviorState}, Current: {SpeciesBehaviorState.Dead}");
            }

            return dead;
        }

        public static Grid<SpeciesCell> Step(
            Grid<SpeciesCell> source,
            CellularSimData simulationData,
            int seed,
            SpeciesSimulationMetrics metrics = null,
            SpeciesCombatResolutionMode combatResolutionMode = SpeciesCombatResolutionMode.LegacyFixedDamage,
            SpeciesAttackOpportunityMode attackOpportunityMode = SpeciesAttackOpportunityMode.Natural,
            SpeciesExperimentalOptions experimentalOptions = null)
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
                metrics,
                combatResolutionMode,
                attackOpportunityMode,
                experimentalOptions);
        }

        public static Grid<SpeciesCell> Step(
            Grid<SpeciesCell> source,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            int seed,
            int maxPopulation = 0,
            IReadOnlyDictionary<TerrainId, TerrainDefinition> terrainDefinitions = null,
            IReadOnlyDictionary<SpeciesId, AlphaOffspringRule> alphaOffspringRules = null,
            SpeciesSimulationMetrics metrics = null,
            SpeciesCombatResolutionMode combatResolutionMode = SpeciesCombatResolutionMode.LegacyFixedDamage,
            SpeciesAttackOpportunityMode attackOpportunityMode = SpeciesAttackOpportunityMode.Natural,
            SpeciesExperimentalOptions experimentalOptions = null)
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
            PrepareStep(source, next, rules, random, metrics, experimentalOptions);
            return CompleteStep(
                source,
                next,
                rules,
                terrainDefinitions,
                alphaOffspringRules,
                maxPopulation,
                random,
                metrics,
                combatResolutionMode,
                attackOpportunityMode,
                seed,
                forcedOpportunity: null,
                experimentalOptions: experimentalOptions);
        }

        public static SpeciesPairedStepResult StepPaired(
            Grid<SpeciesCell> baselineSource,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> baselineRules,
            Grid<SpeciesCell> blockPlusTwoSource,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> blockPlusTwoRules,
            int seed,
            int baselineMaxPopulation,
            int blockPlusTwoMaxPopulation,
            IReadOnlyDictionary<TerrainId, TerrainDefinition> baselineTerrainDefinitions,
            IReadOnlyDictionary<TerrainId, TerrainDefinition> blockPlusTwoTerrainDefinitions,
            IReadOnlyDictionary<SpeciesId, AlphaOffspringRule> baselineAlphaOffspringRules,
            IReadOnlyDictionary<SpeciesId, AlphaOffspringRule> blockPlusTwoAlphaOffspringRules,
            SpeciesSimulationMetrics baselineMetrics,
            SpeciesSimulationMetrics blockPlusTwoMetrics,
            SpeciesCombatResolutionMode combatResolutionMode,
            out Grid<SpeciesCell> baselineNext,
            out Grid<SpeciesCell> blockPlusTwoNext,
            out string pairedOpportunityId,
            IList<SpeciesPairedOpportunityObservation> opportunityObservations = null,
            int tick = 0,
            SpeciesExperimentalOptions experimentalOptions = null)
        {
            if (baselineSource == null || blockPlusTwoSource == null)
            {
                throw new ArgumentNullException(nameof(baselineSource));
            }

            if (baselineRules == null || blockPlusTwoRules == null)
            {
                throw new ArgumentNullException(nameof(baselineRules));
            }

            baselineNext = baselineSource.Copy();
            blockPlusTwoNext = blockPlusTwoSource.Copy();
            var baselineRandom = new System.Random(seed);
            var blockPlusTwoRandom = new System.Random(seed);
            PrepareStep(
                baselineSource,
                baselineNext,
                baselineRules,
                baselineRandom,
                baselineMetrics,
                experimentalOptions);
            PrepareStep(
                blockPlusTwoSource,
                blockPlusTwoNext,
                blockPlusTwoRules,
                blockPlusTwoRandom,
                blockPlusTwoMetrics,
                experimentalOptions);

            var result = BuildPairedStepResult(
                baselineSource,
                baselineRules,
                blockPlusTwoSource,
                blockPlusTwoRules,
                seed,
                tick,
                opportunityObservations,
                out var selectedOpportunity,
                out pairedOpportunityId);
            var executable = selectedOpportunity.HasValue
                && IsOpportunityValidForAttack(
                    baselineSource,
                    baselineNext,
                    baselineRules,
                    selectedOpportunity.Value)
                && IsOpportunityValidForAttack(
                    blockPlusTwoSource,
                    blockPlusTwoNext,
                    blockPlusTwoRules,
                    selectedOpportunity.Value);
            if (!executable && selectedOpportunity.HasValue)
            {
                result = new SpeciesPairedStepResult(
                    result.Scheduled,
                    result.BaselineValid,
                    result.BlockPlusTwoValid,
                    result.CommonValid,
                    result.BaselineOnly,
                    result.BlockPlusTwoOnly,
                    result.BaselineCandidateCount,
                    result.BlockPlusTwoCandidateCount,
                    result.CommonCandidateCount,
                    pairedAttemptExecuted: false,
                    invalidated: true);
                pairedOpportunityId = null;
            }
            else if (executable)
            {
                result = new SpeciesPairedStepResult(
                    result.Scheduled,
                    result.BaselineValid,
                    result.BlockPlusTwoValid,
                    result.CommonValid,
                    result.BaselineOnly,
                    result.BlockPlusTwoOnly,
                    result.BaselineCandidateCount,
                    result.BlockPlusTwoCandidateCount,
                    result.CommonCandidateCount,
                    pairedAttemptExecuted: true,
                    invalidated: false);
            }

            baselineNext = CompleteStep(
                baselineSource,
                baselineNext,
                baselineRules,
                baselineTerrainDefinitions,
                baselineAlphaOffspringRules,
                baselineMaxPopulation,
                baselineRandom,
                baselineMetrics,
                combatResolutionMode,
                SpeciesAttackOpportunityMode.PairedLockstepDiagnostic,
                seed,
                forcedOpportunity: executable ? selectedOpportunity : null,
                experimentalOptions: experimentalOptions);
            blockPlusTwoNext = CompleteStep(
                blockPlusTwoSource,
                blockPlusTwoNext,
                blockPlusTwoRules,
                blockPlusTwoTerrainDefinitions,
                blockPlusTwoAlphaOffspringRules,
                blockPlusTwoMaxPopulation,
                blockPlusTwoRandom,
                blockPlusTwoMetrics,
                combatResolutionMode,
                SpeciesAttackOpportunityMode.PairedLockstepDiagnostic,
                seed,
                forcedOpportunity: executable ? selectedOpportunity : null,
                experimentalOptions: experimentalOptions);
            return result;
        }

        static void PrepareStep(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            System.Random random,
            SpeciesSimulationMetrics metrics,
            SpeciesExperimentalOptions experimentalOptions = null)
        {
            ResolveAging(next);
            ResolveAttackCooldowns(next, experimentalOptions);
            SpeciesBehaviorSystem.Update(source, next, rules, random, metrics);
        }

        static Grid<SpeciesCell> CompleteStep(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            IReadOnlyDictionary<TerrainId, TerrainDefinition> terrainDefinitions,
            IReadOnlyDictionary<SpeciesId, AlphaOffspringRule> alphaOffspringRules,
            int maxPopulation,
            System.Random random,
            SpeciesSimulationMetrics metrics,
            SpeciesCombatResolutionMode combatResolutionMode,
            SpeciesAttackOpportunityMode attackOpportunityMode,
            int seed,
            SpeciesAttackOpportunity? forcedOpportunity,
            SpeciesExperimentalOptions experimentalOptions = null)
        {
            RecordHerbivoreExposureStep(source, rules, metrics, experimentalOptions);
            ResolveAttacks(
                source,
                next,
                rules,
                random,
                metrics,
                combatResolutionMode,
                attackOpportunityMode,
                seed,
                forcedOpportunity,
                experimentalOptions);
            ResolveMovement(source, next, rules, random, metrics);
            ResolveMetabolism(next, rules);
            ResolveTerrainRegrowth(next, terrainDefinitions);
            ResolveStarvation(next, rules, metrics);
            ResolveCrowdingStress(next, rules, metrics);
            ResolveSeedDrops(next, rules, terrainDefinitions, random, metrics);
            ResolveWilt(next, rules, random, metrics);
            ResolveReproduction(next, rules, terrainDefinitions, alphaOffspringRules, random, metrics);
            ResolvePopulationLimit(next, maxPopulation, random, metrics);
            return next;
        }

        static void RecordHerbivoreExposureStep(
            Grid<SpeciesCell> source,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            SpeciesSimulationMetrics metrics,
            SpeciesExperimentalOptions experimentalOptions)
        {
            if (metrics == null || experimentalOptions == null
                || !experimentalOptions.UsesHerbivoreStatLine)
            {
                return;
            }

            metrics.BeginHerbivoreExposureStep();
            var hasCarnivore = false;
            for (var y = 0; y < source.Height && !hasCarnivore; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var cell = source.GetCell(x, y);
                    if (cell.IsCreature
                        && rules.TryGetValue(cell.SpeciesId, out var cellRules)
                        && cellRules.Role == SpeciesRole.Carnivore)
                    {
                        hasCarnivore = true;
                        break;
                    }
                }
            }

            if (!hasCarnivore)
            {
                return;
            }

            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var cell = source.GetCell(x, y);
                    if (cell.IsCreature
                        && rules.TryGetValue(cell.SpeciesId, out var cellRules)
                        && cellRules.Role == SpeciesRole.Herbivore)
                    {
                        metrics.RecordPredatorActiveHerbivoreStep(cell.SpeciesId);
                    }
                }
            }
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

            var populations = new List<(int CellIndex, bool IsCreature)>();
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (cell.IsCreature)
                    {
                        populations.Add((GetIndex(next, x, y), true));
                    }

                    if (cell.IsPlantResource)
                    {
                        populations.Add((GetIndex(next, x, y), false));
                    }
                }
            }

            while (populations.Count > maxPopulation)
            {
                var removeIndex = random.Next(populations.Count);
                var population = populations[removeIndex];
                populations.RemoveAt(removeIndex);
                var x = population.CellIndex % next.Width;
                var y = population.CellIndex / next.Width;
                var cell = next.GetCell(x, y);
                next.SetCell(x, y, population.IsCreature
                    ? MarkCreatureDead(next, x, y, metrics).WithoutEntity()
                    : cell.WithoutPlantResource());
                metrics?.RecordDeath(
                    cell,
                    x,
                    y,
                    SpeciesDeathCause.PopulationLimit,
                    populationLimitRemovals: 1);
            }
        }

        static void ResolveAttacks(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            System.Random random,
            SpeciesSimulationMetrics metrics,
            SpeciesCombatResolutionMode combatResolutionMode,
            SpeciesAttackOpportunityMode attackOpportunityMode,
            int seed,
            SpeciesAttackOpportunity? forcedOpportunity,
            SpeciesExperimentalOptions experimentalOptions = null)
        {
            var controlled = attackOpportunityMode == SpeciesAttackOpportunityMode.FixedRateDiagnostic;
            var paired = attackOpportunityMode == SpeciesAttackOpportunityMode.PairedLockstepDiagnostic;
            var collectHerbivoreStatLine = experimentalOptions != null
                && experimentalOptions.UsesHerbivoreStatLine;
            var useSplitCombatStats = combatResolutionMode == SpeciesCombatResolutionMode.OpposedRoll
                && experimentalOptions != null
                && experimentalOptions.UsesSplitCombatStats;
            var controlledX = -1;
            var controlledY = -1;
            var controlledTargetX = -1;
            var controlledTargetY = -1;
            if (paired)
            {
                if (!forcedOpportunity.HasValue)
                {
                    return;
                }

                controlledX = forcedOpportunity.Value.AttackerX;
                controlledY = forcedOpportunity.Value.AttackerY;
                controlledTargetX = forcedOpportunity.Value.TargetX;
                controlledTargetY = forcedOpportunity.Value.TargetY;
            }
            else if (controlled)
            {
                if (!IsFixedRateDiagnosticTick(seed))
                {
                    return;
                }

                metrics?.RecordControlledOpportunityScheduled();
                if (!TryFindControlledOpportunity(
                    source,
                    rules,
                    seed,
                    out controlledX,
                    out controlledY,
                    out controlledTargetX,
                    out controlledTargetY))
                {
                    metrics?.RecordControlledOpportunityUnfulfilledNoTarget();
                    return;
                }

                metrics?.RecordControlledOpportunityEligible();
            }

            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var attacker = source.GetCell(x, y);
                    if (!attacker.IsCreature
                        || !rules.TryGetValue(attacker.SpeciesId, out var attackerRules)
                        || (useSplitCombatStats
                            ? attackerRules.AttackModifier <= 0 && attackerRules.DamageAmount <= 0
                            : attackerRules.AttackAmount <= 0)
                        || !next.GetCell(x, y).IsCreature)
                    {
                        continue;
                    }

                    if (controlled || paired
                        ? x != controlledX || y != controlledY
                        : next.GetCell(x, y).BehaviorState != SpeciesBehaviorState.Attacking
                            || !ShouldForage(attacker, attackerRules))
                    {
                        continue;
                    }

                    var cooldownRemaining = next.GetCell(x, y).AttackCooldownTicksRemaining;
                    if (experimentalOptions != null
                        && attacker.SpeciesId == FoxSpeciesId
                        && cooldownRemaining > 0)
                    {
                        metrics?.RecordCombatCooldownSuppressed(
                            attacker.SpeciesId,
                            attacker.EntityId,
                            x,
                            y,
                            cooldownRemaining);
                        continue;
                    }

                    var attackPattern = attackerRules.AttackPattern;
                    var startOffset = controlled || paired || attackPattern.Count == 0
                        ? 0
                        : random.Next(attackPattern.Count);
                    for (var offsetIndex = 0; offsetIndex < attackPattern.Count; offsetIndex++)
                    {
                        var offset = attackPattern.Offsets[(startOffset + offsetIndex) % attackPattern.Count];
                        var targetX = x + offset.x;
                        var targetY = y + offset.y;
                        if ((controlled || paired)
                            && (targetX != controlledTargetX || targetY != controlledTargetY))
                        {
                            continue;
                        }

                        if (!source.TryGetCell(targetX, targetY, out var target)
                            || !attackerRules.DietTargetId.HasValue
                            || !SpeciesPerception.IsDietTarget(target, attackerRules.DietTargetId.Value))
                        {
                            continue;
                        }

                        SpeciesRules targetRules = null;
                        var hasTargetRules = target.IsCreature
                            && rules.TryGetValue(target.SpeciesId, out targetRules);
                        var isCarnivoreHerbivoreInteraction = collectHerbivoreStatLine
                            && attackerRules.Role == SpeciesRole.Carnivore
                            && hasTargetRules
                            && targetRules.Role == SpeciesRole.Herbivore;
                        if (target.IsCreature)
                        {
                            metrics?.RecordCombatOpportunity(attacker.SpeciesId);
                            if (isCarnivoreHerbivoreInteraction)
                            {
                                metrics?.RecordHerbivoreEncounter(target.SpeciesId, target.EntityId);
                            }
                        }

                        var currentTarget = target;
                        if (target.IsCreature
                            && (!next.TryGetCell(targetX, targetY, out currentTarget)
                                || !currentTarget.IsCreature))
                        {
                            if (controlled || paired)
                            {
                                metrics?.RecordControlledOpportunityUnfulfilledInvalidated();
                                return;
                            }

                            continue;
                        }

                        var damage = useSplitCombatStats
                            ? attackerRules.DamageAmount
                            : attackerRules.AttackAmount;
                        var hasDirectionalBlock = hasTargetRules
                            && ContainsOffset(targetRules.BlockPattern, new Vector2Int(-offset.x, -offset.y));
                        if (target.IsCreature && combatResolutionMode == SpeciesCombatResolutionMode.OpposedRoll)
                        {
                            // Opposed-roll combat is a universal creature-versus-creature
                            // resolution path. Directional block patterns may still describe
                            // authored defense coverage, but they do not decide whether dice
                            // are rolled; the target's block amount is always the modifier.
                            var attackRoll = random.Next(1, 21);
                            var blockRoll = random.Next(1, 21);
                            var attackModifier = useSplitCombatStats
                                ? attackerRules.AttackModifier
                                : attackerRules.AttackAmount;
                            var blockModifier = hasTargetRules ? targetRules.BlockAmount : 0;
                            var hit = DoesOpposedRollHit(
                                attackRoll,
                                attackModifier,
                                blockRoll,
                                blockModifier);
                            metrics?.RecordCombatRoll(
                                attacker.SpeciesId,
                                target.SpeciesId,
                                attackRoll,
                                attackModifier,
                                blockRoll,
                                blockModifier,
                                hit);
                            if (!hit)
                            {
                                damage = 0;
                            }
                        }
                        else if (hasDirectionalBlock)
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
                            metrics?.RecordCombatAttempt(
                                attacker.SpeciesId,
                                hit: damage > 0,
                                blocked: combatResolutionMode == SpeciesCombatResolutionMode.OpposedRoll
                                    && damage <= 0,
                                damageDealt: Math.Min(damage, currentTarget.Health),
                                lethal: remainingHealth <= 0);
                            metrics?.Record(
                                attacker.SpeciesId,
                                damageDealt: Math.Min(damage, currentTarget.Health));
                            if (remainingHealth > 0)
                            {
                                next.SetCell(targetX, targetY, currentTarget.WithEntity(
                                    currentTarget.SpeciesId,
                                    remainingHealth,
                                    currentTarget.Energy,
                                    currentTarget.Age,
                                    currentTarget.FoodEaten,
                                    currentTarget.FoodReserve,
                                    currentTarget.IsAlpha));
                            }
                            else
                            {
                                next.SetCell(targetX, targetY, MarkCreatureDead(
                                    next,
                                    targetX,
                                    targetY,
                                    metrics).WithoutEntity());
                            }

                            if (remainingHealth <= 0)
                            {
                                if (isCarnivoreHerbivoreInteraction)
                                {
                                    metrics?.RecordHerbivorePreyed(currentTarget.SpeciesId);
                                }

                                metrics?.RecordDeath(
                                    currentTarget,
                                    targetX,
                                    targetY,
                                    SpeciesDeathCause.Combat);
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
                                        : 0).WithBehaviorState(
                                            SpeciesBehaviorState.Eating,
                                            Math.Max(1, currentAttacker.BehaviorStateTicks)));
                                metrics?.RecordFoodAction(attacker.SpeciesId, successful: true, consumedAmount: 1f);
                            }
                            else
                            {
                                metrics?.RecordFoodAction(attacker.SpeciesId, successful: false);
                            }
                        }
                        else
                        {
                            if (currentTarget.IsCreature)
                            {
                                metrics?.RecordCombatAttempt(
                                    attacker.SpeciesId,
                                    hit: false,
                                    blocked: combatResolutionMode == SpeciesCombatResolutionMode.OpposedRoll,
                                    damageDealt: 0,
                                    lethal: false);
                            }
                            metrics?.RecordFoodAction(attacker.SpeciesId, successful: false);
                        }

                        ApplyFoxAttackCooldown(next, x, y, experimentalOptions);

                        break;
                    }
                }
            }
        }

        static bool IsFixedRateDiagnosticTick(int seed)
        {
            return seed % FixedRateDiagnosticPeriodTicks == 0;
        }

        static SpeciesPairedStepResult BuildPairedStepResult(
            Grid<SpeciesCell> baselineSource,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> baselineRules,
            Grid<SpeciesCell> blockPlusTwoSource,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> blockPlusTwoRules,
            int seed,
            int tick,
            IList<SpeciesPairedOpportunityObservation> opportunityObservations,
            out SpeciesAttackOpportunity? selectedOpportunity,
            out string pairedOpportunityId)
        {
            selectedOpportunity = null;
            pairedOpportunityId = null;
            if (!IsFixedRateDiagnosticTick(seed))
            {
                return new SpeciesPairedStepResult(
                    scheduled: false,
                    baselineValid: 0,
                    blockPlusTwoValid: 0,
                    commonValid: 0,
                    baselineOnly: 0,
                    blockPlusTwoOnly: 0,
                    baselineCandidateCount: 0,
                    blockPlusTwoCandidateCount: 0,
                    commonCandidateCount: 0,
                    pairedAttemptExecuted: false,
                    invalidated: false);
            }

            var baselineCandidates = EnumerateAttackOpportunities(baselineSource, baselineRules);
            var blockPlusTwoCandidates = EnumerateAttackOpportunities(blockPlusTwoSource, blockPlusTwoRules);
            var baselineOnly = new List<SpeciesAttackOpportunity>();
            var blockPlusTwoOnly = new List<SpeciesAttackOpportunity>();
            var common = SpeciesAttackOpportunity.Intersect(
                baselineCandidates,
                blockPlusTwoCandidates,
                baselineOnly,
                blockPlusTwoOnly);
            AppendOpportunityObservations(
                baselineSource,
                blockPlusTwoSource,
                baselineCandidates,
                blockPlusTwoCandidates,
                seed,
                tick,
                opportunityObservations);
            if (common.Count > 0)
            {
                selectedOpportunity = common[Math.Abs(seed / FixedRateDiagnosticPeriodTicks) % common.Count];
                pairedOpportunityId = selectedOpportunity.Value.Identity;
            }

            var hasBaselineCandidate = baselineCandidates.Count > 0;
            var hasBlockPlusTwoCandidate = blockPlusTwoCandidates.Count > 0;
            var hasCommonCandidate = common.Count > 0;
            return new SpeciesPairedStepResult(
                scheduled: true,
                baselineValid: hasBaselineCandidate ? 1 : 0,
                blockPlusTwoValid: hasBlockPlusTwoCandidate ? 1 : 0,
                commonValid: hasCommonCandidate ? 1 : 0,
                baselineOnly: hasBaselineCandidate && !hasCommonCandidate ? 1 : 0,
                blockPlusTwoOnly: hasBlockPlusTwoCandidate && !hasCommonCandidate ? 1 : 0,
                baselineCandidateCount: baselineCandidates.Count,
                blockPlusTwoCandidateCount: blockPlusTwoCandidates.Count,
                commonCandidateCount: common.Count,
                pairedAttemptExecuted: false,
                invalidated: false);
        }

        static void AppendOpportunityObservations(
            Grid<SpeciesCell> baselineSource,
            Grid<SpeciesCell> blockPlusTwoSource,
            IReadOnlyList<SpeciesAttackOpportunity> baselineCandidates,
            IReadOnlyList<SpeciesAttackOpportunity> blockPlusTwoCandidates,
            int seed,
            int tick,
            IList<SpeciesPairedOpportunityObservation> observations)
        {
            if (observations == null)
            {
                return;
            }

            var baselineSummary = CountOpportunityPopulation(baselineSource);
            var blockPlusTwoSummary = CountOpportunityPopulation(blockPlusTwoSource);
            var blockRemaining = new Dictionary<SpeciesAttackOpportunity, int>();
            foreach (var opportunity in blockPlusTwoCandidates)
            {
                blockRemaining.TryGetValue(opportunity, out var count);
                blockRemaining[opportunity] = count + 1;
            }

            var baselineOccurrences = new Dictionary<SpeciesAttackOpportunity, int>();
            foreach (var opportunity in baselineCandidates)
            {
                baselineOccurrences.TryGetValue(opportunity, out var occurrence);
                baselineOccurrences[opportunity] = occurrence + 1;
                blockRemaining.TryGetValue(opportunity, out var blockCount);
                var common = blockCount > 0;
                if (common)
                {
                    blockRemaining[opportunity] = blockCount - 1;
                }

                AddOpportunityObservation(
                    observations,
                    baselineSource,
                    blockPlusTwoSource,
                    baselineSummary,
                    blockPlusTwoSummary,
                    opportunity,
                    seed,
                    tick,
                    occurrence,
                    SpeciesOpportunityStrata.Classify(true, common));
            }

            foreach (var opportunity in blockPlusTwoCandidates)
            {
                blockRemaining.TryGetValue(opportunity, out var remaining);
                if (remaining <= 0)
                {
                    continue;
                }

                blockRemaining[opportunity] = remaining - 1;
                baselineOccurrences.TryGetValue(opportunity, out var occurrence);
                AddOpportunityObservation(
                    observations,
                    baselineSource,
                    blockPlusTwoSource,
                    baselineSummary,
                    blockPlusTwoSummary,
                    opportunity,
                    seed,
                    tick,
                    occurrence,
                    SpeciesOpportunityStrata.BlockOnly);
                baselineOccurrences[opportunity] = occurrence + 1;
            }
        }

        static void AddOpportunityObservation(
            IList<SpeciesPairedOpportunityObservation> observations,
            Grid<SpeciesCell> baselineSource,
            Grid<SpeciesCell> blockPlusTwoSource,
            OpportunityPopulationSummary baselineSummary,
            OpportunityPopulationSummary blockPlusTwoSummary,
            SpeciesAttackOpportunity opportunity,
            int seed,
            int tick,
            int occurrence,
            string stratum)
        {
            var baselineState = CaptureOpportunityState(
                baselineSource,
                baselineSummary,
                opportunity,
                stratum != SpeciesOpportunityStrata.BlockOnly);
            var blockPlusTwoState = CaptureOpportunityState(
                blockPlusTwoSource,
                blockPlusTwoSummary,
                opportunity,
                stratum != SpeciesOpportunityStrata.BaselineOnly);
            observations.Add(new SpeciesPairedOpportunityObservation
            {
                seed = seed,
                tick = tick,
                occurrence = occurrence,
                identity = opportunity.Identity,
                eventId = $"{tick}:{opportunity.Identity}:{occurrence}",
                stratum = stratum,
                baseline = baselineState,
                blockPlusTwo = blockPlusTwoState,
            });
        }

        static SpeciesOpportunityState CaptureOpportunityState(
            Grid<SpeciesCell> source,
            OpportunityPopulationSummary summary,
            SpeciesAttackOpportunity opportunity,
            bool present)
        {
            if (!present
                || !source.TryGetCell(opportunity.AttackerX, opportunity.AttackerY, out var attacker)
                || !source.TryGetCell(opportunity.TargetX, opportunity.TargetY, out var target))
            {
                return new SpeciesOpportunityState();
            }

            return new SpeciesOpportunityState
            {
                present = attacker.IsCreature && target.IsCreature,
                attackerSpecies = attacker.SpeciesId.Value,
                targetSpecies = target.SpeciesId.Value,
                attackerX = opportunity.AttackerX,
                attackerY = opportunity.AttackerY,
                targetX = opportunity.TargetX,
                targetY = opportunity.TargetY,
                attackerEntityId = attacker.EntityId,
                targetEntityId = target.EntityId,
                attackerHealth = attacker.Health,
                attackerEnergy = attacker.Energy,
                attackerAge = attacker.Age,
                attackerFoodReserve = attacker.FoodReserve,
                attackerIsAlpha = attacker.IsAlpha,
                attackerBehaviorState = attacker.BehaviorState.ToString(),
                targetHealth = target.Health,
                targetEnergy = target.Energy,
                targetAge = target.Age,
                targetFoodReserve = target.FoodReserve,
                targetIsAlpha = target.IsAlpha,
                terrainId = attacker.TerrainId.Value,
                terrainEnergy = attacker.TerrainEnergy,
                harePopulation = summary.HarePopulation,
                foxPopulation = summary.FoxPopulation,
                plantPopulation = summary.PlantPopulation,
                localHareDensity = CountLocalSpecies(source, opportunity.AttackerX, opportunity.AttackerY, "hare"),
                localFoxDensity = CountLocalSpecies(source, opportunity.AttackerX, opportunity.AttackerY, "fox"),
                localPlantResourceDensity = CountLocalPlantResources(source, opportunity.AttackerX, opportunity.AttackerY),
            };
        }

        readonly struct OpportunityPopulationSummary
        {
            public OpportunityPopulationSummary(int harePopulation, int foxPopulation, int plantPopulation)
            {
                HarePopulation = harePopulation;
                FoxPopulation = foxPopulation;
                PlantPopulation = plantPopulation;
            }

            public int HarePopulation { get; }
            public int FoxPopulation { get; }
            public int PlantPopulation { get; }
        }

        static OpportunityPopulationSummary CountOpportunityPopulation(Grid<SpeciesCell> source)
        {
            var hare = 0;
            var fox = 0;
            var plant = 0;
            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var cell = source.GetCell(x, y);
                    if (cell.IsCreature && string.Equals(cell.SpeciesId.Value, "hare", StringComparison.Ordinal))
                    {
                        hare++;
                    }
                    else if (cell.IsCreature && string.Equals(cell.SpeciesId.Value, "fox", StringComparison.Ordinal))
                    {
                        fox++;
                    }

                    if (cell.IsPlantResource)
                    {
                        plant++;
                    }
                }
            }

            return new OpportunityPopulationSummary(hare, fox, plant);
        }

        static int CountLocalSpecies(
            Grid<SpeciesCell> source,
            int centerX,
            int centerY,
            string species)
        {
            var count = 0;
            for (var y = Math.Max(0, centerY - 1); y <= Math.Min(source.Height - 1, centerY + 1); y++)
            {
                for (var x = Math.Max(0, centerX - 1); x <= Math.Min(source.Width - 1, centerX + 1); x++)
                {
                    var cell = source.GetCell(x, y);
                    if (cell.IsCreature && string.Equals(cell.SpeciesId.Value, species, StringComparison.Ordinal))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        static int CountLocalPlantResources(Grid<SpeciesCell> source, int centerX, int centerY)
        {
            var count = 0;
            for (var y = Math.Max(0, centerY - 1); y <= Math.Min(source.Height - 1, centerY + 1); y++)
            {
                for (var x = Math.Max(0, centerX - 1); x <= Math.Min(source.Width - 1, centerX + 1); x++)
                {
                    if (source.GetCell(x, y).IsPlantResource)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        static IReadOnlyList<SpeciesAttackOpportunity> EnumerateAttackOpportunities(
            Grid<SpeciesCell> source,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules)
        {
            var candidates = new List<SpeciesAttackOpportunity>();
            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var attacker = source.GetCell(x, y);
                    if (!attacker.IsCreature
                        || !rules.TryGetValue(attacker.SpeciesId, out var attackerRules)
                        || attackerRules.AttackAmount <= 0
                        || !attackerRules.DietTargetId.HasValue)
                    {
                        continue;
                    }

                    foreach (var offset in attackerRules.AttackPattern.Offsets)
                    {
                        var candidateX = x + offset.x;
                        var candidateY = y + offset.y;
                        if (!source.TryGetCell(candidateX, candidateY, out var candidate)
                            || !candidate.IsCreature
                            || !SpeciesPerception.IsDietTarget(
                                candidate,
                                attackerRules.DietTargetId.Value))
                        {
                            continue;
                        }

                        candidates.Add(new SpeciesAttackOpportunity(
                            attacker.SpeciesId,
                            x,
                            y,
                            candidate.SpeciesId,
                            candidateX,
                            candidateY,
                            offset));
                    }
                }
            }

            return candidates;
        }

        static bool IsOpportunityValidForAttack(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            SpeciesAttackOpportunity opportunity)
        {
            if (!source.TryGetCell(opportunity.AttackerX, opportunity.AttackerY, out var attacker)
                || !attacker.IsCreature
                || attacker.SpeciesId != opportunity.AttackerSpecies
                || !next.TryGetCell(opportunity.AttackerX, opportunity.AttackerY, out var currentAttacker)
                || !currentAttacker.IsCreature
                || currentAttacker.SpeciesId != opportunity.AttackerSpecies
                || !source.TryGetCell(opportunity.TargetX, opportunity.TargetY, out var target)
                || !target.IsCreature
                || target.SpeciesId != opportunity.TargetSpecies
                || !rules.TryGetValue(opportunity.AttackerSpecies, out var attackerRules)
                || !attackerRules.DietTargetId.HasValue
                || !SpeciesPerception.IsDietTarget(target, attackerRules.DietTargetId.Value)
                || !next.TryGetCell(opportunity.TargetX, opportunity.TargetY, out var currentTarget)
                || !currentTarget.IsCreature
                || !rules.ContainsKey(opportunity.TargetSpecies))
            {
                return false;
            }

            foreach (var offset in attackerRules.AttackPattern.Offsets)
            {
                if (offset == opportunity.Offset
                    && opportunity.AttackerX + offset.x == opportunity.TargetX
                    && opportunity.AttackerY + offset.y == opportunity.TargetY)
                {
                    return true;
                }
            }

            return false;
        }

        static bool TryFindControlledOpportunity(
            Grid<SpeciesCell> source,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            int seed,
            out int attackerX,
            out int attackerY,
            out int targetX,
            out int targetY)
        {
            var candidates = EnumerateAttackOpportunities(source, rules);

            if (candidates.Count > 0)
            {
                var selected = candidates[Math.Abs(seed / FixedRateDiagnosticPeriodTicks) % candidates.Count];
                attackerX = selected.AttackerX;
                attackerY = selected.AttackerY;
                targetX = selected.TargetX;
                targetY = selected.TargetY;
                return true;
            }

            attackerX = -1;
            attackerY = -1;
            targetX = -1;
            targetY = -1;
            return false;
        }

        static void ResolveMovement(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            var lowestMovementCost = float.MaxValue;
            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var cell = source.GetCell(x, y);
                    if (cell.IsPassable)
                    {
                        lowestMovementCost = Math.Min(lowestMovementCost, cell.MovementCost);
                    }
                }
            }

            if (lowestMovementCost == float.MaxValue)
            {
                lowestMovementCost = 1f;
            }

            var movementPasses = 1;
            foreach (var speciesRules in rules.Values)
            {
                movementPasses = Math.Max(
                    movementPasses,
                    (int)Math.Ceiling(
                        (speciesRules.MovementSpeed + speciesRules.FleeMovementSpeedBonus)
                        / lowestMovementCost));
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
                    || currentCell.SpeciesId != sourceCell.SpeciesId)
                {
                    continue;
                }

                if (currentCell.BehaviorState == SpeciesBehaviorState.Sleeping
                    || currentCell.BehaviorState == SpeciesBehaviorState.Attacking)
                {
                    moved[sourceIndex] = true;
                    continue;
                }

                if (TryResolveVisionMovement(
                    source,
                    next,
                    x,
                    y,
                    currentCell,
                    speciesRules,
                    rules,
                    movementPass,
                    plantEnergyValue,
                    moved,
                    claimed,
                    random,
                    metrics))
                {
                    continue;
                }

                if (ShouldForage(currentCell, speciesRules)
                    && TryMove(
                        source,
                        next,
                        x,
                        y,
                        currentCell,
                        speciesRules,
                        speciesRules.DietPattern,
                        movementPass,
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
                        movementPass,
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
                    movementPass,
                    plantEnergyValue,
                    requireDietTarget: false,
                    moved,
                    claimed,
                    random,
                    metrics);
            }
        }

        static bool CanMoveThisPass(float movementSpeed, int movementPass, System.Random random)
        {
            var guaranteedMoves = (int)Math.Floor(movementSpeed);
            if (movementPass < guaranteedMoves)
            {
                return true;
            }

            var fractionalMoveChance = movementSpeed - guaranteedMoves;
            return movementPass == guaranteedMoves
                && fractionalMoveChance > 0f
                && random.NextDouble() < fractionalMoveChance;
        }

        static bool TryMove(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesCell cell,
            SpeciesRules speciesRules,
            GridPattern pattern,
            int movementPass,
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
                movementPass,
                plantEnergyValue,
                moved,
                claimed,
                random,
                metrics);
        }

        static bool TryResolveVisionMovement(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesCell currentCell,
            SpeciesRules speciesRules,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            int movementPass,
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

            if (SpeciesPerception.TryFindThreatTarget(
                source,
                x,
                y,
                currentCell.SpeciesId,
                rules,
                random,
                out var threatTarget))
            {
                if (TryMoveAwayFromThreat(
                    source,
                    next,
                    x,
                    y,
                    currentCell,
                    speciesRules,
                    threatTarget,
                    movementPass,
                    plantEnergyValue,
                    moved,
                    claimed,
                    random,
                    metrics))
                {
                    return true;
                }

                moved[GetIndex(source, x, y)] = true;
                return true;
            }

            var foodTarget = default(SpeciesPerceivedTarget);
            var hasFood = ShouldForage(currentCell, speciesRules)
                && SpeciesPerception.TryFindFoodTarget(source, x, y, speciesRules, random, out foodTarget);
            var canSeekMate = speciesRules.ReproductionChance > 0f
                && speciesRules.ReproductionNeighborCount > 0
                && HasReproductionEnergy(currentCell, speciesRules)
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
                && HasReproductionEnergy(currentCell, speciesRules);

            if (prioritizeMate
                && TryMoveTowardPerceivedTarget(
                    source,
                    next,
                    x,
                    y,
                    currentCell,
                    speciesRules,
                    mateTarget,
                    movementPass,
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
                    movementPass,
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
                    movementPass,
                    plantEnergyValue,
                    moved,
                    claimed,
                    random,
                    metrics);
        }

        static bool TryMoveAwayFromThreat(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesCell cell,
            SpeciesRules speciesRules,
            SpeciesPerceivedTarget threat,
            int movementPass,
            int plantEnergyValue,
            bool[] moved,
            bool[] claimed,
            System.Random random,
            SpeciesSimulationMetrics metrics)
        {
            var currentDistance = Math.Max(
                Math.Abs(x - threat.Location.x),
                Math.Abs(y - threat.Location.y));
            var bestDistance = currentDistance;
            var bestX = -1;
            var bestY = -1;
            var startOffset = speciesRules.MovementPattern.Count == 0
                ? 0
                : random.Next(speciesRules.MovementPattern.Count);
            for (var offsetIndex = 0; offsetIndex < speciesRules.MovementPattern.Count; offsetIndex++)
            {
                var offset = speciesRules.MovementPattern.Offsets[
                    (startOffset + offsetIndex) % speciesRules.MovementPattern.Count];
                var targetX = x + offset.x;
                var targetY = y + offset.y;
                if (!source.IsInBounds(targetX, targetY)
                    || claimed[GetIndex(source, targetX, targetY)])
                {
                    continue;
                }

                var target = source.GetCell(targetX, targetY);
                if (!target.IsPassable || target.IsCreature)
                {
                    continue;
                }

                var distance = Math.Max(
                    Math.Abs(targetX - threat.Location.x),
                    Math.Abs(targetY - threat.Location.y));
                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    bestX = targetX;
                    bestY = targetY;
                }
            }

            return bestX >= 0
                && TryMoveTo(
                    source,
                    next,
                    x,
                    y,
                    cell,
                    speciesRules,
                    bestX,
                    bestY,
                    movementPass,
                    plantEnergyValue,
                    moved,
                    claimed,
                    random,
                    metrics,
                    feedOnDietTarget: false);
        }

        static bool TryMoveTowardPerceivedTarget(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesCell cell,
            SpeciesRules speciesRules,
            SpeciesPerceivedTarget target,
            int movementPass,
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
                movementPass,
                plantEnergyValue,
                moved,
                claimed,
                random,
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
            int movementPass,
            int plantEnergyValue,
            bool[] moved,
            bool[] claimed,
            System.Random random,
            SpeciesSimulationMetrics metrics,
            bool feedOnDietTarget = true)
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
                && feedOnDietTarget
                && SpeciesPerception.IsDietTarget(sourceTarget, speciesRules.DietTargetId.Value))
            {
                if (!TryFeedOnPlant(next, targetX, targetY, x, y, cell, speciesRules, plantEnergyValue, metrics))
                {
                    return false;
                }

                moved[GetIndex(source, x, y)] = true;
                return true;
            }

            var movementSpeed = speciesRules.MovementSpeed
                + (cell.BehaviorState == SpeciesBehaviorState.Fleeing
                    ? speciesRules.FleeMovementSpeedBonus
                    : 0f);
            if (!CanMoveThisPass(
                movementSpeed / sourceTarget.MovementCost,
                movementPass,
                random))
            {
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
                cell.IsAlpha,
                entityId: cell.EntityId).WithBehaviorState(cell.BehaviorState, cell.BehaviorStateTicks));
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
            int movementPass,
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

                if (!CanMoveThisPass(
                    speciesRules.MovementSpeed / source.GetCell(targetX, targetY).MovementCost,
                    movementPass,
                    random))
                {
                    moved[GetIndex(source, x, y)] = true;
                    return true;
                }

                next.SetCell(x, y, source.GetCell(x, y).WithoutEntity());
                next.SetCell(targetX, targetY, next.GetCell(targetX, targetY).WithEntity(
                    cell.SpeciesId,
                    cell.Health,
                    cell.Energy,
                    cell.Age,
                    cell.FoodEaten,
                    cell.FoodReserve,
                    cell.IsAlpha,
                    entityId: cell.EntityId).WithBehaviorState(cell.BehaviorState, cell.BehaviorStateTicks));
                moved[GetIndex(source, x, y)] = true;
                moved[targetIndex] = true;
                claimed[targetIndex] = true;
                metrics?.Record(cell.SpeciesId, movementSteps: 1);
                return true;
            }

            return false;
        }

        static void ResolveAttackCooldowns(
            Grid<SpeciesCell> next,
            SpeciesExperimentalOptions experimentalOptions)
        {
            if (experimentalOptions == null || !experimentalOptions.HasFoxAttackCooldown)
            {
                return;
            }

            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (cell.IsCreature
                        && cell.SpeciesId == FoxSpeciesId
                        && cell.AttackCooldownTicksRemaining > 0)
                    {
                        next.SetCell(x, y, cell.WithAttackCooldown(
                            cell.AttackCooldownTicksRemaining - 1));
                    }
                }
            }
        }

        static void ApplyFoxAttackCooldown(
            Grid<SpeciesCell> next,
            int x,
            int y,
            SpeciesExperimentalOptions experimentalOptions)
        {
            if (experimentalOptions == null || !experimentalOptions.HasFoxAttackCooldown)
            {
                return;
            }

            var attacker = next.GetCell(x, y);
            if (attacker.IsCreature && attacker.SpeciesId == FoxSpeciesId)
            {
                next.SetCell(x, y, attacker.WithAttackCooldown(
                    experimentalOptions.FoxAttackCooldownTicks));
            }
        }

        static void ResolveAging(Grid<SpeciesCell> next)
        {
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (cell.IsOccupied)
                    {
                        next.SetCell(x, y, cell.WithEntity(
                            cell.SpeciesId,
                            cell.Health,
                            cell.Energy,
                            cell.Age + 1,
                            cell.FoodEaten,
                            cell.FoodReserve,
                            cell.IsAlpha));
                    }
                }
            }
        }

        static void ResolveTerrainRegrowth(
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<TerrainId, TerrainDefinition> terrainDefinitions)
        {
            for (var y = 0; y < next.Height; y++)
            {
                for (var x = 0; x < next.Width; x++)
                {
                    var cell = next.GetCell(x, y);
                    if (terrainDefinitions.TryGetValue(cell.TerrainId, out var terrain)
                        && terrain.ProvidesResource
                        && terrain.RegrowthPerTick > 0f)
                    {
                        next.SetCell(x, y, cell.WithTerrainEnergy(
                            cell.TerrainEnergy + terrain.RegrowthPerTick));
                    }
                }
            }
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
                        : MarkCreatureDead(next, x, y, metrics).WithoutEntity());
                    if (remainingEnergy <= 0)
                    {
                        metrics?.RecordDeath(cell, x, y, SpeciesDeathCause.Starvation);
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
                    var plantSpecies = GetResourceSpeciesId(cell);
                    if (!cell.IsPlantResource
                        || !rules.TryGetValue(plantSpecies, out var plantRules)
                        || !IsPlantSpecies(plantSpecies, plantRules)
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
                    var excessMembers = groupSize
                        - (speciesRules.MaxReproductionGroupSize + speciesRules.CrowdingTolerance);
                    if (excessMembers <= 0)
                    {
                        continue;
                    }

                    var remainingEnergy = cell.Energy - excessMembers * speciesRules.CrowdingEnergyPenalty;
                    next.SetCell(x, y, remainingEnergy > 0
                        ? cell.WithEntity(cell.SpeciesId, cell.Health, remainingEnergy, cell.Age, cell.FoodEaten, cell.FoodReserve, cell.IsAlpha)
                        : MarkCreatureDead(next, x, y, metrics).WithoutEntity());
                    if (remainingEnergy <= 0)
                    {
                        metrics?.RecordDeath(cell, x, y, SpeciesDeathCause.Crowding);
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
                    var plantSpecies = GetResourceSpeciesId(cell);
                    if (!cell.IsPlantResource
                        || !rules.TryGetValue(plantSpecies, out var speciesRules)
                        || speciesRules.WiltChance <= 0f
                        || random.NextDouble() > speciesRules.WiltChance)
                    {
                        continue;
                    }

                    next.SetCell(x, y, cell.WithoutPlantResource());
                    metrics?.RecordDeath(cell, x, y, SpeciesDeathCause.Wilt);
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
                        || cell.FoodReserve < 1f
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
                        next.SetCell(x, y, cell.WithEntity(
                            cell.SpeciesId,
                            cell.Health,
                            cell.Energy,
                            cell.Age,
                            cell.FoodEaten,
                            cell.FoodReserve - 1f,
                            cell.IsAlpha));
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
                        || speciesRules.ReproductionChance <= 0f)
                    {
                        continue;
                    }

                    if (!HasReproductionEnergy(currentParent, speciesRules))
                    {
                        metrics?.RecordReproductionOutcome(
                            parent.SpeciesId,
                            SpeciesReproductionOutcome.BlockedEnergy);
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

                    if (sameSpeciesNeighbors < speciesRules.ReproductionNeighborCount)
                    {
                        metrics?.RecordReproductionOutcome(
                            parent.SpeciesId,
                            SpeciesReproductionOutcome.BlockedMateRequirement);
                        continue;
                    }

                    if (speciesRules.MaxReproductionGroupSize > 0
                        && sameSpeciesNeighbors + 1 >= speciesRules.MaxReproductionGroupSize)
                    {
                        metrics?.RecordReproductionOutcome(
                            parent.SpeciesId,
                            SpeciesReproductionOutcome.BlockedGroupLimit);
                        continue;
                    }

                    var reproductionPattern = speciesRules.ReproductionPattern;
                    var startOffset = reproductionPattern.Count == 0 ? 0 : random.Next(reproductionPattern.Count);
                    if (random.NextDouble() > speciesRules.ReproductionChance)
                    {
                        metrics?.RecordReproductionOutcome(
                            parent.SpeciesId,
                            SpeciesReproductionOutcome.FailedChanceRoll);
                        continue;
                    }

                    var requestedLitter = speciesRules.IsPlant
                        ? 1
                        : random.Next(speciesRules.LitterMinimum, speciesRules.LitterMaximum + 1);
                    var births = 0;
                    for (var offsetIndex = 0; offsetIndex < reproductionPattern.Count; offsetIndex++)
                    {
                        if (births >= requestedLitter
                            || GetReproductionEnergy(currentParent)
                                < speciesRules.ReproductionFoodRequired * (births + 1))
                        {
                            break;
                        }

                        var offset = reproductionPattern.Offsets[(startOffset + offsetIndex) % reproductionPattern.Count];
                        var childX = x + offset.x;
                        var childY = y + offset.y;
                        if (!source.IsInBounds(childX, childY))
                        {
                            continue;
                        }

                        var childIndex = GetIndex(source, childX, childY);
                        var childCell = next.GetCell(childX, childY);
                        var parentIsPlant = IsPlantSpecies(parent.SpeciesId, speciesRules);
                        if (claimed[childIndex]
                            || childCell.IsCreature
                            || (parentIsPlant && childCell.IsPlantResource))
                        {
                            continue;
                        }

                        var offspring = parentIsPlant
                            ? SpeciesCell.FromTerrain(
                                terrainDefinitions[TerrainIds.Grass],
                                speciesRules.StartingFoodReserve,
                                parent.SpeciesId)
                            : childCell.WithEntity(
                                parent.SpeciesId,
                                health: 1,
                                energy: speciesRules.ReproductionFoodRequired,
                                age: 0,
                                foodEaten: 0,
                                foodReserve: 0f);
                        if (alphaOffspringRules != null
                            && alphaOffspringRules.TryGetValue(parent.SpeciesId, out var alphaRule))
                        {
                            offspring = alphaRule.Apply(offspring, random);
                        }

                        next.SetCell(childX, childY, offspring);
                        claimed[childIndex] = true;
                        births++;
                        metrics?.Record(parent.SpeciesId, births: 1);
                    }

                    if (births > 0)
                    {
                        metrics?.RecordReproductionOutcome(
                            parent.SpeciesId,
                            SpeciesReproductionOutcome.SuccessfulAttempt);
                        next.SetCell(x, y, ConsumeReproductionEnergy(
                            currentParent,
                            speciesRules.ReproductionFoodRequired * births));
                    }
                    else
                    {
                        metrics?.RecordReproductionOutcome(
                            parent.SpeciesId,
                            SpeciesReproductionOutcome.BlockedNoBirthLocation);
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
                rules.MaximumEnergy > 0
                    ? Math.Min(rules.MaximumEnergy, cell.Energy + energyValue + rules.DigestionEnergyBonus)
                    : cell.Energy + energyValue + rules.DigestionEnergyBonus,
                cell.Age,
                cell.FoodEaten + 1,
                cell.FoodReserve + foodAmount,
                cell.IsAlpha);
        }

        static bool ShouldForage(SpeciesCell cell, SpeciesRules rules)
        {
            return rules.DietTargetId.HasValue && cell.Energy <= rules.ForageBelowEnergy;
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
                metrics?.RecordFoodAction(eater.SpeciesId, successful: false);
                return false;
            }

            var availableEnergy = plant.IsTerrainResource ? plant.TerrainEnergy : plant.FoodReserve;
            if (availableEnergy <= 0f)
            {
                metrics?.RecordFoodAction(eater.SpeciesId, successful: false);
                return false;
            }

            // A plant's energy value also defines the size of each grazing bite.
            // With regrowth at one unit per tick, this makes dense grazing
            // locally self-limiting instead of giving every nearby hare a free
            // full-energy feed every tick.
            var consumedEnergy = Math.Min(Math.Max(1, energyValue), availableEnergy);
            next.SetCell(
                eaterX,
                eaterY,
                CreateFedCell(eater, eaterRules, energyValue, consumedEnergy)
                    .WithBehaviorState(
                        SpeciesBehaviorState.Eating,
                        Math.Max(1, eater.BehaviorStateTicks)));
            metrics?.RecordFoodAction(eater.SpeciesId, successful: true, consumedAmount: consumedEnergy);
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
                metrics?.RecordDeath(plant, plantX, plantY, SpeciesDeathCause.ResourceConsumed);
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

        static SpeciesId GetResourceSpeciesId(SpeciesCell cell)
        {
            return cell.ResourceSpeciesId.IsValid ? cell.ResourceSpeciesId : cell.SpeciesId;
        }

        static int GetReproductionEnergy(SpeciesCell cell)
        {
            return cell.IsCreature
                ? cell.Energy
                : (int)(cell.IsTerrainResource ? cell.TerrainEnergy : cell.FoodReserve);
        }

        static bool HasReproductionEnergy(SpeciesCell cell, SpeciesRules rules)
        {
            var minimumEnergy = rules.Role == SpeciesRole.Carnivore
                && rules.MaximumEnergy > 0
                ? Math.Max(rules.ReproductionFoodRequired, rules.MaximumEnergy / 2)
                : rules.ReproductionFoodRequired;
            return GetReproductionEnergy(cell) > minimumEnergy;
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

namespace SaltyGame
{
    /// <summary>
    /// Chooses a creature's short-term behavior state. It never mutates world
    /// rules; SpeciesSimulation remains responsible for resolving actions.
    /// </summary>
    public static class SpeciesBehaviorSystem
    {
        const int SleepAfterIdleTicks = 10;
        const int SleepDurationTicks = 8;

        public static void Update(
            Grid<SpeciesCell> source,
            Grid<SpeciesCell> next,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            System.Random random,
            SpeciesSimulationMetrics metrics = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (source.Width != next.Width || source.Height != next.Height)
            {
                throw new ArgumentException("The source and next grids must have matching dimensions.", nameof(next));
            }

            metrics?.BeginBehaviorTracking(source);
            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var cell = source.GetCell(x, y);
                    if (!cell.IsCreature || !rules.TryGetValue(cell.SpeciesId, out var speciesRules))
                    {
                        continue;
                    }

                    var state = ChooseState(source, x, y, cell, speciesRules, rules, random);
                    var transitioned = state != cell.BehaviorState;
                    var stateTicks = !transitioned
                        ? cell.BehaviorStateTicks + 1
                        : 1;
                    if (transitioned && metrics != null && metrics.IsTrackedBehaviorCell(cell.SpeciesId, x, y))
                    {
                        metrics.RecordTrackedTransition(
                            cell.SpeciesId,
                            cell.EntityId,
                            cell.Age,
                            x,
                            y,
                            cell.BehaviorState,
                            state);
                        Debug.Log(
                            $"[FSM][Tracked] {SpeciesSimulation.FormatTrackedEntityId(cell.SpeciesId, cell.EntityId)} age {cell.Age} at ({x},{y}) "
                            + $"Previous: {cell.BehaviorState}, Current: {state}");
                    }
                    next.SetCell(x, y, next.GetCell(x, y).WithBehaviorState(state, stateTicks));
                    metrics?.RecordState(cell.SpeciesId, state, transitioned);
                }
            }
        }

        static SpeciesBehaviorState ChooseState(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            SpeciesCell cell,
            SpeciesRules speciesRules,
            IReadOnlyDictionary<SpeciesId, SpeciesRules> rules,
            System.Random random)
        {
            if (SpeciesPerception.TryFindThreatTarget(
                    cells,
                    x,
                    y,
                    cell.SpeciesId,
                    rules,
                    random,
                    out _))
            {
                return SpeciesBehaviorState.Fleeing;
            }

            if (cell.BehaviorState == SpeciesBehaviorState.Sleeping
                && cell.BehaviorStateTicks < SleepDurationTicks)
            {
                return SpeciesBehaviorState.Sleeping;
            }

            if (ShouldForage(cell, speciesRules)
                && SpeciesPerception.TryFindFoodTarget(
                    cells,
                    x,
                    y,
                    speciesRules,
                    random,
                    out var food))
            {
                return IsAdjacent(x, y, food.Location)
                    ? food.Cell.IsCreature
                        ? SpeciesBehaviorState.Attacking
                        : SpeciesBehaviorState.Eating
                    : SpeciesBehaviorState.Hunting;
            }

            if (speciesRules.ReproductionChance > 0f
                && HasReproductionEnergy(cell, speciesRules)
                && SpeciesPerception.TryFindMateTarget(
                    cells,
                    x,
                    y,
                    cell.SpeciesId,
                    speciesRules,
                    random,
                    out _))
            {
                return SpeciesBehaviorState.Mating;
            }

            if (cell.BehaviorState == SpeciesBehaviorState.Fleeing)
            {
                return SpeciesBehaviorState.Wandering;
            }

            if (cell.BehaviorState == SpeciesBehaviorState.Wandering
                && cell.BehaviorStateTicks >= SleepAfterIdleTicks
                && random.NextDouble() < 0.02d)
            {
                return SpeciesBehaviorState.Sleeping;
            }

            return SpeciesBehaviorState.Wandering;
        }

        static bool HasReproductionEnergy(SpeciesCell cell, SpeciesRules rules)
        {
            var minimumEnergy = rules.Role == SpeciesRole.Carnivore
                && rules.MaximumEnergy > 0
                ? Math.Max(rules.ReproductionFoodRequired, rules.MaximumEnergy / 2)
                : rules.ReproductionFoodRequired;
            return cell.Energy > minimumEnergy;
        }

        static bool ShouldForage(SpeciesCell cell, SpeciesRules rules)
        {
            return rules.DietTargetId.HasValue && cell.Energy <= rules.ForageBelowEnergy;
        }

        static bool IsAdjacent(int x, int y, Vector2Int target)
        {
            return Math.Max(Math.Abs(target.x - x), Math.Abs(target.y - y)) <= 1;
        }
    }
}
