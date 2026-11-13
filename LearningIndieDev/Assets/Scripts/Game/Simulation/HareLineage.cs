using System.Collections.Generic;

namespace SaltyGame
{
    // Owned by one journey. Keep records for dead parents so their living descendants
    // can still use the family burrow, including after a checkpoint restore.
    internal sealed class HareLineage
    {
        readonly Dictionary<long, (long Parent, long Mate)> parents;

        internal IReadOnlyDictionary<long, (long Parent, long Mate)> ParentRecords => parents;

        public HareLineage() : this(new Dictionary<long, (long, long)>()) { }

        HareLineage(Dictionary<long, (long, long)> parents)
        {
            this.parents = parents;
        }

        public void RecordBirth(long childId, long parentId, long mateId)
        {
            parents.Add(childId, (parentId, mateId));
        }

        public bool IsDescendant(long hareId, long ancestorId)
        {
            if (hareId <= 0 || ancestorId <= 0 || hareId == ancestorId
                || !parents.TryGetValue(hareId, out var directParents))
            {
                return false;
            }

            var pending = new Stack<(long Parent, long Mate)>();
            var visited = new HashSet<long>();
            pending.Push(directParents);
            while (pending.Count > 0)
            {
                var (parentId, mateId) = pending.Pop();
                if (parentId == ancestorId || mateId == ancestorId)
                {
                    return true;
                }

                if (visited.Add(parentId) && parents.TryGetValue(parentId, out var parentAncestors))
                {
                    pending.Push(parentAncestors);
                }

                if (visited.Add(mateId) && parents.TryGetValue(mateId, out var mateAncestors))
                {
                    pending.Push(mateAncestors);
                }
            }

            return false;
        }

        public HareLineage Copy() => new HareLineage(new Dictionary<long, (long, long)>(parents));
    }
}
