namespace SaltyGame
{
    /// <summary>Resolves a terrain atlas tile from the four cardinal neighbors.</summary>
    public static class TerrainTileResolver
    {
        public const int VariantCount = 16;

        const int North = 1;
        const int East = 2;
        const int South = 4;
        const int West = 8;

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
            var mask = 0;
            if (IsTerrain(cells, x, y + 1, terrainId)) mask |= North;
            if (IsTerrain(cells, x + 1, y, terrainId)) mask |= East;
            if (IsTerrain(cells, x, y - 1, terrainId)) mask |= South;
            if (IsTerrain(cells, x - 1, y, terrainId)) mask |= West;
            return ResolveGrassAtlasIndex(mask);
        }

        public static int ResolveGrassAtlasIndex(int neighborMask)
        {
            if (neighborMask < 0 || neighborMask >= VariantCount)
            {
                throw new System.ArgumentOutOfRangeException(nameof(neighborMask), neighborMask, "Terrain neighbor masks must be four-bit values.");
            }

            return GrassAtlasIndexByMask[neighborMask];
        }

        static bool IsTerrain(Grid<SpeciesCell> cells, int x, int y, TerrainId terrainId)
        {
            return cells.TryGetCell(x, y, out var cell) && cell.TerrainId == terrainId;
        }
    }
}
