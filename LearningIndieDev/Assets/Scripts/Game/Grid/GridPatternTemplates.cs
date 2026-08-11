using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public static class GridPatternTemplates
    {
        public static GridPattern CreateMooreRange(int range)
        {
            if (range < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(range), range, "Range cannot be negative.");
            }

            var offsets = new List<Vector2Int>();
            for (var y = -range; y <= range; y++)
            {
                for (var x = -range; x <= range; x++)
                {
                    if (x != 0 || y != 0)
                    {
                        offsets.Add(new Vector2Int(x, y));
                    }
                }
            }

            return new GridPattern(offsets);
        }
    }
}
