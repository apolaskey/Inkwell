using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Inkwell.Framework
{
    class MadHatter : Enemy
    {
        //Used for determining when to throw teacups
        private float nextThrowTime = 6000;
        private float throwTimeMin = 5000;
        private float throwTimeMax = 6000;
        //made these public in case we change the madhatter battle
        //that is, if we decide to move the scene back, change the table size, ect. 
        public int distMoveUp = 50;
        public int distMoveDown = -50;
        //holds spawned teacups
        public BasicModel[] teacups = new BasicModel[10];

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
        public enum subState
        {
            MoveUp,
            MoveDown,
        }
        subState currentSubState = subState.MoveUp;

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
                    ThrowTeaCup(enemyPosition);
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
                    if (DistanceFromPlayer < PerceptionDistance)
                    {
                        //talk and then ->
                        currentState = State.Moving;
                        currentSubState = subState.MoveUp;
                    }
                    #endregion
                    break;
                case State.Moving:
                    #region Moving
                    switch (currentSubState)
                    {
                        case subState.MoveUp:
                            enemyPosition.Z += Speed;
                            SetNewPosition(enemyPosition);
                            if (enemyPosition.Z >= 50)
                            {
                                currentSubState = subState.MoveDown;
                            }
                            break;
                        case subState.MoveDown:
                            enemyPosition.Z -= Speed;
                            SetNewPosition(enemyPosition);
                            if (enemyPosition.Z <= -50)
                            {
                                currentSubState = subState.MoveUp;
                            }
                            break;
                    }
                    #endregion
                    break;
            }
        }

        private void ThrowTeaCup(Vector3 spawnPosition)
        {
            BasicModel teacup = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.TEAPOT, enemyModel.Link.Position);
        }
        private void ResetThrowTime()
        {
            nextThrowTime = Engine.Randomize(throwTimeMin, throwTimeMax);
        }
    }
}
