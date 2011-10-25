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
    class AliceLevel3 : cLevel
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
            Trigger.Initialize(-115);

            mAvatar.Peek.Load(Engine.TempVector3(-150, 2, 0));
            mAvatar.Peek.SetBounds(-201, -50, -500, 200, -80, 90);

            temp = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XmlLevel3.xml");

            Exit.Initialize(200f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");
            mDialogue.Peek.LoadLevelThree();
        }

        public override void Update()
        {
            mAudio.Peek.Update();
            mAvatar.Peek.Update();
            mPhysics.Peek.Update(temp);
            mCamera.Peek.Update(mAvatar.Peek.PlayerModel.Link.Position);
            mAI.Peek.Update();
            
            if (mInput.Peek.IsKeyPressed(Keys.PageUp))
            {
                mAudio.Peek.SoundVolume += .1f;
            }
            if (mInput.Peek.IsKeyPressed(Keys.PageDown))
            {
                mAudio.Peek.SoundVolume -= .1f;
            }
            if (mInput.Peek.IsAnyKeyDown() && mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueWait)
            {
                mDialogue.Peek.DialogueContinue();
            }
            if (mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueStop && mDialogue.Peek.DialogueKey > -1)
            {
                mLevel.Peek.ChangeLevel(new AliceLevel2_1());
                

                //for (int i = 0; i < mAI.Peek.enemyList.Count;i++)
                //{
                //    if (mAI.Peek.enemyList[i].enemyType == Enemy.EnemyType.WhiteRabbit)
                //    {
                //    mAI.Peek.enemyList[i].targetPosition = Engine.TempVector3(-55, 0, 0);
                //    mAI.Peek.enemyList[i].currentState = Enemy.State.Moving;
                //    }
                //}
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
            base.Kill();
        }
    }

}