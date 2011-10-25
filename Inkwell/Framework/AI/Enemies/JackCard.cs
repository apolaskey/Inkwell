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
    class JackCard : Enemy
    {
        //Mathew Kane's variables for Knockback State***
        float X = 1;
        float AX = 0;
        float AY = 0;
        float Distance, DelayTime;
        public bool check, Neg, Delay;
        //**********************************************

        //used for determining direction to chase player
        Vector3 enemyVector;

        public enum subState
        {
            FindPlayer,
            Chase,
        }
        public subState currentSubState = subState.FindPlayer;

        #region Initialize
        public override void Initialize(Vector3 enemyPosition)
        {
            base.Initialize(enemyPosition);
            enemyType = EnemyType.JackCard;
            enemyModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.PLANE8, enemyPosition);
            Speed = Engine.Randomize(1.5f, 2.0f);
            PerceptionDistance = 60f;
            Health = 35;
            AttackDamage = 10;
            int suit = Engine.Randomize(0, 4);
            switch (suit)
            {
                case 0:
                    String tempCard = mAnimation.Peek.cardTypes[suit];
                    enemyModel.Link.Texture = mAnimation.Peek.faceCards[tempCard.ToString() + "JackIdle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + "Jack".ToString();
                    break;
                case 1:
                    String tempCard1 = mAnimation.Peek.cardTypes[suit];
                    enemyModel.Link.Texture = mAnimation.Peek.faceCards[tempCard1.ToString() + "JackIdle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + "Jack".ToString();
                    break;
                case 2:
                    String tempCard2 = mAnimation.Peek.cardTypes[suit];
                    enemyModel.Link.Texture = mAnimation.Peek.faceCards[tempCard2.ToString() + "JackIdle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + "Jack".ToString();
                    break;
                case 3:
                    String tempCard3 = mAnimation.Peek.cardTypes[suit];
                    enemyModel.Link.Texture = mAnimation.Peek.faceCards[tempCard3.ToString() + "JackIdle0".ToString()];
                    enemyModel.Link.Type = mAnimation.Peek.cardTypes[suit].ToString() + "Jack".ToString();
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
                        currentSubState = subState.FindPlayer;
                        currentState = State.Moving;
                    }
                    #endregion
                    break;
                case State.Moving:
                    #region Moving
                    switch (currentSubState)
                    {
                        case subState.FindPlayer:
                            //(determine vector)
                            enemyVector = Engine.TempVector3(mAvatar.Peek.PlayerModel.Link.Position.X, mAvatar.Peek.PlayerModel.Link.Position.Y + 5, mAvatar.Peek.PlayerModel.Link.Position.Z) - Engine.TempVector3(enemyModel.Link.Position.X, enemyModel.Link.Position.Y, enemyModel.Link.Position.Z);
                            enemyVector.Normalize();
                            enemyVector *= Speed;
                            currentSubState = subState.Chase;
                            break;
                        case subState.Chase:
                            enemyPosition.X += enemyVector.X;
                            enemyPosition.Z += enemyVector.Z;
                            SetNewPosition(enemyPosition);
                            if (DistanceFromPlayer >= 90 ||
                                enemyPosition.X <= boundNegX ||
                                enemyPosition.X >= boundPosX ||
                                enemyPosition.Z <= boundNegZ ||
                                enemyPosition.Z >= boundPosZ)
                            {
                                currentSubState = subState.FindPlayer;
                            }
                            break;
                    }
                    #endregion
                    break;
                case State.Attacking:
                    #region Attacking
                    mAvatar.Peek.HitPlayer(enemyPosition.X, AttackDamage);
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
                        currentSubState = subState.FindPlayer;
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
