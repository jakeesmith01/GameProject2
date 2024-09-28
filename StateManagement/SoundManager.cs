using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;


namespace StateManagement
{
    /// <summary>
    /// Manages sound effects and the background music for the game
    /// </summary>
    public class SoundManager{

        // The sound effect instance for the car idling
        private SoundEffectInstance _carIdle;

        // The sound effect instance for the car driving
        private SoundEffectInstance _carDrive;

        // The sound effect for the car idling
        private SoundEffect _carIdleSound;

        // The sound effect for the car driving
        private SoundEffect _carDrivingSound;

        // The sound effect for winning the race
        private SoundEffect _winSound;

        // The sound effect for losing the race 
        private SoundEffect _loseSound;

        // The background music 
        private Song _backgroundMusic;

        /// <summary>
        /// Loads the content for the sound effects and background music
        /// </summary>
        /// <param name="content">The content manager to load with</param>
        public void LoadContent(ContentManager content){
            _carIdleSound = content.Load<SoundEffect>("carIdle");
            _carDrivingSound = content.Load<SoundEffect>("carDrive2");
            _winSound = content.Load<SoundEffect>("win");
            _loseSound = content.Load<SoundEffect>("lose");
            _backgroundMusic = content.Load<Song>("backgroundMusic");

            _carIdle = _carIdleSound.CreateInstance();
            _carDrive = _carDrivingSound.CreateInstance();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = GameSettings.MusicVolume;
            MediaPlayer.Play(_backgroundMusic);

            _carIdle.Volume = GameSettings.SFXVolume;
            _carDrive.Volume = GameSettings.SFXVolume;

            _carDrive.IsLooped = false;
            _carIdle.IsLooped = false;
        }

        /// <summary>
        /// Plays the car sound effect at the specified SFX volume
        /// </summary>
        public void PlayCarIdleSound(){
            if(_carDrive.State == SoundState.Playing){
                _carDrive.Stop();
            }

            _carIdle.Play();
        }

        /// <summary>
        /// Plays the car driving sound effect at the specified SFX volume
        /// </summary>
        public void PlayCarDrivingSound(){
            if(_carIdle.State == SoundState.Playing){
                _carIdle.Stop();
            }
            _carDrive.Play();
        }

        /// <summary>
        /// Plays the win sound effect at the specified SFX volume
        /// </summary>
        public void PlayWinSound(){
            MediaPlayer.Pause();
            StopCarSounds();
            _winSound.Play(GameSettings.SFXVolume, 0, 0);
        }

        /// <summary>
        /// Plays the lose sound effect at the specified SFX volume
        /// </summary>
        public void PlayLoseSound(){
            MediaPlayer.Pause();
            StopCarSounds();
            _loseSound.Play(GameSettings.SFXVolume, 0, 0);
        }

        public void StopCarSounds(){
            _carIdle.Stop();
            _carDrive.Stop();
        }

        
    }
}