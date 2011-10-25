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
    public sealed class mCamera
    {
        //David Fahr, December 8, 2010.
        //Andrew Polaskey, February 2, 2011.
        /*****************************************HEADER*****************************************/
        #region CameraManager Singleton
        private static volatile mCamera _instance;
        private static object _padlock = new Object();
        public static mCamera Peek
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
                            _instance = new mCamera(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        #endregion
        Dictionary<string, Camera> _Cameras = new Dictionary<string, Camera>();
        FreeCamera _TempFreeCamera = new FreeCamera();
        FixedCamera _TempFixedCamera = new FixedCamera();
        BoundingFrustum _ViewBounds;
        /*Used for Quick Camera Toggle [TIP: F1 Key]*/
        string _CurrentCameraKey;
        /****************************************FUNCTIONS***************************************/ 
        //initialize of freecamera, set data passed from initialization in core
        public void Initialize(Vector3 Target)
        {
            _Cameras.Clear();
            FreeCamera.Initialize(_TempFreeCamera, Engine.TempVector3(0.0f, 10.0f, 0.0f), 5.0f, 1000.0f, mGraphics.Peek.AspectRatio());
            FixedCamera.Initialize(_TempFixedCamera, Vector3.Zero, 5.0f, 1000.0f, mGraphics.Peek.AspectRatio());
            FixedCamera.Update(_TempFixedCamera, Target);
            _ViewBounds = new BoundingFrustum(Matrix.Identity);
            _Cameras.Add("FreeCamera!", _TempFreeCamera);
            _Cameras.Add("FixedCamera!", _TempFixedCamera);
            _CurrentCameraKey = "FixedCamera!";
        }
        public void ResetCameras()
        {

        }
        public void ResizeCameras(float NearClip, float FarClip)
        {
            foreach(KeyValuePair<string, Camera> pair in _Cameras)
            {
                pair.Value.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), mGraphics.Peek.AspectRatio(), NearClip, FarClip);
            }
            
        }
        public mCamera()
        {
            FreeCamera.Initialize(_TempFreeCamera, Engine.TempVector3(0.0f, 10.0f, 0.0f), 5.0f, 1000.0f, mGraphics.Peek.AspectRatio());
            FixedCamera.Initialize(_TempFixedCamera, Vector3.Zero, 5.0f, 1000.0f, mGraphics.Peek.AspectRatio());
            _ViewBounds = new BoundingFrustum(Matrix.Identity);
            _Cameras.Add("FreeCamera!", _TempFreeCamera);
            _Cameras.Add("FixedCamera!", _TempFixedCamera);
            _CurrentCameraKey = "FixedCamera!";
        }
        public void AddCamera(CameraType Type, string CameraName, Vector3 Position, float NearClip, float FarClip)
        {
            switch (Type) //<-- Switch with an enumeration found up top to add in additional cameras
            {
                case CameraType.Free:
                    {
                        FreeCamera Dummy = new FreeCamera();
                        FreeCamera.Initialize(Dummy, Position, NearClip, FarClip, mGraphics.Peek.AspectRatio());
                        _Cameras.Add(CameraName, Dummy);
                    }
                    break;
                case CameraType.Fixed:
                    {
                        FixedCamera Dummy = new FixedCamera();
                        FixedCamera.Initialize(Dummy, Position, NearClip, FarClip, mGraphics.Peek.AspectRatio());
                        _Cameras.Add(CameraName, Dummy);
                    }
                    break;
            }
        }
        public void UseCamera(string CameraKey)
        {
            _CurrentCameraKey = CameraKey;
        }
        //update freecamera, fixedcamera, character position for fixedcamera
        public void Update(Vector3 FixedCameraTarget)
        {
            if (Engine.DebugEnabled)
            {
                if (mInput.Peek.IsKeyPressed(Keys.F1))
                {
                    if (_Cameras[_CurrentCameraKey].Type == CameraType.Fixed)
                        _CurrentCameraKey = "FreeCamera!";
                    else
                        _CurrentCameraKey = "FixedCamera!";
                }
            }
            switch (_Cameras[_CurrentCameraKey].Type)
            {
                case CameraType.Fixed:
                    {
                        FixedCamera.Update(_TempFixedCamera, FixedCameraTarget);
                        _Cameras[_CurrentCameraKey] = _TempFixedCamera;
                    }
                    break;
                case CameraType.Free:
                    {
                        FreeCamera.Update(_TempFreeCamera);
                        _Cameras[_CurrentCameraKey] = _TempFreeCamera;
                    }
                    break;
            }
            _ViewBounds.Matrix = _Cameras[_CurrentCameraKey].View * _Cameras[_CurrentCameraKey].Projection; 
        }

        //update freecamera, fixedcamera, character position for fixedcamera
        public void Update(Vector3 FixedCameraTarget, float CameraTilt)
        {
            if (Engine.DebugEnabled)
            {
                if (mInput.Peek.IsKeyPressed(Keys.F1))
                {
                    if (_Cameras[_CurrentCameraKey].Type == CameraType.Fixed)
                        _CurrentCameraKey = "FreeCamera!";
                    else
                        _CurrentCameraKey = "FixedCamera!";
                }
            }
            switch (_Cameras[_CurrentCameraKey].Type)
            {
                case CameraType.Fixed:
                    {
                        FixedCamera.Update(_TempFixedCamera, FixedCameraTarget, CameraTilt);
                        _Cameras[_CurrentCameraKey] = _TempFixedCamera;
                    }
                    break;
                case CameraType.Free:
                    {
                        FreeCamera.Update(_TempFreeCamera);
                        _Cameras[_CurrentCameraKey] = _TempFreeCamera;
                    }
                    break;
            }
            _ViewBounds.Matrix = _Cameras[_CurrentCameraKey].View * _Cameras[_CurrentCameraKey].Projection;
        }
        //update freecamera, fixedcamera, character position for fixedcamera
        public void Update(Vector3 FixedCameraTarget, int CameraDistance)
        {
            if (Engine.DebugEnabled)
            {
                if (mInput.Peek.IsKeyPressed(Keys.F1))
                {
                    if (_Cameras[_CurrentCameraKey].Type == CameraType.Fixed)
                        _CurrentCameraKey = "FreeCamera!";
                    else
                        _CurrentCameraKey = "FixedCamera!";
                }
            }
            switch (_Cameras[_CurrentCameraKey].Type)
            {
                case CameraType.Fixed:
                    {
                        FixedCamera.Update(_TempFixedCamera, FixedCameraTarget, CameraDistance);
                        _Cameras[_CurrentCameraKey] = _TempFixedCamera;
                    }
                    break;
                case CameraType.Free:
                    {
                        FreeCamera.Update(_TempFreeCamera);
                        _Cameras[_CurrentCameraKey] = _TempFreeCamera;
                    }
                    break;
            }
            _ViewBounds.Matrix = _Cameras[_CurrentCameraKey].View * _Cameras[_CurrentCameraKey].Projection;
        }
        //return current cam
        public Camera ReturnCamera()
        {
            return _Cameras[_CurrentCameraKey];
        }
        public BoundingFrustum ViewBounds()
        {
            return _ViewBounds;
        }
    }
}
