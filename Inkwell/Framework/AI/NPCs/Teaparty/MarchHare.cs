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
    class MarchHare : Enemy
    {
        #region Variables
        //Used for determining when to throw teacups
        private float nextThrowTime = 500;
        private float throwTimeMin = 500;
        private float throwTimeMax = 1000;
        //made these public in case we change the madhatter battle
        //that is, if we decide to move the scene back, change the table size, ect. 
        public int distMoveRight = 50;
        public int distMoveLeft = -50;
        //for adding teacups to list
        static Teacup teacup;

        public enum subState
        {
            MoveRight,
            MoveLeft,
        }
        public subState currentSubState = subState.MoveRight;
        #endregion

        #region Initialize
        public override void Initialize(Vector3 enemyPosition)
        {
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
                currentSubState == subState.MoveRight ||
                currentSubState == subState.MoveLeft)
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
                    if (DistanceFromPlayer <= PerceptionDistance)
                    {
                        //talk and then ->
                        currentSubState = subState.MoveLeft;
                        currentState = State.Moving;
                    }
                    #endregion
                    break;
                case State.Moving:
                    #region Moving
                    switch (currentSubState)
                    {
                        case subState.MoveRight:
                            enemyPosition.X += Speed;
                            SetNewPosition(enemyPosition);
                            if (enemyPosition.X >= distMoveRight)
                            {
                                currentSubState = subState.MoveLeft;
                            }
                            break;
                        case subState.MoveLeft:
                            enemyPosition.X -= Speed;
                            SetNewPosition(enemyPosition);
                            if (enemyPosition.X <= distMoveLeft)
                            {
                                currentSubState = subState.MoveRight;
                            }
                            break;
                    }
                    #endregion
                    break;
            }
        }

        //Throw teacup at player
        private void ThrowTeaCup()
        {
            teacup = new Teacup();
            teacup.Initialize(Engine.TempVector3(enemyPosition.X, enemyPosition.Y + 5, -enemyPosition.Z), false);
            mAI.Peek.teacups.Add(teacup);
        }

        //reset time for spawning teacup
        private void ResetThrowTime()
        {
            nextThrowTime = Engine.Randomize(throwTimeMin, throwTimeMax);
        }
    }
}
