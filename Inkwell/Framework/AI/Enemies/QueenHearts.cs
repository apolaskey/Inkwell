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
    class QueenHearts : Enemy
    {
        #region Variables
        //DebugMessage debugState = new DebugMessage(true);
        //DebugMessage debugSubState = new DebugMessage(true);
        //DebugMessage debugWake = new DebugMessage(true);

        //used for determining when to throw hedgehogs or spawn cards
        private float nextThrowTime = 0;
        private float throwTimeMin = 500;
        private float throwTimeMax = 800;
        //counts the # spawned in current subState
        int numHedgeHogs = 15;
        int countHedgeHogs = 0;
        int numCards = 3;
        int countCards = 0;
        //made these public in case we change the queen battle
        //that is, if we decide to move the scene back, change the room size, ect. 
        public int distMoveUp = 100;
        public int distMoveDown = 0;
        //used for determining when to wake from stunned
        private float currentWakeUpTime;
        private float wakeUpTime = 6000;
        //used for determining length of time for chasing
        private float currentChaseTime;
        private float chaseTime = 6000;
        //position to spawn Hedgehogs or cards
        Vector3 spawnPosition;
        //timer for switching into card spawn state
        private float currentSwitch2ThrowTime;
        private float switch2ThrowTime = 2000;
        //timer for switching out of card spawn state
        private float currentSwitch2SpawnTime;
        private float switch2SpawnTime = 2000;

        public enum subState
        {
            MoveUp,
            MoveDown,
            SpawnCards,
            ThrowHedgeHogs,
            Chasing,
            Return2Throne,
            Stunned,
        }
        public subState currentSubState = subState.MoveUp;
        private subState previousSubState = subState.MoveUp;
        #endregion

        #region Initialize
        public override void Initialize(Vector3 enemyPosition)
        {
            base.Initialize(enemyPosition);
            enemyType = EnemyType.QueenHearts;
            enemyModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.PLANE3, enemyPosition);
            enemyModel.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Animations\\Queen\\queenIdle0");
            Health = 300;
            Speed = 0.6f;
            PerceptionDistance = 60f;
            AttackDamage = 30;
            targetPosition = Engine.TempVector3(720, 0, 50);
        }
        #endregion

        public override void Update()
        {
            base.Update();

            //Animation Fixes!!***********************************************
            if (currentSubState == subState.MoveUp)
                MoveUp = true;
            else
                MoveUp = false;
            if (currentSubState == subState.MoveDown)
                MoveDown = true;
            else
                MoveDown = false;
            if (currentSubState == subState.SpawnCards)
                SpawnCards = true;
            else
                SpawnCards = false;
            if (currentSubState == subState.ThrowHedgeHogs)
                ThrowHedgeHogs = true;
            else 
                ThrowHedgeHogs = false;
            if (currentSubState == subState.Chasing)
                Chasing = true;
            else
                Chasing = false;
            if (currentSubState == subState.Return2Throne)
                Return2Throne = true;
            else
                Return2Throne = false;
            if (currentSubState == subState.Stunned)
                Stunned = true;
            else 
                Stunned = false;
            //***************************************************************
            


            //debugState.Text = "QueenState: " + Convert.ToString(currentState);
            //debugSubState.Text = "QueenSubState: " + Convert.ToString(currentSubState);
            //debugWake.Text = "WakeTime: " + Convert.ToString(currentWakeUpTime);

            #region Determine currentState
            for (int i = 0; i < mAI.Peek.enemyList.Count; i++)
            {
                if (currentSubState != subState.Stunned &&
                    mAI.Peek.enemyList[i].currentState == State.KnockBack &&
                    mAI.Peek.enemyList[i].enemyType == EnemyType.Hedgehog &&
                    Vector3.Distance(mAI.Peek.enemyList[i].enemyModel.Link.Position, enemyPosition) <= 20.0)
                {
                    currentSubState = subState.Stunned;
                    currentState = State.Idle;
                    ResetWakeUpTime();
                }
            }
            if (DistanceFromPlayer <= 21.0 &&
                currentSubState != subState.Stunned)
            {
                currentState = State.Attacking;
            }
            else if (currentState != State.Idle)
            {
                currentState = State.Moving;

                if (countHedgeHogs >= numHedgeHogs)
                {
                    countHedgeHogs = 0;
                    ResetSwitch2SpawnTime();
                    currentSubState = subState.SpawnCards;
                }

                if (currentSubState == subState.MoveUp ||
                    currentSubState == subState.MoveDown &&
                    currentSubState != subState.ThrowHedgeHogs)
                {
                    nextThrowTime -= mTimer.Peek.ElapsedGameTime.Milliseconds;
                    if (nextThrowTime <= 0)
                    {
                        if (currentSubState == subState.MoveUp)
                            previousSubState = subState.MoveUp;
                        if (currentSubState == subState.MoveDown)
                            previousSubState = subState.MoveDown;
                        if (countHedgeHogs < numHedgeHogs)
                        {
                            currentSubState = subState.ThrowHedgeHogs;
                        }
                    }
                }
            }
            #endregion

            switch (currentState)
            {
                case State.Idle:
                    #region Idle
                    if (currentSubState != subState.Stunned)
                    {
                        if (DistanceFromPlayer <= PerceptionDistance)
                        {
                            //talk and then ->
                            currentSubState = subState.MoveUp;
                            currentState = State.Moving;
                        }
                    }
                    else
                    {
                        currentWakeUpTime -= mTimer.Peek.ElapsedGameTime.Milliseconds;
                        if (currentWakeUpTime <= 0)
                        {
                            currentSubState = subState.Chasing;
                            currentState = State.Moving;
                            ResetChaseTime();
                        }
                    }
                    #endregion
                    break;
                case State.Moving:
                    #region Moving
                    switch (currentSubState)
                    {
                        case subState.Chasing:
                            #region Chasing
                            currentChaseTime -= mTimer.Peek.ElapsedGameTime.Milliseconds;
                            if (currentChaseTime <= 0)
                            {
                                currentSubState = subState.Return2Throne;
                            }
                            if (playerPosition.X < enemyPosition.X)
                            {
                                FacingRight = false;
                                enemyPosition.X -= Speed;
                                SetNewPosition(enemyPosition);
                            }
                            if (playerPosition.X > enemyPosition.X)
                            {
                                FacingRight = true;
                                enemyPosition.X += Speed;
                                SetNewPosition(enemyPosition);
                            }
                            if (playerPosition.Z < enemyPosition.Z)
                            {
                                enemyPosition.Z -= Speed;
                                SetNewPosition(enemyPosition);
                            }
                            if (playerPosition.Z > enemyPosition.Z)
                            {
                                enemyPosition.Z += Speed;
                                SetNewPosition(enemyPosition);
                            }
                            #endregion
                            break;
                        case subState.MoveDown:
                            #region MoveDown
                            currentSwitch2ThrowTime -= mTimer.Peek.ElapsedGameTime.Milliseconds;
                            if (currentSwitch2ThrowTime <= 0)
                            {
                                enemyPosition.Z += Speed;
                                SetNewPosition(enemyPosition);
                                if (playerPosition.X < enemyPosition.X)
                                    FacingRight = false;
                                else
                                    FacingRight = true;
                                if (enemyPosition.Z >= distMoveUp)
                                {
                                    currentSubState = subState.MoveUp;
                                }
                            }
                            #endregion
                            break;
                        case subState.MoveUp:
                            #region MoveUp
                            currentSwitch2ThrowTime -= mTimer.Peek.ElapsedGameTime.Milliseconds;
                            if (currentSwitch2ThrowTime <= 0)
                            {
                                enemyPosition.Z -= Speed;
                                SetNewPosition(enemyPosition);
                                if (playerPosition.X < enemyPosition.X)
                                    FacingRight = false;
                                else
                                    FacingRight = true;
                                if (enemyPosition.Z <= distMoveDown)
                                {
                                    currentSubState = subState.MoveDown;
                                }
                            }
                            #endregion
                            break;
                        case subState.SpawnCards:
                            #region SpawnCards
                            currentSwitch2SpawnTime -= mTimer.Peek.ElapsedGameTime.Milliseconds;
                            if (currentSwitch2SpawnTime <= 0)
                            {
                                spawnPosition = Engine.TempVector3(enemyPosition.X, enemyPosition.Y, -enemyPosition.Z);
                                if (countCards < numCards)
                                {
                                    nextThrowTime -= mTimer.Peek.ElapsedGameTime.Milliseconds;
                                    if (nextThrowTime <= 0)
                                    {
                                        mAI.Peek.SpawnRandomCard(spawnPosition);
                                        countCards++;
                                        ResetThrowTime();
                                    }
                                }
                                else
                                {
                                    if (mAI.Peek.enemyList.Count == 1)
                                    {
                                        countCards = 0;
                                        ResetSwitch2ThrowTime();
                                        currentState = State.Moving;
                                        currentSubState = subState.MoveUp;
                                    }
                                }
                            }
                            #endregion
                            break;
                        case subState.ThrowHedgeHogs:
                            #region ThrowHedgehogs
                            spawnPosition = Engine.TempVector3(enemyModel.Link.Position.X, enemyModel.Link.Position.Y + 7, -enemyModel.Link.Position.Z);
                            mAI.Peek.ThrowHedgehog(spawnPosition);
                            ResetThrowTime();
                            countHedgeHogs++;
                            currentSubState = previousSubState;
                            #endregion
                            break;
                        case subState.Return2Throne:
                            #region Return2Throne
                            if (Vector3.Distance(enemyPosition, targetPosition) <= 1.0)
                            {
                                currentSubState = subState.MoveUp;
                            }
                            if (targetPosition.X < enemyPosition.X)
                            {
                                FacingRight = false;
                                enemyPosition.X -= Speed;
                                SetNewPosition(enemyPosition);
                            }
                            if (targetPosition.X > enemyPosition.X)
                            {
                                FacingRight = true;
                                enemyPosition.X += Speed;
                                SetNewPosition(enemyPosition);
                            }
                            if (targetPosition.Z < enemyPosition.Z)
                            {
                                enemyPosition.Z -= Speed;
                                SetNewPosition(enemyPosition);
                            }
                            if (targetPosition.Z > enemyPosition.Z)
                            {
                                enemyPosition.Z += Speed;
                                SetNewPosition(enemyPosition);
                            }
                            #endregion
                            break;
                    }
                    #endregion
                    break;
                case State.Attacking:
                    #region Attacking
                    mAvatar.Peek.HitPlayer(enemyPosition.X, AttackDamage);
                    //mAudio.Peek.PlaySound("PlayerHit");
                    #endregion
                    break;
            }
        }

        #region ResetTimers
        private void ResetThrowTime()
        {
            nextThrowTime = Engine.Randomize(throwTimeMin, throwTimeMax);
        }
        private void ResetWakeUpTime()
        {
            currentWakeUpTime = wakeUpTime;
        }
        private void ResetChaseTime()
        {
            currentChaseTime = chaseTime;
        }
        private void ResetSwitch2ThrowTime()
        {
            currentSwitch2ThrowTime = switch2ThrowTime;
        }
        private void ResetSwitch2SpawnTime()
        {
            currentSwitch2SpawnTime = switch2SpawnTime;
        }
        #endregion
    }
}
