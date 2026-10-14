using System.Collections.Generic;

namespace SaltyGame
{
    public static class ForestEdgeFlourishResolver
    {
        static readonly string[] Names =
        {
            "ForestEdge_Mushroom", "ForestEdge_Shrub_01", "ForestEdge_Shrub_02", "ForestEdge_Shrub_03",
            "ForestEdge_Flower_Cream", "ForestEdge_Flower_Purple", "ForestEdge_Flower_Blue", "ForestEdge_Flower_Yellow",
            "ForestEdge_Mushroom_Green", "ForestEdge_Mushroom_Brown",
        };

        public static IReadOnlyList<string> SpriteNames => Names;

        public static int Resolve(SimulationCellSnapshot cell, int x, int y)
        {
            if (!cell.IsPassable || cell.TerrainId != TerrainIds.Grass
                || cell.TerrainVariantMask != TerrainTileResolver.FullMask)
            {
                return -1;
            }

            // Coordinate hashing keeps decorations still without consuming simulation randomness.
            var hash = unchecked((uint)x * 73856093u ^ (uint)y * 19349663u);
            hash = unchecked((hash ^ (hash >> 13)) * 1274126177u);
            return hash % 3u == 0 ? (int)((hash / 3u) % (uint)Names.Length) : -1;
        }
    }
}
