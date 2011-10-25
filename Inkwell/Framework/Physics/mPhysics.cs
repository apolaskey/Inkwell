using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Inkwell.Framework
{
    /// <summary>
    /// (Singleton) In charge of holding hooks to every collidable object and only caring for ones near the target.
    /// </summary>
    class mPhysics
    {
        private static volatile mPhysics _instance;
        private static object _padlock = new Object();
        private BoundingBox gameBox = new BoundingBox();
        private BoundingBox newBox = new BoundingBox();
        private int collide, collision;
        public float gravity;
        public bool jump, Top;
        DebugMessage dmCollision = DebugMessage.Initialize(true, Color.White, true, "Unable to track box collision! (Not in game?)");
        DebugMessage dmCollision2 = DebugMessage.Initialize(true, Color.White, true, "Unable to track obstacle collision! (Not in game?)");
        private BasicModel[] obstacles; //Andrew Ernst

        public void Load(BasicModel[] _obstacles)
        {
            collide = 0;
            gravity = 2.0f;
            jump = false;
            collision = 0;
        }

        #region gets and sets
        public float GetGravity()
        {
            return gravity;
        }
        public bool GetJump()
        {
            return jump;
        }
        public void SetJump(bool value)
        {
            jump = value;
        }
        public void SetGravity(float grav)
        {
            gravity = grav;
        }
        #endregion

        //Andrew Ernst
        public BasicModel[] Obstacles()
        {
            return obstacles;
        }

        public void Update(BasicModel[] obstacles)
        {
            this.obstacles = obstacles;
            gravity -= 0.05f;

            if (gravity > 2.0f)
            {
                gravity = 2.0f;
            }

            Top = false;

            for (int i = 0; i < obstacles.Length; i++)
            {
                collide = mPhysics.Peek.BoxCollision(mAvatar.Peek.PlayerModel, obstacles[i]);


                switch (collide)
                {
                    case 1: //underneath
                        mAvatar.Peek.PlayerModel.Link.Position.Y = mAvatar.Peek.PlayerModel.Link.OldPosition.Y;
                        gravity = 0.0f;
                        break;
                    case 2: //on top
                        //mAvatar.Peek.PlayerModel.Link.OldPosition.Y = mAvatar.Peek.PlayerModel.Link.BoundingBox.Min.Y + mAvatar.Peek.PlayerModel.Link.Position.Y;
                        mAvatar.Peek.PlayerModel.Link.OldPosition.Y = obstacles[i].Link.Position.Y + obstacles[i].Link.BoundingBox.Max.Y;
                        mAvatar.Peek.PlayerModel.Link.Position.Y = mAvatar.Peek.PlayerModel.Link.OldPosition.Y;
                        gravity = 0.0f;
                        Top = true;
                        jump = false;
                        break;
                    case 3:
                            mAvatar.Peek.PlayerModel.Link.Position.X = mAvatar.Peek.PlayerModel.Link.OldPosition.X;
                        break;
                    case 4:
                            mAvatar.Peek.PlayerModel.Link.Position.Z = mAvatar.Peek.PlayerModel.Link.OldPosition.Z;
                        break;
                    case 5:
                            mAvatar.Peek.PlayerModel.Link.Position.X = mAvatar.Peek.PlayerModel.Link.OldPosition.X;
                        break;
                    case 6:
                            mAvatar.Peek.PlayerModel.Link.Position.Z = mAvatar.Peek.PlayerModel.Link.OldPosition.Z;
                        break;
                }
            }
            if (!Top)
            {
                jump = true;
            }
            collide = 0;
            collision = 0;
            for (int i = 0; i < obstacles.Length; i++)
            {
                collide = mPhysics.Peek.BoxCollision(mAvatar.Peek.shadowBox, obstacles[i]);

                if (collide > 1)
                {
                    collision++;
                    mAvatar.Peek.shadowBox.Link.Position.Y = obstacles[i].Link.BoundingBox.Max.Y + obstacles[i].Link.Position.Y + 0.5f;
                    mAvatar.Peek.shadowBox.Link.Display = true;
                }
            }
            if ((mAvatar.Peek.shadowBox.Link.Position.Y - 3) > mAvatar.Peek.PlayerModel.Link.Position.Y)
            {
                mAvatar.Peek.shadowBox.Link.Position.Y = mAvatar.Peek.PlayerModel.Link.Position.Y + mAvatar.Peek.PlayerModel.Link.BoundingBox.Min.Y + .5f;
            }
            if (collision == 0)
            {
                mAvatar.Peek.shadowBox.Link.Display = false;
            }
        }
        /// <summary>
        /// (BasicModel Version) Determine whether two models are touching each other.
        /// </summary>
        /// <param name="mdl1">First Model</param>
        /// <param name="mdl2">Second Model</param>
        /// <returns>A true or false on whether they have collided.</returns>
        public int BoxCollision(BasicModel mdl1, BasicModel mdl2)
        {
            gameBox.Min = mdl1.Link.Position + mdl1.Link.BoundingBox.Min;
            gameBox.Max = mdl1.Link.Position + mdl1.Link.BoundingBox.Max;

            newBox.Min = mdl2.Link.Position + mdl2.Link.BoundingBox.Min;
            newBox.Max = mdl2.Link.Position + mdl2.Link.BoundingBox.Max;

            int temp = 0;

            if (gameBox.Intersects(newBox))
            {
                /*Under Facing Collision*/
                if ((mdl1.Link.OldPosition.Y + mdl1.Link.BoundingBox.Max.Y) < newBox.Min.Y)
                {
                    if (mdl2.Link.Display)
                        temp = 1;
                    //return 1;
                }
                /*Over Facing Collision*/
                if ((mdl1.Link.OldPosition.Y + mdl1.Link.BoundingBox.Max.Y) > newBox.Max.Y)
                {
                    if (mdl2.Link.Display)
                        temp = 2;
                    //return 2;
                }
                /*Left Facing Collision*/
                if ((mdl1.Link.OldPosition.X + mdl1.Link.BoundingBox.Max.X) < newBox.Min.X)
                {
                    //mdl2.Position += new Vector3(1, 0, 0);
                    if (mdl2.Link.Display)
                        temp = 3;
                    //return 3;
                }
                /*Front Facing Collision*/
                if ((mdl1.Link.OldPosition.Z + mdl1.Link.BoundingBox.Min.Z) > newBox.Max.Z)
                {
                    //mdl2.Position += new Vector3(0, 0, -1);
                    if (mdl2.Link.Display)
                        temp = 4;
                    //return 4;
                }
                /*Right Facing Collision*/
                if ((mdl1.Link.OldPosition.X + mdl1.Link.BoundingBox.Min.X) > newBox.Max.X)
                {
                    //mdl2.Position += new Vector3(-1, 0, 0);
                    if (mdl2.Link.Display)
                        temp = 5;
                    //return 5;
                }
                /*Behind Facing Collision*/
                if ((mdl1.Link.OldPosition.Z + mdl1.Link.BoundingBox.Max.Z) < newBox.Min.Z)
                {
                    //mdl2.Position += new Vector3(0, 0, 1);
                    if (mdl2.Link.Display)
                        temp = 6;
                    //return 6;
                }
                /*Debug Information Hook*/
                if (Engine.DebugEnabled)
                {
                    switch (temp)
                    {
                        case 0: dmCollision2.Text = "Obstacle Collision: Nothing";
                            break;
                        case 1: dmCollision2.Text = "Obstacle Collision: Under Obstacle " + mdl2.ID;
                            break;
                        case 2: dmCollision2.Text = "Obstacle Collision: Over Obstacle " + mdl2.ID;
                            break;
                        case 3: dmCollision2.Text = "Obstacle Collision: Left of Obstacle " + mdl2.ID;
                            break;
                        case 4: dmCollision2.Text = "Obstacle Collision: Front of Obstacle " + mdl2.ID;
                            break;
                        case 5: dmCollision2.Text = "Obstacle Collision: Right of Obstacle " + mdl2.ID;
                            break;
                        case 6: dmCollision2.Text = "Obstacle Collision: Behind Obstacle " + mdl2.ID;
                            break;
                    }
                }
                /*End of Debug Hook*/

                #region push objects

                if (mdl2.Link.IsMovable)
                {
                    switch (temp)
                    {
                        case 3:
                            PushObject(mdl2, 1);
                            break;
                        case 4:
                            PushObject(mdl2, 4);
                            break;
                        case 5:
                            PushObject(mdl2, 2);
                            break;
                        case 6:
                            PushObject(mdl2, 3);
                            break;
                    }
                }

                #endregion
            }
            return temp;
        }
        public void PushObject(BasicModel obj, int direction)
        {
            switch (direction)
            {

                case 1:
                    obj.Link.Position.X += mAvatar.Peek.WalkSpeed;
                    break;
                case 2:
                    obj.Link.Position.X -= mAvatar.Peek.WalkSpeed;
                    break;
                case 3:
                    obj.Link.Position.Z += mAvatar.Peek.WalkSpeed;
                    break;
                case 4:
                    obj.Link.Position.Z -= mAvatar.Peek.WalkSpeed;
                    break;
            }
        }
        public bool BoolBoxCollision(BasicModel mdl1, BasicModel mdl2)
        {
            gameBox.Min = mdl1.Link.Position + mdl1.Link.BoundingBox.Min;
            gameBox.Max = mdl1.Link.Position + mdl1.Link.BoundingBox.Max;

            newBox.Min = mdl2.Link.Position + mdl2.Link.BoundingBox.Min;
            newBox.Max = mdl2.Link.Position + mdl2.Link.BoundingBox.Max;

            bool temp = false;

            if (gameBox.Intersects(newBox))
            {
                if (mdl2.Link.Display)
                    temp = true;

                #region Collision Debug (Polaskey)
                if (Engine.DebugEnabled)
                {
                    if (temp)
                        dmCollision.Text = "Box Collision has occured with Model" + mdl2.ID;
                    else
                        dmCollision.Text = "Box Collision has not occured with Model" + mdl2.ID;
                }
                #endregion
            }
            return temp;
        }
        /// <summary>
        /// Get Access to many common use physics functions.
        /// </summary>
        public static mPhysics Peek
        {
            get
            {
                /*Check to see if we already initialized our component*/
                if (_instance == null)
                {
                    /*Lock it so another thread cant check it*/
                    lock (_padlock)
                    {
                        /*Check one more time just to be extra careful*/
                        if (_instance == null)
                            _instance = new mPhysics(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
    }
}
//<EOC>