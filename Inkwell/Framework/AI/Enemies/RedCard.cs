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
    class RedCard : Enemy
    {
        //Mathew Kane's variables for Knockback State***
        float X = 1;
        float AX = 0;
        float AY = 0;
        float Distance, DelayTime;
        public bool check, Neg, Delay;
        //**********************************************

        public enum subState
        {
            ChaseZ,
            ChaseXLeft,
            ChaseXRight,
        }
        public subState currentSubState = subState.ChaseZ;

        #region Initialize
        public override void Initialize(Vector3 enemyPosition)
        {
            base.Initialize(enemyPosition);
            enemyType = EnemyType.RedCard;
            enemyModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.PLANE8, enemyPosition);
            Speed = Engine.Randomize(0.7f, 1.0f);
            PerceptionDistance = 60f;
            Health = 35;
            AttackDamage = 5;
            int suit = Engine.Randomize(2, 4);
            int number = Engine.Randomize(1, 11);
            switch (number)
            {
                //key ~ suit+number+state
                case 1:
                    enemyModel.Link.Texture = mAnimation.Peek.regCards[mAnimation.Peek.cardTypes[suit].ToString() + number.ToString() + "Idle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + number.ToString();
                    break;
                case 2:
                    enemyModel.Link.Texture = mAnimation.Peek.regCards[mAnimation.Peek.cardTypes[suit].ToString() + number.ToString() + "Idle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + number.ToString();
                    break;
                case 3:
                    enemyModel.Link.Texture = mAnimation.Peek.regCards[mAnimation.Peek.cardTypes[suit].ToString() + number.ToString() + "Idle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + number.ToString();
                    break;
                case 4:
                    enemyModel.Link.Texture = mAnimation.Peek.regCards[mAnimation.Peek.cardTypes[suit].ToString() + number.ToString() + "Idle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + number.ToString();
                    break;
                case 5:
                    enemyModel.Link.Texture = mAnimation.Peek.regCards[mAnimation.Peek.cardTypes[suit].ToString() + number.ToString() + "Idle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + number.ToString();
                    break;
                case 6:
                    enemyModel.Link.Texture = mAnimation.Peek.regCards[mAnimation.Peek.cardTypes[suit].ToString() + number.ToString() + "Idle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + number.ToString();
                    break;
                case 7:
                    enemyModel.Link.Texture = mAnimation.Peek.regCards[mAnimation.Peek.cardTypes[suit].ToString() + number.ToString() + "Idle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + number.ToString();
                    break;
                case 8:
                    enemyModel.Link.Texture = mAnimation.Peek.regCards[mAnimation.Peek.cardTypes[suit].ToString() + number.ToString() + "Idle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + number.ToString();
                    break;
                case 9:
                    enemyModel.Link.Texture = mAnimation.Peek.regCards[mAnimation.Peek.cardTypes[suit].ToString() + number.ToString() + "Idle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + number.ToString();
                    break;
                case 10:
                    enemyModel.Link.Texture = mAnimation.Peek.regCards[mAnimation.Peek.cardTypes[suit].ToString() + number.ToString() + "Idle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + number.ToString();
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
                currentState != State.KnockBack &&
                !mAvatar.Peek.Defending)
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
                    if (DistanceFromPlayer <= PerceptionDistance)
                    {
                        currentState = State.Moving;
                        currentSubState = subState.ChaseZ;
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
                            enemyPosition.X -= Speed;
                            SetNewPosition(enemyPosition);
                            if (enemyPosition.X <= playerPosition.X - 90 ||
                                enemyPosition.X <= boundNegX)
                            {
                                FacingRight = true;
                                currentSubState = subState.ChaseZ;
                            }
                            #endregion
                            break;
                        case subState.ChaseXRight:
                            #region ChaseXRight
                            enemyPosition.X += Speed;
                            SetNewPosition(enemyPosition);
                            if (enemyPosition.X >= playerPosition.X + 90 ||
                                enemyPosition.X >= boundPosX)
                            {
                                FacingRight = false;
                                currentSubState = subState.ChaseZ;
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
                case State.KnockBack:
                    #region KnockBack
                    //Mathew Kane***************************************************************************************************************************
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
                        currentState = State.Moving;
                        currentSubState = subState.ChaseZ;
                        AX = 0;
                        AY = 0;
                        check = false;
                    }
                    //************************************************************************************************************************************
                    #endregion
                    break;
            }
        }
    }
}
