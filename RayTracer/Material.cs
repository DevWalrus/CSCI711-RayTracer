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
            // Ensure normal is normalized
            normal = normal.Normalize();
            // Also normalize the view direction
            viewDir = viewDir.Normalize();

            // Start with ambient contribution (if you have a global ambient or just use object color)
            // Here we simply multiply baseColor by ka
            Color result = baseColor * ka;

            // For each light in the scene, add diffuse + specular if not in shadow
            foreach (var light in world.Lights)
            {
                // Vector from intersection to light
                MyVector toLight = light.Center.Subtract(position).Normalize();

                // Shadow check: spawn a ray toward the light, see if blocked
                if (!IsInShadow(position, toLight, world, light))
                {
                    // --------- Diffuse term ---------
                    double nDotL = Math.Max(0, normal.Dot(toLight));
                    Color diffuse = baseColor * (kd * nDotL);

                    // --------- Specular term ---------
                    // Reflection of "toLight" about "normal"
                    // You can use your built-in Reflect if you want the reflection of the LIGHT direction
                    // R = reflect(L, N) = 2(N·L)N - L. 
                    // Alternatively, use your MyVector.Reflect(...) if "this" = L and param = normal.
                    MyVector reflection = toLight.Reflect(normal);  // or do your own reflection math
                    double rDotV = Math.Max(0, reflection.Dot(viewDir));
                    double specFactor = Math.Pow(rDotV, shininess);
                    Color specular;

                    // Convert light's System.Drawing.Color to your RayTracer.Color (if needed)
                    // so highlights are tinted by the light color. For example:
                    Color lightColor = new Color(
                        light.Color.R / 255.0,
                        light.Color.G / 255.0,
                        light.Color.B / 255.0
                    );

                    specular = lightColor * (ks * specFactor);

                    // --------- Accumulate ---------
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
            // Small offset to avoid self-intersection
            double epsilon = 1e-5;
            Point shadowRayOrigin = new Point(
                position.X + toLight.X * epsilon,
                position.Y + toLight.Y * epsilon,
                position.Z + toLight.Z * epsilon
            );

            Ray shadowRay = new Ray(shadowRayOrigin, toLight);

            // Distance from intersection to light
            double distToLight = shadowRayOrigin.Distance(light.Center);

            // Test intersection with all objects
            foreach (var obj in world.Objects)
            {
                var inter = obj.Intersect(shadowRay, 0);
                if (inter != null && inter.Omega < distToLight)
                {
                    // Another object is in the way => in shadow
                    return true;
                }
            }
            return false;
        }
    }
}
