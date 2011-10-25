using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Inkwell.Framework.Triggers;
using Inkwell.Framework.Graphics;
using Inkwell.Framework.Graphics.Data;


namespace Inkwell.Framework
{
    class AliceLevel9 : cLevel
    {
        //BasicModel _bmTest;
        //BasicModel[] obstacles;
        //int numObstacles = 1;

        ExitTrigger Exit = new ExitTrigger();
        DialogueTrigger Dialogue = new DialogueTrigger();
        DialogueTrigger Dialogue2 = new DialogueTrigger();
        bool isStop;


        BasicModel[] temp;

        public override void Initialize()
        {
            mAudio.Peek.LoadAllSounds();
            mAudio.Peek.PlaySound(mAudio.SoundName.PageTurn2);

            mAvatar.Peek.Load(Engine.TempVector3(-40.0f, 25.0f, 0.0f));
            mAvatar.Peek.SetBounds(-41, 425, -500, 200, -80, 90);

            temp = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XmlLevel9.xml");

            Exit.Initialize(400f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");
            Dialogue.Initialize(380f);
            //Dialogue2.Initialize(180f);
            //isStop = true;
            mDialogue.Peek.LoadLevelNine();
        }

        public override void Update()
        {
            mAudio.Peek.Update();
            mAvatar.Peek.Update();
            mCamera.Peek.Update(mAvatar.Peek.PlayerModel.Link.Position);
            if (mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueStop)
            {
                mAI.Peek.Update();
            }
            
            if (mInput.Peek.IsKeyPressed(Keys.PageUp))
            {
                mAudio.Peek.SoundVolume += .1f;
            }
            if (mInput.Peek.IsKeyPressed(Keys.PageDown))
            {
                mAudio.Peek.SoundVolume -= .1f;
            }

            mPhysics.Peek.Update(temp);


            if (mInput.Peek.IsAnyKeyDown() && mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueWait)
            {
                //if ((mDialogue.Peek.DialogueKey == 3) && isStop)
                //{
                //    mDialogue.Peek.DialogueStop();
                //    mAvatar.Peek.Enable();
                //    Dialogue.MoveTrigger(380);
                //    isStop = false;
                //}
                //else
                mDialogue.Peek.DialogueContinue();
            }

            if (mDialogue.Peek.DialogueKey == 8)
            {
                //foreach (CheshireCat cat in mAI.Peek.enemyList)
                //{
                //    cat.currentSubState = CheshireCat.subState.FadeOut;
                //}
            }

            if (mAI.Peek.enemyList.Count == 1)
            {
                if (Exit.Update(mAvatar.Peek.PlayerModel.Link.Position))
                {
                    mLevel.Peek.ChangeLevel(new AliceLevel10());
                    mFile.Peek.SaveGame(mMenu.Peek.LoadSlot, 9);
                }
            }

            if (Dialogue.Update(mAvatar.Peek.PlayerModel.Link.Position))
            {
                mAI.Peek.enemyList[0].Invisible = false;
                //if (mAnimation.Peek.isFadedIn)
                //{
                //    mDialogue.Peek.DialogueContinue();
                //}
            }
            //mPhysics.Peek.Update(obstacles);

            if (mDialogue.Peek.DialogueKey == -1)
            {
                mAI.Peek.enemyList[0].Talking = false;
            }

            temp[0].Link.MoveTexture.X -= .00005f;
        }
        public override void Draw()
        {
      
            mModel.Peek.Draw();
            Exit.Draw();
        }
        public override void Kill()
        {
            mAudio.Peek.Clear();
            //mLevel.Peek.QueueNextLevel(new AliceLevel2(), null);
            base.Kill();
        }
    }

}