using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace LVQ.NET
{
    class OutputLayer
    {
        NeuronList outputNeurons;
        public OutputLayer(int [,]weights) {
            outputNeurons = NeuronFectory.createNeurons(weights,2);
        }
        public NeuronList getOutputNeuronsList() {
            return outputNeurons;
        }
        public void classify(double []x){
            
        }
    }
}
