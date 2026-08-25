using System;
using System.Collections.Generic;

namespace SaltyGame
{
    public enum TerrainVisualFamily
    {
        Desert,
        Grass,
    }

    public static class TerrainVisualFamilies
    {
        public static TerrainVisualFamily Get(TerrainId terrainId)
        {
            return terrainId == TerrainIds.Grass ? TerrainVisualFamily.Grass : TerrainVisualFamily.Desert;
        }

        public static string GetSpritePrefix(TerrainVisualFamily family)
        {
            return family == TerrainVisualFamily.Grass ? "Grass" : "Desert";
        }
    }

    /// <summary>Resolves cell-centred eight-neighbor blob terrain masks.</summary>
    public static class TerrainTileResolver
    {
        public const int North = 1;
        public const int NorthEast = 2;
        public const int East = 4;
        public const int SouthEast = 8;
        public const int South = 16;
        public const int SouthWest = 32;
        public const int West = 64;
        public const int NorthWest = 128;
        public const int FullMask = 255;
        public const int TerrainVariantCount = 47;

        static readonly int[] ValidMasks =
        {
            0, 1, 4, 5, 7, 16, 17, 20, 21, 23, 28, 29, 31,
            64, 65, 68, 69, 71, 80, 81, 84, 85, 87, 92, 93, 95,
            112, 113, 116, 117, 119, 124, 125, 127, 193, 197, 199,
            209, 213, 215, 221, 223, 241, 245, 247, 253, 255,
        };

        static readonly HashSet<int> ValidMaskSet = new HashSet<int>(ValidMasks);

        public static IReadOnlyList<int> AllValidMasks => ValidMasks;

        public static int ResolveTerrainMask(Grid<SpeciesCell> cells, int x, int y, TerrainId terrainId)
        {
            return NormalizeMask(ComputeRawMask(cells, x, y, TerrainVisualFamilies.Get(terrainId)));
        }

        public static int ComputeRawMask(Grid<SpeciesCell> cells, int x, int y, TerrainVisualFamily family)
        {
            var mask = 0;
            if (IsFamily(cells, x, y + 1, family)) mask |= North;
            if (IsFamily(cells, x + 1, y + 1, family)) mask |= NorthEast;
            if (IsFamily(cells, x + 1, y, family)) mask |= East;
            if (IsFamily(cells, x + 1, y - 1, family)) mask |= SouthEast;
            if (IsFamily(cells, x, y - 1, family)) mask |= South;
            if (IsFamily(cells, x - 1, y - 1, family)) mask |= SouthWest;
            if (IsFamily(cells, x - 1, y, family)) mask |= West;
            if (IsFamily(cells, x - 1, y + 1, family)) mask |= NorthWest;
            return mask;
        }

        public static int NormalizeMask(int rawMask)
        {
            if (rawMask < 0 || rawMask > FullMask)
            {
                throw new ArgumentOutOfRangeException(nameof(rawMask), rawMask, "Blob masks must be eight-bit values.");
            }

            var mask = rawMask;
            // A diagonal-only contact still needs a visible corner bridge.
            // Promote its adjacent cardinals so the result remains one of the
            // authored 47 masks instead of disappearing as an isolated cell.
            if ((mask & NorthEast) != 0) mask |= North | East;
            if ((mask & SouthEast) != 0) mask |= South | East;
            if ((mask & SouthWest) != 0) mask |= South | West;
            if ((mask & NorthWest) != 0) mask |= North | West;
            return mask;
        }

        public static bool IsValidMask(int mask)
        {
            return ValidMaskSet.Contains(mask);
        }

        public static string GetTerrainSpriteName(TerrainId terrainId, int mask)
        {
            return GetTerrainSpriteName(TerrainVisualFamilies.Get(terrainId), mask);
        }

        public static string GetTerrainSpriteName(TerrainVisualFamily family, int mask)
        {
            if (!IsValidMask(mask))
            {
                throw new ArgumentOutOfRangeException(nameof(mask), mask, "Mask is not one of the 47 normalized blob masks.");
            }

            return $"{TerrainVisualFamilies.GetSpritePrefix(family)}_{mask:D3}";
        }

        static bool IsFamily(Grid<SpeciesCell> cells, int x, int y, TerrainVisualFamily family)
        {
            return cells.TryGetCell(x, y, out var cell)
                && TerrainVisualFamilies.Get(cell.TerrainId) == family;
        }
    }
}
