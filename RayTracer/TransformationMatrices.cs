using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using RayTracer;

public static class TransformationMatrices
{
    public static Matrix<double> RotateX(double angle)
    {
        return DenseMatrix.OfArray(new double[,]
        {
            { 1,                0,                 0, 0 },
            { 0, Math.Cos(angle), -Math.Sin(angle), 0 },
            { 0, Math.Sin(angle),  Math.Cos(angle), 0 },
            { 0,                0,                 0, 1 }
        });
    }

    public static Matrix<double> RotateY(double angle)
    {
        return DenseMatrix.OfArray(new double[,]
        {
            {  Math.Cos(angle), 0, Math.Sin(angle), 0 },
            {                0, 1,                0, 0 },
            { -Math.Sin(angle), 0, Math.Cos(angle), 0 },
            {                0, 0,                0, 1 }
        });
    }

    public static Matrix<double> RotateZ(double angle)
    {
        return DenseMatrix.OfArray(new double[,]
        {
            { Math.Cos(angle), -Math.Sin(angle), 0, 0 },
            { Math.Sin(angle),  Math.Cos(angle), 0, 0 },
            {               0,                0, 1, 0 },
            {               0,                0, 0, 1 }
        });
    }

    public static Matrix<double> LinearScaleFromPoint(double scale, Point point)
    {
        return ScaleFromPoint(scale, scale, scale, point);
    }

    public static Matrix<double> ScaleFromPoint(double scaleX, double scaleY, double scaleZ, Point point)
    {
        Matrix<double> translateToPoint = Translate(-point.X, -point.Y, -point.Z);
        Matrix<double> scaling = Scale(scaleX, scaleY, scaleZ);
        Matrix<double> translateBack = Translate(point.X, point.Y, point.Z);

        return translateBack * scaling * translateToPoint;
    }

    public static Matrix<double> Scale(double scaleX, double scaleY, double scaleZ)
    {
        return DenseMatrix.OfArray(new double[,]
        {
            { scaleX, 0, 0, 0 },
            { 0, scaleY, 0, 0 },
            { 0, 0, scaleZ, 0 },
            { 0, 0, 0, 1 }
        });
    }

    public static Matrix<double> LinearScale(double scale)
    {
        return Scale(scale, scale, scale);
    }

    public static Matrix<double> Translate(double tx, double ty, double tz)
    {
        return DenseMatrix.OfArray(new double[,]
        {
            { 1, 0, 0, tx },
            { 0, 1, 0, ty },
            { 0, 0, 1, tz },
            { 0, 0, 0,  1 }
        });
    }
}
