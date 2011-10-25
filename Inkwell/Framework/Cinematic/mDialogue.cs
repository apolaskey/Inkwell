using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Inkwell.Framework
{
    public sealed class mDialogue
    {
        private static volatile mDialogue _instance;
        private static object _padlock = new Object();

        private Texture2D[] t2d_Texture = new Texture2D[15];
        private int intNumOfDialogues;
        private int intDialogueKey;

        public enum DialogueState {DialogueContinue, DialogueWait, DialogueStop};


        DialogueState DialogueWorkerState = DialogueState.DialogueStop;

        /// <summary>
        /// DialogueContinue:   Used for starting and continuing dialogue
        /// DialogueWait:       Wait for the player to hit any key
        /// DialogueStop:       Dialogue is not playing
        /// </summary>
        public DialogueState DialgoueWorkerState
        {
            get
            {
                return DialogueWorkerState;
            }
        }

        public int DialogueKey
        {
            get
            {
                return intDialogueKey;
            }
        }

        /// <summary>
        /// Use in dialogue trigger to kick start the dialogue sequence.
        /// </summary>
        public bool isDialogueStart
        {
            get
            {
                if (intDialogueKey == -1)
                    return true;
                else
                    return false;
            }
        }

        #region Load

        /// <summary>
        /// Loads the dialogue for level one Controls A
        /// </summary>
        public void LoadLevelOneA()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 4;

            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 1_Line 1");
            t2d_Texture[1] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 1_Line 1");
            t2d_Texture[2] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 1_Line 2");
            t2d_Texture[3] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 1_Line 3");
        }

        /// <summary>
        /// Loads the dialogue for level one Controls B
        /// </summary>
        public void LoadLevelOneB()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 4;

            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 1_Line 1");
            t2d_Texture[1] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 1_Line 1");
            t2d_Texture[2] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 1_Line 2");
            t2d_Texture[3] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 1_Line 3b");
        }

        /// <summary>
        /// Loads the dialogue for level two
        /// </summary>
        public void LoadLevelTwo()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 2;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 2_Line 1");
            t2d_Texture[1] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 2_Line 2");
        }

        /// <summary>
        /// Loads the dialogue for level three
        /// </summary>
        public void LoadLevelThree()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 2;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 3_Line 1");
            t2d_Texture[1] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 1");
            //t2d_Texture[2] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 2");
        }


        /// <summary>
        /// Loads the dialogue for level two (second visit)
        /// </summary>
        public void LoadLevelTwoTwo()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 0;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
        }


        /// <summary>
        /// Loads the dialogue for level three (second visit) Controls A
        /// </summary>
        public void LoadLevelThreeTwoA()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 10;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 3");
            t2d_Texture[1] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 3_Line 2");
            t2d_Texture[2] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 4");
            t2d_Texture[3] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 5");
            t2d_Texture[4] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 6");
            t2d_Texture[5] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 7");
            t2d_Texture[6] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 8");
            t2d_Texture[7] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 3_Line 3");
            t2d_Texture[8] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 9");
            t2d_Texture[9] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 3_Line 4");
        }

        /// <summary>
        /// Loads the dialogue for level three (second visit) Controls B
        /// </summary>
        public void LoadLevelThreeTwoB()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 10;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 3");
            t2d_Texture[1] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 3_Line 2");
            t2d_Texture[2] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 4b");
            t2d_Texture[3] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 5");
            t2d_Texture[4] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 6b");
            t2d_Texture[5] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 7b");
            t2d_Texture[6] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 8b");
            t2d_Texture[7] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 3_Line 3");
            t2d_Texture[8] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 3_Line 9");
            t2d_Texture[9] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 3_Line 4");
        }

        /// <summary>
        /// Loads the dialogue for level four
        /// </summary>
        public void LoadLevelFour()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 1;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 4_Line 1");
        }


        /// <summary>
        /// Loads the dialogue for level five
        /// </summary>
        public void LoadLevelFive()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 0;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
        }


        /// <summary>
        /// Loads the dialogue for level six
        /// </summary>
        public void LoadLevelSix()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 13;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 6_Line 1");
            t2d_Texture[1] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Caterpillar\\Caterpillar_Level 6_Line 1");
            t2d_Texture[2] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 6_Line 2");
            t2d_Texture[3] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Caterpillar\\Caterpillar_Level 6_Line 2");
            t2d_Texture[4] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 6_Line 3");
            t2d_Texture[5] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Caterpillar\\Caterpillar_Level 6_Line 3");
            t2d_Texture[6] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 6_Line 4");
            t2d_Texture[7] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Caterpillar\\Caterpillar_Level 6_Line 4");
            t2d_Texture[8] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 6_Line 5");
            t2d_Texture[9] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Caterpillar\\Caterpillar_Level 6_Line 5");
            t2d_Texture[10] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 6_Line 6");
            t2d_Texture[11] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Caterpillar\\Caterpillar_Level 6_Line 6");
            t2d_Texture[12] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 6_Line 7");
        }


        /// <summary>
        /// Loads the dialogue for level seven
        /// </summary>
        public void LoadLevelSeven()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 3;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 7_Line 1");
            t2d_Texture[1] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 7_Line 2");
            t2d_Texture[2] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 7_Line 1");
        }


        /// <summary>
        /// Loads the dialogue for level eight
        /// </summary>
        public void LoadLevelEight()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 11;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\White Rabbit\\White Rabbit_Level 8_Line 1");
            t2d_Texture[1] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 8_Line 1");
            t2d_Texture[2] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Mad Hatter\\Mad Hatter_Level 8_Line 1");
            t2d_Texture[3] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 8_Line 2");
            t2d_Texture[4] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Mad Hatter\\Mad Hatter_Level 8_Line 2");
            t2d_Texture[5] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 8_Line 3");
            t2d_Texture[6] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Mad Hatter\\Mad Hatter_Level 8_Line 3");
            t2d_Texture[7] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 8_Line 4");
            t2d_Texture[8] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Mad Hatter\\Mad Hatter_Level 8_Line 4");
            t2d_Texture[9] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Mad Hatter\\Mad Hatter_Level 8_Line 5");
            t2d_Texture[10] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 8_Line 5");
        }


        /// <summary>
        /// Loads the dialogue for level nine
        /// </summary>
        public void LoadLevelNine()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 8;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Cheshire Cat\\Cheshire Cat_Level 10_Line 1");
            t2d_Texture[1] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 10_Line 1");
            t2d_Texture[2] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Cheshire Cat\\Cheshire Cat_Level 10_Line 2");
            t2d_Texture[3] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 10_Line 2");
            t2d_Texture[4] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Cheshire Cat\\Cheshire Cat_Level 10_Line 3");
            t2d_Texture[5] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 10_Line 3");
            t2d_Texture[6] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Cheshire Cat\\Cheshire Cat_Level 10_Line 4");
            t2d_Texture[7] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 10_Line 4");
        }


        /// <summary>
        /// Loads the dialogue for level ten
        /// </summary>
        public void LoadLevelTen()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 1;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 9_Line 1");
        }


        /// <summary>
        /// Loads the dialogue for level eleven
        /// </summary>
        public void LoadLevelEleven()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 1;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 11_Line 1");
        }


        /// <summary>
        /// Loads the dialogue for level twelve
        /// </summary>
        public void LoadLevelTwelve()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 0;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
        }


        /// <summary>
        /// Loads the dialogue for level thirteen
        /// </summary>
        public void LoadLevelThirteen()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 1;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 13_Line 1");
        }


        /// <summary>
        /// Loads the dialogue for level fourteen
        /// </summary>
        public void LoadLevelFourteen()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 1;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 14_Line 1");
        }


        /// <summary>
        /// Loads the dialogue for level fifteen
        /// </summary>
        public void LoadLevelFifteen()
        {
            intDialogueKey = -1;
            intNumOfDialogues = 9;

            //t2d_Texture[] = Engine.GameContainer.Load<Texture2D>("");
            t2d_Texture[0] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 15_Line 1");
            t2d_Texture[1] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Queen\\Queen_Level 15_Line 1");
            t2d_Texture[2] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Queen\\Queen_Level 15_Line 2");
            t2d_Texture[3] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 15_Line 2");
            t2d_Texture[4] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Queen\\Queen_Level 15_Line 3");
            t2d_Texture[5] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 15_Line 3");
            t2d_Texture[6] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Queen\\Queen_Level 15_Line 4");
            t2d_Texture[7] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 15_Line 4");
            t2d_Texture[8] = Engine.CoreContainer.Load<Texture2D>("Textures\\Dialogue\\Robert Cain\\Robert Cain_Level 15_Line 5");
        }


        #endregion

        /// <summary>
        /// Use this to start and continue dialogue
        /// </summary>
        public void DialogueContinue()
        {
            DialogueWorkerState = DialogueState.DialogueContinue;
        }

        /// <summary>
        /// Use this to force stop dialogue
        /// </summary>
        public void DialogueStop()
        {
            DialogueWorkerState = DialogueState.DialogueStop;
        }

        /// <summary>
        /// Update for the Dialogue
        /// </summary>
        /// <param name="content"></param>
        public void Update()
        {
            if (DialogueWorkerState != DialogueState.DialogueStop)
            {
                switch (DialogueWorkerState)
                {
                    case DialogueState.DialogueContinue:
                        intDialogueKey++;
                        if (intDialogueKey < intNumOfDialogues)
                        {
                            DialogueWorkerState = DialogueState.DialogueWait;
                        }
                        else
                        {
                            mAvatar.Peek.Enable();
                            DialogueWorkerState = DialogueState.DialogueStop;
                        }
                        break;
                    case DialogueState.DialogueWait:
                        break;
                }
            }
        }

        /// <summary>
        /// Draws the dialogue.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (DialogueWorkerState != DialogueState.DialogueStop)
            {
                mGraphics.Peek.ToggleSpriteDraw();

                spriteBatch.Draw(t2d_Texture[intDialogueKey], Engine.TempVector2(640.0f, 720.0f - (t2d_Texture[intDialogueKey].Height / 2)), null, Color.White, 0.0f, Engine.TempVector2((float)t2d_Texture[intDialogueKey].Width / 2.0f, (float)t2d_Texture[intDialogueKey].Height / 2.0f), 1.0f, SpriteEffects.None, 0.0f);
              
                mGraphics.Peek.ToggleSpriteDraw();
            }
        }

        public static mDialogue Peek
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
                            _instance = new mDialogue(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
    }
}
