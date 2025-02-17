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
using Avalonia_Monogame_Dock_Template.Models;
using SkiaSharp;

namespace Avalonia_Monogame_Dock_Template.Monogame;

public class Game1 : Game
{
    private readonly IProjectService _projectService;
    public GraphicsDeviceManager _graphics;
    private AvaloniaGameRenderer _avaloniaRenderer;
    private Point _previousResolution;
    public SpriteBatch _spriteBatch;
    
    public static Game1 Instance { get; private set; }
    public AppEngine AppEngine { get => _appEngine; }

    private bool _mouseLeftPressed = false;
    private bool _mouseLeftPressedPrevious = false;
    public Vector2 _mousePosition;
    private AppEngine _appEngine;
    private EngineMode _engineMode = EngineMode.selectPoints;
    public EngineMode EngineMode { get => _engineMode; set {
            _engineMode = value;
            LoadContent();
        } }

    private Models.ProjectData currentProject;
    public Models.ProjectData CurrentProject { get; set; }

    public Game1(IProjectService projectService)
    {
        Debug.WriteLine("Monogame.Constructor");

        Instance = this;
        this._projectService = projectService;
        GlobalMessageBus.Instance.Listen<EventProjectLoaded>().Subscribe(evt =>
        {
            currentProject = _projectService.getCurrentProjectOrCreateNew();
            CurrentProject = currentProject;
        });

        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        // disable 60fps
         IsFixedTimeStep = false;
         _graphics.SynchronizeWithVerticalRetrace = false;

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
        currentProject = _projectService.getCurrentProjectOrCreateNew();
        CurrentProject = currentProject;

        base.Initialize();

        AppEngine.Initialize();
    }

    private void UpdateAvalonia()
    {
        if (_previousResolution != GraphicsDevice.Viewport.Bounds.Size)
        {
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
        base.LoadContent();
        AppEngine.LoadContent(_spriteBatch);
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
        base.Update(gameTime);
        AppEngine.Update(gameTime);

    }

    protected override void Draw(GameTime gameTime)
    {
        _avaloniaRenderer.Begin();
        _graphics.GraphicsDevice.Clear(Color.SlateGray);

        _spriteBatch.Begin(SpriteSortMode.Immediate);
        base.Draw(gameTime);
        AppEngine.Draw(gameTime);
        _spriteBatch.End();

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
    }

    public void OnMouseReleased()
    {
        _mouseLeftPressed = false;
    }

    internal void OnPointerMoved(Avalonia.Point point)
    {
        _mousePosition = new Vector2((float)point.X, (float)point.Y);
    }

    public Layer getSelectedLayer()
    {
        return _projectService.getSelectedLayer();
    }
}
