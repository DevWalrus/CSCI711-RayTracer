using RayTracer.Objects;

namespace RayTracer
{
    /// <summary>
    /// Simple Phong-like material storing reflection coefficients, shininess, and a base color.
    /// </summary>
    public class PhongShader : IMaterial
    {
        public double _ka;
        public double _kd;
        public double _ks;
        public double _shininess;
        public IMaterial _baseShader;

        public PhongShader(double ka, double kd, double ks, double shininess, IMaterial baseShader)
        {
            _ka = ka;
            _kd = kd;
            _ks = ks;
            _shininess = shininess;
            _baseShader = baseShader;
        }

        /// <inheritdoc/>>
        public Color Shade(Point position, MyVector normal, MyVector viewDir, World world)
        {
            var baseColor = _baseShader.Shade(position, normal, viewDir, world);

            normal = normal.Normalize();
            viewDir = viewDir.Normalize();

            Color result = baseColor * _ka;

            foreach (var light in world.Lights)
            {
                MyVector toLight = light.Center.Subtract(position).Normalize();

                if (!IsInShadow(position, toLight, world, light))
                {
                    double nDotL = Math.Max(0, normal.Dot(toLight));
                    Color diffuse = baseColor * (_kd * nDotL);
                    MyVector reflection = (toLight * -1).Reflect(normal);
                    double rDotV = Math.Max(0, reflection.Dot(viewDir));
                    double specFactor = Math.Pow(rDotV, _shininess);
                    Color specular = light.Color * (_ks * specFactor);

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
