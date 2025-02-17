using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Avalonia_Monogame_Dock_Template.Models
{
    public class BezierPolygon
    {
        public List<BezierCurve> Curves { get; set; } = new List<BezierCurve>();

        public BezierPolygon(List<BezierCurve> curves)
        {
            Curves = curves;
        }

        // Získa všetky body polygónu
        public List<Vector2> GetPolygonPoints()
        {
            List<Vector2> points = new List<Vector2>();
            foreach (var curve in Curves)
            {
                points.AddRange(curve.GetPoints());
            }
            return points;
        }

        // Skontroluje, či je polygón uzavretý
        public bool IsClosed()
        {
            return Curves.Count > 1 && Curves.First().Start == Curves.Last().End;
        }

        // Bounding Box celého polygónu
        public RectangleF GetBoundingBox()
        {
            var allBounds = Curves.Select(c => c.GetBoundingBox()).ToList();
            float minX = allBounds.Min(r => r.Left);
            float minY = allBounds.Min(r => r.Top);
            float maxX = allBounds.Max(r => r.Right);
            float maxY = allBounds.Max(r => r.Bottom);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        // Triangulácia pomocou Ear Clipping metódy
        private List<int> Triangulate(List<Vector2> points)
        {
            List<int> indices = new List<int>();
            List<int> remaining = Enumerable.Range(0, points.Count).ToList();

            while (remaining.Count > 3)
            {
                bool earFound = false;

                for (int i = 0; i < remaining.Count; i++)
                {
                    int prev = remaining[(i - 1 + remaining.Count) % remaining.Count];
                    int curr = remaining[i];
                    int next = remaining[(i + 1) % remaining.Count];

                    if (IsEar(points, prev, curr, next, remaining))
                    {
                        indices.Add(prev);
                        indices.Add(curr);
                        indices.Add(next);
                        remaining.RemoveAt(i);
                        earFound = true;
                        break;
                    }
                }

                if (!earFound) break; // Ak sa nenašlo žiadne "ucho", ukončíme
            }

            if (remaining.Count == 3)
            {
                indices.AddRange(remaining);
            }

            return indices;
        }

        // Overenie, či je trojuholník "ucho" (Ear Clipping)
        private bool IsEar(List<Vector2> points, int prev, int curr, int next, List<int> remaining)
        {
            Vector2 a = points[prev];
            Vector2 b = points[curr];
            Vector2 c = points[next];

            if (CrossProduct(a, b, c) > 0)
                return false; // Trojuholník nie je orientovaný správne

            foreach (var i in remaining)
            {
                if (i != prev && i != curr && i != next && PointInTriangle(points[i], a, b, c))
                    return false;
            }
            return true;
        }

        // Výpočet orientácie pomocou vektorového súčinu
        private float CrossProduct(Vector2 a, Vector2 b, Vector2 c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }

        // Kontrola, či bod leží v trojuholníku (Barycentrické súradnice)
        private bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float d1 = CrossProduct(p, a, b);
            float d2 = CrossProduct(p, b, c);
            float d3 = CrossProduct(p, c, a);
            return (d1 < 0 && d2 < 0 && d3 < 0) || (d1 > 0 && d2 > 0 && d3 > 0);
        }

        // **Vykreslí vyplnený Bézierov polygón pomocou triangulácie**
        public void DrawFilled(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            var points = GetPolygonPoints();
            var indices = Triangulate(points);
            if (indices.Count < 3) return;

            VertexPositionColor[] vertices = new VertexPositionColor[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                vertices[i] = new VertexPositionColor(new Vector3(points[i], 0), Color.White);
            }

            effect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                vertices, 0, vertices.Length,
                indices.ToArray(), 0, indices.Count / 3);
        }
    }
}
