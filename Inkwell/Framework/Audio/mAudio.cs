using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Inkwell.Framework
{
    /// <summary>
    /// (Singleton) In charge of Storing various AudioEffects and disposing of them along with playing active sounds.
    /// </summary>
    public sealed class mAudio
    {
        #region Singleton
        private static volatile mAudio _instance;
        private static object _padlock = new Object();

        public static mAudio Peek
        {
            get
            {
                /*Check to see if we already initialized our component*/
                if (_instance == null)
                {
                    /*Lock it so another thread cant check it*/
                    lock (_padlock)
                    {
                        /*Check one more time just to be extra careful*/
                        if (_instance == null)
                            _instance = new mAudio(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        #endregion

        private Dictionary<string, Song> songs = new Dictionary<string, Song>();
        private Dictionary<SoundName, Sound> sounds = new Dictionary<SoundName, Sound>();
        private Song currentSong = null;
        public SoundEffectInstance[] soundInstances = new SoundEffectInstance[MaxSounds];
        private const int MaxSounds = 16; //means you can only play 16 sounds at a time
        private AudioListener listener = new AudioListener();

        //accessor for passing 3D audio to Sound.cs
        public AudioListener Listener
        {
            get { return listener; }
        }

        //Get/Set Music Volume (1.0f is max)
        public float MusicVolume
        {
            get { return MediaPlayer.Volume; }
            set { MediaPlayer.Volume = MathHelper.Clamp(value, 0.0f, 1.0f); }
        }
        //Get/Set Sound Effects Volume (1.0f is max)
        public float SoundVolume
        {
            get { return SoundEffect.MasterVolume; }
            set { SoundEffect.MasterVolume = MathHelper.Clamp(value, 0.0f, 1.0f); }
        }

        public enum SoundName
        {
            LightAttack,
            LightAttackHit,
            HeavyAttack,
            HeavyAttackHit,
            Jump,
            SpecialOnOff,
            PlayerHit,
            PlayerDeath,
            WormChase,
            WormAttack,
            WormDeath,
            PageTurn1,
            PageTurn2,
        }

        public void Update()
        {
            listener.Position = mAvatar.Peek.PlayerModel.Link.Position;

            for (int i = 0; i < soundInstances.Length; ++i)
            {
                if (soundInstances[i] != null && soundInstances[i].State == SoundState.Stopped)
                {
                    soundInstances[i].Dispose();
                    soundInstances[i] = null;
                }
            }
        }

        #region Load Music/Sound
        //If you are too lazy to load in individual music and sounds
        //just load em All!
        public void LoadAllMusic()
        {
            LoadMusic("Level15", "Audio\\Music\\background2");
        }
        public void LoadAllSounds()
        {
            LoadSound(SoundName.LightAttack, "Audio\\Sound\\whoosh");
            LoadSound(SoundName.LightAttackHit, "Audio\\Sound\\jab");
            LoadSound(SoundName.HeavyAttack, "Audio\\Sound\\whoosh");
            LoadSound(SoundName.HeavyAttackHit, "Audio\\Sound\\jab");
            LoadSound(SoundName.Jump, "Audio\\Sound\\jump");
            LoadSound(SoundName.SpecialOnOff, "Audio\\Sound\\transitionToAbility");
            LoadSound(SoundName.PlayerHit, "Audio\\Sound\\jab");
            LoadSound(SoundName.PlayerDeath, "Audio\\Sound\\dying");
            LoadSound(SoundName.WormChase, "Audio\\Sound\\jump");
            LoadSound(SoundName.WormAttack, "Audio\\Sound\\whoosh");
            LoadSound(SoundName.WormDeath, "Audio\\Sound\\dying");
            LoadSound(SoundName.PageTurn1, "Audio\\Sound\\turnPage1");
            LoadSound(SoundName.PageTurn2, "Audio\\Sound\\turnPage2");
        }
        //if musicName does not already exist, add it to dictionary
        //else do not add it!
        public void LoadMusic(string musicName, string musicPath)
        {
            if (!songs.ContainsKey(musicName))
            {
                songs.Add(musicName, Engine.GameContainer.Load<Song>(musicPath));
            }
        }
        //if soundName does not already exist, add it to dictionary
        //else do not add it!
        public void LoadSound(SoundName soundName, string soundPath)
        {
            Sound sound = new Sound(soundPath);
            if (!sounds.ContainsKey(soundName))
            {
                sounds.Add(soundName, sound);
            }
        }
        #endregion

        #region PlayMusic()
        //Play musicName only if it exists in dictionary
        public void PlayMusic(string musicName)
        {
            if (songs.ContainsKey(musicName))
            {
                currentSong = songs[musicName];
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(currentSong);
            }
        }
        public void PlayMusic(string musicName, bool bLoop)
        {
            if (songs.ContainsKey(musicName))
            {
                currentSong = songs[musicName];
                MediaPlayer.IsRepeating = bLoop;
                MediaPlayer.Play(currentSong);
            }
        }
        #endregion

        #region PlaySound()
        //public void PlaySound(SoundName soundName)
        //{
        //    if (sounds.ContainsKey(soundName))
        //    {
        //        int index = GetAvailableSoundIndex();
        //        if (index != -1)
        //        {
        //            soundInstances[index] = sounds[soundName].Effect.CreateInstance();
        //            if (soundInstances[index].State != SoundState.Playing)
        //            {
        //                soundInstances[index].Play();
        //            }
        //        }
        //    }
        //}
        //Play soundName only if it exists in dictionary
        public void PlaySound(SoundName soundName)
        {
            if (sounds.ContainsKey(soundName))
            {
                sounds[soundName].Play();
            }
        }
        public void PlaySound(SoundName soundName, bool bLoop)
        {
            if (sounds.ContainsKey(soundName))
            {
                sounds[soundName].Loop = bLoop;
                sounds[soundName].Play();
            }
        }
        public void PlaySound(SoundName soundName, bool bLoop, float fPitch, float fPan)
        {
            if (sounds.ContainsKey(soundName))
            {
                sounds[soundName].Loop = bLoop;
                sounds[soundName].Pitch = fPitch;
                sounds[soundName].Pan = fPan;
                sounds[soundName].Play();
            }
        }
        public void PlaySound(SoundName soundName, Vector3 position)
        {
            if (sounds.ContainsKey(soundName))
            {
                sounds[soundName].Position = position;
                sounds[soundName].Apply3D();
                sounds[soundName].Play();
            }
        }
        public void PlaySound(SoundName soundName, Vector3 position, bool bLoop)
        {
            if (sounds.ContainsKey(soundName))
            {
                sounds[soundName].Position = position;
                sounds[soundName].Loop = bLoop;
                sounds[soundName].Apply3D();
                sounds[soundName].Play();
            }
        }
        #endregion

        //find an open slot
        //private int GetAvailableSoundIndex()
        //{
        //    for (int i = 0; i < soundInstances.Length; ++i)
        //    {
        //        if (soundInstances[i] == null)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}

        public void Clear()
        {
            MediaPlayer.Stop();
            songs.Clear();
            sounds.Clear();
        }

        //<EOC>
    }
}
