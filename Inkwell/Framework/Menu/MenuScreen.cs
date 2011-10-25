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
    /// (Abstract Class) Stores methods and basic information for creating a Menu Screen.
    /// </summary>
    abstract class MenuScreen
    {
        public abstract void Initialize();
        public abstract void Load(ContentManager content);
        public abstract void Update();
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
