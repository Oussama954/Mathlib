namespace MathLib
{
    using System;


    public class Decomposition
    {
        private Matrix matrix;
        private double[] rdiag;

        public Decomposition(Matrix value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.matrix = (Matrix) value.Clone();
            double[][] qr = this.matrix.Array;
            int m = value.Rows;
            int n = value.Columns;
            this.rdiag = new double[n];

            for (int k = 0; k < n; k++)
            {
                double nrm = 0;
                for (int i = k; i < m; i++)
                {
                    nrm = Hypotenuse(nrm, qr[i][k]);
                }

                if (nrm != 0.0)
                {
                    if (qr[k][k] < 0)
                    {
                        nrm = -nrm;
                    }

                    for (int i = k; i < m; i++)
                    {
                        qr[i][k] /= nrm;
                    }

                    qr[k][k] += 1.0;

                    for (int j = k + 1; j < n; j++)
                    {
                        double s = 0.0;

                        for (int i = k; i < m; i++)
                        {
                            s += qr[i][k] * qr[i][j];
                        }

                        s = -s / qr[k][k];

                        for (int i = k; i < m; i++)
                        {
                            qr[i][j] += s * qr[i][k];
                        }
                    }
                }

                this.rdiag[k] = -nrm;
            }
        }

        public Matrix Solve(Matrix value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Rows != matrix.Rows)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }

            if (!this.FullRank)
            {
                throw new InvalidOperationException("Matrix is rank deficient.");
            }

            int count = value.Columns;
            Matrix X = value.Clone();
            int m = matrix.Rows;
            int n = matrix.Columns;
            double[][] qr = matrix.Array;

            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j < count; j++)
                {
                    double s = 0.0;

                    for (int i = k; i < m; i++)
                    {
                        s += qr[i][k] * X[i, j];
                    }

                    s = -s / qr[k][k];

                    for (int i = k; i < m; i++)
                    {
                        X[i, j] += s * qr[i][k];
                    }
                }
            }

            // R*X = Y;
            for (int k = n - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                {
                    X[k, j] /= rdiag[k];
                }

                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < count; j++)
                    {
                        X[i, j] -= X[k, j] * qr[i][k];
                    }
                }
            }

            return X.Submatrix(0, n - 1, 0, count - 1);
        }

        public bool FullRank
        {
            get
            {
                int columns = this.matrix.Columns;
                for (int i = 0; i < columns; i++)
                {
                    if (this.rdiag[i] == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private static double Hypotenuse(double a, double b)
        {
            if (Math.Abs(a) > Math.Abs(b))
            {
                double r = b / a;
                return Math.Abs(a) * Math.Sqrt(1 + r * r);
            }

            if (b != 0)
            {
                double r = a / b;
                return Math.Abs(b) * Math.Sqrt(1 + r * r);
            }

            return 0.0;
        }
    }
}