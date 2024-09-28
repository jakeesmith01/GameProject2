using System;
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
        public Vector2 Position = new Vector2(260, 916);

        /// <summary>
        /// The speed of the opponentcar
        /// </summary>
        public float Speed {get; private set;}

        private float _baseSpeed = 100f;

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

        public void Update(GameTime gameTime){
            Position.Y -= Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(gameTime.TotalGameTime.TotalMilliseconds % 750 < gameTime.ElapsedGameTime.TotalMilliseconds){
                Speed = GetRandomSpeed() + _baseSpeed;
            }

            _soundManager.PlayCarDrivingSound();

            if(Position.Y < 32){
                _soundManager.StopCarSounds();
            }
        }

        public void Draw(SpriteBatch spriteBatch){
            spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, 16, 16), new Rectangle(0, 0, 16, 16), Color.White);
        }

        
    }
}