using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Inkwell.Framework.Particle
{
    /// <summary>Direction the Emitter will be facing in. (Useful for thrust based effects).</summary>
    public enum Direction { Up, Down, Left, Right, Forward, Backward};
    /// <summary>
    /// Particle Emitter in charge of working with particles.
    /// </summary>
    abstract class Emitter
    {
        protected Particle[] _pContainer;
        protected VertexPositionColor[] _Points;
        protected Texture2D _t2dMainTexture;
        protected BoundingBox EnviromentBounds;
        public abstract void Initialize(Texture2D TextureForParticle, int ParticleCount, int MaxLife, Direction Direction, BoundingBox EnviromentBounds);
        public abstract void Initialize(Texture2D TextureForParticle, int ParticleCount, int MaxLife, Direction Direction, BoundingBox EnviromentBounds, Color Color);
        public abstract void Update();
        public abstract void Draw();
    }
}
