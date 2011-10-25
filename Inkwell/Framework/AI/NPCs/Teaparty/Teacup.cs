//Author: Andrew A. Ernst

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
#endregion

namespace Inkwell.Framework
{
    public class Teacup
    {
        private Vector3 teacupVector;
        private float DistanceFromPlayer;
        private BasicModel teacupModel;
        private float thrownSpeed;
        public bool dead = false;
        public bool directThrow;

        public void Initialize(Vector3 startPosition, bool directThrow)
        {
            teacupModel = new BasicModel(Engine.GameContainer, ModelProperties.Opaque, "Models/Levels/Level8/teacup1", startPosition);
            //teacupModel.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures/");
            thrownSpeed = Engine.Randomize(1.5f, 2.0f);
            DetermineVector();
        }
        private void DetermineVector()
        {
            teacupVector = Engine.TempVector3(mAvatar.Peek.PlayerModel.Link.Position.X, mAvatar.Peek.PlayerModel.Link.Position.Y + 5, mAvatar.Peek.PlayerModel.Link.Position.Z) - Engine.TempVector3(teacupModel.Link.Position.X, teacupModel.Link.Position.Y, teacupModel.Link.Position.Z);
            teacupVector.Normalize();
            teacupVector *= thrownSpeed;
        }
        public void Update()
        {
            if (directThrow)
                teacupModel.Link.Position += teacupVector;
            else
            {
                teacupModel.Link.Position.X += teacupVector.X;
                teacupModel.Link.Position.Z += teacupVector.Z;
            }

            DistanceFromPlayer = Vector3.Distance(teacupModel.Link.Position, mAvatar.Peek.PlayerModel.Link.Position);

            if (DistanceFromPlayer <= 10.0f)
            {
                mAvatar.Peek.HitPlayer(teacupModel.Link.Position.X, 5);
                dead = true;
            }

            if (DistanceFromPlayer >= 600.0f)
                dead = true;
        }

        public void Clear()
        {
            BasicModel.Remove(teacupModel);
        }
    }
}
