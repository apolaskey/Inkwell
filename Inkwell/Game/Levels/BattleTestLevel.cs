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
    class BattleTestLevel : cLevel
    {
        //BasicModel _bmTest;
        //BasicModel[] obstacles;
        //int numObstacles = 1;

        ExitTrigger Exit = new ExitTrigger();

        BasicModel[] temp;

        public override void Initialize()
        {
            mAudio.Peek.LoadAllSounds();
            mAudio.Peek.PlaySound(mAudio.SoundName.PageTurn2);

            mAvatar.Peek.Load(Engine.TempVector3(0, 2, 0));
            //mAvatar.Peek.SetBounds(-30, 200, -10, 100, -50, 200);
            //_bmTest = new BasicModel(Engine.GameContainer, ModelProperties.Opaque, "Models\\Vegetation Models\\Land", Vector3.Zero);
            //_bmTest.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Environment\\Ground\\Grass");
            //_bmTest.Link.TileAmount.X = 16f; //<-- Controls Tiling
            //_bmTest.Link.TileAmount.Y = 8f;
            //obstacles = new BasicModel[numObstacles];
            //obstacles[0] = _bmTest;

            temp = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XmlLevel10.xml");

            Exit.Initialize(700f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");

        }

        public override void Update()
        {
            mAudio.Peek.Update();
            mAvatar.Peek.Update();
            mCamera.Peek.Update(mAvatar.Peek.PlayerModel.Link.Position);
            mAI.Peek.Update();
            if (mInput.Peek.IsKeyPressed(Keys.E))
            {
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.AliceBookworm);
            }
            if (mInput.Peek.IsKeyPressed(Keys.R))
            {
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.BlackCard);
            }
            if (mInput.Peek.IsKeyPressed(Keys.T))
            {
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.MadHatter);
            }
            if (mInput.Peek.IsKeyPressed(Keys.Y))
            {
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.RedCard);
            }
            if (mInput.Peek.IsKeyPressed(Keys.U))
            {
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.QueenHearts);
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
                mLevel.Peek.ChangeLevel(new AliceLevel9());


            //mPhysics.Peek.Update(obstacles);

            mPhysics.Peek.Update(temp);
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