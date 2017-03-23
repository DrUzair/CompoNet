using System;
using System.Collections;
using System.Text;

namespace ConSelFAM.NET
{
    public class LayerF1 : ArrayList
    {
        //public LayerF1(int f1NeuronCount,int f2NeuronCount)
        //{
        //    for (int i = 0; i < f1NeuronCount; i++)
        //    {
        //        base.Add(new F1Neuron(f2NeuronCount));
        //    }
        //}
        public LayerF1(int f1NeuronCount)
        {
            for (int i = 0; i < f1NeuronCount; i++)
            {
                base.Add(new F1Neuron());                
            }

        }
        public double[] processInput(double[] i,double alpha,LayerF2 F2Neurons) // Orienting Subsystem
        {            
            int f2index = 0;
            double[] f1Output = new double[F2Neurons.Count];
            while (f2index < F2Neurons.Count)
            {
                F2Neuron f2Neuron = (F2Neuron)F2Neurons[f2index];
                int f1index = 0;
                double[] z = new double[i.Length];
                double I_FuzzyAND_Z = 0.0;
                while(f1index < base.Count){
                    z[f1index] = f2Neuron.getWeight(f1index);
                    I_FuzzyAND_Z += Math.Min(z[f1index], i[f1index]);
                    f1index++;
                }                 
                f1Output[f2index++] = Math.Round((I_FuzzyAND_Z / (alpha + norm(z))), 4);                
            }
            return f1Output;         
        }

        private double norm(double[] vector)
        {
            double norm = 0;
            for (int i = 0; i < vector.Length; i++)
            {
                norm += vector[i];
            }
            return norm;
        }

    }
}
