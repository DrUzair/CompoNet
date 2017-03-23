using System;
using System.Collections.Generic;
using System.Text;

namespace LVQ.NET
{
    public class OutputPattern
    {
        private float[] o;
        public OutputPattern(int size) { 
            o = new float[size];
        }
        public float [] getOutputPattern(){
            return o;
        }
        public void setOutput(float output, int index){
            o[index]=output;
        }
    }
}
