using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Inkwell.Framework.Particle;
using Inkwell.Framework.Triggers;
using Inkwell.Framework.Graphics;

namespace Inkwell.Framework
{
    class DemoLevel: cLevel
    {
        BasicModel test, Land, Foliage, test2;
        DustEmitter Dust = new DustEmitter();
        RainEmitter Rain = new RainEmitter();
        ExitTrigger Exit = new ExitTrigger();
        public override void Initialize()
        {
            Foliage = new BasicModel(Engine.GameContainer, ModelProperties.Vegetation, Assets.VEG_LAND, Engine.TempVector3(0.0f, 0.0f, 0.0f));
            mAvatar.Peek.Load(Engine.TempVector3(0.0f, 10.0f, 0.0f));
            Land = new BasicModel(Engine.GameContainer, ModelProperties.Opaque, Assets.LAND, Vector3.Zero);
            Land.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Environment\\Ground\\Grass1");
            Land.Link.TileAmount.X = 4.0f;
            Land.Link.TileAmount.Y = 4.0f;
            Land.Link.Display = true;
            test2 = new BasicModel(Engine.GameContainer, ModelProperties.Opaque, "Models\\Levels\\Level 2\\flatShroom", Vector3.Zero);
            test2.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Level Textures\\Level 2\\flatShroom");

            Dust.Initialize(Engine.GameContainer.Load<Texture2D>("Textures\\Environment\\Particles\\Dust"), 1024, 100, Direction.Up, Land.Link.BoundingBox, Color.DarkOliveGreen);
            Rain.Initialize(Engine.GameContainer.Load<Texture2D>("Textures\\Environment\\Particles\\Dust"), 2048, 100, Direction.Up, Land.Link.BoundingBox, Color.CornflowerBlue);

            Exit.Initialize(Land.Link.BoundingBox.Max.X);
            Exit.LoadTexture("Loading Screens\\ExitLevel");

            
            mAudio.Peek.LoadAllMusic();
            mAudio.Peek.LoadAllSounds();
            base.Initialize();
        }
        public override void Update()
        {
            if (mInput.Peek.IsKeyDown(Keys.R))
            {
                mLevel.Peek.ReloadLevel();
            }
            Dust.Update();
            Rain.Update();
            mAvatar.Peek.Update();

            /*How to Update the Exit Trigger*/
            if (Exit.Update(mAvatar.Peek.PlayerModel.Link.Position))
                mLevel.Peek.ChangeLevel(new BattleTestLevel());

            mCamera.Peek.Update(mAvatar.Peek.PlayerModel.Link.Position);
        }
        public override void Draw()
        {
            
            mModel.Peek.Draw();
            Dust.Draw();
            Rain.Draw();

            Exit.Draw();
        }
        public override void Kill()
        {
 	        base.Kill();
        }
    }
}
