//Author: Andrew A. Ernst

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Inkwell.Framework
{
    class AliceLevel12 : cLevel
    {
        BasicModel[] temp;
        ExitTrigger Exit = new ExitTrigger();

        //Variables for switching battle levels
        int level = 1;
        bool spawned = true;
        bool levelCount = true;
        bool shut = true;

        public override void Initialize()
        {
            mAudio.Peek.LoadAllSounds();
            mAudio.Peek.PlaySound(mAudio.SoundName.PageTurn1);

            mAvatar.Peek.Load(Engine.TempVector3(0, 0, 0));
            mAvatar.Peek.SetBounds(-127, 1000, -1000, 1000, -130, 65);

            temp = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XmlLevel12.xml");
            BloomEffect.Settings = BloomSettings.PresetSettings[1];
            Exit.Initialize(930f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");
        }
        public override void Update()
        {
            
            mAudio.Peek.Update();
            mAvatar.Peek.Update();

            for (int i = 0; i < mAI.Peek.enemyList.Count; i++)
            {
                mAI.Peek.enemyList[i].SetBounds(-132, 132, -500, 500, -135, 70);
            }
           
            mPhysics.Peek.Update(temp);
            mCamera.Peek.Update(mAvatar.Peek.PlayerModel.Link.Position);
            mAI.Peek.Update();
            if (mAI.Peek.enemyList.Count == 0 && levelCount)
            {
                level++;
                spawned = false;
                levelCount = false;
                shut = false;
            }
            switch (level)
            {
                case 2:
                    Update2ndLevel();
                    break;
                case 3:
                    Update3rdLevel();
                    break;
                case 4:
                    Update4thLevel();
                    break;
            }
        }

        #region Update2ndLevel()
        private void Update2ndLevel()
        {
            for (int i = 0; i < mAI.Peek.enemyList.Count; i++)
            {
                mAI.Peek.enemyList[i].SetBounds(180, 444, -500, 500, -135, 70);
            }
            if (!spawned)
            {
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.AceCard, Engine.TempVector3(325, 55, 30));
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.RedCard, Engine.TempVector3(350, 55, 0));
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.BlackCard, Engine.TempVector3(375, 55, -30));
                spawned = true;
                levelCount = true;
            }
            if (mAvatar.Peek.PlayerModel.Link.Position.X >= 200)
            {
                shut = true;
                temp[0].Link.Position.Y += 2;
                if (temp[0].Link.Position.Y >= 0)
                {
                    temp[0].Link.Position.Y = 0;
                }
            }
            else if (!shut)
            {
                temp[0].Link.Position.Y--;
                if (temp[0].Link.Position.Y <= -90)
                {
                    temp[0].Link.Position.Y = -90;
                }
            }
        }
        #endregion

        #region Update3rdLevel()
        private void Update3rdLevel()
        {
            for (int i = 0; i < mAI.Peek.enemyList.Count; i++)
            {
                mAI.Peek.enemyList[i].SetBounds(492, 756, -500, 500, -135, 70);
            }
            if (!spawned)
            {
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.AceCard, Engine.TempVector3(600, 110, 10));
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.RedCard, Engine.TempVector3(600, 110, 5));
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.BlackCard, Engine.TempVector3(600, 110, -5));
                mAI.Peek.SpawnEnemy(Enemy.EnemyType.BlackCard, Engine.TempVector3(600, 110, -10));
                spawned = true;
                levelCount = true;
            }
            if (mAvatar.Peek.PlayerModel.Link.Position.X >= 520)
            {
                shut = true;
                temp[1].Link.Position.Y += 2;
                if (temp[1].Link.Position.Y >= 55)
                {
                    temp[1].Link.Position.Y = 55;
                }
            }
            else if (!shut)
            {
                temp[1].Link.Position.Y--;
                if (temp[1].Link.Position.Y <= -35)
                {
                    temp[1].Link.Position.Y = -35;
                }
            }
        }
        #endregion

        #region Update4thLevel()
        private void Update4thLevel()
        {
            if (Exit.Update(mAvatar.Peek.PlayerModel.Link.Position))
                mLevel.Peek.ChangeLevel(new AliceLevel14());

            if (mAvatar.Peek.PlayerModel.Link.Position.X >= 850)
            {
                shut = true;
                temp[2].Link.Position.Y += 2;
                if (temp[2].Link.Position.Y >= 110)
                {
                    temp[2].Link.Position.Y = 110;
                }
            }
            else if (!shut)
            {
                temp[2].Link.Position.Y--;
                if (temp[2].Link.Position.Y <= 20)
                {
                    temp[2].Link.Position.Y = 20;
                }
            }
        }
        #endregion

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
