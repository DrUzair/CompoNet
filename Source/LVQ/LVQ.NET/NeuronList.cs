using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace LVQ.NET
{
    public class NeuronList : ArrayList
    {
        short LIST_TYPE;
        public NeuronList(short i) {
            LIST_TYPE = i;
        }
    }
}
