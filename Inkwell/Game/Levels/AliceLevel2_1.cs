using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Inkwell.Framework
{
    class AliceLevel2_1 : cLevel
    {
        #region initializers

        BasicModel[] obstacles;
        int direction = 0;
        Vector3 startPosition = new Vector3(-40.0f, -119.5f, 0.0f);
        DialogueTrigger Dialogue = new DialogueTrigger();
        ExitTrigger Exit = new ExitTrigger();
        float clockRotation = 0.0f;

        #endregion

        public override void Initialize()
        {
            #region loads

            mAudio.Peek.LoadAllSounds();
            mAudio.Peek.PlaySound(mAudio.SoundName.PageTurn1);
            mAudio.Peek.LoadMusic("Level15", "Audio\\Music\\background2");
            mAvatar.Peek.Load(startPosition);
            mAvatar.Peek.SetBounds(-41, 925, -500, 200, -80, 150);

            mDialogue.Peek.LoadLevelTwo();

            Dialogue.Initialize(-35f);
            Exit.Initialize(925f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");
            obstacles = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XMLLevel2-1.xml");

            #endregion
        }
        public override void Update()
        {
            #region interaction

            clockRotation += .0075f;
            obstacles[35].Link.RotationMatrix = Matrix.CreateTranslation(Engine.TempVector3(-11.5f, 0.0f, 0.0f)) * Matrix.CreateRotationY(clockRotation);

            if (direction == 0)
            {
                obstacles[7].Link.Position.Z += .5f;
                if (obstacles[7].Link.Position.Z >= 60)
                {
                    direction = 1;
                }
                if (mPhysics.Peek.BoxCollision(mAvatar.Peek.PlayerModel, obstacles[7]) == 2)
                {
                    mAvatar.Peek.PlayerModel.Link.Position.Z += .5f;
                }
                if (mPhysics.Peek.BoxCollision(mAvatar.Peek.PlayerModel, obstacles[19]) > 0)
                {
                    obstacles[35].Link.Display = false;
                }
            }
            else
            {
                obstacles[7].Link.Position.Z -= .5f;
                if (obstacles[7].Link.Position.Z <= -60)
                {
                    direction = 0;
                }
                if (mPhysics.Peek.BoxCollision(mAvatar.Peek.PlayerModel, obstacles[7]) == 2)
                {
                    mAvatar.Peek.PlayerModel.Link.Position.Z -= .5f;
                }
            }
            if (mPhysics.Peek.BoxCollision(mAvatar.Peek.PlayerModel, obstacles[2]) > 0)
            {
                mAvatar.Peek.PlayerModel.Link.Position = startPosition;
                mAudio.Peek.PlaySound(mAudio.SoundName.PlayerDeath);
            }

            obstacles[2].Link.MoveTexture.Y -= .001f;
            obstacles[2].Link.MoveTexture.X -= .0003f;
            obstacles[3].Link.MoveTexture.X -= .00005f;

            #endregion

            mAudio.Peek.Update();
            mAvatar.Peek.Update();
            mPhysics.Peek.Update(obstacles);
            mCamera.Peek.Update(mAvatar.Peek.PlayerModel.Link.Position, 100);

            if (mInput.Peek.IsAnyKeyDown() && mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueWait)
            {
                mDialogue.Peek.DialogueContinue();
            }
            if (Dialogue.Update(mAvatar.Peek.PlayerModel.Link.Position))
            {
                mDialogue.Peek.DialogueContinue();
            }
            if (Exit.Update(mAvatar.Peek.PlayerModel.Link.Position))
                mLevel.Peek.ChangeLevel(new AliceLevel3());
        }
        public override void Draw()
        {
            mModel.Peek.Draw();
            Exit.Draw();
        }
        public override void Kill()
        {
            base.Kill();
        }
    }
}