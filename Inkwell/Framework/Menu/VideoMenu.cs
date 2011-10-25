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
    class VideoMenu: MenuScreen
    {
        struct menuItem
        {
            public Texture2D t2d_Texture;
            public Texture2D t2d_Texture_Original;
            public Texture2D t2d_Texture_Highlight;
            public Texture2D t2d_Texture_Selected;
            public Texture2D t2d_Texture_Selected_Original;
            public Texture2D t2d_Texture_Gray;
            public Vector2 v2_Position;
            public Vector2 v2_Size;
        }

        public enum VideoMenuState
        {
            RESOLUTION,
            FULLSCREEN,
            POST,
            BACK,
            VOID
        }

        public enum ResolutionOptions
        {
            ZERO,
            ONE,
            TWO,
            THREE,
            FOUR,
            FIVE,
            SIX
        }

        private menuItem menuBackground;
        private menuItem[] menuItems;
        private menuItem[] resItems;
        private const int int_NumOfOptions = 4;
        private const int int_NumOfResOptions = 7;
        private bool isBusy = false;
        private bool isChanging = false;

        public VideoMenuState CurrentWorkerState;
        public VideoMenuState PreviousWorkerState;

        public ResolutionOptions CurrentResState;
        public ResolutionOptions PreviousResState;

        public override void Initialize()
        {
            menuBackground = new menuItem();

            menuItems = new menuItem[int_NumOfOptions];
            for (int i = 0; i < int_NumOfOptions; i++)
            {
                menuItems[i] = new menuItem();
            }

            resItems = new menuItem[int_NumOfResOptions];
            for (int i = 0; i < int_NumOfResOptions; i++)
            {
                resItems[i] = new menuItem();
            }

            CurrentWorkerState = VideoMenuState.BACK;
            CurrentResState = ResolutionOptions.ZERO;
        }

        public void ChangeDefaults()
        {
            for (int i = 0; i < 4; i++)
            {
                if (i == (int)CurrentResState)
                {
                    resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected_Original;
                    resItems[i].v2_Position = Engine.TempVector2(resItems[0].v2_Position.X, resItems[0].v2_Position.Y + (50 * i));
                }
                else
                {
                    resItems[i].t2d_Texture = resItems[i].t2d_Texture_Gray;
                    resItems[i].v2_Position = Engine.TempVector2(resItems[0].v2_Position.X, resItems[0].v2_Position.Y + (50 * i));
                }
            }

            for (int i = 4; i < int_NumOfResOptions; i++)
            {
                if (i == (int)CurrentResState)
                {
                    resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected_Original;
                    resItems[i].v2_Position = Engine.TempVector2(resItems[0].v2_Position.X + 175, resItems[0].v2_Position.Y + (50 * (i - 4)));
                }
                else
                {
                    resItems[i].t2d_Texture = resItems[i].t2d_Texture_Gray;
                    resItems[i].v2_Position = Engine.TempVector2(resItems[0].v2_Position.X + 175, resItems[0].v2_Position.Y + (50 * (i - 4)));
                }
            }

            if (mGraphics.Peek.IsFullScreen)
            {
                menuItems[1].t2d_Texture = menuItems[1].t2d_Texture_Selected_Original;
            }

            if (mGraphics.Peek.PostProcessing)
            {
                menuItems[2].t2d_Texture = menuItems[2].t2d_Texture_Selected_Original;
            }
        }

        public override void Load(ContentManager content)
        {
            menuBackground.t2d_Texture = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Background\\Video_Background");
            menuBackground.v2_Position = Vector2.Zero;
            menuBackground.v2_Size = Engine.TempVector2(1280, 720);


            #region Textures

            menuItems[0].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_Resolution_Normal");
            menuItems[0].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_Resolution_Select");
            menuItems[0].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_Resolution_Press");
          
            menuItems[1].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_Fullscreen_Normal");
            menuItems[1].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_Fullscreen_Select");
            menuItems[1].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_Fullscreen_Press");
            menuItems[1].t2d_Texture_Selected_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_Fullscreen_Pressed");
            
            menuItems[2].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_Post Processing_Normal");
            menuItems[2].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_Post Processing_Select");
            menuItems[2].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_Post Processing_Press");
            menuItems[2].t2d_Texture_Selected_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_Post Processing_Pressed");
            
            menuItems[3].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Normal");
            menuItems[3].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Select");
            menuItems[3].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Button_Back_Press");

            resItems[0].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_800x600_Normal");
            resItems[0].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_800x600_Select");
            resItems[0].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_800x600_Press");
            resItems[0].t2d_Texture_Selected_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_800x600_Pressed");
            resItems[0].t2d_Texture_Gray = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_800x600_NotActive");

            resItems[1].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1024x768_Normal");
            resItems[1].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1024x768_Select");
            resItems[1].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1024x768_Press");
            resItems[1].t2d_Texture_Selected_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1024x768_Pressed");
            resItems[1].t2d_Texture_Gray = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1024x768_NotActive");

            resItems[2].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1280x720_Normal");
            resItems[2].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1280x720_Select");
            resItems[2].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1280x720_Press");
            resItems[2].t2d_Texture_Selected_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1280x720_Pressed");
            resItems[2].t2d_Texture_Gray = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1280x720_NotActive");

            resItems[3].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1366x768_Normal");
            resItems[3].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1366x768_Select");
            resItems[3].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1366x768_Press");
            resItems[3].t2d_Texture_Selected_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1366x768_Pressed");
            resItems[3].t2d_Texture_Gray = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1366x768_NotActive");

            resItems[4].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1440x900_Normal");
            resItems[4].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1440x900_Select");
            resItems[4].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1440x900_Press");
            resItems[4].t2d_Texture_Selected_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1440x900_Pressed");
            resItems[4].t2d_Texture_Gray = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1440x900_NotActive");

            resItems[5].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1680x1050_Normal");
            resItems[5].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1680x1050_Select");
            resItems[5].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1680x1050_Press");
            resItems[5].t2d_Texture_Selected_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1680x1050_Pressed");
            resItems[5].t2d_Texture_Gray = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1680x1050_NotActive");

            resItems[6].t2d_Texture_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1920x1080_Normal");
            resItems[6].t2d_Texture_Highlight = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1920x1080_Select");
            resItems[6].t2d_Texture_Selected = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1920x1080_Press");
            resItems[6].t2d_Texture_Selected_Original = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1920x1080_Pressed");
            resItems[6].t2d_Texture_Gray = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Video\\Buttons\\Video_Button_1920x1080_NotActive");

            #endregion

            #region Sizes

            menuItems[0].v2_Size = Engine.TempVector2(191, 40);
            menuItems[1].v2_Size = Engine.TempVector2(236, 40);
            menuItems[2].v2_Size = Engine.TempVector2(308, 40);
            menuItems[3].v2_Size = Engine.TempVector2(175, 39);

            resItems[0].v2_Size = Engine.TempVector2(174, 38);
            resItems[1].v2_Size = Engine.TempVector2(174, 38);
            resItems[2].v2_Size = Engine.TempVector2(174, 38);
            resItems[3].v2_Size = Engine.TempVector2(174, 38);
            resItems[4].v2_Size = Engine.TempVector2(173, 38);
            resItems[5].v2_Size = Engine.TempVector2(173, 38);
            resItems[6].v2_Size = Engine.TempVector2(173, 38);

            #endregion

            menuItems[0].t2d_Texture = menuItems[0].t2d_Texture_Highlight;
            menuItems[0].v2_Position = Engine.TempVector2(550 - menuItems[0].v2_Size.X, 325);

            for (int i = 1; i < int_NumOfOptions - 1; i++)
            {
                menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Original;
                menuItems[i].v2_Position = Engine.TempVector2(550 - menuItems[i].v2_Size.X, menuItems[0].v2_Position.Y + (75 * i));
            }

            menuItems[3].t2d_Texture = menuItems[3].t2d_Texture_Original;
            menuItems[3].v2_Position = Engine.TempVector2(550, 580);

            resItems[0].t2d_Texture = resItems[0].t2d_Texture_Selected_Original;
            resItems[0].v2_Position = Engine.TempVector2(700, 325);

            for (int i = 1; i < 4; i++)
            {
                resItems[i].t2d_Texture = resItems[i].t2d_Texture_Gray;
                resItems[i].v2_Position = Engine.TempVector2(resItems[0].v2_Position.X, resItems[0].v2_Position.Y + (50 * i));
            }

            for (int i = 4; i < int_NumOfResOptions; i++)
            {
                resItems[i].t2d_Texture = resItems[i].t2d_Texture_Gray;
                resItems[i].v2_Position = Engine.TempVector2(resItems[0].v2_Position.X + 175, resItems[0].v2_Position.Y + (50 * (i - 4)));
            }
        }

        /// <summary>
        /// Goes to the "previous" option
        /// </summary>
        private void CycleBack()
        {
            if (CurrentWorkerState == VideoMenuState.RESOLUTION)
            {
                CurrentWorkerState = VideoMenuState.BACK;
            }
            else
            {
                CurrentWorkerState--;
            }

            for (int i = 0; i < int_NumOfOptions; i++)
            {
                if (i == (int)CurrentWorkerState)
                {
                    if (i == 1 && mGraphics.Peek.IsFullScreen)
                    {
                        menuItems[1].t2d_Texture = menuItems[1].t2d_Texture_Selected;
                    }
                    else if (i == 2 && mGraphics.Peek.PostProcessing)
                    {
                        menuItems[2].t2d_Texture = menuItems[2].t2d_Texture_Selected;
                    }
                    else
                    {
                        menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Highlight;
                    }
                }

                else if (i == 1 && mGraphics.Peek.IsFullScreen)
                {
                    menuItems[1].t2d_Texture = menuItems[1].t2d_Texture_Selected_Original;
                }
                else if (i == 2 && mGraphics.Peek.PostProcessing)
                {
                    menuItems[2].t2d_Texture = menuItems[2].t2d_Texture_Selected_Original;
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
            if (CurrentWorkerState == VideoMenuState.BACK)
            {
                CurrentWorkerState = VideoMenuState.RESOLUTION;
            }
            else
            {
                CurrentWorkerState++;
            }

            for (int i = 0; i < int_NumOfOptions; i++)
            {
                if (i == (int)CurrentWorkerState)
                {
                    if (i == 1 && mGraphics.Peek.IsFullScreen)
                    {
                        menuItems[1].t2d_Texture = menuItems[1].t2d_Texture_Selected;
                    }
                    else if (i == 2 && mGraphics.Peek.PostProcessing)
                    {
                        menuItems[2].t2d_Texture = menuItems[2].t2d_Texture_Selected;
                    }
                    else
                    {
                        menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Highlight;
                    }
                }
                else if (i == 1 && mGraphics.Peek.IsFullScreen)
                {
                    menuItems[1].t2d_Texture = menuItems[1].t2d_Texture_Selected_Original;
                }
                else if (i == 2 && mGraphics.Peek.PostProcessing)
                {
                    menuItems[2].t2d_Texture = menuItems[2].t2d_Texture_Selected_Original;
                }
                else
                {
                    menuItems[i].t2d_Texture = menuItems[i].t2d_Texture_Original;
                }
            }
        }

        /// <summary>
        /// Goes to the "previous" resolution option
        /// </summary>
        private void CycleResBack()
        {
            if (CurrentResState == ResolutionOptions.ZERO)
            {
                CurrentResState = ResolutionOptions.SIX;
            }
            else
            {
                CurrentResState--;
            }

            for (int i = 0; i < int_NumOfResOptions; i++)
            {
                if (i == (int)CurrentResState)
                {
                    resItems[i].t2d_Texture = resItems[i].t2d_Texture_Highlight;
                }
                else
                {
                    if (i == (int)PreviousResState)
                    {
                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                    }
                    else
                    {
                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Original;
                    }
                }
            }
        }

        /// <summary>
        /// Goes to the "next" resolution option
        /// </summary>
        private void CycleResForward()
        {
            if (CurrentResState == ResolutionOptions.SIX)
            {
                CurrentResState = ResolutionOptions.ZERO;
            }
            else
            {
                CurrentResState++;
            }

            for (int i = 0; i < int_NumOfResOptions; i++)
            {
                if (i == (int)CurrentResState)
                {
                    resItems[i].t2d_Texture = resItems[i].t2d_Texture_Highlight;
                }
                else
                {
                    if (i == (int)PreviousResState)
                    {
                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                    }
                    else
                    {
                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Original;
                    }
                }
            }
        }

        /// <summary>
        /// Puts the worker state on void
        /// </summary>
        public void MenuVoid()
        {
            PreviousWorkerState = CurrentWorkerState;
            CurrentWorkerState = VideoMenuState.VOID;
        }

        public override void Update()
        {
            if (CurrentWorkerState != VideoMenuState.VOID)
            {
                if (isBusy)
                {
                    if (mInput.Peek.IsKeyPressed(Keys.W) || mInput.Peek.IsKeyPressed(Keys.Up))
                    {
                        CycleResBack();
                    }

                    if (mInput.Peek.IsKeyPressed(Keys.S) || mInput.Peek.IsKeyPressed(Keys.Down))
                    {
                        CycleResForward();
                    }

                    #region CycleLeftRight

                    #region CycleLeft

                    if (mInput.Peek.IsKeyPressed(Keys.A) || mInput.Peek.IsKeyPressed(Keys.Left))
                    {
                        switch (CurrentResState)
                        {
                            case ResolutionOptions.FOUR:
                                CurrentResState = ResolutionOptions.ZERO;
                                for (int i = 0; i < int_NumOfResOptions; i++)
                                {
                                    if (i == (int)CurrentResState)
                                    {
                                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Highlight;
                                    }
                                    else
                                    {
                                        if (i == (int)PreviousResState)
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                                        }
                                        else
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Original;
                                        }
                                    }
                                }
                                break;

                            case ResolutionOptions.FIVE:
                                CurrentResState = ResolutionOptions.ONE;
                                for (int i = 0; i < int_NumOfResOptions; i++)
                                {
                                    if (i == (int)CurrentResState)
                                    {
                                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Highlight;
                                    }
                                    else
                                    {
                                        if (i == (int)PreviousResState)
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                                        }
                                        else
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Original;
                                        }
                                    }
                                }
                                break;

                            case ResolutionOptions.SIX:
                                CurrentResState = ResolutionOptions.TWO;
                                for (int i = 0; i < int_NumOfResOptions; i++)
                                {
                                    if (i == (int)CurrentResState)
                                    {
                                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Highlight;
                                    }
                                    else
                                    {
                                        if (i == (int)PreviousResState)
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                                        }
                                        else
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Original;
                                        }
                                    }
                                }
                                break;
                        }

                        for (int i = 0; i < int_NumOfResOptions; i++)
                        {
                            if (i == (int)CurrentResState)
                            {
                                resItems[i].t2d_Texture = resItems[i].t2d_Texture_Highlight;
                            }
                            else
                            {
                                if (i == (int)PreviousResState)
                                {
                                    resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                                }
                                else
                                {
                                    resItems[i].t2d_Texture = resItems[i].t2d_Texture_Original;
                                }
                            }
                        }
                    }

                    #endregion

                    #region CycleRight

                    if (mInput.Peek.IsKeyPressed(Keys.D) || mInput.Peek.IsKeyPressed(Keys.Right))
                    {
                        switch (CurrentResState)
                        {
                            case ResolutionOptions.ZERO:
                                CurrentResState = ResolutionOptions.FOUR;
                                for (int i = 0; i < int_NumOfResOptions; i++)
                                {
                                    if (i == (int)CurrentResState)
                                    {
                                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Highlight;
                                    }
                                    else
                                    {
                                        if (i == (int)PreviousResState)
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                                        }
                                        else
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Gray;
                                        }
                                    }
                                }
                                break;

                            case ResolutionOptions.ONE:
                                CurrentResState = ResolutionOptions.FIVE;
                                for (int i = 0; i < int_NumOfResOptions; i++)
                                {
                                    if (i == (int)CurrentResState)
                                    {
                                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Highlight;
                                    }
                                    else
                                    {
                                        if (i == (int)PreviousResState)
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                                        }
                                        else
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Original;
                                        }
                                    }
                                }
                                break;

                            case ResolutionOptions.TWO:
                                CurrentResState = ResolutionOptions.SIX;
                                for (int i = 0; i < int_NumOfResOptions; i++)
                                {
                                    if (i == (int)CurrentResState)
                                    {
                                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Highlight;
                                    }
                                    else
                                    {
                                        if (i == (int)PreviousResState)
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                                        }
                                        else
                                        {
                                            resItems[i].t2d_Texture = resItems[i].t2d_Texture_Original;
                                        }
                                    }
                                }
                                break;
                        }

                        for (int i = 0; i < int_NumOfResOptions; i++)
                        {
                            if (i == (int)CurrentResState)
                            {
                                resItems[i].t2d_Texture = resItems[i].t2d_Texture_Highlight;
                            }
                            else
                            {
                                if (i == (int)PreviousResState)
                                {
                                    resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                                }
                                else
                                {
                                    resItems[i].t2d_Texture = resItems[i].t2d_Texture_Original;
                                }
                            }
                        }
                    }

                    #endregion

                    #endregion

                    if (mInput.Peek.IsKeyPressed(Keys.Enter))
                    {
                        switch (CurrentResState)
                        {
                            case ResolutionOptions.ZERO:
                                mGraphics.Peek.BackBufferResolution = Engine.TempVector2(800, 600);
                                break;

                            case ResolutionOptions.ONE:
                                mGraphics.Peek.BackBufferResolution = Engine.TempVector2(1024, 768);
                                break;

                            case ResolutionOptions.TWO:
                                mGraphics.Peek.BackBufferResolution = Engine.TempVector2(1280, 720);
                                break;

                            case ResolutionOptions.THREE:
                                mGraphics.Peek.BackBufferResolution = Engine.TempVector2(1366, 768);
                                break;

                            case ResolutionOptions.FOUR:
                                mGraphics.Peek.BackBufferResolution = Engine.TempVector2(1440, 900);
                                break;

                            case ResolutionOptions.FIVE:
                                mGraphics.Peek.BackBufferResolution = Engine.TempVector2(1680, 1050);
                                break;

                            case ResolutionOptions.SIX:
                                mGraphics.Peek.BackBufferResolution = Engine.TempVector2(1950, 1080);
                                break;
                        }
                        for (int i = 0; i < int_NumOfResOptions; i++)
                        {
                            if (i == (int)CurrentResState)
                            {
                                resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                            }

                            else
                            {
                                resItems[i].t2d_Texture = resItems[i].t2d_Texture_Gray;
                            }
                        }
                        menuItems[0].t2d_Texture = menuItems[0].t2d_Texture_Highlight;
                        isBusy = false;
                    }

                }

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
                            case VideoMenuState.RESOLUTION:
                                PreviousResState = CurrentResState;
                                //menuItems[0].t2d_Texture = menuItems[0].t2d_Texture_Selected;
                                for (int i = 0; i < int_NumOfResOptions; i++)
                                {
                                    if (i == (int)CurrentResState)
                                    {
                                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Selected;
                                    }
                                    else
                                    {
                                        resItems[i].t2d_Texture = resItems[i].t2d_Texture_Original;
                                    }
                                }
                                isBusy = true;
                                break;

                            case VideoMenuState.FULLSCREEN:
                                if (mGraphics.Peek.IsFullScreen)
                                {
                                    mGraphics.Peek.IsFullScreen = false;
                                    mGraphics.Peek.Graphics.ApplyChanges();
                                    menuItems[1].t2d_Texture = menuItems[1].t2d_Texture_Highlight;
                                }
                                else
                                {
                                    mGraphics.Peek.IsFullScreen = true;
                                    mGraphics.Peek.Graphics.ApplyChanges();
                                    menuItems[1].t2d_Texture = menuItems[1].t2d_Texture_Selected;
                                }
                                break;

                            case VideoMenuState.POST:
                                if (mGraphics.Peek.PostProcessing)
                                {
                                    mGraphics.Peek.PostProcessing = false;
                                    menuItems[2].t2d_Texture = menuItems[2].t2d_Texture_Highlight;
                                }
                                else
                                {
                                    mGraphics.Peek.PostProcessing = true;
                                    menuItems[2].t2d_Texture = menuItems[2].t2d_Texture_Selected;
                                }
                                break;

                            case VideoMenuState.BACK:
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

            spriteBatch.Draw(menuBackground.t2d_Texture, new Rectangle((int) menuBackground.v2_Position.X, (int) menuBackground.v2_Position.Y, (int) menuBackground.v2_Size.X, (int) menuBackground.v2_Size.Y), Color.White);

            for (int i = 0; i < int_NumOfOptions; i++)
            {
                spriteBatch.Draw(menuItems[i].t2d_Texture, new Rectangle((int)menuItems[i].v2_Position.X, (int)menuItems[i].v2_Position.Y, (int)menuItems[i].v2_Size.X, (int) menuItems[i].v2_Size.Y), Color.White);
            }

            for (int i = 0; i < int_NumOfResOptions; i++)
            {
                spriteBatch.Draw(resItems[i].t2d_Texture, new Rectangle((int)resItems[i].v2_Position.X, (int)resItems[i].v2_Position.Y, (int)resItems[i].v2_Size.X, (int)resItems[i].v2_Size.Y), Color.White);
            }

            mGraphics.Peek.ToggleSpriteDraw();
        }
    }
}
