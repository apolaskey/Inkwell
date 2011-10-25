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
    class CheshireCat : Enemy
    {
        public enum subState
        {
            Invisible,
            Talking,
            FadeIn,
            FadeOut
        }
        public subState currentSubState;
        public override void Initialize(Vector3 enemyPosition)
        {
            
            currentSubState = subState.Invisible;
            base.Initialize(enemyPosition);
            NPC = true;
            enemyType = EnemyType.CheshireCat;
            enemyModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, Assets.PLANE6, enemyPosition);
            Speed = 1.0f;
            PerceptionDistance = 60f;
            enemyModel.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Animations\\Cat\\catFadeIn0");
        }

        public override void Update()
        {
            base.Update();

            switch (currentState)
            {
                case State.Idle:
                    #region Idle
                    switch (currentSubState)
                    {
                        case subState.Invisible:
                            if (Invisible == false)
                            {                               
                                currentSubState = subState.FadeIn;
                            }
                            break;
                        case subState.Talking:
                            if (Talking == false)
                            {
                                currentSubState = subState.FadeOut;
                            }
                            break;
                        case subState.FadeIn:
                            if (FadeIn == false)
                            {
                                currentSubState = subState.Talking;
                            }
                            break;
                        case subState.FadeOut:
                            if (FadeOut == false)
                            {
                                currentSubState = subState.Invisible;
                            }
                            break;
                        
                    }
                    #endregion
                    break;

            }
            if (currentSubState == subState.Invisible)
                Invisible = true;
            else
                Invisible = false;
            if (currentSubState == subState.FadeIn)
                FadeIn = true;
            else
                FadeIn = false;
            if (currentSubState == subState.Talking)
                Talking = true;
            else
                Talking = false;
            if (currentSubState == subState.FadeOut)
                FadeOut = true;
            else
                FadeOut = false;
        }
    }
}

