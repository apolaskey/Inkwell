using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Storage;


namespace Inkwell.Framework
{


    /// <summary>
    /// (Singleton) In Charge of loading files --Currently haven't though out the design process on this.
    /// </summary>
    class mFile
    {
        public List<BasicModel> StaticItems = new List<BasicModel>();
        private static volatile mFile _instance;
        private static object _padlock = new Object();

        #region Get Attribute Statements
        //String ID = xmlReader.GetAttribute("ID");
        //String pos = xmlReader.GetAttribute("position");
        //String rot = xmlReader.GetAttribute("rotation");
        //String alph = xmlReader.GetAttribute("alpha");
        //String ambi = xmlReader.GetAttribute("ambientColor");
        //String diffu = xmlReader.GetAttribute("diffuseColor");
        //String scal = xmlReader.GetAttribute("scalar");
        //String strLocModel = xmlReader.GetAttribute("locationModel");
        //String strLocTexture = xmlReader.GetAttribute("locationTexture");
        //String props = xmlReader.GetAttribute("modelProps");
        //
        //Vector3 position = ReadFloat3(pos);
        //Vector3 rotation = ReadFloat3(rot);
        //Vector4 abmient = ReadFloat4(ambi);
        //Vector4 diffuse = ReadFloat4(diffu);
        //float alpha = ReadFloat(alph);
        //float scalar = ReadFloat(scal);
        //ModelProperties properties = (ModelProperties)Enum.Parse(typeof(ModelProperties), props);
        #endregion

        public static mFile Peek
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
                            _instance = new mFile(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }

        //VARIABLES 
        //private XmlWriter xmlWriter;
        //private XmlReader xmlReader;
        private XmlDocument xmlLevelDialog = new XmlDocument();
        private XmlElement xmlDialogElement;
        private XmlNodeList xmlDialogNodeList;
        private float previousGameTime = 0.0f;
        private TimerData startTime = new TimerData();

        private String pos;
        private String rot;
        private String scal;
        private String strLocModel;
        private String strLocTexture;
        private String props;
        private String tile;
        private String flip;
        private String diffEn;

        private Vector3 position;
        private Vector3 rotation;
        private float scalar;
        private ModelProperties properties;
        private Vector2 tileAmount;
        private Vector2 flipVec;
        private BasicModel a;

        private String ET; 

        private Enemy.EnemyType EnemyType;




        public void loadLevelDialog(String strFileLocation)
        {
            xmlLevelDialog.Load(strFileLocation);
            xmlDialogElement = xmlLevelDialog.DocumentElement;

        }

        public String retrieveDialog(String strCharacter, String strLine)
        {
            xmlDialogNodeList = xmlDialogElement.SelectNodes(strCharacter);
            foreach (XmlNode xmlDialogNode in xmlDialogNodeList)
            {
                return xmlDialogNode[strLine].InnerText;

            }

            return "Error in " + strCharacter + "on line " + strLine;
        }


