/* 190706
 * Uzair Ahmad
 * OSLAB, Kyung Hee University
 * South Korea 
 */


using System;
using System.Collections.Generic;
using System.Text;

namespace ConSelFAM.NET
{
    public class MapFieldConnection
    {
        double weight;
        MapFieldCategory category;
        ConSelFAM.NET.F2Neuron f2Neuron;
        public MapFieldConnection(MapFieldCategory category, ConSelFAM.NET.F2Neuron f2Neuron, int weight)
        {
            this.category = category;            
            this.f2Neuron = f2Neuron;
         //   this.f2Neuron.addMapFieldConnection(this);
            this.f2Neuron.setMapFieldConnection(this);
            this.weight = weight; 
        }
        public ConSelFAM.NET.F2Neuron getF2Neuron() { return f2Neuron; }
        public MapFieldCategory getCategory() { return category; }
        public double getWeight(){return weight;}
        public void setWeight(double w) { weight = w; }
    }
}
