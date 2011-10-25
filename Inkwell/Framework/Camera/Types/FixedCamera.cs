using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Inkwell.Framework
{
    public class FixedCamera:Camera
    {
        /****************************************************************************************/
        public Vector3 lookAt;
        private int _DeltaScrollValue, _PreviousScrollValue, _CurrentScrollValue;
        private int CameraZoomClamp = 6000;
        DebugMessage ZoomAmount = DebugMessage.Initialize(true, Microsoft.Xna.Framework.Graphics.Color.White, true, "Camera Zoom: Calculating...");
        /****************************************************************************************/
        public static void Initialize(FixedCamera data, Vector3 camPos, float nearClip, float farClip, float AspectRatio)
        {
            data.Position = camPos;
            data.AspectRatio = AspectRatio;
            data.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), data.AspectRatio, nearClip, farClip);
            data.View = Matrix.CreateLookAt(data.Position, Vector3.Backward, Vector3.Up);
            data.Type = CameraType.Fixed;
        }
        /****************************************************************************************/
        public void Zoom()
        {
            _PreviousScrollValue = _CurrentScrollValue;
            _CurrentScrollValue = mInput.Peek.GetMouseState().ScrollWheelValue;
            _DeltaScrollValue = _PreviousScrollValue - _CurrentScrollValue;
            if (Engine.DebugEnabled)
                ZoomAmount.Text = "Camera Zoom: " + _DeltaScrollValue;
            if (_CurrentScrollValue >= 0)
                _CurrentScrollValue = 0;
            if (_CurrentScrollValue <= -(CameraZoomClamp))
                _CurrentScrollValue = -CameraZoomClamp;
        }
        public static void Update(FixedCamera data, Vector3 charPos)
        {
            //75.0f
            data.Zoom();
            data.Position = Engine.TempVector3(charPos.X, charPos.Y + 15.0f - ((data._CurrentScrollValue / 60)), charPos.Z + 80.0f - (data._CurrentScrollValue / 30));
            data.View = Matrix.CreateLookAt(data.Position, charPos, Vector3.Up);
        }
        public static void Update(FixedCamera data, Vector3 charPos, float Tilt)
        {
            //75.0f
            data.Zoom();
            data.Position = Engine.TempVector3(charPos.X, (charPos.Y + Tilt) - ((data._CurrentScrollValue / 60)), charPos.Z + 80.0f - (data._CurrentScrollValue / 30));
            data.View = Matrix.CreateLookAt(data.Position, charPos, Vector3.Up);
        }
        public static void Update(FixedCamera data, Vector3 charPos, int distance)
        {
            //75.0f
            data.Zoom();
            data.Position = Engine.TempVector3(charPos.X, (charPos.Y + 15.0f + distance / 3) - ((data._CurrentScrollValue / 60)), charPos.Z + 80.0f + distance - (data._CurrentScrollValue / 30));
            data.View = Matrix.CreateLookAt(data.Position, charPos, Vector3.Up);
        }
        /****************************************************************************************/
    }
}
