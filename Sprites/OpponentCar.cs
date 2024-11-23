using System;
using System.Collections;
using System.Collections.Generic;
using Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Screens;
using StateManagement;

namespace Sprites{
    public class OpponentCar{
        /// <summary>
        /// The position of the opponentcar
        /// </summary>
        public Vector2 Position = new Vector2();

        public Queue<Vector2> Waypoints;

        private Vector2 _movement = Vector2.Zero;

        private Vector2 _currentTarget;

        private const float WaypointThreshold = 2.0f;

        private float Rotation;

        private float TurnSpeed = 2f;

        public bool SpeedBoostActive = false;

        public BoundingRectangle Hitbox;

        public OpponentCar(Vector2 position, Queue<Vector2> waypoints)
        {
            Position = position;
            Waypoints = waypoints;
            Speed = _baseSpeed;
            Hitbox = new BoundingRectangle(position, 16, 16);

            if (Waypoints.Count > 0)
            {
                _currentTarget = Waypoints.Peek();
            }
        }

        /// <summary>
        /// The speed of the opponentcar
        /// </summary>
        public float Speed {get; private set;}

        private float _baseSpeed = 75f;

        private float _speedVariation = 75f;

        private SoundManager _soundManager = new SoundManager();

        private Texture2D _texture;

        private float GetRandomSpeed(){
            Random rand = new Random();
            return (float)(rand.NextDouble() * _speedVariation);
        }

        public void LoadContent(ContentManager content){
            _texture = content.Load<Texture2D>("opponentcar");
            _soundManager.LoadContent(content);
        }

        public void SpeedBoost()
        {
            Speed = 150f;
            SpeedBoostActive = true;
        }

        public void SpeedBoostOff()
        {
            Speed = _baseSpeed;
            SpeedBoostActive = false;
        }


        public void Update(GameTime gameTime, List<RoadBlock> roadBlocks){
            if(Waypoints.Count > 0)
            {
                Vector2 direction = _currentTarget - Position;
                float distance = direction.Length();

                foreach(var roadBlock in roadBlocks)
                {
                    if (roadBlock.CheckCollision(Hitbox))
                    {
                        direction = HandleCollision(direction, roadBlock.Position);
                        break;
                    }
                }

                if(distance < WaypointThreshold)
                {
                    Waypoints.Dequeue();

                    if(Waypoints.Count > 0)
                    {
                        _currentTarget = Waypoints.Peek();
                        direction = _currentTarget - Position;
                    }
                }
                
                if(distance > 0)
                {
                    Vector2 smoothDir = Vector2.Lerp(_movement, direction, 0.1f);
                    smoothDir.Normalize();

                    Position += smoothDir * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Hitbox.X = Position.X;
                    Hitbox.Y = Position.Y;

                    Rotation = (float)Math.Atan2(smoothDir.Y, smoothDir.X) + MathHelper.PiOver2;

                    _movement = smoothDir;
                }

            }

            _soundManager.PlayCarDrivingSound();

        }

        public Vector2 HandleCollision(Vector2 currentDirection, Vector2 barrierPosition)
        {
            Vector2 collisionDirection = Position - barrierPosition;
            if(collisionDirection.Length() > 0)
            {
                collisionDirection.Normalize();
            }

            Vector2 avoidance = new Vector2(-collisionDirection.Y, collisionDirection.X);

            Vector2 adjustedDirection = Vector2.Lerp(currentDirection, avoidance, 0.5f);
            adjustedDirection.Normalize();

            return adjustedDirection;
            
        }

        public void Draw(SpriteBatch spriteBatch){
            Vector2 origin = new Vector2(8, 8);
            spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, 16, 16), new Rectangle(0, 0, 16, 16), Color.White, Rotation, origin, SpriteEffects.None, 0f);
        }

        
    }
}