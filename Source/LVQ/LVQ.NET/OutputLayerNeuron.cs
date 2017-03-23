using System;
using System.Collections.Generic;
using System.Text;

namespace LVQ.NET
{
    class OutputLayerNeuron : Neuron
    {
        public OutputLayerNeuron(Synapse []s){
            base.inputSynapses = s;
        }
        override public void fire(InputPattern I)
        {
            float []input = I.getInputPattern();
            double sum = 0;
            for( int i = 0; i < inputSynapses.GetLength(0) ; i ++){
                sum +=inputSynapses[i].getSynapseWeight() * input[i];
            }
            base.output = (float)sum;
        }        
    }
    
}
