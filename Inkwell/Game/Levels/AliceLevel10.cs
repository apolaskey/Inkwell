using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Inkwell.Framework.Triggers;
using Inkwell.Framework.Graphics;


namespace Inkwell.Framework
{
    class AliceLevel10 : cLevel
    {
        //BasicModel _bmTest;
        //BasicModel[] obstacles;
        //int numObstacles = 1;

        ExitTrigger Exit = new ExitTrigger();
        DialogueTrigger Dialogue = new DialogueTrigger();

        BasicModel[] temp;

        public override void Initialize()
        {
            mAudio.Peek.LoadAllSounds();
            mAudio.Peek.PlaySound(mAudio.SoundName.PageTurn2);

            mAvatar.Peek.Load(Engine.TempVector3(-40.0f, 25.0f, 0.0f));
            mAvatar.Peek.SetBounds(-41, 425, -500, 200, -80, 90);
       
            temp = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XmlLevel10.xml");


            Exit.Initialize(400f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");
            Dialogue.Initialize(20f);
            mDialogue.Peek.LoadLevelTen();
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

            if (mInput.Peek.IsAnyKeyDown() && (mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueWait))
            {
                mDialogue.Peek.DialogueContinue();
            }

            if (mAI.Peek.enemyList.Count == 0)
            {
                if (Exit.Update(mAvatar.Peek.PlayerModel.Link.Position))
                    mLevel.Peek.ChangeLevel(new AliceLevel11());
            }

            if (Dialogue.Update(mAvatar.Peek.PlayerModel.Link.Position) && mDialogue.Peek.isDialogueStart)
            {
                mDialogue.Peek.DialogueContinue();
            }
            //mPhysics.Peek.Update(obstacles);

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