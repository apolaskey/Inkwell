using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Inkwell.Framework.Graphics;



namespace Inkwell.Framework
{
    class UI: MenuScreen
    {
        struct menuItem
        {
            public Texture2D t2d_Texture;
            public Vector2 v2_Position;
        }

        private menuItem UIBackground;
        private Texture2D[] numbers;
        private menuItem inkMeters;
        private menuItem[] inkNumbers;
        private const int int_NumOfInkNumbers = 3;
        private menuItem healthMeters;
        private menuItem[] healthNumbers;
        private const int int_NumOfHealthNumbers = 3;
        private int int_Length = 0;
        private char[] char_Temp;

        public override void Initialize()
        {
            UIBackground = new menuItem();

            numbers = new Texture2D[10];

            inkMeters = new menuItem();

            inkNumbers = new menuItem[int_NumOfInkNumbers];
            for (int i = 0; i < int_NumOfInkNumbers; i++)
            {
                inkNumbers[i] = new menuItem();
            }

            healthMeters = new menuItem();

            healthNumbers = new menuItem[int_NumOfHealthNumbers];
            for (int i = 0; i < int_NumOfHealthNumbers; i++)
            {
                healthNumbers[i] = new menuItem();
            }

            char_Temp = new char[4];
        }

        public override void Load(ContentManager content)
        {
            #region Textures

            UIBackground.t2d_Texture = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\UI\\HUD Background");

            for (int i = 0; i < 10; i++)
            {
                numbers[i] = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\UI\\Numbers\\" + i.ToString());
            }

            inkMeters.t2d_Texture = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\UI\\HUD Ink");

            healthMeters.t2d_Texture = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\UI\\HUD Health Bar");

            #endregion

            UIBackground.v2_Position = Engine.TempVector2(640, UIBackground.t2d_Texture.Height / 2);

            inkMeters.v2_Position = Engine.TempVector2(UIBackground.v2_Position.X - 165, UIBackground.v2_Position.Y);

            for (int i = 0; i < int_NumOfInkNumbers; i++)
            {
                inkNumbers[i].t2d_Texture = numbers[0];
                inkNumbers[i].v2_Position = Engine.TempVector2(1150 + (inkNumbers[0].t2d_Texture.Width * i * 0.5f), 620);
            }

            healthMeters.v2_Position = Engine.TempVector2(UIBackground.v2_Position.X + 50, UIBackground.v2_Position.Y);

            for (int i = 0; i < int_NumOfHealthNumbers; i++)
            {
                healthNumbers[i].t2d_Texture = numbers[0];
                healthNumbers[i].v2_Position = Engine.TempVector2(45 + (healthNumbers[0].t2d_Texture.Width * i * 0.5f), 620);
            }
        }

        public override void Update()
        {
            char_Temp = mAvatar.Peek.GetCurrentInk.ToString().ToCharArray();

            int_Length = char_Temp.Length - 1;
            for (int i = int_NumOfInkNumbers - 1; i > -1; i--)
            {
                if (int_Length > -1)
                {
                    inkNumbers[i].t2d_Texture = numbers[(int)Char.GetNumericValue(char_Temp[int_Length])];
                }

                else
                {
                    inkNumbers[i].t2d_Texture = numbers[0];
                }

                int_Length--;
            }

            char_Temp = mAvatar.Peek.GetCurrentHealth.ToString().ToCharArray();

            int_Length = char_Temp.Length - 1;
            for (int i = int_NumOfHealthNumbers - 1; i > -1; i--)
            {
                if (int_Length > -1)
                {
                    healthNumbers[i].t2d_Texture = numbers[(int)Char.GetNumericValue(char_Temp[int_Length])];
                }

                else
                {
                    healthNumbers[i].t2d_Texture = numbers[0];
                }

                int_Length--;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            mGraphics.Peek.ToggleSpriteDraw();

            spriteBatch.Draw(UIBackground.t2d_Texture, UIBackground.v2_Position, null, Color.White, 0.0f, Engine.TempVector2(UIBackground.t2d_Texture.Width / 2, UIBackground.t2d_Texture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.Draw(inkMeters.t2d_Texture, Engine.TempVector2(inkMeters.v2_Position.X, inkMeters.v2_Position.Y + (inkMeters.t2d_Texture.Height * (100 - mAvatar.Peek.GetCurrentInk) / 100)), new Rectangle(0, (int)(inkMeters.t2d_Texture.Height * (100 - mAvatar.Peek.GetCurrentInk) / 100), (int)inkMeters.t2d_Texture.Width, (int)inkMeters.t2d_Texture.Height), Color.White, 0.0f, Engine.TempVector2(inkMeters.t2d_Texture.Width / 2, inkMeters.t2d_Texture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);

            //for (int i = 0; i < int_NumOfInkNumbers; i++)
            //{
            //    spriteBatch.Draw(inkNumbers[i].t2d_Texture, Engine.TempVector2(inkNumbers[i].v2_Position.X, inkNumbers[i].v2_Position.Y), null, Color.White, 0.0f, Engine.TempVector2(inkNumbers[i].t2d_Texture.Width / 2, inkNumbers[i].t2d_Texture.Height / 2), 0.5f, SpriteEffects.None, 0.0f);
            //}

            spriteBatch.Draw(healthMeters.t2d_Texture, healthMeters.v2_Position, new Rectangle(0, 0, (int)(healthMeters.t2d_Texture.Width * (mAvatar.Peek.GetCurrentHealth) / 100), (int)healthMeters.t2d_Texture.Height), Color.White, 0.0f, Engine.TempVector2(healthMeters.t2d_Texture.Width / 2, healthMeters.t2d_Texture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);

            //for (int i = 0; i < int_NumOfHealthNumbers; i++)
            //{
            //    spriteBatch.Draw(healthNumbers[i].t2d_Texture, Engine.TempVector2(healthNumbers[i].v2_Position.X, healthNumbers[i].v2_Position.Y), null, Color.White, 0.0f, Engine.TempVector2(healthNumbers[i].t2d_Texture.Width / 2, healthNumbers[i].t2d_Texture.Height / 2), 0.5f, SpriteEffects.None, 0.0f);
            //}

            mGraphics.Peek.ToggleSpriteDraw();
        }
    }
}
