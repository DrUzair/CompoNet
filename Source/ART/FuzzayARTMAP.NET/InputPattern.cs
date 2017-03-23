using System;
using System.Collections.Generic;
using System.Text;

namespace ConSelFAM.NET
{
    public class InputPattern
    {
        private int[,] pattern;
        public InputPattern(int rows, int columns) {
            pattern = new int[rows, columns];
        }
        public int[,] getInputPattern(){
            return pattern;
        }
        public void setIputPattern(int [,]i){
            pattern = i;
        }
    }
}
