using System;

namespace MathLib
{
    /// <summary>
    /// https://introcs.cs.princeton.edu/java/97data/MultipleLinearRegression.java
    /// https://github.com/lutzroeder/Mapack/tree/master/Source
    /// </summary>
    public class PickupModel
    {
        private readonly int _n;        
        private readonly int _p;        
        private readonly Matrix _beta;  
        private readonly double _sse;        
        private readonly double _sst;         

        public PickupModel(double[][] x, double[] y)
        {
            if (x.Length != y.Length) throw new Exception("dimensions don't agree");
            _n = y.Length;
            _p = x[0].Length;    

            var X = new Matrix(x);

            var Y = new Matrix(y,_n);

            var decomposition = new Decomposition(X);
            _beta = decomposition.Solve(Y);

            double sum = 0.0;
            for (int i = 0; i < _n; i++)
                sum += y[i];
            double mean = sum / _n;

            for (int i = 0; i < _n; i++)
            {
                double dev = y[i] - mean;
                _sst += dev * dev;
            }

            var residuals = X * _beta - Y;
            _sse = residuals.FrobeniusNorm * residuals.FrobeniusNorm;

        }

        public double R2()
        {
            return 1.0 - _sse / _sst;
        }
    }


}
