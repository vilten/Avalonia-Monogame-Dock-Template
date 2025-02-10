using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Avalonia_Monogame_Dock_Template.Monogame
{
    public class AppEngine
    {
        private SpriteBatch _spriteBatch;

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
            _spriteBatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
            _spriteBatch.DrawCircle(new CircleF(new Vector2(1740, 809), 4), 8, Color.Bisque, 1);
            _spriteBatch.End();
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
