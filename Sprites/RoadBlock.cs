using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Collision;
using MonoGame.Extended.Screens;

namespace Sprites
{
    public class RoadBlock
    {
        public static Texture2D _texture;

        public Vector2 Position { get; private set; }

        public BoundingRectangle Hitbox { get; private set; }

        public RoadBlock(Vector2 position)
        {
            Position = position;
            Hitbox = new BoundingRectangle((int)position.X + 4, (int)position.Y + 4, 6, 6);
        }

        public static void LoadContent(ContentManager content)
        {
            if(_texture == null)
            {
                _texture = content.Load<Texture2D>("misc_props");
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(_texture != null)
            {
                spriteBatch.Draw(_texture, Position, new Rectangle(80, 0, 16, 16) ,Color.White);
            }
        }

        public bool CheckCollision(BoundingRectangle other)
        {
            return Hitbox.CollidesWith(other);
        }
    }
}
