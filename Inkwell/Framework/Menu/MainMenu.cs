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
    class MainMenu: MenuScreen
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

        public enum MainMenuState
        {
            NEWGAME,
            LOADGAME,
            OPTIONS,
            EXIT,
            VOID
        }

        private menuItem menuBackground;
        private menuItem[] menuItems;
        private const int int_NumOfOptions = 4;
        private bool isChanging = false;

        public MainMenuState CurrentWorkerState;
        public MainMenuState PreviousWorkerState;

        public override void Initialize()
        {
            menuBackground = new menuItem();

            menuItems = new menuItem[int_NumOfOptions];
            for (int i = 0; i < int_NumOfOptions; i++)
            {
               menuItems[i] = new menuItem();
            }

            CurrentWorkerState = MainMenuState.NEWGAME;
        }

        public override void Load(ContentManager content)
        {
            menuBackground.t2d_Texture = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Background\\Main Menu_Background");
            menuBackground.v2_Position = Vector2.Zero;
            menuBackground.v2_Size = Engine.TempVector2(1280, 720);

            #region Textures

            menuItems[0].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_New Game_Normal");
            menuItems[0].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_New Game_Select");
            menuItems[0].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_New Game_Press");
 
            menuItems[1].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_Load Game_Normal");
            menuItems[1].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_Load Game_Select");
            menuItems[1].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_Load Game_Press");
      
            menuItems[2].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_Options_Normal");
            menuItems[2].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_Options_Select");
            menuItems[2].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_Options_Press");
       
            menuItems[3].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_Quit_Normal");
            menuItems[3].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_Quit_Select");
            menuItems[3].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Main Menu\\Buttons\\Main Menu_Button_Quit_Press");
            
            #endregion

            #region Sizes

            menuItems[0].v2_Size = Engine.TempVector2(279, 58);
            menuItems[1].v2_Size = Engine.TempVector2(291, 58);
            menuItems[2].v2_Size = Engine.TempVector2(299, 58);
            menuItems[3].v2_Size = Engine.TempVector2(174, 58);

            #endregion

            menuItems[0].t2d_Texture = menuItems[0].t2d_Texture_Highlight;
            menuItems[0].v2_Position = Engine.TempVector2(150, 450);

            for (int i = 1; i < int_NumOfOptions; i++)
            {
                menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Original;
                menuItems[i].v2_Position = Engine.TempVector2(menuItems[0].v2_Position.X + (260 * i), menuItems[0].v2_Position.Y);
            }
        }

        #region Cycling

        /// <summary>
        /// Goes to the "previous" option
        /// </summary>
        private void CycleBack()
        {
            if (CurrentWorkerState == MainMenuState.NEWGAME)
            {
                CurrentWorkerState = MainMenuState.EXIT;
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
            if (CurrentWorkerState == MainMenuState.EXIT)
            {
                CurrentWorkerState = MainMenuState.NEWGAME;
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
            CurrentWorkerState = MainMenuState.VOID;
        }

        public override void Update()
        {
            if (CurrentWorkerState != MainMenuState.VOID)
            {
                if(mInput.Peek.IsKeyPressed(Keys.A) || mInput.Peek.IsKeyPressed(Keys.Left))
                {
                    CycleBack();
                }

                if (mInput.Peek.IsKeyPressed(Keys.D) || mInput.Peek.IsKeyPressed(Keys.Right))
                {
                    CycleForward();
                }

                if (isChanging)
                {
                    switch (CurrentWorkerState)
                    {
                        case MainMenuState.NEWGAME:
                            mMenu.Peek.WorkerState = mMenu.MenuState.NEWGAME;
                            break;

                        case MainMenuState.LOADGAME:
                            mMenu.Peek.WorkerState = mMenu.MenuState.LOAD;
                            break;

                        case MainMenuState.OPTIONS:
                            mMenu.Peek.WorkerState = mMenu.MenuState.OPTIONS;
                            break;

                        case MainMenuState.EXIT:
                            mMenu.Peek.WorkerState = mMenu.MenuState.EXIT;
                            break;

                        default:
                            break;
                    }

                    menuItems[(int) CurrentWorkerState].t2d_Texture = menuItems[(int) CurrentWorkerState].t2d_Texture_Highlight;

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
