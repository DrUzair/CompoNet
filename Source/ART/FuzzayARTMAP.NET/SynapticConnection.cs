using System;
using System.Collections.Generic;
using System.Text;

namespace ConSelFAM.NET
{
    public class SynapticConnection
    {
        private F1Neuron f1Neuron;
        private F2Neuron f2Neuron;
        private double weight; // To Down Connection Weigth
        
        // In Fuzzy ART, td_weight = bu_weight therefore, this implementation uses weight instead for both.
        public SynapticConnection(F1Neuron f1Neuron, F2Neuron f2Neuron) {
            this.f1Neuron = f1Neuron;
            this.f2Neuron = f2Neuron;
        }
        public SynapticConnection(F2Neuron f2Neuron, F1Neuron f1Neuron) {
            this.f1Neuron = f1Neuron;
            this.f2Neuron = f2Neuron;
        }
        public SynapticConnection() { }
        public F2Neuron getF2Neuron() {
            return f2Neuron;
        }
        public F1Neuron getF1Neuron() {
            return f1Neuron;
        }
        public void setF1Neuron(F1Neuron f1Neuron) { this.f1Neuron = f1Neuron; }
        public void setF2Neuron(F2Neuron f2Neuron) { this.f2Neuron = f2Neuron; }
        
        public double getWeight() { return weight; }
        public void setWeight(double w) { weight = w; } 
    }  
}
