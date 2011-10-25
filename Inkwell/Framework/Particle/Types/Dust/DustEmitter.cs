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
    class DustEmitter: Emitter
    {
        cModel _test = new cModel();
        const float DIVISIONAL_COEFFICENT = 256.0f;
        const float LIFE_COEFFCIENT = .05f;
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
                _Points[i].Color = Color.White;
                _Points[i].Position = Engine.TempVector3(Engine.Randomize((int)EnviromentBounds.Min.X, (int)EnviromentBounds.Max.X),
                    Engine.Randomize((int)EnviromentBounds.Min.Y, (int)EnviromentBounds.Max.Y + 100),
                    Engine.Randomize((int)EnviromentBounds.Min.Z, (int)EnviromentBounds.Max.Z));
                _pContainer[i].Velocity.Y = Engine.Randomize(1, 3) / 3;
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
                    Engine.Randomize((int)EnviromentBounds.Min.Y, (int)EnviromentBounds.Max.Y + 100),
                    Engine.Randomize((int)EnviromentBounds.Min.Z, (int)EnviromentBounds.Max.Z));
                float a = Engine.Randomize(2, 6);
                _pContainer[i].Velocity.Y = (a / DIVISIONAL_COEFFICENT);
                if(Engine.Randomize(1, 3) == 1)
                _pContainer[i].Velocity.X = (a / DIVISIONAL_COEFFICENT);
                else _pContainer[i].Velocity.X = -(a / DIVISIONAL_COEFFICENT);

                if(Engine.Randomize(1, 3) == 2)
                    _pContainer[i].Velocity.Z = (a / DIVISIONAL_COEFFICENT);
                else _pContainer[i].Velocity.Z = -(a / DIVISIONAL_COEFFICENT);
            }
        }

        public override void Update()
        {
            /*This totally makes me ::Sad Face:: but I cant really think of another way to check*/
            for (int i = 0; i < _pContainer.Length; i++)
            {
                _Points[i].Position += _pContainer[i].Velocity;

                if (_Points[i].Position.Y > EnviromentBounds.Max.Y + 100)
                {
                    _Points[i].Position = Engine.TempVector3(Engine.Randomize((int)EnviromentBounds.Min.X, (int)EnviromentBounds.Max.X),
                    0.0f,
                    Engine.Randomize((int)EnviromentBounds.Min.Z, (int)EnviromentBounds.Max.Z));
                }
                else if (_Points[i].Position.X > EnviromentBounds.Max.X)
                {
                    _Points[i].Position = Engine.TempVector3(Engine.Randomize((int)EnviromentBounds.Min.X, (int)EnviromentBounds.Max.X),
                    0.0f,
                    Engine.Randomize((int)EnviromentBounds.Min.Z, (int)EnviromentBounds.Max.Z));
                }
                else if(_Points[i].Position.X < EnviromentBounds.Min.X)
                {
                    _Points[i].Position = Engine.TempVector3(Engine.Randomize((int)EnviromentBounds.Min.X, (int)EnviromentBounds.Max.X),
                    0.0f,
                    Engine.Randomize((int)EnviromentBounds.Min.Z, (int)EnviromentBounds.Max.Z));
                }
                else if (_Points[i].Position.Z > EnviromentBounds.Max.Z)
                {
                    _Points[i].Position = Engine.TempVector3(Engine.Randomize((int)EnviromentBounds.Min.X, (int)EnviromentBounds.Max.X),
                    0.0f,
                    Engine.Randomize((int)EnviromentBounds.Min.Z, (int)EnviromentBounds.Max.Z));
                }
                else if (_Points[i].Position.Z < EnviromentBounds.Min.Z)
                {
                    _Points[i].Position = Engine.TempVector3(Engine.Randomize((int)EnviromentBounds.Min.X, (int)EnviromentBounds.Max.X),
                    0.0f,
                    Engine.Randomize((int)EnviromentBounds.Min.Z, (int)EnviromentBounds.Max.Z));
                }
            }
        }

        public override void Draw()
        {
            Particle.Draw(base._Points, base._t2dMainTexture);
        }
    }
}
