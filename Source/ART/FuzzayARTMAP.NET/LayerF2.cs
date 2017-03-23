using System;
using System.Collections;
using System.Text;

namespace ConSelFAM.NET
{
    public class LayerF2 : ArrayList
    {   
        public LayerF2()
        {
        }
        public double[] getCluster(int f2NeuronID)
        {
            return ((F2Neuron)base[f2NeuronID]).getProtoTypeCluster();// cluster;
        }
        public object[,] processInput(double[] T) {
            object[,] maxIndexANDF2Vector = new object[1, 2];            
            int maxIndex = 0;
            double max = T[maxIndex];

            //WTA Implementation
            for (int i = 0; i < T.Length; i++) {
                if (T[i] != -1 & T[i] >= max) {
                    maxIndex = i;
                    max = T[i];
                }
            }
            maxIndexANDF2Vector[0, 0] = maxIndex;
            int tdConnectionCount = ((F2Neuron)base[0]).getSynapticConnectionsCount();
            double[] Vi = new double[tdConnectionCount];
            for (int i = 0; i < tdConnectionCount; i++)
            {
                Vi[i] = ((F2Neuron)base[maxIndex]).getWeight(i);
            }
            maxIndexANDF2Vector[0, 1] = Vi;

            return maxIndexANDF2Vector;
        }
        public double[] updateWeights(double[] z_td_J_old, double []I, int J,double beta)
        {
            double[] z_td_J_new = new double[I.Length];
            for (int i = 0; i < I.Length; i++) 
            {              
                 //z_td_J_new[i] = Math.Round(beta*(Math.Min(I[i],z_td_J_old[i]))+(1-beta)*z_td_J_old[i],2);                
                z_td_J_new[i] = beta * (Math.Min(I[i], z_td_J_old[i])) + (1 - beta) * z_td_J_old[i];                
            }
            ((F2Neuron)base[J]).setWeights(z_td_J_new);
            ((F2Neuron)base[J]).setProtoTypeCluster(I);
            return z_td_J_new;
        }
        public int AddF2Neuron(LayerF1 f1Layer,double []pattern) // to be called from CAFuzzyART.feedInput() method where pattern = tdConnweights
        {

            F2Neuron newF2Neuron = new F2Neuron(f1Layer.Count);
            base.Add(newF2Neuron);
            SynapticConnection []connections = newF2Neuron.getConnections();
            for (int i = 0; i < connections.Length; i++) {                
                connections[i].setF1Neuron((F1Neuron)f1Layer[i]);
                connections[i].setF2Neuron(newF2Neuron);
                ((F1Neuron)f1Layer[i]).AddSynapticConnection(connections[i]);               
                connections[i].setWeight(pattern[i]);                
            }
            newF2Neuron.setProtoTypeCluster(pattern);
            return base.Count - 1;
        }
        public int AddF2Neuron(LayerF1 f1Layer, double[] tdConnWeights,double []prototype) // to be called from CAFuzzyART(string path) method where pattern != tdConnweights
        {

            F2Neuron newF2Neuron = new F2Neuron(f1Layer.Count);
            base.Add(newF2Neuron);
            SynapticConnection[] connections = newF2Neuron.getConnections();
            for (int i = 0; i < connections.Length; i++)
            {
                connections[i].setF1Neuron((F1Neuron)f1Layer[i]);
                connections[i].setF2Neuron(newF2Neuron);
                ((F1Neuron)f1Layer[i]).AddSynapticConnection(connections[i]);
                connections[i].setWeight(tdConnWeights[i]);
            }
            newF2Neuron.setProtoTypeCluster(prototype);
            return base.Count - 1;
        }
        private double norm(double[] p) {
            double norm = 0;
            for (int i = 0; i < p.Length; i++) {
                norm = norm + p[i];
            }
            return norm;
        }
    }
}
