using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Avalonia_Monogame_Dock_Template.Monogame
{
    public class Engine
    {
        private SpriteBatch _spriteBatch;

        public Game1 Instance { get; private set; }

        public Engine(Game1 _instance)
        {
            this.Instance = _instance;
        }

        public void Initialize()
        {
            Debug.WriteLine("Engine initialize");
        }

        internal void LoadContent(Microsoft.Xna.Framework.Graphics.SpriteBatch _spriteBatch)
        {
            Debug.WriteLine("Engine load content");
            this._spriteBatch = _spriteBatch;
        }

        internal void Update(GameTime gameTime)
        {
        }

        internal void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
            _spriteBatch.DrawCircle(new CircleF(new Vector2(1740, 809), 4), 8, Color.Bisque, 1);
            _spriteBatch.End();
        }

        internal void UnloadContent()
        {
            Debug.WriteLine("Engine unload content");
        }
    }
}
