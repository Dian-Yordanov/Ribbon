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

using Ribbon.Utilities;

namespace Ribbon
{
    public class Base3DObject : DrawableGameComponent
    {
        string modelName;
        string shaderName;

        public Base3DCamera camera
        {
            get { return (Base3DCamera)Game.Services.GetService(typeof(Base3DCamera)); }
        }

        /// <summary>
        /// Position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Scale
        /// </summary>
        public Vector3 Scale;

        /// <summary>
        /// Rotation
        /// </summary>
        public Quaternion Rotation;

        Model mesh;

        /// <summary>
        /// World
        /// </summary>
        public Matrix World;
#if WINDOWS
        public Effect Effect;
#endif
#if WINDOWS_PHONE
        public BasicEffect Effect;
#endif
        public string TextureName;

        Matrix[] transforms;
        Matrix meshWorld;
        Matrix meshWVP;

        public Vector3 LightPosition = new Vector3(10, 10, 0);
        public Color AmbientColor;
        public Color DiffuseColor;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="modelAssetName"></param>
        /// <param name="shaderAssetName"></param>
        public Base3DObject(Game game, string modelAssetName, string shaderAssetName, string textureAssetName)
            : base(game)
        {
            Position = Vector3.Zero;
            Scale = Vector3.One;
            Rotation = Quaternion.Identity;
            TextureName = textureAssetName;
            modelName = modelAssetName;
            shaderName = shaderAssetName;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            World = Matrix.CreateScale(Scale) *
                      Matrix.CreateFromQuaternion(Rotation) *
                      Matrix.CreateTranslation(Position);

            if (mesh == null && modelName != string.Empty)
            {
                mesh = Game.Content.Load<Model>(modelName);

                transforms = new Matrix[mesh.Bones.Count];
                mesh.CopyAbsoluteBoneTransformsTo(transforms);
            }
#if WINDOWS
            if (Effect == null)
                Effect = Game.Content.Load<Effect>(shaderName);
#endif
#if WINDOWS_PHONE
            if (Effect == null)
                Effect = new BasicEffect(Game.GraphicsDevice);
#endif
        }


        /// <summary>
        /// Method to translate object
        /// </summary>
        /// <param name="distance"></param>
        public void Translate(Vector3 distance)
        {
            Position += GameComponentHelper.Translate3D(distance, Rotation);
        }
        public void LookAt(Vector3 target, float speed)
        {
            GameComponentHelper.LookAt(target, speed, Position, ref Rotation, Vector3.Forward);
        }
        /// <summary>
        /// Method to rotate object
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public void Rotate(Vector3 axis, float angle)
        {
            GameComponentHelper.Rotate(axis, angle, ref Rotation);
        }
#if WINDOWS
        public virtual void Draw(GameTime gameTime, Effect effect)
#endif
#if WINDOWS_PHONE
        public virtual void Draw(GameTime gameTime, BasicEffect effect)
#endif
        {
            Game.GraphicsDevice.BlendState = BlendState.Opaque;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

#if WINDOWS
            if (effect.Parameters["AmbientColor"] != null)
            {
                effect.Parameters["AmbientIntensity"].SetValue(.25f);
                effect.Parameters["AmbientColor"].SetValue(DiffuseColor.ToVector4());
            }

            if (effect.Parameters["DiffuseColor"] != null)
            {
                effect.Parameters["DiffuseIntensity"].SetValue(1);
                effect.Parameters["DiffuseColor"].SetValue(DiffuseColor.ToVector4());
            }

            if (effect.Parameters["CameraPosition"] != null)
                effect.Parameters["CameraPosition"].SetValue(camera.Position);

            if (effect.Parameters["LightDirection"] != null)
                effect.Parameters["LightDirection"].SetValue(LightPosition - Position);
#endif
#if WINDOWS_PHONE
            effect.AmbientLightColor = AmbientColor.ToVector3();
            effect.DiffuseColor = DiffuseColor.ToVector3();
            effect.DirectionalLight0.Direction = Vector3.Up + Vector3.Left;
            effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();
            effect.LightingEnabled = true;
            effect.Projection = camera.Projection;
            effect.SpecularColor = Color.White.ToVector3();
            effect.SpecularPower = 35;
            effect.PreferPerPixelLighting = true;
            effect.View = camera.View;
#endif

            foreach (ModelMesh meshM in mesh.Meshes)
            {
                // Do the world stuff. 
                // Scale * transform * pos * rotation
                meshWorld = transforms[meshM.ParentBone.Index] * World;
                meshWVP = meshWorld * camera.View * camera.Projection;
#if WINDOWS
                if (effect.Parameters["world"] != null)
                    effect.Parameters["world"].SetValue(meshWorld);
                if (effect.Parameters["wvp"] != null)
                    effect.Parameters["wvp"].SetValue(meshWVP);
#endif
#if WINDOWS_PHONE
                effect.World = meshWorld;
#endif
                effect.CurrentTechnique.Passes[0].Apply();

                foreach (ModelMeshPart meshPart in meshM.MeshParts)
                {
                    Game.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                    Game.GraphicsDevice.Indices = meshPart.IndexBuffer;
                    Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.VertexOffset, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                }
            }

        }
        /// <summary>
        /// Draw method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            Draw(gameTime, Effect);
        }
    }
}
