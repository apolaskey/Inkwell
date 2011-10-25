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
    /// <summary>Abstract Class for creating enemy units.</summary>
    public abstract class Enemy
    {
        //private accessors
        public bool Dead { get; private set; }
        public bool PlayerCollide { get; private set; }
        public float DistanceFromPlayer { get; private set; }

        //public variables
        public bool ImmuneToAbility; // Mathew Kane
        public bool Hit; //Mathew Kane
        public bool NPC; //NPCs cannot be attacked
        public int Health;
        public float Speed;
        public float PerceptionDistance;
        //public int AttackDistance; //not being used
        public int AttackDamage;
        public bool FacingRight;
        public int enemyTextureCount; //David Fahr
        public Vector3 enemyPosition;
        public Vector3 playerPosition;
        public Vector3 targetPosition;
        public BasicModel enemyModel;
        public int boundNegX, boundPosX, boundNegY, boundPosY, boundNegZ, boundPosZ;

        public enum EnemyType
        {
            AceCard,
            AliceBookworm,
            BlackCard,
            Caterpillar,
            CheshireCat,
            Hedgehog,
            JackCard,
            KingHearts,
            MarchHare,
            MadHatter,
            QueenHearts,
            RedCard,
            WhiteRabbit,
        }
        public EnemyType enemyType;

        public enum State
        {
            Idle,
            Moving,
            Attacking,
            KnockBack,
        }
        public State currentState = State.Idle;


        //Animation Fixes**********************
        public bool MoveUp = false;
        public bool MoveDown = false;
        public bool SpawnCards = false;
        public bool ThrowHedgeHogs = false;
        public bool Chasing = false;
        public bool Return2Throne = false;
        public bool Stunned = false;
        public bool StunAttack = false;
        //*************************************
        public bool Invisible = true;
        public bool Talking = false;
        public bool FadeIn = false;
        public bool FadeOut = false;

        //virtual keyword implies that this will 
        //likely be overwritten in derived classes
        public virtual void Initialize(Vector3 enemyPosition)
        {
            Health = 100;
            Dead = false;
            Hit = false;
            FacingRight = false;
            NPC = false;
            ImmuneToAbility = true;
            SetBounds(-1000, 1000, -1000, 1000, -1000, 1000);
        }

        //virtual keyword implies that this will 
        //likely be overwritten in derived classes
        public virtual void Update()
        {
            enemyPosition = enemyModel.Link.Position;
            playerPosition = mAvatar.Peek.PlayerModel.Link.Position;

            if (Health <= 0)
                Dead = true;

            DistanceFromPlayer = Vector3.Distance(enemyPosition, playerPosition);

            if (FacingRight)
                enemyModel.Link.HorizontalTextureFlip = true;
            else
                enemyModel.Link.HorizontalTextureFlip = false;

            //if (mPhysics.Peek.BoxCollision(mAvatar.Peek.PlayerModel, enemyModel) > 0)
            //    PlayerCollide = true;
            //else
            //    PlayerCollide = false;

            if (DistanceFromPlayer <= 6.0f)
                PlayerCollide = true;
            else
                PlayerCollide = false;
        }

        //passes new position to enemy model
        protected void SetNewPosition(Vector3 enemyPosition)
        {
            enemyModel.Link.Position = enemyPosition;
        }

        #region Bounds
        //prevents enemies from running through walls, flying in the air, ect.
        public void SetBounds(int NegX, int PosX, int NegY, int PosY, int NegZ, int PosZ)
        {
            boundNegX = NegX;
            boundPosX = PosX;
            boundNegY = NegY;
            boundPosY = PosY;
            boundNegZ = NegZ;
            boundPosZ = PosZ;
        }
        void Bounds()
        {
            if (enemyPosition.X < boundNegX)
            {
                enemyPosition.X = boundNegX;
            }
            if (enemyPosition.X > boundPosX)
            {
                enemyPosition.X = boundPosX;
            }
            if (enemyPosition.Y < boundNegY)
            {
                enemyPosition.Y = boundNegY;
            }
            if (enemyPosition.Y > boundPosY)
            {
                enemyPosition.Y = boundPosY;
            }
            if (enemyPosition.Z < boundNegZ)
            {
                enemyPosition.Z = boundNegZ;
            }
            if (enemyPosition.Z > boundPosZ)
            {
                enemyPosition.Z = boundPosZ;
            }
        }
        #endregion

        //clears resources when enemy is removed
        public void Clear()
        {
            BasicModel.Remove(enemyModel);
        }
    }
}