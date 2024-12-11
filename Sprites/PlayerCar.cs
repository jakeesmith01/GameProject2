using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using StateManagement;
using Screens;
using Microsoft.Xna.Framework.Audio;
using Camera;
using Collision;

namespace Sprites{
    public class PlayerCar{

        // The texture for the player car
        private Texture2D _texture;

        private Texture2D _debug;

        // The bounding rectangle (hitbox) of the car

        // The position of the car
        public Vector2 Position = new Vector2();

        public Vector2 Velocity = Vector2.Zero;

        private Vector2 _acceleration = Vector2.Zero;

        public float Speed = 75f;
        private float _maxSpeed = 400f;
        private float _accelerationRate = 120f;
        private float turnSpeed = 2.5f;
        private float _drag = 0.98f;
        private Vector2 _collisionVelocity = Vector2.Zero;
        private float _rotation = 0f;

        private Vector2 _movement;

        private SoundManager _soundManager = new SoundManager();

        public BoundingRectangle Hitbox;

        public bool SpeedBoostActive = false;

        
        public PlayerCar(Vector2 position)
        {
            Position = position;
            Hitbox = new BoundingRectangle(Position.X - 4.3f, Position.Y - 6, 9, 13);
        }

        public void LoadContent(ContentManager content){
            _texture = content.Load<Texture2D>("playercartexture");
            _soundManager.LoadContent(content);
            _debug = content.Load<Texture2D>("debugTexture");
        }

        public void HandleCollision(BoundingRectangle blockRect)
        {
            _soundManager.PlayHitSound();

            // Use the current car rectangle
            BoundingRectangle carRect = Hitbox;

            // Calculate minimal translation vector to get out of collision
            float overlapX = 0f, overlapY = 0f;

            float overlapLeft = carRect.Right - blockRect.Left;
            float overlapRight = blockRect.Right - carRect.Left;
            float overlapTop = carRect.Bottom - blockRect.Top;
            float overlapBottom = blockRect.Bottom - carRect.Top;

            // Check horizontal overlap
            if (carRect.Right > blockRect.Left && carRect.Left < blockRect.Right)
            {
                overlapX = (overlapLeft < overlapRight) ? -overlapLeft : overlapRight;
            }

            // Check vertical overlap
            if (carRect.Bottom > blockRect.Top && carRect.Top < blockRect.Bottom)
            {
                overlapY = (overlapTop < overlapBottom) ? -overlapTop : overlapBottom;
            }

            Vector2 correction = Vector2.Zero;
            if (Math.Abs(overlapX) < Math.Abs(overlapY))
            {
                correction.X = overlapX;
            }
            else
            {
                correction.Y = overlapY;
            }

            // Move the car so it no longer collides
            Position += correction;

            
            Velocity = -Velocity * 0.95f;

            
            Hitbox.X = Position.X - 4.3f;
            Hitbox.Y = Position.Y - 6;
        }

        /// <summary>
        /// Used when the car drives over a speed boost.
        /// </summary>
        public void SpeedBoost()
        {
            _maxSpeed = 430f;
            _accelerationRate = 180f;
            SpeedBoostActive = true;
        }

        /// <summary>
        /// Used when the car's speed boost wears off.
        /// </summary>
        public void SpeedBoostOff()
        {
            _maxSpeed = 420f;
            _accelerationRate = 120f;
            SpeedBoostActive = false;
        }

        public void HandleInput(GameTime gameTime, InputState input, int playerIndex)
        {
            if(input == null){
                throw new ArgumentNullException(nameof(input));
            }

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            bool handbrake = keyboardState.IsKeyDown(Keys.Space);

            turnSpeed = handbrake ? 2.5f : 1.2f;

            if (keyboardState.IsKeyDown(Keys.A))
            {
                _rotation -= turnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if(keyboardState.IsKeyDown(Keys.D))
            {
                _rotation += turnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            Vector2 forward = new Vector2((float)Math.Cos(_rotation - MathHelper.PiOver2), (float)Math.Sin(_rotation - MathHelper.PiOver2));
            Vector2 right = new Vector2(-forward.Y, forward.X);

            if (keyboardState.IsKeyDown(Keys.W))
            {
                Velocity += forward * _accelerationRate * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _soundManager.PlayCarDrivingSound();
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                Velocity -= forward * _accelerationRate * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _soundManager.PlayCarDrivingSound();
            }

            if (!keyboardState.IsKeyDown(Keys.W) && !keyboardState.IsKeyDown(Keys.S) && !keyboardState.IsKeyDown(Keys.A) && !keyboardState.IsKeyDown(Keys.D)){
                _soundManager.PlayCarIdleSound();
            }

            if(Velocity.Length() > _maxSpeed)
            {
                Velocity.Normalize();
                Velocity *= _maxSpeed;
            }

            float forwardSpeed = Vector2.Dot(Velocity, forward);
            float sideSpeed = Vector2.Dot(Velocity, right);

            float forwardFriction = 0.98f;
            float sideFriction = 0.9f;

            if (handbrake)
            {
                forwardFriction = 0.99f;
                sideFriction = 0.98f;
            }

            forwardSpeed *= forwardFriction;
            sideSpeed *= sideFriction;

            Velocity = forwardSpeed * forward + sideSpeed * right;


            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Hitbox.X = Position.X - 4.3f;
            Hitbox.Y = Position.Y - 6;
        }

        

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(_debug, new Rectangle((int)Hitbox.X, (int)Hitbox.Y, (int)Hitbox.Width, (int)Hitbox.Height), Color.Red * 0.5f);
            spriteBatch.Draw(_texture, Position, new Rectangle(0, 0, 16, 16), Color.White, _rotation, new Vector2(8, 8), 1.0f, SpriteEffects.None, 0);
        }

    }
}