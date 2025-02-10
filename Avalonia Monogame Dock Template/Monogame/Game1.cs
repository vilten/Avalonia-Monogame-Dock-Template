using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls.Shapes;
using Avalonia_Monogame_Dock_Template.Events.Project;
using Avalonia_Monogame_Dock_Template.Events;
using Avalonia_Monogame_Dock_Template.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;

namespace Avalonia_Monogame_Dock_Template.Monogame;

public class Game1 : Game
{
    private readonly IProjectService _projectService;
    public GraphicsDeviceManager _graphics;
    private AvaloniaGameRenderer _avaloniaRenderer;
    private Point _previousResolution;
    private SpriteBatch _spriteBatch;
    
    public static Game1 Instance { get; private set; }
    public AppEngine AppEngine { get => _appEngine; }

    private bool _mouseLeftPressed = false;
    private bool _mouseLeftPressedPrevious = false;
    private Vector2 _mousePosition;
    private AppEngine _appEngine;
    private EngineMode _engineMode = EngineMode.edit;
    public EngineMode EngineMode { get => _engineMode; set => _engineMode = value; }

    private List<Vector2> _points = new List<Vector2>();
    private Models.ProjectData currentProject;

    public Game1(IProjectService projectService)
    {
        Debug.WriteLine("Monogame.Constructor");

        Instance = this;
        this._projectService = projectService;
        GlobalMessageBus.Instance.Listen<EventProjectLoaded>().Subscribe(evt =>
        {
            currentProject = this._projectService.CurrentProject;
        });

        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // engine init
        _appEngine = new AppEngine(this);
    }

    protected override void Initialize()
    {
        Debug.WriteLine("Monogame.Initialize");
        _avaloniaRenderer = new AvaloniaGameRenderer(GraphicsDevice);
        _avaloniaRenderer.Resolution = _previousResolution;
        _graphics.PreferredBackBufferHeight = _previousResolution.Y;
        _graphics.PreferredBackBufferWidth = _previousResolution.X;
        currentProject = _projectService.CurrentProject;

        base.Initialize();
        _points.Add(new Vector2(0, 0));

        AppEngine.Initialize();
    }

    private void UpdateAvalonia()
    {
        if (_previousResolution != GraphicsDevice.Viewport.Bounds.Size)
        {
            Debug.WriteLine($"Monogame.UpdateAvalonia {GraphicsDevice.Viewport.Bounds.Width}");
            _previousResolution = GraphicsDevice.Viewport.Bounds.Size;
            _avaloniaRenderer.Resolution = _previousResolution;
            _graphics.PreferredBackBufferHeight = _previousResolution.Y;
            _graphics.PreferredBackBufferWidth = _previousResolution.X;
            _graphics.ApplyChanges();
        }
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Debug.WriteLine("Monogame.LoadContent");
        AppEngine.LoadContent(_spriteBatch);
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        Debug.WriteLine("Monogame.UnLoadContent");
        AppEngine.UnloadContent();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {

        UpdateAvalonia();

        AppEngine.Update(gameTime);

        if (_mouseLeftPressed != _mouseLeftPressedPrevious)
        {
            _points.Add(_mousePosition);
        }
        _mouseLeftPressedPrevious = _mouseLeftPressed;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _avaloniaRenderer.Begin();
        _graphics.GraphicsDevice.Clear(Color.SlateGray);

        AppEngine.Draw(gameTime);
        _spriteBatch.Begin(SpriteSortMode.Immediate);
        foreach (Vector2 point in _points)
        {
            //_spriteBatch.DrawCircle(new CircleF(point, 4), 8, Color.White, 1);
            //_spriteBatch.DrawRectangle(new RectangleF(point - new Vector2(2,2), new SizeF(6,6)), Color.White, 1);
            List<Vector2> vector2s = new List<Vector2>();
            vector2s.Add(new Vector2(0,0));
            vector2s.Add(new Vector2(0,10));
            vector2s.Add(new Vector2(10,10));
            MonoGame.Extended.Shapes.Polygon polygon = new MonoGame.Extended.Shapes.Polygon(vector2s);
            _spriteBatch.DrawPolygon(point, polygon, Color.Blue,1,1);
        }
        _spriteBatch.End();

        base.Draw(gameTime);
        _avaloniaRenderer.End();
    }

    internal void OnPointerWheelChanged(float y)
    {
        var zoomDelta = y > 0 ? 0.1f : -0.1f;
        //_camera.Zoom += zoomDelta;
    }

    public void OnMousePressed()
    {
        _mouseLeftPressed = true;
        Debug.WriteLine($"pressed {_mousePosition}");
    }

    public void OnMouseReleased()
    {
        _mouseLeftPressed = false;
        Debug.WriteLine($"released {_mousePosition}");
    }

    internal void OnPointerMoved(Avalonia.Point point)
    {
        _mousePosition = new Vector2((float)point.X, (float)point.Y);
    }
}
