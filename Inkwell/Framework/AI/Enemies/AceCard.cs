//Author: Andrew A. Ernst

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Inkwell.Framework.Graphics.Data;
#endregion

namespace Inkwell.Framework
{
    class AceCard : Enemy
    {
        //Mathew Kane's variables for Knockback State***
        float X = 1;
        float AX = 0;
        float AY = 0;
        float Distance, DelayTime;
        public bool check, Neg, Delay;
        //**********************************************

        private float currentRecoverTime;
        private float recoverTime = 4000;

        public enum subState
        {
            ChaseZ,
            ChaseXLeft,
            ChaseXRight,
            Recovering,
        }
        public subState currentSubState = subState.ChaseZ;

        #region Initialize
        public override void Initialize(Vector3 enemyPosition)
        {
            base.Initialize(enemyPosition);
            enemyType = EnemyType.AceCard;
            enemyModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.PLANE8, enemyPosition);
            Speed = Engine.Randomize(0.3f, 0.4f);
            PerceptionDistance = 60f;
            Health = 35;
            AttackDamage = 15;
            int suit = Engine.Randomize(0, 4);
            switch (suit)
            {
                case 0:
                    String tempCard = mAnimation.Peek.cardTypes[suit];
                    enemyModel.Link.Texture = mAnimation.Peek.faceCards[tempCard.ToString() + "AceIdle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + "Ace".ToString();
                    break;
                case 1:
                    String tempCard1 = mAnimation.Peek.cardTypes[suit];
                    enemyModel.Link.Texture = mAnimation.Peek.faceCards[tempCard1.ToString() + "AceIdle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + "Ace".ToString();
                    break;
                case 2:
                    String tempCard2 = mAnimation.Peek.cardTypes[suit];
                    enemyModel.Link.Texture = mAnimation.Peek.faceCards[tempCard2.ToString() + "AceIdle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + "Ace".ToString();
                    break;
                case 3:
                    String tempCard3 = mAnimation.Peek.cardTypes[suit];
                    enemyModel.Link.Texture = mAnimation.Peek.faceCards[tempCard3.ToString() + "AceIdle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + "Ace".ToString();
                    break;
            }
        }
        public void SpawnedByQueen(Vector3 enemyPosition)
        {
            Initialize(enemyPosition);
            currentState = State.Moving;
        }
        #endregion

        public override void Update()
        {
            base.Update();

            #region Determine currentState
            if (PlayerCollide &&
                currentSubState != subState.Recovering &&
                currentState != State.KnockBack)
                currentState = State.Attacking;
            else if (currentState != State.Idle &&
                currentState != State.KnockBack)
            {
                currentState = State.Moving;
            }
            #endregion

            switch (currentState)
            {
                case State.Idle:
                    #region Idle
                    if (currentSubState != subState.Recovering)
                    {
                        if (DistanceFromPlayer <= PerceptionDistance)
                        {
                            currentSubState = subState.ChaseZ;
                            currentState = State.Moving;
                        }
                    }
                    else
                    {
                        currentRecoverTime -= mTimer.Peek.ElapsedGameTime.Milliseconds;
                        if (currentRecoverTime <= 0)
                        {
                            currentSubState = subState.ChaseZ;
                            currentState = State.Moving;
                        }
                    }
                    #endregion
                    break;
                case State.Moving:
                    #region Moving
                    switch (currentSubState)
                    {
                        case subState.ChaseZ:
                            #region ChaseZ
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
                            if (playerPosition.Z > enemyPosition.Z - 1.5f && playerPosition.Z < enemyPosition.Z + 1.5f)
                            {
                                if (playerPosition.X < enemyPosition.X)
                                {
                                    currentSubState = subState.ChaseXLeft;
                                }
                                if (playerPosition.X > enemyPosition.X)
                                {
                                    currentSubState = subState.ChaseXRight;
                                }
                            }
                            #endregion
                            break;
                        case subState.ChaseXLeft:
                            #region ChaseXLeft
                            enemyPosition.X -= Speed * 9;
                            SetNewPosition(enemyPosition);
                            if (enemyPosition.X <= playerPosition.X - 90 ||
                                enemyPosition.X <= boundNegX)
                            {
                                FacingRight = true;
                                ResetRecoverTime();
                                currentSubState = subState.Recovering;
                                currentState = State.Idle;
                            }
                            #endregion
                            break;
                        case subState.ChaseXRight:
                            #region ChaseXRight
                            enemyPosition.X += Speed * 9;
                            SetNewPosition(enemyPosition);
                            if (enemyPosition.X >= playerPosition.X + 90 ||
                                enemyPosition.X >= boundPosX)
                            {
                                FacingRight = false;
                                ResetRecoverTime();
                                currentSubState = subState.Recovering;
                                currentState = State.Idle;
                            }
                            #endregion
                            break;
                    }
                    #endregion
                    break;
                case State.Attacking:
                    #region Attacking
                    mAvatar.Peek.HitPlayer(enemyPosition.X, AttackDamage);
                    ResetRecoverTime();
                    currentSubState = subState.Recovering;
                    currentState = State.Idle;
                    #endregion
                    break;
                case State.KnockBack:
                    #region KnockBack
                    //MATHEW KANE***************************************************************************************************************************
                    if (!check)
                    {
                        Distance = mAvatar.Peek.PlayerModel.Link.Position.X - enemyModel.Link.Position.X;//Get the distance between the character and enemy
                        if (Distance > 0)//Finds what side the enemy is to the player
                        {
                            Neg = false;
                        }
                        else
                        {
                            Neg = true;
                            Distance *= -1;// makes sure the distance is postive
                        }

                        Distance /= 2;//cut it to reduse how far the enemy gose when hit
                        while (AY >= 0)//finds X when Y is 0
                        {
                            AY = -10 * ((float)Math.Pow((AX / Distance), 2)) + (13 - Distance);//The math will be replaced with something better don't worry
                            AX--;
                        }
                        AY = 0;//reset Y
                        AX += 2;//Set the x two steps back
                        X = -1 * AX;// Used for distance
                        check = true;//makes sure this if statement is only looked at once till everything is done
                    }
                    else
                    {
                        DelayTime += mTimer.Peek.ElapsedGameTime.Milliseconds;
                        if (DelayTime > 30)
                        {
                            Delay = true;
                            DelayTime = 0;
                        }
                        else
                        {
                            Delay = false;
                        }
                    }
                    if (Delay)
                    {
                        AY = -10 * ((float)Math.Pow((AX / Distance), 2)) + (13 - Distance);//The math
                        if (Neg)//Finds what direction should the enemy go when hit
                        {
                            enemyModel.Link.Position.Y += AY;
                            enemyModel.Link.Position.X += AX + X;
                        }
                        else
                        {
                            enemyModel.Link.Position.Y += AY;
                            enemyModel.Link.Position.X -= AX + X;
                        }
                        AX++;
                    }
                    if (enemyModel.Link.Position.Y < 0.0f)//Once the enemy hits the ground (y=0 for now ), resets everything
                    {
                        Health -= 15;
                        enemyModel.Link.Position.Y = 0;
                        currentState = State.Idle;
                        currentSubState = subState.Recovering;
                        AX = 0;
                        AY = 0;
                        check = false;
                    }
                    //************************************************************************************************************************************
                    #endregion
                    break;
            }
        }

        private void ResetRecoverTime()
        {
            currentRecoverTime = recoverTime;
        }
    }
}
