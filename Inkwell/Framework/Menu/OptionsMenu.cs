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
    class OptionsMenu: MenuScreen
    {
        struct menuItem
        {
            public Texture2D t2d_Texture;
            public Texture2D t2d_Texture_Original;
            public Texture2D t2d_Texture_Highlight;
            public Texture2D t2d_Texture_Press;
            public Vector2 v2_Position;
            public Vector2 v2_Size;
        }

        public enum OptionsMenuState
        {
            AUDIO,
            VIDEO,
            CONTROLS,
            BACK,
            VOID
        }

        private menuItem menuBackground;
        private menuItem[] menuItems;
        private const int int_NumOfOptions = 4;
        private bool isChanging = false;

        public OptionsMenuState CurrentWorkerState;
        public OptionsMenuState PreviousWorkerState;

        public override void Initialize()
        {
            menuBackground = new menuItem();

            menuItems = new menuItem[int_NumOfOptions];
            for (int i = 0; i < int_NumOfOptions; i++)
            {
               menuItems[i] = new menuItem();
            }

            CurrentWorkerState = OptionsMenuState.AUDIO;
        }

        public override void Load(ContentManager content)
        {
            menuBackground.t2d_Texture = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Options\\Background\\Options_Background");
            menuBackground.v2_Position = Vector2.Zero;
            menuBackground.v2_Size = Engine.TempVector2(1280, 720);

            #region Textures

            menuItems[0].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Options\\Buttons\\Options_Button_Audio_Normal");
            menuItems[0].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Options\\Buttons\\Options_Button_Audio_Select");
            menuItems[0].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Options\\Buttons\\Options_Button_Audio_Press");
           
            menuItems[1].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Options\\Buttons\\Options_Button_Video_Normal");
            menuItems[1].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Options\\Buttons\\Options_Button_Video_Select");
            menuItems[1].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Options\\Buttons\\Options_Button_Video_Press");
           
            menuItems[2].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Options\\Buttons\\Options_Button_Controls_Normal");
            menuItems[2].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Options\\Buttons\\Options_Button_Controls_Select");
            menuItems[2].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Options\\Buttons\\Options_Button_Controls_Press");
          
            menuItems[3].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Normal");
            menuItems[3].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Select");
            menuItems[3].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Press");

            #endregion

            #region Sizes

            menuItems[0].v2_Size = Engine.TempVector2(235, 58);
            menuItems[1].v2_Size = Engine.TempVector2(235, 58);
            menuItems[2].v2_Size = Engine.TempVector2(242, 58);
            menuItems[3].v2_Size = Engine.TempVector2(175, 39);

            #endregion

            menuItems[0].t2d_Texture = menuItems[0].t2d_Texture_Highlight;
            menuItems[0].v2_Position = Engine.TempVector2(275, 415);

            for (int i = 1; i < int_NumOfOptions - 1; i++)
            {
                menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Original;
                menuItems[i].v2_Position = Engine.TempVector2(menuItems[0].v2_Position.X + (240 * i), menuItems[0].v2_Position.Y);
            }

            menuItems[3].t2d_Texture = menuItems[3].t2d_Texture_Original;
            menuItems[3].v2_Position = Engine.TempVector2(550, 580);
        }

        #region Cycling

        /// <summary>
        /// Goes to the "previous" option
        /// </summary>
        private void CycleBack()
        {
            PreviousWorkerState = CurrentWorkerState;

            if (CurrentWorkerState == OptionsMenuState.AUDIO)
            {
                CurrentWorkerState = OptionsMenuState.BACK;
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
            PreviousWorkerState = CurrentWorkerState;

            if (CurrentWorkerState == OptionsMenuState.BACK)
            {
                CurrentWorkerState = OptionsMenuState.AUDIO;
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
        /// Goes directly to Back options
        /// </summary>
        private void CycleDown()
        {
            if (CurrentWorkerState != OptionsMenuState.BACK)
            {
                PreviousWorkerState = CurrentWorkerState;
                CurrentWorkerState = OptionsMenuState.BACK;
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
        /// Goes back to previous option if skipped to Back
        /// </summary>
        private void CycleUp()
        {
            if (CurrentWorkerState == OptionsMenuState.BACK)
            {
                if (PreviousWorkerState == OptionsMenuState.BACK)
                {
                    CurrentWorkerState = OptionsMenuState.AUDIO;
                }

                else
                {
                    CurrentWorkerState = PreviousWorkerState;
                }
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

        #endregion

        /// <summary>
        /// Puts the worker state on void
        /// </summary>
        public void MenuVoid()
        {
            PreviousWorkerState = CurrentWorkerState;
            CurrentWorkerState = OptionsMenuState.VOID;
        }

        public override void Update()
        {
            if (CurrentWorkerState != OptionsMenuState.VOID)
            {
                if(mInput.Peek.IsKeyPressed(Keys.A) || mInput.Peek.IsKeyPressed(Keys.Left))
                {
                    CycleBack();
                }

                if (mInput.Peek.IsKeyPressed(Keys.D) || mInput.Peek.IsKeyPressed(Keys.Right))
                {
                    CycleForward();
                }

                if (mInput.Peek.IsKeyPressed(Keys.S) || mInput.Peek.IsKeyPressed(Keys.Down))
                {
                    CycleDown();
                }

                if (mInput.Peek.IsKeyPressed(Keys.W) || mInput.Peek.IsKeyPressed(Keys.Up))
                {
                    CycleUp();
                }

                if (isChanging)
                {
                    switch (CurrentWorkerState)
                    {
                        case OptionsMenuState.AUDIO:
                            mMenu.Peek.WorkerState = mMenu.MenuState.AUDIO;
                            break;

                        case OptionsMenuState.VIDEO:
                            mMenu.Peek.WorkerState = mMenu.MenuState.VIDEO;
                            break;

                        case OptionsMenuState.CONTROLS:
                            mMenu.Peek.WorkerState = mMenu.MenuState.CONTROLS;
                            break;

                        case OptionsMenuState.BACK:
                            mFile.Peek.SaveOptions();
                            mMenu.Peek.WorkerState = mMenu.MenuState.MAIN;
                            break;

                        default:
                            break;
                    }

                    menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;

                    isChanging = false;

                    MenuVoid();
                }

                if (mInput.Peek.IsKeyPressed(Keys.Enter))
                {
                    menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Press;
                    isChanging = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            mGraphics.Peek.ToggleSpriteDraw();
            spriteBatch.Draw(menuBackground.t2d_Texture, new Rectangle((int)menuBackground.v2_Position.X, (int)menuBackground.v2_Position.Y, (int)menuBackground.v2_Size.X, (int)menuBackground.v2_Size.Y), Color.White);

            for (int i = 0; i < int_NumOfOptions; i++)
            {
                spriteBatch.Draw(menuItems[i].t2d_Texture, new Rectangle((int)menuItems[i].v2_Position.X, (int)menuItems[i].v2_Position.Y, (int)menuItems[i].v2_Size.X, (int)menuItems[i].v2_Size.Y), Color.White);
            }

            mGraphics.Peek.ToggleSpriteDraw();
        }
    }
}
