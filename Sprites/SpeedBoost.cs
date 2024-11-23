using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Collision;

namespace Sprites
{
    public class SpeedBoost
    {
        public static Texture2D _texture;

        public BoundingRectangle Hitbox { get; private set; }

        public Vector2 Position { get; private set; }

        public SpeedBoost(Vector2 position)
        {
            Position = position;
            Hitbox = new BoundingRectangle((int)position.X, (int)position.Y, 16, 16);
        }

        public static void LoadContent(ContentManager content)
        {
            if(_texture == null)
            {
                _texture = content.Load<Texture2D>("misc_props");
            }
        }

        /// <summary>
        /// Checks if a car has driven over a speed boost or not
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CheckCollision(BoundingRectangle other)
        {
            return Hitbox.CollidesWith(other);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(_texture != null)
            {
                spriteBatch.Draw(_texture, Position, new Rectangle(0, 0, 16, 16), Color.White);
            }
            else
            {
                Console.WriteLine("SpeedBoost texture is null");
            }
        }
    }
}
