using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace LVQ.NET
{
    class HiddenLayer
    {
        NeuronList hiddenLayerNeurons;
        public HiddenLayer(double [,]weights) {
            hiddenLayerNeurons = NeuronFectory.createNeurons(weights,1);
        }

        public NeuronList getHiddenLayerNeurons(){
            return hiddenLayerNeurons;
        }
    }
}
