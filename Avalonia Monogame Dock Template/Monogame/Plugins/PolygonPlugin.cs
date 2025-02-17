using System.Collections.Generic;
using Avalonia_Monogame_Dock_Template.Models;
using Avalonia_Monogame_Dock_Template.Models.Wrapper;
using IconPacks.Avalonia.PhosphorIcons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using Poly2Tri;
using System.Linq;

namespace Avalonia_Monogame_Dock_Template.Monogame.Plugins
{
    public class PolygonPlugin : Engine.EnginePluginAbstract
    {
        public override string EnginePluginName => "PolygonPlugin";
        Texture2D pixelTexture;
        private BasicEffect _basicEffect;
        private VertexPositionColor[] _polygonVertices;
        private short[] _indices;

        public override void Register(AppEngineWrapper appEngineWrapper)
        {
            appEngineWrapper.RegisterLoadContent(EngineMode.selectPolygons, LoadContent);
            appEngineWrapper.RegisterDrawPolygon(EngineMode.selectPolygons, DrawPolygon);
            appEngineWrapper.RegisterDraw(EngineMode.selectPolygons, Draw);
            appEngineWrapper.RegisterUnloadContent(EngineMode.selectPolygons, UnloadContent);
        }

        void LoadContent(SpriteBatch _spriteBatch)
        {
            pixelTexture = new Texture2D(Instance.GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
            _basicEffect = new BasicEffect(Instance.GraphicsDevice)
            {
                VertexColorEnabled = true,  // Umožňuje farbu vertexov
                LightingEnabled = false     // Osvetlenie vypnuté pre jednoduchosť
            };

            // Nastavenie základných matíc (svetový, pohľadový a projekčný transform)
            _basicEffect.World = Matrix.Identity; // Žiadna transformácia
            _basicEffect.View = Matrix.CreateLookAt(
                new Vector3(0, 0, 10),  // Kamera na pozícii (0,0,10)
                Vector3.Zero,            // Pozeráme na stred scény
                Vector3.Up               // "Hore" je os Y
            );
            _basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                Instance.GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f
            );



            List<Vector2> polygonPoints = new List<Vector2>
                {
                    new Vector2(100, 100),
                    new Vector2(200, 50),
                    new Vector2(300, 100),
                    new Vector2(250, 200),
                    new Vector2(150, 200)
                };

            Poly2Tri.Triangulation.Polygon.Polygon polygon = new Poly2Tri.Triangulation.Polygon.Polygon(polygonPoints.Select(p => new Poly2Tri.Triangulation.Polygon.PolygonPoint(p.X, p.Y)).ToList());

            // Triangulácia polygonu pomocou Poly2Tri
            P2T.Triangulate(polygon);

            // Konverzia výsledných trojuholníkov na vertexy
            _polygonVertices = polygon.Triangles.SelectMany(t => new[]
            {
                new VertexPositionColor(new Vector3((float)t.Points[0].X, (float)t.Points[0].Y, 0), Color.Red),
                new VertexPositionColor(new Vector3((float)t.Points[1].X, (float)t.Points[1].Y, 0), Color.Green),
                new VertexPositionColor(new Vector3((float)t.Points[2].X, (float)t.Points[2].Y, 0), Color.Blue)
            }).ToArray();

            _indices = Enumerable.Range(0, _polygonVertices.Length).Select(i => (short)i).ToArray();

        }

        public void DrawPolygon(GameTime gameTime, BezierPolygon bezierPolygon)
        {
            // TODO
            // bezierPolygon.DrawFilled(Instance.GraphicsDevice, _basicEffect);
            bezierPolygon.Curves.ForEach(curve =>
            {
                curve.Draw(Instance._spriteBatch);
                curve.DrawPoints(Instance._spriteBatch, pixelTexture);
                curve.DrawControlPoints(Instance._spriteBatch, pixelTexture);
            });
        }

        public override void RegisterToolGroups(RightToolsPanelWrapper rightToolsPanelWrapper)
        {
            rightToolsPanelWrapper.RegisterGroup("Polygons");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            // Instance._spriteBatch.DrawRectangle(testBezierPolygon.GetBoundingBox(), Color.Yellow);
            // bezierPolygon.Draw(Instance._spriteBatch, pixelTexture, Color.Red);
            _basicEffect.CurrentTechnique.Passes[0].Apply();

            Instance.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                _polygonVertices,
                0,
                _polygonVertices.Length,
                _indices,
                0,
                _polygonVertices.Length / 3
            );
        }

        public override void RegisterButtonInGroup(RightToolsPanelWrapper rightToolsPanelWrapper)
        {
            rightToolsPanelWrapper.RegisterButtonInGroup("Polygons", new ToolButton(msg =>
            {
                Instance.EngineMode = EngineMode.selectPolygons;
            })
            {
                Name = "Select Polygons",
                Icon = PackIconPhosphorIconsKind.Selection,
                MessageType = "toolButton.selectPolygons"
            });
            rightToolsPanelWrapper.RegisterButtonInGroup("Polygons", new ToolButton(msg =>
            {
                Instance.EngineMode = EngineMode.createPolygons;
            })
            {
                Name = "Create Polygons",
                Icon = PackIconPhosphorIconsKind.BezierCurve,
                MessageType = "toolButton.createPolygons"
            });
            rightToolsPanelWrapper.RegisterButtonInGroup("Polygons", new ToolButton(msg =>
            {
                Instance.EngineMode = EngineMode.transformPolygons;
            })
            {
                Name = "Transform Polygons",
                Icon = PackIconPhosphorIconsKind.ArrowsOutCardinal,
                MessageType = "toolButton.transformPolygons"
            });
        }

        void UnloadContent() { }
    }
}
