using System;
using System.Collections.Generic;
using System.Text;

namespace LVQ.NET
{
    public class InputPattern {        
        private float []I;
        public InputPattern(float[] i) {
            I = i;
        }
        public float[] getInputPattern(){
            return I;
        }
        public float getInputValue(int index){
            return I[index];
        }
    }    
}
