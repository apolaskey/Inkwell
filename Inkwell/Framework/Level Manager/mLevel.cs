using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Inkwell.Framework.Graphics.Data;
/*Andrew Polaskey - 12.13.2010*/
namespace Inkwell.Framework
{
    public sealed class mLevel
    {
        /****************************************************************************************/
        private static mLevel _Instance = null;
        private static readonly object _Pad = new object();
        /*****************************************HEADER*****************************************/
        public enum LevelState { Loading, Playing, Paused, CallingKill, Killing, None } //<-- Holds all of our states that a Level can be in
        private LevelState _PrevState;
        private LevelState _CurrentState = LevelState.None;
        private cLevel _CurrentLevel = null; //<-- Level Holder
        private cLevel _NextLevel = null; //<-- Level Temp Holder
        private bool _LevelInitialized = false;
        private bool _LevelOnceUpdated = false;
        private bool _LockLevel = false;
        Texture2D _t2dLoading;
        /****************************************FUNCTIONS***************************************/
        public void Initialize(ContentManager Content, String strLoadingTexture)
        {
            _t2dLoading = Content.Load<Texture2D>(strLoadingTexture);
        }
        /// <summary>
        /// Assign the Next Level to be Loaded (Try to Execute within the Level's Kill to Safe Memory).
        /// </summary>
        /// <param name="Level">(cLevel) Level to load</param>
        /// <param name="t2dNextLevelTexture">(Texture2D) Texture to use as the Loading Screen CAN BE NULL it will re-use previous texture.</param>
        public void QueueNextLevel(cLevel Level, Texture2D t2dNextLevelTexture)
        {
            if (t2dNextLevelTexture != null)
                _t2dLoading = t2dNextLevelTexture;
            _NextLevel = Level;
        }
        /// <summary>Internal Function to make state changes easier. [Modifies Previous State]</summary>
        /// <param name="State">(LevelState) The State to swap into.</param>
        private void ChangeState(LevelState State)
        {
            _PrevState = _CurrentState;
            _CurrentState = State;
        }
        /// <summary>Determine whether the level has been intialized and is safe to work with.</summary>
        /// <returns>(Bool) Whether the Level has been initialized or not.</returns>
        public bool IsLevelInitialized()
        {
            return _LevelInitialized;
        }/// <summary>
        /// Pauses the level
        /// </summary>
        public void PauseGame()
        {
            ChangeState(LevelState.Paused);
        }
        /// <summary>Update the Level Management System.</summary>
        public void Update()
        {
            switch (_CurrentState)
            {
                case LevelState.Loading:
                    _CurrentLevel = _NextLevel; //<-- Set the Next Level as the Current Level
                    _CurrentLevel.Initialize();
                    //try
                    //{
                    //    _CurrentLevel.Initialize(); //<-- Call the Initialize function on the Current Level
                    //}
                    //catch(Exception e)
                    //{
                    //    mDebug.Peek.MessagePrompt("Failed to Queue Next Level!" + "\r\n" + e.Message + "\r\n" + e.StackTrace, "Failure!");
                    //}
                    _NextLevel = null; //<-- Null out the Data on Next Level
                    _LevelInitialized = true; //<-- Flag as Initialized
                    ChangeState(LevelState.Playing); //Swap States we are ready for play.
                    break;
                case LevelState.Killing:
                    if (_LevelInitialized) //<-- Determine if we even have data to kill.
                    {
                        if (!_LockLevel)
                        {
                            mMenu.Peek.WorkerState = mMenu.MenuState.VOID;
                            _CurrentLevel.Kill(); //<-- Call the Current Level's Kill Command so it can perform it's internal cleanup.
                            _CurrentLevel = null; //<-- Null out the Current Level.
                        }
                        _LevelInitialized = false; //<-- Flag Initialized for false to ensure safety.
                        _LevelOnceUpdated = false;
                    }
                    ChangeState(LevelState.Loading); //<-- Swap out states
                    break;
                case LevelState.Playing:
                    if (_LevelInitialized) //<-- Check to see if it's safe to Update the Level
                    {
                        mMenu.Peek.WorkerState = mMenu.MenuState.UI;
                        _LevelOnceUpdated = true;
                        _CurrentLevel.Update(); //<-- Call the Level's Update Logic.
                    }
                    break;
                case LevelState.Paused:
                    break;
            }
        }
        /// <summary>Draw our Level by detecting the current level.</summary>
        public void Draw()
        {
            if (_LevelInitialized && _LevelOnceUpdated && _CurrentState != LevelState.CallingKill)
                _CurrentLevel.Draw();
            else
            {
                mGraphics.Peek.ToggleSpriteDraw();
                mGraphics.Peek.SpriteBatch.Draw(_t2dLoading, Vector2.Zero, null, Color.White);
                mGraphics.Peek.ToggleSpriteDraw();
                if (_CurrentState == LevelState.CallingKill)
                    _CurrentState = LevelState.Killing;
            }
        }

        /// <summary>Force a Change in the Level this automatically kill the old level and load the new one.</summary>
        /// <param name="NextLevel">(Abstracted Level) A level to load.</param>
        public void ChangeLevel()
        {
            mAnimation.Peek.heroFrameSpeed.Clear();
            ChangeState(LevelState.CallingKill);
        }
        /// <summary>Force a Change in the Level this automatically kill the old level and load the new one.</summary>
        /// <param name="NextLevel">(Abstracted Level) A level to load.</param>
        public void ChangeLevel(cLevel NextLevel)
        {
            _NextLevel = NextLevel;
            ChangeState(LevelState.CallingKill);
        }
        public void ReloadLevel()
        {
            _NextLevel = _CurrentLevel;
            ChangeState(LevelState.CallingKill);
        }
        /****************************************************************************************/
        public static mLevel Peek
        {
            get
            {
                lock (_Pad)
                {
                    if (_Instance == null)
                        _Instance = new mLevel();
                    return _Instance;
                }
            }
        }
        /******************************************EOF*******************************************/
    }
}
