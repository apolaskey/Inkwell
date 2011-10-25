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
    class AliceLevel11 : cLevel
    {
        #region initializers

        int direction = 0, direction2 = 1;
        float rockRotation = 0.0f;
        BasicModel[] obstacles;
        Vector3 startPosition = Engine.TempVector3(-76.0f, -36.0f, 80.0f);
        Vector3 secondStartPosition = Engine.TempVector3(244, 13, -140);
        float rockStartZ = -180;
        ExitTrigger Exit = new ExitTrigger();

        #endregion

        public override void Initialize()
        {
            #region loads
            Exit.Initialize(465f);
            Exit.LoadTexture("Loading Screens\\ExitLevel");
            mAudio.Peek.LoadAllSounds();
            mAudio.Peek.PlaySound(mAudio.SoundName.PageTurn1);
            mAudio.Peek.LoadMusic("Level15", "Audio\\Music\\background2");
            mAvatar.Peek.Load(startPosition);
            mAvatar.Peek.SetBounds(-90, 525, -200, 200, -190, 190);
            obstacles = mFile.Peek.XmlReaderLoadLevel("..\\..\\..\\Content\\Level XML\\XMLLevel11.xml");
            #endregion
        }
        public override void Update()
        {
            #region spike interaction

            if (direction == 0)
            {
                obstacles[31].Link.Position.Y += .2f;

                if (obstacles[31].Link.Position.Y >= 13)
                {
                    direction = 1;
                }
            }
            else
            {
                obstacles[31].Link.Position.Y -= .8f;

                if (obstacles[31].Link.Position.Y <= -24)
                {
                    direction = 0;
                }
            }
            if (direction2 == 0)
            {
                obstacles[37].Link.Position.Y -= 1.2f;

                if (obstacles[37].Link.Position.Y <= -24)
                {
                    direction2 = 1;
                }
            }
            else
            {
                obstacles[37].Link.Position.Y += .2f;

                if (obstacles[37].Link.Position.Y >= 13)
                {
                    direction2 = 0;
                }
            }

            if (mPhysics.Peek.BoolBoxCollision(mAvatar.Peek.PlayerModel, obstacles[31])
                || mPhysics.Peek.BoolBoxCollision(mAvatar.Peek.PlayerModel, obstacles[37]))
            {
                mAvatar.Peek.PlayerModel.Link.Position = secondStartPosition;
            }

            #endregion

            #region rock interaction

            rockRotation += .05f;

            obstacles[55].Link.RotationMatrix = Matrix.CreateRotationX(rockRotation);
            obstacles[56].Link.RotationMatrix = Matrix.CreateRotationX(rockRotation);
            obstacles[57].Link.RotationMatrix = Matrix.CreateRotationX(rockRotation);
            obstacles[59].Link.RotationMatrix = Matrix.CreateRotationX(rockRotation);
            obstacles[60].Link.RotationMatrix = Matrix.CreateRotationX(rockRotation);
            obstacles[61].Link.RotationMatrix = Matrix.CreateRotationX(rockRotation);
            obstacles[62].Link.RotationMatrix = Matrix.CreateRotationX(rockRotation);
            
            obstacles[56].Link.Position.Z += 0.5f;
            obstacles[57].Link.Position.Z += 0.5f;
            obstacles[55].Link.Position.Z += 0.5f;
            obstacles[59].Link.Position.Z += 0.5f;
            obstacles[60].Link.Position.Z += 0.5f;
            obstacles[61].Link.Position.Z += 0.5f;
            obstacles[62].Link.Position.Z += 0.5f;

            if (obstacles[56].Link.Position.Z > 115)
            {
                obstacles[56].Link.Position.Z = rockStartZ;
            }
            else if (obstacles[55].Link.Position.Z > 115)
            {
                obstacles[55].Link.Position.Z = rockStartZ;
            }
            else if (obstacles[57].Link.Position.Z > 115)
            {
                obstacles[57].Link.Position.Z = rockStartZ;
            }
            else if (obstacles[59].Link.Position.Z > 115)
            {
                obstacles[59].Link.Position.Z = rockStartZ;
            }
            else if (obstacles[60].Link.Position.Z > 115)
            {
                obstacles[60].Link.Position.Z = rockStartZ;
            }
            else if (obstacles[61].Link.Position.Z > 115)
            {
                obstacles[61].Link.Position.Z = rockStartZ;
            }
            else if (obstacles[62].Link.Position.Z > 115)
            {
                obstacles[62].Link.Position.Z = rockStartZ;
            }
            if (mPhysics.Peek.BoolBoxCollision(mAvatar.Peek.PlayerModel, obstacles[55]) || mPhysics.Peek.BoolBoxCollision(mAvatar.Peek.PlayerModel, obstacles[60]) ||
                mPhysics.Peek.BoolBoxCollision(mAvatar.Peek.PlayerModel, obstacles[56]) || mPhysics.Peek.BoolBoxCollision(mAvatar.Peek.PlayerModel, obstacles[61]) ||
                mPhysics.Peek.BoolBoxCollision(mAvatar.Peek.PlayerModel, obstacles[57]) || mPhysics.Peek.BoolBoxCollision(mAvatar.Peek.PlayerModel, obstacles[62]) ||
                mPhysics.Peek.BoolBoxCollision(mAvatar.Peek.PlayerModel, obstacles[59]))
            {
                mAvatar.Peek.PlayerModel.Link.Position = secondStartPosition;
            }
            #endregion

            #region fog interaction

            obstacles[66].Link.MoveTexture.X += .0001f;

            if (mPhysics.Peek.BoolBoxCollision(mAvatar.Peek.PlayerModel, obstacles[66]))
            {
                mAvatar.Peek.PlayerModel.Link.Position = startPosition;
                mAvatar.Peek.shadowBox.Link.Position = startPosition;
            }

            #endregion

            #region Updates

            mAudio.Peek.Update();
            mAvatar.Peek.Update();
            mPhysics.Peek.Update(obstacles);
            mCamera.Peek.Update(mAvatar.Peek.PlayerModel.Link.Position, 115.0f);

            if (Exit.Update(mAvatar.Peek.PlayerModel.Link.Position))
                mLevel.Peek.ChangeLevel(new AliceLevel12());

            #endregion
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