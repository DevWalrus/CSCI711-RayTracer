namespace RayTracer.Objects
{
    public abstract class RenderableObject
    {
        public Color material;

        public RenderableObject(Color material)
        {
            this.material = material;
        }

        public abstract Color? Intersect(Ray ray);
    }
}
