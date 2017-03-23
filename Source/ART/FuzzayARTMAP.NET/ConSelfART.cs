/*200606
 * FuzzyArt Class
 * 
 */
using System;
using System.Collections;
using System.Text;

namespace ConSelFAM.NET
{
    public class ConSelfART
    {
        private double epsilon;
        public ContextField contextField= new ContextField();
        public LayerF1 F1;
        //public LayerF2 F2;
        double[] i; //input vector
        int CategoryCountLimit = -1;
        bool complementCoding = false;
        bool delayUpdate = false;
        double rho;
        double alpha; // Choice Parameter
        double beta; // Learning Rate        
        public ConSelfART(int inputVectorLength, double rho, double rhoInc, double alpha, double beta, bool cc)
        {
            this.rho = rho;
            this.epsilon = rhoInc;
            this.alpha = alpha;
            this.beta = beta;    
            //F2 = new LayerF2();
            complementCoding = cc;
            if (cc)
            {
                i = new double[inputVectorLength*2];
                F1 = new LayerF1(inputVectorLength*2);
            }
            else {
                i = new double[inputVectorLength];
                F1 = new LayerF1(inputVectorLength);
            }
        }
        public ConSelfART(int inputVectorLength, int categoryCountLimit, double rho, double alpha, double beta, bool cc)
        {
            this.CategoryCountLimit = categoryCountLimit;
            this.rho = rho;
            this.alpha = alpha;
            this.beta = beta;
            //F2 = new LayerF2();
            complementCoding = cc;
            if (cc)
            {
                i = new double[inputVectorLength*2];
                F1 = new LayerF1(inputVectorLength*2);
            }
            else
            {
                i = new double[inputVectorLength];
                F1 = new LayerF1(inputVectorLength);
            }
        }                
        public void reSetRho(double r)
        {
            rho = r;
        }
        private double[] complement(double []vector) {
            double[] complementVector = new double[vector.Length * 2];
            for (int i = vector.Length; i < complementVector.Length; i++) {
                complementVector[i - vector.Length] = vector[i - vector.Length];
                complementVector[i] = Math.Round(1 - vector[i - vector.Length],4);
            }
            return complementVector;
        }
        public void delayWeightUpdates(bool d) {
            delayUpdate = d;
        }
        public object[,] getCluster(int contextCode, double[] I)
        {            
            LayerF2 F2 = (LayerF2)contextField[contextCode];                                       
            object[,] cluster = new object[1, 2];
            double[] T = new double[F2.Count];
            double[] i = new double[I.Length * 2];
            if (complementCoding)
            {
                i = new double[I.Length * 2];
                i = complement(I);
            }
            else
            {
                i = new double[I.Length];
                i = I;
            }
            T = F1.processInput(i, alpha,F2);
            int repeatCount = 1;
        Repeater: // Control shall come up here in case there are more than one members in T,            
            object[,] j_AND_Z_td_J = F2.processInput(T);
            int j = (int)j_AND_Z_td_J[0, 0];
            double[] Z_td_J = (double[])j_AND_Z_td_J[0, 1];
            //Adaptation Phase                    
            if (vigilancePass(i, Z_td_J))
            {
                cluster[0, 0] = Z_td_J;
                cluster[0, 1] = j;
            }
            else
            {
                if (repeatCount <= T.Length)
                {
                    T[j] = -1;
                    repeatCount++;
                    goto Repeater;
                }
                // MinDist F2Neuron does not resonate with this i, make a new category for it
                else
                {
                    double[] d = { -1.0 };
                    cluster[0, 0] = d;
                    cluster[0, 1] = -1;
                }
            }
            return cluster;
        }
        public object[,] feedInput(int contextCode,double[] I,int catCode)
        {           
            object[,] output = new object[1, 2];
            double[] z_td_J_new = new double[i.Length];                  
            if (complementCoding)
            {                
                i = complement(I);             
            }
            else
            {             
                i = I;
            }
            if (!contextField.ContainsKey(contextCode))
            {
                //Add Context Code
                contextField.Add(contextCode, new LayerF2());
            }            
            //Load Context
            LayerF2 F2Neurons = (LayerF2)contextField[contextCode];                                       
            if (F2Neurons.Count == 0)
            {
                output[0, 1] = F2Neurons.AddF2Neuron(F1, i);
                output[0, 0] = ((F2Neuron)F2Neurons[0]).getProtoTypeCluster();                    
            }
            else
            {
                double[] T = new double[F2Neurons.Count];
                // Recognition Phase (Orientation Subsystem)
                T = F1.processInput(i, alpha, F2Neurons);
                int repeatCount = 1; 
            Repeater: // Control shall come up here in case there are more than one members in T,            
                object[,] j_AND_Z_td_J = F2Neurons.processInput(T);
                int J = (int)j_AND_Z_td_J[0, 0];               
                double[] Z_td_J = (double[])j_AND_Z_td_J[0, 1];
                //Adaptation Phase                                        
                if (vigilancePass(i, Z_td_J))
                {
                    if (((F2Neuron)F2Neurons[J]).getMapFieldConnection().getCategory().getCode() == catCode)
                    {
                        z_td_J_new = F2Neurons.updateWeights(Z_td_J, i, J, beta);
                        output[0, 0] = z_td_J_new;
                        output[0, 1] = J;
                    }
                    else { // invoke match tracking
                        double normIandZ = 0.0;
                        double normI = 0.0;
                        for (int index = 0; index < i.Length; index++)
                        {
                            normI += Math.Abs(i[index]);
                            normIandZ += Math.Abs((Math.Min(i[index], Z_td_J[index])));
                        }
                        double ro = Math.Round((normIandZ / normI), 4)+this.epsilon;                         
                        reSetRho(ro);
                        T[J] = -1;
                        repeatCount++;
                        goto Repeater;       
                    }                    
                }
                else
                {
                    if (repeatCount < T.Length){
                        T[J] = -1;
                        repeatCount++;
                        goto Repeater;                    
                    }
                    // MinDist F2Neuron does not resonate with this i, make a new category for it
                    else
                    {
                        if (CategoryCountLimit != -1 & F2Neurons.Count == CategoryCountLimit)
                        {
                            output[0, 1] = "-1";
                            output[0, 0] = i;
                        }
                        else
                        {
                            output[0, 1] = F2Neurons.AddF2Neuron(F1, i);
                            output[0, 0] = ((F2Neuron)F2Neurons[F2Neurons.Count - 1]).getProtoTypeCluster();
                        }
                    }
                }
            }
            
