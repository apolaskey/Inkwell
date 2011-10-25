using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inkwell.Framework
{
    public sealed class mEffect
    {
        /*****************************************HEADER*****************************************/
        /*Singleton Start*/
        #region Singleton
        private static volatile mEffect _instance;
        private static object _padlock = new Object();
        public static mEffect Peek
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
                            _instance = new mEffect(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        #endregion
        /*Globals Start*/
        public Vector3 SunPosition = Vector3.Zero;
        public Color SunColor = Color.White;
        public float SunIntensity = 0.8f;
        Effect _MasterEffect, _VegEffect, _PointEffect;
        /*Bloom Post Process Things*/
        public Effect BloomExtract;
        public Effect BloomCombine;
        public Effect GaussianBlur;
        /*Depth of Field Post-Process Things*/
        public Texture2D DepthTexture1, SceneTexture, BlurredSceneTexture;
        public DepthStencilBuffer PureDepthBuffer, DoFDepthBuffer;
        public Effect MainEffect, PostDoF, PostBlur;
        public EffectTechnique EnvironmentShader, DepthMapShader, DoFShader;
        public RenderTarget2D PureRender, RenderBlurred, RenderBlurred2, DepthTarget;
        /****************************************FUNCTIONS***************************************/
        public void Initialize(GraphicsDevice Device)
        {
            SunPosition = Engine.TempVector3(0.0f, 1.0f, 1.0f);
            _MasterEffect = Engine.CoreContainer.Load<Effect>(Assets.MASTER_EFFECT);
            _VegEffect = Engine.CoreContainer.Load<Effect>(Assets.VEGETATION_EFFECT);
            _PointEffect = Engine.CoreContainer.Load<Effect>(Assets.POINTSPRITE_EFFECT);
            BloomExtract = Engine.CoreContainer.Load<Effect>(Assets.BLOOMEXTRACT_EFFECT);
            BloomCombine = Engine.CoreContainer.Load<Effect>(Assets.BLOOMCOMBINE_EFFECT);
            GaussianBlur = Engine.CoreContainer.Load<Effect>(Assets.GAUSSIANBLUR_EFFECT);
            //_PostEffect = Engine.CoreContainer.Load<Effect>(Assets.POSTPROCESS_EFFECT);

            BloomEffect.Load(Engine.CoreContainer, mGraphics.Peek.Device(), Assets.BLOOMEXTRACT_EFFECT, Assets.BLOOMCOMBINE_EFFECT, Assets.GAUSSIANBLUR_EFFECT);
            BloomEffect.Enabled = true; //<-- Set to True to enable
            Vignette.Load(Engine.CoreContainer, mGraphics.Peek.Device());
        }
        /****************************************************************************************/
        public Effect MasterEffect()
        {
            return this._MasterEffect;
        }
        public Effect VegetationEffect()
        {
            return this._VegEffect;
        }
        public Effect PointEffect()
        {
            return this._PointEffect;
        }
        public void InitializeDepthOfField()
        {

        }
        public void RenderDepthMapBegin(DepthStencilBuffer depthBuffer)
        {
            mGraphics.Peek.Graphics.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
            mGraphics.Peek.Graphics.GraphicsDevice.SetRenderTarget(0, DepthTarget);

            PureDepthBuffer = mGraphics.Peek.Graphics.GraphicsDevice.DepthStencilBuffer;

            /*Have our Graphics Device us this depth buffer*/
            mGraphics.Peek.Graphics.GraphicsDevice.DepthStencilBuffer = depthBuffer;
            /*Call Draw Scenes afterwards*/
        }
        public void EndRenderDepthMap()
        {
            mGraphics.Peek.Graphics.GraphicsDevice.SetRenderTarget(0, null);
            mGraphics.Peek.Graphics.GraphicsDevice.DepthStencilBuffer = PureDepthBuffer;
            DepthTexture1 = DepthTarget.GetTexture();
        }
    }
}
