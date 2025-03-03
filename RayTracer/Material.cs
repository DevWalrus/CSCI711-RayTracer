using RayTracer.Objects;

namespace RayTracer
{
    /// <summary>
    /// Simple Phong-like material storing reflection coefficients, shininess, and a base color.
    /// </summary>
    public class Material
    {
        public double ka;        // Ambient coefficient
        public double kd;        // Diffuse coefficient
        public double ks;        // Specular coefficient
        public double shininess; // Shininess exponent
        public Color baseColor;  // Base color (albedo)

        public Material(double ka, double kd, double ks, double shininess, Color baseColor)
        {
            this.ka = ka;
            this.kd = kd;
            this.ks = ks;
            this.shininess = shininess;
            this.baseColor = baseColor;
        }

        /// <summary>
        /// Calculates the Phong-like shading at the given intersection.
        /// </summary>
        /// <param name="position">Intersection point in world space.</param>
        /// <param name="normal">Surface normal at the intersection.</param>
        /// <param name="viewDir">Direction from the intersection point back toward the camera.</param>
        /// <param name="world">Scene data (objects, lights) for shadows, etc.</param>
        /// <returns>A floating-point Color (R,G,B in [0..1] or higher) representing radiance.</returns>
        public Color Shade(Point position, MyVector normal, MyVector viewDir, World world)
        {
            normal = normal.Normalize();
            viewDir = viewDir.Normalize();

            Color result = baseColor * ka;

            foreach (var light in world.Lights)
            {
                MyVector toLight = light.Center.Subtract(position).Normalize();

                if (!IsInShadow(position, toLight, world, light))
                {
                    double nDotL = Math.Max(0, normal.Dot(toLight));
                    Color diffuse = baseColor * (kd * nDotL);
                    MyVector reflection = (toLight * -1).Reflect(normal);
                    double rDotV = Math.Max(0, reflection.Dot(viewDir));
                    double specFactor = Math.Pow(rDotV, shininess);
                    Color specular = light.Color * (ks * specFactor);

                    result += (diffuse + specular);
                }
            }

            return result;
        }


        /// <summary>
        /// Checks if the path from the intersection point to the light is blocked by another object.
        /// </summary>
        private bool IsInShadow(Point position, MyVector toLight, World world, LightSource light)
        {
            double epsilon = 1e-5;
            Point shadowRayOrigin = new Point(
                position.X + toLight.X * epsilon,
                position.Y + toLight.Y * epsilon,
                position.Z + toLight.Z * epsilon
            );

            Ray shadowRay = new Ray(shadowRayOrigin, toLight);
            double distToLight = shadowRayOrigin.Distance(light.Center);

            foreach (var obj in world.Objects)
            {
                var inter = obj.Intersect(shadowRay, 0);
                if (inter != null && inter.Omega < distToLight)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
