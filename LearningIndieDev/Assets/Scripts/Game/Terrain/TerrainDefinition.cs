using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace SaltyGame
{
    public sealed class TerrainDefinition
    {
        public TerrainDefinition(
            TerrainId id,
            bool isPassable,
            float movementCost,
            bool providesResource,
            Color presentationColor,
            float regrowthPerTick = 0f)
        {
            if (!id.IsValid)
            {
                throw new ArgumentException("Terrain id cannot be empty.", nameof(id));
            }

            if (movementCost <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(movementCost), movementCost, "Movement cost must be greater than zero.");
            }

            if (regrowthPerTick < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(regrowthPerTick), regrowthPerTick, "Regrowth cannot be negative.");
            }

            Id = id;
            IsPassable = isPassable;
            MovementCost = movementCost;
            ProvidesResource = providesResource;
            PresentationColor = presentationColor;
            RegrowthPerTick = regrowthPerTick;
        }

        public TerrainId Id { get; }
        public bool IsPassable { get; }
        public float MovementCost { get; }
        public bool ProvidesResource { get; }
        public Color PresentationColor { get; }
        public float RegrowthPerTick { get; }
    }

    public static class TerrainDefaults
    {
        public static TerrainDefinition Bare { get; } = new TerrainDefinition(
            TerrainIds.Bare,
            isPassable: true,
            movementCost: 1f,
            providesResource: false,
            presentationColor: new Color(0.35f, 0.2f, 0.1f));

        public static TerrainDefinition Grass { get; } = new TerrainDefinition(
            TerrainIds.Grass,
            isPassable: true,
            movementCost: 1f,
            providesResource: true,
            presentationColor: new Color(0.2f, 0.75f, 0.25f));

        public static IReadOnlyDictionary<TerrainId, TerrainDefinition> Create()
        {
            var definitions = new Dictionary<TerrainId, TerrainDefinition>
            {
                [TerrainIds.Bare] = Bare,
                [TerrainIds.Grass] = Grass,
            };

            return new ReadOnlyDictionary<TerrainId, TerrainDefinition>(definitions);
        }
    }
}
