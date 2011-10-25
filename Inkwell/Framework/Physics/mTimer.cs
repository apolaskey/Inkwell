using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Inkwell.Framework
{
    /*Courtesy of Sgt.Cuddles overseen by Andrew Polaskey*/
    public sealed class mTimer
    {
        //TIMER VARIABLES
        TimerData _eTimerData = new TimerData();
        TimerData _tTimerData = new TimerData();

        //SINGLETON VARIABLES
        private static mTimer _Instance = null;
        private static readonly object _PadLock = new object();

        public TimerData ElapsedGameTime
        {
            get { return _eTimerData; }
        }

        public TimerData ElapsedTotalGameTime
        {
            get { return _tTimerData; }
        }


        public void Initialize()
        {
            //ELAPSED TIMER DATA
            _eTimerData.Milliseconds = 0.0f;
            _eTimerData.Seconds = 0.0f;
            _eTimerData.Minutes = 0.0f;
            _eTimerData.Hours = 0.0f;
            _eTimerData.Days = 0.0f;

            //ELAPSED TIMER DATA
            _tTimerData.Milliseconds = 0.0f;
            _tTimerData.Seconds = 0.0f;
            _tTimerData.Minutes = 0.0f;
            _tTimerData.Hours = 0.0f;
            _tTimerData.Days = 0.0f;
        }

        public void Update(ref GameTime gameTime)
        {
            //ELAPSED TIME BEING SET
            _eTimerData.Milliseconds = (float)gameTime.ElapsedGameTime.Milliseconds;
            _eTimerData.Seconds = (float)gameTime.ElapsedGameTime.Seconds;
            _eTimerData.Minutes = (float)gameTime.ElapsedGameTime.Minutes;
            _eTimerData.Hours = (float)gameTime.ElapsedGameTime.Hours;
            _eTimerData.Days = (float)gameTime.ElapsedGameTime.Days;

            //TOTAL TIME BEING SET
            _tTimerData.Milliseconds = (float)gameTime.TotalGameTime.TotalMilliseconds;
            _tTimerData.Seconds = (float)gameTime.TotalGameTime.TotalSeconds;
            _tTimerData.Minutes = (float)gameTime.TotalGameTime.TotalMinutes;
            _tTimerData.Hours = (float)gameTime.TotalGameTime.TotalHours;
            _tTimerData.Days = (float)gameTime.TotalGameTime.TotalDays;
        }

        public void CreateElapsedTimeSnapshot(TimerData Data)
        {
            Data = _eTimerData; 
        }
        public void CreateTotalTimeSnapshot(TimerData Data)
        {
            Data = _tTimerData;
        }
        public static mTimer Peek
        {
            get
            {
                /*Check to see if we already initialized our component*/
                if (_Instance == null)
                {
                    /*Lock it so another thread cant check it*/
                    lock (_PadLock)
                    {
                        /*Check one more time just to be extra careful*/
                        if (_Instance == null)
                            _Instance = new mTimer(); //<-- Create our component
                    }
                }
                return _Instance;
            }
        }
    }

    public struct TimerData
    {
        public float Milliseconds, Seconds, Minutes, Hours, Days;
    }
}