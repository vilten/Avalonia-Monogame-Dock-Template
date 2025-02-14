using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia_Monogame_Dock_Template.Controls;
using Avalonia_Monogame_Dock_Template.Events;
using Avalonia_Monogame_Dock_Template.Models;
using Avalonia_Monogame_Dock_Template.Models.Wrapper;
using IconPacks.Avalonia.PhosphorIcons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Avalonia_Monogame_Dock_Template.Monogame.Plugins
{
    public class PointsPlugin : Engine.EnginePluginAbstract
    {
        Texture2D pixelTexture;
        public override string EnginePluginName => "PointsPlugin";
        MouseState _previousMouseState = Mouse.GetState();
        private Vector2 _startSelection;  // Počiatočný bod
        private bool _isSelecting = false; // Indikátor, či sa kreslí výber
        private readonly List<Verticle> _selectedVerticles = new List<Verticle>();
        private TransformableRectangle transformRect;
        PackIconPhosphorIconsKind _previousCursorType = PackIconPhosphorIconsKind.None;

        void LoadContent(SpriteBatch _spriteBatch)
        {
            Debug.WriteLine("kuravaaaaaaa");
            pixelTexture = new Texture2D(Instance.GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
        }

        void UnloadContent() { }

        public void DrawVerticle(GameTime gameTime, Verticle verticle)
        {
            // Farba bodu – tu červená s 50 % opacity:
            Color pointColor = verticle.Color * verticle.opacity;
            if (_selectedVerticles.Contains(verticle))
                pointColor = Color.Red;

            // Vykreslíme "bod" ako štvorec so stredom v pointPosition.
            // Aby bol stred štvoreca na pointPosition, odpočítame polovicu veľkosti.
            Rectangle destination = new Rectangle(
                (int)(verticle.position.X - verticle.size / 2),
                (int)(verticle.position.Y - verticle.size / 2),
                (int)verticle.size,
                (int)verticle.size);

            Instance._spriteBatch.Draw(pixelTexture, destination, pointColor);
        }

        public void DrawSelectPoints(GameTime gameTime)
        {
            if (_isSelecting)
            {
                var min = Vector2.Min(Instance._mousePosition, _startSelection);
                var max = Vector2.Max(Instance._mousePosition, _startSelection);
                Rectangle selectionRectangle = new Rectangle((int)min.X, (int)min.Y, (int)max.X - (int)min.X, (int)max.Y - (int)min.Y);

                DrawDashedRectangle(selectionRectangle, Color.LightGray);
            }
        }

        private void DrawDashedRectangle(Rectangle rect, Color color)
        {
            int dashLength = 5;
            int gapLength = 3;

            // Kreslenie vrchného a spodného okraja
            for (int x = rect.Left; x < rect.Right; x += dashLength + gapLength)
            {

                Instance._spriteBatch.Draw(pixelTexture, new Rectangle(x, rect.Top, dashLength, 1), color);
                Instance._spriteBatch.Draw(pixelTexture, new Rectangle(x, rect.Bottom, dashLength, 1), color);
            }

            // Kreslenie ľavého a pravého okraja
            for (int y = rect.Top; y < rect.Bottom; y += dashLength + gapLength)
            {
                Instance._spriteBatch.Draw(pixelTexture, new Rectangle(rect.Left, y, 1, dashLength), color);
                Instance._spriteBatch.Draw(pixelTexture, new Rectangle(rect.Right, y, 1, dashLength), color);
            }
        }

        public override void RegisterToolGroups(RightToolsPanelWrapper rightToolsPanelWrapper)
        {
            rightToolsPanelWrapper.RegisterGroup("Points");
        }

        public override void RegisterButtonInGroup(RightToolsPanelWrapper rightToolsPanelWrapper)
        {
            rightToolsPanelWrapper.RegisterButtonInGroup("Points", new ToolButton(msg =>
                {
                    Instance.EngineMode = EngineMode.selectPoints;
                })
            {
                Name = "Select Points",
                Icon = PackIconPhosphorIconsKind.Selection,
                MessageType = "toolButton.selectPoints"
            });
            rightToolsPanelWrapper.RegisterButtonInGroup("Points", new ToolButton(msg =>
                {
                    Instance.EngineMode = EngineMode.createPoints;
                })
            {
                Name = "Create Points",
                Icon = PackIconPhosphorIconsKind.Pencil,
                MessageType = "toolButton.createPoints"
            });
            rightToolsPanelWrapper.RegisterButtonInGroup("Points", new ToolButton(msg =>
                {
                    Instance.EngineMode = EngineMode.transformPoints;
                })
            {
                Name = "Transform Points",
                Icon = PackIconPhosphorIconsKind.ArrowsOutCardinal,
                MessageType = "toolButton.transformPoints"
            });
        }

        public override void Register(AppEngineWrapper appEngineWrapper)
        {
            appEngineWrapper.RegisterLoadContent(EngineMode.all, LoadContent);
            appEngineWrapper.RegisterDrawVerticle(EngineMode.all, DrawVerticle);
            appEngineWrapper.RegisterUpdate(EngineMode.createPoints, UpdateCreatePoints);
            appEngineWrapper.RegisterUpdate(EngineMode.selectPoints, UpdateSelectPoints);
            appEngineWrapper.RegisterUpdate(EngineMode.transformPoints, UpdateTransformPoints);
            appEngineWrapper.RegisterDraw(EngineMode.selectPoints, DrawSelectPoints);
            appEngineWrapper.RegisterDraw(EngineMode.transformPoints, DrawTransformPoints);
            appEngineWrapper.RegisterUnloadContent(EngineMode.all, UnloadContent);
        }

        private void DrawTransformPoints(GameTime time)
        {
            if (transformRect != null)
                transformRect.Draw(Instance._spriteBatch);
        }

        private void UpdateTransformPoints(GameTime time)
        {
            MouseState mouseState = Mouse.GetState();
            if (_selectedVerticles.Count > 0 && transformRect == null)
            {
                var minVerticle = _selectedVerticles[0].position;
                var maxVerticle = _selectedVerticles[0].position;
                foreach (var verticle in _selectedVerticles)
                {
                    minVerticle.X = float.Min(minVerticle.X, verticle.position.X);
                    minVerticle.Y = float.Min(minVerticle.Y, verticle.position.Y);
                    maxVerticle.X = float.Max(maxVerticle.X, verticle.position.X);
                    maxVerticle.Y = float.Max(maxVerticle.Y, verticle.position.Y);
                }
                transformRect = new TransformableRectangle(Instance.GraphicsDevice, minVerticle, maxVerticle - minVerticle, _selectedVerticles);
            }
            if (transformRect != null)
                transformRect.Update(mouseState, _previousMouseState, Instance._mousePosition);
            _previousMouseState = mouseState;
        }

        private void UpdateCreatePoints(GameTime time)
        {
            PackIconPhosphorIconsKind curorType = PackIconPhosphorIconsKind.Cursor;
            if (_previousCursorType != curorType)
            {
                GlobalMessageBus.Instance.SendMessage(curorType);
                _previousCursorType = curorType;
            }

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released && Instance._mousePosition != new Vector2(-1, -1))
            {
                Debug.WriteLine("kfdsa343");
                LayerItem layerItem = new LayerItem();
                layerItem.LayerItemType = LayerItemType.Point;
                Verticle verticle = new Verticle()
                {
                    position = Instance._mousePosition,
                    opacity = 1,
                    size = 10,
                    Color = Color.White,
                };
                layerItem.Verticles.Add(verticle);
                Instance.getSelectedLayer().LayerItems.Add(layerItem);
            }
            _previousMouseState = mouseState;
        }

        private void UpdateSelectPoints(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && !_isSelecting)
            {
                _isSelecting = true;
                _startSelection = Instance._mousePosition;
            }

            if (mouseState.LeftButton == ButtonState.Released && _isSelecting)
            {
                _isSelecting = false;
                EvaluateSelection(); // Vyhodnotenie vybranej oblasti
            }
            PackIconPhosphorIconsKind curorType = PackIconPhosphorIconsKind.Selection;
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                curorType = PackIconPhosphorIconsKind.SelectionPlus;
            if (keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt))
                curorType = PackIconPhosphorIconsKind.SelectionSlash;
            if (_previousCursorType != curorType)
            {
                GlobalMessageBus.Instance.SendMessage(curorType);
                _previousCursorType = curorType;
            }

        }

        private void EvaluateSelection()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (!keyboardState.IsKeyDown(Keys.LeftShift) && !keyboardState.IsKeyDown(Keys.RightShift) && !keyboardState.IsKeyDown(Keys.LeftAlt) && !keyboardState.IsKeyDown(Keys.RightAlt))
            {
                _selectedVerticles.Clear();
                transformRect = null;
            }
            var min = Vector2.Min(Instance._mousePosition, _startSelection);
            var max = Vector2.Max(Instance._mousePosition, _startSelection);
            var altPressed = !keyboardState.IsKeyDown(Keys.LeftAlt) && !keyboardState.IsKeyDown(Keys.RightAlt);
            Rectangle selectionRectangle = new Rectangle((int)min.X, (int)min.Y, (int)max.X - (int)min.X, (int)max.Y - (int)min.Y);
            foreach (var layerItem in Instance.getSelectedLayer().LayerItems)
            {
                foreach (var verticle in layerItem.Verticles)
                {
                    if (selectionRectangle.Contains(verticle.position) && !_selectedVerticles.Contains(verticle) && altPressed)
                        _selectedVerticles.Add(verticle);
                    if (selectionRectangle.Contains(verticle.position) && _selectedVerticles.Contains(verticle) && !altPressed)
                        _selectedVerticles.Remove(verticle);
                    //transformRect = null;
                }
            }

            Debug.WriteLine($"Selected Area: {selectionRectangle}");
        }
    }
}

