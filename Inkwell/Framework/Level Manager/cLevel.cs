using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inkwell.Framework.Graphics.Data;
/*Andrew Polaskey - 12.13.2010*/
namespace Inkwell.Framework
{
    public abstract class cLevel
    {
        //public int ID = Engine.NULLED_INT; //<-- NULL Value
        /// <summary>Used to Load or Re-load Content back to it's original state.</summary>
        public virtual void Initialize() 
        {
            Engine.PurgeGarbageHeap();
        }
        /// <summary>Used to Hook into Core's Update and capture updates frame-by-frame.</summary>
        public abstract void Update();
        /// <summary>Used to Draw various information to the screen.</summary>
        public abstract void Draw();
        /// <summary>Used to Destroy Content created by a level.</summary>
        public virtual void Kill() 
        {
            mAI.Peek.enemyList.Clear();
            mAnimation.Peek.heroFrameSpeed.Clear();
            mModel.Peek.CleanUp();
            mAudio.Peek.Clear();
            mDebug.CleanUp();
            Engine.GameContainer.Unload();
            Engine.PurgeGarbageHeap();
        }
    }
}
