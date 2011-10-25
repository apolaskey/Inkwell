 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inkwell.Framework.Graphics.Data
{
    //David Fahr, January 20, 2010. 
    class mAnimation
    {
        #region Animation Singleton
        private static volatile mAnimation _instance;
        private static object _padlock = new Object();
        public static mAnimation Peek
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
                            _instance = new mAnimation(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Texture Arrays
        //hero textures
        Texture2D[] heroJump = new Texture2D[1];
        Texture2D[] heroMove = new Texture2D[12];
        Texture2D[] heroIdle = new Texture2D[12];
        Texture2D[] heroAttackH = new Texture2D[6];
        Texture2D[] heroAttackL = new Texture2D[6];
        Texture2D[] heroAttackA = new Texture2D[12];
        Texture2D[] heroDefend = new Texture2D[1];

        //white rabbit textures
        Texture2D[] rabbitMove = new Texture2D[3];
        Texture2D[] rabbitIdle = new Texture2D[1];

        //caterpillar textures
        Texture2D[] caterpillarMove = new Texture2D[56];

        //cat textures
        Texture2D[] catIdle = new Texture2D[14];
        Texture2D[] catFadeIn = new Texture2D[22];
        Texture2D[] catFadeOut = new Texture2D[22];

        ////worm textures
        Texture2D[] wormMove = new Texture2D[5];
        Texture2D[] wormAttack = new Texture2D[10];

        ////card textures
        public Dictionary<String, Texture2D> faceCards = new Dictionary<String, Texture2D>();
        public Dictionary<String, Texture2D> regCards = new Dictionary<String, Texture2D>();

        ////mad hatter texutes
        Texture2D[] hatterIdle = new Texture2D[1];
        Texture2D[] hatterMove = new Texture2D[6];

        //queen textures
        Texture2D[] queenIdle = new Texture2D[1];
        Texture2D[] queenMove = new Texture2D[2];
        Texture2D[] queenPoint = new Texture2D[10];
        Texture2D[] queenStun = new Texture2D[8];
        Texture2D[] queenStunned = new Texture2D[1];
        #endregion

        //specific char bool
        public bool isFadedIn = false;

        private String heroCurrState, heroPrevState, enemyCurrState;
        private int enemyFrameCount;
        private int[] textureSpeeds;
        private int tempy, tempy2;
        public bool changeState = false;
        public Dictionary<String, int> heroFrameSpeed = new Dictionary<String, int>();
        public String[] queenStates = new String[] { "queenIdle0", "queenMove", "queenPoint", "queenStun", "queenStunned" };
        public String[] cardTypes = new String[] { "club", "spade", "diamond", "heart", };
        public String[] cardStates = new String[] { "Attack0", "Attack1", "Idle0" };
        public String[] faceTypes = new String[] { "Ace", "Jack", "King" };
        public String[,] textureNames = new String[,] {  //pass name of each array and folder name 
            { "heroJump", "Character" }, { "heroMove", "Character" }, { "heroIdle", "Character" }, { "heroAttackL", "Character" }, { "heroAttackH", "Character" }, { "heroAttackA", "Character" },
            { "rabbitMove", "WhiteRabbit" }, { "rabbitIdle", "WhiteRabbit" },
            { "caterpillarMove", "Caterpillar" },
            { "catIdle", "Cat"}, { "catFadeIn", "Cat"}, { "catFadeOut", "Cat"},
            { "wormMove", "BookWorm"}, { "wormAttack", "BookWorm"},
            {"hatterIdle", "Mad Hatter" }, { "hatterThrow", "Mad Hatter"}, 
            { "queenIdle", "Queen"}, { "queenMove", "Queen"}, { "queenPoint", "Queen"}, { "queenStun", "Queen"}, { "queenStunned", "Queen"}
            };

        public void Load()
        {
            //jump, move, idle, attackL, attachH, attackA
            textureSpeeds = new int[6] { 0, 1, 1, 3, 2, 0 };

            for (int i = 1; i < textureSpeeds.Length; i++)
            {
                heroFrameSpeed.Add(textureNames[i, 0] + "Count", 0);
                heroFrameSpeed.Add(textureNames[i, 0] + "Frame", 0);
            }

            //load all reg card textures into dictionary ~120 count...
            //key ~ suit+number+state
            for (int i = 1; i < 11; i++)
            {
                tempy = 0;
                tempy2 = 0;
                for (int x = 1; x < 13; x++)
                {
                    if (tempy2 >= 3)
                    {
                        tempy++;
                        tempy2 = 0;
                    }
                    if (tempy >= 5)
                    {
                        tempy = 0;
                    }
                    regCards[cardTypes[tempy].ToString() + i.ToString() + cardStates[tempy2].ToString()] = Engine.GameContainer.Load<Texture2D>("Textures\\Animations\\Cards\\" + cardTypes[tempy].ToString() + "\\"
                                                                                                                                                + cardTypes[tempy].ToString() + i.ToString() + cardStates[tempy2].ToString());
                    tempy2++;
                }
            }

            //load all reg card textures into dictionary ~36 count...
            //key ~ suit+type+state
            for (int i = 0; i < 4; i++)
            {
                tempy = 0;
                tempy2 = 0;
                for (int x = 0; x < 9; x++)
                {
                    if (tempy2 >= 3)
                    {
                        tempy++;
                        tempy2 = 0;
                    }
                    if (tempy >= 5)
                    {
                        tempy = 0;
                    }
                    faceCards[cardTypes[i].ToString() + faceTypes[tempy].ToString() + cardStates[tempy2].ToString()] = Engine.GameContainer.Load<Texture2D>("Textures\\Animations\\Cards\\" + cardTypes[i].ToString() + "\\"
                                                                                                                                                           + cardTypes[i].ToString() + faceTypes[tempy].ToString() + cardStates[tempy2].ToString());
                    tempy2++;
                }
            }

            Texture2D[][] textureArray = new Texture2D[][] { //pass each array of textures into the array - same order as prev
                heroJump, heroMove, heroIdle, heroAttackL, heroAttackH,  heroAttackA, 
                rabbitMove, rabbitIdle,
                caterpillarMove,
                catIdle, catFadeIn, catFadeOut,
                wormMove, wormAttack,
                hatterIdle, hatterMove,
                queenIdle, queenMove, queenPoint, queenStun, queenStunned
                };

            for (int i = 0; i < textureArray.Count(); i++)
            {
                int tempInt;
                String tempString, tempString2;
                tempInt = textureArray.Count(); // tempInt to texture array count
                Texture2D tempHold; // tempArray to new Texture2D array with length element i array length

                for (int x = 0; x < textureArray[i].Count(); x++) //loop until element i array length
                {
                    tempString = textureNames[i, 0] + x.ToString(); //tempString to name of first string in i set
                    tempString2 = textureNames[i, 1]; //tempString2 to texture name of second string in i set
                    tempHold = Engine.GameContainer.Load<Texture2D>("Textures\\Animations\\" + tempString2.ToString() + "\\" + tempString.ToString()); //load texture into temp array
                    textureArray[i][x] = tempHold; //position x of element i array = tempArray texture 
                }
            }
        }

        public void initializeHeroAnimation(Enum state)
        {
            heroPrevState = heroCurrState;
            int tempInt = 0;
            heroCurrState = Convert.ToString(state); //convert current hero state to string
            if (heroCurrState != heroPrevState) //if the state has changed
            {
                for (int i = 1; i < textureSpeeds.Length; i++)
                {
                    heroFrameSpeed[textureNames[i, 0] + "Count"] = 0;
                    heroFrameSpeed[textureNames[i, 0] + "Frame"] = 0;
                }
            }
            else

                if (heroCurrState == "Standing" ||
                    heroCurrState == "Talking")
                {

                    tempInt = heroIdle.Count(); //tempInt to total number of texutres in Idle array
                    mAvatar.Peek.PlayerModel.Link.Texture = heroIdle[heroFrameSpeed["heroIdleFrame"]]; //playermodel texture to heroidle texture of element heroFrame
                    heroFrameSpeed["heroIdleCount"]++;

                }
                else if (heroCurrState == "Walking" ||
                    heroCurrState == "DashingLeft" ||
                    heroCurrState == "DashingRight")
                {

                    tempInt = heroMove.Count();
                    mAvatar.Peek.PlayerModel.Link.Texture = heroMove[heroFrameSpeed["heroMoveFrame"]];
                    heroFrameSpeed["heroMoveCount"]++;
                }
                else if (heroCurrState == "AttackingLight")
                {
                    tempInt = heroAttackL.Count();
                    mAvatar.Peek.PlayerModel.Link.Texture = heroAttackL[heroFrameSpeed["heroAttackLFrame"]];
                    heroFrameSpeed["heroAttackLCount"]++;

                }
                else if (heroCurrState == "AttackingHeavy")
                {
                    tempInt = heroAttackH.Count();
                    mAvatar.Peek.PlayerModel.Link.Texture = heroAttackH[heroFrameSpeed["heroAttackHFrame"]];
                    heroFrameSpeed["heroAttackHCount"]++;

                }
                else if (heroCurrState == "Ability")
                {
                    tempInt = heroAttackA.Count();
                    mAvatar.Peek.PlayerModel.Link.Texture = heroAttackA[heroFrameSpeed["heroAttackAFrame"]];
                    heroFrameSpeed["heroAttackACount"]++;
                }
                else if (heroCurrState == "Jumping" ||
                    heroCurrState == "Falling" ||
                    heroCurrState == "Hit")
                {
                    tempInt = heroJump.Count();
                    mAvatar.Peek.PlayerModel.Link.Texture = heroJump[0];
                }


            for (int i = 1; i < textureSpeeds.Length; i++)
            {

                if (heroFrameSpeed[textureNames[i, 0] + "Frame"] >= tempInt - 1)
                {
                    if (heroFrameSpeed["heroAttackHFrame"] >= 5)
                    {
                        heroFrameSpeed["heroAttackHFrame"] = 5;
                    }
                    else
                        heroFrameSpeed[textureNames[i, 0] + "Frame"] = 0;
                }
                if (heroFrameSpeed[textureNames[i, 0] + "Count"] > textureSpeeds[i]) //speed 
                {
                    if (heroFrameSpeed["heroAttackHFrame"] >= 5)
                    {
                        heroFrameSpeed["heroAttackHFrame"] = 5;
                    }
                    else
                        heroFrameSpeed[textureNames[i, 0] + "Frame"]++;
                    heroFrameSpeed[textureNames[i, 0] + "Count"] = 0;
                }
            }
        }

        public String checkEnemyState(Enemy currEnemy)
        {
            enemyCurrState = Convert.ToString(currEnemy.currentState);
            //idle
            if (enemyCurrState == "KnockBack" ||
                enemyCurrState == "Idle")
            {
                enemyCurrState = "Move";
                return enemyCurrState;
            }
            else
            {
                enemyCurrState = "Attack";
                return enemyCurrState;
            }
        }

        public void initializeEnemyAnimation(List<Enemy> enemyList)
        {
            foreach (Enemy enemy in enemyList)
            {
                //ENEMY
                #region Queen
                if (enemy.enemyType == Enemy.EnemyType.QueenHearts)
                {
                    if (enemy.MoveUp)
                    {
                        enemy.enemyTextureCount = 0;
                        enemy.enemyModel.Link.Texture = queenMove[1];
                    }
                    else if (enemy.MoveDown)
                    {
                        enemy.enemyTextureCount = 0;
                        enemy.enemyModel.Link.Texture = queenMove[0];

                    }
                    else if (enemy.SpawnCards)
                    {
                        if (enemy.enemyTextureCount >= queenPoint.Count() - 1)
                        {
                            enemy.enemyTextureCount = 0; //reset counter
                        }
                        enemy.enemyModel.Link.Texture = queenPoint[enemy.enemyTextureCount];
                        enemyFrameCount++;
                    }
                    else if (enemy.ThrowHedgeHogs)
                    {
                        
                    }
                    else if (enemy.StunAttack)
                    {
                        if (enemy.enemyTextureCount >= queenStun.Count() - 1)
                        {
                            enemy.enemyTextureCount = 0; //reset counter
                        }
                        enemy.enemyModel.Link.Texture = queenStun[enemy.enemyTextureCount];
                        enemyFrameCount++;
                    }
                    else if (enemy.Stunned)
                    {
                        enemy.enemyTextureCount = 0;
                        enemy.enemyModel.Link.Texture = queenStunned[0];
                    }           
                }
                #endregion
                #region Ace Card
                if (enemy.enemyType == Enemy.EnemyType.AceCard)
                {
                    checkEnemyState(enemy); //check the state and put into one of the groups
                    if (enemyCurrState == "Move")
                    {
                        String tempCard = enemy.enemyModel.Link.Type;
                        enemy.enemyModel.Link.Texture = faceCards[tempCard.ToString() + "Idle0".ToString()];
                    }
                    if (enemyCurrState == "Attack")
                    {
                        String tempCard = enemy.enemyModel.Link.Type;
                        if (enemy.enemyTextureCount >= 2)
                        {
                            enemy.enemyTextureCount = 0;
                        }
                        enemy.enemyModel.Link.Texture = faceCards[tempCard.ToString() + "Attack".ToString() + enemy.enemyTextureCount.ToString()];
                        enemyFrameCount++;
                    }
                }
                #endregion
                #region Jack Card
                if (enemy.enemyType == Enemy.EnemyType.JackCard)
                {
                    checkEnemyState(enemy); //check the state and put into one of the groups
                    if (enemyCurrState == "Move")
                    {
                        String tempCard = enemy.enemyModel.Link.Type;
                        enemy.enemyModel.Link.Texture = faceCards[tempCard.ToString() + "Idle0".ToString()];
                    }
                    if (enemyCurrState == "Attack")
                    {
                        String tempCard = enemy.enemyModel.Link.Type;
                        if (enemy.enemyTextureCount >= 2)
                        {
                            enemy.enemyTextureCount = 0;
                        }
                        enemy.enemyModel.Link.Texture = faceCards[tempCard.ToString() + "Attack".ToString() + enemy.enemyTextureCount.ToString()];
                        enemyFrameCount++;
                    }
                }
                #endregion
                #region King Card
                if (enemy.enemyType == Enemy.EnemyType.KingHearts)
                {

                }
                #endregion
                #region Black Card
                if (enemy.enemyType == Enemy.EnemyType.BlackCard)
                {
                    checkEnemyState(enemy); //check the state and put into one of the groups
                    if (enemyCurrState == "Move")
                    {
                        String tempCard = enemy.enemyModel.Link.Type;
                        enemy.enemyModel.Link.Texture = regCards[tempCard.ToString() + "Idle0".ToString()];
                    }
                    if (enemyCurrState == "Attack")
                    {
                        String tempCard = enemy.enemyModel.Link.Type;
                        if (enemy.enemyTextureCount >= 2)
                        {
                            enemy.enemyTextureCount = 0;
                        }
                        enemy.enemyModel.Link.Texture = regCards[tempCard.ToString() + "Attack".ToString() + enemy.enemyTextureCount.ToString()];
                        enemyFrameCount++;
                    }
                }
                #endregion
                #region Red Card
                if (enemy.enemyType == Enemy.EnemyType.RedCard)
                {
                    checkEnemyState(enemy); //check the state and put into one of the groups
                    if (enemyCurrState == "Move")
                    {
                        String tempCard = enemy.enemyModel.Link.Type;
                        enemy.enemyModel.Link.Texture = regCards[tempCard.ToString() + "Idle0".ToString()];
                    }
                    if (enemyCurrState == "Attack")
                    {
                        String tempCard = enemy.enemyModel.Link.Type;
                        if (enemy.enemyTextureCount >= 2)
                        {
                            enemy.enemyTextureCount = 0;
                        }
                        enemy.enemyModel.Link.Texture = regCards[tempCard.ToString() + "Attack".ToString() + enemy.enemyTextureCount.ToString()];
                        enemyFrameCount++;
                    }
                }
                #endregion
                #region Worm
                if (enemy.enemyType == Enemy.EnemyType.AliceBookworm)
                {
                    checkEnemyState(enemy); //check the state and put into one of the groups
                    if (enemyCurrState == "Move")
                    {
                        if (enemy.enemyTextureCount >= wormMove.Count() - 1) //if enemy counter is at last element
                        {
                            enemy.enemyTextureCount = 0; //reset counter
                        }
                        enemy.enemyModel.Link.Texture = wormMove[enemy.enemyTextureCount];
                        enemyFrameCount++;
                    }
                    else if (enemyCurrState == "Attack")
                    {
                        if (enemy.enemyTextureCount >= wormAttack.Count() - 1)
                        {
                            enemy.enemyTextureCount = 0;
                        }
                        enemy.enemyModel.Link.Texture = wormAttack[enemy.enemyTextureCount];
                        enemyFrameCount++;
                    }
                }
                #endregion
                //NPC
                #region White Rabbit
                if (enemy.enemyType == Enemy.EnemyType.WhiteRabbit)
                {
                        if (enemy.currentState == Enemy.State.Idle)
                        {
                            enemyFrameCount = 0;
                            enemy.enemyModel.Link.Texture = rabbitIdle[0];
                            
                        }
                        else if (enemy.currentState == Enemy.State.Moving)
                        {
                            if (enemy.enemyTextureCount >= rabbitMove.Count() - 1) //if enemy counter is at last element                        
                            {
                                enemy.enemyTextureCount = 0; //reset counter
                            }
                            enemy.enemyModel.Link.Texture = rabbitMove[enemy.enemyTextureCount];
                            enemyFrameCount++;
                        }
                }
                #endregion
                #region Caterpillar
                if (enemy.enemyType == Enemy.EnemyType.Caterpillar)
                {
                        if (enemy.enemyTextureCount >= caterpillarMove.Count() - 1) //if enemy counter is at last element
                        {
                            enemy.enemyTextureCount = 0; //reset counter
                        }
                        enemy.enemyModel.Link.Texture = caterpillarMove[enemy.enemyTextureCount];
                        enemyFrameCount++;
                }
                #endregion
                #region Cat
                if (enemy.enemyType == Enemy.EnemyType.CheshireCat)
                {
                    if (enemy.Invisible)
                    {
                        enemy.enemyTextureCount = 0;
                        enemy.enemyModel.Link.Texture = catFadeIn[0];
                    }
                    else if (enemy.Talking)
                    {
                        if (enemy.enemyTextureCount >= catIdle.Count() - 1) //if enemy counter is at last element
                        {
                            enemy.enemyTextureCount = 0; //reset counter
                        }
                        enemy.enemyModel.Link.Texture = catIdle[enemy.enemyTextureCount];
                        enemyFrameCount++;

                    }
                    else if (enemy.FadeIn)
                    {
                        enemy.enemyModel.Link.Texture = catFadeIn[enemy.enemyTextureCount];
                        if (enemy.enemyTextureCount >= catFadeIn.Count() - 1) //if enemy counter is at last element
                        {
                            enemy.enemyTextureCount = 0; //reset counter
                            
                            isFadedIn = true;
                            mDialogue.Peek.DialogueContinue();
                            enemy.FadeIn = false;
                            //AliceLevel9
                        }
                        if (enemy.enemyTextureCount < catFadeIn.Count() - 1)
                        {
                            if (enemy.enemyTextureCount != catFadeIn.Count() - 1)
                            {
                                enemyFrameCount++;
                            }
                        }

                    }
                    else if (enemy.FadeOut)
                    {
                        if (changeState)
                        {
                            enemy.enemyTextureCount = catFadeOut.Count() - catFadeOut.Count();
                        }
                        if (enemy.enemyTextureCount >= catFadeOut.Count() - 1) //if enemy counter is at last element
                        {
                            enemy.enemyModel.Link.Texture = catFadeIn[0];
                        }

                        if (enemy.enemyTextureCount < catFadeOut.Count() - 1)
                        {
                            changeState = false;
                            enemy.enemyModel.Link.Texture = catFadeOut[enemy.enemyTextureCount];
                            enemyFrameCount++;
                        }
                    }                
                }
                #endregion
                #region Mad Hatter
                if (enemy.enemyType == Enemy.EnemyType.MadHatter)
                {
                    if (enemy.currentState == Enemy.State.Idle)
                    {
                        enemyFrameCount = 0;
                        enemy.enemyModel.Link.Texture = hatterIdle[0];
                    }
                    if (enemy.currentState == Enemy.State.Moving ||
                        enemy.currentState == Enemy.State.Attacking)
                    {
                        if (enemy.enemyTextureCount >= hatterMove.Count() - 1) //if enemy counter is at last element                        
                        {
                            enemy.enemyTextureCount = 0; //reset counter
                        }
                        enemy.enemyModel.Link.Texture = hatterMove[enemy.enemyTextureCount];
                        enemyFrameCount++;
                    }
                }
                #endregion

            }
        }

        public void Update(List<Enemy> enemyList)
        {
            enemyFrameCount++;
            if (enemyFrameCount > 8)
            {
                foreach (Enemy enemy in enemyList)
                {
                    enemy.enemyTextureCount += 1; //increase the counter of each enemy +1
                    initializeEnemyAnimation(enemyList); //call initialize enemy again to update each textures 
                    enemyFrameCount = 0; //reset timer
                }
            }
        }
    }
}




