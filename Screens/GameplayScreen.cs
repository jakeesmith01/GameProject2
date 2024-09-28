using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StateManagement;
using Sprites;

namespace Screens
{
    public class GameplayScreen : GameScreen
    {
        /// <summary>
        /// The content manager used to load content for this screen.
        /// </summary>
        private ContentManager _content;

        // The texture for the main road tile
        private Texture2D _roadTexture;

        private SoundManager _soundManager = new SoundManager();

        // The texture for the details
        private Texture2D _detailTexture;

        private Texture2D _finishTexture;

        // Holds the indices of the randomized tiles so that it doesnt constantly generate new ones
        private int[,] _tileIndices;

        private readonly Random rand = new Random();

        /// <summary>
        /// The alpha to be added when paused
        /// </summary>
        private float _pauseAlpha;

        // the width of the screen
        private int _width = 576;
        
        // the height of the screen
        private int _height = 952;

        // The tile size of the roads
        private int _tileSize = 64;

        // The tile size of the details 
        private int _detailTileSize = 16;

        // The player car
        private PlayerCar _player;

        // The opponent car
        private OpponentCar _opponent;

        private const float FinishLineDistance = 5000f;

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
            
            _player = new PlayerCar();
            _opponent = new OpponentCar();
        }

        /// <summary>
        /// Loads content for this screen 
        /// </summary>
        public override void Activate()
        {
            if(_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _roadTexture = _content.Load<Texture2D>("road");
            _detailTexture = _content.Load<Texture2D>("details");
            _finishTexture = _content.Load<Texture2D>("finishline");

            int tilesPerRow = _width / _detailTileSize;
            int tilesPerColumn = _height / _detailTileSize;

            _tileIndices = new int[tilesPerRow, tilesPerColumn];

            for(int i = 0; i < tilesPerRow; i++){
                for(int j = 0; j < tilesPerColumn; j++){
                    _tileIndices[i, j] = rand.Next(1, 4);
                }
            }

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
                // Game logic goes here
                _player.Update(gameTime);
                _opponent.Update(gameTime);

                if(_player.Position.Y < 32){
                    ScreenManager.AddScreen(new WinScreen(), ControllingPlayer);
                    _soundManager.PlayWinSound();
                }

                if(_opponent.Position.Y < 32){
                    ScreenManager.AddScreen(new LoseScreen(), ControllingPlayer);
                    _soundManager.PlayLoseSound();
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

            int roadStartX = (_width / 2) - _tileSize;

            spriteBatch.Begin();

            // drawing the roads 
            for(int y = 0; y < (_height / _tileSize) + 1; y++){ 
                spriteBatch.Draw(_roadTexture, new Rectangle(roadStartX, y * _tileSize, _tileSize, _tileSize), Color.White);
                spriteBatch.Draw(_roadTexture, new Rectangle(roadStartX + _tileSize, y * _tileSize, _tileSize, _tileSize), Color.White);

            }

            for(int y = 0; y < _height / _detailTileSize; y++){

                for(int x = 0; x < (((_width / 2) - _tileSize) / _detailTileSize); x++){
                    int detailIndex = _tileIndices[x, y];
                    Rectangle detailSrc = new Rectangle(detailIndex * _detailTileSize, 0, _detailTileSize, _detailTileSize);
                    spriteBatch.Draw(_detailTexture, new Rectangle(x * _detailTileSize, y * _detailTileSize, _detailTileSize, _detailTileSize), detailSrc, Color.White);
                }

                for(int x = ((_width / 2) + _tileSize) / _detailTileSize; x < (_width / _detailTileSize); x++){
                    int detailIndex = _tileIndices[x, y];
                    Rectangle detailSrc = new Rectangle(detailIndex * _detailTileSize, 0, _detailTileSize, _detailTileSize);
                    spriteBatch.Draw(_detailTexture, new Rectangle(x * _detailTileSize, y * _detailTileSize, _detailTileSize, _detailTileSize), detailSrc, Color.White);
                }
            }

            int finishLineY = 32;
            spriteBatch.Draw(_finishTexture, new Rectangle(roadStartX + 8, finishLineY, (_tileSize * 2) - 16, _tileSize / 3), Color.White);

            _opponent.Draw(spriteBatch);
            _player.Draw(spriteBatch);
            spriteBatch.End();

        }

    }
}