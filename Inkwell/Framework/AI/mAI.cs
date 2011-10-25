//Author: Andrew A. Ernst

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Inkwell.Framework.Graphics.Data;
#endregion

namespace Inkwell.Framework
{
    /// <summary>
    /// (Singleton) In charge of updating AI based Entities.
    /// </summary>
    public sealed class mAI
    {
        #region Singleton
        private static volatile mAI _instance;
        private static object _padlock = new Object();

        public static mAI Peek
        {
            get
            {
                /*Check to see if we already initialized our component*/
                if (_instance == null)
                {
                    /*Lock it so another thread cant check it*/
                    lock (_padlock)
                    {
                        /*Check one more time just to be extra careful*/
                        if (_instance == null)
                            _instance = new mAI(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        #endregion

        //stores all enemies & NPCs in the game
        public List<Enemy> enemyList = new List<Enemy>();
        //List for storing spawned teacups
        public List<Teacup> teacups = new List<Teacup>();

        #region Update
        //update
        public void Update()
        {
            mAnimation.Peek.Update(enemyList);

            //update enemies & clear dead ones
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].Update();
                if (enemyList[i].Dead)
                {
                    enemyList[i].Clear();
                    enemyList.Remove(enemyList[i]);
                }
            }

            //update teacups & clear dead ones
            for (int i = 0; i < teacups.Count; i++)
            {
                teacups[i].Update();
                if (teacups[i].dead)
                {
                    teacups[i].Clear();
                    teacups.Remove(teacups[i]);
                }
            }

            //Clears all enemies (for testing)
            if (mInput.Peek.IsKeyDown(Keys.K))
            {
                Clear();
            }
        }
        #endregion

        #region SpawnEnemy()
        /// <summary>spawn enemy based on enemyType at random position</summary>
        public void SpawnEnemy(Enemy.EnemyType enemyType)
        {
            int spawnZ = Engine.Randomize(-100, 100);
            int spawnX = Engine.Randomize(-100, 100);
            Vector3 enemyPosition = new Vector3(spawnX, 0, spawnZ);
            ConstructEnemy(enemyType, enemyPosition);
        }
        /// <summary>spawn enemy based on enemyType at enemyPosition</summary>
        public void SpawnEnemy(Enemy.EnemyType enemyType, Vector3 enemyPosition)
        {
            ConstructEnemy(enemyType, enemyPosition);
        }
        public void ThrowHedgehog(Vector3 enemyPosition)
        {
            Hedgehog hedgeHog = new Hedgehog();
            hedgeHog.ThrownInitialize(enemyPosition);
            enemyList.Add(hedgeHog);
        }
        public void SpawnRandomCard(Vector3 enemyPosition)
        {
            int cardType = Engine.Randomize(1, 5);
            switch (cardType)
            {
                case 1:
                    RedCard redCard = new RedCard();
                    redCard.SpawnedByQueen(enemyPosition);
                    enemyList.Add(redCard);
                    break;
                case 2:
                    BlackCard blackCard = new BlackCard();
                    blackCard.SpawnedByQueen(enemyPosition);
                    enemyList.Add(blackCard);
                    break;
                case 3:
                    AceCard aceCard = new AceCard();
                    aceCard.SpawnedByQueen(enemyPosition);
                    enemyList.Add(aceCard);
                    break;
                case 4:
                    JackCard jackCard = new JackCard();
                    jackCard.SpawnedByQueen(enemyPosition);
                    enemyList.Add(jackCard);
                    break;
            }
        }
        #endregion

        #region ConstructEnemy()
        //initialize enemy & add to enemyList
        private void ConstructEnemy(Enemy.EnemyType enemyType, Vector3 enemyPosition)
        {
            switch (enemyType)
            {
                case Enemy.EnemyType.AceCard:
                    {
                        AceCard aceCard = new AceCard();
                        aceCard.Initialize(enemyPosition);
                        enemyList.Add(aceCard);
                    }
                    break;
                case Enemy.EnemyType.AliceBookworm:
                    {
                        AliceBookworm aliceBookworm = new AliceBookworm();
                        aliceBookworm.Initialize(enemyPosition);
                        enemyList.Add(aliceBookworm);
                    }
                    break;
                case Enemy.EnemyType.BlackCard:
                    {
                        BlackCard blackCard = new BlackCard();
                        blackCard.Initialize(enemyPosition);
                        enemyList.Add(blackCard);
                    }
                    break;
                case Enemy.EnemyType.Caterpillar:
                    {
                        Caterpillar caterpillar = new Caterpillar();
                        caterpillar.Initialize(enemyPosition);
                        enemyList.Add(caterpillar);
                    }
                    break;
                case Enemy.EnemyType.CheshireCat:
                    {
                        CheshireCat cheshireCat = new CheshireCat();
                        cheshireCat.Initialize(enemyPosition);
                        enemyList.Add(cheshireCat);
                    }
                    break;
                case Enemy.EnemyType.Hedgehog:
                    {
                        Hedgehog hedgeHog = new Hedgehog();
                        hedgeHog.Initialize(enemyPosition);
                        enemyList.Add(hedgeHog);
                    }
                    break;
                case Enemy.EnemyType.JackCard:
                    {
                        JackCard jackCard = new JackCard();
                        jackCard.Initialize(enemyPosition);
                        enemyList.Add(jackCard);
                    }
                    break;
                case Enemy.EnemyType.KingHearts:
                    {
                        KingHearts kingHearts = new KingHearts();
                        kingHearts.Initialize(enemyPosition);
                        enemyList.Add(kingHearts);
                    }
                    break;
                case Enemy.EnemyType.MarchHare:
                    {
                        MarchHare marchHare = new MarchHare();
                        marchHare.Initialize(enemyPosition);
                        enemyList.Add(marchHare);
                    }
                    break;
                case Enemy.EnemyType.MadHatter:
                    {
                        MadHatter madHatter = new MadHatter();
                        madHatter.Initialize(enemyPosition);
                        enemyList.Add(madHatter);
                    }
                    break;
                case Enemy.EnemyType.QueenHearts:
                    {
                        QueenHearts queenHearts = new QueenHearts();
                        queenHearts.Initialize(enemyPosition);
                        enemyList.Add(queenHearts);
                    }
                    break;
                case Enemy.EnemyType.RedCard:
                    {
                        RedCard redCard = new RedCard();
                        redCard.Initialize(enemyPosition);
                        enemyList.Add(redCard);
                    }
                    break;
                case Enemy.EnemyType.WhiteRabbit:
                    {
                        WhiteRabbit whiteRabbit = new WhiteRabbit();
                        whiteRabbit.Initialize(enemyPosition);
                        enemyList.Add(whiteRabbit);
                    }
                    break;
            }
            mAnimation.Peek.initializeEnemyAnimation(enemyList); //David Fahr
        }
        #endregion

        #region Clear()
        //clears each enemy in list and their textures
        public void Clear()
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].Clear();
            }
            enemyList.Clear();
        }
        #endregion

        //<EOC>
    }
}