/*
using System;
using System.Linq;
using System.Reflection;

public abstract class EnginePluginAbstract
{
    public virtual void Initialize()
    {
        Console.WriteLine("Default EnginePluginAbstract Initialize");
    }
}

// Príkladové pluginy
public class MyPlugin1 : EnginePluginAbstract
{
    public override void Initialize() => Console.WriteLine("MyPlugin1 initialized");
}

public class MyPlugin2 : EnginePluginAbstract
{
    public override void Initialize() => Console.WriteLine("MyPlugin2 initialized");
}

public class MyPluginWithoutOverride : EnginePluginAbstract
{
    // Táto trieda neprepisuje Initialize()
}

class Program
{
    static void Main()
    {
        Type baseType = typeof(EnginePluginAbstract);
        string methodName = "Initialize"; // Názov metódy, ktorú hľadáme

        // Získame všetky odvodené triedy
        var pluginTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t))
            .ToList();

        foreach (var type in pluginTypes)
        {
            // Skontrolujeme, či má metódu 'Initialize' override
            var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            if (method != null) // Override existuje
            {
                Console.WriteLine($"Initializing plugin: {type.Name}");
                var instance = (EnginePluginAbstract)Activator.CreateInstance(type);
                instance.Initialize();
            }
            else
            {
                Console.WriteLine($"Skipping {type.Name} (no override of {methodName})");
            }
        }
    }
}

*/