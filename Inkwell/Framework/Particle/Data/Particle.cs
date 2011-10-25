using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Inkwell.Framework;

namespace Inkwell.Framework.Particle
{
    /// <summary>(Abstract) Stores graphical information and Position of a particle used with an emitter.</summary>
    public class Particle
    {
        /// <summary>Velocity of the Particle, it's current speed.</summary>
        public Vector3 Velocity = Vector3.Zero;
        /// <summary>Rate at which it falls this is not a real weight.</summary>
        public float Weight = 0.0f;
        /// <summary>Life of the Particle useful for just tracking some information</summary>
        public float Life = 0.0f;
        public bool Initialized = false;
        public bool Active = false;
        public Direction Direction = Direction.Up;
        public static void Draw(VertexPositionColor[] Points, Texture2D Texture)
        {
            //PUT IN DRAW CODE FOR PARTICLES HERE
            mGraphics.Peek.Device().RenderState.PointSpriteEnable = true;
            mGraphics.Peek.ToggleAlphaBlending(true);
            mGraphics.Peek.Device().RenderState.DepthBufferWriteEnable = false;
            mGraphics.Peek.Device().VertexDeclaration = mGraphics.Peek.vdPositionColor;
            mEffect.Peek.PointEffect().Parameters["WVPMatrix"].SetValue(Matrix.Identity * mCamera.Peek.ReturnCamera().View * mCamera.Peek.ReturnCamera().Projection);
            mEffect.Peek.PointEffect().Parameters["SpriteTexture"].SetValue(Texture);
            mEffect.Peek.PointEffect().Parameters["ViewportHeight"].SetValue(mGraphics.Peek.Device().Viewport.Height);
            mEffect.Peek.PointEffect().Parameters["ViewportHeight"].SetValue(25.0f);

            mEffect.Peek.PointEffect().Begin();
            for (int i = 0; i < mEffect.Peek.PointEffect().CurrentTechnique.Passes.Count; i++)
            {
                mEffect.Peek.PointEffect().CurrentTechnique.Passes[i].Begin();
                mGraphics.Peek.Device().DrawUserPrimitives<VertexPositionColor>(PrimitiveType.PointList, Points, 0, Points.Length);
                mEffect.Peek.PointEffect().CurrentTechnique.Passes[i].End();
            }
            mEffect.Peek.PointEffect().End();

            mGraphics.Peek.Device().RenderState.PointSpriteEnable = false;
            mGraphics.Peek.Device().RenderState.DepthBufferWriteEnable = true;
            mGraphics.Peek.ToggleAlphaBlending(false);
        }
    }
}
