using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inkwell.Framework.Graphics
{
    //David Fahr, January 15, 2010. 
    class mDamage
    {
        #region Damage Singleton
        private static volatile mDamage _instance;
        private static object _padlock = new Object();
        public static mDamage Peek
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
                            _instance = new mDamage(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        #endregion
        BasicModel damagePanel, critPanel;
        String damageCount, critCount;
        Texture2D[] normalDamage = new Texture2D[19];
        Texture2D[] critDamage = new Texture2D[19];
        List<BasicModel> damageList;
        List<BasicModel> critList;
        float panelSpeed;

        public void Load()
        {
            damageList = new List<BasicModel>();
            critList = new List<BasicModel>();
            panelSpeed = 0.4f;
            for (int i = 0; i < normalDamage.Count(); i++)
            {
                damageCount = i.ToString(); //convert damage passed to string
                normalDamage[i] = Engine.PersistantContainer.Load<Texture2D>("Textures\\Damage\\NormDamage\\" + damageCount.ToString());
            }

            for (int i = 0; i < critDamage.Count(); i++)
            {
                critCount = i.ToString(); //convert damage passed to string      
                critDamage[i] = Engine.PersistantContainer.Load<Texture2D>("Textures\\Damage\\CritDamage\\" + critCount.ToString());
            }
        }

        public void critInitialize(int damage, Vector3 position)
        {
            position.Y = mAvatar.Peek.PlayerModel.Link.Position.Y;
            position.Z *= -1;
            critPanel = new BasicModel(Engine.PersistantContainer, ModelProperties.Alpha, "Models\\Planes\\Plane4", position);
            critPanel.Link.Texture = critDamage[damage];
            critList.Add(critPanel);
        }

        public void damageInitialize(int damage, Vector3 position)
        {
            position.Y = mAvatar.Peek.PlayerModel.Link.Position.Y;
            position.Z *= -1; //fix positioning of z for damage panel
            damagePanel = new BasicModel(Engine.PersistantContainer, ModelProperties.Alpha, "Models\\Planes\\Plane4", position);
            damagePanel.Link.Texture = normalDamage[damage];
            damageList.Add(damagePanel);
        }

        public void Update()
        {
            for (int i = 0; i < damageList.Count; i++)
            {
                BasicModel t = damageList[i];
                t.Link.Position.Z += 0.01f;
                if (i % 2 != 0)
                {
                    t.Link.Position.Y += panelSpeed;
                    t.Link.Position.X += 0.2f;
                }
                else
                {
                    t.Link.Position.Y += panelSpeed;
                }

                if (t.Link.Position.Y > mAvatar.Peek.PlayerModel.Link.Position.Y + 25) //if panel is above a certain point
                {
                    BasicModel.Remove(t);
                    damageList.Remove(t);
                }
            }

            for (int i = 0; i < critList.Count; i++)
            {
                BasicModel t = critList[i];
                t.Link.Position.Z += 0.01f;
                if (t.Link.Position.Y <  mAvatar.Peek.PlayerModel.Link.Position.Y + 20)
                {
                    t.Link.Position.Y += 3f;
                    t.Link.Position.X += 1;
                }
                else if (t.Link.Position.Y >=  mAvatar.Peek.PlayerModel.Link.Position.Y + 20)
                {
                    t.Link.Position.Y += 0.1f;
                    t.Link.Position.X += 0.03f;
                }

                if (t.Link.Position.Y > mAvatar.Peek.PlayerModel.Link.Position.Y + 35)
                {
                    BasicModel.Remove(t);
                    critList.Remove(t);
                }
            }
        }       
    }
}
