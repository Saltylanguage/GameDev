using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyGame
{
    public sealed class GridPattern
    {
        readonly IReadOnlyList<Vector2Int> offsets;

        public GridPattern(IEnumerable<Vector2Int> offsets)
        {
            if (offsets == null)
            {
                throw new ArgumentNullException(nameof(offsets));
            }

            var copiedOffsets = new List<Vector2Int>(offsets);
            this.offsets = copiedOffsets.AsReadOnly();
        }

        public IReadOnlyList<Vector2Int> Offsets => offsets;
        public int Count => offsets.Count;
    }
}
