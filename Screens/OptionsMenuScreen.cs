using StateManagement;

namespace Screens
{
    // The options screen is brought up over the top of the main menu
    // screen, and gives the user a chance to configure the game
    // in various hopefully useful ways.
    public class OptionsMenuScreen : MenuScreen
    {
        // The menuentry for audio options
        private readonly MenuEntry _audioMenuEntry;

        // The menuentry for sound effects audio options
        private readonly MenuEntry _sfxAudioMenuEntry;

        // The current audio volume
        private static float CurrentMusicVolume = GameSettings.MusicVolume;

        // The current sfx volume
        private static float CurrentSFXVolume = GameSettings.SFXVolume;

        public OptionsMenuScreen() : base("Options")
        {
            _audioMenuEntry = new MenuEntry(string.Empty);
            _sfxAudioMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            _audioMenuEntry.Selected += AudioMenuEntrySelected;
            _sfxAudioMenuEntry.Selected += SFXAudioMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(_audioMenuEntry);
            MenuEntries.Add(_sfxAudioMenuEntry);
            MenuEntries.Add(back);
        }

        // Fills in the latest values for the options screen menu text.
        private void SetMenuEntryText()
        {
            _audioMenuEntry.Text = $"Music Volume: {CurrentMusicVolume.ToString("P0")}";
            _sfxAudioMenuEntry.Text = $"SFX Volume: {CurrentSFXVolume.ToString("P0")}";
        }

        /// <summary>
        /// Event handler for when the SFXAudio menu entry is selected, increments audio by 0.1f, until 1.0f which it resets to 0.1f
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SFXAudioMenuEntrySelected(object sender, PlayerIndexEventArgs e){
            CurrentSFXVolume += 0.1f;
            GameSettings.SFXVolume = CurrentSFXVolume;

            if (CurrentSFXVolume > 1.01f)
                CurrentSFXVolume = 0.1f;
                GameSettings.SFXVolume = CurrentSFXVolume;
            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Audio menu entry is selected, increments audio by 0.1f, until 1.0f which it resets to 0.1f
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AudioMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            CurrentMusicVolume += 0.1f;
            GameSettings.MusicVolume = CurrentMusicVolume;

            if (CurrentMusicVolume > 1.01f)
                CurrentMusicVolume = 0.1f;
                GameSettings.MusicVolume = CurrentMusicVolume;
            SetMenuEntryText();
        }
    }
}