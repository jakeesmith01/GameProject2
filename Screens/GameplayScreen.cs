using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StateManagement;
using Sprites;
using Camera;
using System.Collections.Generic;

namespace Screens
{
    public class GameplayScreen : GameScreen
    {
        /// <summary>
        /// The content manager used to load content for this screen.
        /// </summary>
        private ContentManager _content;

        private SoundManager _soundManager = new SoundManager();

        private readonly Random rand = new Random();

        private MapObjectManager _mapManager;

        /// <summary>
        /// The alpha to be added when paused
        /// </summary>
        private float _pauseAlpha;

        // The player car
        private PlayerCar _player;

        private const float SpeedBoostTime = 1.25f;

        private float playerSpeedBoostTimer = 0.0f;

        private float opponentSpeedBoostTimer = 0.0f;

        // The opponent car
        private OpponentCar _opponent;

        private List<RoadBlock> roadBlocks;

        private List<SpeedBoost> speedBoosts;

        private CarCamera _camera;

        /// <summary>
        /// The input action for pausing the game
        /// </summary>
        private readonly InputAction _pauseAction;

        /// <summary>
        /// The GameplayScreen Constructor
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Escape }, true);
        }

        /// <summary>
        /// Loads content for this screen 
        /// </summary>
        public override void Activate()
        {
            if(_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _camera = new CarCamera(ScreenManager.Game.GraphicsDevice);

            _mapManager = new MapObjectManager(ScreenManager);
            _mapManager.LoadMap("DesertLevel2.0");

            roadBlocks = _mapManager.RoadBlocks;
            speedBoosts = _mapManager.SpeedBoosts;
            _player = new PlayerCar(_mapManager.playerCarPosition);
            _opponent = new OpponentCar(_mapManager.opponentCarPosition, _mapManager.Waypoints);

            _camera.Follow(_player.Position);
            _player.LoadContent(_content);
            _opponent.LoadContent(_content);

            _soundManager.LoadContent(_content);
            // small sleep to simulate loading
            Thread.Sleep(1000);

            // reset elapsed game time 
            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Deactivates the screen
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
        }

        /// <summary>
        /// Unloads the content for the screen
        /// </summary>
        public override void Unload()
        {
            _content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if(coveredByOtherScreen){
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            }
            else{
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);
            }

            if(IsActive){

                if(_opponent.Position.Y <= 24)
                {
                    ScreenManager.AddScreen(new LoseScreen(), ControllingPlayer);
                }
                else if(_player.Position.Y <= 24)
                {
                    ScreenManager.AddScreen(new WinScreen(), ControllingPlayer);
                }

                // Game logic goes here
                _camera.Follow(_player.Position);
                _opponent.Update(gameTime, roadBlocks);

                if(_player.SpeedBoostActive)
                {
                    playerSpeedBoostTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (playerSpeedBoostTimer >= SpeedBoostTime)
                    {
                        _player.SpeedBoostOff();
                        playerSpeedBoostTimer = 0.0f;
                    }
                }

                if (_opponent.SpeedBoostActive)
                {
                    opponentSpeedBoostTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (opponentSpeedBoostTimer >= SpeedBoostTime)
                    {
                        _opponent.SpeedBoostOff();
                        opponentSpeedBoostTimer = 0.0f;
                    }
                }

                // Add collision checking for the road blocks here
                foreach (var roadBlock in roadBlocks)
                {
                    if (roadBlock.CheckCollision(_player.Hitbox))
                    {
                        Vector2 collisionDirection = _player.Position - roadBlock.Position;

                        if(collisionDirection != Vector2.Zero)
                        {
                            collisionDirection.Normalize();
                        }

                        _player.HandleCollision(collisionDirection, 10f);

                        break;
                    }
                }

                foreach(var speedBoost in speedBoosts)
                {
                    if(speedBoost.CheckCollision(_player.Hitbox))
                    {
                        _player.SpeedBoost();
                        playerSpeedBoostTimer = 0.0f;
                        break;
                    }
                }

                foreach(var speedBoost in speedBoosts)
                {
                    if(speedBoost.CheckCollision(_opponent.Hitbox))
                    {
                        _opponent.SpeedBoost();
                        opponentSpeedBoostTimer = 0.0f;
                        break;
                    }
                }

                
            }
        }

        /// <summary>
        /// Handles input from the players
        /// </summary>
        /// <param name="gameTime">The current gametime</param>
        /// <param name="input">The current inputstate</param>
        /// <exception cref="ArgumentNullException">Thrown if input is null</exception>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if(input == null){
                throw new ArgumentNullException(nameof(input));
            }

            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            
            PlayerIndex player;
            if(_pauseAction.Occurred(input, ControllingPlayer, out player)){
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else{
                // Handle input here
                _player.HandleInput(gameTime, input, playerIndex);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Gray);

            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

            _mapManager.Draw(_camera, spriteBatch);

            
            _opponent.Draw(spriteBatch);
            _player.Draw(spriteBatch);

            spriteBatch.End();
        }

    }
}