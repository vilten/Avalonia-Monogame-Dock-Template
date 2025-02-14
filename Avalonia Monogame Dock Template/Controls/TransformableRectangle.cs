using Avalonia_Monogame_Dock_Template.Models;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Avalonia.Input;
using Avalonia_Monogame_Dock_Template.Events.Project;
using Avalonia_Monogame_Dock_Template.Events;
using System;
using IconPacks.Avalonia.PhosphorIcons;

namespace Avalonia_Monogame_Dock_Template.Controls
{
    public class TransformableRectangle
    {
        public Vector2 Position;
        public Vector2 Size;
        public float Rotation;
        private Texture2D pixel;

        private const int HandleSize = 10; // Veľkosť uchopovacích bodov
        private Rectangle[] handles = new Rectangle[9]; // 4 rohy + stred
        public Rectangle originalRectangle;
        private readonly List<Verticle> _selectedVerticles;
        private readonly List<Vector2> _originalSelectedVerticles = new List<Vector2>();
        PackIconPhosphorIconsKind _previousCursorType = PackIconPhosphorIconsKind.None;

        public TransformableRectangle(GraphicsDevice graphics, Vector2 position, Vector2 size, List<Verticle> selectedVerticles)
        {
            Position = position;
            Size = size;
            Rotation = 0f;

            // Vytvorenie 1-pixelovej textúry pre kreslenie čiar
            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });

            originalRectangle = new Rectangle(position.ToPoint(), size.ToPoint());
            foreach (var verticle in selectedVerticles)
            {
                _originalSelectedVerticles.Add(new Vector2(verticle.position.X, verticle.position.Y));
            }
            _selectedVerticles = selectedVerticles;

