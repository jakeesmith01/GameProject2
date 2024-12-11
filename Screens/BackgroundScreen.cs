using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StateManagement;

namespace Screens{
    public class BackgroundScreen : GameScreen
    {
        /// <summary>
        /// The content manager used to load content for this screen.
        /// </summary>
        private ContentManager _content;

        /// <summary>
        /// The background texture for this screen.
        /// </summary>
        private Texture2D _background;

        /// <summary>
        /// BackgroundScreen Constructor
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Loads graphics content for this screen. 
        /// </summary>
        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("pixelracer");
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void Unload()
        {
            _content.Unload();
        }

        /// <summary>
        /// Draws the background screen and ensures it does not transition off
        /// </summary>
        /// <param name="gameTime">The current gameTime</param>
        /// <param name="otherScreenHasFocus">A boolean determining if this screen has focus or not</param>
        /// <param name="coveredByOtherScreen">This will be overriden and always passed as false</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        /// <summary>
        /// Draws the background screen
        /// </summary>
        /// <param name="gameTime">The current gametime</param>
        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(_background, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();
        }
    }
}