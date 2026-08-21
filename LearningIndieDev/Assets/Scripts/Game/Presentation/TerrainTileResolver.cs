namespace SaltyGame
{
    /// <summary>Resolves terrain presentation from the eight surrounding neighbors.</summary>
    public static class TerrainTileResolver
    {
        public const int CardinalVariantCount = 16;
        public const int NeighborMaskCount = 256;
        // Compatibility alias for existing editor/test callers that enumerate atlas variants.
        public const int VariantCount = CardinalVariantCount;

        public const int North = 1;
        public const int East = 2;
        public const int South = 4;
        public const int West = 8;
        public const int NorthEast = 16;
        public const int SouthEast = 32;
        public const int SouthWest = 64;
        public const int NorthWest = 128;
        public const int CardinalMask = North | East | South | West;
        public const int DiagonalMask = NorthEast | SouthEast | SouthWest | NorthWest;

        // Terrain_01 is authored in visual order rather than bit-mask order.
        // Keep this table as the only art-layout seam for future atlas revisions.
        static readonly int[] GrassAtlasIndexByMask =
        {
            5, 13, 12, 14,
            1, 10, 3, 2,
            4, 11, 9, 7,
            8, 6, 15, 0,
        };

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
            return ResolveGrassAtlasIndex(ComputeNeighborMask(cells, x, y, terrainId));
        }

        /// <summary>
        /// Computes N/E/S/W plus NE/SE/SW/NW terrain continuity for a cell.
        /// The mask is presentation-only and never becomes simulation state.
        /// </summary>
        public static int ComputeNeighborMask(
            Grid<SpeciesCell> cells,
            int x,
            int y,
            TerrainId terrainId)
        {
            var mask = 0;
            if (IsTerrain(cells, x, y + 1, terrainId)) mask |= North;
            if (IsTerrain(cells, x + 1, y, terrainId)) mask |= East;
            if (IsTerrain(cells, x, y - 1, terrainId)) mask |= South;
            if (IsTerrain(cells, x - 1, y, terrainId)) mask |= West;
            if (IsTerrain(cells, x + 1, y + 1, terrainId)) mask |= NorthEast;
            if (IsTerrain(cells, x + 1, y - 1, terrainId)) mask |= SouthEast;
            if (IsTerrain(cells, x - 1, y - 1, terrainId)) mask |= SouthWest;
            if (IsTerrain(cells, x - 1, y + 1, terrainId)) mask |= NorthWest;
            return mask;
        }

        /// <summary>Returns diagonal bits whose adjacent cardinal sides are connected but whose corner is open.</summary>
        public static int GetOpenDiagonalCorners(int neighborMask)
        {
            ValidateNeighborMask(neighborMask);
            var openCorners = 0;
            if ((neighborMask & (North | East | NorthEast)) == (North | East)) openCorners |= NorthEast;
            if ((neighborMask & (South | East | SouthEast)) == (South | East)) openCorners |= SouthEast;
            if ((neighborMask & (South | West | SouthWest)) == (South | West)) openCorners |= SouthWest;
            if ((neighborMask & (North | West | NorthWest)) == (North | West)) openCorners |= NorthWest;
            return openCorners;
        }

        /// <summary>
        /// Resolves the current 16-tile atlas. Diagonal state is computed and exposed
        /// separately; until diagonal corner art exists, the atlas uses the cardinal
        /// projection as a deliberate fallback rather than silently discarding the mask.
        /// </summary>
        public static int ResolveGrassAtlasIndex(int neighborMask)
        {
            ValidateNeighborMask(neighborMask);
            return ResolveCardinalAtlasIndex(neighborMask & CardinalMask);
        }

        public static int ResolveCardinalAtlasIndex(int cardinalMask)
        {
            if (cardinalMask < 0 || cardinalMask >= CardinalVariantCount)
            {
                throw new System.ArgumentOutOfRangeException(nameof(cardinalMask), cardinalMask, "Cardinal terrain neighbor masks must be four-bit values.");
            }

            return GrassAtlasIndexByMask[cardinalMask];
        }

        static void ValidateNeighborMask(int neighborMask)
        {
            if (neighborMask < 0 || neighborMask >= NeighborMaskCount)
            {
                throw new System.ArgumentOutOfRangeException(nameof(neighborMask), neighborMask, "Terrain neighbor masks must be eight-bit values.");
            }
        }

        static bool IsTerrain(Grid<SpeciesCell> cells, int x, int y, TerrainId terrainId)
        {
            return cells.TryGetCell(x, y, out var cell) && cell.TerrainId == terrainId;
        }
    }
}
