using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Inkwell.Framework
{
    class BloomSettings
    {
        // Name of a preset bloom setting, for display to the user.
        public readonly string Name;


        // Controls how bright a pixel needs to be before it will bloom.
        // Zero makes everything bloom equally, while higher values select
        // only brighter colors. Somewhere between 0.25 and 0.5 is good.
        public readonly float BloomThreshold;


        // Controls how much blurring is applied to the bloom image.
        // The typical range is from 1 up to 10 or so.
        public readonly float BlurAmount;


        // Controls the amount of the bloom and base images that
        // will be mixed into the final scene. Range 0 to 1.
        public readonly float BloomIntensity;
        public readonly float BaseIntensity;


        // Independently control the color saturation of the bloom and
        // base images. Zero is totally desaturated, 1.0 leaves saturation
        // unchanged, while higher values increase the saturation level.
        public readonly float BloomSaturation;
        public readonly float BaseSaturation;


        public BloomSettings(string name, float bloomThreshold, float blurAmount,
                             float bloomIntensity, float baseIntensity,
                             float bloomSaturation, float baseSaturation)
        {
            Name = name;
            BloomThreshold = bloomThreshold;
            BlurAmount = blurAmount;
            BloomIntensity = bloomIntensity;
            BaseIntensity = baseIntensity;
            BloomSaturation = bloomSaturation;
            BaseSaturation = baseSaturation;
        }
        //new BloomSettings("Contrasted",    1.0f, 1.0f, 1.0f, 1.0f, 6.0f, 2.0f)
        //new BloomSettings("Contrasted",    0.9f, 4.0f, 1.5f, 1.0f, 6.0f, 2.0f)
        //new BloomSettings("Subtle",      0.7f,   4.0f,   1.5f,     .95f,    .8f,       .95f)
        /// <summary>
        /// Table of preset bloom settings, used by the sample program.
        /// </summary>
        public static BloomSettings[] PresetSettings =
        {
            //                Name             Thresh  Blur    Bloom  Base     BloomSat    BaseSat
            //new BloomSettings("Contrasted Bloom",    0.7f, 4.0f, 1.5f, 0.95f, 0.8f, 2.0f)
            new BloomSettings("Subtle",      0.8f,   4.0f,   1.5f,     .8f,    1.0f,       1.5f)
        };
    }//END
}
