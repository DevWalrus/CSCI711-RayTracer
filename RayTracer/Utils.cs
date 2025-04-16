namespace RayTracer.Utils
{
    public static class MatrixComparer
    {
        public static bool AreEqual(double[,] matrix1, double[,] matrix2, double epsilon = 1e-9)
        {
            if (matrix1.GetLength(0) != matrix2.GetLength(0) ||
                matrix1.GetLength(1) != matrix2.GetLength(1))
            {
                return false;
            }

            for (int i = 0; i < matrix1.GetLength(0); i++)
            {
                for (int j = 0; j < matrix1.GetLength(1); j++)
                {
                    if (Math.Abs(matrix1[i, j] - matrix2[i, j]) > epsilon)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

}
