using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Avalonia_Monogame_Dock_Template.Monogame
{
    public class MonoGameControl : Control
    {
        ////////////////////////////////////////////////////////////////////////////////
        /// Avalonia Direct Properties
        ////////////////////////////////////////////////////////////////////////////////
        #region Avalonia Direct Properties

        /// <summary>
        ///     Property for Avalonia that is used to change the default background color used when the control is rendered.
        /// </summary>
        public static readonly DirectProperty<MonoGameControl, IBrush> DefaultBackgroundProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, IBrush>(nameof(MonoGameControl.DefaultBackground), g => g.DefaultBackground, (s, v) => s.DefaultBackground = v);

        /// <summary>
        ///     Property for Avalonia that is used to define the <see cref="Microsoft.Xna.Framework.Game"/> instance being played
        ///     by the control
        /// </summary>
        public static readonly DirectProperty<MonoGameControl, Game1?> GameProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, Game1?>(nameof(MonoGameControl.Game), g => g.Game, (s, v) => s.Game = v);

        /// <summary>
        ///     Property for Avalonia that is used to define the <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.BackBufferWidth"/>
        ///     value used.
        /// </summary>
        public static readonly DirectProperty<MonoGameControl, int> BackBufferWidthProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, int>(nameof(MonoGameControl.BackBufferWidth), g => g.BackBufferWidth, (s, v) => s.BackBufferWidth = v);

        /// <summary>
        ///     Property for Avalonia that is used to define the <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.BackBufferHeight"/>
        ///     value used.
        /// </summary>
        public static readonly DirectProperty<MonoGameControl, int> BackBufferHeightProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, int>(nameof(MonoGameControl.BackBufferHeight), g => g.BackBufferHeight, (s, v) => s.BackBufferHeight = v);

        /// <summary>
        ///     Property for Avalonia that is used to define the <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.BackBufferFormat"/>
        ///     value used.
        /// </summary>
        public static readonly DirectProperty<MonoGameControl, SurfaceFormat> BackBufferFormatProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, SurfaceFormat>(nameof(MonoGameControl.BackBufferFormat), g => g.BackBufferFormat, (s, v) => s.BackBufferFormat = v);

        /// <summary>
        ///     Property for Avalonia that is used to define the <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.DepthStencilFormat"/>
        ///     value used.
        /// </summary>
        public static readonly DirectProperty<MonoGameControl, DepthFormat> DepthStencilFormatProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, DepthFormat>(nameof(MonoGameControl.DepthStencilFormat), g => g.DepthStencilFormat, (s, v) => s.DepthStencilFormat = v);

        /// <summary>
        ///     Property for Avalonia that is used to define the <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.PresentationInterval"/>
        ///     value used.
        /// </summary>
        public static readonly DirectProperty<MonoGameControl, PresentInterval> PresentationIntervalProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, PresentInterval>(nameof(MonoGameControl.PresentationInterval), g => g.PresentationInterval, (s, v) => s.PresentationInterval = v);

        /// <summary>
        ///     Property for Avalonia that is used to define the <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.IsFullScreen"/>
        ///     value used.
        /// </summary>
        public static readonly DirectProperty<MonoGameControl, bool> IsFullScreenProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, bool>(nameof(MonoGameControl.IsFullScreen), g => g.IsFullScreen, (s, v) => s.IsFullScreen = v);

        /// <summary>
        ///     Property for Avalonia that is used to define the <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters.IsFullScreen"/>
        ///     value used.
        /// </summary>
        public static readonly DirectProperty<MonoGameControl, bool> PauseProperty =
            AvaloniaProperty.RegisterDirect<MonoGameControl, bool>(nameof(MonoGameControl.IsPaused), g => g.IsPaused, (s, v) => s.IsPaused = v);

        #endregion

        ////////////////////////////////////////////////////////////////////////////////
        /// Fields
        ////////////////////////////////////////////////////////////////////////////////
        #region Fields

        private readonly Stopwatch _timer;
        private readonly GameTime _gameTime;

        private Game1? _game;
        private byte[] _buffer;
        private WriteableBitmap? _writableBitmap;
        private bool _hasBeenInitialized;
        private PresentationParameters _presentationParameters;
        private bool _isPaused;
        private HashSet<Microsoft.Xna.Framework.Input.Keys> _keysDownHash;

        #endregion Fields

        ////////////////////////////////////////////////////////////////////////////////
        /// Properties
        ////////////////////////////////////////////////////////////////////////////////
        #region Properties

        /// <summary>
        ///     Gets or Sets the default background color to use when rendering the control in an Avalonia view
        /// </summary>
        public IBrush DefaultBackground { get; set; } = Brushes.Silver;

        /// <summary>
        ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Game"/> instance being played by the control.
        /// </summary>
        public Game1? Game
        {
            get => _game;
            set
            {
                if (_game == value)
                {
                    return;
                }

                _game = value;

                if (_hasBeenInitialized)
                {
                    Initialize();
                }
            }
        }

        /// <summary>
        ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.PresentationParameters"/> used by the 
        ///     graphics device of the game.
        /// </summary>
        public PresentationParameters PresentationParameters
        {
            get => _presentationParameters;
            set
            {
                if (_presentationParameters == value) { return; }
                _presentationParameters = value;
                HandlePresentationParameterChange(_game);
            }
        }

        /// <summary>
        ///     Gets or Sets the width, in pixels, of the back buffer used by the graphics device of the game.
        /// </summary>
        public int BackBufferWidth
        {
            get => _presentationParameters.BackBufferWidth;
            set
            {
                if (_presentationParameters.BackBufferWidth == value) { return; }
                _presentationParameters.BackBufferWidth = value;
                HandlePresentationParameterChange(_game);
            }
        }

        /// <summary>
        ///     Gets or Sets the height, in pixels, of hte back buffer used by the graphics device of the game.
        /// </summary>
        public int BackBufferHeight
        {
            get => _presentationParameters.BackBufferHeight;
            set
            {
                if (_presentationParameters.BackBufferHeight == value) { return; }
                _presentationParameters.BackBufferHeight = value;
                HandlePresentationParameterChange(_game);
            }
        }

        /// <summary>
        ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.SurfaceFormat"/> of the back buffer used
        ///     by the graphics device of the game.
        /// </summary>
        public SurfaceFormat BackBufferFormat
        {
            get => _presentationParameters.BackBufferFormat;
            set
            {
                if (_presentationParameters.BackBufferFormat == value) { return; }
                _presentationParameters.BackBufferFormat = value;
                HandlePresentationParameterChange(_game);
            }
        }

        /// <summary>
        ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.DepthFormat"/> used by the graphics device
        ///     of the game
        /// </summary>
        public DepthFormat DepthStencilFormat
        {
            get => _presentationParameters.DepthStencilFormat;
            set
            {
                if (_presentationParameters.DepthStencilFormat == value) { return; }
                _presentationParameters.DepthStencilFormat = value;
                HandlePresentationParameterChange(_game);
            }
        }

        /// <summary>
        ///     Gets or Sets the <see cref="Microsoft.Xna.Framework.Graphics.PresentInterval"/> used by the graphics
        ///     device of the game.
        /// </summary>
        public PresentInterval PresentationInterval
        {
            get => _presentationParameters.PresentationInterval;
            set
            {
                if (_presentationParameters.PresentationInterval == value) { return; }
                _presentationParameters.PresentationInterval = value;
                HandlePresentationParameterChange(_game);
            }
        }

        /// <summary>
        ///     Gets or Sets a value that indicates if the graphics of the game should be rendered in full screen.
        /// </summary>
        public bool IsFullScreen
        {
            get => _presentationParameters.IsFullScreen;
            set
            {
                if (_presentationParameters.IsFullScreen == value) { return; }
                _presentationParameters.IsFullScreen = value;
                HandlePresentationParameterChange(_game);
            }
        }

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (_isPaused == value) { return; }
                _isPaused = value;
            }
        }

        #endregion Properties

        /// <summary>
        ///     Initialzies a new instance of the <see cref="MonoGameControl"/> class.
        /// </summary>
        public MonoGameControl()
        {
            Focusable = true;
            _timer = new Stopwatch();
            _gameTime = new GameTime();
            _buffer = Array.Empty<byte>();
            _keysDownHash = new HashSet<Keys>();

            _presentationParameters = new PresentationParameters()
            {
                BackBufferWidth = 1,
                BackBufferHeight = 1,
                BackBufferFormat = SurfaceFormat.Color,
                DepthStencilFormat = DepthFormat.Depth24,
                PresentationInterval = PresentInterval.Immediate,
                IsFullScreen = false
            };
        }

        /// <summary>
        ///     Renders the visual to a <see cref="DrawingContext"/>.
        /// </summary>
        /// <param name="context">
        ///     The drawing context.
        /// </param>
        public override void Render(DrawingContext context)
        {
            //  Ensure we have everythign needed to render a frame of the game to the control
            //  otherwise render a blank control with the default background color.
            if (IsPaused ||
                Game is not Game1 game ||
               Game.GraphicsDevice is not GraphicsDevice device ||
               _writableBitmap is null ||
               Bounds.Width < 1 ||
               Bounds.Height < 1 ||
               !HandleDeviceReset(device))
            {
                context.DrawRectangle(DefaultBackground, null, new Rect(0, 0, Bounds.Size.Width, Bounds.Size.Height));
                return;
            }

            //  Run a single frame of the game, where the graphics are written to the back buffer, then extract the
            //  back bufer data and draw it to the control.
            RunSingleFrame(game);
            ExtractFrame(device, _writableBitmap);
            //Debug.Print("kwamba" + Bounds.Size);
            //Debug.Print("kwamba2" + _writableBitmap.Size);
            context.DrawImage(_writableBitmap, new Rect(_writableBitmap.Size), new Rect(0, 0, Bounds.Width, Bounds.Height));
        }

        /// <summary>
        ///     Positions child elements as part of a layout pass.
        /// </summary>
        /// <param name="finalSize">
        ///     The size available to the control.
        /// </param>
        /// <returns>
        ///     The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            finalSize = base.ArrangeOverride(finalSize);

            //  If the size is not the same as the writable bitmap, we need to reset the device
            if (finalSize != _writableBitmap?.Size && Game?.GraphicsDevice is GraphicsDevice device)
            {
                ResetDevice(device, finalSize);
            }

            return finalSize;
        }

        /// <summary>
        ///     Called when the control is added to a rooted visual tree.
        /// </summary>
        /// <param name="e">
        ///     The event args.
        /// </param>
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Start();
        }

        private bool HandleDeviceReset(GraphicsDevice device)
        {
            if (device.GraphicsDeviceStatus == GraphicsDeviceStatus.NotReset)
            {
                ResetDevice(device, Bounds.Size);
            }

            return device.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal;
        }

        private void HandlePresentationParameterChange(Game1? game)
        {
            if (game is null) { return; }
            ResetDevice(game.GraphicsDevice, new Size(_presentationParameters.BackBufferWidth, _presentationParameters.BackBufferHeight));
        }

        private void Start()
        {
            if (_hasBeenInitialized) { return; }

            Initialize();
            _timer.Start();
            _hasBeenInitialized = true;
        }

        private void Initialize()
        {
            //  Get the window handle that the Avalonia project is running in and set that as the window handle of
            //  the game
            if (this.GetVisualRoot() is Window window && window.TryGetPlatformHandle()?.Handle is IntPtr handle)
            {
                _presentationParameters.DeviceWindowHandle = handle;
                Microsoft.Xna.Framework.Input.Mouse.WindowHandle = handle;
            }

            if (Game is not Game1 game) { return; }

            Type keyboardType = typeof(Microsoft.Xna.Framework.Input.Keyboard);
            MethodInfo? setActiveMethodInfo = keyboardType.GetMethod("SetActive", BindingFlags.NonPublic | BindingFlags.Static);
            setActiveMethodInfo?.Invoke(null, new object[] { true });


            if (game.GraphicsDevice is GraphicsDevice device)
            {
                ResetDevice(device, Bounds.Size);
            }

            RunSingleFrame(game);
        }

        private void ResetDevice(GraphicsDevice device, Size size)
        {
            //  Ensure a minimum width and height of 1
            int width = Math.Max(1, (int)Math.Ceiling(size.Width));
            int height = Math.Max(1, (int)Math.Ceiling(size.Height));

            device.Viewport = new Viewport(0, 0, width, height);
            _presentationParameters.BackBufferWidth = width;
            _presentationParameters.BackBufferHeight = height;
            device.Reset(_presentationParameters);

            //  Recreate the writable bitmap
            InitializeWritableBitmap(device);
        }

        [MemberNotNull(nameof(_writableBitmap))]
        private void InitializeWritableBitmap(GraphicsDevice device)
        {
            _writableBitmap?.Dispose();
            _writableBitmap = new WriteableBitmap(
                size: new PixelSize(device.Viewport.Width, device.Viewport.Height),
                dpi: new Vector(96d, 96d),
                format: PixelFormat.Rgba8888,
                alphaFormat: AlphaFormat.Opaque);
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            _game.OnPointerWheelChanged((float)e.Delta.Y);
            base.OnPointerWheelChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _keysDownHash.Add(ConvertKeytoKey(e.Key));
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _keysDownHash.Remove(ConvertKeytoKey(e.Key));
            base.OnKeyUp(e);
        }

        private Microsoft.Xna.Framework.Input.Keys ConvertKeytoKey(Key key) => key switch
        {
            Key.None => Microsoft.Xna.Framework.Input.Keys.None,
            Key.Back => Microsoft.Xna.Framework.Input.Keys.Back,
            Key.Tab => Microsoft.Xna.Framework.Input.Keys.Tab,
            Key.Enter => Microsoft.Xna.Framework.Input.Keys.Enter,
            Key.CapsLock => Microsoft.Xna.Framework.Input.Keys.CapsLock,
            Key.Escape => Microsoft.Xna.Framework.Input.Keys.Escape,
            Key.Space => Microsoft.Xna.Framework.Input.Keys.Space,
            Key.PageUp => Microsoft.Xna.Framework.Input.Keys.PageUp,
            Key.PageDown => Microsoft.Xna.Framework.Input.Keys.PageDown,
            Key.End => Microsoft.Xna.Framework.Input.Keys.End,
            Key.Home => Microsoft.Xna.Framework.Input.Keys.Home,
            Key.Left => Microsoft.Xna.Framework.Input.Keys.Left,
            Key.Up => Microsoft.Xna.Framework.Input.Keys.Up,
            Key.Right => Microsoft.Xna.Framework.Input.Keys.Right,
            Key.Down => Microsoft.Xna.Framework.Input.Keys.Down,
            Key.Select => Microsoft.Xna.Framework.Input.Keys.Select,
            Key.Print => Microsoft.Xna.Framework.Input.Keys.Print,
            Key.Execute => Microsoft.Xna.Framework.Input.Keys.Execute,
            Key.PrintScreen => Microsoft.Xna.Framework.Input.Keys.PrintScreen,
            Key.Insert => Microsoft.Xna.Framework.Input.Keys.Insert,
            Key.Delete => Microsoft.Xna.Framework.Input.Keys.Delete,
            Key.Help => Microsoft.Xna.Framework.Input.Keys.Help,
            Key.D0 => Microsoft.Xna.Framework.Input.Keys.D0,
            Key.D1 => Microsoft.Xna.Framework.Input.Keys.D1,
            Key.D2 => Microsoft.Xna.Framework.Input.Keys.D2,
            Key.D3 => Microsoft.Xna.Framework.Input.Keys.D3,
            Key.D4 => Microsoft.Xna.Framework.Input.Keys.D4,
            Key.D5 => Microsoft.Xna.Framework.Input.Keys.D5,
            Key.D6 => Microsoft.Xna.Framework.Input.Keys.D6,
            Key.D7 => Microsoft.Xna.Framework.Input.Keys.D7,
            Key.D8 => Microsoft.Xna.Framework.Input.Keys.D8,
            Key.D9 => Microsoft.Xna.Framework.Input.Keys.D9,
            Key.A => Microsoft.Xna.Framework.Input.Keys.A,
            Key.B => Microsoft.Xna.Framework.Input.Keys.B,
            Key.C => Microsoft.Xna.Framework.Input.Keys.C,
            Key.D => Microsoft.Xna.Framework.Input.Keys.D,
            Key.E => Microsoft.Xna.Framework.Input.Keys.E,
            Key.F => Microsoft.Xna.Framework.Input.Keys.F,
            Key.G => Microsoft.Xna.Framework.Input.Keys.G,
            Key.H => Microsoft.Xna.Framework.Input.Keys.H,
            Key.I => Microsoft.Xna.Framework.Input.Keys.I,
            Key.J => Microsoft.Xna.Framework.Input.Keys.J,
            Key.K => Microsoft.Xna.Framework.Input.Keys.K,
            Key.L => Microsoft.Xna.Framework.Input.Keys.L,
            Key.M => Microsoft.Xna.Framework.Input.Keys.M,
            Key.N => Microsoft.Xna.Framework.Input.Keys.N,
            Key.O => Microsoft.Xna.Framework.Input.Keys.O,
            Key.P => Microsoft.Xna.Framework.Input.Keys.P,
            Key.Q => Microsoft.Xna.Framework.Input.Keys.Q,
            Key.R => Microsoft.Xna.Framework.Input.Keys.R,
            Key.S => Microsoft.Xna.Framework.Input.Keys.S,
            Key.T => Microsoft.Xna.Framework.Input.Keys.T,
            Key.U => Microsoft.Xna.Framework.Input.Keys.U,
            Key.V => Microsoft.Xna.Framework.Input.Keys.V,
            Key.W => Microsoft.Xna.Framework.Input.Keys.W,
            Key.X => Microsoft.Xna.Framework.Input.Keys.X,
            Key.Y => Microsoft.Xna.Framework.Input.Keys.Y,
            Key.Z => Microsoft.Xna.Framework.Input.Keys.Z,
            Key.LWin => Microsoft.Xna.Framework.Input.Keys.LeftWindows,
            Key.RWin => Microsoft.Xna.Framework.Input.Keys.RightWindows,
            Key.Apps => Microsoft.Xna.Framework.Input.Keys.Apps,
            Key.Sleep => Microsoft.Xna.Framework.Input.Keys.Sleep,
            Key.NumPad0 => Microsoft.Xna.Framework.Input.Keys.NumPad0,
            Key.NumPad1 => Microsoft.Xna.Framework.Input.Keys.NumPad1,
            Key.NumPad2 => Microsoft.Xna.Framework.Input.Keys.NumPad2,
            Key.NumPad3 => Microsoft.Xna.Framework.Input.Keys.NumPad3,
            Key.NumPad4 => Microsoft.Xna.Framework.Input.Keys.NumPad4,
            Key.NumPad5 => Microsoft.Xna.Framework.Input.Keys.NumPad5,
            Key.NumPad6 => Microsoft.Xna.Framework.Input.Keys.NumPad6,
            Key.NumPad7 => Microsoft.Xna.Framework.Input.Keys.NumPad7,
            Key.NumPad8 => Microsoft.Xna.Framework.Input.Keys.NumPad8,
            Key.NumPad9 => Microsoft.Xna.Framework.Input.Keys.NumPad9,
            Key.Multiply => Microsoft.Xna.Framework.Input.Keys.Multiply,
            Key.Add => Microsoft.Xna.Framework.Input.Keys.Add,
            Key.Separator => Microsoft.Xna.Framework.Input.Keys.Separator,
            Key.Subtract => Microsoft.Xna.Framework.Input.Keys.Subtract,
            Key.Decimal => Microsoft.Xna.Framework.Input.Keys.Decimal,
            Key.Divide => Microsoft.Xna.Framework.Input.Keys.Divide,
            Key.F1 => Microsoft.Xna.Framework.Input.Keys.F1,
            Key.F2 => Microsoft.Xna.Framework.Input.Keys.F2,
            Key.F3 => Microsoft.Xna.Framework.Input.Keys.F3,
            Key.F4 => Microsoft.Xna.Framework.Input.Keys.F4,
            Key.F5 => Microsoft.Xna.Framework.Input.Keys.F5,
            Key.F6 => Microsoft.Xna.Framework.Input.Keys.F6,
            Key.F7 => Microsoft.Xna.Framework.Input.Keys.F7,
            Key.F8 => Microsoft.Xna.Framework.Input.Keys.F8,
            Key.F9 => Microsoft.Xna.Framework.Input.Keys.F9,
            Key.F10 => Microsoft.Xna.Framework.Input.Keys.F10,
            Key.F11 => Microsoft.Xna.Framework.Input.Keys.F11,
            Key.F12 => Microsoft.Xna.Framework.Input.Keys.F12,
            Key.F13 => Microsoft.Xna.Framework.Input.Keys.F13,
            Key.F14 => Microsoft.Xna.Framework.Input.Keys.F14,
            Key.F15 => Microsoft.Xna.Framework.Input.Keys.F15,
            Key.F16 => Microsoft.Xna.Framework.Input.Keys.F16,
            Key.F17 => Microsoft.Xna.Framework.Input.Keys.F17,
            Key.F18 => Microsoft.Xna.Framework.Input.Keys.F18,
            Key.F19 => Microsoft.Xna.Framework.Input.Keys.F19,
            Key.F20 => Microsoft.Xna.Framework.Input.Keys.F20,
            Key.F21 => Microsoft.Xna.Framework.Input.Keys.F21,
            Key.F22 => Microsoft.Xna.Framework.Input.Keys.F22,
            Key.F23 => Microsoft.Xna.Framework.Input.Keys.F23,
            Key.F24 => Microsoft.Xna.Framework.Input.Keys.F24,
            Key.NumLock => Microsoft.Xna.Framework.Input.Keys.NumLock,
            Key.Scroll => Microsoft.Xna.Framework.Input.Keys.Scroll,
            Key.LeftShift => Microsoft.Xna.Framework.Input.Keys.LeftShift,
            Key.RightShift => Microsoft.Xna.Framework.Input.Keys.RightShift,
            Key.LeftCtrl => Microsoft.Xna.Framework.Input.Keys.LeftControl,
            Key.RightCtrl => Microsoft.Xna.Framework.Input.Keys.RightControl,
            Key.LeftAlt => Microsoft.Xna.Framework.Input.Keys.LeftAlt,
            Key.RightAlt => Microsoft.Xna.Framework.Input.Keys.RightAlt,
            Key.BrowserBack => Microsoft.Xna.Framework.Input.Keys.BrowserBack,
            Key.BrowserForward => Microsoft.Xna.Framework.Input.Keys.BrowserForward,
            Key.BrowserRefresh => Microsoft.Xna.Framework.Input.Keys.BrowserRefresh,
            Key.BrowserStop => Microsoft.Xna.Framework.Input.Keys.BrowserStop,
            Key.BrowserSearch => Microsoft.Xna.Framework.Input.Keys.BrowserSearch,
            Key.BrowserFavorites => Microsoft.Xna.Framework.Input.Keys.BrowserFavorites,
            Key.BrowserHome => Microsoft.Xna.Framework.Input.Keys.BrowserHome,
            Key.VolumeMute => Microsoft.Xna.Framework.Input.Keys.VolumeMute,
            Key.VolumeDown => Microsoft.Xna.Framework.Input.Keys.VolumeDown,
            Key.VolumeUp => Microsoft.Xna.Framework.Input.Keys.VolumeUp,
            Key.MediaNextTrack => Microsoft.Xna.Framework.Input.Keys.MediaNextTrack,
            Key.MediaPreviousTrack => Microsoft.Xna.Framework.Input.Keys.MediaPreviousTrack,
            Key.MediaStop => Microsoft.Xna.Framework.Input.Keys.MediaStop,
            Key.MediaPlayPause => Microsoft.Xna.Framework.Input.Keys.MediaPlayPause,
            Key.LaunchMail => Microsoft.Xna.Framework.Input.Keys.LaunchMail,
            Key.SelectMedia => Microsoft.Xna.Framework.Input.Keys.SelectMedia,
            Key.LaunchApplication1 => Microsoft.Xna.Framework.Input.Keys.LaunchApplication1,
            Key.LaunchApplication2 => Microsoft.Xna.Framework.Input.Keys.LaunchApplication2,
            Key.OemSemicolon => Microsoft.Xna.Framework.Input.Keys.OemSemicolon,
            Key.OemPlus => Microsoft.Xna.Framework.Input.Keys.OemPlus,
            Key.OemComma => Microsoft.Xna.Framework.Input.Keys.OemComma,
            Key.OemMinus => Microsoft.Xna.Framework.Input.Keys.OemMinus,
            Key.OemPeriod => Microsoft.Xna.Framework.Input.Keys.OemPeriod,
            Key.OemQuestion => Microsoft.Xna.Framework.Input.Keys.OemQuestion,
            Key.OemTilde => Microsoft.Xna.Framework.Input.Keys.OemTilde,
            Key.OemOpenBrackets => Microsoft.Xna.Framework.Input.Keys.OemOpenBrackets,
            Key.OemPipe => Microsoft.Xna.Framework.Input.Keys.OemPipe,
            Key.OemCloseBrackets => Microsoft.Xna.Framework.Input.Keys.OemCloseBrackets,
            Key.OemQuotes => Microsoft.Xna.Framework.Input.Keys.OemQuotes,
            Key.Oem8 => Microsoft.Xna.Framework.Input.Keys.Oem8,
            Key.OemBackslash => Microsoft.Xna.Framework.Input.Keys.OemBackslash,
            //Key.ProcessKey = Microsoft.Xna.Framework.Input.Keys.ProcessKey,
            Key.Attn => Microsoft.Xna.Framework.Input.Keys.Attn,
            Key.CrSel => Microsoft.Xna.Framework.Input.Keys.Crsel,
            Key.ExSel => Microsoft.Xna.Framework.Input.Keys.Exsel,
            Key.EraseEof => Microsoft.Xna.Framework.Input.Keys.EraseEof,
            Key.Play => Microsoft.Xna.Framework.Input.Keys.Play,
            Key.Zoom => Microsoft.Xna.Framework.Input.Keys.Zoom,
            Key.Pa1 => Microsoft.Xna.Framework.Input.Keys.Pa1,
            // Key.ChatPadGreen => Microsoft.Xna.Framework.Input.Keys.ChatPadGreen ,
            // Key.ChatPadOrange => Microsoft.Xna.Framework.Input.Keys.ChatPadOrange ,
            Key.OemClear => Microsoft.Xna.Framework.Input.Keys.OemClear,
            Key.Pause => Microsoft.Xna.Framework.Input.Keys.Pause,
            Key.ImeConvert => Microsoft.Xna.Framework.Input.Keys.ImeConvert,
            Key.ImeNonConvert => Microsoft.Xna.Framework.Input.Keys.ImeNoConvert,
            Key.KanaMode => Microsoft.Xna.Framework.Input.Keys.Kana,
            Key.KanjiMode => Microsoft.Xna.Framework.Input.Keys.Kanji,
            Key.OemAuto => Microsoft.Xna.Framework.Input.Keys.OemAuto,
            Key.OemCopy => Microsoft.Xna.Framework.Input.Keys.OemCopy,
            Key.OemEnlw => Microsoft.Xna.Framework.Input.Keys.OemEnlW,
            _ => Microsoft.Xna.Framework.Input.Keys.None
        };

        private void RunSingleFrame(Game1 game)
        {
            _gameTime.ElapsedGameTime = _timer.Elapsed;
            _gameTime.TotalGameTime += _gameTime.ElapsedGameTime;
            _timer.Restart();

            try
            {
                Type keyboardType = typeof(Microsoft.Xna.Framework.Input.Keyboard);
                MethodInfo? setKeysMethod = keyboardType.GetMethod("SetKeys", BindingFlags.NonPublic | BindingFlags.Static);
                setKeysMethod?.Invoke(null, new object[] { _keysDownHash.ToList() });

                game.RunOneFrame();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Normal);
            }
        }

        private void ExtractFrame(GraphicsDevice device, WriteableBitmap? writeAbleBitmap)
        {
            if (_writableBitmap is null) { return; }

            using (ILockedFramebuffer lockedFrameBuffer = writeAbleBitmap.Lock())
            {
                //  Determine the length of the buffer
                int size = lockedFrameBuffer.RowBytes * lockedFrameBuffer.Size.Height;

                //  Resize internal buffer if needed
                if (_buffer.Length < size)
                {
                    Array.Resize(ref _buffer, size);
                }

                //  Pull the data from the graphics device back buffer
                device.GetBackBufferData(_buffer, 0, size);

                //  Copy the backbuffer data into the writable bitmap
                Marshal.Copy(_buffer, 0, lockedFrameBuffer.Address, size);

            }
        }
    }
}
