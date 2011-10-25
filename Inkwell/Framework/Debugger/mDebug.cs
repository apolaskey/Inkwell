using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace Inkwell.Framework
{
    /*Andrew Polaskey - 12.10.2010*/
    public sealed class mDebug
    {
        /****************************************************************************************/
        private static mDebug _Instance = null;
        private static readonly object _Pad = new object();
        /*****************************************HEADER*****************************************/
        [DllImport("user32.dll", CharSet = CharSet.Auto)] //Used for MessageBox
        private static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type); //The actual function in the DLL
        public List<MessageData> _lstDebugText;
        private SpriteFont _sprDebugFont;
        private Stopwatch _sWatch; //Used to track FPS
        private bool _bDisplay = true; //Display the debug component?
        private Texture2D _t2dBackground;
        private float _fBackgroundWidth;
        private Rectangle _rBackground;
        /****************************************************************************************/
        /*Var's to Determine FPS*/
        private int _iCurrentFPS;
        private int _iPreviousFPS;
        private int _iFrameCount;
        private double _dTimer;
        private DebugMessage _dmFPS;
        /****************************************************************************************/
        /*Var's to determine Memory Allocation*/
        private DebugMessage _dmGarbage;
        private DebugMessage _dmWorkingSet;
        private int _iGarbageCollection;
        private int _iWorkingSet;
        /*Used to Enable Debug Overlay with Different Fill States*/
        bool bWasWireFramed;
        FillMode SavedFillMode;
        public Texture2D GrayTexture
        {
            get { return this._t2dBackground; }
        }
        /****************************************FUNCTIONS***************************************/
        /// <summary>
        /// (Void) Initialize the LiveDebug Component with a valid SpriteFont.
        /// </summary>
        /// <param name="Content">(ContentManager) Active Content Pool</param>
        /// <param name="sprFontAssetLocation">(SpriteFont) Location to the SpriteFont resource to use.</param>
        public void Initialize(ContentManager Content, String sprFontAssetLocation)
        {
            _iCurrentFPS = 0;
            _iPreviousFPS = 0;
            _iFrameCount = 0;
            _dTimer = 0;
            _iGarbageCollection = 0;
            _iWorkingSet = 0;
            _fBackgroundWidth = 0;
            bWasWireFramed = false;
            _lstDebugText = new List<MessageData>();
            _t2dBackground = new Texture2D(mGraphics.Peek.Device(), 1, 1);
            _rBackground = new Rectangle();
            _sprDebugFont = Content.Load<SpriteFont>(sprFontAssetLocation);
            _dmFPS = new DebugMessage("FPS: Calculating...", true); //Initialize our Debug Message for FPS
            _dmGarbage = new DebugMessage("Garbage Heap: Calculating...", true);
            _dmWorkingSet = new DebugMessage("Working Set: Calculating...", true);
            _sWatch = new Stopwatch();
            _sWatch = Stopwatch.StartNew();
            Color[] temp = new Color[1];
            temp[0] = new Color(0, 0, 0, 128);
            _t2dBackground.SetData(temp);
            temp = null;
        }
        /****************************************************************************************/
        public void Update()
        {
            if (Engine.DebugEnabled)
            {
                /****************************************************************************************/
                /*Begin our Update*/
                _sWatch.Stop();
                _dTimer += _sWatch.Elapsed.TotalMilliseconds; //Used for FPS
                if (mInput.Peek.IsKeyPressed(Keys.F11))
                    mModel.Peek.DrawBoundingBoxes = mModel.Peek.DrawBoundingBoxes ? (false) : (true);
                if (mInput.Peek.IsKeyPressed(Keys.F5))
                    mGraphics.Peek.FillMode(FillMode.Solid);
                if (mInput.Peek.IsKeyPressed(Keys.F6))
                    mGraphics.Peek.FillMode(FillMode.WireFrame);
                if (mInput.Peek.IsKeyPressed(Keys.F7))
                    mGraphics.Peek.FillMode(FillMode.Point);
                /****************************************************************************************/
                /*End our Update*/
                _sWatch.Reset();
                _sWatch.Start();
                /****************************************************************************************/
            }
        }
        /****************************************************************************************/
        /// <summary>
        /// (Void) Draw out all of the Debug Text.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Engine.DebugEnabled)
            {
                bWasWireFramed = false;
                SavedFillMode = mGraphics.Peek.Device().RenderState.FillMode;
                if (mGraphics.Peek.Device().RenderState.FillMode != FillMode.Solid)
                {
                    mGraphics.Peek.FillMode(FillMode.Solid);
                    bWasWireFramed = true;
                }
                //for (int i = _lstDebugText.Count - 1; i >= 0; i--) //For Counting Backwords
                if (_bDisplay)
                {
                    mGraphics.Peek.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
                    mGraphics.Peek.SpriteBatch.Draw(_t2dBackground, _rBackground, Color.White);
                    for (int i = 0; i < _lstDebugText.Count; i++)
                    {
                        if(_lstDebugText[i].Display)
                            mGraphics.Peek.SpriteBatch.DrawString(_sprDebugFont, _lstDebugText[i].Text, _lstDebugText[i].Position, _lstDebugText[i].Color);
                    }
                    mGraphics.Peek.SpriteBatch.End();
                }
                /****************************************************************************************/
                /*Determine Framerate & Calculate other useful things*/
                if (_dTimer >= 1000)
                {
                    _iPreviousFPS = _iCurrentFPS;
                    _iCurrentFPS = _iFrameCount;
                    Engine.TextBuilder.Remove(0, Engine.TextBuilder.Length);
                    Engine.TextBuilder.Append("FPS: ");
                    Engine.TextBuilder.Append(_iCurrentFPS.ToString());
                    _dmFPS.Text = _iCurrentFPS == _iPreviousFPS ? (_dmFPS.Text) : (Engine.TextBuilder.ToString());
                    if (_dmFPS.Text != "FPS: Calculating...")
                    {
                        _iGarbageCollection = ((int)System.GC.GetTotalMemory(false) / 1024);
                        _iWorkingSet = ((int)Environment.WorkingSet / 1024);
                        /*Garbage Heap*/
                        Engine.ClearTextBuilder(); 
                        Engine.TextBuilder.Append("Garbage Heap: ");
                        Engine.TextBuilder.Append(_iGarbageCollection.ToString());
                        Engine.TextBuilder.Append("KB");
                        _dmGarbage.Text = Engine.TextBuilder.ToString();
                        /*Working Set*/
                        Engine.ClearTextBuilder();
                        Engine.TextBuilder.Append("Client Size: ");
                        Engine.TextBuilder.Append(_iWorkingSet.ToString());
                        Engine.TextBuilder.Append("KB");
                        _dmWorkingSet.Text = Engine.TextBuilder.ToString();
                    }
                    _iFrameCount = 0;
                    _dTimer = 0;
                }
                else
                    _iFrameCount++;
                /****************************************************************************************/
                if(bWasWireFramed)
                mGraphics.Peek.FillMode(SavedFillMode);
            }
        }
        /****************************************************************************************/
        /// <summary>
        /// (Int) Return a valid and free ID on the Debug Text List.
        /// </summary>
        public int CreateMessageID()
        {
            int iTemp = 0; //To store our ID
            bool bListFull = true; //Used for further checking to ensure our list needs to grow
            for (int i = 0; i < _lstDebugText.Count; i++) //Search our list for a disposed message
            {
                if (_lstDebugText[i].Disposed)
                {
                    iTemp = i;
                    bListFull = false;
                    _lstDebugText[i].Display = true;
                    _lstDebugText[i].Disposed = false;
                    break; //We found a disposed member lets not waste anymore time here
                }
            }

            if (bListFull)//If we looked through our list and haven't found a disposed message lets increase our list
            {
                MessageData dummy = new MessageData();
                _lstDebugText.Add(dummy);
                iTemp = _lstDebugText.Count - 1;
            }
            //_rBackground = new Rectangle(0, 0, 225, _lstDebugText.Count * 20);
            _rBackground.Height = _lstDebugText.Count * 20;
            return iTemp;

        }
        /****************************************************************************************/
        /// <summary>
        /// Display A MessageBox with an error (this (shouldnt') won't crash the application).
        /// </summary>
        /// <param name="Text">Text within the box</param>
        /// <param name="Title">Text on the title</param>
        public void MessagePrompt(String Text, String Title)
        {
            MessageBox(new IntPtr(0), Text, Title, 0);
        }
        /****************************************************************************************/
        /// <summary>
        /// (Void) Set the message text at the specific location of the LiveDebug List.
        /// </summary>
        /// <param name="ID">Location in the List of the Text (Have DebugMessage Pull this information).</param>
        /// <param name="strText">(String) Set Text held in the list.</param>
        public void SetMessageText(int ID, String strText)
        {
            if (Engine.DebugEnabled)
            {
                if (strText != _lstDebugText[ID].Text)
                {
                    _lstDebugText[ID].Text = strText;
                    if (_sprDebugFont.MeasureString(strText).X > _fBackgroundWidth)
                    {
                        _fBackgroundWidth = _sprDebugFont.MeasureString(strText).X;
                        _rBackground = new Rectangle(0, 0, (int)_fBackgroundWidth, _lstDebugText.Count * 20);
                    }
                }
            }
        }
        public void CleanUp()
        {
            for (int i = _lstDebugText.Count - 1; i > 0; i--)
            {
                if (!_lstDebugText[i].Persistent)
                    _lstDebugText.RemoveAt(i);
            }
            _rBackground = new Rectangle(0, 0, (int)_fBackgroundWidth, _lstDebugText.Count * 20);
        }
        /****************************************************************************************/
        public static mDebug Peek
        {
            get
            {
                lock (_Pad)
                {
                    if (_Instance == null)
                        _Instance = new mDebug();
                    return _Instance;
                }
            }
        }
        /******************************************EOF*******************************************/
    }
}
