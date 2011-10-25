using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;



namespace Inkwell.Framework
{
    class AudioMenu: MenuScreen
    {
        struct menuItem
        {
            public Texture2D t2d_Texture;
            public Texture2D t2d_Texture_Original;
            public Texture2D t2d_Texture_Highlight;
            public Texture2D t2d_Texture_Selected;
            public Vector2 v2_Position;
            public Vector2 v2_Size;
        }

        public enum AudioMenuState
        {
            SOUNDVOLUME,
            MUSICVOLUME,
            BACK,
            VOID
        }

        private menuItem menuBackground;
        private menuItem[] menuItems;
        private SpriteFont sf_Font;
        private Color clr_SoundColor = Color.Red;
        private Color clr_MusicColor = Color.Red;
        private const int int_NumOfOptions = 3;
        private bool isBusy = false;
        private bool isChanging = false;

        public AudioMenuState CurrentWorkerState;
        public AudioMenuState PreviousWorkerState;

        public override void Initialize()
        {
            menuBackground = new menuItem();

            menuItems = new menuItem[int_NumOfOptions];
            for (int i = 0; i < int_NumOfOptions; i++)
            {
               menuItems[i] = new menuItem();
            }

            CurrentWorkerState = AudioMenuState.SOUNDVOLUME;
        }

        public override void Load(ContentManager content)
        {
            menuBackground.t2d_Texture = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Audio\\Background\\Audio_Background");
            menuBackground.v2_Position = Vector2.Zero;
            menuBackground.v2_Size = Engine.TempVector2(1280, 720);

            #region Textures

            menuItems[0].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Audio\\Buttons\\Audio_Button_Sound Volume_Normal");
            menuItems[0].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Audio\\Buttons\\Audio_Button_Sound Volume_Select");
            menuItems[0].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Audio\\Buttons\\Audio_Button_Sound Volume_Press");
 
            menuItems[1].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Audio\\Buttons\\Audio_Button_Music Volume_Normal");
            menuItems[1].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Audio\\Buttons\\Audio_Button_Music Volume_Select");
            menuItems[1].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Audio\\Buttons\\Audio_Button_Music Volume_Press");

            menuItems[2].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Normal");
            menuItems[2].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Select");
            menuItems[2].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Press");

            #endregion

            #region Sizes

            menuItems[0].v2_Size = Engine.TempVector2(304, 44);
            menuItems[1].v2_Size = Engine.TempVector2(304, 44);
            menuItems[2].v2_Size = Engine.TempVector2(175, 39);

            #endregion

            menuItems[0].t2d_Texture = menuItems[0].t2d_Texture_Highlight;
            menuItems[0].v2_Position = Engine.TempVector2(315, 340);

            menuItems[1].t2d_Texture = menuItems[1].t2d_Texture_Original;
            menuItems[1].v2_Position = Engine.TempVector2(315, 435);

            menuItems[2].t2d_Texture = menuItems[2].t2d_Texture_Original;
            menuItems[2].v2_Position = Engine.TempVector2(550, 580);

            sf_Font = content.Load<SpriteFont>("Fonts\\Debug");
        }

        /// <summary>
        /// Goes to the "previous" option
        /// </summary>
        private void CycleBack()
        {
            if (CurrentWorkerState == AudioMenuState.SOUNDVOLUME)
            {
                CurrentWorkerState = AudioMenuState.BACK;
            }
            else
            {
                CurrentWorkerState--;
            }

            for (int i = 0; i < int_NumOfOptions; i++)
            {
                if (i == (int)CurrentWorkerState)
                {
                    menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Highlight;
                }
                else
                {
                    menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Original;
                }
            }
        }

        /// <summary>
        /// Goes to the "next" option
        /// </summary>
        private void CycleForward()
        {
            if (CurrentWorkerState == AudioMenuState.BACK)
            {
                CurrentWorkerState = AudioMenuState.SOUNDVOLUME;
            }
            else
            {
                CurrentWorkerState++;
            }

            for (int i = 0; i < int_NumOfOptions; i++)
            {
                if (i == (int)CurrentWorkerState)
                {
                    menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Highlight;
                }
                else
                {
                    menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Original;
                }
            }
        }

        /// <summary>
        /// Puts the worker state on void
        /// </summary>
        public void MenuVoid()
        {
            PreviousWorkerState = CurrentWorkerState;
            CurrentWorkerState = AudioMenuState.VOID;
        }

