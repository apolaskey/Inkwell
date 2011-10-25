using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Inkwell.Framework
{
    /*Andrew Polaskey - 12.10.2010*/
    /// <summary>Data Class used for practically every model in Inkwel [Very Large however Optimized to the best of my ability also contains things it shouldn't].</summary>
    public class cModel: IComparable<cModel> //<-- We use the IComparable interface to take advantage of some sorting algorithims provided by Microsoft; it's basic and slow but it works
    { 
        /****************************************************************************************/
        #region Public Members
        /// <summary>(Model) The Model associated with this Object.</summary>
        public Model Model;
        /// <summary>(Vector3) Current Position of our Model.</summary>
        public Vector3 Position = Vector3.Zero;
        /// <summary>(Vector3) Old Position of our Model.</summary>
        public Vector3 OldPosition = Vector3.Zero;
        /// <summary>(Vector3) A Vector3 used to determine the Ambient Color of an Object [TIP]: Color.CHOICE.ToVector4()</summary>
        public Vector4 AmbientColor = Color.White.ToVector4();
        /// <summary>(Vector3) A Vector3 used to determine the Ambient Color of an Object [TIP]: Color.CHOICE.ToVector4()</summary>
        public Vector4 DiffuseColor = Color.White.ToVector4();
        /// <summary>(Vector3) Diffuse lighting direction [TIP: GPU Coords these are not World Cords]</summary>
        public Vector3 DiffuseDirection = new Vector3(0.0f, 1.0f, 1.0f);
        /// <summary>(float) Intensity of the Diffuse Lighting [TIP: Should not exceede 1.0f]</summary>
        public float DiffuseIntensity = .8f;
        /// <summary>(Bool) Enable the usage of Diffuse Lighting.</summary>
        public bool DiffuseEnabled = false;
        /// <summary>(Bool) If set to false this will use the Direction of the Diffuse Light determined by this Model.</summary>
        public bool EnableSunLighting = false;
        /// <summary>(float) Intensity of the Ambient Lighting [TIP: Should not exceede 1.0f]</summary>
        public float AmbientIntensity = 1.0f;
        /// <summary>(Matrix - Array) A Matrix containing the 3D Editor's Transforms.</summary>
        public Matrix[] Transforms;
        /// <summary>(Matrix) A Rotation Matrix used to rotate the Model [TIP: RotationMatrix *= Matrix.CreateRotation"VECTOR"(float amount)]</summary>
        public Matrix RotationMatrix = Matrix.Identity;
        /// <summary>(Matrix) A Scale Matrix used to grow or shrink the Model [TIP: ScalerMatrix = Matrix.CreateScale(float amount) -- Amounts going negative flip the model 1.0f is default 0.0f is a single pixel]</summary>
        public Matrix ScalerMatrix = Matrix.Identity;
        /// <summary>(Texture2D) The Texture Associated with this Model.</summary>
        public Texture2D Texture;
        /// <summary>(BoundingBox) The BoundingBox associated with this model.</summary>
        public BoundingBox BoundingBox;
        /// <summary>(Float) Determine the full transparency of the model.</summary>
        public float Opacity = 1.0f;
        /// <summary>(Bool) Flag the system for removal afterwards it should remain untouched as it interacts with other components.</summary>
        public bool Disposed = false;
        /// <summary>Used for various things; this value is automatic and should not be altered. --TODO: Flag as Readonly</summary>
        public bool Initialized = false;
        /// <summary>Determine whether the Model is Visible or not.</summary>
        public bool Display = true;
        /// <summary>Used for Various Things; this value is automatic and should not be altered. --TODO: Flag as Readonly</summary>
        public bool AlphaEnabled = false;
        /// <summary>(Vector2) Tile the Texture by a certain amount. [WARNING: At no point should the value be 0]</summary>
        public Vector2 TileAmount = new Vector2(1.0f, 1.0f);
        /// <summary>(Vector2) Slide the Texture by an amount. [Tip: A value between 0 - 1 is suggested to see results]</summary>
        public Vector2 MoveTexture = new Vector2(0.0f, 0.0f);
        /// <summary>(Bool) Flip the TexCoords Horizontally.</summary>
        public bool HorizontalTextureFlip = false;
        /// <summary>(Bool) Flip the TexCoords Vertically.</summary>
        public bool VerticalTextureFlip = false;
        /// <summary> (Bool) Depicts whether the object is movable or not.</summary>
        public bool IsMovable = false;
        /// <summary>(Bool) Determine if we are utilizing a Vegetation Processed Model. [WARNING: Set Automatically DO NOT TOUCH]</summary>
        public bool VegetationEnabled = false;
        /// <summary>(BoundingBox) The BoundingBox associated with this model. [WARNING: DO NOT USE THIS]</summary>
        public BoundingBox PureBoundingBox;
        /// <summary>Function used to correct bounding box issues [TIP: EXPERIMENTAL - I believe this will conflict with Physics]</summary>
        public String Type;

        public void UpdateBoundingBox()
        {
            BoundingBox.Max.X = PureBoundingBox.Max.X + Position.X;
            BoundingBox.Max.Y = PureBoundingBox.Max.Y + Position.Y;
            BoundingBox.Max.Z = PureBoundingBox.Max.Z + Position.Z;

            BoundingBox.Min.X = PureBoundingBox.Min.X + Position.X;
            BoundingBox.Min.Y = PureBoundingBox.Min.Y + Position.Y;
            BoundingBox.Min.Z = PureBoundingBox.Min.Z + Position.Z;
        }
        #endregion
        static BasicEffect effect; //<-- Used to prevent memory growth
        static bool ChangedStates = false;
        /****************************************************************************************/
        /// <summary>Standard Draw using the MasterEffect Shader.</summary>
        public static void Draw(cModel ModelInfo, Camera Camera)
        {
            /****************************************************************************************/
            /*This allows for animation*/
            ModelInfo.Model.CopyAbsoluteBoneTransformsTo(ModelInfo.Transforms);
            /****************************************************************************************/
            /*Assign a Texture ONLY if we are null*/
            if (ModelInfo.Texture == null)
                ModelInfo.Texture = Engine.NULLED_TEXTURE;
            if (ModelInfo.AlphaEnabled)
            {
                mGraphics.Peek.ToggleAlphaBlending(true);
                ChangedStates = true;
            }
            else
            {
                mGraphics.Peek.ToggleAlphaBlending(false);
                mGraphics.Peek.SetDepthBuffer(true);//<-- Something fucking broke w/Z-sort I got no clue...I just know this fixes it and I don't know why
            }
            /****************************************************************************************/
            for (int m = 0; m < ModelInfo.Model.Meshes.Count; m++)
            {
                for (int e = 0; e < ModelInfo.Model.Meshes[m].Effects.Count; e++) //<-- Meshes can have multiple effects
                {
                    switch (ModelInfo.Model.Meshes[m].Effects[e].CurrentTechnique.Name)
                    {
                        case "DrawModel":
                            SetMasterEffect(ModelInfo, Camera, m, e);
                            break;
                        case "Billboards":
                            SetVegEffect(ModelInfo, Camera, m, e);
                            ChangedStates = true;
                            break;
                        case "BasicEffect":
                            SetBasicEffect(ModelInfo, Camera, m, e);
                            break;
                    }  
                }
                ModelInfo.Model.Meshes[m].Draw(); //<-- After all of the Shader Params have been used Draw the object
            }
            /*Revert some States if we draw something special*/
            if (ChangedStates)
            {
                mGraphics.Peek.ToggleAlphaBlending(false);
                mGraphics.Peek.CullingState(CullMode.CullCounterClockwiseFace);
                ChangedStates = false;
            }
            /****************************************************************************************/
        }
        /// <summary>(void) Helper for setting the master effect.</summary>
        private static void SetMasterEffect(cModel ModelInfo, Camera Camera, int m, int e)
        {
            ModelInfo.Model.Meshes[m].Effects[e].Begin();
            for (int p = 0; p < ModelInfo.Model.Meshes[m].Effects[e].CurrentTechnique.Passes.Count; p++) //Effects can have multiple passes
            {
                ModelInfo.Model.Meshes[m].Effects[e].CurrentTechnique.Passes[p].Begin();
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["World"].SetValue(ModelInfo.Transforms[ModelInfo.Model.Meshes[m].ParentBone.Index] * ModelInfo.ScalerMatrix * ModelInfo.RotationMatrix * Matrix.CreateTranslation(ModelInfo.Position));
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["View"].SetValue(Camera.View);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["Project"].SetValue(Camera.Projection);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["AmbientColor"].SetValue(ModelInfo.AmbientColor);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["AmbientIntensity"].SetValue(ModelInfo.AmbientIntensity);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["DiffuseEnabled"].SetValue(ModelInfo.DiffuseEnabled);
                if (ModelInfo.DiffuseEnabled)
                {
                    if (ModelInfo.EnableSunLighting)
                    {
                        ModelInfo.Model.Meshes[m].Effects[e].Parameters["DiffuseColor"].SetValue(mEffect.Peek.SunColor.ToVector4());
                        ModelInfo.Model.Meshes[m].Effects[e].Parameters["DiffuseLightDirection"].SetValue(mEffect.Peek.SunPosition);
                        ModelInfo.Model.Meshes[m].Effects[e].Parameters["DiffuseIntensity"].SetValue(mEffect.Peek.SunIntensity);
                    }
                    else
                    {
                        ModelInfo.Model.Meshes[m].Effects[e].Parameters["DiffuseColor"].SetValue(ModelInfo.DiffuseColor);
                        ModelInfo.Model.Meshes[m].Effects[e].Parameters["DiffuseLightDirection"].SetValue(ModelInfo.DiffuseDirection);
                        ModelInfo.Model.Meshes[m].Effects[e].Parameters["DiffuseIntensity"].SetValue(ModelInfo.DiffuseIntensity);
                    }
                }
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["ModelTexture"].SetValue(ModelInfo.Texture);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["TextureScaleX"].SetValue(ModelInfo.TileAmount.X);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["TextureScaleY"].SetValue(ModelInfo.TileAmount.Y);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["TextureOffsetX"].SetValue(ModelInfo.MoveTexture.X);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["TextureOffsetY"].SetValue(ModelInfo.MoveTexture.Y);
                if (!ModelInfo.VerticalTextureFlip)
                    ModelInfo.Model.Meshes[m].Effects[e].Parameters["VerticalTexCoord"].SetValue(1);
                else ModelInfo.Model.Meshes[m].Effects[e].Parameters["VerticalTexCoord"].SetValue(-1);
                if (!ModelInfo.HorizontalTextureFlip)
                    ModelInfo.Model.Meshes[m].Effects[e].Parameters["HorizontalTexCoord"].SetValue(1);
                else ModelInfo.Model.Meshes[m].Effects[e].Parameters["HorizontalTexCoord"].SetValue(-1);
                ModelInfo.Model.Meshes[m].Effects[e].CommitChanges();
                ModelInfo.Model.Meshes[m].Effects[e].CurrentTechnique.Passes[p].End();
            }
            ModelInfo.Model.Meshes[m].Effects[e].End();
        }
        /// <summary>(void) Helper for setting the vegetation effect.</summary>
        private static void SetVegEffect(cModel ModelInfo, Camera Camera, int m, int e)
        {
            ModelInfo.Model.Meshes[m].Effects[e].Begin();
            for (int p = 0; p < ModelInfo.Model.Meshes[m].Effects[e].CurrentTechnique.Passes.Count; p++)
            {
                ModelInfo.Model.Meshes[m].Effects[e].CurrentTechnique.Passes[p].Begin();
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["World"].SetValue(ModelInfo.Transforms[ModelInfo.Model.Meshes[m].ParentBone.Index]);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["View"].SetValue(mCamera.Peek.ReturnCamera().View);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["Projection"].SetValue(mCamera.Peek.ReturnCamera().Projection);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["v3Position"].SetValue(ModelInfo.Position);
                ModelInfo.Model.Meshes[m].Effects[e].Parameters["WindTime"].SetValue(mTimer.Peek.ElapsedTotalGameTime.Seconds / 3f);
                ModelInfo.Model.Meshes[m].Effects[e].CurrentTechnique.Passes[p].End();
            }
            ModelInfo.Model.Meshes[m].Effects[e].End();
        }
        /// <summary>Helper to setup fully 3D models w/Baked Textures (non UVW Mapped)</summary>
        private static void SetBasicEffect(cModel ModelInfo, Camera Camera, int m, int e)
        {
            /*Basically checks the effect to see if it's a BasicEffect*/
            effect = ModelInfo.Model.Meshes[m].Effects[e] as BasicEffect; //<-- Somewhat like a foreach check this is some crazy shit (if it's not a basiceffect it's null)
            effect.EnableDefaultLighting();
            effect.DiffuseColor = Engine.TempVector3(ModelInfo.DiffuseColor.X, ModelInfo.DiffuseColor.Y, ModelInfo.DiffuseColor.Z);
            effect.AmbientLightColor = Engine.TempVector3(ModelInfo.AmbientColor.X, ModelInfo.AmbientColor.Y, ModelInfo.AmbientColor.Z);
            effect.PreferPerPixelLighting = true;
            effect.View = Camera.View;
            effect.Projection = Camera.Projection;
            effect.World = ModelInfo.Transforms[ModelInfo.Model.Meshes[m].ParentBone.Index] * ModelInfo.ScalerMatrix * ModelInfo.RotationMatrix * Matrix.CreateTranslation(ModelInfo.Position);
        }
        /// <summary>Loader to Spawn a cModel (useful for non-sorted things or sort controlled items</summary>
        public static void Load(ContentManager Content, cModel ModelInfo, string strAssetLocation, Vector3 v3StartingPosition)
        {
            ModelInfo.Model = Content.Load<Model>(strAssetLocation);
            ModelInfo.Transforms = new Matrix[ModelInfo.Model.Bones.Count];
            ModelInfo.Position.X = v3StartingPosition.X;
            ModelInfo.Position.Y = v3StartingPosition.Y;
            ModelInfo.Position.Z = v3StartingPosition.Z == 0.0f ? (0.0f) : (-v3StartingPosition.Z);
            ModelInfo.Initialized = true;

            /*Create our Bounding Box for this Model*/
            ModelInfo.Model.CopyAbsoluteBoneTransformsTo(ModelInfo.Transforms);

            foreach (ModelMesh mesh in ModelInfo.Model.Meshes)
            {
                VertexPositionNormalTexture[] vertices =
                new VertexPositionNormalTexture[mesh.VertexBuffer.SizeInBytes / VertexPositionNormalTexture.SizeInBytes];

                mesh.VertexBuffer.GetData<VertexPositionNormalTexture>(vertices);

                Vector3[] vertexs = new Vector3[vertices.Length];

                for (int index = 0; index < vertexs.Length; index++)
                    vertexs[index] = vertices[index].Position;

                Matrix M2 = ModelInfo.Transforms[mesh.ParentBone.Index];//<-- Dummy Matrix, this is a loader function so screw the GC

                Vector3.Transform(vertexs, ref M2, vertexs);
                ModelInfo.PureBoundingBox = BoundingBox.CreateMerged(ModelInfo.BoundingBox, BoundingBox.CreateFromPoints(vertexs));
            }
            /*Assign Our Effect to this Model*/
            foreach (ModelMesh mesh in ModelInfo.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = mEffect.Peek.MasterEffect();
                }
            }
        }
        /*Compare method so that we can sort these bad boys*/
        public int CompareTo(cModel Other)
        {
            float a = MathHelper.Distance(mCamera.Peek.ReturnCamera().Position.Z, this.Position.Z);
            float b = MathHelper.Distance(mCamera.Peek.ReturnCamera().Position.Z, Other.Position.Z);
            /*General Rule to Z-sort is sort based upon the distance an object is away from the camera
             * Alpha Objects get drawn back-front and Opaque Objects get drawn front-back if we use
             * XNA's Pixel Rejection (Depth Write) we can turn it on and have it calculate the Pixel depth
             * to do the rest of the work for us a better solution would to stray away from that system.
             * 
             * Another Downside to this implementation is that it is PER-OBJECT so full transparency will look
             * out of place or sides may be pissing this method is useful for basic windows but glass or see-through
             * full 3D models won't appear correct unless meshed correctly.*/
            if (this.AlphaEnabled)
            {
                if (a > b)
                    return -1; //<-- If THIS object is farther away from the camera --it's element location
                if (b > a)
                    return 1; //<-- If the OTHER object is farther away push THIS object ++it's element location
            }
            else //<-- This is used for checking Opaque Objects
            {
                if (a > b)
                    return 1; //<-- If the THIS object is farther ++it's element location
                else if (a < b)
                    return -1; //<-- If THIS object is closer to the camera --it's element location
            }
            return 0;
        }
        /*EOF*/
    }
}
