using RayTracer.RayMath;

namespace RayTracer.Shaders
{
    public struct ShadingContext
    {
        /// <summary>
        /// Intersection point in world space.
        /// </summary>
        public Point WorldPosition;

        /// <summary>
        /// Intersection point in object space.
        /// </summary>
        public Point LocalPosition;

        /// <summary>
        /// Surface normal in world space.
        /// </summary>
        public MyVector Normal;

        /// <summary>
        /// The direction from the intersection point to the camera, in world space.
        /// </summary>
        public MyVector ViewDirection;
    }
}
