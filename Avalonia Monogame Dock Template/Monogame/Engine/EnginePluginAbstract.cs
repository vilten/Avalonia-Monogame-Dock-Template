using System.Collections.Generic;
using System.Diagnostics;
using Avalonia_Monogame_Dock_Template.Models;
using Avalonia_Monogame_Dock_Template.Models.Wrapper;
using Avalonia_Monogame_Dock_Template.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avalonia_Monogame_Dock_Template.Monogame.Engine
{
    public abstract class EnginePluginAbstract()
    {
        public Game1 Instance { get; set; }

        // Abstraktná metóda, ktorú musia odvodené triedy implementovať
        public abstract string EnginePluginName { get; }

        public abstract void Register(AppEngineWrapper appEngineWrapper);

        // Voliteľná virtuálna metóda, ktorá môže byť volaná pred alebo po Execute
        public virtual void Initialize()
        {
            Trace.WriteLine($"Initializing task: {EnginePluginName}");
        }

        public virtual void Update(GameTime gameTime)
        {
            //Debug.WriteLine($"Update task: {EnginePluginName}");
        }

        public virtual void Draw(GameTime gameTime)
        {
            //Debug.WriteLine($"Draw task: {EnginePluginName}");
        }

        public virtual void RegisterToolGroups(RightToolsPanelWrapper rightToolsPanelWrapper)
        {
            Trace.WriteLine($"Register tool groups task: {EnginePluginName}");
        }

        public virtual void RegisterButtonInGroup(RightToolsPanelWrapper rightToolsPanelWrapper)
        {
            Trace.WriteLine($"Register button into groups task: {EnginePluginName}");
        }
    }
}
