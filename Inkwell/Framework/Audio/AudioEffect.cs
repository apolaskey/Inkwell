using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Inkwell.Framework
{
    /// <summary>
    /// Used in conjunction with the AudioManager, stores information for audio effects.
    /// </summary>
    public class AudioEffect
    {
        //STORE INFORMATION FOR AUDIO
        SoundEffect _sndEffect;

        SoundEffectInstance _sndInstance;
        public AudioEffect(ContentManager Content, String strAssetLocation, bool bLooping, float fVolume, float fPitch)
        {
            _sndEffect = Content.Load<SoundEffect>(strAssetLocation);
            _sndInstance = _sndEffect.CreateInstance();
            _sndInstance.IsLooped = bLooping;
            _sndInstance.Volume = fVolume;
            _sndInstance.Pitch = fPitch;
        }

        public void Play()
        {
            if (_sndInstance.State != SoundState.Playing)
            {
                _sndInstance.Play();
            }
        }

        public void Stop()
        {
            if (_sndInstance.State == SoundState.Playing || _sndInstance.State == SoundState.Paused)
            {
                _sndInstance.Stop();
            }
        
        }

        
        
    }
}