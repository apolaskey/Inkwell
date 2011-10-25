using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Inkwell.Framework
{
    static class Skybox
    {
        private static Effect _SkyboxEffect;
        public static TextureCube SkyboxTexture;
        public static Model SkyboxModel;
        private static int TextureOffsetX, TextureOffsetY;
        public static float SkyboxSize = 100.0f;
        public static void Initialize(ContentManager Content, string strEffectLocation, string strModelLocation, string strTextureLocation)
        {
            _SkyboxEffect = Content.Load<Effect>(strEffectLocation);
            SkyboxModel = Content.Load<Model>(strModelLocation);
            SkyboxTexture = Content.Load<TextureCube>(strTextureLocation);
            TextureOffsetX = TextureOffsetY = 0;

            for (int m = 0; m < SkyboxModel.Meshes.Count; m++)
                for (int p = 0; p < SkyboxModel.Meshes[m].MeshParts.Count; p++)
                    SkyboxModel.Meshes[m].MeshParts[p].Effect = _SkyboxEffect;
        }
        public static void Update(int iXAmount, int iYAmount)
        {
            TextureOffsetX = iXAmount; 
            TextureOffsetY = iYAmount;
        }
        public static void Draw(Matrix View, Matrix Projection, Vector3 CameraPosition)
        {
            mGraphics.Peek.SetDepthBuffer(false);
            mGraphics.Peek.CullingState(CullMode.CullClockwiseFace);
            for (int m = 0; m < SkyboxModel.Meshes.Count; m++)
            {
                for (int e = 0; e < SkyboxModel.Meshes[m].Effects.Count; e++)
                {
                    SkyboxModel.Meshes[m].Effects[e].Begin();
                    SkyboxModel.Meshes[m].Effects[e].CurrentTechnique.Passes[0].Begin();
                    SkyboxModel.Meshes[m].Effects[e].Parameters["World"].SetValue(Matrix.CreateScale(SkyboxSize) * Matrix.CreateTranslation(CameraPosition));

                    SkyboxModel.Meshes[m].Effects[e].Parameters["View"].SetValue(View);
                    SkyboxModel.Meshes[m].Effects[e].Parameters["Projection"].SetValue(Projection);

                    SkyboxModel.Meshes[m].Effects[e].Parameters["SkyBoxTexture"].SetValue(SkyboxTexture);

                    SkyboxModel.Meshes[m].Effects[e].Parameters["CameraPosition"].SetValue(CameraPosition);

                    SkyboxModel.Meshes[m].Effects[e].CurrentTechnique.Passes[0].End();
                    SkyboxModel.Meshes[m].Effects[e].End();
                }
                SkyboxModel.Meshes[m].Draw();
            }
            mGraphics.Peek.CullingState(CullMode.CullCounterClockwiseFace);
            mGraphics.Peek.SetDepthBuffer(true);
        }

    }//<-- EOF
}
