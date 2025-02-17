using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using IconPacks.Avalonia.Core;
using IconPacks.Avalonia.PhosphorIcons;

namespace Avalonia_Monogame_Dock_Template.Controls
{
    public static class CursorManager
    {
        private static readonly Dictionary<PackIconPhosphorIconsKind, Cursor> CursorCache = new();

        public static Cursor Create(Visual visual, PixelSize size, PixelPoint hotSpot)
        {
            using var rtb = new RenderTargetBitmap(size, new(96, 96));
            rtb.Render(visual);
            return new Cursor(rtb, hotSpot);
        }

        public static Cursor GetCursor(PackIconPhosphorIconsKind packIconPhosphorIconsKind)
        {
            if (CursorCache.TryGetValue(packIconPhosphorIconsKind, out var cachedCursor))
            {
                return cachedCursor;
            }

            string data = null;
            PackIconDataFactory<PackIconPhosphorIconsKind>.DataIndex.Value?.TryGetValue(packIconPhosphorIconsKind, out data);

            if (data == null)
            {
                return Cursor.Default; // Ak nie sú dostupné dáta, vrátiť predvolený kurzor
            }

            var shadowPath = new Path
            {
                Data = StreamGeometry.Parse(data),
                StrokeThickness = 2,
                Fill = Brushes.Black,
                Stroke = Brushes.Black,
                Opacity = 1,
                RenderTransform = new ScaleTransform(0.9, 0.9)
            };

            var shadowPath2 = new Path
            {
                Data = StreamGeometry.Parse(data),
                StrokeThickness = 3,
                Fill = Brushes.Black,
                Stroke = Brushes.Black,
                Opacity = 1,
                RenderTransform = new ScaleTransform(0.6, 0.6)
            };

            var originalPath = new Path
            {
                Data = StreamGeometry.Parse(data),
                StrokeThickness = 0,
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                RenderTransform = new ScaleTransform(0.8, 0.8)
            };

            int length = 24;
            var grid = new Grid();
            grid.Children.Add(shadowPath);
            grid.Children.Add(shadowPath2);
            grid.Children.Add(originalPath);

            var vb = new Viewbox
            {
                Width = length,
                Height = length,
                Child = grid
            };
            vb.Measure(new(length, length));
            vb.Arrange(new Avalonia.Rect(0, 0, length, length));

            var newCursor = Create(vb, new Avalonia.PixelSize(length, length), new Avalonia.PixelPoint(5, 5));
            CursorCache[packIconPhosphorIconsKind] = newCursor;

            return newCursor;
        }
    }
}
