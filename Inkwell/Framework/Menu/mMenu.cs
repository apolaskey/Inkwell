using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Inkwell.Framework
{
    /// <summary>
    /// (Singleton) In charge of drawing the menu screen during appropriate times; will most likely need hooks
    /// in the core to function correctly.
    /// </summary>
    public sealed class mMenu
    {
        private static volatile mMenu _instance;
        private static object _padlock = new Object();

        LogoMenu LogoScreen = new LogoMenu();
        MainMenu MainMenuScreen = new MainMenu();
        LoadMenu LoadMenuScreen = new LoadMenu();
        OptionsMenu OptionsMenuScreen = new OptionsMenu();
        AudioMenu AudioMenuScreen = new AudioMenu();
        VideoMenu VideoMenuScreen = new VideoMenu();
        ControlsMenu ControlsMenuScreen = new ControlsMenu();
        PauseMenu PauseMenuScreen = new PauseMenu();
        PauseOptionsMenu PauseOptionsMenuScreen = new PauseOptionsMenu();
        GameOverMenu GameOverMenuScreen = new GameOverMenu();
        AreYouSureMenu AreYouSureScreen = new AreYouSureMenu();
        NewGameMenu NewGameScreen = new NewGameMenu();
        DeleteSaveMenu DeleteSaveScreen = new DeleteSaveMenu();
        UI UIScreen = new UI();

        public enum MenuState
        {
            LOGO,
            MAIN,
            LOAD,
            OPTIONS,
            AUDIO,
            VIDEO,
            CONTROLS,
            PAUSE,
            PAUSEOPTIONS,
            GAMEOVER,
            AREYOUSURE,
            NEWGAME,
            DELETESAVE,
            UI,
            EXIT,
            VOID
        }

        MenuState MenuWorkerState = MenuState.LOGO;
        MenuState PreviousWorkerState = MenuState.LOGO;
        MenuState TempState = MenuState.LOGO;

        /////////////////////////  Options Variables  /////////////////////////////

        //bool isKeyboardDefault = true;
        //bool isGlow = false;
        //bool isDepthOfField = false;
        int intSoundVolume = 100;
        int intMusicVolume = 100;
        int intLoadSlot = 0;

        #region Accessor

        public MenuState WorkerState
        {
            get
            {
                return this.MenuWorkerState;
            }
            set
            {
                switch (value)
                {
                    case MenuState.MAIN:
                        NewGameScreen.LoadSlots();
                        MainMenuScreen.CurrentWorkerState = MainMenuScreen.PreviousWorkerState;
                        break;
                        
                    case MenuState.LOAD:
                        LoadMenuScreen.LoadSlots();
                        LoadMenuScreen.CurrentWorkerState = LoadMenuScreen.PreviousWorkerState;
                        break;

                    case MenuState.OPTIONS:
                        OptionsMenuScreen.CurrentWorkerState = OptionsMenuScreen.PreviousWorkerState;
                        break;

                    case MenuState.AUDIO:
                        AudioMenuScreen.CurrentWorkerState = AudioMenuScreen.PreviousWorkerState;
                        break;

                    case MenuState.VIDEO:
                        VideoMenuScreen.CurrentWorkerState = VideoMenuScreen.PreviousWorkerState;
                        break;

                    case MenuState.CONTROLS:
                        ControlsMenuScreen.CurrentWorkerState = ControlsMenuScreen.PreviousWorkerState;
                        break;

                    case MenuState.PAUSE:
                        PauseMenuScreen.CurrentWorkerState = PauseMenuScreen.PreviousWorkerState;
                        break;

                    case MenuState.PAUSEOPTIONS:
                        PauseOptionsMenuScreen.CurrentWorkerState = PauseOptionsMenuScreen.PreviousWorkerState;
                        break;

                    case MenuState.GAMEOVER:
                        GameOverMenuScreen.CurrentWorkerState = GameOverMenuScreen.PreviousWorkerState;
                        break;

                    case MenuState.AREYOUSURE:
                        AreYouSureScreen.CurrentWorkerState = AreYouSureScreen.PreviousWorkerState;
                        break;

                    case MenuState.NEWGAME:
                        NewGameScreen.CurrentWorkerState = NewGameScreen.PreviousWorkerState;
                        break;

                    case MenuState.DELETESAVE:
                        DeleteSaveScreen.CurrentWorkerState = DeleteSaveScreen.PreviousWorkerState;
                        break;

                    default:
                        break;

                }

                PreviousWorkerState = MenuWorkerState;
                this.MenuWorkerState = value;
            }
        }

        //public bool isKeysDefault
        //{
        //    get
        //    {
        //        return this.isKeyboardDefault;
        //    }

        //    set
        //    {
        //        this.isKeyboardDefault = value;
        //    }
        //}

        //public bool Glow
        //{
        //    get
        //    {
        //        return this.isGlow;
        //    }

        //    set
        //    {
        //        this.isGlow = value;
        //    }
        //}

        //public bool DepthOfField
        //{
        //    get
        //    {
        //        return this.isDepthOfField;
        //    }

        //    set
        //    {
        //        this.isDepthOfField = value;
        //    }
        //}

        public int SoundVolume
        {
            get
            {
                return this.intSoundVolume;
            }

            set
            {
                this.intSoundVolume = value;
            }
        }

        public int MusicVolume
        {
            get
            {
                return this.intMusicVolume;
            }

            set
            {
                this.intMusicVolume = value;
            }
        }

        public int LoadSlot
        {
            get
            {
                return this.intLoadSlot;
            }

            set
            {
                this.intLoadSlot = value;
            }
        }

        #endregion

        /// <summary>
        /// Use this to pause the game
        /// </summary>
        public void Pause()
        {
            PauseMenuScreen.isUpdating = false;
            PauseMenuScreen.CurrentWorkerState = PauseMenuScreen.PreviousWorkerState;
            MenuWorkerState = MenuState.PAUSE;
        }

        /// <summary>
        /// Use this only for the "back" of Audio and Video menus
        /// </summary>
        public void GoToPreviousState()
        {
            TempState = MenuWorkerState;
            MenuWorkerState = PreviousWorkerState;
            PreviousWorkerState = TempState;

            switch (MenuWorkerState)
            {
                case MenuState.MAIN:
                    MainMenuScreen.CurrentWorkerState = MainMenuScreen.PreviousWorkerState;
                    break;

                case MenuState.LOAD:
                    LoadMenuScreen.CurrentWorkerState = LoadMenuScreen.PreviousWorkerState;
                    break;

                case MenuState.OPTIONS:
                    OptionsMenuScreen.CurrentWorkerState = OptionsMenuScreen.PreviousWorkerState;
                    break;

                case MenuState.AUDIO:
                    AudioMenuScreen.CurrentWorkerState = AudioMenuScreen.PreviousWorkerState;
                    break;

                case MenuState.VIDEO:
                    VideoMenuScreen.CurrentWorkerState = VideoMenuScreen.PreviousWorkerState;
                    break;

                case MenuState.PAUSE:
                    PauseMenuScreen.CurrentWorkerState = PauseMenuScreen.PreviousWorkerState;
                    break;

                case MenuState.PAUSEOPTIONS:
                    PauseOptionsMenuScreen.CurrentWorkerState = PauseOptionsMenuScreen.PreviousWorkerState;
                    break;

                case MenuState.GAMEOVER:
                    GameOverMenuScreen.CurrentWorkerState = GameOverMenuScreen.PreviousWorkerState;
                    break;

                case MenuState.AREYOUSURE:
                    AreYouSureScreen.CurrentWorkerState = AreYouSureScreen.PreviousWorkerState;
                    break;

                case MenuState.NEWGAME:
                    NewGameScreen.CurrentWorkerState = NewGameScreen.PreviousWorkerState;
                    break;

                case MenuState.DELETESAVE:
                    DeleteSaveScreen.CurrentWorkerState = DeleteSaveScreen.PreviousWorkerState;
                    break;

                default:
                    break;
            }
        }

        public void Initialize()
        {
            LogoScreen.Initialize();
            MainMenuScreen.Initialize();
            LoadMenuScreen.Initialize();
            OptionsMenuScreen.Initialize();
            AudioMenuScreen.Initialize();
            VideoMenuScreen.Initialize();
            ControlsMenuScreen.Initialize();
            PauseMenuScreen.Initialize();
            PauseOptionsMenuScreen.Initialize();
            GameOverMenuScreen.Initialize();
            AreYouSureScreen.Initialize();
            NewGameScreen.Initialize();
            DeleteSaveScreen.Initialize();
            UIScreen.Initialize();
        }

        public void Load(ContentManager content)
        {
            LogoScreen.Load(content);
            MainMenuScreen.Load(content);
            LoadMenuScreen.Load(content);
            OptionsMenuScreen.Load(content);
            AudioMenuScreen.Load(content);
            VideoMenuScreen.Load(content);
            ControlsMenuScreen.Load(content);
            PauseMenuScreen.Load(content);
            PauseOptionsMenuScreen.Load(content);
            GameOverMenuScreen.Load(content);
            AreYouSureScreen.Load(content);
            NewGameScreen.Load(content);
            DeleteSaveScreen.Load(content);
            UIScreen.Load(content);
            mFile.Peek.LoadOptions(); 
        }

        /// <summary>
        /// Use this to set the defaults in the Options menu
        /// </summary>
        /// <param name="Resolution"></param>
        public void SettingDefaults(string Resolution)
        {
            switch(Resolution)
            {
                case "800 600":
                    VideoMenuScreen.CurrentResState = VideoMenu.ResolutionOptions.ZERO;
                    break;
                case "1024 768":
                    VideoMenuScreen.CurrentResState = VideoMenu.ResolutionOptions.ONE;
                    break;
                case "1280 720":
                    VideoMenuScreen.CurrentResState = VideoMenu.ResolutionOptions.TWO;
                    break;
                case "1366 768":
                    VideoMenuScreen.CurrentResState = VideoMenu.ResolutionOptions.THREE;
                    break;
                case "1440 900":
                    VideoMenuScreen.CurrentResState = VideoMenu.ResolutionOptions.FOUR;
                    break;
                case "1680 1050":
                    VideoMenuScreen.CurrentResState = VideoMenu.ResolutionOptions.FIVE;
                    break;
                case "1920 1080":
                    VideoMenuScreen.CurrentResState = VideoMenu.ResolutionOptions.SIX;
                    break;
            }

            for (int i = 0; i < 7; i++)
            {
                VideoMenuScreen.ChangeDefaults();
            }

        }

        public void Update()
        {
            if (MenuWorkerState != MenuState.VOID)
            {
                switch (MenuWorkerState)
                {
                    case MenuState.LOGO:
                        LogoScreen.Update();
                        break;

                    case MenuState.MAIN:
                        MainMenuScreen.Update();
                        break;

                    case MenuState.LOAD:
                        LoadMenuScreen.Update();
                        break;

                    case MenuState.OPTIONS:
                        OptionsMenuScreen.Update();
                        break;

                    case MenuState.AUDIO:
                        AudioMenuScreen.Update();
                        break;

                    case MenuState.VIDEO:
                        VideoMenuScreen.Update();
                        break;

                    case MenuState.CONTROLS:
                        ControlsMenuScreen.Update();
                        break;

                    case MenuState.PAUSE:
                        PauseMenuScreen.Update();
                        break;

                    case MenuState.PAUSEOPTIONS:
                        PauseOptionsMenuScreen.Update();
                        break;

                    case MenuState.GAMEOVER:
                        GameOverMenuScreen.Update();
                        break;

                    case MenuState.AREYOUSURE:
                        AreYouSureScreen.Update();
                        break;

                    case MenuState.NEWGAME:
                        NewGameScreen.Update();
                        break;

                    case MenuState.DELETESAVE:
                        DeleteSaveScreen.Update();
                        break;

                    case MenuState.UI:
                        UIScreen.Update();
                        break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch(MenuWorkerState)
            {
                case MenuState.LOGO:
                    LogoScreen.Draw(spriteBatch);
                    break;

                case MenuState.MAIN:
                    MainMenuScreen.Draw(spriteBatch);
                    break;

                case MenuState.LOAD:
                    LoadMenuScreen.Draw(spriteBatch);
                    break;

                case MenuState.OPTIONS:
                    OptionsMenuScreen.Draw(spriteBatch);
                    break;

                case MenuState.AUDIO:
                    AudioMenuScreen.Draw(spriteBatch);
                    break;

                case MenuState.VIDEO:
                    VideoMenuScreen.Draw(spriteBatch);
                    break;

                case MenuState.CONTROLS:
                    ControlsMenuScreen.Draw(spriteBatch);
                    break;

                case MenuState.PAUSE:
                    PauseMenuScreen.Draw(spriteBatch);
                    break;

                case MenuState.PAUSEOPTIONS:
                    PauseOptionsMenuScreen.Draw(spriteBatch);
                    break;

                case MenuState.GAMEOVER:
                    GameOverMenuScreen.Draw(spriteBatch);
                    break;

                case MenuState.AREYOUSURE:
                    AreYouSureScreen.Draw(spriteBatch);
                    break;

                case MenuState.NEWGAME:
                    NewGameScreen.Draw(spriteBatch);
                    break;

                case MenuState.DELETESAVE:
                    DeleteSaveScreen.Draw(spriteBatch);
                    break;

                case MenuState.UI:
                    UIScreen.Draw(spriteBatch);
                    break;
            }
        }

        public static mMenu Peek
        {
            get
            {
                /*Check to see if we already initialized our component*/
                if (_instance == null)
                {
                    /*Lock it so another thread cant check it*/
                    lock (_padlock)
                    {
                        /*Check one more time just to be extra careful*/
                        if (_instance == null)
                            _instance = new mMenu(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        //<EOC>
    }
}
