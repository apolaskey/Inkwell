using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;



namespace Inkwell.Framework
{
    class LogoMenu: MenuScreen
    {
        struct menuItem
        {
            public Texture2D t2d_Texture;
            public Vector2 v2_Position;
            public Vector2 v2_Size;
        }

        private menuItem menuBackground;

        public override void Initialize()
        {
            menuBackground = new menuItem();
        }

        public override void Load(ContentManager content)
        {
            menuBackground.t2d_Texture = Engine.CoreContainer.Load<Texture2D>("Textures\\Menu\\Logo");
            menuBackground.v2_Position = Vector2.Zero;
            menuBackground.v2_Size = Engine.TempVector2(1280, 720);
        }

        public override void Update()
        {
            if (mInput.Peek.IsAnyKeyDown())
            {
                mMenu.Peek.WorkerState = mMenu.MenuState.MAIN;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            mGraphics.Peek.ToggleSpriteDraw();
            spriteBatch.Draw(menuBackground.t2d_Texture, new Rectangle((int)menuBackground.v2_Position.X, (int)menuBackground.v2_Position.Y, (int)menuBackground.v2_Size.X, (int)menuBackground.v2_Size.Y), Color.White);
            mGraphics.Peek.ToggleSpriteDraw();
        }
    }
}
