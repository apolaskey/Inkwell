using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
/*Andrew Polaskey - 12.10.2010*/
namespace Inkwell.Framework
{
    /****************************************************************************************/
    public class DebugMessage
    {
        private int _ID = Engine.NULLED_INT; //Hot little hack for nullable ints
        public DebugMessage(bool Persistent)
        {
            if (_ID == Engine.NULLED_INT)
                _ID = mDebug.Peek.CreateMessageID();
            SetPosition(_ID, new Vector2(0.0f, (_ID * 20.0f)));
            mDebug.Peek._lstDebugText[_ID].Persistent = Persistent;
        }
        public DebugMessage(String strText, bool Persistent)
        {
            if (_ID == Engine.NULLED_INT)
                _ID = mDebug.Peek.CreateMessageID();
            SetPosition(_ID, new Vector2(0.0f, (_ID * 20.0f)));
            mDebug.Peek._lstDebugText[_ID].Persistent = Persistent;
            mDebug.Peek.SetMessageText(_ID, strText);
        }
        public String Text
        {
            get { return ReturnMessageText(_ID); }
            set 
            { 
                if(ReturnMessageText(_ID) != value)
                mDebug.Peek.SetMessageText(_ID, value);
            }
        }
        public Color Color
        {
            get { return ReturnTextColor(_ID); }
            set { SetTextColor(_ID, value); }
        }
        public Vector2 Position
        {
            get { return GetPosition(_ID); }
            set { SetPosition(_ID, value); }
        }
        public void Dispose()
        {
            mDebug.Peek._lstDebugText[_ID].Disposed = true;
            mDebug.Peek._lstDebugText[_ID].Display = false;
        }

        /****************************************************************************************/
        /// <summary>
        /// (String) Returns the Text being held inside of a specific ID in the LiveDebug Component. [DebugMessage Part]
        /// </summary>
        /// <param name="ID">Location in the List of the Text (Have DebugMessage Pull this information).</param>
        /// <returns>(String) Current Text held in the list.</returns>
        public String ReturnMessageText(int ID)
        {
               return mDebug.Peek._lstDebugText[ID].Text;
        }
        /****************************************************************************************/
        /// <summary>
        /// Return the current color value of the Text.
        /// </summary>
        /// <param name="ID">(int) ID our location in the list to edit.</param>
        /// <returns>(Color) Returns a Color value.</returns>
        public Color ReturnTextColor(int ID)
        {
            return mDebug.Peek._lstDebugText[ID].Color;
        }
        /****************************************************************************************/
        public void SetTextColor(int ID, Color color)
        {
            mDebug.Peek._lstDebugText[ID].Color = color;
        }
        /****************************************************************************************/
        public Vector2 GetPosition(int ID)
        {
            return mDebug.Peek._lstDebugText[ID].Position;
        }
        /****************************************************************************************/
        public void SetPosition(int ID, Vector2 Position)
        {
            mDebug.Peek._lstDebugText[ID].Position = Position;
        }
        /****************************************************************************************/
    }
    /****************************************************************************************/
    public class MessageData
    {
        public String Text = "NULL";
        public Color Color = Color.White;
        public Vector2 Position = Vector2.Zero;
        public bool Display = true;
        public bool Disposed = false;
        public bool Persistent = false;
    }
}