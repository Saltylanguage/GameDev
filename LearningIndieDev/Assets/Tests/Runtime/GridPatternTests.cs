using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    [TestFixture]
    public sealed class GridPatternTests
    {
        [Test]
        public void ConstructorStoresOffsetsInTheirSuppliedOrder()
        {
            var pattern = new GridPattern(new[]
            {
                Vector2Int.up,
                Vector2Int.right,
                Vector2Int.down,
            });

            Assert.That(pattern.Count, Is.EqualTo(3));
            Assert.That(pattern.Offsets[0], Is.EqualTo(Vector2Int.up));
            Assert.That(pattern.Offsets[1], Is.EqualTo(Vector2Int.right));
            Assert.That(pattern.Offsets[2], Is.EqualTo(Vector2Int.down));
        }

        [Test]
        public void ConstructorCopiesTheSuppliedOffsets()
        {
            var source = new List<Vector2Int> { Vector2Int.left };
            var pattern = new GridPattern(source);

            source[0] = Vector2Int.right;
            source.Add(Vector2Int.up);

            Assert.That(pattern.Count, Is.EqualTo(1));
            Assert.That(pattern.Offsets[0], Is.EqualTo(Vector2Int.left));
        }

        [Test]
        public void ConstructorRejectsNullOffsets()
        {
            Assert.Throws<ArgumentNullException>(() => new GridPattern(null));
        }
    }
}
