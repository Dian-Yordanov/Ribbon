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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Base3DCamera : GameComponent
    {
        /// <summary>
        /// Position
        /// </summary>
        protected Vector3 position;
        /// <summary>
        /// Scale
        /// </summary>
        protected Vector3 scale;
        /// <summary>
        /// Rotation
        /// </summary>
        protected Quaternion rotation;
        /// <summary>
        /// World
        /// </summary>
        protected Matrix world;
        #region I3DCamera Members

        /// <summary>
        /// Position
        /// </summary>
        public Vector3 Position
        {
            get
            { return position; }
            set
            { position = value; }
        }
        /// <summary>
        /// Scale
        /// </summary>
        public Vector3 Scale
        {
            get
            { return scale; }
            set
            { scale = value; }
        }
        /// <summary>
        /// Rotation
        /// </summary>
        public Quaternion Rotation
        {
            get
            { return rotation; }
            set
            { rotation = value; }
        }
        /// <summary>
        /// World
        /// </summary>
        public Matrix World
        {
            get { return world; }
        }

        #endregion

        /// <summary>
        /// View
        /// </summary>
        protected Matrix view;
        /// <summary>
        /// View
        /// </summary>
        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }
        /// <summary>
        /// Projection
        /// </summary>
        protected Matrix projection;
        /// <summary>
        /// Projection
        /// </summary>
        public Matrix Projection
        {
            get { return projection; }
        }
        /// <summary>
        /// View port
        /// </summary>
        public Viewport Viewport;

        /// <summary>
        /// Frustum
        /// </summary>
        public BoundingFrustum Frustum
        {
            get
            {
                return new BoundingFrustum(Matrix.Multiply(View, Projection));
            }
        }

        /// <summary>
        /// View ports min depth
        /// </summary>
        protected float minDepth;
        /// <summary>
        /// Viewports max depth.
        /// </summary>
        protected float maxDepth;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="minDepth"></param>
        /// <param name="maxDepth"></param>
        public Base3DCamera(Game game, float minDepth, float maxDepth) : base(game)
        {
            position = Vector3.Zero;
            scale = Vector3.One;
            rotation = Quaternion.Identity;
            this.minDepth = minDepth;
            this.maxDepth = maxDepth;
        }
        /// <summary>
        /// Initialization
        /// </summary>
        public override void Initialize()
        {
            Viewport = Game.GraphicsDevice.Viewport;
            Viewport.MinDepth = minDepth;
            Viewport.MaxDepth = maxDepth;
        }
       
        /// <summary>
        /// Method to update.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            world = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(Position);
            view = Matrix.Invert(world);

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 4, (float)Viewport.Width / (float)Viewport.Height, Viewport.MinDepth, Viewport.MaxDepth);
            base.Update(gameTime);
        }
        /// <summary>
        /// Method to translate object
        /// </summary>
        /// <param name="distance"></param>
        public void Translate(Vector3 distance)
        {
            Position += GameComponentHelper.Translate3D(distance, rotation);
        }
        /// <summary>
        /// Method to rotate object
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public void Rotate(Vector3 axis, float angle)
        {
            GameComponentHelper.Rotate(axis, angle, ref rotation);
        }
        public void LookAt(Vector3 target, float speed)
        {
            GameComponentHelper.LookAt(target, speed, position, ref rotation, Vector3.Forward);
        }
    }
}
