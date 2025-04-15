using RayTracer.Objects;

namespace RayTracer.Shaders
{
    /// <summary>
    /// Simple Phong-like material storing reflection coefficients, shininess, and a base color.
    /// </summary>
    public class PhongShader : Material
    {
        public double _ka;
        public double _kd;
        public double _ks;
        public double _shininess;
        public Material _baseShader;

        public PhongShader(
            double ka, 
            double kd, 
            double ks, 
            double shininess, 
            Material baseShader,
            double reflectivity = 0,
            double transparency = 0,
            double indexOfRefraction = 0
        ) : base(reflectivity, transparency, indexOfRefraction)
        {
            _ka = ka;
            _kd = kd;
            _ks = ks;
            _shininess = shininess;
            _baseShader = baseShader;
        }

        /// <inheritdoc/>
        public override Color Shade(ShadingContext shading, World world)
        {
            var baseColor = _baseShader.Shade(shading, world);

            var normal = shading.Normal.Normalize();
            var viewDir = shading.ViewDirection.Normalize();

            Color result = baseColor * _ka;

            foreach (var light in world.Lights)
            {
                MyVector toLight = light.Center.Subtract(shading.WorldPosition).Normalize();
                double shadowFactor = ShadowAttenuation(shading.WorldPosition, toLight, world, light);
                if (shadowFactor > 0.0)
                {
                    double nDotL = Math.Max(0, normal.Dot(toLight));
                    Color diffuse = baseColor * (_kd * nDotL * shadowFactor);
                    MyVector reflection = (toLight * -1).Reflect(normal);
                    double rDotV = Math.Max(0, reflection.Dot(viewDir));
                    double specFactor = Math.Pow(rDotV, _shininess);
                    Color specular = light.Color * (_ks * specFactor * shadowFactor);

                    result += diffuse + specular;
                }
            }

            return result;
        }


        /// <summary>
        /// Checks if the path from the intersection point to the light is blocked by another object.
        /// </summary>
        private double ShadowAttenuation(Point position, MyVector toLight, World world, LightSource light)
        {
            double epsilon = 1e-5;
            Point shadowRayOrigin = new Point(
                position.X + toLight.X * epsilon,
                position.Y + toLight.Y * epsilon,
                position.Z + toLight.Z * epsilon
            );

            Ray shadowRay = new Ray(shadowRayOrigin, toLight);
            double distToLight = shadowRayOrigin.Distance(light.Center);
            double attenuation = 1.0;

            foreach (var obj in world.Objects)
            {
                var inter = obj.Intersect(shadowRay, 0);
                if (inter != null && inter.Omega < distToLight)
                {
                    attenuation *= obj.Material.Transparency;
                    if (attenuation < 0.01)
                        return 0.0;
                }
            }
            return attenuation;
        }
    }
}
