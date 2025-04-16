using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using RayTracer.RayMath;

public static class MatrixExtensions
{

    public static Point TransformPoint(this Matrix<double> matrix, Point point)
    {
        if (matrix.RowCount != 4 || matrix.ColumnCount != 4)
        {
            throw new ArgumentException("Transformation matrix must be 4x4.");
        }

        // Create the homogeneous coordinate vector.
        Vector<double> homogeneousPoint = DenseVector.OfArray([point.X, point.Y, point.Z, 1]);

        // Compute the transformed x, y, and z.
        double x = matrix[0, 0] * homogeneousPoint[0] + matrix[0, 1] * homogeneousPoint[1] +
                   matrix[0, 2] * homogeneousPoint[2] + matrix[0, 3] * homogeneousPoint[3];

        double y = matrix[1, 0] * homogeneousPoint[0] + matrix[1, 1] * homogeneousPoint[1] +
                   matrix[1, 2] * homogeneousPoint[2] + matrix[1, 3] * homogeneousPoint[3];

        double z = matrix[2, 0] * homogeneousPoint[0] + matrix[2, 1] * homogeneousPoint[1] +
                   matrix[2, 2] * homogeneousPoint[2] + matrix[2, 3] * homogeneousPoint[3];

        // Compute the w component from the last row.
        double w = matrix[3, 0] * homogeneousPoint[0] + matrix[3, 1] * homogeneousPoint[1] +
                   matrix[3, 2] * homogeneousPoint[2] + matrix[3, 3] * homogeneousPoint[3];

        // If w is not 1, perform the homogeneous division.
        if (Math.Abs(w) > 1e-8)
        {
            x /= w;
            y /= w;
            z /= w;
        }

        return new Point(x, y, z);
    }
}
