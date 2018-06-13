namespace MathLib
{
    using System;
    using System.IO;
    using System.Globalization;

    public class Matrix
    {
        public Matrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Array = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                this.Array[i] = new double[columns];
            }
        }


        public Matrix(double[][] value)
        {
            this.Rows = value.Length;
            this.Columns = value[0].Length;

            for (int i = 0; i < Rows; i++)
            {
                if (value[i].Length != Columns)
                {
                    throw new ArgumentException("Argument out of range.");
                }
            }

            this.Array = value;
        }
        public Matrix(double [] value, int m)
        {
            
           int  n = (m != 0 ? value.Length / m : 0);
            if (m * n != value.Length)
            {
                throw new ArgumentException("Array length must be a multiple of m.");
            }

            var A = new double[m][];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i][j] = value[i + j * m];
                }
            }

            this.Array = A;
        }

        public override bool Equals(object obj)
        {
            return Equals(this, (Matrix) obj);
        }

        public static bool Equals(Matrix left, Matrix right)
        {
            if (((object) left) == ((object) right))
            {
                return true;
            }

            if ((((object) left) == null) || (((object) right) == null))
            {
                return false;
            }

            if ((left.Rows != right.Rows) || (left.Columns != right.Columns))
            {
                return false;
            }

            for (int i = 0; i < left.Rows; i++)
            {
                for (int j = 0; j < left.Columns; j++)
                {
                    if (left[i, j] != right[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return (this.Rows + this.Columns);
        }

        internal double[][] Array { get; }

        public int Rows { get; }

        public int Columns { get; }

        public bool Square => (Rows == Columns);

        public double this[int row, int column]
        {
            set { this.Array[row][column] = value; }

            get { return this.Array[row][column]; }
        }

        public Matrix Submatrix(int startRow, int endRow, int startColumn, int endColumn)
        {
            if ((startRow > endRow) || (startColumn > endColumn) ||
                (startRow < 0) || (startRow >= this.Rows) || (endRow < 0)
                || (endRow >= this.Rows) || (startColumn < 0)
                || (startColumn >= this.Columns) || (endColumn < 0)
                || (endColumn >= this.Columns))
            {
                throw new ArgumentException("Argument out of range.");
            }

            Matrix X = new Matrix(endRow - startRow + 1, endColumn - startColumn + 1);
            double[][] x = X.Array;
            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startColumn; j <= endColumn; j++)
                {
                    x[i - startRow][j - startColumn] = Array[i][j];
                }
            }

            return X;
        }

        public Matrix Clone()
        {
            Matrix X = new Matrix(Rows, Columns);
            double[][] x = X.Array;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    x[i][j] = Array[i][j];
                }
            }

            return X;
        }

        public double InfinityNorm
        {
            get
            {
                double f = 0;
                for (int i = 0; i < Rows; i++)
                {
                    double s = 0;
                    for (int j = 0; j < Columns; j++)
                        s += Math.Abs(Array[i][j]);
                    f = Math.Max(f, s);
                }

                return f;
            }
        }

        public double FrobeniusNorm
        {
            get
            {
                double f = 0;
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        f = Hypotenuse(f, Array[i][j]);
                    }
                }

                return f;
            }
        }

        public static bool operator ==(Matrix left, Matrix right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Matrix left, Matrix right)
        {
            return !Equals(left, right);
        }

        public static Matrix operator +(Matrix left, Matrix right)
        {
            int rows = left.Rows;
            int columns = left.Columns;
            double[][] data = left.Array;

            if ((rows != right.Rows) || (columns != right.Columns))
            {
                throw new ArgumentException("Matrix dimension do not match.");
            }

            Matrix X = new Matrix(rows, columns);
            double[][] x = X.Array;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    x[i][j] = data[i][j] + right[i, j];
                }
            }

            return X;
        }


        public static Matrix operator -(Matrix left, Matrix right)
        {
            int rows = left.Rows;
            int columns = left.Columns;
            double[][] data = left.Array;

            if ((rows != right.Rows) || (columns != right.Columns))
            {
                throw new ArgumentException("Matrix dimension not match.");
            }

            Matrix X = new Matrix(rows, columns);
            double[][] x = X.Array;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    x[i][j] = data[i][j] - right[i, j];
                }
            }

            return X;
        }


        public static Matrix operator *(Matrix left, double right)
        {
            int rows = left.Rows;
            int columns = left.Columns;
            double[][] data = left.Array;

            Matrix X = new Matrix(rows, columns);

            double[][] x = X.Array;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    x[i][j] = data[i][j] * right;
                }
            }

            return X;
        }


        public static Matrix operator *(Matrix left, Matrix right)
        {
            int rows = left.Rows;
            double[][] data = left.Array;

            if (right.Rows != left.Columns)
            {
                throw new ArgumentException("Matrix dimensions are not valid.");
            }

            int columns = right.Columns;
            Matrix X = new Matrix(rows, columns);
            double[][] x = X.Array;

            int size = left.Columns;
            double[] column = new double[size];
            for (int j = 0; j < columns; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    column[k] = right[k, j];
                }

                for (int i = 0; i < rows; i++)
                {
                    double[] row = data[i];
                    double s = 0;
                    for (int k = 0; k < size; k++)
                    {
                        s += row[k] * column[k];
                    }

                    x[i][j] = s;
                }
            }

            return X;
        }
         public override string ToString()
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        writer.Write(this.Array[i][j] + " ");
                    }

                    writer.WriteLine();
                }

                return writer.ToString();
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