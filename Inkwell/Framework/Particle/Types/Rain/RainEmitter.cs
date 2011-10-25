using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Inkwell.Framework.Particle
{
    /// <summary>Generates Dust on Screen in 3D Space</summary>
    class RainEmitter : Emitter
    {
        cModel _test = new cModel();
        const float MAX_MOVEMENT = 1.0f;
        private Vector3 WindStrength = Vector3.Zero;
        public override void Initialize(Texture2D TextureForParticle, int ParticleCount, int MaxLife, Direction Direction, BoundingBox EnviromentBounds)
        {
            base._t2dMainTexture = TextureForParticle;
            base._pContainer = new Particle[ParticleCount];
            base.EnviromentBounds = EnviromentBounds;
            base._Points = new VertexPositionColor[ParticleCount];
            for (int i = 0; i < ParticleCount; i++)
            {
                _pContainer[i] = new Particle();
                _pContainer[i].Life = Engine.Randomize(MaxLife / 4, MaxLife);
                _Points[i].Color = Color.CornflowerBlue;
                _Points[i].Position = Engine.TempVector3(Engine.Randomize((int)EnviromentBounds.Min.X, (int)EnviromentBounds.Max.X),
                    Engine.Randomize((int)EnviromentBounds.Min.Y, (int)EnviromentBounds.Max.Y + 200),
                    Engine.Randomize((int)EnviromentBounds.Min.Z, (int)EnviromentBounds.Max.Z));
                _pContainer[i].Velocity.Y = -Engine.Randomize(3, 8);
            }
        }
        public override void Initialize(Texture2D TextureForParticle, int ParticleCount, int MaxLife, Direction Direction, BoundingBox EnviromentBounds, Color Color)
        {
            base._t2dMainTexture = TextureForParticle;
            base._pContainer = new Particle[ParticleCount];
            base.EnviromentBounds = EnviromentBounds;
            base._Points = new VertexPositionColor[ParticleCount];
            for (int i = 0; i < ParticleCount; i++)
            {
                _pContainer[i] = new Particle();
                _pContainer[i].Life = Engine.Randomize(MaxLife / 4, MaxLife);
                _Points[i].Color = Color;
                _Points[i].Position = Engine.TempVector3(Engine.Randomize((int)EnviromentBounds.Min.X, (int)EnviromentBounds.Max.X),
                    Engine.Randomize((int)EnviromentBounds.Min.Y, (int)EnviromentBounds.Max.Y + 200),
                    Engine.Randomize((int)EnviromentBounds.Min.Z, (int)EnviromentBounds.Max.Z));
                _pContainer[i].Velocity.Y = -Engine.Randomize(3, 8);
            }
        }

        public override void Update()
        {
            for (int i = 0; i < _pContainer.Length; i++)
            {
                _Points[i].Position += _pContainer[i].Velocity;

                if (_Points[i].Position.Y <= EnviromentBounds.Min.Y - 5)
                {
                    _pContainer[i].Velocity.Y = -Engine.Randomize(3, 8);
                    _Points[i].Position.Y = 200;
                }
            }
        }

        public override void Draw()
        {
            Particle.Draw(base._Points, base._t2dMainTexture);
        }
    }
}
