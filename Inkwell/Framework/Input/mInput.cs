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
    /// <summary>
    /// (Singleton) In charge of handling input and associated events; will need an update hook and possibly a draw hook in the core.
    /// </summary>
    class mInput
    {
        /*****************************************HEADER*****************************************/
        #region Input Singleton
        private static volatile mInput _instance;
        private static object _padlock = new Object();

        public static mInput Peek
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
                            _instance = new mInput(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        #endregion
        private KeyboardState kbs_previousKeyboardState = new KeyboardState(); //Previous keyboard state
        private KeyboardState kbs_currentKeyboardState = new KeyboardState(); //Current keyboard state 
        private MouseState ms_previousMouseState = new MouseState(); //Previous mouse state 
        private MouseState ms_currentMouseState = new MouseState(); //Current mouse state 
        private Vector2 v2d_previousMousePosition = new Vector2(); //Previous mouse position
        private Vector2 v2d_currentMousePosition = new Vector2(); //Current mouse position 
        private Vector2 v2d_MouseDelta = new Vector2(); //Delta between the two mouse positions 
        private int int_previousKeyboardLength = new int(); //previous number of keys down
        private int int_currentKeyboardLength = new int(); //current number of keys down
        //get a rectangle in here for the mouse position 
        //add a draw function to change the cursor graphic
        /****************************************FUNCTIONS***************************************/
        //Update function updates all the variables to be used with other get functions. 
        //Bobby Spivey, November 23, 2010. 
        public void Update()
        {
            //update current states of the keyboard, mouse, and mouse positions. 
            //Bobby Spivey, November 23, 2010
            ms_previousMouseState = ms_currentMouseState;
            ms_currentMouseState = Mouse.GetState();
            kbs_previousKeyboardState = kbs_currentKeyboardState;
            kbs_currentKeyboardState = Keyboard.GetState();

            v2d_previousMousePosition.X = v2d_currentMousePosition.X;
            v2d_previousMousePosition.Y = v2d_currentMousePosition.Y;

            v2d_currentMousePosition.X = ms_currentMouseState.X;
            v2d_currentMousePosition.Y = ms_currentMouseState.Y;


            int_previousKeyboardLength = int_currentKeyboardLength;
            int_currentKeyboardLength = Keyboard.GetState().GetPressedKeys().Length;

            //Calculations to get the Mouse Delta so we can use it to find the direction that the mouse
            //has moved.
            /*Moving this up here Bobby & now locking the mouse using SetPosition below*/
            v2d_MouseDelta.X = v2d_previousMousePosition.X - v2d_currentMousePosition.X;
            v2d_MouseDelta.Y = v2d_previousMousePosition.Y - v2d_currentMousePosition.Y;

            //Clamp to keep the mouse from leaving the maximum screen resolution for the game. 
            //Not sure how the singletons work, so this may need revision later. 
            //Bobby Spivey, November 28, 2010
            /*Cleaned this up a little bit by adding an else everything is fine just a minor perf. gain*/
            if (v2d_currentMousePosition.X < 0)
            {
                v2d_currentMousePosition.X = 0 + 5;
            }
            else if (v2d_currentMousePosition.X > mGraphics.Peek.BackBufferResolution.X)
            {
                v2d_currentMousePosition.X = mGraphics.Peek.BackBufferResolution.X - 5;
            }
            if (v2d_currentMousePosition.Y < 0)
            {
                v2d_currentMousePosition.Y = mGraphics.Peek.IsFullScreen ? (0) : (-10);
            }
            else if (v2d_currentMousePosition.Y > mGraphics.Peek.BackBufferResolution.Y)
            {
                v2d_currentMousePosition.Y = mGraphics.Peek.BackBufferResolution.Y - 5;
            }
            /*This is responsible for locking out the mouse position*/
            if (Engine.WindowIsActive)
                Mouse.SetPosition((int)v2d_currentMousePosition.X, (int)v2d_currentMousePosition.Y);
        }
        /****************************************************************************************/

        //Function to check if a specified key is down. It returns true or false. 
        //Bobby Spivey, December 1, 2010. 
        public bool IsKeyDown(Keys key)
        {
            return kbs_currentKeyboardState.IsKeyDown(key);

        }
        /****************************************************************************************/
        /// <summary>
        /// Function to check if a specific key was pressed. [Bobby Spivey, December 1, 2010.]
        /// </summary>
        /// <param name="key">(Keys) The key that your checking.</param>
        /// <returns>(bool) True or false on whether the key was pressed.</returns>
        public bool IsKeyPressed(Keys key)
        {
            if (kbs_previousKeyboardState.IsKeyUp(key))
                return kbs_currentKeyboardState.IsKeyDown(key);
            else
                return false;
        }

        /****************************************************************************************/
        //Function to check if a key is down at all
        //Bobby Spivey, February 5, 2011
        public bool IsAnyKeyDown()
        {
            if (int_currentKeyboardLength > int_previousKeyboardLength)
                return true;
            else
                return false;
        }

        /****************************************************************************************/
        //Function to check if the Right Mouse button is pressed. If it is return true, else false. 
        //Bobby Spivey, November 23, 2010. 
        public bool IsRightButtonPressed()
        {
            if (ms_currentMouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }
        /****************************************************************************************/
        //Function to check if the Left Mouse button is pressed. If it is return true, else false. 
        //Bobby Spivey, November 23, 2010. 
        public bool IsLeftButtonPressed()
        {
            if (ms_currentMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }
        /****************************************************************************************/
        //Fuction to check the current position of the mouse. Returns a Vector2 data member. 
        //Bobby Spivey, November 23, 2010. 
        public Vector2 GetMousePosition()
        {
            return v2d_currentMousePosition;
        }
        /****************************************************************************************/
        public MouseState GetMouseState()
        {
            return ms_currentMouseState;
        }
        /****************************************************************************************/
        //Function to return the delta between the previous and current mouse positions. Allows us to find 
        //the direction that the mouse has moved. 
        //BobbySpivey, November 28, 2010
        public Vector2 GetMouseDelta()
        {
            return v2d_MouseDelta;
        }
        /****************************************************************************************/
        //<EOC>
    }
}