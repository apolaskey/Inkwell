//Author: Andrew A. Ernst

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Inkwell.Framework.Graphics;

namespace Inkwell.Framework
{
    class AliceLevel15 : cLevel
    {
        BasicModel castleCarpet;
        BasicModel[] temp;

        public override void Initialize()
        {
            BloomEffect.Settings = BloomSettings.PresetSettings[1];
            mAudio.Peek.LoadAllSounds();
            mAudio.Peek.PlaySound(mAudio.SoundName.PageTurn1);

            mAvatar.Peek.Load(Engine.TempVector3(10, 1, 55));
            mAvatar.Peek.SetBounds(-20, 720, 0, 200, -12, 117);

            castleCarpet = new BasicModel(Engine.GameContainer, ModelProperties.Opaque, "Models\\Levels\\Level15\\Castle_Carpet", Engine.TempVector3(350, 0, -55));
            castleCarpet.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Level Textures\\Level15\\RedCarpet");
            castleCarpet.Link.TileAmount.Y = 20f;

            temp = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XmlLevel15.xml");
        }
        public override void Update()
        {
            for (int i = 0; i < mAI.Peek.enemyList.Count; i++)
            {
                mAI.Peek.enemyList[i].SetBounds(-20, 720, 0, 200, -17, 145);
            }
            mAudio.Peek.Update();
            mAvatar.Peek.Update();
            mPhysics.Peek.Update(temp);
            mCamera.Peek.Update(mAvatar.Peek.PlayerModel.Link.Position);
            mAI.Peek.Update();
            if (mInput.Peek.IsKeyPressed(Keys.R))
            {
                mAvatar.Peek.FullRestore();
            }
            
        }
        public override void Draw()
        {

            mModel.Peek.Draw();
        }
        public override void Kill()
        {
            mAudio.Peek.Clear();
            base.Kill();
        }
    }
}