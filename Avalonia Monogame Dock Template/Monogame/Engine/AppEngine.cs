using System.Collections.Generic;
using System.Diagnostics;
using Avalonia_Monogame_Dock_Template.Monogame.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SkiaSharp;

namespace Avalonia_Monogame_Dock_Template.Monogame
{
    public class AppEngine
    {
        private SpriteBatch _spriteBatch;


        BasicEffect _basicEffect;

        // Zoznam bodov definujúcich polygon (v poradí)
        List<Vector2> _polygonVertices;
        // Indexy trojuholníkov, ktoré vytvorí triangulátor
        List<int> _indices;
        // Vrcholy pre kreslenie
        VertexPositionColor[] _vertices;

        public Game1 Instance { get; private set; }

        public AppEngine(Game1 _instance)
        {
            this.Instance = _instance;
        }

        public void Initialize()
        {
            Debug.WriteLine("Engine initialize");
            switch (this.Instance.EngineMode)
            {
                case EngineMode.create:
                    break;
                case EngineMode.edit:
                    break;
                case EngineMode.animation:
                    break;
            }

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
            _basicEffect = new BasicEffect(Instance.GraphicsDevice)
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

        internal void LoadContent(SpriteBatch _spriteBatch)
        {
            Debug.WriteLine("Engine load content");
            this._spriteBatch = _spriteBatch;
            switch (Instance.EngineMode)
            {
                case EngineMode.create:
                    break;
                case EngineMode.edit:
                    break;
                case EngineMode.animation:
                    break;
            }
        }

        internal void Update(GameTime gameTime)
        {
            switch (Instance.EngineMode)
            {
                case EngineMode.create:
                    break;
                case EngineMode.edit:
                    break;
                case EngineMode.animation:
                    break;
            }
        }

        internal void Draw(GameTime gameTime)
        {
            switch (Instance.EngineMode)
            {
                case EngineMode.create:
                    break;
                case EngineMode.edit:
                    break;
                case EngineMode.animation:
                    break;
            }

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
        }

        internal void UnloadContent()
        {
            Debug.WriteLine("Engine unload content");
            switch (Instance.EngineMode)
            {
                case EngineMode.create:
                    break;
                case EngineMode.edit:
                    break;
                case EngineMode.animation:
                    break;
            }
        }
    }
}
