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
    class AliceLevel7 : cLevel
    {

        ExitTrigger Exit = new ExitTrigger();
        DialogueTrigger Dialogue = new DialogueTrigger();
        bool isStop;

        BasicModel sky;

        BasicModel[] temp;
        //int numObstacles = 11;

        public override void Initialize()
        {
            mAudio.Peek.LoadAllSounds();
            mAudio.Peek.PlaySound(mAudio.SoundName.PageTurn2);

            mAvatar.Peek.Load(Engine.TempVector3(-90, 2, 0));
            mAvatar.Peek.SetBounds(-80, 330, -500, 200, -80, 100);


            temp = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XmlLevel7.xml");

            sky = new BasicModel(Engine.GameContainer, ModelProperties.Opaque, "Models\\Levels\\Level 2\\sky", new Vector3(200.0f, 100.0f, 100.0f));
            sky.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Level Textures\\Level 2\\sky");

            //mAI.Peek.SpawnEnemy(Enemy.EnemyType.WhiteRabbit, Engine.TempVector3(-30, 0, 0));
       
            
            Exit.Initialize(320f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");
            Dialogue.Initialize(-40f);
            isStop = true;
            mDialogue.Peek.LoadLevelSeven();
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

            sky.Link.MoveTexture.X -= .00005f;


            if (mAI.Peek.enemyList.Count == 1)
            {
                if (Exit.Update(mAvatar.Peek.PlayerModel.Link.Position))
                {
                    mLevel.Peek.ChangeLevel(new AliceLevel8());
                }
            }

            if (mDialogue.Peek.DialogueKey == -1)
            {
                if (Dialogue.Update(mAvatar.Peek.PlayerModel.Link.Position))
                {
                    mDialogue.Peek.DialogueContinue();
                }
            }

            if (mDialogue.Peek.DialogueKey == 0)
            {
                //foreach (WhiteRabbit rabbit in mAI.Peek.enemyList)
                //{
                //    rabbit.targetPosition = Engine.TempVector3(300, 0, 0);
                //    rabbit.currentState = Enemy.State.Moving;
                //}
            }

            if (mInput.Peek.IsAnyKeyDown() && mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueWait)
            {
                if (mDialogue.Peek.DialogueKey == 0 && isStop)
                {
                    mDialogue.Peek.DialogueStop();
                    mAvatar.Peek.Enable();
                    Dialogue.MoveTrigger(275);
                    isStop = false;
                }
                else
                    mDialogue.Peek.DialogueContinue();
            }

            if (mAI.Peek.enemyList.Count == 1)
            {
                if (Dialogue.Update(mAvatar.Peek.PlayerModel.Link.Position))
                {
                    mDialogue.Peek.DialogueContinue();
                }
            }

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