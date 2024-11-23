using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collision;
using Microsoft.Xna.Framework;

namespace Collision
{
    /// <summary>
    /// A bounding rectangle for collision detection
    /// </summary>
    public struct BoundingRectangle
    {
        /// <summary>
        /// The x position of the rectangle
        /// </summary>
        public float X;

        /// <summary>
        /// The y position of the rectangle
        /// </summary>
        public float Y;

        /// <summary>
        /// The width of the rectangle
        /// </summary>
        public float Width;

        /// <summary>
        /// The height of the rectangle
        /// </summary>
        public float Height;


        public float Left => X;

        public float Right => X + Width;

        public float Top => Y;

        public float Bottom => Y + Height;

        public BoundingRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public BoundingRectangle(Vector2 position, float width, float height)
        {
            X = position.X;
            Y = position.Y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Determines if it collides with the other bounding rectangle
        /// </summary>
        /// <param name="other">The other bounding rectangle</param>
        /// <returns>true for collision, false otherwise</returns>
        public bool CollidesWith(BoundingRectangle other)
        {
            return CollisionHelper.Collides(this, other);
        }
    }
}