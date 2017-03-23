using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace LVQ.NET
{
    public class NeuronFectory
    {
        public static NeuronList neuronList;
        public static NeuronList createNeurons(double [,]w,short LIST_TYPE){
            neuronList = new NeuronList(LIST_TYPE);
            for (int i = 0; i < w.GetLength(0); i++)
            {
                if (LIST_TYPE == 1)
                {                    
                    Neuron.Synapse []s = new Neuron.Synapse[w.GetLength(1)];
                    for (int j = 0; j < w.GetLength(1); j++)
                    {
                        s[j] = new Neuron.Synapse(w[i,j]);                        
                    }
                    neuronList.Add(new HiddenLayerNeuron(s));
                }                
            }
            return neuronList;
        }
        public static NeuronList createNeurons(int[,] w, short LIST_TYPE)
        {
            neuronList = new NeuronList(LIST_TYPE);
            for (int i = 0; i < w.GetLength(0); i++)
            {                
                    Neuron.Synapse[] s = new Neuron.Synapse[w.GetLength(1)];
                    for (int j = 0; j < w.GetLength(1); j++)
                    {
                        s[j] = new Neuron.Synapse(w[i, j]);
                    }
                    neuronList.Add(new OutputLayerNeuron(s));             
            }
            return neuronList;
        }
    }
    
}
