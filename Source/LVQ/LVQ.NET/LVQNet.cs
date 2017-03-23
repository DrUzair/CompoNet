using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace LVQ.NET
{    
    public class LVQNet
    {
        HiddenLayer layerOne;
        OutputLayer layerTwo;
        CompetitionBox competitionBox = new CompetitionBox();
        public LVQNet(WeightsMatrix wm) {
           layerOne = new HiddenLayer(wm.IW);
           layerTwo = new OutputLayer(wm.LW);
        }
        public OutputPattern execute(InputPattern I){
            IEnumerator IEnum = (layerOne.getHiddenLayerNeurons()).GetEnumerator();
            while (IEnum.MoveNext()) {
                ((HiddenLayerNeuron)IEnum.Current).fire(I);
            }
            
            float []x = competitionBox.compete(layerOne);
            I = new InputPattern(x);
            IEnum = (layerTwo.getOutputNeuronsList()).GetEnumerator();
            OutputPattern o = new OutputPattern(layerTwo.getOutputNeuronsList().Count);
            int i = 0;
            while (IEnum.MoveNext())
            {
                ((OutputLayerNeuron)IEnum.Current).fire(I);
                o.setOutput(((OutputLayerNeuron)IEnum.Current).getOutput(),i++);
            }
            return o;
        }
    }
    

    class CompetitionBox
    {
        public float[] compete(HiddenLayer hiddenLayer)
        {
            NeuronList list = hiddenLayer.getHiddenLayerNeurons();
            IEnumerator listEnum = list.GetEnumerator();
            float[] x = new float[list.Count];
            int i = 0;
            while (listEnum.MoveNext()) {
                x[i++]=((HiddenLayerNeuron)listEnum.Current).getOutput();
            }
            double max = x[0];
            int maxIndex = 0;
            for (int j = x.Length - 1; j > 0; j--)
            {
                if (max < x[j])
                    maxIndex = j;
                max = Math.Max(max, x[j]);
            }

            for (int k = 0; k < x.Length; k++) {
                x[k] = 0;
                if (k == maxIndex)
                    x[k] = 1;
            }
            return x;
        }
    }
}
