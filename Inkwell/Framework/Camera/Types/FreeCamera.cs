using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
/*Holds our Camera Information*/
namespace Inkwell.Framework
{
    /*Andrew Polaskey - 12.10.2010*/
    /// <summary>
    /// A free roaming camera without any form of bounds [Useful for Debugging and showing off][Working missing some features.]
    /// </summary>
    public class FreeCamera: Camera
    {
        /****************************************************************************************/
        public Vector2 v2LockMouse;
        public float Speed;
        public float TurnSpeed;
        //public int ScreenCenterX, ScreenCenterY;
        public bool RightClicked;
        private Vector2 OldPosition = Vector2.Zero;
        private Vector3 forward = new Vector3();
        private Vector3 left = new Vector3();
        /****************************************************************************************/
        public static void Initialize(FreeCamera data, Vector3 Position, float NearClip, float FarClip, float AspectRatio)
        {
            data.Projection = Matrix.Identity;//<-- Ensure we are effectively reset
            data.View = Matrix.Identity;//<-- Ensure we are effectively reset
            data.Position = Position;
            data.AspectRatio = AspectRatio;//<-- Usually just viewport width / height however allowing for mod for effects
            data.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, AspectRatio, NearClip, FarClip);
            data.Speed = 25.0f;
            data.TurnSpeed = 25.0f;
            //data.ScreenCenterX = (int)GraphicsManager.Peek.BackBufferResolution.X / 2;//<-- Lock mouse
            //data.ScreenCenterY = (int)GraphicsManager.Peek.BackBufferResolution.Y / 2;
            data.RightClicked = false;
            data.Type = CameraType.Free;
        }
        /****************************************************************************************/
        public static void Update(FreeCamera data)
        {

            if (mInput.Peek.IsRightButtonPressed())
            {
                //if (!data.RightClicked)
                //{
                    //data.OldPosition = InputManager.Peek.GetMousePosition();
                    //data.RightClicked = true;
                    //Mouse.SetPosition((int)data.OldPosition.X, (int)data.OldPosition.Y);
                //}
                //else
                //{
                    //Mouse.SetPosition((int)data.OldPosition.X, (int)data.OldPosition.Y);
                    data.Angle.X -= MathHelper.ToRadians((mInput.Peek.GetMouseDelta().Y) * data.TurnSpeed * 0.01f);
                    data.Angle.Y -= MathHelper.ToRadians((mInput.Peek.GetMouseDelta().X) * data.TurnSpeed * 0.01f);

                    //Decent method to clamp camera
                    if (data.Angle.X <= -MathHelper.PiOver2)
                        data.Angle.X = -MathHelper.PiOver2;
                    else
                        if (data.Angle.X >= MathHelper.PiOver2)
                            data.Angle.X = MathHelper.PiOver2;

                    if (data.Angle.Y >= MathHelper.TwoPi)
                        data.Angle.Y = 0.0f;
                    else
                        if (data.Angle.Y <= -MathHelper.TwoPi)
                            data.Angle.Y = 0.0f;
                //}

            }
            data.left = Vector3.Normalize(Engine.TempVector3((float)Math.Cos(data.Angle.Y), 0.0f, (float)Math.Sin(data.Angle.Y)));
            data.forward = Vector3.Normalize(Engine.TempVector3((float)Math.Sin(-data.Angle.Y), (float)Math.Sin(data.Angle.X), (float)Math.Cos(-data.Angle.Y)));

            if (mInput.Peek.IsKeyDown(Keys.Up))
                data.Position -= data.forward * data.Speed * .16f;

            if (mInput.Peek.IsKeyDown(Keys.Down))
                data.Position += data.forward * data.Speed * .16f;

            if (mInput.Peek.IsKeyDown(Keys.Left))
                data.Position -= data.left * data.Speed * .16f;

            if (mInput.Peek.IsKeyDown(Keys.Right))
                data.Position += data.left * data.Speed * .16f;

            if (mInput.Peek.IsKeyDown(Keys.PageUp))
                data.Position += Vector3.Up * data.Speed * .16f;

            data.View = Matrix.Identity;
            data.View *= Matrix.CreateTranslation(-data.Position);
            data.View *= Matrix.CreateRotationZ(data.Angle.Z);
            data.View *= Matrix.CreateRotationY(data.Angle.Y);
            data.View *= Matrix.CreateRotationX(data.Angle.X);
        }
        /****************************************************************************************/
    }
}