using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Inkwell.Framework
{
    /// <summary>
    /// (struct) Communicator link to grabbing a slot on the live debugger and displaying information 
    /// (DebugMessage NAME = DebugMessage.Initialize(StringBuilder); Method to Assign it to the system)
    /// </summary>
    struct DebugMessage
    {
        private int Key;
        private bool Disposed;
        /*##############STATIC TEMPS###################*/
        static mDebug.DebugData _TempData;
        static DebugMessage _TempMsg = new DebugMessage();
        public static DebugMessage Initialize(bool Locked, Color Color, bool Persistent, string strInitialText)
        {
            _TempData = new mDebug.DebugData(); //<-- Prevents Static Linking, garbage should be nonexistant as space has been allocated for a full Temp
            _TempData.Text = strInitialText;
            _TempData.Color = Color;
            _TempData.Display = true;
            _TempData.Disposed = false;
            _TempData.Locked = Locked;
            _TempData.Persistent = Persistent;
            _TempData.Position = Vector2.Zero;

            _TempMsg.Key = mDebug.AddMessage(_TempData);
            _TempMsg.Disposed = false;
            return _TempMsg;
        }
        public string Text
        {
            get { if (!Disposed) return mDebug.DebugContainer[Key].Text; else return "Object is Disposed!"; }
            set { if (!Disposed) mDebug.SetText(value, Key); }
        }
        public Vector2 Position
        {
            get { if (!Disposed) return mDebug.DebugContainer[Key].Position; else return Vector2.Zero; }
            set { if(!Disposed) mDebug.DebugContainer[Key].Position = value; }
        }
        public Color Color
        {
            get { if (!Disposed) return mDebug.DebugContainer[Key].Color; else return Color.Red; }
            set { if (!Disposed) mDebug.DebugContainer[Key].Color = value; }
        }
        public void Remove()
        {
            if (!Disposed)
            {
                mDebug.Remove(Key);
                Disposed = true;
            }
        }
    }
    /// <summary>(Static Class) A Visual Debugger used to quickly get information on the screen; utilizes stringbuilders however garbage still occurs need to find out why!</summary>
    static class mDebug
    {    
        /// <summary>(class) Data for the Debugger it overall dosn't do anything outside of the debugger rather it is used internally.</summary>
        public class DebugData
        {
            public string Text;
            public bool Display;
            public Color Color;
            public bool Disposed;
            public Vector2 Position;
            public bool Locked;
            public bool Persistent;
        }
        /*****************************************HEADER*****************************************/
        public static Vector2 BarPosition = Vector2.Zero;
        public static List<DebugData> DebugContainer;
        public static int HeightOffset = 2;
        private static Texture2D _t2dBackground;
        private static FillMode _PreviousFillMode;
        public static Texture2D GrayTexture
        {
            get { return _t2dBackground; }
        }
        private static SpriteFont _spfDebug;
        private static Rectangle _BarBounds;
        private static int _iLockedMessages, _iUnlockedMessages;
        /*Built-in Debug Information*/
        private static DebugMessage _FPS, _InitialWorkingSet, _WorkingSet, _GarbageHeap, _RunTime, _UpdateTime, _DrawTime;
        private static long _frameCount = 0;
        private static int _iFramesThisSecond = 0;
        private static float _UpdateDelay = 1000.0f, _fpsDelay = 1000.0f;
        private static float _Timer = 0.0f, _fpsTimer = 0.0f;
        /*Precision Timer Outside of XNA*/
        public static Stopwatch Stopwatch;
        private static Stopwatch _swWatchUpdate, _swWatchDraw;
        /****************************************************************************************/
        /****************************************FUNCTIONS***************************************/
        #region Public Functions
        public static void Initialize(GraphicsDevice Device, ContentManager Content, string strFontAsset, Vector2 v2Position)
        {
            _t2dBackground = new Texture2D(Device, 1, 1);
            Color[] _tempColor = new Color[1];
            _tempColor[0] = new Color(0.0f, 0.0f, 0.0f, 0.5f);
            _t2dBackground.SetData(_tempColor);

            DebugContainer = new List<DebugData>();

            BarPosition = v2Position;

            _spfDebug = Content.Load<SpriteFont>(strFontAsset);

            _BarBounds = new Rectangle(0, 0, 0, 0);
            /*Init the Built in Information*/
            _FPS = DebugMessage.Initialize(true, Color.White, true, "FPS: Calculating...");

            _InitialWorkingSet = DebugMessage.Initialize(true, Color.Green, true, ("Initial Working Set: ") + (Environment.WorkingSet / 1024) + ("KB"));

            _WorkingSet = DebugMessage.Initialize(true, Color.White, true, ("Working Set: ") + (Environment.WorkingSet / 1024) + ("KB"));
            _WorkingSet.Color = Color.Yellow;

            _GarbageHeap = DebugMessage.Initialize(true, Color.White, true, ("Garbage Heap: ") + (System.GC.GetTotalMemory(false) / 1024) + ("KB"));
            _GarbageHeap.Color = Color.Red;

            _RunTime = DebugMessage.Initialize(true, Color.White, true, ("Run Time: Calculating..."));

            _UpdateTime = DebugMessage.Initialize(true, Color.White, true, ("Update Time: Calculating..."));

            _DrawTime = DebugMessage.Initialize(true, Color.White, true, ("Draw Time: Calculating..."));

            /*Init Threaded Timer*/
            Stopwatch = new Stopwatch();
            _swWatchUpdate = new Stopwatch(); _swWatchDraw = new Stopwatch();
        }
        public static void BeginUpdateProbe()
        {
            _swWatchUpdate.Reset();
            _swWatchUpdate.Start();
        }
        public static void EndUpdateProbe()
        {
            _swWatchUpdate.Stop();
            _UpdateTime.Text = ("Update Time: ") + _swWatchUpdate.Elapsed.TotalMilliseconds.ToString() + ("ms");
        }
        public static void Update(GameTime gameTime)
        {
            _Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_Timer >= _UpdateDelay)
            {
                _WorkingSet.Text = ("Working Set: ") + (Environment.WorkingSet / 1024) + ("KB");

                _GarbageHeap.Text = ("Garbage Heap: ") + (System.GC.GetTotalMemory(false) / 1024) + ("KB");
                _Timer = 0.0f;
            }
            /*Update our Debug Time for the entire client runtime*/
            _RunTime.Text = ("Run Time: ") + (gameTime.TotalGameTime.Days) + ("D:") + (gameTime.TotalGameTime.Hours) + ("H:") + (gameTime.TotalGameTime.Minutes)
                + ("M:") + (gameTime.TotalGameTime.Seconds) + ("S");
        }
        public static void BeginDrawProbe()
        {
            _iFramesThisSecond++; //<-- Placing here to Prevent Overhead on FPS measurements
            _frameCount++;
            _swWatchDraw.Reset();
            _swWatchDraw.Start();
        }
        public static void EndDrawProbe()
        {
            _swWatchDraw.Stop();
            _DrawTime.Text = ("Draw Time: ") + _swWatchDraw.Elapsed.TotalMilliseconds.ToString() + ("ms");
        }
        public static void Draw(SpriteBatch SpriteBatch, GameTime gameTime, Matrix SpriteScaleMatrix)
        {
            _PreviousFillMode = SpriteBatch.GraphicsDevice.RenderState.FillMode;
            SpriteBatch.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
            /*Before Drawing Check our FPS*/
            _fpsTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_fpsTimer >= _fpsDelay)
            {
                /*Old Deprecated Method Fraps will report this correctly however only with FixedTimeStep Disabled*/
                //_dFPS = (1000.0f / gameTime.ElapsedGameTime.TotalMilliseconds);
                //_dFPS = Math.Round(_dFPS, 0);
                //_FPS.Text = ("FPS: ") + (_dFPS);
                _FPS.Text = "FPS: " + _iFramesThisSecond;
                if (_iFramesThisSecond >= 60)
                    _FPS.Color = Color.Green;
                else
                    if (_iFramesThisSecond <= 45 && _iFramesThisSecond > 30)
                        _FPS.Color = Color.Yellow;
                    else
                        if (_iFramesThisSecond <= 30)
                            _FPS.Color = Color.Red;
                _fpsTimer = 0.0f;
                _iFramesThisSecond = 0;
            }
            //SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, SpriteScaleMatrix);
            mGraphics.Peek.ToggleSpriteDraw();
            SpriteBatch.Draw(_t2dBackground, _BarBounds, Color.White);
            for (int i = 0; i < DebugContainer.Count; i++)
            {
                SpriteBatch.DrawString(_spfDebug, DebugContainer[i].Text, DebugContainer[i].Position + BarPosition, DebugContainer[i].Color);
            }
            mGraphics.Peek.ToggleSpriteDraw();
            SpriteBatch.GraphicsDevice.RenderState.FillMode = _PreviousFillMode;
        }
        public static int AddMessage(DebugData Msg)
        {
            DebugContainer.Add(Msg);
            CleanLockedDebugMessages();
            return DebugContainer.Count - 1;
        }
        public static void SetText(string Text, int MsgKey)
        {
            if (DebugContainer[MsgKey].Text != Text)
            {
                DebugContainer[MsgKey].Text = Text;

                if(DebugContainer[MsgKey].Locked)
                if (_spfDebug.MeasureString(Text).X > _BarBounds.Width)
                    _BarBounds.Width = (int)_spfDebug.MeasureString(Text.ToString()).X;
            }
        }
        public static void Remove(int MsgKey)
        {
            DebugContainer.RemoveAt(MsgKey);
        }
        public static void CleanUp()
        {
            for (int i = DebugContainer.Count - 1; i >= 0; i--)
            {
                if (DebugContainer[i].Disposed || !DebugContainer[i].Persistent)
                    DebugContainer.RemoveAt(i);
            }
            CleanLockedDebugMessages();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
        #endregion
        /****************************************************************************************/
        #region Private Functions
        private static void CleanLockedDebugMessages()
        {
            int Height = (int)_spfDebug.MeasureString(DebugContainer[0].Text).Y - HeightOffset;
            _iLockedMessages = 0;
            _iUnlockedMessages = 0;
            for (int i = 0; i < DebugContainer.Count; i++)
            {
                if (DebugContainer[i].Locked)
                {
                    DebugContainer[i].Position.Y = _iLockedMessages * Height;
                    _BarBounds.Height = (int)DebugContainer[i].Position.Y + Height;
                    if (_BarBounds.Width < ((int)_spfDebug.MeasureString(DebugContainer[i].Text).X))
                        _BarBounds.Width = (int)_spfDebug.MeasureString(DebugContainer[i].Text).X;
                    _iLockedMessages++;
                }
                else _iUnlockedMessages++;
            }
        }
        #endregion
    }
}
