using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Ribbon.Utilities
{
    /// <summary>
    /// Helper functions
    /// </summary>
    public static class GameComponentHelper
    {
        /// <summary>
        /// Default neutral rotation value.
        /// </summary>
        public static Quaternion baseQuaternion = new Quaternion(0, 0, 0, 1);
        /// <summary>
        /// Method to turn a 3D object to face a position in world space.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static void LookAt(Vector3 target, float speed, Vector3 position, ref Quaternion rotation, Vector3 fwd)
        {
            if (fwd == Vector3.Zero)
                fwd = Vector3.Forward;

            Vector3 tminusp = target - position;
            Vector3 ominusp = fwd;

            if (tminusp == Vector3.Zero)
                return;

            tminusp.Normalize();

            float theta = (float)System.Math.Acos(Vector3.Dot(tminusp, ominusp));
            Vector3 cross = Vector3.Cross(ominusp, tminusp);

            if (cross == Vector3.Zero)
                return;

            cross.Normalize();

            Quaternion targetQ = Quaternion.CreateFromAxisAngle(cross, theta);
            rotation = Quaternion.Slerp(rotation, targetQ, speed);
        }
        /// <summary>
        /// Method to rotate an object.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <param name="rotation"></param>
        public static void Rotate(Vector3 axis, float angle, ref Quaternion rotation)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(rotation));
            rotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * rotation);
        }
        /// <summary>
        /// Method to translate a 3D object
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector3 Translate3D(Vector3 distance, Quaternion rotation)
        {
            return Vector3.Transform(distance, Matrix.CreateFromQuaternion(rotation));
        }
        /// <summary>
        /// Method to translate a 3D object
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 Translate3D(Vector3 distance)
        {
            return Vector3.Transform(distance, Matrix.CreateFromQuaternion(baseQuaternion));
        }
        
    }
}
