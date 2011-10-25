using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inkwell.Framework.Triggers
{
    abstract class Trigger
    {
        protected float PositionX;
        public abstract void Initialize(float PositionX);
        public abstract bool Update(Vector3 PlayerPosition);
        public abstract void Draw();
    }
}
