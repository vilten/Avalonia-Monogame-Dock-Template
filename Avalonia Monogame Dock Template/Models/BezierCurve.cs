using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Avalonia_Monogame_Dock_Template.Models
{
    public class BezierCurve
    {
        public Verticle Start { get; set; }
        public Verticle End { get; set; }
        public Verticle Control1 { get; set; }
        public Verticle Control2 { get; set; }
        public int Segments { get; set; } = 20;
        public float StartPercent { get; set; } = 0;
        public float EndPercent { get; set; } = 1;

        public BezierCurve(Verticle start, Verticle control1, Verticle control2, Verticle end, int segments = 20, float startPercent = 0, float endPercent = 1)
        {
            Start = start;
            Control1 = control1;
            Control2 = control2;
            End = end;
            Segments = segments;
            StartPercent = startPercent;
            EndPercent = endPercent;
        }

        // Vypočíta bod na Bézierovej krivke pre parameter t (0 - 1)
        public Vector2 GetPoint(float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            return (uuu * Start.position) +
                   (3 * uu * t * Control1.position) +
                   (3 * u * tt * Control2.position) +
                   (ttt * End.position);
        }

        // Generuje body na krivke (rozlíšenie = počet bodov)
        public List<Vector2> GetPoints()
        {
            List<Vector2> points = new List<Vector2>();
            StartPercent = MathHelper.Clamp(StartPercent, 0f, 1f);
            EndPercent = MathHelper.Clamp(EndPercent, 0f, 1f);

            if (StartPercent > EndPercent)
                return points; // Neplatný rozsah, vrátime prázdny zoznam

            for (int i = 0; i <= Segments; i++)
            {
                float t = MathHelper.Lerp(StartPercent, EndPercent, i / (float)Segments);
                float oneMinusT = 1 - t;

                Vector2 point = (oneMinusT * oneMinusT * oneMinusT * Start.position) +
                                (3 * oneMinusT * oneMinusT * t * Control1.position) +
                                (3 * oneMinusT * t * t * Control2.position) +
                                (t * t * t * End.position);

                points.Add(point);
            }
            return points;
        }

        // Vráti minimálny obal (Bounding Box)
        public RectangleF GetBoundingBox()
        {
            var allPoints = new List<Vector2> { Start.position, End.position, Control1.position, Control2.position };
            float minX = allPoints.Min(p => p.X);
            float minY = allPoints.Min(p => p.Y);
            float maxX = allPoints.Max(p => p.X);
            float maxY = allPoints.Max(p => p.Y);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        // Posunie všetky body krivky o daný vektor
        public void Move(Vector2 delta)
        {
            Start.position += delta;
            End.position += delta;
            Control1.position += delta;
            Control2.position += delta;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            List<Vector2> vector2s = GetPoints();
            for (int i = 0; i < vector2s.Count - 1; i++)
            {
                spriteBatch.DrawLine(vector2s[i], vector2s[i + 1], Color.White, 1f);
            }
        }

        public void DrawPoints(SpriteBatch spriteBatch, Texture2D pointTexture)
        {
            DrawPoint(spriteBatch, pointTexture, Start.position, Color.White);
            DrawPoint(spriteBatch, pointTexture, End.position, Color.White);
        }

        private void DrawPoint(SpriteBatch spriteBatch, Texture2D pointTexture, Vector2 position, Color color)
        {
            pointTexture.SetData(new[] { color });
            spriteBatch.Draw(pointTexture, new Rectangle((int)position.X - 5, (int)position.Y - 5, 10, 10), color);
        }

        public void DrawControlPoints(SpriteBatch spriteBatch, Texture2D pointTexture)
        {
            int HandleSize = 8;
            //DrawPoint(spriteBatch, pointTexture, Control1.position, Color.White);
            //DrawPoint(spriteBatch, pointTexture, Control2.position, Color.White);
            spriteBatch.DrawRectangle(new Rectangle((int)Control1.position.X - HandleSize / 2, (int)Control1.position.Y - HandleSize / 2, HandleSize, HandleSize), Color.White, 1f);
            spriteBatch.DrawRectangle(new Rectangle((int)Control2.position.X - HandleSize / 2, (int)Control2.position.Y - HandleSize / 2, HandleSize, HandleSize), Color.White, 1f);
            // Kreslíme vodiace čiary
            spriteBatch.DrawLine(Start.position, Control1.position, Color.Gray, 1);
            // _spriteBatch.DrawLine(p1, p2, Color.Gray, 1);
            spriteBatch.DrawLine(End.position, Control2.position, Color.Gray, 1);
        }
    }
}
