using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Inkwell.Framework
{
    /*Andrew Polaskey - 12.10.2010*/
    /// <summary>
    /// (Singleton) In charge of storing screen resolution and graphics cap's; Will need a hook into Core's Update & Perhaps Load.
    /// </summary>
    class mGraphics
    {
        /****************************************************************************************/
        private static volatile mGraphics _instance;
        private static object _padlock = new Object();
        /*****************************************HEADER*****************************************/
        private Vector2 _v2SpriteResolution = new Vector2(1280.0f, 720.0f); //Controls Window Size (This is also our target resolution for 2D materials)
        private Vector2 _v2BufferResolution = new Vector2(1280.0f, 720.0f); //Controls Backbuffer Size (Increasing can yield better image quality at a cost)
        /****************************************************************************************/
        /*SpriteBatch Related Globals*/
        /// <summary>(Matrix) Used to scale our spriteBatch when we re-size the window past its given internal buffer.</summary>
        public Matrix SpriteBatchScale = Matrix.Identity;
        public SpriteBatch SpriteBatch;
        /// <summary>(VertexDeclaration) Used for Drawing Vertices by Hand [Position/Color VD]</summary>
        public VertexDeclaration vdPositionColor;
        /// <summary>(VertexDeclaration) Used for Drawing Vertices by Hand [Position/Normal/Texture VD]</summary>
        public VertexDeclaration vdPositionNormalTexture;
        /// <summary>(GraphicsDeviceCapabilities) A Class with some information on our Graphics Adapter (Used to Resolve Errors with lower-end hardware)</summary>
        public GraphicsDeviceCapabilities GraphicsCaps;
        /// <summary>(Bool) Enable/Disable Post Processing</summary>
        public bool PostProcessing = true;
        private bool _bSprBatchEnabled = false;
        /// <summary>(ResolveTexture2D) Resolve Texture for pushing the Backbuffer into a Texture2D object</summary>
        public ResolveTexture2D SceneTexture;
        /// <summary>(RenderTarget2D) A Render Target for Scene Additions</summary>
        public RenderTarget2D SceneRenderTarget;
        private bool _DeviceChanged = false;
        /****************************************FUNCTIONS***************************************/
        public void Initialize(ContentManager content)
        {
            SpriteBatch = new SpriteBatch(Core.Graphics.GraphicsDevice);
            vdPositionColor = new VertexDeclaration(Core.Graphics.GraphicsDevice, VertexPositionColor.VertexElements);
            vdPositionNormalTexture = new VertexDeclaration(Core.Graphics.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            GraphicsCaps = GraphicsAdapter.DefaultAdapter.GetCapabilities(DeviceType.Hardware);
            SceneTexture = new ResolveTexture2D(mGraphics.Peek.Device(), mGraphics.Peek.Device().PresentationParameters.BackBufferWidth,
                mGraphics.Peek.Device().PresentationParameters.BackBufferHeight, 1, mGraphics.Peek.Device().PresentationParameters.BackBufferFormat);
            SceneRenderTarget = CreateRenderTarget(1, mGraphics.Peek.Device().PresentationParameters.BackBufferFormat);
            mCamera.Peek.Initialize(Engine.TempVector3(0.0f, 0.0f, 0.0f));
        }
        public void Update()
        {
            if (_DeviceChanged)
            {
                RebuildGraphicsDevice();
                BloomEffect.Reset(Core.Graphics.GraphicsDevice);
                Vignette.Reset(Core.Graphics.GraphicsDevice);
                mCamera.Peek.ResizeCameras(5.0f, 2000.0f);
                Engine.PurgeGarbageHeap();
                _DeviceChanged = false;
            }
        }
        private void RebuildGraphicsDevice()
        {
            Graphics.GraphicsDevice.Reset();
            Graphics.PreferredBackBufferHeight = (int)this._v2BufferResolution.Y;
            Graphics.PreferredBackBufferWidth = (int)this._v2BufferResolution.X;
            SpriteBatchScale = Matrix.CreateScale((_v2BufferResolution / _v2SpriteResolution).X, (_v2BufferResolution / _v2SpriteResolution).Y, 1.0f);
            Graphics.ApplyChanges();

            SceneTexture.Dispose();
            SceneTexture = null;
            SceneRenderTarget.Dispose();
            SceneRenderTarget = null;
            SceneTexture = new ResolveTexture2D(mGraphics.Peek.Device(), Graphics.PreferredBackBufferWidth,
            Graphics.PreferredBackBufferHeight, 1, mGraphics.Peek.Device().PresentationParameters.BackBufferFormat);
            SceneRenderTarget = CreateRenderTarget(1, mGraphics.Peek.Device().PresentationParameters.BackBufferFormat);
        }
        /*************************************ACCESSORS******************************************/
        /// <summary>(Bool) Determine whether the application is in fullscreen mode or not.</summary>
        public bool IsFullScreen
        {
            get { return Graphics.IsFullScreen; }
            set { Graphics.IsFullScreen = value; _DeviceChanged = true; }
        }
        public bool DeviceChanged
        {
            get { return this._DeviceChanged; }
        }
        /****************************************************************************************/
        /// <summary>(Vector2) Modify the Internal Resolution</summary>
        public Vector2 ScreenResolution
        {
            get { return this._v2SpriteResolution; }
            set { this._v2SpriteResolution = value; this._DeviceChanged = true; }
        }
        /****************************************************************************************/
        /// <summary>
        /// (float) Determines the Aspect Ratio of our Viewport.
        /// </summary>
        /// <returns>A float value with the Screen Height / Screen Width</returns>
        public float AspectRatio() { return Core.Graphics.GraphicsDevice.Viewport.AspectRatio; }
        /****************************************************************************************/
        /// <summary>(Vector2) Modify the Buffer Resolution</summary>
        public Vector2 BackBufferResolution
        {
            get { return this._v2BufferResolution; }
            set
            {
                    this._v2BufferResolution = value;
                    this._DeviceChanged = true;
            }
        }
        /****************************************************************************************/
        /// <summary>(GraphicsDevice) Quick Capture to the active GraphicsDevice.</summary>
        public GraphicsDevice Device()
        {
            return Core.Graphics.GraphicsDevice;
        }
        /****************************************************************************************/
        /// <summary>Determine what type of mode our Graphics Device is rendering in.</summary>
        /// <param name="Fill">Wireframe, Point, (Default) Solid</param>
        public void FillMode(FillMode Fill)
        {
            Core.Graphics.GraphicsDevice.RenderState.FillMode = Fill;
        }
        /****************************************************************************************/
        /// <summary>(GraphicsDeviceManager) Quick Capture to the active GraphicsDeviceManager.</summary>
        public GraphicsDeviceManager Graphics
        {
            get { return Core.Graphics; }
            set { Core.Graphics = value; }
        }
        /****************************************************************************************/
        /// <summary>(Void) Enable Depth Checks.</summary>
        public void SetDepthBuffer(bool enabled)
        {
            Core.Graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = enabled;
            Core.Graphics.GraphicsDevice.RenderState.DepthBufferEnable = enabled;
        }
        /****************************************************************************************/
        /// <summary>Begins a SpriteBatch Draw with the appropriate settings.</summary>
        public void ToggleSpriteDraw()
        {
            _bSprBatchEnabled = !_bSprBatchEnabled; //<-- Tell the system to toggle drawing of sprites
            if(_bSprBatchEnabled) //<-- Actually check for the toggle and act accordingly
                SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState, SpriteBatchScale);
            else SpriteBatch.End();
        }
        /****************************************************************************************/
        public void UseAlphaTest(bool Choice)
        {
            Core.Graphics.GraphicsDevice.RenderState.AlphaTestEnable = Choice;
        }
        /// <summary>
        /// (Void) Allows the usage of Alpha Textures for 3D Content.
        /// </summary>
        public void ToggleAlphaBlending(bool Enabled)
        {
            if (Enabled)
            {
                /*All of this enables Alpha Pixels*/
                Device().RenderState.AlphaBlendEnable = true; //<-- Allows Alpha channel
                Device().RenderState.SourceBlend = Blend.SourceAlpha;
                Device().RenderState.AlphaSourceBlend = Blend.One;
                Device().RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                Device().RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
                Device().RenderState.BlendFunction = BlendFunction.Add;
            }
            else Device().RenderState.AlphaBlendEnable = false; //<-- Disable Alpha Channe
            
        }
        /****************************************************************************************/
        /// <summary>
        /// (Void) Set how we cull our models [TIP: Default is CounterClockwise]
        /// </summary>
        /// <param name="Mode"></param>
        public void CullingState(CullMode Mode)
        {
            Core.Graphics.GraphicsDevice.RenderState.CullMode = Mode;
        }
        /****************************************************************************************/
        /// <summary>
        /// (Void) Resets the Culling Mode to it's Original.
        /// </summary>
        public void ResetCullingState()
        {
            Core.Graphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
        }
        /****************************************************************************************/
        /// <summary>
        /// (Void) Apply Anti-aliasing to lower jagged edges appearing on 3D content.
        /// </summary>
        /// <param name="bChoice">(Bool) Whether we have MSAA enabled or not.</param>
        /// <param name="mstLevel">(MultiSampleType) The amount of MSAA we are applying.</param>
        public void AntiAliasing(bool bChoice, MultiSampleType mstLevel)
        {
            Core.Graphics.PreferMultiSampling = bChoice;
            Core.Graphics.GraphicsDevice.PresentationParameters.MultiSampleType = mstLevel;
            Core.Graphics.GraphicsDevice.PresentationParameters.MultiSampleQuality = 0;
            _DeviceChanged = true;
        }
        /// <summary>(Internal Helper) Checks the backbuffer's texture size</summary>
        bool CheckTextureSize(int width, int height, out int newwidth, out int newheight)
        {
            bool retval = false;

            GraphicsCaps = GraphicsAdapter.DefaultAdapter.GetCapabilities(DeviceType.Hardware);

            //MODERN HARDWARE SHOULDN'T NEED THIS CRAP
            if (GraphicsCaps.TextureCapabilities.RequiresPower2)
            {
                retval = true;  // Return true to indicate the numbers changed

                 /*Find the nearest base two log of the current width, 
                 and go up to the next integer */               
                double exp = Math.Ceiling(Math.Log(width) / Math.Log(2));
                 //and use that as the exponent of the new width
                width = (int)(exp * exp);
                 //Repeat the process for height
                exp = Math.Ceiling(Math.Log(height) / Math.Log(2));
                height = (int)(exp * exp);
            }
            if (GraphicsCaps.TextureCapabilities.RequiresSquareOnly)
            {
                retval = true;  // Return true to indicate numbers changed
                width = Math.Max(width, height);
                height = width;
            }

            newwidth = Math.Min(GraphicsCaps.MaxTextureWidth, width);
            newheight = Math.Min(GraphicsCaps.MaxTextureHeight, height);
            return retval;
        }
        /// <summary>(Internal Helper) Assists in generating a DepthStencilBuffer</summary>
        private DepthStencilBuffer CreateDepthStencil(RenderTarget2D target)
        {
            return new DepthStencilBuffer(target.GraphicsDevice, target.Width,
                target.Height, target.GraphicsDevice.DepthStencilBuffer.Format,
                target.MultiSampleType, target.MultiSampleQuality);
        }
        /// <summary>Check to see if we have support for the specified DpethFormat</summary>
        public DepthStencilBuffer CreateDepthStencil(RenderTarget2D target, DepthFormat depth)
        {
            if (GraphicsAdapter.DefaultAdapter.CheckDepthStencilMatch(DeviceType.Hardware,
               GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Format, target.Format,
                depth))
            {
                return new DepthStencilBuffer(target.GraphicsDevice, target.Width,
                    target.Height, depth, target.MultiSampleType, target.MultiSampleQuality);
            }
            else
                return CreateDepthStencil(target);
        }
        /// <summary>Generate a RenderTarget2D with the appropriate properties for our scene.</summary>
        public RenderTarget2D CreateRenderTarget(int numberLevels, SurfaceFormat surface)
        {
            MultiSampleType type = Core.Graphics.GraphicsDevice.PresentationParameters.MultiSampleType;

            // If the card can't use the surface format
            if (!GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(
                DeviceType.Hardware, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Format,
                TextureUsage.None, QueryUsages.None, ResourceType.RenderTarget,
                surface))
            {
                // Fall back to current display format
                surface = Device().DisplayMode.Format;
            }
            // Or it can't accept that surface format 
            // with the current AA settings
            else if (!GraphicsAdapter.DefaultAdapter.CheckDeviceMultiSampleType(DeviceType.Hardware, surface, Device().PresentationParameters.IsFullScreen, type))
            {
                // Fall back to no antialiasing
                type = MultiSampleType.None;
            }

            int width, height;

            // See if we can use our buffer size as our texture
            CheckTextureSize(Core.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Core.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
                out width, out height);

            // Create our render target
            return new RenderTarget2D(Core.Graphics.GraphicsDevice,
                width, height, numberLevels, surface,
                type, 0);

        }
        /// <summary>Assign a Rendertarget to the Graphics Device</summary>
        public void BeginTargetedDraw(RenderTarget2D rTarget)
        {
            Core.Graphics.GraphicsDevice.SetRenderTarget(0, rTarget);
        }
        /// <summary>End the Assignment of the Rendertarget.</summary>
        public void EndTargetedDraw()
        {
            Core.Graphics.GraphicsDevice.SetRenderTarget(0, null);
        }
        /****************************************************************************************/
        /// <summary>Handle to the Graphics Component</summary>
        public static mGraphics Peek
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
                            _instance = new mGraphics(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
        /****************************************************************************************/
        //<EOC>
    }
}
