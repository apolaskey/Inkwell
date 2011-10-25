using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Inkwell.Framework
{
    public enum CameraType { Free, Fixed }
    public abstract class Camera
    {
        public Vector3 Position = Vector3.Zero;
        public Matrix Projection = Matrix.Identity;
        public Matrix View = Matrix.Identity;
        public Vector3 Angle;
        public float AspectRatio;
        public CameraType Type;
    }
}
