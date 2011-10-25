using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Inkwell.Framework.Cinematic
{
    /// <summary>
    /// (Singleton) In Charge of updating and drawing video content.
    /// </summary>
    public sealed class mCinematic
    {
        private static volatile mCinematic _instance;
        private static object _padlock = new Object();

        public static mCinematic Peek
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
                            _instance = new mCinematic(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        //<EOC>
    }
}
