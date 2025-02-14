using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia_Monogame_Dock_Template.Monogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Timers;
using Splat.ModeDetection;

namespace Avalonia_Monogame_Dock_Template.Models.Wrapper
{
    public class AppEngineWrapper(AppEngine appEngine)
    {
        internal void RegisterDraw(EngineMode mode, Action<GameTime> draw)
        {
            appEngine.RegisterDraw(mode, draw);
        }

        internal void RegisterUpdate(EngineMode mode, Action<GameTime> update)
        {
            appEngine.RegisterUpdate(mode, update);
        }

        internal void RegisterLoadContent(EngineMode mode, Action<SpriteBatch> loadContent)
        {
            appEngine.RegisterLoadContent(mode, loadContent);
        }

        internal void RegisterUnloadContent(EngineMode mode, Action unloadContent)
        {
            appEngine.RegisterUnloadContent(mode, unloadContent);
        }

        internal void RegisterDrawVerticle(EngineMode mode, Action<GameTime, Verticle> drawVerticle)
        {
            appEngine.RegisterDrawVerticle(mode, drawVerticle);
        }
    }
}
