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
    class GameOverMenu: MenuScreen
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

        public enum GameOverMenuState
        {
            RETRY,
            MAINMENU,
            EXIT,
            VOID
        }

        private menuItem menuGray;
        private menuItem menuBackground;
        private menuItem[] menuItems;
        private const int int_NumOfOptions = 3;
        private bool isChanging = false;

        public GameOverMenuState CurrentWorkerState;
        public GameOverMenuState PreviousWorkerState;
        public bool isUpdating;

        public override void Initialize()
        {
            menuGray = new menuItem();
            menuBackground = new menuItem();

            menuItems = new menuItem[int_NumOfOptions];
            for (int i = 0; i < int_NumOfOptions; i++)
            {
               menuItems[i] = new menuItem();
            }

            CurrentWorkerState = GameOverMenuState.RETRY;
        }

        public override void Load(ContentManager content)
        {
            menuGray.t2d_Texture = mDebug.GrayTexture;
            menuGray.v2_Position = Vector2.Zero;
            menuGray.v2_Size = Engine.TempVector2(1280, 720);
            menuBackground.t2d_Texture = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Game Over\\Background\\Game Over_Background");
            menuBackground.v2_Position = Engine.TempVector2(640, 360);
            menuBackground.v2_Size = Engine.TempVector2(510, 549);

            #region Textures

            menuItems[0].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Game Over\\Buttons\\Game Over_Button_Retry_Normal");
            menuItems[0].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Game Over\\Buttons\\Game Over_Button_Retry_Select");
            menuItems[0].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Game Over\\Buttons\\Game Over_Button_Retry_Press");

            menuItems[1].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Game Over\\Buttons\\Game Over_Button_Main Menu_Normal");
            menuItems[1].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Game Over\\Buttons\\Game Over_Button_Main Menu_Select");
            menuItems[1].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Game Over\\Buttons\\Game Over_Button_Main Menu_Press");

            menuItems[2].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Game Over\\Buttons\\Game Over_Button_Quit_Normal");
            menuItems[2].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Game Over\\Buttons\\Game Over_Button_Quit_Select");
            menuItems[2].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Game Over\\Buttons\\Game Over_Button_Quit_Press");

            #endregion

            #region Sizes

            menuItems[0].v2_Size = Engine.TempVector2(281, 53);
            menuItems[1].v2_Size = Engine.TempVector2(281, 43);
            menuItems[2].v2_Size = Engine.TempVector2(281, 43);

            #endregion

            menuItems[0].t2d_Texture = menuItems[0].t2d_Texture_Highlight;
            menuItems[0].v2_Position = Engine.TempVector2(500, 310);

            for (int i = 1; i < int_NumOfOptions; i++)
            {
                menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Original;
                menuItems[i].v2_Position = Engine.TempVector2(menuItems[0].v2_Position.X, menuItems[0].v2_Position.Y + (75 * i));
            }
        }
        #region Cycling

        /// <summary>
        /// Goes to the "previous" option
        /// </summary>
        private void CycleBack()
        {
            if (CurrentWorkerState == GameOverMenuState.RETRY)
            {
                CurrentWorkerState = GameOverMenuState.EXIT;
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
            if (CurrentWorkerState == GameOverMenuState.EXIT)
            {
                CurrentWorkerState = GameOverMenuState.RETRY;
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

        #endregion

        /// <summary>
        /// Puts the worker state on void
        /// </summary>
        public void MenuVoid()
        {
            PreviousWorkerState = CurrentWorkerState;
            CurrentWorkerState = GameOverMenuState.VOID;
        }

        public override void Update()
        {
            if (CurrentWorkerState != GameOverMenuState.VOID)
            {
                if(mInput.Peek.IsKeyPressed(Keys.W) || mInput.Peek.IsKeyPressed(Keys.Up))
                {
                    CycleBack();
                }

                if (mInput.Peek.IsKeyPressed(Keys.S) || mInput.Peek.IsKeyPressed(Keys.Down))
                {
                    CycleForward();
                }


                if (mInput.Peek.IsKeyPressed(Keys.P) && isUpdating)
                {
                    isUpdating = false;
                    MenuVoid();
                    mMenu.Peek.WorkerState = mMenu.MenuState.VOID;
                }

                if (isChanging)
                {
                    switch (CurrentWorkerState)
                    {
                        case GameOverMenuState.RETRY:
                            mMenu.Peek.WorkerState = mMenu.MenuState.VOID;
                            mLevel.Peek.ReloadLevel();
                            mLevel.Peek.ChangeLevel();
                            mAvatar.Peek.HealthAdd(100);
                            //mAvatar.Peek.FullRestore();
                            break;

                        case GameOverMenuState.MAINMENU:
                            mMenu.Peek.WorkerState = mMenu.MenuState.AREYOUSURE;
                            break;

                        case GameOverMenuState.EXIT:
                            mMenu.Peek.WorkerState = mMenu.MenuState.EXIT;
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
            spriteBatch.Draw(menuGray.t2d_Texture, new Rectangle((int)menuGray.v2_Position.X, (int)menuGray.v2_Position.Y, (int)menuGray.v2_Size.X, (int)menuGray.v2_Size.Y), Color.White);
            spriteBatch.Draw(menuBackground.t2d_Texture, Engine.TempVector2(menuBackground.v2_Position.X, menuBackground.v2_Position.Y), null, Color.White, 0.0f, Engine.TempVector2(menuBackground.t2d_Texture.Width / 2, menuBackground.t2d_Texture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);// new Rectangle((int) menuBackground.v2_Position.X, (int) menuBackground.v2_Position.Y, (int) menuBackground.v2_Size.X, (int) menuBackground.v2_Size.Y), Color.White);

            for (int i = 0; i < int_NumOfOptions; i++)
            {
                spriteBatch.Draw(menuItems[i].t2d_Texture, new Rectangle((int)menuItems[i].v2_Position.X, (int)menuItems[i].v2_Position.Y, (int)menuItems[i].v2_Size.X, (int)menuItems[i].v2_Size.Y), Color.White);
            }

            mGraphics.Peek.ToggleSpriteDraw();
        }
    }
}
