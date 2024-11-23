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

        // The bounding rectangle (hitbox) of the car

        // The position of the car
        public Vector2 Position = new Vector2();

        public float Speed = 75f;

        private float _speedVariation = 75f;

        private Vector2 _movement;

        private SoundManager _soundManager = new SoundManager();

        public BoundingRectangle Hitbox;

        public bool SpeedBoostActive = false;

        
        public PlayerCar(Vector2 position)
        {
            Position = position;
            Hitbox = new BoundingRectangle(position, 16, 16);
        }

        public void LoadContent(ContentManager content){
            _texture = content.Load<Texture2D>("playercartexture");
            _soundManager.LoadContent(content);
        }

        public void HandleCollision(Vector2 collisionDirection, float knockbackDistance)
        {
            Position += collisionDirection * knockbackDistance;

            Hitbox.X = Position.X;
            Hitbox.Y = Position.Y;
        }

        /// <summary>
        /// Used when the car drives over a speed boost.
        /// </summary>
        public void SpeedBoost()
        {
            Speed = 150f;
            SpeedBoostActive = true;
        }

        /// <summary>
        /// Used when the car's speed boost wears off.
        /// </summary>
        public void SpeedBoostOff()
        {
            Speed = 75f;
            SpeedBoostActive = false;
        }

        public void HandleInput(GameTime gameTime, InputState input, int playerIndex)
        {
            if(input == null){
                throw new ArgumentNullException(nameof(input));
            }

            var keyboardState = input.CurrentKeyboardStates[playerIndex];

            var movement = Vector2.Zero;

            if(keyboardState.IsKeyDown(Keys.W)){
                movement.Y--; 
                _soundManager.PlayCarDrivingSound();
            }

            if(keyboardState.IsKeyDown(Keys.S)){
                movement.Y++;
                _soundManager.PlayCarDrivingSound();
            }

            if(keyboardState.IsKeyDown(Keys.A)){
                movement.X--;
                _soundManager.PlayCarDrivingSound();
            }

            if(keyboardState.IsKeyDown(Keys.D)){
                movement.X++;
                _soundManager.PlayCarDrivingSound();
            }

            if(!keyboardState.IsKeyDown(Keys.W) && !keyboardState.IsKeyDown(Keys.S) && !keyboardState.IsKeyDown(Keys.A) && !keyboardState.IsKeyDown(Keys.D)){
                _soundManager.PlayCarIdleSound();
            }

            if(movement.Length() > 1){
                movement.Normalize();
            }

            _movement = movement;
            Position += movement * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Hitbox.X = Position.X;
            Hitbox.Y = Position.Y;
        }

        /// <summary>
        /// Determines the direction the player car is facing and return the corresponding tile index for the texture
        /// </summary>
        /// <returns></returns>
        private int GetDirectionIndex()
        {
            if(_movement.Y < 0 && _movement.X == 0) return 0; //up
            if(_movement.Y < 0 && _movement.X > 0) return 1; //up right
            if(_movement.Y == 0 && _movement.X > 0) return 2; //right
            if(_movement.Y > 0 && _movement.X > 0) return 3; //down right
            if(_movement.Y > 0 && _movement.X == 0) return 4; //down
            if(_movement.Y > 0 && _movement.X < 0) return 5; //down left
            if(_movement.Y == 0 && _movement.X < 0) return 6; //left
            if(_movement.Y < 0 && _movement.X < 0) return 7; //up left
            return 0; // default to facing up
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int tileIndex = GetDirectionIndex();
            Rectangle src = new Rectangle(tileIndex * 16, 0, 16, 16);


            spriteBatch.Draw(_texture, Position, src, Color.White);
        }

    }
}