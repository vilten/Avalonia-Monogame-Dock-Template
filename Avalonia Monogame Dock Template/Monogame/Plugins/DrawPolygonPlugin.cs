using System.Collections.Generic;
using Avalonia_Monogame_Dock_Template.Models.Wrapper;
using Avalonia_Monogame_Dock_Template.Monogame.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avalonia_Monogame_Dock_Template.Monogame.Plugins
{
    public class DrawPolygonPlugin : Monogame.Engine.EnginePluginAbstract
    {
        BasicEffect? _basicEffect;

        // Zoznam bodov definujúcich polygon (v poradí)
        List<Vector2>? _polygonVertices;
        // Indexy trojuholníkov, ktoré vytvorí triangulátor
        List<int>? _indices;
        // Vrcholy pre kreslenie
        VertexPositionColor[]? _vertices;

        public override string EnginePluginName => "DrawPolygonPlugin";

        public override void Initialize()
        {
            base.Initialize();
            // Definujte body pre polygon – napríklad štvoruholník
            _polygonVertices = new List<Vector2>
            {
                new Vector2(100, 100),
                new Vector2(200, 100),
                new Vector2(50, 200),
                new Vector2(200, 200),
                new Vector2(200, 300),
                new Vector2(100, 300),
                new Vector2(100, 200),
            };

            // Triangulácia polygonu pomocou EarClipTriangulator
            // Metóda Triangulate vracia IEnumerable<int> s indexami trojuholníkov
            _indices = new List<int>(Triangulator.Triangulate(_polygonVertices));

            // Prekonvertujeme body na VertexPositionColor (s farbou výplne, napr. Red)
            _vertices = new VertexPositionColor[_polygonVertices.Count];
            for (int i = 0; i < _polygonVertices.Count; i++)
            {
                // Z balíčka Vector2 vytvoríme Vector3 (z-ová súradnica = 0)
                _vertices[i] = new VertexPositionColor(new Vector3(_polygonVertices[i], 0), Color.Red);
            }

            // Inicializácia BasicEffect – nastavíme premenné pre 2D kreslenie
            this._basicEffect = new BasicEffect(Instance.GraphicsDevice)
            {
                VertexColorEnabled = true,
                Projection = Matrix.CreateOrthographicOffCenter
                (
                    0,
                    Instance.GraphicsDevice.Viewport.Width,
                    Instance.GraphicsDevice.Viewport.Height,
                    0,
                    0,
                    1
                )
            };

        }

        public override void Draw(GameTime gameTime)
        {
            if (_basicEffect != null && _vertices != null && _indices != null)
                foreach (var pass in _basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    Instance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                        PrimitiveType.TriangleList,
                        _vertices,      // pole vrcholov
                        0,              // offset
                        _vertices.Length,
                        _indices.ToArray(), // pole indexov
                        0,
                        _indices.Count / 3 // počet trojuholníkov
                    );
                }
            base.Draw(gameTime);
        }

        void LoadContent(SpriteBatch spriteBatch)
        {

        }

        void UnloadContent() { }

        public override void Register(AppEngineWrapper appEngineWrapper)
        {
            appEngineWrapper.RegisterLoadContent(EngineMode.all, LoadContent);
            appEngineWrapper.RegisterUnloadContent(EngineMode.all, UnloadContent);
        }
    }
}
