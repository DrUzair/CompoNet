using System;
using System.Collections;
using System.Text;

namespace ConSelFAM.NET
{
    
    public class F1Neuron
    {
        //SynapticConnection []connections;
        ArrayList buConnections = new ArrayList(); // Bottom Up connections
        public F1Neuron() { }
        public F1Neuron(int connectionCount) {            
            for (int i = 0; i < connectionCount; i++) {
                buConnections.Add(new SynapticConnection());
            }            
        }
        public double getWeight(int connectionIndex)
        {
            double w = ((SynapticConnection)buConnections[connectionIndex]).getWeight();//in Fuzzy Art both td_weight and bu_weight are same therefore this implementation uses weight for both
            return w;
        }
        public void setWeight(double w, int connectionIndex) {
            ((SynapticConnection)buConnections[connectionIndex]).setWeight(w);
        }
        public int getSynapticConnectionsCount(){
            return buConnections.Count;
        }
        public void AddSynapticConnection(SynapticConnection c) {
            buConnections.Add(c);
        }
    }
    
}
