using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Collision
{
    public static class CollisionHelper
    {
        /// <summary>
        /// Determines if two BoundingRectangles collide
        /// </summary>
        /// <param name="a">The first bounding rectangle</param>
        /// <param name="b">The second bounding rectangle</param>
        /// <returns>true for collision, false otherwise</returns>
        public static bool Collides(BoundingRectangle a, BoundingRectangle b)
        {
            return !(a.Right < b.Left || a.Left > b.Right || a.Top > b.Bottom || a.Bottom < b.Top);
        }
    }
}