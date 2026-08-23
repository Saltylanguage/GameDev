using NUnit.Framework;
using UnityEngine;

namespace SaltyGame.Tests
{
    public sealed class TerrainPaintPreviewTests
    {
        [Test]
        public void ScreenPositionMapsToGridWithBottomLeftOrigin()
        {
            var board = new Rect(10f, 20f, 200f, 100f);

            Assert.That(TerrainPaintPreview.TryGetCellAtPosition(
                board, new Vector2(15f, 25f), 20, 10, out var topX, out var topY), Is.True);
            Assert.That((topX, topY), Is.EqualTo((0, 9)));

            Assert.That(TerrainPaintPreview.TryGetCellAtPosition(
                board, new Vector2(205f, 115f), 20, 10, out var bottomX, out var bottomY), Is.True);
            Assert.That((bottomX, bottomY), Is.EqualTo((19, 0)));

            Assert.That(TerrainPaintPreview.TryGetCellAtPosition(
                board, Vector2.zero, 20, 10, out _, out _), Is.False);
        }
    }
}
