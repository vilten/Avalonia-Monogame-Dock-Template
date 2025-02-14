using System.Collections.Generic;
using Avalonia_Monogame_Dock_Template.Monogame;
using Avalonia_Monogame_Dock_Template.Monogame.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avalonia_Monogame_Dock_Template.Services.Plugins
{
    public interface IPluginsService
    {
        List<EnginePluginAbstract> EnginePlugins { get; set; }
    }
}
