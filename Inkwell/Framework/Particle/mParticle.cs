using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Inkwell.Framework
{
    /// <summary>
    /// Particle Manager tasked with storing particle emitters and their appropriate locations and effects.
    /// </summary>
    class mParticle
    {
        private static volatile mParticle _instance;
        private static object _padlock = new Object();

        public static mParticle Peek
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
                            _instance = new mParticle(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        //<EOC>
    }
}
