using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Avalonia_Monogame_Dock_Template.Monogame
{
    public class AvaloniaGameRenderer
    {
        private Microsoft.Xna.Framework.Point _virtualResolution;
        private Microsoft.Xna.Framework.Point _resolution;
        // private Microsoft.Xna.Framework.Vector2 _scale;
        private GraphicsDevice _device;
        private RenderTarget2D? _renderTarget;
        private RenderTargetBinding[] _previousRenderTargets;
        private SpriteBatch _spriteBatch;
        private bool _isDirty;

        public Microsoft.Xna.Framework.Point VirtualResolution
        {
            get => _virtualResolution;
            set
            {
                if (_virtualResolution == value) { return; }
                _virtualResolution = value;
                _isDirty = true;
            }
        }

        public Microsoft.Xna.Framework.Point Resolution
        {
            get => _resolution;
            set
            {
                if (_resolution == value) { return; }
                _resolution = value;
                _isDirty = true;
            }
        }

        public Microsoft.Xna.Framework.Color ClearColor { get; set; } = Microsoft.Xna.Framework.Color.CornflowerBlue;

        public AvaloniaGameRenderer(GraphicsDevice device)
        {
            _device = device;
            //_virtualResolution = new Microsoft.Xna.Framework.Point(_device.Viewport.Width, _device.Viewport.Height);
            _virtualResolution = new Microsoft.Xna.Framework.Point(device.Adapter.CurrentDisplayMode.Width, device.Adapter.CurrentDisplayMode.Height);
            _resolution.X = _device.Viewport.Width;
            _resolution.Y = _device.Viewport.Height;
            _spriteBatch = new SpriteBatch(_device);
            _isDirty = false;
            _previousRenderTargets = Array.Empty<RenderTargetBinding>();

            CreateRenderTarget(_device.Viewport.Width, _device.Viewport.Height);
        }

        [MemberNotNull(nameof(_renderTarget))]
        private void CreateRenderTarget(int width, int height)
        {
            try
            {
                _renderTarget?.Dispose();
            }
            catch { }

            _renderTarget = new RenderTarget2D(_device, width, height);
        }

        public void Begin()
        {
            if (_isDirty)
            {
                Update();
            }

            _previousRenderTargets = _device.GetRenderTargets();
            _device.SetRenderTarget(_renderTarget);
            _device.Clear(ClearColor);
        }

        public void End()
        {
            Microsoft.Xna.Framework.Vector2 position = new Microsoft.Xna.Framework.Vector2(Resolution.X / 2f, Resolution.Y / 2f);
            //Microsoft.Xna.Framework.Vector2 origin = new Microsoft.Xna.Framework.Vector2(Resolution.X / 2f, Resolution.Y / 2f);

            _device.SetRenderTargets(_previousRenderTargets);
            _device.Clear(ClearColor);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_renderTarget, position, null, Microsoft.Xna.Framework.Color.White, 0, position, 1f, SpriteEffects.None, 0);
            _spriteBatch.End();
        }

        private void Update()
        {
            _isDirty = false;

            //_scale = VirtualResolution.ToVector2() / Resolution.ToVector2();
            //_scale = new Microsoft.Xna.Framework.Vector2(Math.Min(_scale.X, _scale.Y), Math.Min(_scale.X, _scale.Y));
            CreateRenderTarget(Resolution.X, Resolution.Y);
        }
    }
}
