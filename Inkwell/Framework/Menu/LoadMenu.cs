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
    class LoadMenu: MenuScreen
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

        struct loadSlot
        {
            //public Texture2D t2d_Texture;
            public String str_Level;
            public String str_Time;
        }

        public enum LoadMenuState
        {
            SAVEONE,
            SAVETWO,
            SAVETHREE,
            SAVEFOUR,
            BACK,
            VOID
        }

        private menuItem menuBackground;
        private menuItem[] menuItems;
        private loadSlot[] loadSlots;
        private Vector2 v2_tempTime;
        private SpriteFont sf_Font;
        private int int_NumOfOptions;
        private bool isChanging = false;

        public LoadMenuState CurrentWorkerState;
        public LoadMenuState PreviousWorkerState;

        public override void Initialize()
        {
            menuBackground = new menuItem();

            int_NumOfOptions = 5;

            menuItems = new menuItem[5];
            for (int i = 0; i < int_NumOfOptions; i++)
            {
               menuItems[i] = new menuItem();
            }

            loadSlots = new loadSlot[4];
            for (int i = 0; i < 4; i++)
            {
                loadSlots[i] = new loadSlot();
            }

            v2_tempTime = Vector2.Zero;

            CurrentWorkerState = LoadMenuState.BACK;
            PreviousWorkerState = LoadMenuState.BACK;
        }

        public void LoadSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                loadSlots[i].str_Level = mFile.Peek.getLevel(i + 1);
                v2_tempTime = mFile.Peek.getTime(i + 1);
                loadSlots[i].str_Time = "Hours: " + v2_tempTime.X.ToString() + " Minutes: " + v2_tempTime.Y.ToString();
                if (loadSlots[i].str_Level == "Level: 0")
                {
                    loadSlots[i].str_Level = "EMPTY";
                    loadSlots[i].str_Time = "";
                }
            }
        }

        public override void Load(ContentManager content)
        {
            menuBackground.t2d_Texture = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Background\\Load Menu_Background");
            menuBackground.v2_Position = Vector2.Zero;
            menuBackground.v2_Size = Engine.TempVector2(1280, 720);

            #region Textures

            menuItems[0].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 1_Normal");
            menuItems[0].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 1_Select");
            menuItems[0].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 1_Press");

            menuItems[1].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 2_Normal");
            menuItems[1].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 2_Select");
            menuItems[1].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 2_Press");

            menuItems[2].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 3_Normal");
            menuItems[2].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 3_Select");
            menuItems[2].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 3_Press");

            menuItems[3].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 4_Normal");
            menuItems[3].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 4_Select");
            menuItems[3].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Load Menu\\Buttons\\Load Menu_Button_Save 4_Press");
       
            menuItems[4].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Normal");
            menuItems[4].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Select");
            menuItems[4].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Press");
            
            #endregion

            #region Sizes

            menuItems[0].v2_Size = Engine.TempVector2(194, 43);
            menuItems[1].v2_Size = Engine.TempVector2(194, 43);
            menuItems[2].v2_Size = Engine.TempVector2(194, 43);
            menuItems[3].v2_Size = Engine.TempVector2(194, 43);
            menuItems[4].v2_Size = Engine.TempVector2(175, 39);

            #endregion

            for (int i = 0; i < (int_NumOfOptions - 1); i++)
            {
                menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Original;
                menuItems[i].v2_Position = Engine.TempVector2(235 + (200 * i), 500);
            }

            menuItems[4].t2d_Texture = menuItems[4].t2d_Texture_Highlight;
            menuItems[4].v2_Position = Engine.TempVector2(550, 580);

            LoadSlots();

            sf_Font = content.Load<SpriteFont>("Fonts\\Debug");
        }

        #region Cycling

        /// <summary>
        /// Goes to the "previous" option
        /// </summary>
        private void CycleBack()
        {
            PreviousWorkerState = CurrentWorkerState;

            if (CurrentWorkerState == LoadMenuState.SAVEONE)
            {
                CurrentWorkerState = LoadMenuState.BACK;
            }
            else
            {
                if (CurrentWorkerState == LoadMenuState.BACK)
                {
                    if (int_NumOfOptions != 1)
                    {
                        CurrentWorkerState = (LoadMenuState)(int_NumOfOptions - 2);
                    }
                }
                else
                {
                    CurrentWorkerState--;
                }
            }

            for (int i = 0; i < 5; i++)
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

            if (CurrentWorkerState == LoadMenuState.BACK)
            {
                    if (int_NumOfOptions != 1)
                    {
                        CurrentWorkerState = LoadMenuState.SAVEONE;
                    }
            }
            else
            {
                if (CurrentWorkerState == (LoadMenuState)(int_NumOfOptions - 2))
                {
                    CurrentWorkerState = LoadMenuState.BACK;
                }
                else
                {
                    CurrentWorkerState++;
                }
            }

            for (int i = 0; i < 5; i++)
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
            if (CurrentWorkerState != LoadMenuState.BACK)
            {
                PreviousWorkerState = CurrentWorkerState;
                CurrentWorkerState = LoadMenuState.BACK;
            }

            for (int i = 0; i < 5; i++)
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
            if (CurrentWorkerState == LoadMenuState.BACK)
            {
                if (PreviousWorkerState == LoadMenuState.BACK)
                {
                    CurrentWorkerState = LoadMenuState.SAVEONE;
                }

                else
                {
                    CurrentWorkerState = PreviousWorkerState;
                }
            }

            for (int i = 0; i < 5; i++)
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
            CurrentWorkerState = LoadMenuState.VOID;
        }

        public override void Update()
        {
            if (CurrentWorkerState != LoadMenuState.VOID)
            {
                if (mInput.Peek.IsKeyPressed(Keys.A) || mInput.Peek.IsKeyPressed(Keys.Left))
                {
                    CycleBack();
                }

                if (mInput.Peek.IsKeyPressed(Keys.D) || mInput.Peek.IsKeyPressed(Keys.Right))
                {
                    CycleForward();
                }

                if (mInput.Peek.IsKeyPressed(Keys.W) || mInput.Peek.IsKeyPressed(Keys.Up))
                {
                    CycleUp();
                }

                if (mInput.Peek.IsKeyPressed(Keys.S) || mInput.Peek.IsKeyPressed(Keys.Down))
                {
                    CycleDown();
                }

                if (isChanging)
                {
                    switch (CurrentWorkerState)
                    {
                        case LoadMenuState.BACK:
                            mMenu.Peek.WorkerState = mMenu.MenuState.MAIN;
                            break;

                        case LoadMenuState.SAVEONE:
                            if (loadSlots[0].str_Level != "EMPTY")
                            {
                                mMenu.Peek.LoadSlot = 1;
                                mFile.Peek.LoadGame(mMenu.Peek.LoadSlot);
                                menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;
                                mFile.Peek.setStartTime();
                                mMenu.Peek.WorkerState = mMenu.MenuState.VOID;
                                MenuVoid();
                            }
                            else
                            {
                                menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;
                            }
                            break;

                        case LoadMenuState.SAVETWO:
                            if (loadSlots[1].str_Level != "EMPTY")
                            {
                                mMenu.Peek.LoadSlot = 2;
                                mFile.Peek.LoadGame(mMenu.Peek.LoadSlot);
                                menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;
                                mFile.Peek.setStartTime();
                                mMenu.Peek.WorkerState = mMenu.MenuState.VOID;
                                MenuVoid();
                            }
                            else
                            {
                                menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;
                            }
                            break;

                        case LoadMenuState.SAVETHREE:
                            if (loadSlots[2].str_Level != "EMPTY")
                            {
                                mMenu.Peek.LoadSlot = 3;
                                mFile.Peek.LoadGame(mMenu.Peek.LoadSlot);
                                menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;
                                mFile.Peek.setStartTime();
                                mMenu.Peek.WorkerState = mMenu.MenuState.VOID;
                                MenuVoid();
                            }
                            else
                            {
                                menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;
                            }
                            break;

                        case LoadMenuState.SAVEFOUR:
                            if (loadSlots[3].str_Level != "EMPTY")
                            {
                                mMenu.Peek.LoadSlot = 4;
                                mFile.Peek.LoadGame(mMenu.Peek.LoadSlot);
                                menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;
                                mFile.Peek.setStartTime();
                                mMenu.Peek.WorkerState = mMenu.MenuState.VOID;
                                MenuVoid();
                            }
                            else
                            {
                                menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;
                            }
                            break;

                        default:
                            break;
                    }

                    isChanging = false;
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

            for (int i = 0; i < int_NumOfOptions - 1; i++)
            {
                spriteBatch.Draw(menuItems[i].t2d_Texture, new Rectangle((int)menuItems[i].v2_Position.X, (int)menuItems[i].v2_Position.Y, (int)menuItems[i].v2_Size.X, (int)menuItems[i].v2_Size.Y), Color.White);
            }

            for (int i = 0; i < 4; i++)
            {
                spriteBatch.DrawString(sf_Font, loadSlots[i].str_Level, Engine.TempVector2(menuItems[i].v2_Position.X + 15, menuItems[i].v2_Position.Y - 150), Color.White);
                spriteBatch.DrawString(sf_Font, loadSlots[i].str_Time, Engine.TempVector2(menuItems[i].v2_Position.X + 15, menuItems[i].v2_Position.Y - 100), Color.White);
            }

            spriteBatch.Draw(menuItems[4].t2d_Texture, new Rectangle((int)menuItems[4].v2_Position.X, (int)menuItems[4].v2_Position.Y, (int)menuItems[4].v2_Size.X, (int)menuItems[4].v2_Size.Y), Color.White);

            mGraphics.Peek.ToggleSpriteDraw();
        }
    }
}
