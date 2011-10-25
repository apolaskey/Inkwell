using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/*Andrew Polaskey 12.11.2010*/
namespace Inkwell.Framework
{
    /// <summary>
    /// (Singleton) In charge of Storing various Model's and disposing of them.
    /// </summary>
    public sealed class mModel
    {
        /*****************************************HEADER*****************************************/
        #region Singleton
        private static volatile mModel _instance;
        private static object _padlock = new Object();
        public static mModel Peek
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
                            _instance = new mModel(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        #endregion
        /// <summary>Container utilizing all of the Alpha Based Models</summary>
        public cModel[] AlphaContainer; //<-- In array format to get additional speedup on searching
        /// <summary>Container utilizing all of the Opaque Based Models</summary>
        public cModel[] OpaqueContainer;
        /*Temporary Content Holders*/
        cModel[] _tempAlphaArray = new cModel[1];
        cModel[] _tempOpaqueArray = new cModel[1];
        private Stopwatch _sWatch = new Stopwatch();
        private int DrawingModels = 0;
        public bool DrawBoundingBoxes = true;
        static BoundingBox TempBox = new BoundingBox();
        DebugMessage ZSortDebug = DebugMessage.Initialize(true, Color.White, true, "Z-Sort on NULL Models: Calculating...");
        ManualResetEvent[] _ThreadResets = new ManualResetEvent[2];
        WaitCallback[] Callbacks = new WaitCallback[2];
        /****************************************FUNCTIONS***************************************/
        public void Initialize()
        {
            AlphaContainer = new cModel[1];
            OpaqueContainer = new cModel[1];
            for (int i = 0; i < _ThreadResets.Length; i++)
                _ThreadResets[i] = new ManualResetEvent(false);

            Callbacks[0] = new WaitCallback(ThreadOpaqueSort);
            Callbacks[1] = new WaitCallback(ThreadAlphaSort);
        }
        /// <summary>Cleanup all of the Models on the system.[TODO: Keep Persitent Models and Move them accordingly]</summary>
        public void CleanUp()
        {
            for (int i = 0; i < AlphaContainer.Length; i++)
            {
                AlphaContainer[i] = null;
                AlphaContainer = new cModel[1];
                _tempAlphaArray[i] = null;
                _tempAlphaArray = new cModel[1];
            }
            for (int i = 0; i < OpaqueContainer.Length; i++)
            {
                OpaqueContainer[i] = null;
                OpaqueContainer = new cModel[1];
                _tempOpaqueArray[i] = null;
                _tempOpaqueArray = new cModel[1];
            }
        }
        /****************************************************************************************/
        /// <summary>Create a valid ID for our Array that we can use for our objects.</summary>
        public int CreateMessageIDAlpha()
        {
            int iTemp = 0; //To store our ID
            bool bListFull = true; //Used for further checking to ensure our list needs to grow
            for (int i = 0; i < AlphaContainer.Length; i++) //Search our list for a disposed message
            {
                if (AlphaContainer[i] == null || AlphaContainer[i].Disposed)
                {
                    iTemp = i;
                    bListFull = false;
                    cModel dummy = new cModel();
                    dummy.AlphaEnabled = true;
                    AlphaContainer[i] = dummy;
                    break; //We found a disposed member lets not waste anymore time here
                }
            }

            if (bListFull)//If we looked through our list and haven't found a disposed message lets increase our list
            {
                cModel dummy = new cModel();
                dummy.AlphaEnabled = true;
                return SyncAdd(dummy, true);
                
            }
            else
                return iTemp; //Hand out the re-used ID finally

        }
        /****************************************************************************************/
        /// <summary>Create a valid ID for our Array that we can use for our objects.</summary>
        public int CreateMessageIDOpaque()
        {
            int iTemp = 0; //To store our ID
            bool bListFull = true; //Used for further checking to ensure our list needs to grow
            for (int i = 0; i < OpaqueContainer.Length; i++) //Search our list for a disposed message
            {
                if (OpaqueContainer[i] == null || OpaqueContainer[i].Disposed)
                {
                    iTemp = i;
                    bListFull = false;
                    cModel dummy = new cModel();
                    dummy.AlphaEnabled = false;
                    OpaqueContainer[i] = dummy;
                    break; //We found a disposed member lets not waste anymore time here
                }
            }

            if (bListFull)//If we looked through our list and haven't found a disposed message lets increase our list
            {
                cModel dummy = new cModel();
                dummy.AlphaEnabled = false;
                return SyncAdd(dummy, false);
            }
            else
                return iTemp; //Hand out the re-used ID finally

        }
        /****************************************************************************************/
        /// <summary>Synchronize the Model into the AlphaContainer or OpaqueContainer inside of an array for better sort performance.</summary>
        private int SyncAdd(cModel Model, bool bAlphaContainer)
        {
            int temp = 0;
            if (bAlphaContainer)
            {
                cModel[] _safeContainer = new cModel[AlphaContainer.Length];
                AlphaContainer.CopyTo(_safeContainer, 0);
                AlphaContainer = new cModel[_safeContainer.Length + 1]; //<-- TODO: Power of 2 to take advantage of additional CPU ASM
                _safeContainer.CopyTo(AlphaContainer, 0);
                temp = _safeContainer.Length;
                AlphaContainer[temp] = Model;
            }
            else
            {
                cModel[] _safeContainer = new cModel[OpaqueContainer.Length];
                OpaqueContainer.CopyTo(_safeContainer, 0);
                OpaqueContainer = new cModel[_safeContainer.Length + 1]; //<-- TODO: Power of 2 to take advantage of additional CPU ASM
                _safeContainer.CopyTo(OpaqueContainer, 0);
                temp = _safeContainer.Length;
                OpaqueContainer[temp] = Model;
            }
            return temp;
        }
        private void ThreadOpaqueSort(object ResetIndex)
        {
            if (_tempOpaqueArray.Length != OpaqueContainer.Length)
                _tempOpaqueArray = new cModel[OpaqueContainer.Length];

            OpaqueContainer.CopyTo(_tempOpaqueArray, 0);

            Array.Sort(_tempOpaqueArray);
            _ThreadResets[(int)ResetIndex].Set();
        }
        private void ThreadAlphaSort(object ResetIndex)
        {
            if (_tempAlphaArray.Length != AlphaContainer.Length)
                _tempAlphaArray = new cModel[AlphaContainer.Length];

            AlphaContainer.CopyTo(_tempAlphaArray, 0);

            Array.Sort(_tempAlphaArray);
            _ThreadResets[(int)ResetIndex].Set();
        }
        public void Update()
        {
            if (Engine.DebugEnabled)
                _sWatch.Start();//<-- Start a timer to track sort perf.

            /*Perhaps: Apply Fustrum Sotrting according to Z-distance on the Camera*/
            #region Sort Models According to their Z
            ThreadPool.QueueUserWorkItem(Callbacks[0], (object)0);
            ThreadPool.QueueUserWorkItem(Callbacks[1], (object)1);
            ManualResetEvent.WaitAll(_ThreadResets);
            for (int i = 0; i < _ThreadResets.Length; i++)
                _ThreadResets[i].Reset();

            if (Engine.DebugEnabled)
                _sWatch.Stop();
            #endregion
        }
        /****************************************************************************************/
        public void Draw()
        {
            //if (Engine.DebugEnabled)
            //    _sWatch.Start();//<-- Start a timer to track sort perf.
            ///*Perhaps: Apply Fustrum Sotrting according to Z-distance on the Camera*/
            //#region Sort Models According to their Z
            ///*Create our arrays only if they are too small*/
            //if (_tempAlphaArray.Length != AlphaContainer.Length)
            //    _tempAlphaArray = new cModel[AlphaContainer.Length]; //<-- Create two arrays for both of our lists (this is just a GET IT WORKING method)
            //if (_tempOpaqueArray.Length != OpaqueContainer.Length)
            //    _tempOpaqueArray = new cModel[OpaqueContainer.Length];

            //AlphaContainer.CopyTo(_tempAlphaArray, 0); //<-- Copy the contents into the temp arrays
            //OpaqueContainer.CopyTo(_tempOpaqueArray, 0);

            //Array.Sort(_tempAlphaArray);
            //Array.Sort(_tempOpaqueArray);

            //if (Engine.DebugEnabled)
            //    _sWatch.Stop();
            //#endregion

            #region Bounding Box Debug Hook
            /*Draw our Debug Boxes if we are Live Debug mode*/
            if (Engine.DebugEnabled)
            {


                if (DrawBoundingBoxes)
                {
                    mGraphics.Peek.SetDepthBuffer(true);
                    for (int i = 0; i < _tempOpaqueArray.Length; i++)
                    {
                        if (_tempOpaqueArray[i] != null)
                        {
                            if (_tempOpaqueArray[i].Initialized && !_tempOpaqueArray[i].Disposed)
                            {
                                DebugBB.Draw(_tempOpaqueArray[i], Color.Red);
                            }
                        }
                    }
                    for (int i = 0; i < AlphaContainer.Length; i++)
                    {
                        if (_tempAlphaArray[i] != null)
                        {
                            if (_tempAlphaArray[i].Initialized && !_tempAlphaArray[i].Disposed)
                                DebugBB.Draw(_tempAlphaArray[i], Color.Blue);
                        }
                    }
                    mGraphics.Peek.SetDepthBuffer(false);
                }
            }
            #endregion

            #region Draw Alpha Models & Opaque Models
            /*Loop through our Arrays and Draw their contents Opaque's going first*/
            //for (int i = 0; i < _tempOpaqueArray.Length; i++)
            for (int i = 0; i < _tempOpaqueArray.Length; i++)
            {
                if (_tempOpaqueArray[i] != null)
                {
                    if (_tempOpaqueArray[i].Initialized && _tempOpaqueArray[i].Display && !_tempOpaqueArray[i].Disposed)
                    {
                        TempBox = _tempOpaqueArray[i].BoundingBox;
                        TempBox.Max = _tempOpaqueArray[i].BoundingBox.Max + _tempOpaqueArray[i].Position;
                        TempBox.Min = _tempOpaqueArray[i].BoundingBox.Min + _tempOpaqueArray[i].Position;
                        if (mCamera.Peek.ViewBounds().Intersects(TempBox))
                        {
                            DrawingModels++;
                            cModel.Draw(_tempOpaqueArray[i], mCamera.Peek.ReturnCamera());
                        }
                    }
                }
            }

            /*Loop through our Array of Alpha's ensuring they go last*/
            for (int i = 0; i < _tempAlphaArray.Length; i++)
            {
                if (_tempAlphaArray[i] != null)
                {
                    if (_tempAlphaArray[i].Initialized && _tempAlphaArray[i].Display && !_tempAlphaArray[i].Disposed)
                    {
                        TempBox = _tempAlphaArray[i].BoundingBox;
                        TempBox.Max = _tempAlphaArray[i].BoundingBox.Max + _tempAlphaArray[i].Position;
                        TempBox.Min = _tempAlphaArray[i].BoundingBox.Min + _tempAlphaArray[i].Position;
                        if (mCamera.Peek.ViewBounds().Intersects(TempBox))
                        {
                            DrawingModels++;
                            cModel.Draw(_tempAlphaArray[i], mCamera.Peek.ReturnCamera());
                        }
                    }
                }
            }
            #endregion
            if (Engine.DebugEnabled)
            {
                /*Take Advantage of a check to reset the stopwatch*/
                ZSortDebug.Text = "Z-Sort on " + DrawingModels + " Models: " + _sWatch.Elapsed.TotalMilliseconds + "ms";
                DrawingModels = 0;
                _sWatch.Reset();
            }
        }
        /****************************************************************************************/
        //<EOC>
    }
}
