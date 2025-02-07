namespace RayTracer.Objects
{
    abstract class Triangle : RenderableObject
    {
        private List<double> _verticies;
        private Ray _normal;

        public Triangle(List<double> verticies, Ray normal, Color material) : base(material)
        {
            _verticies = verticies;
            _normal = normal;
        }

        /// <inheritdoc/>
        public override void Intersect(Ray ray)
        {

        }
    }
}
