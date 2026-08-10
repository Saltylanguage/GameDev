using UnityEngine;

namespace SaltyGame
{
    public readonly struct CaveCell
    {
        static readonly GridPattern neighborhood = new GridPattern(new[]
        {
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1),
        });

        public CaveCell(bool isWall)
        {
            IsWall = isWall;
        }

        public static GridPattern Neighborhood => neighborhood;
        public bool IsWall { get; }
    }
}
