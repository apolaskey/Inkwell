//Author: Andrew A. Ernst

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
#endregion

namespace Inkwell.Framework
{
    class MadHatter : Enemy
    {
        #region Variables
        //Used for determining when to throw teacups
        private float nextThrowTime = 500;
        private float throwTimeMin = 1;
        private float throwTimeMax = 1000;
        //made these public in case we change the madhatter battle
        //that is, if we decide to move the scene back, change the table size, ect. 
        public int distMoveUp = 50;
        public int distMoveDown = -50;
        //for adding teacups to list
        static Teacup teacup;

        public enum subState
        {
            MoveUp,
            MoveDown,
        }
        public subState currentSubState = subState.MoveUp;
        #endregion

        #region Initialize
        public override void Initialize(Vector3 enemyPosition)
        {
            currentState = State.Moving;
            base.Initialize(enemyPosition);
            NPC = true;
            enemyType = EnemyType.MadHatter;
            enemyModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.PLANE6, enemyPosition);
            enemyModel.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Enemies\\MadHatter");
            Speed = 1.5f;
            PerceptionDistance = 60f;
        }
        #endregion

        public override void Update()
        {
            base.Update();

            #region Determine currentState
            if (currentState == State.Moving &&
                currentSubState == subState.MoveUp ||
                currentSubState == subState.MoveDown)
            {
                nextThrowTime -= mTimer.Peek.ElapsedGameTime.Milliseconds;
                if (nextThrowTime < 0)
                {
                    ThrowTeaCup();
                    ResetThrowTime();
                }
            }
            if (PlayerCollide)
            {
                //stop throwing teacups?
            }
            else if (currentState != State.Idle)
            {
                currentState = State.Moving;
            }
            #endregion

            switch (currentState)
            {
                case State.Idle:
                    #region Idle
                    //if (DistanceFromPlayer <= PerceptionDistance)
                    //{
                    //    //talk and then ->
                    //    currentSubState = subState.MoveUp;
                    //    currentState = State.Moving;
                    //}
                    #endregion
                    break;
                case State.Moving:
                    #region Moving
                    //switch (currentSubState)
                    //{
                    //    case subState.MoveUp:
                    //        enemyPosition.Z += Speed;
                    //        SetNewPosition(enemyPosition);
                    //        if (enemyPosition.Z >= distMoveUp)
                    //        {
                    //            currentSubState = subState.MoveDown;
                    //        }
                    //        break;
                    //    case subState.MoveDown:
                    //        enemyPosition.Z -= Speed;
                    //        SetNewPosition(enemyPosition);
                    //        if (enemyPosition.Z <= distMoveDown)
                    //        {
                    //            currentSubState = subState.MoveUp;
                    //        }
                    //        break;
                    //}
                    #endregion
                    break;
            }
        }

        //Throw teacup at player
        private void ThrowTeaCup()
        {
            teacup = new Teacup();
            teacup.Initialize(Engine.TempVector3(enemyPosition.X, enemyPosition.Y + 5, -enemyPosition.Z), true);
            mAI.Peek.teacups.Add(teacup);
        }

        //reset time for spawning teacup
        private void ResetThrowTime()
        {
            nextThrowTime = Engine.Randomize(throwTimeMin, throwTimeMax);
        }
    }
}
