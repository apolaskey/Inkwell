using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using Inkwell.Framework.Triggers;

namespace Inkwell.Framework
{
    class DialogueTrigger: Trigger
    {
        public Texture2D Texture;
        public Vector2 TexOrigin;
        public Color Color;
        float Rotation;
        float Scale;
        bool Display = false;
        bool isStart = true;

        public override void Initialize(float PositionX)
        {
            base.PositionX = PositionX;
            this.Rotation = 0.0f;
            this.Scale = 1.0f;
            this.Color = Color.White;
        }
        public void LoadTexture(string strAssetLocation)
        {
            this.Texture = Engine.GameContainer.Load<Texture2D>(strAssetLocation);
            this.TexOrigin = Engine.TempVector2(this.Texture.Width / 2, this.Texture.Height / 2);
        }
        public void LoadTexture(Texture2D Texture)
        {
            this.Texture = Texture;
            this.TexOrigin = Engine.TempVector2(this.Texture.Width / 2, this.Texture.Height / 2);
        }

        /// <summary>
        /// If return true, activate mDialogue.Peek.DialogueContinue();
        /// </summary>
        /// <param name="TargetBounds"></param>
        /// <param name="PositionOffset"></param>
        /// <returns></returns>
        public override bool Update(Vector3 PlayerPosition)
        {
            if (PlayerPosition.X >= base.PositionX && isStart)
            {
                mAvatar.Peek.Disable();
                isStart = false;
                return true;
            }

            else
            {
                return false;
            }
        }

        /// <summary>
        /// Use to move trigger
        /// </summary>
        /// <param name="Position"></param>
        public void MoveTrigger(float Position)
        {
            base.PositionX = Position;
            isStart = true;
        }

        public override void Draw()
        {
            if (Display)
            {
                mGraphics.Peek.ToggleSpriteDraw();
                mGraphics.Peek.SpriteBatch.Draw(Texture, mGraphics.Peek.ScreenResolution / 2, null, Color, Rotation, TexOrigin, Scale, SpriteEffects.None, 0.0f);
                mGraphics.Peek.ToggleSpriteDraw();
            }
        }
    }
}
