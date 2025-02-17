using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Avalonia_Monogame_Dock_Template.Monogame;
using Avalonia_Monogame_Dock_Template.Monogame.Engine;

namespace Avalonia_Monogame_Dock_Template.Services.Plugins
{
    public class PluginsService : IPluginsService
    {
        public List<EnginePluginAbstract> EnginePlugins { get; set; } = new List<EnginePluginAbstract>();

        public PluginsService()
        {
            Debug.WriteLine("Plugins service");
            Type baseType = typeof(EnginePluginAbstract);

            // Získame všetky odvodené triedy
            var pluginTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
                .ToList();

            foreach (var type in pluginTypes)
            {
                EnginePlugins.Add(Activator.CreateInstance(type) as EnginePluginAbstract);
                Debug.WriteLine($"plugin loaded {type.Name}");
            }
        }
    }
}
