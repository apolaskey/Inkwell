using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
/*Andrew Polaskey - 12.10.2010*/
namespace Inkwell.Framework
{
    /****************************************************************************************/
    public class DebugMessage
    {
        private int _ID = -1; //Hot little hack for nullable ints
        public DebugMessage()
        {
            if (_ID == -1)
                _ID = LiveDebug.Peek.CreateMessageID();
            LiveDebug.Peek.SetPosition(_ID, new Vector2(0.0f, (_ID * 20.0f)));
        }
        public String Text
        {
            get { return LiveDebug.Peek.ReturnMessageText(_ID); }
            set 
            { 
                if(LiveDebug.Peek.ReturnMessageText(_ID) != value)
                LiveDebug.Peek.SetMessageText(_ID, value);
            }
        }
        public Color Color
        {
            get { return LiveDebug.Peek.ReturnTextColor(_ID); }
            set { LiveDebug.Peek.SetTextColor(_ID, value); }
        }
        public Vector2 Position
        {
            get { return LiveDebug.Peek.GetPosition(_ID); }
            set { LiveDebug.Peek.SetPosition(_ID, value); }
        }
    }
    /****************************************************************************************/
    public class MessageData
    {
        public String Text = "NULL";
        public Color Color = Color.White;
        public Vector2 Position = Vector2.Zero;
        public bool Display = true;
        public bool Disposed = false;
    }
}