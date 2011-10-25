using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Inkwell.Framework
{
    /// <summary>Bloom Post-Process [COST - HIGH --Non Threaded]</summary>
    static class BloomEffect
    {
        private static Effect _BloomExtract, _BloomCombine, _GaussianBlur;
        public static bool Enabled = true;
        private static RenderTarget2D renderTarget1, renderTarget2;
        public static RenderTarget2D BloomTarget;
        static BloomSettings settings = BloomSettings.PresetSettings[0];
        static EffectParameter horzWeightsParam, horzOffsetsParam, vertWeightsParam, vertOffsetsParam;
        public static BloomSettings Settings { get { return settings; } set { settings = value; PreComputeBlurEffect(); } }
        static Vector2 delta = Vector2.Zero;
        static int sampleCount = 0;
        static float[] horzWeights, vertWeights;
        static Vector2[] horzOffsets, vertOffsets;


        /// <summary>Configure the bloom effect backbuffer</summary>
        public static void Load(ContentManager Content, GraphicsDevice Device, string BloomExtractLocation, string BloomCombineLocation, string GaussianBlurLocation)
        {
            _BloomExtract = Content.Load<Effect>(BloomExtractLocation);
            _BloomCombine = Content.Load<Effect>(BloomCombineLocation);
            _GaussianBlur = Content.Load<Effect>(GaussianBlurLocation);
            renderTarget1 = CreateRenderTarget(Device, 1, Device.PresentationParameters.BackBufferFormat);
            renderTarget2 = CreateRenderTarget(Device, 1, Device.PresentationParameters.BackBufferFormat);
            BloomTarget = CreateRenderTarget(Device, 1, Device.PresentationParameters.BackBufferFormat);
            PreComputeBlurEffect();
        }
        public static void Reset(GraphicsDevice Device)
        {
            renderTarget1.Dispose();
            renderTarget1 = null;
            renderTarget2.Dispose();
            renderTarget2 = null;
            BloomTarget.Dispose();
            BloomTarget = null;
            renderTarget1 = CreateRenderTarget(Device, 1, Device.PresentationParameters.BackBufferFormat);
            renderTarget2 = CreateRenderTarget(Device, 1, Device.PresentationParameters.BackBufferFormat);
            BloomTarget = CreateRenderTarget(Device, 1, Device.PresentationParameters.BackBufferFormat);
            PreComputeBlurEffect();
        }
        /// <summary>(Internal Helper) Checks the backbuffer's texture size</summary>
        static bool CheckTextureSize(int width, int height, out int newwidth, out int newheight)
        {
            bool retval = false;

            GraphicsDeviceCapabilities Caps;
            Caps = GraphicsAdapter.DefaultAdapter.GetCapabilities(DeviceType.Hardware);

            //MODERN HARDWARE SHOULDN'T NEED THIS CRAP
            //if (Caps.TextureCapabilities.RequiresPower2)
            //{
            //    retval = true;  // Return true to indicate the numbers changed

            //    /*Find the nearest base two log of the current width, 
            //    and go up to the next integer */
            //    double exp = Math.Ceiling(Math.Log(width) / Math.Log(2));
            //    //and use that as the exponent of the new width
            //    width = (int)(exp * exp);
            //    //Repeat the process for height
            //    exp = Math.Ceiling(Math.Log(height) / Math.Log(2));
            //    height = (int)(exp * exp);
            //}
            //if (Caps.TextureCapabilities.RequiresSquareOnly)
            //{
            //    retval = true;  // Return true to indicate numbers changed
            //    width = Math.Max(width, height);
            //    height = width;
            //}

            newwidth = Math.Min(Caps.MaxTextureWidth, width);
            newheight = Math.Min(Caps.MaxTextureHeight, height);
            return retval;
        }
        public static RenderTarget2D CreateRenderTarget(GraphicsDevice Device, int numberLevels, SurfaceFormat surface)
        {
            MultiSampleType type = Device.PresentationParameters.MultiSampleType;

            // If the card can't use the surface format
            if (!GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(
                DeviceType.Hardware, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Format,
                TextureUsage.None, QueryUsages.None, ResourceType.RenderTarget,
                surface))
            {
                // Fall back to current display format
                surface = Device.DisplayMode.Format;
            }
            // Or it can't accept that surface format 
            // with the current AA settings
            else if (!GraphicsAdapter.DefaultAdapter.CheckDeviceMultiSampleType(DeviceType.Hardware, surface, Device.PresentationParameters.IsFullScreen, type))
            {
                // Fall back to no antialiasing
                type = MultiSampleType.None;
            }

            int width, height;

            // See if we can use our buffer size as our texture
            CheckTextureSize(Device.PresentationParameters.BackBufferWidth,
                Device.PresentationParameters.BackBufferHeight,
                out width, out height);

            // Create our render target
            return new RenderTarget2D(Device,
                width, height, numberLevels, surface,
                type, 0);

        }
        public static void BloomToggle()
        {
            // Switch to the next bloom settings preset?
            //if(mInput.Peek.IsKeyPressed(Keys.U))
            //{
            //    bloomSettingsIndex = (bloomSettingsIndex + 1) %
            //                         BloomSettings.PresetSettings.Length;

            //    settings = BloomSettings.PresetSettings[bloomSettingsIndex];
            //}

            // Toggle bloom on or off?
            if (mInput.Peek.IsKeyPressed(Keys.B))
            {
                BloomEffect.Enabled = !BloomEffect.Enabled;
            }
        }
        public static void PreComputeBlurEffect()
        {
            /*Prepare Horizontal Params*/
            SetBlurEffectParameters(1.0f / (float)renderTarget1.Width, 0, ref horzWeights, ref horzOffsets, horzWeightsParam, horzOffsetsParam);
            /*Prepare Vert Params*/
            SetBlurEffectParameters(0, 1.0f / (float)renderTarget1.Height, ref vertWeights, ref vertOffsets, vertWeightsParam, vertOffsetsParam);
        }
        /// <summary>Extract the Colors of our Image to use for Blooming.</summary>
        static void ExtractColors(SpriteBatch SpriteBatch, Texture2D t2dTexture)
        {
            SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

            _BloomExtract.Parameters["BloomThreshold"].SetValue(Settings.BloomThreshold);
            _BloomExtract.Begin();
            _BloomExtract.CurrentTechnique.Passes[0].Begin();

            SpriteBatch.Draw(t2dTexture, Vector2.Zero, Color.White);

            SpriteBatch.End();
            _BloomExtract.CurrentTechnique.Passes[0].End();
            _BloomExtract.End();

        }        /// <summary>(Helper) Used in conjunction with SetBlurEffectParameters()</summary>
        static float ComputeGaussian(float n)
        {
            return (float)((1.0 / Math.Sqrt(2 * 3.14f * Settings.BlurAmount)) *
                           Math.Exp(-(n * n) / (2 * Settings.BlurAmount * Settings.BlurAmount)));
        }
        /// <summary>(Helper) Responsible for preparing the Gaussian Bloom shader.</summary>
        static void SetBlurEffectParameters(float dx, float dy, ref float[] floatWeights, ref Vector2[] vector2Offsets, EffectParameter weightsParameter, EffectParameter offsetsParameter)
        {
            // Look up the sample weight and offset effect parameters.
            weightsParameter = mEffect.Peek.GaussianBlur.Parameters["SampleWeights"];
            offsetsParameter = mEffect.Peek.GaussianBlur.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            sampleCount = weightsParameter.Elements.Count;
            floatWeights = new float[sampleCount];
            vector2Offsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            floatWeights[0] = ComputeGaussian(0);
            vector2Offsets[0] = Vector2.Zero;

            // Maintain a sum of all the weighting values.
            float totalWeights = floatWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                floatWeights[i * 2 + 1] = weight;
                floatWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                /*By offsetting the samples a bit we can take advantage of bilinear filtering that automatically gets performed for Monitor inputs*/
                float sampleOffset = i * 2 + 1.5f;

                delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                vector2Offsets[i * 2 + 1] = delta;
                vector2Offsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < floatWeights.Length; i++)
            {
                floatWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
        }
        /// <summary>Fairly Fast Method to Blur an Image (Costs go up with higher Resolutions)</summary>
        static void GaussianBlur(SpriteBatch SpriteBatch, Texture2D t2dTexture, bool BlurredHorizontally)
        {
            //SetBlurEffectParameters(1.0f / (float)renderTarget1.Width, 0, ref horzWeights, ref horzOffsets, horzWeightsParam, horzOffsetsParam);
            //else SetBlurEffectParameters(0, 1.0f / (float)renderTarget1.Height, ref vertWeights, ref vertOffsets, vertWeightsParam, vertOffsetsParam);
            if (BlurredHorizontally)
            {
                mEffect.Peek.GaussianBlur.Parameters["SampleWeights"].SetValue(horzWeights);
                mEffect.Peek.GaussianBlur.Parameters["SampleOffsets"].SetValue(horzOffsets);
            }
            else
            {
                mEffect.Peek.GaussianBlur.Parameters["SampleWeights"].SetValue(vertWeights);
                mEffect.Peek.GaussianBlur.Parameters["SampleOffsets"].SetValue(vertOffsets);
            }

            SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            _GaussianBlur.Begin();
            _GaussianBlur.CurrentTechnique.Passes[0].Begin();

            SpriteBatch.Draw(t2dTexture, Vector2.Zero, Color.White);

            SpriteBatch.End();
            _GaussianBlur.CurrentTechnique.Passes[0].End();
            _GaussianBlur.End();
        }
        /// <summary>Finally Combine our Processed Image with our Original Scene creating a nice Bloom Effect.</summary>
        static void BloomImage(SpriteBatch SpriteBatch, Texture2D t2dTexture, Texture2D resolveTexture)
        {
            SpriteBatch.GraphicsDevice.Textures[1] = resolveTexture;
            SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

            _BloomCombine.Parameters["BloomIntensity"].SetValue(Settings.BloomIntensity);
            _BloomCombine.Parameters["BaseIntensity"].SetValue(Settings.BaseIntensity);
            _BloomCombine.Parameters["BloomSaturation"].SetValue(Settings.BloomSaturation);
            _BloomCombine.Parameters["BaseSaturation"].SetValue(Settings.BaseSaturation);
            _BloomCombine.Begin();
            _BloomCombine.CurrentTechnique.Passes[0].Begin();

            SpriteBatch.Draw(t2dTexture, Vector2.Zero, Color.White);

            SpriteBatch.End();
            _BloomCombine.CurrentTechnique.Passes[0].End();
            _BloomCombine.End();
        }

        /// <summary>Display the bloomed image</summary>
        public static void Draw(SpriteBatch SpriteBatch, Texture2D SceneImage)
        {
            if (Enabled == true)
            {
                /*Pull the pixels that we want to bloom out of the Shader*/
                SpriteBatch.GraphicsDevice.SetRenderTarget(0, renderTarget1);
                ExtractColors(SpriteBatch, SceneImage);
                SpriteBatch.GraphicsDevice.SetRenderTarget(0, null);

                /*Blur Horizontally*/
                SpriteBatch.GraphicsDevice.SetRenderTarget(0, renderTarget2);
                GaussianBlur(SpriteBatch, renderTarget1.GetTexture(), true);
                SpriteBatch.GraphicsDevice.SetRenderTarget(0, null);

                /*Blur Vertically*/
                SpriteBatch.GraphicsDevice.SetRenderTarget(0, renderTarget1);
                GaussianBlur(SpriteBatch, renderTarget2.GetTexture(), false);
                SpriteBatch.GraphicsDevice.SetRenderTarget(0, null);

                /*Combine our Extracted & Blurred Pixels w/Our Scene Image*/
                SpriteBatch.GraphicsDevice.SetRenderTarget(0, BloomTarget);
                BloomImage(SpriteBatch, renderTarget1.GetTexture(), SceneImage);
                SpriteBatch.GraphicsDevice.SetRenderTarget(0, null);
            }
        }
    }
}
