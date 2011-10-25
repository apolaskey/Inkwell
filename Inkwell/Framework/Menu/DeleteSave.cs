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
    class DeleteSaveMenu: MenuScreen
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

        public enum DeleteSaveMenuState
        {
            YES,
            NO,
            VOID
        }

        private menuItem menuBackground;
        private menuItem[] menuItems;
        private const int int_NumOfOptions = 2;
        private bool isChanging = false;

        public DeleteSaveMenuState CurrentWorkerState;
        public DeleteSaveMenuState PreviousWorkerState;
        public bool isUpdating;

        public override void Initialize()
        {
            menuBackground = new menuItem();

            menuItems = new menuItem[int_NumOfOptions];
            for (int i = 0; i < int_NumOfOptions; i++)
            {
               menuItems[i] = new menuItem();
            }

            CurrentWorkerState = DeleteSaveMenuState.NO;
            PreviousWorkerState = DeleteSaveMenuState.NO;
            isUpdating = false;
        }

        public override void Load(ContentManager content)
        {
            menuBackground.t2d_Texture = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\DeleteSave\\Background\\Overwrite Warning Background");
            menuBackground.v2_Position = Engine.TempVector2(640, 360);
            menuBackground.v2_Size = Engine.TempVector2(1280, 720);

            #region Textures

            menuItems[0].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\AreYouSure\\Buttons\\AreYouSure_Button_Yes_Normal");
            menuItems[0].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\AreYouSure\\Buttons\\AreYouSure_Button_Yes_Select");
            menuItems[0].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\AreYouSure\\Buttons\\AreYouSure_Button_Yes_Press");

            menuItems[1].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\AreYouSure\\Buttons\\AreYouSure_Button_No_Normal");
            menuItems[1].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\AreYouSure\\Buttons\\AreYouSure_Button_No_Select");
            menuItems[1].t2d_Texture_Press = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\AreYouSure\\Buttons\\AreYouSure_Button_No_Press");
            
            #endregion

            #region Sizes

            menuItems[0].v2_Size = Engine.TempVector2(105, 43);
            menuItems[1].v2_Size = Engine.TempVector2(105, 43);

            #endregion

            menuItems[0].t2d_Texture = menuItems[0].t2d_Texture_Original;
            menuItems[0].v2_Position = Engine.TempVector2(570, 470);

            menuItems[1].t2d_Texture = menuItems[1].t2d_Texture_Highlight;
            menuItems[1].v2_Position = Engine.TempVector2(570, 520);
   
        }
        #region Cycling

        /// <summary>
        /// Goes to the "previous" option
        /// </summary>
        private void CycleBack()
        {
            if (CurrentWorkerState == DeleteSaveMenuState.YES)
            {
                CurrentWorkerState = DeleteSaveMenuState.NO;
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
            if (CurrentWorkerState == DeleteSaveMenuState.NO)
            {
                CurrentWorkerState = DeleteSaveMenuState.YES;
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
            CurrentWorkerState = DeleteSaveMenuState.VOID;
        }

        public override void Update()
        {
            if (CurrentWorkerState != DeleteSaveMenuState.VOID)
            {
                if(mInput.Peek.IsKeyPressed(Keys.W) || mInput.Peek.IsKeyPressed(Keys.Up))
                {
                    CycleBack();
                }

                if (mInput.Peek.IsKeyPressed(Keys.S) || mInput.Peek.IsKeyPressed(Keys.Down))
                {
                    CycleForward();
                }

                if (isChanging)
                {
                    switch (CurrentWorkerState)
                    {
                        case DeleteSaveMenuState.YES:
                            //mFile.Peek.ClearSave(mMenu.Peek.LoadSlot);
                            mLevel.Peek.QueueNextLevel(new AliceLevel1(), null);
                            mLevel.Peek.ChangeLevel();
                            menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;
                            mFile.Peek.setStartTime();
                            mMenu.Peek.WorkerState = mMenu.MenuState.VOID;
                            MenuVoid();
                           break;

                        case DeleteSaveMenuState.NO:
                            mMenu.Peek.WorkerState = mMenu.MenuState.NEWGAME;
                            menuItems[(int)CurrentWorkerState].t2d_Texture = menuItems[(int)CurrentWorkerState].t2d_Texture_Highlight;
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

                isUpdating = true;
 
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            mGraphics.Peek.ToggleSpriteDraw();
            spriteBatch.Draw(menuBackground.t2d_Texture, Engine.TempVector2(menuBackground.v2_Position.X, menuBackground.v2_Position.Y), null, Color.White, 0.0f, Engine.TempVector2(menuBackground.t2d_Texture.Width / 2, menuBackground.t2d_Texture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);// new Rectangle((int) menuBackground.v2_Position.X, (int) menuBackground.v2_Position.Y, (int) menuBackground.v2_Size.X, (int) menuBackground.v2_Size.Y), Color.White);

            for (int i = 0; i < int_NumOfOptions; i++)
            {
                spriteBatch.Draw(menuItems[i].t2d_Texture, new Rectangle((int)menuItems[i].v2_Position.X, (int)menuItems[i].v2_Position.Y, (int)menuItems[i].v2_Size.X, (int)menuItems[i].v2_Size.Y), Color.White);
            }

            mGraphics.Peek.ToggleSpriteDraw();
        }
    }
}
