using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Inkwell.Framework
{
    class Hedgehog : Enemy
    {
        //MATHEW KANE's variables for Knockback State***
        float X = 1;
        float AX = 0;
        float AY = 0;
        float Distance, DelayTime;
        public bool check, Neg, Delay;
        //**********************************************

        public enum subState
        {
            Moving2Target,
            ThrownByQueen,
        }
        subState currentSubState = subState.Moving2Target;

        public override void Initialize(Vector3 enemyPosition)
        {
            base.Initialize(enemyPosition);
            enemyType = EnemyType.Hedgehog;
            enemyModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.PLANE6, enemyPosition);
            enemyModel.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Enemies\\Hedgehog");
            Speed = Engine.Randomize(0.3f, 0.6f);
            PerceptionDistance = 60f;
            DetermineTargetPosition();
            currentState = State.Moving;
        }
        public override void Update()
        {
            base.Update();

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
                            break;
                        case subState.ThrownByQueen:

                            break;
                    }
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
                        Health -= 50;
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

        private void DetermineTargetPosition()
        {
            targetPosition = Engine.TempVector3(mAvatar.Peek.PlayerModel.Link.Position.X, mAvatar.Peek.PlayerModel.Link.Position.Y, mAvatar.Peek.PlayerModel.Link.Position.Z);
        }
    }
}
