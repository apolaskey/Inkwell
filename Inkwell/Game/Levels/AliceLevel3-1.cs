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
    class AliceLevel3_1 : cLevel
    {

        ExitTrigger Exit = new ExitTrigger();
        DialogueTrigger Trigger = new DialogueTrigger();

        BasicModel[] temp;

        public override void Initialize()
        {
            mAudio.Peek.LoadAllSounds();
            mAudio.Peek.PlaySound(mAudio.SoundName.PageTurn2);
            Trigger.Initialize(-180);

            mAvatar.Peek.Load(Engine.TempVector3(-200, 2, 0));
            mAvatar.Peek.SetBounds(-201, 400, -500, 200, -48, 90);

            temp = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XmlLevel3-1.xml");

            Exit.Initialize(375f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");
            if (!mAvatar.Peek.FlipControls)
                mDialogue.Peek.LoadLevelThreeTwoA();
            else
                mDialogue.Peek.LoadLevelThreeTwoB();
        }

        public override void Update()
        {
            mAI.Peek.Update();
            mAudio.Peek.Update();
            mAvatar.Peek.Update();

            mPhysics.Peek.Update(temp);
            mCamera.Peek.Update(mAvatar.Peek.PlayerModel.Link.Position);
           

            if (mInput.Peek.IsKeyPressed(Keys.PageUp))
            {
                mAudio.Peek.SoundVolume += .1f;
            }
            if (mInput.Peek.IsKeyPressed(Keys.PageDown))
            {
                mAudio.Peek.SoundVolume -= .1f;
            }

            if (mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueWait)
            {
                if (mInput.Peek.IsAnyKeyDown())
                    mDialogue.Peek.DialogueContinue();
            }

            if (mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueStop && mDialogue.Peek.DialogueKey > -1)
            {
                for (int i = 0; i < mAI.Peek.enemyList.Count; i++)
                {
                    if (mAI.Peek.enemyList[i].enemyType == Enemy.EnemyType.WhiteRabbit)
                    {
                        mAI.Peek.enemyList[i].targetPosition = Engine.TempVector3(500, 0, 0);
                        mAI.Peek.enemyList[i].currentState = Enemy.State.Moving;
                    }
                }
            }

           

            if (Exit.Update(mAvatar.Peek.PlayerModel.Link.Position))
                mLevel.Peek.ChangeLevel(new AliceLevel4());

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
            //mLevel.Peek.QueueNextLevel(new AliceLevel2(), null);
            base.Kill();
        }
    }

}