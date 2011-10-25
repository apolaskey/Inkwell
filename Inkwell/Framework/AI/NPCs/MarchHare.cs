using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Inkwell.Framework
{
    class MarchHare : Enemy
    {
        public override void Initialize(Vector3 enemyPosition)
        {
            base.Initialize(enemyPosition);
            NPC = true;
            enemyType = EnemyType.MarchHare;
            enemyModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.PLANE6, enemyPosition);
            enemyModel.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Enemies\\MarchHare");
            Speed = 1.0f;
        }

        public override void Update()
        {
            base.Update();

            switch (currentState)
            {
                case State.Idle:
                    //talk?
                    break;
                case State.Moving:
                    #region Moving
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
            }
        }
    }
}
