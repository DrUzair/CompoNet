using System;
using System.Collections;
using System.Text;

namespace ConSelFAM.NET
{
    public class F2Neuron
    {
        double[] prototype;
        //public bool isCommitted(){
        //    return committed;
        //}
       // public void setCommitted(bool c) { committed = c; }
        MapFieldConnection mapFieldConnection;
        SynapticConnection []tdconnections;        
        ArrayList mapFieldConnections = new ArrayList();

        public void addMapFieldConnection(MapFieldConnection connection) {
            mapFieldConnections.Add(connection);
        }

        public void setMapFieldConnection(MapFieldConnection conn) {
            mapFieldConnection = conn;
        }
        public MapFieldConnection getMapFieldConnection() {
            return mapFieldConnection;
        }
        public ArrayList getMapFieldConnections() {
            return mapFieldConnections;
        }
        ArrayList synapticConnections = new ArrayList();
        public F2Neuron(int f1NeuronCount) {
            tdconnections = new SynapticConnection[f1NeuronCount];
            for (int i = 0; i < tdconnections.Length; i++)
            {
                tdconnections[i] = new SynapticConnection();
            }
            prototype = new double[f1NeuronCount];
            //committed = false;
        }
        public void setProtoTypeCluster(double[] c) { prototype = c; }
        public double[] getProtoTypeCluster() { return prototype; }
        public double getWeight(int neuronIndex)
        {
            return tdconnections[neuronIndex].getWeight();
        }
        public void setWeights(double[] weights)
        {
            for (int i = 0; i < tdconnections.Length; i++)
            {
                tdconnections[i].setWeight(weights[i]); 
            }
        }
        public void setWeight(int w, int connectionIndex) {
            tdconnections[connectionIndex].setWeight(w);
        }
        public int getSynapticConnectionsCount()
        {
            return tdconnections.Length;
        }
        public SynapticConnection[] getConnections() {
            return tdconnections;
        }
    }    
}
