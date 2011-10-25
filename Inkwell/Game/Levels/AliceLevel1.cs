using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Inkwell.Framework.Triggers;


namespace Inkwell.Framework
{
    class AliceLevel1 : cLevel
    {
        //BasicModel _bmTest;
        //BasicModel[] obstacles;
        //int numObstacles = 1;

        ExitTrigger Exit = new ExitTrigger();
        DialogueTrigger Trigger = new DialogueTrigger();

        BasicModel[] temp;

        public override void Initialize()
        {
            mAudio.Peek.LoadAllSounds();
            mAudio.Peek.PlaySound(mAudio.SoundName.PageTurn2);
            Trigger.Initialize(10);

            mAvatar.Peek.Load(Engine.TempVector3(0, 2, 0));
            mAvatar.Peek.SetBounds(-1,300,-500,200,-80,90);

            temp = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XmlLevel1.xml");

            Exit.Initialize(300f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");
            if (!mAvatar.Peek.FlipControls) //this is not right for some reason. Should be !
                mDialogue.Peek.LoadLevelOneA();
            else
                mDialogue.Peek.LoadLevelOneB();
        }

        public override void Update()
        {
            mAI.Peek.Update();
            mAudio.Peek.Update();
            mAvatar.Peek.Update();

            mPhysics.Peek.Update(temp);
            mCamera.Peek.Update(mAvatar.Peek.PlayerModel.Link.Position,50);
            
            if (mInput.Peek.IsAnyKeyDown() && mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueWait)
            {
                mDialogue.Peek.DialogueContinue();
            }

            if (mDialogue.Peek.DialogueKey == 2)
            {
                foreach (WhiteRabbit rabbit in mAI.Peek.enemyList)
                {
                    rabbit.targetPosition = Engine.TempVector3(500, 0, 0);
                    rabbit.currentState = Enemy.State.Moving;
                }
            }
            
            if (mInput.Peek.IsKeyPressed(Keys.PageUp))
            {
                mAudio.Peek.SoundVolume += .1f;
            }
            if (mInput.Peek.IsKeyPressed(Keys.PageDown))
            {
                mAudio.Peek.SoundVolume -= .1f;
            }

           

            if (Exit.Update(mAvatar.Peek.PlayerModel.Link.Position))
                mLevel.Peek.ChangeLevel(new AliceLevel2());

            if (Trigger.Update(mAvatar.Peek.PlayerModel.Link.Position))
                mDialogue.Peek.DialogueContinue();

            
            //mPhysics.Peek.Update(obstacles);

        }
        public override void Draw()
        {
            mModel.Peek.Draw();

            Exit.Draw();
        }
        public override void Kill()
        {
            mAudio.Peek.Clear();
           // mLevel.Peek.QueueNextLevel(new AliceLevel2(), null);
            base.Kill();
        }
    }

}