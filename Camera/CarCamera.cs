using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera
{
    public class CarCamera
    {
        private readonly GraphicsDevice _graphics;

        /// <summary>
        /// The position of the camera
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// The zoom factor of the camera
        /// </summary>
        public float Zoom { get; set; } = 3.5f;

        /// <summary>
        /// The rotation angle of the camera 
        /// </summary>
        public float Rotation { get; set; } = 0f;

        /// <summary>
        /// Bounds of the map
        /// </summary>
        private Rectangle _mapBounds = new Rectangle(0, 0, 288, 1920);

        private int _viewportWidth;
        private int _viewportHeight;

        /// <summary>
        /// A camera that follows the playerCar
        /// </summary>
        /// <param name="graphics"></param>
        public CarCamera(GraphicsDevice graphics)
        {
            _graphics = graphics;
            
            _viewportWidth = _graphics.Viewport.Width;
            _viewportHeight = _graphics.Viewport.Height;
        }

        /// <summary>
        /// Centers the camera on the target, clamping it to the map bounds
        /// </summary>
        /// <param name="target"></param>
        public void Follow(Vector2 target)
        {
            float halfWidth = _viewportWidth / (2f * Zoom);
            float halfHeight = _viewportHeight / (2f * Zoom);

            float clampedX = MathHelper.Clamp(target.X, _mapBounds.Left + halfWidth, _mapBounds.Right - halfWidth);
            float clampedY = MathHelper.Clamp(target.Y, _mapBounds.Top + halfHeight, _mapBounds.Bottom - halfHeight);

            Position = new Vector2(clampedX, clampedY);
        }

        /// <summary>
        /// Returns the transformation matrix for the camera
        /// </summary>
        /// <returns></returns>
        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(Zoom, Zoom, 1) *
                        Matrix.CreateTranslation(new Vector3(_viewportWidth / 2f, _viewportHeight / 2f, 0));
        }
    }
}