        //A lot of this is experimental code. Will be able to test 
        //it on the next merge and clean it up afterwards. Needs some cleaning up, will 
        //go into different classes. 
        public BasicModel[] XmlReaderLoadLevel(string level)
        {
            string newLevel = level;
            XmlTextReader xmlReader;

            this.StaticItems.Clear();

            xmlReader = new XmlTextReader(newLevel);
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            String asset = xmlReader.Name;
                            switch (xmlReader.Name)
                            {
                                case "doodad":
                                    {
                                        /* Example: <doodad position ="0 0 0" rotation="0 0 0" scalar="1" 
                                         diffuseEnabled="true" locationModel="Models\" locationTexture="Textures\" 
                                         modelProps="Opaque" tileVec="1 1" flipVec="1 1" /> */
                                         pos = xmlReader.GetAttribute("position");
                                         rot = xmlReader.GetAttribute("rotation");
                                         scal = xmlReader.GetAttribute("scalar");
                                         strLocModel = xmlReader.GetAttribute("locationModel");
                                         strLocTexture = xmlReader.GetAttribute("locationTexture");
                                         props = xmlReader.GetAttribute("modelProps");
                                         tile = xmlReader.GetAttribute("tileVec");
                                         flip = xmlReader.GetAttribute("flipVec");
                                         diffEn = xmlReader.GetAttribute("diffuseEnabled"); 

                                         position = ReadFloat3(pos);
                                         rotation = ReadFloat3(rot);
                                        
                                         scalar = ReadFloat(scal);
                                         properties = (ModelProperties)Enum.Parse(typeof(ModelProperties), props);
                                         tileAmount = ReadFloat2(tile);
                                         flipVec = ReadFloat2(flip);
                                         a = new BasicModel(Engine.GameContainer, properties, strLocModel, position);
                                         a.Link.Texture = Engine.GameContainer.Load<Texture2D>(strLocTexture);
                                         a.Link.DiffuseEnabled = Convert.ToBoolean(diffEn);

                                         if (a.Link.DiffuseEnabled)
                                         {
                                             a.Link.DiffuseIntensity = 0.8f;
                                             a.Link.AmbientIntensity = 0.4f;
                                             a.Link.AmbientColor = Color.White.ToVector4();
                                         }

                                        a.Link.TileAmount = tileAmount;
                                        a.Link.RotationMatrix *= Matrix.CreateRotationX(rotation.X);
                                        a.Link.RotationMatrix *= Matrix.CreateRotationY(rotation.Y);
                                        a.Link.RotationMatrix *= Matrix.CreateRotationZ(rotation.Z);
                                        a.Link.ScalerMatrix *= Matrix.CreateScale(scalar); 
                                        StaticItems.Add(a);


                                        break;
                                    }
                                case "enemy":
                                    {
                                         ET = xmlReader.GetAttribute("EnemyType");
                                         pos = xmlReader.GetAttribute("position");


                                        EnemyType = (Enemy.EnemyType)Enum.Parse(typeof(Enemy.EnemyType), ET);
                                         position = ReadFloat3(pos);



                                        mAI.Peek.SpawnEnemy(EnemyType, position);

                                        break;
                                    }


                            }
                            break;
                        }
                }
            }
            return StaticItems.ToArray();
        }

        public void XmlLoadCharacter()
        {
            XmlTextReader xmlReader;

            xmlReader = new XmlTextReader("XmlAvatar.xml");
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            String asset = xmlReader.Name;
                            switch (xmlReader.Name)
                            {
                                case "character":
                                    {
                                        int iHealth = (int)ReadFloat(xmlReader.GetAttribute("maxHealth"));
                                        int iInk = (int)ReadFloat(xmlReader.GetAttribute("maxInk"));

                                        mAvatar.Peek.SetMaxHealth(iHealth);
                                        mAvatar.Peek.SetMaxInk(iInk);

                                        break;
                                    }
                            }
                            break;
                        }
                }
            }

            //XmlTextReader xmlReader;

            //xmlReader = new XmlTextReader("XMLAvatar.xml");
            //    if (xmlReader.Name == "character")
            //    {
            //        mAvatar.Peek.SetMaxHealth((int)ReadFloat(xmlReader.GetAttribute("maxHealth")));
            //        mAvatar.Peek.SetMaxInk((int)ReadFloat(xmlReader.GetAttribute("maxInk")));
            //        xmlReader.Close();
            //    }


        }

        //Riddled with testing code, but it will write. Just need to get what we have to write. 
        //Bobby Spivey
        //January 16, 2011
        public static void XmlWriter()
        {
            XmlTextWriter write = new XmlTextWriter("XmlSave.xml", null);
            write.WriteStartDocument();
            write.Formatting = Formatting.Indented;
            write.WriteStartElement("book");
            write.Formatting = Formatting.Indented;

            write.WriteStartElement("level1");
            write.WriteAttributeString("position", "3 4 5");
            write.WriteEndElement();
            write.WriteElementString("level", "blach");
            write.WriteElementString("level", "level4");

            Console.WriteLine("wrote!");
            write.WriteEndElement();
            write.Close();
        }

        public void SaveOptions()
        {
            XmlTextWriter write = new XmlTextWriter("..\\..\\..\\Content\\Level XML\\XmlSettings.xml", null);
            write.WriteStartDocument();
            write.Formatting = Formatting.Indented;
            write.WriteStartElement("settings");
            write.Formatting = Formatting.Indented;
            String compName = System.Environment.MachineName;
            String resolution = ReadString(mGraphics.Peek.BackBufferResolution);
            String fullscreen = Convert.ToString(mGraphics.Peek.IsFullScreen);
            String musicVolume = Convert.ToString(mMenu.Peek.MusicVolume);
            String soundEffects = Convert.ToString(mMenu.Peek.SoundVolume);
            String postProcess = Convert.ToString(mGraphics.Peek.PostProcessing);
            String controls = Convert.ToString(mAvatar.Peek.FlipControls);
            


            write.WriteAttributeString("machinename", compName);
            write.WriteAttributeString("resolution", resolution);
            write.WriteAttributeString("fullscreen", fullscreen);
            write.WriteAttributeString("musicVolume", musicVolume);
            write.WriteAttributeString("soundEffects", soundEffects);
            write.WriteAttributeString("controls", controls);
            write.WriteAttributeString("postprocess", postProcess); 
            write.WriteEndElement();
            write.Close();
            ;
        }

        public void LoadOptions()
        {
            XmlTextReader xmlReader;
            xmlReader = new XmlTextReader("..\\..\\..\\Content\\Level XML\\XmlSettings.xml");
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            String asset = xmlReader.Name;
                            switch (xmlReader.Name)
                            {
                                case "settings":
                                    {
                                        String machinename = xmlReader.GetAttribute("machinename");

                                        if (machinename == System.Environment.MachineName)
                                        {

                                            String res = xmlReader.GetAttribute("resolution");
                                            String fscn = xmlReader.GetAttribute("fullscreen");
                                            String musVol = xmlReader.GetAttribute("musicVolume");
                                            String sndEff = xmlReader.GetAttribute("soundEffects");
                                            String ctrl = xmlReader.GetAttribute("controls");
                                            String postproc = xmlReader.GetAttribute("postProcess"); 

                                            mGraphics.Peek.BackBufferResolution = ReadFloat2(res);
                                            mGraphics.Peek.IsFullScreen = Convert.ToBoolean(fscn);
                                            mMenu.Peek.MusicVolume = Convert.ToInt16(musVol);
                                            mMenu.Peek.SoundVolume = Convert.ToInt16(sndEff);
                                            mAvatar.Peek.FlipControls = Convert.ToBoolean(ctrl);
                                            mGraphics.Peek.PostProcessing = Convert.ToBoolean(postproc); 

                                            mMenu.Peek.SettingDefaults(res); 
                                            mGraphics.Peek.Graphics.ApplyChanges();

                                        }

                                        xmlReader.Close();
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
            xmlReader.Close();
            ;
        }

        public void SaveGame(int slot, int level)
        {
            
            XmlTextWriter write = new XmlTextWriter("..\\..\\..\\Content\\Level XML\\XmlSave" + slot + ".xml", null);
            write.WriteStartDocument();
            write.Formatting = Formatting.Indented;
            write.WriteStartElement("save");
            write.Formatting = Formatting.Indented;
            int minutes = (int)(((mTimer.Peek.ElapsedTotalGameTime.Minutes - startTime.Minutes)+ previousGameTime));
            write.WriteAttributeString("level", Convert.ToString(level));
            write.WriteAttributeString("minutes", Convert.ToString(minutes)); 
            write.WriteEndElement();
            write.Close();
        }

        public void LoadGame(int slot)
        {
            setStartTime();
            XmlTextReader xmlReader;
            xmlReader = new XmlTextReader("..\\..\\..\\Content\\Level XML\\XmlSave" + slot + ".xml");
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            String asset = xmlReader.Name;
                            switch (xmlReader.Name)
                            {
                                case "save":
                                    {

                                        String lvl = xmlReader.GetAttribute("level");
                                        String mins = xmlReader.GetAttribute("minutes");

                                        previousGameTime = ReadFloat(mins); 
                                        int level = (int)ReadFloat(lvl);
                                        switch (level)
                                        {
                                            case 1:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel1(), null);
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel2(), null);
                                                    break; 
                                                }
                                            case 3:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel1(), null);
                                                    break;
                                                }
                                            case 4:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel4(), null);
                                                    break;
                                                }
                                            case 5:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel5(), null);
                                                    break;
                                                }
                                            case 6:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel6(), null);
                                                    break;
                                                }
                                            case 7:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel7(), null);
                                                    break;
                                                }
                                            case 8:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel8(), null);
                                                    break;
                                                }
                                            case 9:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel9(), null);
                                                    break;
                                                }
                                            case 10:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel10(), null);
                                                    break;
                                                }
                                            case 11:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel11(), null);
                                                    break;
                                                }
                                            case 12:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel12(), null);
                                                    break;
                                                }
                                            case 13:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel13(), null);
                                                    break;
                                                }
                                            case 14:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel14(), null);
                                                    break;
                                                }
                                            case 15:
                                                {
                                                    mLevel.Peek.QueueNextLevel(new AliceLevel15(), null);
                                                    break;
                                                }

                                        }
                                        mLevel.Peek.ChangeLevel();
                                        //cLevel levels = (cLevel)"AliceLevel" + ReadFloat(lvl).ToString();
                                        int minutes = (int)ReadFloat(mins); 
                                    }
                                    xmlReader.Close();
                                    break;

                            }
                            break;
                        }
                }
            }
            xmlReader.Close();
        }

        public void clearSave(int slot)
        {
            XmlTextWriter write = new XmlTextWriter("..\\..\\..\\Content\\Level XML\\XmlSave" + slot + ".xml", null);
            write.WriteStartDocument();
            write.Formatting = Formatting.Indented;
            write.WriteStartElement("save");
            write.Formatting = Formatting.Indented;
            write.WriteAttributeString("level", Convert.ToString(0));
            write.WriteAttributeString("hours", Convert.ToString(0));
            write.WriteAttributeString("minutes", Convert.ToString(0));
            write.WriteEndElement();
            write.Close();

        }

        public void setStartTime()
        {
            mTimer.Peek.CreateElapsedTimeSnapshot(startTime); 
        }

        public Vector2 getTime(int slot)
        {
            Vector2 time = new Vector2(0, 0);
            XmlTextReader xmlReader;
            xmlReader = new XmlTextReader("..\\..\\..\\Content\\Level XML\\XmlSave" + slot + ".xml");
            float minutes = 0; 
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            String asset = xmlReader.Name;
                            switch (xmlReader.Name)
                            {
                                case "save":
                                    {
                                        String mins = xmlReader.GetAttribute("minutes");

                                        minutes = ReadFloat(mins);

                                        time.X = (int)(minutes / 60);
                                        time.Y = (int)(minutes % 60);



                                    }
                                    xmlReader.Close();
                                    break;

                            }
                            break;
                        }
                }
            }
            xmlReader.Close();
            return time; 
        }

        public String getLevel(int slot)
        {
            {
                String level = "Level: 0";
                XmlTextReader xmlReader;
                xmlReader = new XmlTextReader("..\\..\\..\\Content\\Level XML\\XmlSave" + slot + ".xml");
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                String asset = xmlReader.Name;
                                switch (xmlReader.Name)
                                {
                                    case "save":
                                        {
                                            level = "Level: " + xmlReader.GetAttribute("level");
                                        }
                                        xmlReader.Close();
                                        break;
                                }
                                break;
                            }
                    }
                }
                xmlReader.Close();
                return level;
            }

        }

        //Writes enemies to the file location given. 
        //Still experimental, needs some work. Namely, the container for all the enemies in game. 
        //Bobby Spivey
        //January 16, 2011
        public static void writeEnemy(String location)// mEnemy enemy)
        {
            XmlTextWriter write = new XmlTextWriter(location, null);
            write.WriteStartDocument();
            write.Formatting = Formatting.Indented;
            write.WriteStartElement("enemies");
            write.Formatting = Formatting.Indented;

            write.WriteStartElement("enemy");
            write.WriteAttributeString("EnemyType", "worm");
            write.WriteAttributeString("position", "3 4 5");
            write.WriteAttributeString("color", "1 1 1 1");
            write.WriteAttributeString("opacity", "1");
            write.WriteAttributeString("horizontalFlip", "true");
            write.WriteAttributeString("verticalFlip", "true");
            write.WriteAttributeString("vegetation", "true");

            write.WriteEndElement();
            write.Close();
        }


        //http://forums.create.msdn.com/forums/p/52271/333095.aspx

        //I'll try to explain this to the best of my knowledge: 
        //The first line takes everything before the first "space," the second line removes everything 
        //from the space and back. Repeat until we get (x, y).
        //Bobby Spivey
        //January 22, 2011
        public static Vector2 ReadFloat2(string float2)
        {
            string x = float2.Substring(0, float2.IndexOf(' '));
            float2 = float2.Remove(0, x.Length + 1);
            string y = float2.Substring(0, float2.Length);


            return new Vector2(float.Parse(x), float.Parse(y));
        }

        //I'll try to explain this to the best of my knowledge: 
        //The first line takes everything before the first "space," the second line removes everything 
        //from the space and back. Repeat until we get (x, y, z).
        //Bobby Spivey
        //January 16, 2011
        public static Vector3 ReadFloat3(string float3)
        {
            string x = float3.Substring(0, float3.IndexOf(' '));
            float3 = float3.Remove(0, x.Length + 1);
            string y = float3.Substring(0, float3.IndexOf(' '));
            float3 = float3.Remove(0, y.Length + 1);
            string z = float3.Substring(0, float3.Length);
            //float3 = float3.Remove(0, y.Length);  

            return new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
        }

        //The first line takes everything before the first "space," the second line removes everything 
        //from the space and back. Repeat until we get (x, y, z, w).
        //Bobby Spivey
        //January 16, 2011
        public static Vector4 ReadFloat4(string float4)
        {
            string x = float4.Substring(0, float4.IndexOf(' '));
            float4 = float4.Remove(0, x.Length + 1);
            string y = float4.Substring(0, float4.IndexOf(' '));
            float4 = float4.Remove(0, y.Length + 1);
            string z = float4.Substring(0, float4.IndexOf(' '));
            float4 = float4.Remove(0, z.Length + 1);
            string w = float4.Substring(0, float4.Length);

            return new Vector4(float.Parse(x), float.Parse(y), float.Parse(z), float.Parse(w));
        }

        //The first line takes the value of the string, second line returns the converted float. 
        //Bobby Spivey
        //January 16, 2011
        public float ReadFloat(string float1)
        {

            string x = float1.Substring(0, float1.Length);
            return (float)Convert.ToDouble(x); 
            
        }

        //Color is weird in that passing it a Vector4 makes it take them as "percentages." 
        //Instead, I just return a new color object that parses (x, y, z, w) as bytes. 
        //So instead of having to put "0.2" in to get "R = 51," we can just put in "51" . 
        //Bobby Spivey
        //January 16, 2011
        public Color ReadColor(string clr)
        {
            string x = clr.Substring(0, clr.IndexOf(' '));
            clr = clr.Remove(0, x.Length + 1);
            string y = clr.Substring(0, clr.IndexOf(' '));
            clr = clr.Remove(0, y.Length + 1);
            string z = clr.Substring(0, clr.IndexOf(' '));
            clr = clr.Remove(0, y.Length + 1);
            string w = clr.Substring(0, clr.Length);
            Color color = new Color(byte.Parse(x), byte.Parse(y), byte.Parse(z), byte.Parse(w));


            return color;
        }

        public String ReadString(Vector2 res)
        {
            String resolution = res.X + " " + res.Y;
            return resolution;
        }



        //<EOC>
    }
}
