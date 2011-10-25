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
    class AliceLevel8 : cLevel
    {
        //BasicModel _bmTest;
        //BasicModel[] obstacles;
        //int numObstacles = 1;

        ExitTrigger Exit = new ExitTrigger();
        //DialogueTrigger trigger = new DialogueTrigger();

        BasicModel[] temp;

        public override void Initialize()
        {
            //trigger.Initialize(0);
            mAudio.Peek.LoadAllSounds();
            mAudio.Peek.PlaySound(mAudio.SoundName.PageTurn2);
            
            mAvatar.Peek.Load(Engine.TempVector3(-300, -48, 0));
            mAvatar.Peek.SetBounds(-325, 400, -60, 140, -20, 30);
            //mAvatar.Peek.SetBounds(-30, 200, -176, 600, -50, 176);
            

            temp = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XmlLevel8.xml");

            Exit.Initialize(395f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");

        }

        public override void Update()
        {
            mAudio.Peek.Update();
            mAvatar.Peek.Update();
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

            mPhysics.Peek.Update(temp);

            if (Exit.Update(mAvatar.Peek.PlayerModel.Link.Position))
                mLevel.Peek.ChangeLevel(new AliceLevel9());
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