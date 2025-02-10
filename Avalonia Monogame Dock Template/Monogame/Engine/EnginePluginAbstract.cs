using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avalonia_Monogame_Dock_Template.Monogame.Engine
{
    public abstract class EnginePluginAbstract
    {
        // Abstraktná metóda, ktorú musia odvodené triedy implementovať
        public abstract string EnginePluginName { get; }

        public abstract void Execute();

        // Voliteľná virtuálna metóda, ktorá môže byť volaná pred alebo po Execute
        public virtual void Initialize()
        {
            Debug.WriteLine($"Initializing task: {EnginePluginName}");
        }

        public virtual void LoadContent(SpriteBatch _spriteBatch)
        {
            Debug.WriteLine($"LoadContent task: {EnginePluginName}");
        }

        public virtual void Update(GameTime gameTime)
        {
            Debug.WriteLine($"Update task: {EnginePluginName}");
        }

        public virtual void Draw(GameTime gameTime)
        {
            Debug.WriteLine($"Draw task: {EnginePluginName}");
        }

        public virtual void UnloadContent()
        {
            Debug.WriteLine($"UnloadContent task: {EnginePluginName}");
        }
    }
}
