using System.Collections.Generic;
using Avalonia_Monogame_Dock_Template.Models;
using Avalonia_Monogame_Dock_Template.Models.Wrapper;
using IconPacks.Avalonia.PhosphorIcons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Avalonia_Monogame_Dock_Template.Monogame.Plugins
{
    public class PolygonPlugin : Engine.EnginePluginAbstract
    {
        public override string EnginePluginName => "PolygonPlugin";
        Texture2D pixelTexture;

        public override void Register(AppEngineWrapper appEngineWrapper)
        {
            appEngineWrapper.RegisterLoadContent(EngineMode.all, LoadContent);
            appEngineWrapper.RegisterDrawPolygon(EngineMode.all, DrawPolygon);
            appEngineWrapper.RegisterDraw(EngineMode.all, Draw);
            appEngineWrapper.RegisterUnloadContent(EngineMode.all, UnloadContent);
        }

        void LoadContent(SpriteBatch _spriteBatch)
        {
            pixelTexture = new Texture2D(Instance.GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
        }

        public void DrawPolygon(GameTime gameTime, BezierPolygon bezierPolygon)
        {
            // TODO
            //bezierPolygon.DrawFilled(Instance._spriteBatch, BasicEffect.);
        }

        public override void RegisterToolGroups(RightToolsPanelWrapper rightToolsPanelWrapper)
        {
            rightToolsPanelWrapper.RegisterGroup("Polygons");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            //Debug.WriteLine($"Draw task: {EnginePluginName}");
            var v1 = new Verticle { position = new Vector2(100, 100) };
            var v2 = new Verticle { position = new Vector2(400, 100) };
            var v3 = new Verticle { position = new Vector2(400, 400) };
            var v4 = new Verticle { position = new Vector2(100, 400) };

            var c1 = new Verticle { position = new Vector2(120, 120) };
            var c11 = new Verticle { position = new Vector2(80, 80) };
            var c2 = new Verticle { position = new Vector2(380, 120) };
            var c22 = new Verticle { position = new Vector2(420, 120) };
            var c3 = new Verticle { position = new Vector2(380, 380) };
            var c33 = new Verticle { position = new Vector2(420, 420) };
            var c4 = new Verticle { position = new Vector2(80, 380) };
            var c44 = new Verticle { position = new Vector2(120, 420) };

            var curve1 = new BezierCurve(v1, c1, c2, v2);
            var curve2 = new BezierCurve(v2, c22, c3, v3);
            var curve3 = new BezierCurve(v3, c33, c4, v4);
            var curve4 = new BezierCurve(v4, c44, c11, v1); // Uzatvorenie

            curve1.Draw(Instance._spriteBatch);
            curve1.DrawPoints(Instance._spriteBatch, pixelTexture);
            curve1.DrawControlPoints(Instance._spriteBatch, pixelTexture);
            curve2.Draw(Instance._spriteBatch);
            curve2.DrawPoints(Instance._spriteBatch, pixelTexture);
            curve2.DrawControlPoints(Instance._spriteBatch, pixelTexture);
            curve3.Draw(Instance._spriteBatch);
            curve3.DrawPoints(Instance._spriteBatch, pixelTexture);
            curve3.DrawControlPoints(Instance._spriteBatch, pixelTexture);
            curve4.Draw(Instance._spriteBatch);
            curve4.DrawPoints(Instance._spriteBatch, pixelTexture);
            curve4.DrawControlPoints(Instance._spriteBatch, pixelTexture);

            var bezierPolygon = new BezierPolygon(new List<BezierCurve> { curve1, curve2, curve3, curve4 });

            Instance._spriteBatch.DrawRectangle(bezierPolygon.GetBoundingBox(), Color.Yellow);
            //bezierPolygon.Draw(Instance._spriteBatch, pixelTexture, Color.Red);
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