            return output;            
        }
        //public void commitUpdate(double[] Z_td_J, double []i, int j, double beta)
        //{
        //    if (complementCoding)
        //        F2.updateWeights(Z_td_J, complement(i), j, beta);
        //    else
        //        F2.updateWeights(Z_td_J, i, j, beta);
        //}
        //public double[] getCluster(int id)
        //{
        //    return F2.getCluster(id);
        //}
        //public int getClusterCount() {
        //    return F2.Count;
        //}
        private bool vigilancePass(double[] I, double[] Z) {
            double normIandZ = 0;            
            double normI = 0;
            bool resonate = false;
            //if (complementCoding)
            //{
                
            //    double Rj_new = 0.0;
            //    for (int index = 0; index < I.Length / 2; index++)
            //    {
            //        Rj_new += Math.Abs((Math.Max(I[index], Z[index]))) - Math.Abs((Math.Min(I[index], Z[index + (I.Length / 2)])));
            //    }                
            //    if (Rj_new > ((I.Length / 2) * (1 - rho)))
            //        resonate = false;
            //    else
            //        resonate = true;
            //}
            //else
            {
                for (int index = 0; index < I.Length; index++)
                {
                    normI += Math.Abs(I[index]);
                    normIandZ += Math.Abs((Math.Min(I[index], Z[index])));
                }
                double match = Math.Round((normIandZ / normI),4);
                if (rho <= (match))
                    resonate = true;
                else
                    resonate = false;
            }
            return resonate;
        }   
    }
}
