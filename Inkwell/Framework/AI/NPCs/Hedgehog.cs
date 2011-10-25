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
    class Hedgehog : Enemy
    {
        #region Variables
        //Mathew Kane's variables for Knockback State***
        float X = 1;
        float AX = 0;
        float AY = 0;
        float Distance, DelayTime;
        public bool check, Neg, Delay;
        //**********************************************

        //used for determining direction of hedgehog when thrown
        Vector3 enemyVector;
        //used for determining speed of hedgehog when thrown
        float thrownSpeed;
        //used for determining when hedgehog collides with objects
        BasicModel[] obstacles = mPhysics.Peek.Obstacles();
        //to prevent too many bounces when thrown
        int bouncecount = 0;

        public enum subState
        {
            Moving2Target,
            ThrownByQueen,
        }
        public subState currentSubState = subState.Moving2Target;
        #endregion

        #region Initialize
        public override void Initialize(Vector3 enemyPosition)
        {
            base.Initialize(enemyPosition);
            NPC = true;
            enemyType = EnemyType.Hedgehog;
            enemyModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.PLANE4, enemyPosition);
            enemyModel.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Enemies\\Hedgehog");
            PerceptionDistance = 60f;
            currentState = State.Idle;
            Speed = Engine.Randomize(0.3f, 0.5f);
            targetPosition = enemyPosition;
        }
        public void ThrownInitialize(Vector3 enemyPosition)
        {
            base.Initialize(enemyPosition);
            NPC = true;
            enemyType = EnemyType.Hedgehog;
            enemyModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.PLANE4, enemyPosition);
            enemyModel.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Enemies\\Hedgehog");
            currentState = State.Moving;
            currentSubState = subState.ThrownByQueen;
            thrownSpeed = Engine.Randomize(1.0f, 1.5f);
            DetermineVector();
        }
        private void DetermineVector()
        {
            enemyVector = Engine.TempVector3(mAvatar.Peek.PlayerModel.Link.Position.X, mAvatar.Peek.PlayerModel.Link.Position.Y, mAvatar.Peek.PlayerModel.Link.Position.Z) - Engine.TempVector3(enemyModel.Link.Position.X, enemyModel.Link.Position.Y, enemyModel.Link.Position.Z);
            enemyVector.Normalize();
            enemyVector *= thrownSpeed;
        }
        #endregion

        public override void Update()
        {
            base.Update();

            if (bouncecount > 2)
                Health = 0;

            if (PlayerCollide &&
                currentState != State.KnockBack)
            {
                if (currentSubState == subState.ThrownByQueen)
                    mAvatar.Peek.HitPlayer(enemyPosition.X, 2);
                else
                    mAvatar.Peek.HitPlayer(enemyPosition.X, 0);
            }

            switch (currentState)
            {
                case State.Idle:
                    #region Idle
                    #endregion
                    break;
                case State.Moving:
                    #region Moving
                    switch (currentSubState)
                    {
                        case subState.Moving2Target:
                            #region Moving2Target
                            if (Vector3.Distance(targetPosition, enemyPosition) <= 1.0)
                            {
                                enemyPosition = targetPosition;
                                currentState = State.Idle;
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
                        case subState.ThrownByQueen:
                            #region ThrownbyQueen

                            enemyPosition += enemyVector;
                            SetNewPosition(enemyPosition);

                            if (Vector3.Distance(enemyPosition, playerPosition) >= 300)
                            {
                                Health = 0;
                            }
                            for (int i = 0; i < obstacles.Length; i++)
                            {
                                int collide = mPhysics.Peek.BoxCollision(enemyModel, obstacles[i]);

                                switch (collide)
                                {
                                    case 1:
                                        enemyVector.Y *= -1;
                                        bouncecount++;
                                        break;
                                    case 2:
                                        enemyVector.Y *= -1;
                                        bouncecount++;
                                        break;
                                    case 3:
                                        enemyVector.X *= -1;
                                        bouncecount++;
                                        break;
                                    case 4:
                                        enemyVector.Z *= -1;
                                        bouncecount++;
                                        break;
                                    case 5:
                                        enemyVector.X *= -1;
                                        bouncecount++;
                                        break;
                                    case 6:
                                        enemyVector.Z *= -1;
                                        bouncecount++;
                                        break;
                                }
                            }
                            #endregion
                            break;
                    }
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
                        Health = 0;
                        enemyModel.Link.Position.Y = 0;
                        currentState = State.Moving;
                        currentSubState = subState.Moving2Target;
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
