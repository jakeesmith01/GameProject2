using System;
using System.Collections.Generic;
using Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Screens;
using StateManagement;

namespace Sprites
{
    public class OpponentCar
    {
        public Vector2 Position = new Vector2();
        public Queue<Vector2> Waypoints;

        
        private Vector2 _velocity = Vector2.Zero;
        private Vector2 _acceleration = Vector2.Zero;

        
        private float _maxSpeed = 500f;
        private float _accelerationRate = 180f;
        private float _drag = 0.98f;

        private Vector2 _currentTarget;
        private const float WaypointThreshold = 50f; 

        private float Rotation;
        private float TurnSpeed = 1.5f; 
        

        public bool SpeedBoostActive = false;
        public BoundingRectangle Hitbox;

        private float _baseSpeed = 75f;
        private float _speedVariation = 75f;
        private SoundManager _soundManager = new SoundManager();
        private Texture2D _texture;
        private Texture2D _debug;

        public float Speed { get; private set; }

        public OpponentCar(Vector2 position, Queue<Vector2> waypoints)
        {
            Position = position;
            Waypoints = waypoints;
            Speed = _baseSpeed;
            
            Hitbox = new BoundingRectangle(Position.X - 4, Position.Y - 6, 9, 10);

            if (Waypoints.Count > 0)
            {
                _currentTarget = Waypoints.Peek();
            }
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("opponentcar");
            _debug = content.Load<Texture2D>("debugTexture");
            _soundManager.LoadContent(content);
        }

        /// <summary>
        /// Used when the car drives over a speed boost.
        /// </summary>
        public void SpeedBoost()
        {
            _maxSpeed = 550f;
            _accelerationRate = 220f;
            SpeedBoostActive = true;
        }

        /// <summary>
        /// Used when the car's speed boost wears off.
        /// </summary>
        public void SpeedBoostOff()
        {
            _maxSpeed = 440f;
            _accelerationRate = 130f;
            SpeedBoostActive = false;
        }

        /// <summary>
        /// The update method, handles the pathing and physics of the NPC car
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        /// <param name="roadBlocks">A list of road blocks to determine collision with</param>
        public void Update(GameTime gameTime, List<RoadBlock> roadBlocks)
        {
            // If no waypoints, just idle
            if (Waypoints.Count == 0)
            {
                _acceleration = Vector2.Zero;
                ApplyPhysics(gameTime);
                return;
            }

            Vector2 toTarget = _currentTarget - Position;
            float distance = toTarget.Length();

            // Collision Check
            foreach (var roadBlock in roadBlocks)
            {
                if (roadBlock.CheckCollision(Hitbox))
                {
                    HandleCollision(roadBlock.Hitbox);
                    toTarget = _currentTarget - Position;
                    distance = toTarget.Length();
                    break;
                }
            }

            // If close enough to current waypoint, move to the next
            if (distance < WaypointThreshold)
            {
                Waypoints.Dequeue();
                if (Waypoints.Count > 0)
                {
                    _currentTarget = Waypoints.Peek();
                    toTarget = _currentTarget - Position;
                    distance = toTarget.Length();
                }
                else
                {
                    // No more waypoints
                    _acceleration = Vector2.Zero;
                    ApplyPhysics(gameTime);
                    return;
                }
            }

            if (toTarget.Length() > 0)
            {
                toTarget.Normalize();

                
                float desiredRotation = (float)Math.Atan2(toTarget.Y, toTarget.X) + MathHelper.PiOver2;

                
                float rotationDiff = desiredRotation - Rotation;

                
                rotationDiff = (float)((rotationDiff + Math.PI) % (2 * Math.PI) - Math.PI);

                
                if (rotationDiff > 0)
                    Rotation += TurnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else if (rotationDiff < 0)
                    Rotation -= TurnSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                
                Vector2 forward = new Vector2((float)Math.Cos(Rotation - MathHelper.PiOver2),
                                              (float)Math.Sin(Rotation - MathHelper.PiOver2));

                
                _acceleration = forward * _accelerationRate;
            }
            else
            {
                _acceleration = Vector2.Zero;
            }

            ApplyPhysics(gameTime);
            _soundManager.PlayCarDrivingSound();
        }

        /// <summary>
        /// Applys physics to the NPC car
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        private void ApplyPhysics(GameTime gameTime)
        {
            
            _velocity += _acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            
            _velocity *= _drag;

            
            if (_velocity.Length() > _maxSpeed)
            {
                _velocity.Normalize();
                _velocity *= _maxSpeed;
            }

            
            Position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            
            Hitbox.X = Position.X - 4f;
            Hitbox.Y = Position.Y - 6;
        }

        /// <summary>
        /// Handles collision between the car and a road block.
        /// </summary>
        /// <param name="blockRect">The hitbox of the road block.</param>
        public void HandleCollision(BoundingRectangle blockRect)
        {
            _soundManager.PlayHitSound();
            BoundingRectangle carRect = Hitbox;

            float overlapX = 0f, overlapY = 0f;

            float overlapLeft = carRect.Right - blockRect.Left;
            float overlapRight = blockRect.Right - carRect.Left;
            float overlapTop = carRect.Bottom - blockRect.Top;
            float overlapBottom = blockRect.Bottom - carRect.Top;

            
            if (carRect.Right > blockRect.Left && carRect.Left < blockRect.Right)
            {
                overlapX = (overlapLeft < overlapRight) ? -overlapLeft : overlapRight;
            }

            
            if (carRect.Bottom > blockRect.Top && carRect.Top < blockRect.Bottom)
            {
                overlapY = (overlapTop < overlapBottom) ? -overlapTop : overlapBottom;
            }

            
            Vector2 correction = Vector2.Zero;
            if (Math.Abs(overlapX) < Math.Abs(overlapY))
            {
                correction.X = overlapX + 8;
            }
            else
            {
                correction.Y = overlapY + 4;
            }

            
            Position += correction;
            Hitbox.X = Position.X - 4f;
            Hitbox.Y = Position.Y - 6;

            _velocity = -_velocity * 0.9f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(8, 8);
            //spriteBatch.Draw(_debug, new Rectangle((int)Hitbox.X, (int)Hitbox.Y, (int)Hitbox.Width, (int)Hitbox.Height), Color.Red * 0.5f);
            spriteBatch.Draw(_texture,
                             new Rectangle((int)Position.X, (int)Position.Y, 16, 16),
                             new Rectangle(0, 0, 16, 16),
                             Color.White,
                             Rotation,
                             origin,
                             SpriteEffects.None,
                             0f);
        }
    }
}
