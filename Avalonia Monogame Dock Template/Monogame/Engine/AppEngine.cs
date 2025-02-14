using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia_Monogame_Dock_Template.Models;
using Avalonia_Monogame_Dock_Template.Models.Wrapper;
using Avalonia_Monogame_Dock_Template.Services.Plugins;
using DynamicData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Splat;

namespace Avalonia_Monogame_Dock_Template.Monogame
{
    public class AppEngine
    {
        private readonly IPluginsService _pluginsService;
        private AppEngineWrapper appEngineWrapper;
        private SpriteBatch? _spriteBatch;
        private Game1 _instance;

        private Dictionary<EngineMode, List<Action<SpriteBatch>>> _loadContentCalls { get; set; } = new Dictionary<EngineMode, List<Action<SpriteBatch>>>();
        private Dictionary<EngineMode, List<Action>> _unloadContentCalls { get; set; } = new Dictionary<EngineMode, List<Action>>();
        private Dictionary<EngineMode, List<Action<GameTime>>> _updateContentCalls { get; set; } = new Dictionary<EngineMode, List<Action<GameTime>>>();
        private Dictionary<EngineMode, List<Action<GameTime>>> _drawContentCalls { get; set; } = new Dictionary<EngineMode, List<Action<GameTime>>>();
        private Dictionary<EngineMode, List<Action<GameTime, Verticle>>> _drawVerticleCalls { get; set; } = new Dictionary<EngineMode, List<Action<GameTime, Verticle>>>();


        public AppEngine(Game1 instance)
        {
            appEngineWrapper = new AppEngineWrapper(this);
            _instance = instance;
            Instance = _instance;
            _pluginsService = Locator.Current.GetService<IPluginsService>() ?? throw new InvalidOperationException("IPluginsService not registered");
            _pluginsService?.EnginePlugins.ForEach(plugin =>
            {
                plugin.Instance = Instance;
                plugin.Register(appEngineWrapper);
            });

        }

        public Game1 Instance { get; private set; }

        public void Initialize()
        {
            _pluginsService?.EnginePlugins.ForEach(plugin =>
            {
                plugin.Instance = Instance;
                plugin.Initialize();
            });
        }

        internal void LoadContent(SpriteBatch _spriteBatch)
        {
            this._spriteBatch = _spriteBatch;
            _pluginsService?.EnginePlugins.ForEach(plugin =>
            {
                plugin.Instance = Instance;
                //plugin.LoadContent(_spriteBatch);
            });
            if (_loadContentCalls.ContainsKey(Instance.EngineMode))
                foreach (var loadContent in _loadContentCalls[Instance.EngineMode])
                {
                    loadContent(_spriteBatch);
                };
            if (_loadContentCalls.ContainsKey(EngineMode.all))
                foreach (var loadContent in _loadContentCalls[EngineMode.all])
                {
                    loadContent(_spriteBatch);
                };

        }

        internal void Update(GameTime gameTime)
        {
            if (_updateContentCalls.ContainsKey(Instance.EngineMode))
                foreach (var update in _updateContentCalls[Instance.EngineMode])
                {
                    update(gameTime);
                };
            if (_updateContentCalls.ContainsKey(EngineMode.all))
                foreach (var update in _updateContentCalls[EngineMode.all])
                {
                    update(gameTime);
                };
        }

        internal void Draw(GameTime gameTime)
        {
            for (int index = Instance.CurrentProject.Layers.Count - 1; -1 < index; index--)
            {
                var layer = Instance.CurrentProject.Layers[index];
                if (layer.ViewportVisible)
                    for (int indexItems = 0; layer.LayerItems.Count > indexItems; indexItems++)
                    {
                        var layerItem = layer.LayerItems[indexItems];
                        switch (layerItem.LayerItemType)
                        {
                            case LayerItemType.Point:
                                for (int i = 0; i < layerItem.Verticles.Count; i++)
                                {
                                    if (_drawVerticleCalls.ContainsKey(Instance.EngineMode))
                                        foreach (var drawVerticle in _drawVerticleCalls[Instance.EngineMode])
                                        {
                                            drawVerticle(gameTime, layerItem.Verticles[i]);
                                        };
                                    if (_drawVerticleCalls.ContainsKey(EngineMode.all))
                                        foreach (var drawVerticle in _drawVerticleCalls[EngineMode.all])
                                        {
                                            drawVerticle(gameTime, layerItem.Verticles[i]);
                                        };
                                }
                                break;
                        }
                    }
            }
            if (_drawContentCalls.ContainsKey(Instance.EngineMode))
                foreach (var draw in _drawContentCalls[Instance.EngineMode])
                {
                    draw(gameTime);
                };
            if (_drawContentCalls.ContainsKey(EngineMode.all))
                foreach (var draw in _drawContentCalls[EngineMode.all])
                {
                    draw(gameTime);
                };

        }

        internal void UnloadContent()
        {
            Trace.WriteLine("Engine unload content");
            if (_unloadContentCalls.ContainsKey(Instance.EngineMode))
                foreach (var unloadContent in _unloadContentCalls[Instance.EngineMode])
                {
                    unloadContent();
                };
            if (_unloadContentCalls.ContainsKey(EngineMode.all))
                foreach (var unloadContent in _unloadContentCalls[EngineMode.all])
                {
                    unloadContent();
                };
        }

        internal void RegisterLoadContent(EngineMode mode, Action<SpriteBatch> loadContent)
        {
            if (!_loadContentCalls.ContainsKey(mode))
                _loadContentCalls.Add(mode, new List<Action<SpriteBatch>>());
            _loadContentCalls[mode].Add(loadContent);
        }

        internal void RegisterUnloadContent(EngineMode mode, Action unloadContent)
        {
            if (!_unloadContentCalls.ContainsKey(mode))
                _unloadContentCalls.Add(mode, new List<Action>());
            _unloadContentCalls[mode].Add(unloadContent);
        }

        internal void RegisterDraw(EngineMode mode, Action<GameTime> draw)
        {
            if (!_drawContentCalls.ContainsKey(mode))
                _drawContentCalls.Add(mode, new List<Action<GameTime>>());
            _drawContentCalls[mode].Add(draw);
        }

        internal void RegisterUpdate(EngineMode mode, Action<GameTime> update)
        {
            if (!_updateContentCalls.ContainsKey(mode))
                _updateContentCalls.Add(mode, new List<Action<GameTime>>());
            _updateContentCalls[mode].Add(update);
        }

        internal void RegisterDrawVerticle(EngineMode mode, Action<GameTime, Verticle> drawVerticle)
        {
            if (!_drawVerticleCalls.ContainsKey(mode))
                _drawVerticleCalls.Add(mode, new List<Action<GameTime, Verticle>>());
            _drawVerticleCalls[mode].Add(drawVerticle);
        }
    }
}
