/* 190706
 * Uzair Ahmad
 * OSLAB, Kyung Hee University
 * South Korea 
 */


using System;
using System.Collections.Generic;
using System.Text;

namespace FuzzyARTMAP.NET
{
    public class MapFieldConnection
    {
        double weight;
        MapFieldCategory category;
        ContextAwareFuzzyART.NET.F2Neuron f2Neuron;
        public MapFieldConnection(MapFieldCategory category, ContextAwareFuzzyART.NET.F2Neuron f2Neuron, int weight)
        {
            this.category = category;            
            this.f2Neuron = f2Neuron;
            this.f2Neuron.addMapFieldConnection(this);
            this.weight = weight; 
        }
        public ContextAwareFuzzyART.NET.F2Neuron getF2Neuron() { return f2Neuron; }
        public MapFieldCategory getCategory() { return category; }
        public double getWeight(){return weight;}
        public void setWeight(double w) { weight = w; }
    }
}
