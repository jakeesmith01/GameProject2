using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;
using StateManagement;

namespace Screens{
    public class LoseScreen : MenuScreen
    {
	    public LoseScreen() : base("You Lost!") 
	    {
            var PlayAgainMenuEntry = new MenuEntry("Play Again");
            var ReturnToMainMenuEntry = new MenuEntry("Return to Main Menu");
            var QuitGameMenuEntry = new MenuEntry("Quit Game");

            PlayAgainMenuEntry.Selected += PlayAgainMenuEntrySelected;
            ReturnToMainMenuEntry.Selected += ReturnToMainMenuEntrySelected;
            QuitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            MenuEntries.Add(PlayAgainMenuEntry);
            MenuEntries.Add(ReturnToMainMenuEntry);
            MenuEntries.Add(QuitGameMenuEntry);

		    TransitionOnTime = TimeSpan.FromSeconds(1.5);
		    TransitionOffTime = TimeSpan.FromSeconds(0.5);
	    }

        public void PlayAgainMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(GameSettings.LevelID));
        }

        public void ReturnToMainMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen(), new MainMenuScreen());
        }

        public void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit?";
            var confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        private void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
	    
    }
}
