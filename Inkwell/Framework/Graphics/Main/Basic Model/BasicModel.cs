using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Inkwell.Framework
{
    /*Andrew Polaskey - 12.10.2010*/
    public enum ModelProperties { Alpha, Opaque, Vegetation, Full3D };
    /// <summary>(Class) Model Pipeline for various managed types.</summary>
    public class BasicModel
    {
        /*****************************************HEADER*****************************************/
        protected int _ID = Engine.NULLED_INT; //<-- It's "null" value
        protected ModelProperties _Properties;
        /// Constructor for the BasicModel being created. (Quick Load)
        /// </summary>
        /// <param name="TextureType">Flag for the Texture, if at any point in time it will be alpha make it so.</param>
        /// <param name="strAssetLocation">Asset Location of our Model.</param>
        /// <param name="v3StartingPos">Location in 3D space where the model will start in.</param>
        public BasicModel(ContentManager Content, ModelProperties TextureType, String strAssetLocation, Vector3 v3StartingPos)
        {
            this._Properties = TextureType;
            if (_ID == Engine.NULLED_INT)
            {
                switch (_Properties)
                {
                    case ModelProperties.Alpha:
                        this._ID = mModel.Peek.CreateMessageIDAlpha();
                        PrepareModel(Content, mModel.Peek.AlphaContainer, this, strAssetLocation, v3StartingPos);
                        HelpCreateBoundingBox(mModel.Peek.AlphaContainer, this);
                        HelpPrepareMasterEffect(mModel.Peek.AlphaContainer, this);
                        break;
                    case ModelProperties.Opaque:
                        this._ID = mModel.Peek.CreateMessageIDOpaque();
                        PrepareModel(Content, mModel.Peek.OpaqueContainer, this, strAssetLocation, v3StartingPos);
                        HelpCreateBoundingBox(mModel.Peek.OpaqueContainer, this);
                        HelpPrepareMasterEffect(mModel.Peek.OpaqueContainer, this);
                        break;
                    case ModelProperties.Full3D:
                        this._ID = mModel.Peek.CreateMessageIDOpaque();
                        PrepareModel(Content, mModel.Peek.OpaqueContainer, this, strAssetLocation, v3StartingPos);
                        HelpCreateBoundingBox(mModel.Peek.OpaqueContainer, this);
                        break;
                    case ModelProperties.Vegetation:
                        this._ID = mModel.Peek.CreateMessageIDOpaque();
                        PrepareModel(Content, mModel.Peek.OpaqueContainer, this, strAssetLocation, v3StartingPos);
                        Dictionary<string, object> tagData = (Dictionary<string, object>)mModel.Peek.OpaqueContainer[this._ID].Model.Tag;
                        mModel.Peek.OpaqueContainer[this._ID].PureBoundingBox = (BoundingBox)tagData["BoundingBox"];
                        mModel.Peek.OpaqueContainer[this._ID].BoundingBox = mModel.Peek.OpaqueContainer[this._ID].PureBoundingBox;
                        break;
                }
            }
        }
        /****************************************FUNCTIONS***************************************/
        /// <summary>(INT) ID to the location in Memory of the Model.</summary>
        public int ID
        {
            get { return this._ID; }
        }
        /****************************************************************************************/
        /// <summary>(cModel) Snags a direct link to the BasicModel in question.</summary>
        public cModel Link
        {
            get
            {
                switch (_Properties)
                {
                    case ModelProperties.Alpha:
                        return mModel.Peek.AlphaContainer[_ID];
                    case ModelProperties.Full3D:
                        return mModel.Peek.OpaqueContainer[_ID];
                    case ModelProperties.Opaque:
                        return mModel.Peek.OpaqueContainer[_ID];
                    case ModelProperties.Vegetation:
                        return mModel.Peek.AlphaContainer[_ID];
                    default:
                        return null; //<-- Shouldn't occur but the compilier was being gay
                }
            }
            set
            {
                switch (_Properties)
                {
                    case ModelProperties.Alpha:
                        mModel.Peek.AlphaContainer[_ID] = value;
                        break;
                    case ModelProperties.Full3D:
                        mModel.Peek.OpaqueContainer[_ID] = value;
                        break;
                    case ModelProperties.Opaque:
                        mModel.Peek.OpaqueContainer[_ID] = value;
                        break;
                    case ModelProperties.Vegetation:
                        mModel.Peek.AlphaContainer[_ID] = value;
                        break;
                }
            }
        }

        /****************************************************************************************/
        /// <summary>Sets some Defaults for the Model [TIP: This is generally shared among most models]</summary>
        private static void PrepareModel(ContentManager Content, cModel[] Container, BasicModel Model, string strAssetLocation, Vector3 v3StartingPosition)
        {
            Container[Model._ID].Model = Content.Load<Model>(strAssetLocation);
            Container[Model._ID].Transforms = new Matrix[Container[Model._ID].Model.Bones.Count];
            Container[Model._ID].Position.X = v3StartingPosition.X;
            Container[Model._ID].Position.Y = v3StartingPosition.Y;
            Container[Model._ID].Position.Z = v3StartingPosition.Z == 0.0f ? (0.0f) : (-v3StartingPosition.Z);
            Container[Model._ID].Initialized = true;
        }
        /****************************************************************************************/
        /// <summary>Helps setup the MasterEffect on the Model [INFO: This override the built in BasicEffect with our new effect, could of been done through the Content Pipeline]</summary>
        private static void HelpPrepareMasterEffect(cModel[] Container, BasicModel Model)
        {
            foreach (ModelMesh mesh in Container[Model._ID].Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = mEffect.Peek.MasterEffect();
                }
            }
        }
        /****************************************************************************************/
        /// <summary>(void) Helper to generate Bounding Boxes.</summary>
        private static void HelpCreateBoundingBox(cModel[] Container, BasicModel Model)
        {
            /*Create our Bounding Box for this Model*/
            Container[Model._ID].Model.CopyAbsoluteBoneTransformsTo(Container[Model._ID].Transforms);

            foreach (ModelMesh mesh in Container[Model._ID].Model.Meshes)
            {
                VertexPositionNormalTexture[] vertices =
                new VertexPositionNormalTexture[mesh.VertexBuffer.SizeInBytes / VertexPositionNormalTexture.SizeInBytes];

                mesh.VertexBuffer.GetData<VertexPositionNormalTexture>(vertices);

                Vector3[] vertexs = new Vector3[vertices.Length];

                for (int index = 0; index < vertexs.Length; index++)
                    vertexs[index] = vertices[index].Position;

                Matrix M2 = Container[Model._ID].Transforms[mesh.ParentBone.Index];//<-- Dummy Matrix

                Vector3.Transform(vertexs, ref M2, vertexs);
                Container[Model._ID].BoundingBox = BoundingBox.CreateMerged(Container[Model._ID].BoundingBox, BoundingBox.CreateFromPoints(vertexs));
                Container[Model._ID].PureBoundingBox = Container[Model._ID].BoundingBox;
            }
        }
        /****************************************************************************************/
        /// <summary>(void) Helper to generate Bounding Boxes.</summary>
        private static void HelpCreateBoundingBox(cModel[] Container, BasicModel Model, Model CopyFromModel)
        {
            /*Create our Bounding Box for this Model*/
            CopyFromModel.CopyAbsoluteBoneTransformsTo(Container[Model._ID].Transforms);

            foreach (ModelMesh mesh in CopyFromModel.Meshes)
            {
                VertexPositionNormalTexture[] vertices =
                new VertexPositionNormalTexture[mesh.VertexBuffer.SizeInBytes / VertexPositionNormalTexture.SizeInBytes];

                mesh.VertexBuffer.GetData<VertexPositionNormalTexture>(vertices);

                Vector3[] vertexs = new Vector3[vertices.Length];

                for (int index = 0; index < vertexs.Length; index++)
                {
                    vertexs[index] = vertices[index].Position;
                }
                Matrix M2 = Container[Model._ID].Transforms[mesh.ParentBone.Index];//<-- Dummy Matrix

                Vector3.Transform(vertexs, ref M2, vertexs);
                Container[Model._ID].BoundingBox = BoundingBox.CreateMerged(Container[Model._ID].BoundingBox, BoundingBox.CreateFromPoints(vertexs));
                Container[Model._ID].PureBoundingBox = Container[Model._ID].BoundingBox;
            }
        }
        /****************************************************************************************/
        /// <summary>(Vegetation) Create's a Axis-Aligned Bounding Box for the Object utilizing the Correct Matrix Transforms.</summary>
        public static void CreateBoundingBox(BasicModel Model, Model ModelToCopyBoxFrom)
        {
            switch (Model._Properties)
            {
                case ModelProperties.Alpha:
                    HelpCreateBoundingBox(mModel.Peek.AlphaContainer, Model, ModelToCopyBoxFrom);
                    break;
                case ModelProperties.Opaque:
                    HelpCreateBoundingBox(mModel.Peek.OpaqueContainer, Model, ModelToCopyBoxFrom);
                    break;
                case ModelProperties.Full3D:
                    HelpCreateBoundingBox(mModel.Peek.OpaqueContainer, Model, ModelToCopyBoxFrom);
                    break;
                case ModelProperties.Vegetation:
                    HelpCreateBoundingBox(mModel.Peek.AlphaContainer, Model, ModelToCopyBoxFrom);
                    break;
            }
        }
        /****************************************************************************************/
        /// <summary>Tell the ModelManager to flag this model for deletion.</summary>
        public static void Remove(BasicModel Model)
        {
            if (Model._Properties == ModelProperties.Alpha)
            {
                mModel.Peek.AlphaContainer[Model._ID].Disposed = true;
                mModel.Peek.AlphaContainer[Model._ID].Display = false;
            }
            else
            {
                mModel.Peek.OpaqueContainer[Model._ID].Disposed = true;
                mModel.Peek.OpaqueContainer[Model._ID].Display = false;
            }
        }
        /******************************************EOF*******************************************/
    }
}