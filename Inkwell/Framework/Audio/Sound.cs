using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Inkwell.Framework
{
    public class Sound
    {
        private SoundEffect effect;
        private SoundEffectInstance instance;
        private float volume = mAudio.Peek.SoundVolume;
        private float pitch = 0.0f;
        private float pan = 0.0f;
        private bool loop = false;
        private AudioEmitter emitter = new AudioEmitter();

        //accessors & mutators
        public float Pitch
        {
            get { return pitch; }
            set { pitch = MathHelper.Clamp(value, -1.0f, 1.0f); }
        }
        public float Pan
        {
            get { return pan; }
            set { pan = MathHelper.Clamp(value, -1.0f, 1.0f); }
        }
        public bool Loop
        {
            get { return loop; }
            set { loop = value; }
        }
        public Vector3 Position
        {
            get { return emitter.Position; }
            set { emitter.Position = value; }
        }

        //Constructor
        public Sound(string soundPath)
        {
            this.effect = Engine.GameContainer.Load<SoundEffect>(soundPath);
            this.instance = effect.CreateInstance();
            instance.Volume = this.volume;
            instance.Pitch = this.pitch;
            instance.Pan = this.pan;
            instance.IsLooped = this.loop;
        }

        //Turn into 3D sound
        public void Apply3D()
        {
            if (!instance.IsDisposed)
                instance.Apply3D(mAudio.Peek.Listener, emitter);
        }
        //sound effect current state (0 = Playing, 1 = Paused, 2 = Stopped)
        public SoundState State
        {
            get { return this.instance.State; }
        }
        //create instance of sound effect
        public void CreateInstance()
        {
            if (instance.IsDisposed)
                instance = effect.CreateInstance();
        }
        //dispose sound instance
        public void Dispose()
        {
            if (!instance.IsDisposed)
            {
                instance.Stop();
                instance.Dispose();
            }
        }
        //play sound
        public void Play()
        {
            if (!instance.IsDisposed)
                if (instance.State != SoundState.Playing)
                    instance.Play();
        }
        //pause sound
        public void Pause()
        {
            if (!instance.IsDisposed)
                if (instance.State == SoundState.Playing)
                    instance.Pause();
        }
        //resume sound if paused
        public void Resume()
        {
            if (!instance.IsDisposed)
                if (instance.State == SoundState.Paused)
                    instance.Resume();
        }
        //stop sound completely from playing or paused
        public void Stop()
        {
            if (!instance.IsDisposed)
                if (instance.State == SoundState.Playing || instance.State == SoundState.Paused)
                    instance.Stop();
        }
    }
}