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

namespace Ribbon
{
    
    public class Ribbon : DrawableGameComponent
    {
        public Color AmbientColor;
        public Color DiffuseColor;
        public Vector3 Top;
        public Vector3 Bottom;

        VertexPositionNormalTexture[] verts;

#if WINDOWS
        Effect effect;
#endif
#if WINDOWS_PHONE
        BasicEffect effect;
#endif
        //DynamicIndexBuffer ib;
        DynamicVertexBuffer vb;

        short[] index;
        short[] indexBlock = new short[] { 0, 1, 2, 2, 3, 0 };

        public int Count
        {
            get 
            {
                if (verts != null)
                    return verts.Length / 4;
                else
                    return 0;
            }
        }

        public Ribbon(Game game) : base(game)
        { }

        protected override void LoadContent()
        {
            base.LoadContent();
#if WINDOWS
            effect = Game.Content.Load<Effect>("Shaders/RibbonShader");
#endif
#if WINDOWS_PHONE
            effect = new BasicEffect(Game.GraphicsDevice);
#endif
        }

        public void AddSection(Vector3 position,Quaternion rotation)
        {
            Vector3 TopStart = position + ((Vector3.Up + Vector3.Left) * .5f);
            Vector3 TopEnd = position + ((Vector3.Up + Vector3.Right) * .5f);
            Vector3 BottomStart = position + ((Vector3.Down + Vector3.Left) * .5f);
            Vector3 BottomEnd = position + ((Vector3.Down + Vector3.Right) * .5f);
            
            VertexPositionNormalTexture[] vertsOld = verts;
            short[] indexOld = index;

            TopEnd = Vector3.Transform(TopEnd, Matrix.CreateFromQuaternion(rotation));
            BottomEnd = Vector3.Transform(BottomEnd, Matrix.CreateFromQuaternion(rotation));

            if (vertsOld != null)
            {
                verts = new VertexPositionNormalTexture[vertsOld.Length + 4];
                vertsOld.CopyTo(verts, 0);
                
                BottomStart = vertsOld[vertsOld.Length - 4].Position;
                TopStart = vertsOld[vertsOld.Length-1].Position;

                // Bottom Right
                verts[vertsOld.Length + 0] = new VertexPositionNormalTexture();
                verts[vertsOld.Length + 0].Position = BottomEnd;
                // Bottom Left
                verts[vertsOld.Length + 1] = new VertexPositionNormalTexture();
                verts[vertsOld.Length + 1].Position = BottomStart;
                // Top Left
                verts[vertsOld.Length + 2] = new VertexPositionNormalTexture();
                verts[vertsOld.Length + 2].Position = TopStart;
                // Top Right
                verts[vertsOld.Length + 3] = new VertexPositionNormalTexture();
                verts[vertsOld.Length + 3].Position = TopEnd;

                index = new short[indexOld.Length + 6];
                indexOld.CopyTo(index, 0);
            }
            else
            {
                verts = new VertexPositionNormalTexture[4];

                // Bottom Right
                verts[0] = new VertexPositionNormalTexture();
                verts[0].Position = BottomEnd;
                // Bottom Left
                verts[1] = new VertexPositionNormalTexture();
                verts[1].Position = BottomStart;
                // Top Left
                verts[2] = new VertexPositionNormalTexture();
                verts[2].Position = TopStart;
                // Top Right
                verts[3] = new VertexPositionNormalTexture();
                verts[3].Position = TopEnd;

                index = new short[6];
            }

            // Normals
            verts[verts.Length - 4].Normal = Vector3.Normalize(Vector3.Cross(verts[verts.Length - 4].Position - verts[verts.Length - 1].Position, verts[verts.Length - 4].Position - verts[verts.Length - 3].Position));
            verts[verts.Length - 3].Normal = Vector3.Normalize(Vector3.Cross(verts[verts.Length - 3].Position - verts[verts.Length - 4].Position, verts[verts.Length - 3].Position - verts[verts.Length - 2].Position));
            verts[verts.Length - 2].Normal = Vector3.Normalize(Vector3.Cross(verts[verts.Length - 2].Position - verts[verts.Length - 3].Position, verts[verts.Length - 2].Position - verts[verts.Length - 1].Position));
            verts[verts.Length - 1].Normal = Vector3.Normalize(Vector3.Cross(verts[verts.Length - 1].Position - verts[verts.Length - 2].Position, verts[verts.Length - 1].Position - verts[verts.Length - 4].Position));

            // Texcooreds
            verts[verts.Length - 4].TextureCoordinate = new Vector2(1, 1);
            verts[verts.Length - 3].TextureCoordinate = new Vector2(0, 1);
            verts[verts.Length - 2].TextureCoordinate = new Vector2(0, 0);
            verts[verts.Length - 1].TextureCoordinate = new Vector2(1, 0);

            if (indexOld != null)
            {
                for (int i = 0; i < indexBlock.Length; i++)
                {
                    index[(indexOld.Length) + i] = (short)(indexBlock[i] + (4 * (Count - 1)));
                }                
            }
            else
                index = indexBlock;

            vb = new DynamicVertexBuffer(Game.GraphicsDevice, typeof(VertexPositionNormalTexture), verts.Length, BufferUsage.WriteOnly);
            vb.SetData(verts);
        }

        public void Reset()
        {
            vb = null;
            index = null;
            verts = null;
        }

        public Base3DCamera camera
        {
            get { return (Base3DCamera)Game.Services.GetService(typeof(Base3DCamera)); }
        }
        public override void Update(GameTime gameTime)
        {
#if WINDOWS
            effect.Parameters["world"].SetValue(Matrix.Identity);
            effect.Parameters["wvp"].SetValue(Matrix.Identity * camera.View * camera.Projection);
            effect.Parameters["CameraPosition"].SetValue(camera.Position);            
#endif
#if WINDOWS_PHONE
            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.AmbientLightColor = AmbientColor.ToVector3();
            effect.DiffuseColor = DiffuseColor.ToVector3();
            effect.DirectionalLight0.Direction = Vector3.Left;
            effect.LightingEnabled = true;
            effect.Projection = camera.Projection;
            effect.SpecularPower = 35;
           
#endif
            if (verts != null)
            {
                verts[verts.Length - 1].Position = Top;
                verts[verts.Length - 4].Position = Bottom;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            if (vb != null)
            {
                Game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                Game.GraphicsDevice.BlendState = BlendState.Opaque;
                Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
#if WINDOWS
                //Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                effect.Parameters["DiffuseColor"].SetValue(DiffuseColor.ToVector4());
                effect.Parameters["AmbientColor"].SetValue(AmbientColor.ToVector4());
#endif
                Game.GraphicsDevice.SetVertexBuffer(vb);

                effect.CurrentTechnique.Passes[0].Apply();

                Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, verts, 0, verts.Length, index, 0, verts.Length / 2);
            }
        }
    }
    
}
