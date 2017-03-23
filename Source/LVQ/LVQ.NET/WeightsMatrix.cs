using System;
using System.Collections.Generic;
using System.Text;

namespace LVQ.NET
{
    public class WeightsMatrix
    {
        public double[,] IW;
        public int[,] LW;
        public WeightsMatrix(double[,] hiddenLayerWeights, int[,] outputLayerWeights)
        {
            IW = hiddenLayerWeights;
            LW = outputLayerWeights;
        }
    }
}
