using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace Inkwell.Framework
{
    /*Some World Constants*/
    public enum AnistropicLevel { One = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Eleven, Twelve, Thirteen, Fourteen, Fifteen, Sixteen };
    //public enum Vector { X, Y, Z }; //<-- Deprecated Old Code no longer needed
    /// <summary>A Class that contains information utilized throughout the Engine.</summary>
    static public class Engine
    {
        /****************************************************************************************/
        public const int NULLED_INT = -1; //<-- Used for various int based checks
        public static Texture2D NULLED_TEXTURE;
        public static bool GameInitialized = false; //<-- Used to determine some pre-load states
        public static ContentManager CoreContainer, GameContainer, PersistantContainer;
        private static Vector3 TEMP_VECTOR3 = Vector3.Zero;
        private static Vector2 TEMP_VECTOR2 = Vector2.Zero;
        public static bool DebugEnabled = true;
        public static bool WindowIsActive = true;
        public static StringBuilder TextBuilder = new StringBuilder("NULL", 64);
        private static Random _RandomGen = new Random();

        static DebugMessage SunPosition, SunColor, SunIntensity;
        /****************************************************************************************/
        public static Vector3 TempVector3(float X, float Y, float Z)
        {
            TEMP_VECTOR3 = Vector3.Zero;
            TEMP_VECTOR3.X = X; TEMP_VECTOR3.Y = Y; TEMP_VECTOR3.Z = Z;
            return TEMP_VECTOR3;
        }
        public static Vector2 TempVector2(float X, float Y)
        {
            TEMP_VECTOR2 = Vector2.Zero;
            TEMP_VECTOR2.X = X; TEMP_VECTOR2.Y = Y;
            return TEMP_VECTOR2;
        } 
        public static void ClearTextBuilder()
        {
            TextBuilder.Remove(0, TextBuilder.Length);
        }
        public static int Randomize(int Min, int Max)
        {
            _RandomGen.Next();
            _RandomGen.Next();
            _RandomGen.Next();
            return _RandomGen.Next(Min, Max);
        }
        public static float Randomize(float Min, float Max)
        {
            _RandomGen.Next();
            _RandomGen.Next();
            _RandomGen.Next();
            return (float)_RandomGen.NextDouble() * (Max - Min) + Min;
        }
        /// <summary>(Void) Notifies the Garbage Collector that we want to comb through the entire heap and clean it up.</summary>
        public static void PurgeGarbageHeap()
        {
            System.GC.Collect(); //<-- Tell the GC "HEY I WANT YOU TO WORK NOW!"
            System.GC.WaitForPendingFinalizers(); //<-- Tell the GC to work until all finalizers (deconstructors) are called
            while (System.GC.WaitForFullGCComplete() == GCNotificationStatus.Succeeded) { }; //<-- Tell the GC that we want to freeze threads until all of it's work is done
            
        }
        /****************************************************************************************/
        public static void Initialize(GameServiceContainer Services)
        {
            CoreContainer = new ContentManager(Services);
            CoreContainer.RootDirectory = "Content";
            GameContainer = new ContentManager(Services);
            GameContainer.RootDirectory = "Content";
            PersistantContainer = new ContentManager(Services);
            PersistantContainer.RootDirectory = "Content";
            NULLED_TEXTURE = CoreContainer.Load<Texture2D>(Assets.NULL_TEXTURE);
            mDebug.Initialize(mGraphics.Peek.Device(), CoreContainer, Assets.DEBUG_FONT, Vector2.Zero);

            SunColor = DebugMessage.Initialize(true, Color.White, true, "Sun Color: Calculating...");
            SunIntensity = DebugMessage.Initialize(true, Color.White, true, "Sun Intensity: Calculating...");
            SunPosition = DebugMessage.Initialize(true, Color.White, true, "Sun Position: Calculating...");
        }
        /****************************************************************************************/
        public static void Update(GameTime gameTime)
        {
            if(mInput.Peek.IsKeyPressed(Keys.F12))
                DebugEnabled = !DebugEnabled;

            if (DebugEnabled)
            {

                if (mInput.Peek.IsKeyPressed(Keys.F11))
                    DebugBB.DebugBoxes = !DebugBB.DebugBoxes;

                if (mInput.Peek.IsKeyPressed(Keys.F6))
                    mGraphics.Peek.FillMode(FillMode.WireFrame);

                if (mInput.Peek.IsKeyPressed(Keys.F7))
                    mGraphics.Peek.FillMode(FillMode.Point);

                if (mInput.Peek.IsKeyPressed(Keys.F5))
                    mGraphics.Peek.FillMode(FillMode.Solid);

                if (mInput.Peek.IsKeyPressed(Keys.B))
                    mGraphics.Peek.PostProcessing = !mGraphics.Peek.PostProcessing;


                if (mInput.Peek.IsKeyPressed(Keys.F10))
                {
                    mGraphics.Peek.Graphics.SynchronizeWithVerticalRetrace = !mGraphics.Peek.Graphics.SynchronizeWithVerticalRetrace;
                    mGraphics.Peek.Graphics.ApplyChanges();
                }


                if (mInput.Peek.IsKeyDown(Keys.NumPad8))
                    mEffect.Peek.SunPosition.Y += .1f;
                if (mInput.Peek.IsKeyDown(Keys.NumPad2))
                    mEffect.Peek.SunPosition.Y -= .1f;
                if (mInput.Peek.IsKeyDown(Keys.NumPad4))
                    mEffect.Peek.SunPosition.X -= .1f;
                if (mInput.Peek.IsKeyDown(Keys.NumPad6))
                    mEffect.Peek.SunPosition.X += .1f;
                if (mInput.Peek.IsKeyDown(Keys.NumPad9))
                    mEffect.Peek.SunPosition.Z += .1f;
                if (mInput.Peek.IsKeyDown(Keys.NumPad1))
                    mEffect.Peek.SunPosition.Z -= .1f;


                SunColor.Text = "Sun Color: " + mEffect.Peek.SunColor;
                SunIntensity.Text = "Sun Intensity: " + mEffect.Peek.SunIntensity;
                SunPosition.Text = "Sun Position: " + mEffect.Peek.SunPosition;

                mDebug.Update(gameTime);
            }
        }
        /****************************************************************************************/
    }
}
