using System;
using System.Collections.Generic;
using System.Text;

namespace LVQ.NET
{
    class HiddenLayerNeuron : Neuron
    {        
        public HiddenLayerNeuron(Synapse []synapses){
            base.inputSynapses = synapses;
        }        
        override public void fire(InputPattern I){
            //z = -sqrt(sum(w-p)^2)
            float sum = 0.0F;
            for(int i = 0; i < inputSynapses.Length ; i ++){
                sum += (float)Math.Pow(inputSynapses[i].getSynapseWeight() - I.getInputValue(i),2);
            }
            base.output =  (float)(-1*Math.Sqrt(sum));            
        }        
    }
}
