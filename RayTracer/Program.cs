using RayTracer.Tests;

namespace RayTracer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var argSet = new HashSet<string>(args);

            if (argSet.Contains("-t") || argSet.Contains("--test"))
                TestRunner.RunAllTests();
        }
    }
}
