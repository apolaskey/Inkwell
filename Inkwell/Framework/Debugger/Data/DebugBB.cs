using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Inkwell.Framework
{
    /// <summary>
    /// Provides a set of methods for the rendering BoundingBoxs.
    /// </summary>
    public static class DebugBB
    {
        /****************************************************************************************/
        static VertexPositionColor[] verts = new VertexPositionColor[8];
        static int[] indices = new int[]
        {
            0, 1,
            1, 2,
            2, 3,
            3, 0,
            0, 4,
            1, 5,
            2, 6,
            3, 7,
            4, 5,
            5, 6,
            6, 7,
            7, 4,
        };
        static BasicEffect effect;
        static VertexDeclaration vertDecl;
        static Vector3[] corners;
        public static bool DebugBoxes = true;
        /****************************************************************************************/
        /// <summary>
        /// Renders the bounding box for debugging purposes.
        /// </summary>
        /// <param name="box">The box to render.</param>
        /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
        /// <param name="view">The current view matrix.</param>
        /// <param name="projection">The current projection matrix.</param>
        /// <param name="color">The color to use drawing the lines of the box.</param>
        public static void Draw(cModel Model, Color color)
        {
            if (DebugBoxes)
            {
                if (effect == null)
                {
                    effect = new BasicEffect(mGraphics.Peek.Device(), null);
                    effect.VertexColorEnabled = true;
                    effect.LightingEnabled = false;
                    vertDecl = new VertexDeclaration(mGraphics.Peek.Device(), VertexPositionColor.VertexElements);
                }

                corners = new Vector3[Model.BoundingBox.GetCorners().Length];
                corners = Model.BoundingBox.GetCorners();

                for (int i = 0; i < 8; i++)
                {
                    verts[i].Position = corners[i] + Model.Position;
                    verts[i].Color = color;
                }

                mGraphics.Peek.Device().VertexDeclaration = vertDecl;

                effect.View = mCamera.Peek.ReturnCamera().View;
                effect.Projection = mCamera.Peek.ReturnCamera().Projection;

                effect.Begin();
                for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
                {
                    effect.CurrentTechnique.Passes[i].Begin();
                    mGraphics.Peek.Device().DrawUserIndexedPrimitives(PrimitiveType.LineList, verts, 0, 8, indices, 0, indices.Length / 2);
                    effect.CurrentTechnique.Passes[i].End();
                }
                effect.End();
            }
        }
        public static void Draw(BoundingBox Box, Color color)
        {
            if (DebugBoxes)
            {
                if (effect == null)
                {
                    effect = new BasicEffect(mGraphics.Peek.Device(), null);
                    effect.VertexColorEnabled = true;
                    effect.LightingEnabled = false;
                    vertDecl = new VertexDeclaration(mGraphics.Peek.Device(), VertexPositionColor.VertexElements);
                }

                corners = new Vector3[Box.GetCorners().Length];
                corners = Box.GetCorners();

                for (int i = 0; i < 8; i++)
                {
                    verts[i].Position = corners[i];
                    verts[i].Color = color;
                }

                mGraphics.Peek.Device().VertexDeclaration = vertDecl;

                effect.View = mCamera.Peek.ReturnCamera().View;
                effect.Projection = mCamera.Peek.ReturnCamera().Projection;

                effect.Begin();
                for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
                {
                    effect.CurrentTechnique.Passes[i].Begin();
                    mGraphics.Peek.Device().DrawUserIndexedPrimitives(PrimitiveType.LineList, verts, 0, 8, indices, 0, indices.Length / 2);
                    effect.CurrentTechnique.Passes[i].End();
                }
                effect.End();
            }
        }
        /****************************************************************************************/
    }
}
