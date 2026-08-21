using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    /// <summary>
    /// A combat opportunity identity that deliberately excludes entity ids and
    /// combat outcomes so it remains comparable across paired worlds.
    /// </summary>
    public readonly struct SpeciesAttackOpportunity : IEquatable<SpeciesAttackOpportunity>
    {
        public SpeciesAttackOpportunity(
            SpeciesId attackerSpecies,
            int attackerX,
            int attackerY,
            SpeciesId targetSpecies,
            int targetX,
            int targetY,
            Vector2Int offset)
        {
            AttackerSpecies = attackerSpecies;
            AttackerX = attackerX;
            AttackerY = attackerY;
            TargetSpecies = targetSpecies;
            TargetX = targetX;
            TargetY = targetY;
            Offset = offset;
        }

        public SpeciesId AttackerSpecies { get; }
        public int AttackerX { get; }
        public int AttackerY { get; }
        public SpeciesId TargetSpecies { get; }
        public int TargetX { get; }
        public int TargetY { get; }
        public Vector2Int Offset { get; }

        public string Identity =>
            $"{AttackerSpecies.Value}@{AttackerX},{AttackerY}->{TargetSpecies.Value}@{TargetX},{TargetY}:{Offset.x},{Offset.y}";

        public bool Equals(SpeciesAttackOpportunity other)
        {
            return AttackerSpecies == other.AttackerSpecies
                && AttackerX == other.AttackerX
                && AttackerY == other.AttackerY
                && TargetSpecies == other.TargetSpecies
                && TargetX == other.TargetX
                && TargetY == other.TargetY
                && Offset == other.Offset;
        }

        public override bool Equals(object obj)
        {
            return obj is SpeciesAttackOpportunity other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = AttackerSpecies.GetHashCode();
                hash = (hash * 397) ^ AttackerX;
                hash = (hash * 397) ^ AttackerY;
                hash = (hash * 397) ^ TargetSpecies.GetHashCode();
                hash = (hash * 397) ^ TargetX;
                hash = (hash * 397) ^ TargetY;
                hash = (hash * 397) ^ Offset.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(SpeciesAttackOpportunity left, SpeciesAttackOpportunity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SpeciesAttackOpportunity left, SpeciesAttackOpportunity right)
        {
            return !left.Equals(right);
        }

        public static IReadOnlyList<SpeciesAttackOpportunity> Intersect(
            IReadOnlyList<SpeciesAttackOpportunity> baseline,
            IReadOnlyList<SpeciesAttackOpportunity> blockPlusTwo,
            IList<SpeciesAttackOpportunity> baselineOnly,
            IList<SpeciesAttackOpportunity> blockPlusTwoOnly)
        {
            if (baseline == null)
            {
                throw new ArgumentNullException(nameof(baseline));
            }

            if (blockPlusTwo == null)
            {
                throw new ArgumentNullException(nameof(blockPlusTwo));
            }

            if (baselineOnly == null)
            {
                throw new ArgumentNullException(nameof(baselineOnly));
            }

            if (blockPlusTwoOnly == null)
            {
                throw new ArgumentNullException(nameof(blockPlusTwoOnly));
            }

            var blockSet = new HashSet<SpeciesAttackOpportunity>(blockPlusTwo);
            var baselineSet = new HashSet<SpeciesAttackOpportunity>(baseline);
            var common = new List<SpeciesAttackOpportunity>();
            foreach (var opportunity in baseline)
            {
                if (blockSet.Contains(opportunity))
                {
                    common.Add(opportunity);
                }
                else if (!baselineOnly.Contains(opportunity))
                {
                    baselineOnly.Add(opportunity);
                }
            }

            foreach (var opportunity in blockPlusTwo)
            {
                if (!baselineSet.Contains(opportunity) && !blockPlusTwoOnly.Contains(opportunity))
                {
                    blockPlusTwoOnly.Add(opportunity);
                }
            }

            return common;
        }
    }

    public static class SpeciesOpportunityStrata
    {
        public const string Common = "COMMON";
        public const string BaselineOnly = "BASELINE_ONLY";
        public const string BlockOnly = "BLOCK_ONLY";

        public static string Classify(bool baselineValid, bool blockPlusTwoValid)
        {
            if (baselineValid && blockPlusTwoValid)
            {
                return Common;
            }

            return baselineValid ? BaselineOnly : BlockOnly;
        }
    }

    [Serializable]
    public sealed class SpeciesOpportunityState
    {
        public bool present;
        public string attackerSpecies;
        public string targetSpecies;
        public int attackerX;
        public int attackerY;
        public int targetX;
        public int targetY;
        public long attackerEntityId;
        public long targetEntityId;
        public int attackerHealth;
        public int attackerEnergy;
        public int attackerAge;
        public float attackerFoodReserve;
        public bool attackerIsAlpha;
        public string attackerBehaviorState;
        public int targetHealth;
        public int targetEnergy;
        public int targetAge;
        public float targetFoodReserve;
        public bool targetIsAlpha;
        public string terrainId;
        public float terrainEnergy;
        public int harePopulation;
        public int foxPopulation;
        public int plantPopulation;
        public int localHareDensity;
        public int localFoxDensity;
        public int localPlantResourceDensity;
    }

    [Serializable]
    public sealed class SpeciesPairedOpportunityObservation
    {
        public int seed;
        public int tick;
        public int occurrence;
        public string eventId;
        public string identity;
        public string stratum;
        public SpeciesOpportunityState baseline;
        public SpeciesOpportunityState blockPlusTwo;
    }

    public readonly struct SpeciesPairedStepResult
    {
        internal SpeciesPairedStepResult(
            bool scheduled,
            int baselineValid,
            int blockPlusTwoValid,
            int commonValid,
            int baselineOnly,
            int blockPlusTwoOnly,
            int baselineCandidateCount,
            int blockPlusTwoCandidateCount,
            int commonCandidateCount,
            bool pairedAttemptExecuted,
            bool invalidated)
        {
            Scheduled = scheduled;
            BaselineValid = baselineValid;
            BlockPlusTwoValid = blockPlusTwoValid;
            CommonValid = commonValid;
            BaselineOnly = baselineOnly;
            BlockPlusTwoOnly = blockPlusTwoOnly;
            BaselineCandidateCount = baselineCandidateCount;
            BlockPlusTwoCandidateCount = blockPlusTwoCandidateCount;
            CommonCandidateCount = commonCandidateCount;
            PairedAttemptExecuted = pairedAttemptExecuted;
            Invalidated = invalidated;
        }

        public bool Scheduled { get; }
        public int BaselineValid { get; }
        public int BlockPlusTwoValid { get; }
        public int CommonValid { get; }
        public int BaselineOnly { get; }
        public int BlockPlusTwoOnly { get; }
        public int BaselineCandidateCount { get; }
        public int BlockPlusTwoCandidateCount { get; }
        public int CommonCandidateCount { get; }
        public bool PairedAttemptExecuted { get; }
        public bool Invalidated { get; }
        public int UnionValid => BaselineValid + BlockPlusTwoValid - CommonValid;
    }

    public sealed class SpeciesPairedOpportunityControl
    {
        readonly List<string> pairedOpportunityIds = new List<string>();
        readonly List<SpeciesPairedOpportunityObservation> opportunityObservations =
            new List<SpeciesPairedOpportunityObservation>();

        public int Scheduled { get; private set; }
        public int BaselineValid { get; private set; }
        public int BlockPlusTwoValid { get; private set; }
        public int CommonValid { get; private set; }
        public int BaselineOnly { get; private set; }
        public int BlockPlusTwoOnly { get; private set; }
        public int BaselineCandidateCount { get; private set; }
        public int BlockPlusTwoCandidateCount { get; private set; }
        public int CommonCandidateCount { get; private set; }
        public int PairedAttempts { get; private set; }
        public int PairedMismatches { get; private set; }
        public int Invalidated { get; private set; }
        public int UnionValid => BaselineValid + BlockPlusTwoValid - CommonValid;
        public int UnionCandidateCount => BaselineCandidateCount + BlockPlusTwoCandidateCount - CommonCandidateCount;
        public IReadOnlyList<string> PairedOpportunityIds => pairedOpportunityIds;
        public IReadOnlyList<SpeciesPairedOpportunityObservation> OpportunityObservations => opportunityObservations;

        internal void Add(
            SpeciesPairedStepResult result,
            string pairedOpportunityId,
            IReadOnlyList<SpeciesPairedOpportunityObservation> observations = null)
        {
            Scheduled += result.Scheduled ? 1 : 0;
            BaselineValid += result.BaselineValid;
            BlockPlusTwoValid += result.BlockPlusTwoValid;
            CommonValid += result.CommonValid;
            BaselineOnly += result.BaselineOnly;
            BlockPlusTwoOnly += result.BlockPlusTwoOnly;
            BaselineCandidateCount += result.BaselineCandidateCount;
            BlockPlusTwoCandidateCount += result.BlockPlusTwoCandidateCount;
            CommonCandidateCount += result.CommonCandidateCount;
            PairedAttempts += result.PairedAttemptExecuted ? 1 : 0;
            PairedMismatches += result.PairedAttemptExecuted ? 0 : result.CommonValid > 0 ? 1 : 0;
            Invalidated += result.Invalidated ? 1 : 0;
            if (result.PairedAttemptExecuted && !string.IsNullOrEmpty(pairedOpportunityId))
            {
                pairedOpportunityIds.Add(pairedOpportunityId);
            }

            if (observations != null)
            {
                opportunityObservations.AddRange(observations);
            }
        }
    }
}
