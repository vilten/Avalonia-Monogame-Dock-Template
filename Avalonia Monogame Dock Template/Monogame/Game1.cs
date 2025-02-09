using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avalonia_Monogame_Dock_Template.Monogame;

public class Game1 : Game
{
    public GraphicsDeviceManager _graphics;
    private AvaloniaGameRenderer _avaloniaRenderer;
    private Point _previousResolution;
    private SpriteBatch _spriteBatch;

    public static Game1 Instance { get; private set; }
    public Engine Engine { get => _engine; }

    private bool _mouseLeftPressed;
    private Vector2 _mousePosition;
    private Engine _engine;
    private EngineMode _engineMode = EngineMode.edit;
    public EngineMode EngineMode { get => _engineMode; set => _engineMode = value; }

    public Game1()
    {
        Debug.WriteLine("Monogame.Constructor");

        Instance = this;

        _graphics = new GraphicsDeviceManager(this);
        //{
        //    PreferredBackBufferWidth = 1920,  // Šírka okna
        //    PreferredBackBufferHeight = 500, // Výška okna
        //    IsFullScreen = false,              // Fullscreen režim
        //    HardwareModeSwitch = true,
        //};
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // engine init
        _engine = new Engine(this);
    }

    protected override void Initialize()
    {
        Debug.WriteLine("Monogame.Initialize");
        _avaloniaRenderer = new AvaloniaGameRenderer(GraphicsDevice);
        _avaloniaRenderer.Resolution = _previousResolution;
        _graphics.PreferredBackBufferHeight = _previousResolution.Y;
        _graphics.PreferredBackBufferWidth = _previousResolution.X;
        base.Initialize();

        Engine.Initialize();
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
        Engine.LoadContent(_spriteBatch);
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        Debug.WriteLine("Monogame.UnLoadContent");
        Engine.UnloadContent();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {

        UpdateAvalonia();

        Engine.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _avaloniaRenderer.Begin();
        _graphics.GraphicsDevice.Clear(Color.SlateGray);

        Engine.Draw(gameTime);

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
