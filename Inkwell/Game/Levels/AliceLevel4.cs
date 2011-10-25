using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Inkwell.Framework.Particle;
using Inkwell.Framework.Triggers;

namespace Inkwell.Framework
{
    class AliceLevel4 : cLevel
    {
        BasicModel[] obstacles;
        ExitTrigger Exit = new ExitTrigger();
        DialogueTrigger Dialogue = new DialogueTrigger();
        public override void Initialize()
        {

            mAvatar.Peek.Load(Engine.TempVector3(-145.0f, 20.0f, 0.0f));

            Exit.Initialize(380f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");
            Dialogue.Initialize(75);
            mDialogue.Peek.LoadLevelFour();
            mAvatar.Peek.SetBounds(-150, 390, -500, 200, -120, 100);
            obstacles = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XmlLevel4.xml");
            mAudio.Peek.LoadAllMusic();
            mAudio.Peek.LoadAllSounds();
            base.Initialize();
        }
        public override void Update()
        {
            mAI.Peek.Update();
            mAudio.Peek.Update();
            mAvatar.Peek.Update();

            if (mInput.Peek.IsKeyDown(Keys.R))
            {
                mLevel.Peek.ReloadLevel();
            }
            
            
            mPhysics.Peek.Update(obstacles);
            
            mDialogue.Peek.Update();

            if (mInput.Peek.IsAnyKeyDown() && mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueWait)
            {
                mDialogue.Peek.DialogueContinue();
            }

            /*How to Update the Exit Trigger*/
            if (Exit.Update(mAvatar.Peek.PlayerModel.Link.Position))
                mLevel.Peek.ChangeLevel(new AliceLevel5());
            if (Dialogue.Update(mAvatar.Peek.PlayerModel.Link.Position))
                mDialogue.Peek.DialogueContinue();

            mCamera.Peek.Update(mAvatar.Peek.PlayerModel.Link.Position,100);
            
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
