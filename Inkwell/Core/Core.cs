using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Threading;

using Inkwell.Framework;

namespace Inkwell
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Core : Microsoft.Xna.Framework.Game
    {
        /*****************************************HEADER*****************************************/
        public static GraphicsDeviceManager Graphics;
        /****************************************************************************************/
        /// <summary>
        /// Constructor for the Core of the Application.
        /// </summary>
        public Core()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //GraphicsManager.Peek.IsFullScreen = true;
            mGraphics.Peek.BackBufferResolution = new Vector2(800.0f, 600.0f);
            //mGraphics.Peek.BackBufferResolution = new Vector2(1280.0f, 720.0f);
            //mGraphics.Peek.BackBufferResolution = new Vector2(1680.0f, 1050.0f);
            mGraphics.Peek.Graphics.IsFullScreen = false;
            base.IsMouseVisible = true;
        }
        /****************************************************************************************/
        /// <summary>
        /// Main Initializer for our XNA injection,
        /// this will bypass some of the components we don't need and prevent from loading them.
        /// </summary>
        /// <returns></returns>
        private void Startup()
        {
            /****************************************************************************************/
            /*Please Don't Alter this order it might cause conflicts*/
            Engine.Initialize(Services); //<-- Load The Engine Var's First
            mTimer.Peek.Initialize();
            //mGraphics.Peek.AntiAliasing(true, MultiSampleType.FourSamples);
            mGraphics.Peek.Initialize(Engine.CoreContainer); //<-- Initialize the Renderer
            mModel.Peek.Initialize();
            mEffect.Peek.Initialize(mGraphics.Peek.Device());
            mLevel.Peek.Initialize(Engine.PersistantContainer, Assets.LOADING_DEFAULT);
            Skybox.Initialize(Engine.PersistantContainer,
                "Graphics\\Shaders\\Skybox",
                "Models\\Basic Objects\\Skybox",
                "Textures\\Environment\\Sky Textures\\Clouds");
            mGraphics.Peek.PostProcessing = true;
            /****************************************************************************************/
            /*+++Add initialization Logic Below here!+++*/
            //mLevel.Peek.QueueNextLevel(new DemoLevel(), null); //<-- Controls the First Level that Loads
            mMenu.Peek.Initialize();
            mMenu.Peek.Load(Engine.CoreContainer);
            Engine.DebugEnabled = false;
            /*+++Add initialization Logic Above me!+++*/
            /****************************************************************************************/
        }
        /****************************************************************************************/
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Engine.GameInitialized) //Only update Internals after we succesfully loaded XNA
            {
                mDebug.BeginUpdateProbe();
                /****************************************************************************************/
                /*Update Input First*/
                Engine.WindowIsActive = this.IsActive;
                mInput.Peek.Update();
                mGraphics.Peek.Update();
                /****************************************************************************************/
                /*Allow our game to Exit*/
                if (mMenu.Peek.WorkerState == mMenu.MenuState.EXIT) { this.Exit(); }
                /*Debug Exit*/
                if (mInput.Peek.IsKeyDown(Keys.LeftShift) && mInput.Peek.IsKeyDown(Keys.Escape)) { this.Exit(); }
                /*Turn of Logic Locks*/
                if (mInput.Peek.IsKeyPressed(Keys.F9) && Engine.DebugEnabled) { this.IsFixedTimeStep = !this.IsFixedTimeStep; }
                /****************************************************************************************/
                /*Update Engine Variables*/
                Engine.Update(gameTime); 
                mTimer.Peek.Update(ref gameTime);
                /****************************************************************************************/
                if ((int)mMenu.Peek.WorkerState > 10)
                {
                    /*Level Management System*/
                    //if (mInput.Peek.IsKeyPressed(Keys.L))
                    //    mLevel.Peek.ReloadLevel(); //<-- La Magic of the Level Manager all in one function
                    mLevel.Peek.Update();
                    if (mInput.Peek.IsKeyPressed(Keys.Escape) && mDialogue.Peek.DialgoueWorkerState == mDialogue.DialogueState.DialogueStop)
                    {
                        mMenu.Peek.Pause();
                    }
                }
                mMenu.Peek.Update();
                /****************************************************************************************/
                //if (mInput.Peek.IsKeyPressed(Keys.I))
                //{
                //    mDialogue.Peek.DialogueContinue();
                //}
                mDialogue.Peek.Update();
                mModel.Peek.Update();
                /****************************************************************************************/
                mDebug.EndUpdateProbe();
            }
            base.Update(gameTime); //<-- Ensure that we are always updating XNA
        }
        /****************************************************************************************/
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (Engine.GameInitialized && !mGraphics.Peek.DeviceChanged)
            {
                mDebug.BeginDrawProbe();
                if (mGraphics.Peek.PostProcessing && GraphicsDevice.RenderState.FillMode == FillMode.Solid) //<-- "Assuming" that Bloom is off this increases performance for the "off" by roughly 300 fps from the old method
                {
                    Skybox.Draw(mCamera.Peek.ReturnCamera().View, mCamera.Peek.ReturnCamera().Projection, mCamera.Peek.ReturnCamera().Position);
                    mLevel.Peek.Draw();
                    GraphicsDevice.ResolveBackBuffer(mGraphics.Peek.SceneTexture); //<-- Clear our Backbuffer
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    BloomEffect.Draw(mGraphics.Peek.SpriteBatch, mGraphics.Peek.SceneTexture);
                    Vignette.Draw(mGraphics.Peek.SpriteBatch, BloomEffect.BloomTarget.GetTexture());
                    mGraphics.Peek.SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                    mGraphics.Peek.SpriteBatch.Draw(Vignette.VignetteTarget.GetTexture(), Vector2.Zero, Color.White);
                    mGraphics.Peek.SpriteBatch.End();
                }
                else if (GraphicsDevice.RenderState.FillMode == FillMode.Solid)
                {
                    Skybox.Draw(mCamera.Peek.ReturnCamera().View, mCamera.Peek.ReturnCamera().Projection, mCamera.Peek.ReturnCamera().Position);
                    mLevel.Peek.Draw();
                    GraphicsDevice.ResolveBackBuffer(mGraphics.Peek.SceneTexture); //<-- Clear our Backbuffer
                    Vignette.Draw(mGraphics.Peek.SpriteBatch, mGraphics.Peek.SceneTexture);
                    mGraphics.Peek.SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                    mGraphics.Peek.SpriteBatch.Draw(Vignette.VignetteTarget.GetTexture(), Vector2.Zero, Color.White);
                    mGraphics.Peek.SpriteBatch.End();
                }
                else
                {
                    Skybox.Draw(mCamera.Peek.ReturnCamera().View, mCamera.Peek.ReturnCamera().Projection, mCamera.Peek.ReturnCamera().Position);
                    mLevel.Peek.Draw();
                }
                mDialogue.Peek.Draw(mGraphics.Peek.SpriteBatch);
                mMenu.Peek.Draw(mGraphics.Peek.SpriteBatch);
                if(Engine.DebugEnabled)
                mDebug.Draw(mGraphics.Peek.SpriteBatch, gameTime, mGraphics.Peek.SpriteBatchScale);
            }
            else if(!Engine.GameInitialized)
            {
                /*Draw Our first frame*/
                Startup();
                GraphicsDevice.Clear(Color.CornflowerBlue); //Flush our Buffer
                base.Draw(gameTime);
                Engine.GameInitialized = true;
            }
            mDebug.EndDrawProbe();
            /*Draw XNA's Materials*/
            base.Draw(gameTime);
            //GraphicsDevice.Clear(Color.Black);
        }
        /****************************************************************************************/
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Core game = new Core())
            {
                game.Run(); //<-- Starts our Game this is the Application Main point.
            }
        }
        /****************************************************************************************/
    }
}
