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
    class AliceBookworm : Enemy
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
            Chasing,
            Switch2Flee,
            Fleeing,
            Back2Chase,
        }
        public subState currentSubState = subState.Chasing;

        #region Initialize
        public override void Initialize(Vector3 enemyPosition)
        {
            base.Initialize(enemyPosition);
            enemyType = EnemyType.AliceBookworm;
            enemyModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.PLANE6, enemyPosition);
            enemyModel.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Animations\\Bookworm\\wormMove0");
            Speed = Engine.Randomize(0.1f, 0.4f);
            PerceptionDistance = 60f;
            Health = 25;
            AttackDamage = 2;
        }
        #endregion

        public override void Update()
        {
            base.Update();

            #region Determine currentState
            if (Hit && currentState != State.KnockBack)
            {
                currentState = State.Moving;
                currentSubState = subState.Switch2Flee;
            }
            else if (currentState != State.Idle &&
                     currentState != State.KnockBack &&
                     currentSubState != subState.Switch2Flee &&
                     currentSubState != subState.Fleeing &&
                     currentSubState != subState.Back2Chase)
            {
                if (PlayerCollide)
                    currentState = State.Attacking;
                else
                {
                    currentState = State.Moving;
                    currentSubState = subState.Chasing;
                }
            }
            #endregion

            switch (currentState)
            {
                case State.Idle:
                    #region Idle
                    if (DistanceFromPlayer <= PerceptionDistance)
                    {
                        if (enemyPosition.Y < 0)
                        {
                            currentState = State.Moving;
                            currentSubState = subState.Back2Chase;
                        }
                        else
                        {
                            currentState = State.Moving;
                            currentSubState = subState.Chasing;
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
                            mAudio.Peek.PlaySound(mAudio.SoundName.WormChase, enemyPosition);
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
                        case subState.Switch2Flee:
                            #region Switch2Flee
                            if (enemyPosition.Y > -20)
                            {
                                enemyPosition.Y -= 0.6f;
                                SetNewPosition(enemyPosition);
                            }
                            if (enemyPosition.Y < -20)
                            {
                                enemyPosition.Y = -20;
                                SetNewPosition(enemyPosition);
                            }
                            if (enemyPosition.Y == -20)
                            {
                                currentSubState = subState.Fleeing;
                            }
                            #endregion
                            break;
                        case subState.Fleeing:
                            #region Fleeing
                            if (mAvatar.Peek.FacingRight)
                            {
                                targetPosition = Engine.TempVector3(playerPosition.X - 40, -10, playerPosition.Z);
                            }
                            else
                            {
                                targetPosition = Engine.TempVector3(playerPosition.X + 40, -10, playerPosition.Z);
                            }
                            if (enemyPosition.X < targetPosition.X + 0.5 &&
                                enemyPosition.X > targetPosition.X - 0.5)
                            {
                                currentSubState = subState.Back2Chase;
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
                        case subState.Back2Chase:
                            #region Back2Chase
                            if (playerPosition.X < enemyPosition.X)
                                FacingRight = false;
                            if (playerPosition.X > enemyPosition.X)
                                FacingRight = true;

                            if (enemyPosition.Y < 0)
                            {
                                enemyPosition.Y += 0.6f;
                                SetNewPosition(enemyPosition);
                            }
                            if (enemyPosition.Y > 0)
                            {
                                enemyPosition.Y = 0;
                                SetNewPosition(enemyPosition);
                            }
                            if (enemyPosition.Y == 0)
                            {
                                currentSubState = subState.Chasing;
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
                        currentSubState = subState.Chasing;
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
