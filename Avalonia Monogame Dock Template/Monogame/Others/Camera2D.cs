using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Avalonia_Monogame_Dock_Template.Monogame
{
    public class Camera2D
    {
        private float _zoom;
        private Vector2 _position;
        private float _rotation;
        private Vector2 _origin;

        public Camera2D(Viewport viewport)
        {
            _zoom = 1f;
            _position = Vector2.Zero;
            _rotation = 0f;

            // Nastavenie stredu obrazovky ako východiskový bod
            _origin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
        }

        public float Zoom
        {
            get => _zoom;
            set
            {
                if (value < 0.1f) // Zabránenie negatívnym alebo príliš malým hodnotám
                    value = 0.1f;

                // Úprava pozície pri zmene zoomu
                var previousZoom = _zoom;
                _zoom = value;

                // Posun pozície kamery, aby zoom zostal voči stredu
                _position += (_origin - _position) * (1 - previousZoom / _zoom);
            }
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public void Move(Vector2 offset)
        {
            _position += offset;
        }

        public float Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(-_position.X, -_position.Y, 0) * // Posun kamery
                   Matrix.CreateTranslation(-_origin.X, -_origin.Y, 0) *    // Nastavenie stredu
                   Matrix.CreateScale(_zoom, _zoom, 1) *                    // Aplikácia zoomu
                   Matrix.CreateRotationZ(_rotation) *                     // Rotácia
                   Matrix.CreateTranslation(_origin.X, _origin.Y, 0);      // Presun späť
        }
    }

}
