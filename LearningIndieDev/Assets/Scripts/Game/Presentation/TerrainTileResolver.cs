namespace SaltyGame
{
    /// <summary>
    /// Resolves the 15 non-empty corner combinations in the Grass_/Desert_
    /// dual-grid terrain sets. The mask is presentation-only.
    /// </summary>
    public static class TerrainTileResolver
    {
        public const int TerrainVariantCount = 15;
        public const int CornerMaskCount = 16;
        public const int VariantCount = TerrainVariantCount;

        public const int SouthWest = 1;
        public const int SouthEast = 2;
        public const int NorthWest = 4;
        public const int NorthEast = 8;
        public const int FullVariantIndex = TerrainVariantCount - 1;

        static readonly string[] VariantNamesByMask =
        {
            string.Empty,
            "TopRight",
            "TopLeft",
            "TopMiddle",
            "BottomRight",
            "MiddleRight",
            "DiagStripUpRight",
            "DiagTopRight",
            "BottomLeft",
            "DiagStripDownRight",
            "MiddleLeft",
            "DiagTopLeft",
            "BottomMiddle",
            "DiagBottomRight",
            "DiagBottomLeft",
            "Full",
        };

        public static string GetVariantName(int variantIndex)
        {
            if (variantIndex < 0 || variantIndex >= TerrainVariantCount)
            {
                throw new System.ArgumentOutOfRangeException(nameof(variantIndex), variantIndex, "Terrain variant indices must be 0-14.");
            }

            return VariantNamesByMask[variantIndex + 1];
        }

        public static int ResolveGrassTileIndex(Grid<SpeciesCell> cells, int x, int y)
        {
            return ResolveTerrainTileIndex(cells, x, y, TerrainIds.Grass);
        }

        public static int ResolveTerrainTileIndex(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            TerrainId terrainId)
        {
            return ResolveTerrainAtlasIndex(ComputeCornerMask(cells, x, y, terrainId));
        }

        /// <summary>
        /// Samples the four simulation cells around the visual tile centered at
        /// (x,y). The current cell is the north-east corner, so icons remain
        /// centered while terrain edges use the dual-grid combinations.
        /// </summary>
        public static int ComputeCornerMask(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            TerrainId terrainId)
        {
            var mask = 0;
            if (IsTerrain(cells, x - 1, y - 1, terrainId)) mask |= SouthWest;
            if (IsTerrain(cells, x, y - 1, terrainId)) mask |= SouthEast;
            if (IsTerrain(cells, x - 1, y, terrainId)) mask |= NorthWest;
            if (IsTerrain(cells, x, y, terrainId)) mask |= NorthEast;
            return mask;
        }

        public static int ResolveTerrainAtlasIndex(int cornerMask)
        {
            if (cornerMask < 0 || cornerMask >= CornerMaskCount)
            {
                throw new System.ArgumentOutOfRangeException(nameof(cornerMask), cornerMask, "Terrain corner masks must be four-bit values.");
            }

            return cornerMask == 0 ? -1 : cornerMask - 1;
        }

        public static string GetTerrainSpriteName(TerrainId terrainId, int variantIndex)
        {
            var family = terrainId == TerrainIds.Grass ? "Grass" : "Desert";
            return $"{family}_{GetVariantName(variantIndex)}";
        }

        static bool IsTerrain(Grid<SpeciesCell> cells, int x, int y, TerrainId terrainId)
        {
            return cells.TryGetCell(x, y, out var cell) && cell.TerrainId == terrainId;
        }
    }
}