            UpdateHandles();
        }

        private void UpdateHandles()
        {
            handles[0] = new Rectangle((int)Position.X - HandleSize / 2, (int)Position.Y - HandleSize / 2, HandleSize, HandleSize); // Horný ľavý roh
            handles[1] = new Rectangle((int)(Position.X + Size.X) - HandleSize / 2, (int)Position.Y - HandleSize / 2, HandleSize, HandleSize); // Horný pravý roh
            handles[2] = new Rectangle((int)Position.X - HandleSize / 2, (int)(Position.Y + Size.Y) - HandleSize / 2, HandleSize, HandleSize); // Dolný ľavý roh
            handles[3] = new Rectangle((int)(Position.X + Size.X) - HandleSize / 2, (int)(Position.Y + Size.Y) - HandleSize / 2, HandleSize, HandleSize); // Dolný pravý roh
            handles[4] = new Rectangle((int)(Position.X + Size.X / 2) - HandleSize / 2, (int)(Position.Y + Size.Y / 2) - HandleSize / 2, HandleSize, HandleSize); // Stred
            handles[5] = new Rectangle((int)(Position.X + Size.X / 2) - HandleSize / 2, (int)Position.Y - HandleSize / 2, HandleSize, HandleSize); // Top middle
            handles[6] = new Rectangle((int)Position.X - HandleSize / 2, (int)(Position.Y + Size.Y / 2) - HandleSize / 2, HandleSize, HandleSize); // Left middle
            handles[7] = new Rectangle((int)(Position.X + Size.X) - HandleSize / 2, (int)(Position.Y + Size.Y / 2) - HandleSize / 2, HandleSize, HandleSize); // Right middle
            handles[8] = new Rectangle((int)(Position.X + Size.X / 2) - HandleSize / 2, (int)(Position.Y + Size.Y) - HandleSize / 2, HandleSize, HandleSize); // Bottom middle
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Nakreslenie obdĺžnika
            //DrawDashedRectangle(spriteBatch, Position, Size, HandleSize, Color.White);
            //DrawDashedRectangle(spriteBatch, new Rectangle(Position.ToPoint(), Size.ToPoint()), Color.White);
            DrawDashedRectangle(spriteBatch, new Rectangle(Position.ToPoint(), Size.ToPoint()), Color.White, Rotation);
            //spriteBatch.Draw(pixel, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, 1), Color.Red); // Horná hrana
            //spriteBatch.Draw(pixel, new Rectangle((int)Position.X, (int)(Position.Y + Size.Y), (int)Size.X, 1), Color.Red); // Dolná hrana
            //spriteBatch.Draw(pixel, new Rectangle((int)Position.X, (int)Position.Y, 1, (int)Size.Y), Color.Red); // Ľavá hrana
            //spriteBatch.Draw(pixel, new Rectangle((int)(Position.X + Size.X), (int)Position.Y, 1, (int)Size.Y), Color.Red); // Pravá hrana

            // Nakreslenie uchopovacích bodov
            foreach (var handle in handles)
            {
                spriteBatch.DrawRectangle(handle, Color.White);
                //spriteBatch.Draw(pixel, handle, Color.Blue);
            }
        }

        private void DrawDashedRectangle(SpriteBatch spriteBatch, Vector2 position, Vector2 size, int dashLength, Color color)
        {
            for (int x = 0; x < size.X; x += dashLength * 2)
            {
                spriteBatch.Draw(pixel, new Rectangle((int)(position.X + x), (int)position.Y, dashLength, 1), color);
                spriteBatch.Draw(pixel, new Rectangle((int)(position.X + x), (int)(position.Y + size.Y), dashLength, 1), color);
            }
            for (int y = 0; y < size.Y; y += dashLength * 2)
            {
                spriteBatch.Draw(pixel, new Rectangle((int)position.X, (int)(position.Y + y), 1, dashLength), color);
                spriteBatch.Draw(pixel, new Rectangle((int)(position.X + size.X), (int)(position.Y + y), 1, dashLength), color);
            }
        }

        private void DrawDashedRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            int dashLength = 5;
            int gapLength = 3;

            // Kreslenie vrchného a spodného okraja
            for (int x = rect.Left; x < rect.Right; x += dashLength + gapLength)
            {

                spriteBatch.Draw(pixel, new Rectangle(x, rect.Top, dashLength, 1), color);
                spriteBatch.Draw(pixel, new Rectangle(x, rect.Bottom, dashLength, 1), color);
            }

            // Kreslenie ľavého a pravého okraja
            for (int y = rect.Top; y < rect.Bottom; y += dashLength + gapLength)
            {
                spriteBatch.Draw(pixel, new Rectangle(rect.Left, y, 1, dashLength), color);
                spriteBatch.Draw(pixel, new Rectangle(rect.Right, y, 1, dashLength), color);
            }
        }

        private void DrawDashedRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color, float rotation)
        {
            int dashLength = 5;
            int gapLength = 3;
            Vector2 center = new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);

            Matrix rotationMatrix = Matrix.CreateRotationZ(rotation);

            void DrawRotatedLine(Vector2 start, Vector2 end)
            {
                Vector2 transformedStart = Vector2.Transform(start - center, rotationMatrix) + center;
                Vector2 transformedEnd = Vector2.Transform(end - center, rotationMatrix) + center;
                Vector2 direction = transformedEnd - transformedStart;
                float length = direction.Length();
                direction.Normalize();

                for (float i = 0; i < length; i += dashLength + gapLength)
                {
                    Vector2 segmentStart = transformedStart + direction * i;
                    Vector2 segmentEnd = transformedStart + direction * Math.Min(i + dashLength, length);
                    spriteBatch.Draw(pixel, new Rectangle((int)segmentStart.X, (int)segmentStart.Y, (int)(segmentEnd - segmentStart).Length(), 1), color);
                }
            }

            Vector2 topLeft = new Vector2(rect.Left, rect.Top);
            Vector2 topRight = new Vector2(rect.Right, rect.Top);
            Vector2 bottomLeft = new Vector2(rect.Left, rect.Bottom);
            Vector2 bottomRight = new Vector2(rect.Right, rect.Bottom);

            DrawRotatedLine(topLeft, topRight);     // Horná hrana
            DrawRotatedLine(topRight, bottomRight); // Pravá hrana
            DrawRotatedLine(bottomRight, bottomLeft); // Spodná hrana
            DrawRotatedLine(bottomLeft, topLeft);   // Ľavá hrana
        }



        public void Update(MouseState mouseState, MouseState prevMouseState, Vector2 mousePosition)
        {
            Vector2 mousePos = mousePosition;
            KeyboardState keyboardState = Keyboard.GetState();
            bool shift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
            bool alt = keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);
            bool ctrl = keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);

            PackIconPhosphorIconsKind curorType = PackIconPhosphorIconsKind.CursorFill;
            // Detekcia, či hráč klikol na niektorý z uchopovacích bodov
            for (int i = 0; i < handles.Length; i++)
            {
                if ((handles[i].Contains(mousePos) || Vector2.Distance(mousePos, handles[i].Center.ToVector2()) < 30))
                {
                    // detekcia kurzora
                    if (i == 0)
                        if (ctrl)
                            curorType = PackIconPhosphorIconsKind.ArrowsClockwiseFill;
                        else
                            curorType = PackIconPhosphorIconsKind.ArrowUpLeftFill;
                    else if (i == 1)
                        if (ctrl)
                            curorType = PackIconPhosphorIconsKind.ArrowsClockwiseFill;
                        else
                            curorType = PackIconPhosphorIconsKind.ArrowUpRightFill;
                    else if (i == 2)
                        if (ctrl)
                            curorType = PackIconPhosphorIconsKind.ArrowsClockwiseFill;
                        else
                            curorType = PackIconPhosphorIconsKind.ArrowDownLeftFill;
                    else if (i == 3)
                        if (ctrl)
                            curorType = PackIconPhosphorIconsKind.ArrowsClockwiseFill;
                        else
                            curorType = PackIconPhosphorIconsKind.ArrowDownRightFill;
                    else if (i == 4)
                        curorType = PackIconPhosphorIconsKind.ArrowsOutCardinalFill;
                    else if (i == 5)
                        curorType = PackIconPhosphorIconsKind.ArrowUpFill;
                    else if (i == 6)
                        curorType = PackIconPhosphorIconsKind.ArrowLeftFill;
                    else if (i == 7)
                        curorType = PackIconPhosphorIconsKind.ArrowRightFill;
                    else if (i == 8)
                        curorType = PackIconPhosphorIconsKind.ArrowDownFill;

                    if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
                    {
                        DraggingHandle = i;
                        break;
                    }
                }
            }
            if (_previousCursorType != curorType)
            {
                GlobalMessageBus.Instance.SendMessage(curorType);
                _previousCursorType = curorType;
            }

            // Presúvanie podľa uchopovacieho bodu
            if (DraggingHandle != -1 && mouseState.LeftButton == ButtonState.Pressed)
            {

                if (DraggingHandle == 4) // Stred - presun
                {
                    Position = mousePos - new Vector2(Size.X / 2, Size.Y / 2);
                }
                else if (ctrl) // Rotácia
                {
                    Vector2 center = Position + Size / 2;
                    Vector2 direction = mousePos - center;
                    float angle = (float)Math.Atan2(direction.Y, direction.X);

                    if (shift) // Zaokrúhlenie na 5-stupňové kroky
                    {
                        float degrees = MathHelper.ToDegrees(angle);
                        degrees = (float)Math.Round(degrees / 5) * 5;
                        angle = MathHelper.ToRadians(degrees);
                    }

                    Rotation = angle; // Premenná Rotation ukladá uhol v radiánoch
                }
                else // Rohy - škálovanie
                {
                    Vector2 newSize = Size;
                    Vector2 newPosition = Position;

                    if (DraggingHandle == 0) // Horný ľavý roh
                    {
                        Vector2 diff = Position - mousePos;
                        newPosition = mousePos;
                        newSize += diff;
                    }
                    else if (DraggingHandle == 5) // Horná stena
                    {
                        newSize = new Vector2(Size.X, Position.Y + Size.Y - mousePos.Y);
                        newPosition = new Vector2(Position.X, mousePos.Y);
                    }
                    else if (DraggingHandle == 6) // Ľavá stena
                    {
                        newSize = new Vector2(Position.X + Size.X - mousePos.X, Size.Y);
                        newPosition = new Vector2(mousePos.X, Position.Y);
                    }
                    else if (DraggingHandle == 7) // Pravá stena
                    {
                        newSize = new Vector2(mousePos.X - Position.X, Size.Y);
                    }
                    else if (DraggingHandle == 8) // Dolná stena
                    {
                        newSize = new Vector2(Size.X, mousePos.Y - Position.Y);
                    }
                    else if (DraggingHandle == 1) // Horný pravý roh
                    {
                        newSize = new Vector2(mousePos.X - Position.X, Size.Y + (Position.Y - mousePos.Y));
                        newPosition.Y = mousePos.Y;
                    }
                    else if (DraggingHandle == 2) // Dolný ľavý roh
                    {
                        newSize = new Vector2(Size.X + (Position.X - mousePos.X), mousePos.Y - Position.Y);
                        newPosition.X = mousePos.X;
                    }
                    else if (DraggingHandle == 3) // Dolný pravý roh
                    {
                        newSize = mousePos - Position;
                    }

                    if (shift) // Zachovanie pomeru strán
                    {
                        float aspectRatio = Size.X / Size.Y;
                        if (DraggingHandle == 0 || DraggingHandle == 1 || DraggingHandle == 2 || DraggingHandle == 3)
                        {
                            if (Math.Abs(newSize.X) > Math.Abs(newSize.Y))
                                newSize.Y = newSize.X / aspectRatio;
                            else
                                newSize.X = newSize.Y * aspectRatio;
                        }
                    }

                    if (!alt) // Zachovanie stredu
                    {
                        Vector2 center = Position + Size / 2;
                        newPosition = center - newSize / 2;
                    }

                    Size = newSize;
                    Position = newPosition;
                }

                UpdateHandles();

                if (_selectedVerticles.Count > 0)
                {
                    float scaleX = Size.X / originalRectangle.Size.X;
                    float scaleY = Size.Y / originalRectangle.Size.Y;

                    for (int index = 0; index < _selectedVerticles.Count; index++)
                    {
                        Vector2 distance = _originalSelectedVerticles[index] - originalRectangle.Center.ToVector2();
                        distance.X *= scaleX;
                        distance.Y *= scaleY;
                        _selectedVerticles[index].position = distance + handles[4].Center.ToVector2();
                    }
                }
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                DraggingHandle = -1;
            }

            /*
            // Presúvanie podľa uchopovacieho bodu
            if (DraggingHandle != -1 && mouseState.LeftButton == ButtonState.Pressed)
            {
                if (DraggingHandle == 4) // Stred - presun
                {
                    Position = mousePos - new Vector2(Size.X / 2, Size.Y / 2);
                }
                else // Rohy - škálovanie
                {
                    KeyboardState keyboardState = Keyboard.GetState();
                    bool shift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
                    bool alt = keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);

                    Vector2 newSize = Size;
                    Vector2 newPosition = Position;

                    if (DraggingHandle == 0) // Horný ľavý roh
                    {
                        Vector2 diff = Position - mousePos;
                        newPosition = mousePos;
                        newSize += diff;
                    }
                    else if (DraggingHandle == 5) // Horná stena
                    {
                        newSize = new Vector2(Size.X, Position.Y + Size.Y - mousePos.Y);
                        newPosition = new Vector2(Position.X, mousePos.Y);
                    }
                    else if (DraggingHandle == 6) // Ľavá stena
                    {
                        newSize = new Vector2(Position.X + Size.X - mousePos.X, Size.Y);
                        newPosition = new Vector2(mousePos.X, Position.Y);
                    }
                    else if (DraggingHandle == 7) // Pravá stena
                    {
                        newSize = new Vector2(mousePos.X - Position.X, Size.Y);
                    }
                    else if (DraggingHandle == 8) // Dolná stena
                    {
                        newSize = new Vector2(Size.X, mousePos.Y - Position.Y);
                    }
                    else if (DraggingHandle == 1) // Horný pravý roh
                    {
                        newSize = new Vector2(mousePos.X - Position.X, Size.Y + (Position.Y - mousePos.Y));
                        newPosition.Y = mousePos.Y;
                    }
                    else if (DraggingHandle == 2) // Dolný ľavý roh
                    {
                        newSize = new Vector2(Size.X + (Position.X - mousePos.X), mousePos.Y - Position.Y);
                        newPosition.X = mousePos.X;
                    }
                    else if (DraggingHandle == 3) // Dolný pravý roh
                    {
                        newSize = mousePos - Position;
                    }

                    if (shift) // Zachovanie pomeru strán
                    {
                        float aspectRatio = Size.X / Size.Y;
                        if (DraggingHandle == 0 || DraggingHandle == 1 || DraggingHandle == 2 || DraggingHandle == 3)
                        {
                            if (Math.Abs(newSize.X) > Math.Abs(newSize.Y))
                                newSize.Y = newSize.X / aspectRatio;
                            else
                                newSize.X = newSize.Y * aspectRatio;
                        }
                    }

                    if (!alt) // Zachovanie stredu
                    {
                        Vector2 center = Position + Size / 2;
                        newPosition = center - newSize / 2;
                    }

                    Size = newSize;
                    Position = newPosition;
                }

                UpdateHandles();

                if (_selectedVerticles.Count > 0)
                {
                    float scaleX = Size.X / originalRectangle.Size.X;
                    float scaleY = Size.Y / originalRectangle.Size.Y;

                    for (int index = 0; index < _selectedVerticles.Count; index++)
                    {
                        Vector2 distance = _originalSelectedVerticles[index] - originalRectangle.Center.ToVector2();
                        distance.X *= scaleX;
                        distance.Y *= scaleY;
                        _selectedVerticles[index].position = distance + handles[4].Center.ToVector2();
                    }
                }
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                DraggingHandle = -1;
            }
            */
            /*
            // Presúvanie podľa uchopovacieho bodu
            if (DraggingHandle != -1 && mouseState.LeftButton == ButtonState.Pressed)
            {
                if (DraggingHandle == 4) // Stred - presun
                {
                    Position = mousePos - new Vector2(Size.X / 2, Size.Y / 2);
                }
                else // Rohy - škálovanie
                {
                    KeyboardState keyboardState = Keyboard.GetState();
                    bool shift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
                    bool ctrl = keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);
                    if (DraggingHandle == 0) // Horný ľavý roh
                    {
                        Vector2 diff = Position - mousePos;
                        Position = mousePos;
                        Size += diff;
                    }
                    else if (DraggingHandle == 5)
                    {
                        Size = new Vector2(Size.X, Position.Y + Size.Y - mousePos.Y);
                        Position = new Vector2(Position.X, mousePos.Y);
                    }
                    else if (DraggingHandle == 6)
                    {
                        Size = new Vector2(Position.X + Size.X - mousePos.X, Size.Y);
                        Position = new Vector2(mousePos.X, Position.Y);
                    }
                    else if (DraggingHandle == 7)
                    {
                        Size = new Vector2(mousePos.X - Position.X, Size.Y);
                    }
                    else if (DraggingHandle == 8)
                    {
                        Size = new Vector2(Size.X, mousePos.Y - Position.Y);
                    }
                    else if (DraggingHandle == 1) // Horný pravý roh
                    {
                        Size = new Vector2(mousePos.X - Position.X, Size.Y + (Position.Y - mousePos.Y));
                        Position.Y = mousePos.Y;
                    }
                    else if (DraggingHandle == 2) // Dolný ľavý roh
                    {
                        Size = new Vector2(Size.X + (Position.X - mousePos.X), mousePos.Y - Position.Y);
                        Position.X = mousePos.X;
                    }
                    else if (DraggingHandle == 3) // Dolný pravý roh
                    {
                        Size = mousePos - Position;
                    }
                }

                UpdateHandles();
                if (_selectedVerticles.Count > 0)
                {
                    float scaleX = Size.X / originalRectangle.Size.X;
                    float scaleY = Size.Y / originalRectangle.Size.Y;

                    for (int index = 0; index < _selectedVerticles.Count; index++)
                    {
                        Vector2 distance = _originalSelectedVerticles[index] - originalRectangle.Center.ToVector2();
                        distance.X *= scaleX;
                        distance.Y *= scaleY;
                        _selectedVerticles[index].position = distance + handles[4].Center.ToVector2();
                    }
                }
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                DraggingHandle = -1;
            }
            */
        }

        private int DraggingHandle = -1;
    }
}
