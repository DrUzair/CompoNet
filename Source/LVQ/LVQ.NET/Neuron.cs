using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace LVQ.NET
{
    public class Neuron
    {
        public Synapse []inputSynapses;        
        protected float output;
        virtual public void fire(InputPattern I){            
        }
        virtual public float getOutput() {
            return output;
        }
        public Neuron(Synapse []s) {
            inputSynapses = s;
        }
        public Neuron() { }
        public class Synapse
        {
            double weight;            
            public Synapse(double w) {
                weight = w;
            }
            public double getSynapseWeight(){
                return weight;
            }
            public void setSynapseWeight(double w) {
                weight = w;
            }
        }
    }   
    
}
