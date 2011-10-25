using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Inkwell.Framework
{
    static class Vignette
    {
        public static bool Enabled = false;
        static Effect _VigEffect;
        public static RenderTarget2D VignetteTarget;
        public static void Load(ContentManager Content, GraphicsDevice Device)
        {
            _VigEffect = Content.Load<Effect>("Graphics\\Shaders\\VignetteEffect");
            VignetteTarget = mGraphics.Peek.CreateRenderTarget(1, Device.PresentationParameters.BackBufferFormat);
        }
        public static void Reset(GraphicsDevice Device)
        {
            VignetteTarget.Dispose();
            VignetteTarget = null;
            VignetteTarget = mGraphics.Peek.CreateRenderTarget(1, Device.PresentationParameters.BackBufferFormat);
        }
        public static void Draw(SpriteBatch SpriteBatch, Texture2D SceneTexture)
        {
                mGraphics.Peek.BeginTargetedDraw(VignetteTarget);
                SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
                {
                    _VigEffect.Begin();
                    {
                        _VigEffect.Parameters["VignetteRadius"].SetValue(0.0f); //<-- Controls the Radius of the Effect
                        _VigEffect.CurrentTechnique.Passes[0].Begin();
                        {
                            SpriteBatch.Draw(SceneTexture, Vector2.Zero, Color.Red);
                            _VigEffect.CurrentTechnique.Passes[0].End();
                        }
                    }
                    _VigEffect.End();
                }
                SpriteBatch.End();
                mGraphics.Peek.EndTargetedDraw();
        }
    }
}
