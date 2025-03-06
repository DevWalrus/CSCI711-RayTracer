namespace RayTracer
{
    /// <summary>
    /// Defines the contract for material shading in the ray tracing engine.
    /// Implementations of this interface are responsible for calculating the radiance (color) at a given surface 
    /// intersection based on the material's properties, the surface geometry, and the lighting conditions present 
    /// in the scene.
    /// </summary>
    public interface IMaterial
    {
        /// <summary>
        /// Computes the shaded color at an intersection point on a surface.
        /// </summary>
        /// <param name="position">The intersection point in world space where the ray hits the surface.</param>
        /// <param name="normal">The normalized surface normal at the intersection point.</param>
        /// <param name="viewDir">The normalized direction vector from the intersection point towards the camera.</param>
        /// <param name="world">
        /// The scene context containing lights and objects. This parameter allows the shading function
        /// to take into account additional factors such as shadows, reflections, and other lighting effects.
        /// </param>
        /// <returns>
        /// A <see cref="Color"/> representing the computed radiance at the intersection point. The color's 
        /// components are typically in a floating-point format, allowing for values outside the standard [0, 1] range
        /// to accommodate high dynamic range rendering.
        /// </returns>
        public Color Shade(Point position, MyVector normal, MyVector viewDir, World world);
    }
}
