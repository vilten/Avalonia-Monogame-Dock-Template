using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Avalonia_Monogame_Dock_Template.Monogame;

public class Game1 : Game
{
    public string _projectPath = "../../../../Projects/test/";
    public string _projectFilename = "project.vanim";
    public GraphicsDeviceManager _graphics;
    private AvaloniaGameRenderer _avaloniaRenderer;
    private Point _previousResolution;
    public static Game1 Instance { get; private set; }

    private SpriteBatch _spriteBatch;
    // Zoznam vrstiev
    private System.Collections.Generic.List<Layer> _layers;
    Effect multiplyEffect;
    // private BasicEffect _effect;
    TcpCommandListener commandListener;

    private Camera2D _camera;
    private MouseState _previousMouseState;
    private SpriteFont _font;
    private int _fps;
    private int _frameCounter;
    private double _elapsedTime;
    public Game1()
    {
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
    }

    protected override void Initialize()
    {
        _avaloniaRenderer = new AvaloniaGameRenderer(GraphicsDevice);
        _avaloniaRenderer.Resolution = _previousResolution;

        // TODO: Add your initialization logic here
        _layers = LoadLayersFromYaml(_projectPath + _projectFilename);
        _camera = new Camera2D(GraphicsDevice.Viewport);

        //var udpListener = new UdpPacketListener(5000); // Port 12345
        //Task.Run(() => udpListener.StartListening());

        base.Initialize();

        //base.LoadContent();

        commandListener = new TcpCommandListener(5000);
        Task.Run(() => commandListener.Start());
    }

    private void UpdateAvalonia()
    {
        if (_previousResolution != GraphicsDevice.Viewport.Bounds.Size)
        {
            _previousResolution = GraphicsDevice.Viewport.Bounds.Size;
            _avaloniaRenderer.Resolution = _previousResolution;
        }
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        multiplyEffect = Content.Load<Effect>("shaders/MultiplyEffect");
        _font = Content.Load<SpriteFont>("Fonts/DefaultFont");

        // Načítanie textúr z disku
        foreach (var layer in _layers)
        {
            if (File.Exists(_projectPath + layer.ImagePath))
            {
                using (var stream = File.OpenRead(_projectPath + layer.ImagePath))
                {
                    layer.Texture = Texture2D.FromStream(GraphicsDevice, stream);
                }
            }
            else
            {
                System.Console.WriteLine($"File not found: {layer.ImagePath}");
            }
        }
    }

    protected override void UnloadContent()
    {
        // Zastav listener pri ukončení hry
        commandListener.Stop();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        UpdateAvalonia();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        var mouseState = Mouse.GetState();

        // Skontroluj, či je stlačené pravé tlačidlo myši
        if (mouseState.RightButton == ButtonState.Pressed)
        {
            var deltaX = mouseState.X - _previousMouseState.X;
            var deltaY = mouseState.Y - _previousMouseState.Y;

            // Posuň kameru podľa pohybu myši
            _camera.Move(new Vector2(-deltaX / _camera.Zoom, -deltaY / _camera.Zoom));
        }

        // Zoomovanie pomocou kolieska myši
        int scrollDelta = mouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;
        if (scrollDelta != 0)
        {
            float newZoom = _camera.Zoom + (scrollDelta > 0 ? 0.1f : -0.1f);
            _camera.Zoom = newZoom;
        }

        _previousMouseState = mouseState;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _avaloniaRenderer.Begin();
        //GraphicsDevice.Clear(Color.Aqua);
        _graphics.GraphicsDevice.Clear(Color.SlateGray);



        foreach (var layer in _layers)
        {
            var blendState = BlendModeFactory.GetBlendState(layer.BlendMode);
            // var colorWithOpacity = new Color(layer.Colorize.R, layer.Colorize.G, layer.Colorize.B, (byte)(255 * layer.Opacity));

            _spriteBatch.Begin(SpriteSortMode.Immediate, blendState: blendState, effect: multiplyEffect, transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(layer.Texture, layer.Position, Color.White);
            _spriteBatch.End();
        }

        // Počítanie FPS
        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        _frameCounter++;

        if (_elapsedTime >= 1.0)
        {
            _fps = _frameCounter;
            _frameCounter = 0;
            _elapsedTime = 0;
        }

        // Vykreslenie textu FPS
        _spriteBatch.Begin();
        _spriteBatch.DrawString(_font, $"FPS: {_fps}", new Vector2(20, 10), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
        _spriteBatch.End();

        base.Draw(gameTime);
        _avaloniaRenderer.End();
    }

    private System.Collections.Generic.List<Layer> LoadLayersFromYaml(string filePath)
    {
        // Načítanie YAML súboru
        var yamlText = File.ReadAllText(filePath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var layers = deserializer.Deserialize<LayerContainer>(yamlText);
        return layers.Layers;
    }

    internal void OnPointerWheelChanged(float y)
    {
        _camera.Zoom += y > 0 ? 0.1f : -0.1f;
    }

    // Trieda pre načítanie YAML údajov
    public class Layer
    {
        public string ImagePath { get; set; }
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public string BlendMode { get; set; }
        public float Opacity { get; set; } = 1.0f;
        public Color Colorize { get; set; } = Color.White;
    }

    public class LayerContainer
    {
        public System.Collections.Generic.List<Layer> Layers { get; set; }
    }
}