        public override void Update()
        {
            if (CurrentWorkerState != AudioMenuState.VOID)
            {
                #region VolumeControls

                if (isBusy)
                {         
                    switch (CurrentWorkerState)
                    {
                        case AudioMenuState.SOUNDVOLUME:
                            if (mInput.Peek.IsKeyDown(Keys.W) || mInput.Peek.IsKeyDown(Keys.Up)
                                || mInput.Peek.IsKeyDown(Keys.D) || mInput.Peek.IsKeyDown(Keys.Right))
                            {
                                if (mMenu.Peek.SoundVolume != 100)
                                {
                                    mMenu.Peek.SoundVolume++;
                                }
                            }

                            if (mInput.Peek.IsKeyDown(Keys.S) || mInput.Peek.IsKeyDown(Keys.Down)
                                || mInput.Peek.IsKeyDown(Keys.A) || mInput.Peek.IsKeyDown(Keys.Left))
                            {
                                if (mMenu.Peek.SoundVolume != 0)
                                {
                                    mMenu.Peek.SoundVolume--;
                                }
                            }

                            if (mInput.Peek.IsKeyPressed(Keys.Enter))
                            {
                                clr_SoundColor = Color.Red;
                                isBusy = false;
                            }

                            break;

                        case AudioMenuState.MUSICVOLUME:
                            if (mInput.Peek.IsKeyDown(Keys.W) || mInput.Peek.IsKeyDown(Keys.Up)
                                || mInput.Peek.IsKeyDown(Keys.D) || mInput.Peek.IsKeyDown(Keys.Right))
                            {
                                if (mMenu.Peek.MusicVolume != 100)
                                {
                                    mMenu.Peek.MusicVolume++;
                                }
                            }

                            if (mInput.Peek.IsKeyDown(Keys.S) || mInput.Peek.IsKeyDown(Keys.Down)
                                || mInput.Peek.IsKeyDown(Keys.A) || mInput.Peek.IsKeyDown(Keys.Left))
                            {
                                if (mMenu.Peek.MusicVolume != 0)
                                {
                                    mMenu.Peek.MusicVolume--;
                                }
                            }

                            if (mInput.Peek.IsKeyPressed(Keys.Enter))
                            {
                                clr_MusicColor = Color.Red;
                                isBusy = false;
                            }

                            break;
                    }
                }

                #endregion

                else
                {
                    if (mInput.Peek.IsKeyPressed(Keys.W) || mInput.Peek.IsKeyPressed(Keys.Up))
                    {
                        CycleBack();
                    }

                    if (mInput.Peek.IsKeyPressed(Keys.S) || mInput.Peek.IsKeyPressed(Keys.Down))
                    {
                        CycleForward();
                    }

                    if (isChanging)
                    {
                        mMenu.Peek.GoToPreviousState();

                        menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;

                        isChanging = false;

                        MenuVoid();
                    }

                    if (mInput.Peek.IsKeyPressed(Keys.Enter))
                    {
                        switch (CurrentWorkerState)
                        {

                            case AudioMenuState.SOUNDVOLUME:
                                clr_SoundColor = Color.Green;
                                isBusy = true;
                                break;

                            case AudioMenuState.MUSICVOLUME:
                                clr_MusicColor = Color.Green;
                                isBusy = true;
                                break;

                            case AudioMenuState.BACK:
                                menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Selected;
                                isChanging = true;
                                break;

                            default:
                                break;
                        }

                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            mGraphics.Peek.ToggleSpriteDraw();
            spriteBatch.Draw(menuBackground.t2d_Texture, new Rectangle((int)menuBackground.v2_Position.X, (int)menuBackground.v2_Position.Y, (int)menuBackground.v2_Size.X, (int)menuBackground.v2_Size.Y), Color.White);

            spriteBatch.DrawString(sf_Font, mMenu.Peek.SoundVolume.ToString(), Engine.TempVector2(menuItems[0].v2_Position.X + 375, menuItems[0].v2_Position.Y + 5), clr_SoundColor, 0.0f, Vector2.Zero, Engine.TempVector2(1.5f * (float) mGraphics.Peek.BackBufferResolution.X / 1280.0f, 1.5f * (float) mGraphics.Peek.BackBufferResolution.Y / 720.0f), SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(sf_Font, mMenu.Peek.MusicVolume.ToString(), Engine.TempVector2(menuItems[1].v2_Position.X + 375, menuItems[1].v2_Position.Y + 5), clr_MusicColor, 0.0f, Vector2.Zero, Engine.TempVector2(1.5f * (float) mGraphics.Peek.BackBufferResolution.X / 1280.0f, 1.5f * (float) mGraphics.Peek.BackBufferResolution.Y / 720.0f), SpriteEffects.None, 0.0f);

            for (int i = 0; i < int_NumOfOptions; i++)
            {
                spriteBatch.Draw(menuItems[i].t2d_Texture, new Rectangle((int)menuItems[i].v2_Position.X, (int)menuItems[i].v2_Position.Y, (int)menuItems[i].v2_Size.X, (int)menuItems[i].v2_Size.Y), Color.White);
            }

            mGraphics.Peek.ToggleSpriteDraw();
        }
    }
}
