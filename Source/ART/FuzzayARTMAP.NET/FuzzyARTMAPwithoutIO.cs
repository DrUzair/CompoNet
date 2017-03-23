/* 190706
 * Uzair Ahmad
 * OSLAB, Kyung Hee University
 * South Korea 
 */
using System;
using System.Collections;
using System.Text;
using ContextAwareFuzzyART.NET;
namespace FuzzyARTMAP.NET
{
    public class FuzzyARTMAP
    {
        ContextAwareFuzzyART.NET.ContextAwareFuzzyART ARTModule;
        MAPField mapField;        
        double rho;
        double rhoIncrement;
        bool complementCoding;
        double alpha;
        double beta;
        public FuzzyARTMAP(int inputComponentCount,double initialRho,double rhoInc, double alpha, double beta, bool complementCoding, bool delayUpdate) {
            ARTModule = new ContextAwareFuzzyART.NET.ContextAwareFuzzyART(inputComponentCount,-1,initialRho, alpha, beta, complementCoding);
            ARTModule.delayWeightUpdates(delayUpdate);
            mapField = new MAPField();            
            if(complementCoding)
                ARTModule.F1 = new LayerF1(2*inputComponentCount);
            else
                ARTModule.F1 = new LayerF1(inputComponentCount);
            rho = initialRho;
            rhoIncrement = rhoInc;
            this.complementCoding = complementCoding;
            this.alpha = alpha;
            this.beta = beta;
        }

        double[] complementCode(double[] Pattern) {
            double[] cPattern = new double[Pattern.Length * 2];
            for (int i = 0, c = Pattern.Length; i < Pattern.Length; i++)
            {
                cPattern[i] = Pattern[i];
                cPattern[c++] = Math.Round(1 - Pattern[i], 2);
            }
            return cPattern;        
        }
        public int learn(int contextCode,double[] Pattern, int categoryCode) {
            // Initialize Rho with System Rho
            ARTModule.reSetRho(rho);

            double rhoInc = 0.0;
        
        Repeater:
            rhoInc += rhoIncrement;
            int f2NeuronCountBefore = 0;            
            ContextAwareFuzzyART.NET.LayerF2 f2Neurons = (ContextAwareFuzzyART.NET.LayerF2)ARTModule.contextField[contextCode]; // just to initialize
            //load context
            if (ARTModule.contextField.ContainsKey(contextCode))
            {
                f2Neurons = (ContextAwareFuzzyART.NET.LayerF2)ARTModule.contextField[contextCode];
                f2NeuronCountBefore = f2Neurons.Count;
            }            
            object [,]artCluster = ARTModule.feedInput(contextCode,Pattern);
            int J = (int)artCluster[0, 1];
            double[] ZJ = (double[])artCluster[0, 0];

            f2Neurons = (ContextAwareFuzzyART.NET.LayerF2)ARTModule.contextField[contextCode];
            int f2NeuronCountAfter = f2Neurons.Count;

            bool categoriesUpdated = false;
            while (!categoriesUpdated) {
                if ( /*New Category*/ !mapField.categoryExists(categoryCode) && /*New_F2Neuron*/ (f2NeuronCountBefore != f2NeuronCountAfter))
                {
                    //Update F2Neurons to have connection with this category
                    mapField.addNewCategory(f2Neurons, "", categoryCode);
                    //Update Categories, Except this category(categoryCode), to have connection with this new F2 Neuron
                    mapField.updateCategories(categoryCode, (F2Neuron)f2Neurons[J]);
                    // Associate new F2Neuron with this Category 
                    //mapField.associate(categoryCode, J);
                    mapField.associate(categoryCode, (F2Neuron)f2Neurons[J]);
                    // All connections are updated, turn on the flag
                    categoriesUpdated = true;
                    // break out
                    break;
                }
                if ( /*New Category*/ !mapField.categoryExists(categoryCode) && /* NOT New_F2Neuron*/ (f2NeuronCountBefore == f2NeuronCountAfter))
                {
                    //Update F2Neurons to have connection with this category
                    mapField.addNewCategory(f2Neurons, "", categoryCode);
                    // All connections are updated, turn on the flag
                    categoriesUpdated = true;
                    // break out
                    break;
                }
                if ( /*NOT New Category*/ mapField.categoryExists(categoryCode) && /* New_F2Neuron*/ (f2NeuronCountBefore != f2NeuronCountAfter)) 
                {
                    //Update Categories to have connection with this new F2 Neuron
                    mapField.updateCategories((F2Neuron)f2Neurons[J]);
                    // Associate new F2Neuron with this Category 
                    //mapField.associate(categoryCode, J);
                    mapField.associate(categoryCode, (F2Neuron)f2Neurons[J]);
                    // break out
                    break;
                }
                if ( /*Not New Category*/ mapField.categoryExists(categoryCode) && /* NOT New_F2Neuron*/ (f2NeuronCountBefore == f2NeuronCountAfter))
                {
                    // All connections are updated, turn on the flag
                    categoriesUpdated = true;
                }
            }

            //if (!mapField.categoryExists(categoryCode))
            //{
            //    mapField.addNewCategory(f2Neurons, "", categoryCode);
            //    categoriesUpdated = true;
            //}// Uptil this point all F2 neurons have got associatedConnection with this category

            
            if (f2NeuronCountBefore != f2NeuronCountAfter)
            {
                //if (!categoriesUpdated)
                //{
                //    // This F2Neuron is newly added, therefore update all MapFiled categories (in this context) to have connection with this neuron
                //    mapField.updateCategories((F2Neuron)f2Neurons[J]);           
                //}
                // Since this F2Neuron is newly created
                // Bind it to this category
                
            }

            if (mapField.getConnectionWeight(J, categoryCode) == 1)
            {                
                return mapField.getAssociatedCategory(f2Neurons, J);                
            }
            else {
                double norm_ztd_AND_pattern = norm(fuzzyIntersection(Pattern, ZJ));
                double norm_pattern = norm(Pattern);
                //ARTModule.reSetRho(Math.Round((norm_ztd_AND_pattern / norm_pattern) + rhoIncrement, 2));                                
                ARTModule.reSetRho(rho + rhoInc);                
                goto Repeater;
            }
            return mapField.getAssociatedCategory(f2Neurons, J);
        }
        
        private double norm(double[] vector) {
            double norm = 0 ;
            for (int i = 0; i < vector.Length; i++) {                
                    norm += vector[i];
            }
            return Math.Round(norm,2); 
        }

        private double[] fuzzyIntersection(double []vectorA, double []vectorB){
            double []intersection = new double[vectorA.Length];
            for(int i = 0 ; i < vectorA.Length ; i++){                
                intersection[i] = Math.Min(vectorA[i],vectorB[i]);                
            }
            return intersection;
        }

        public int recall(int contextCode, double[] Pattern)
        {
            ARTModule.reSetRho(rho);
            int code = -1; // Don't Know   
            LayerF2 contextNeurons = (LayerF2)ARTModule.contextField[contextCode];        
            object [,]artCluster = ARTModule.getCluster(contextCode, Pattern);
            if ((int)artCluster[0, 1] == -1)
                code = -1;
            else
            {
                F2Neuron f2Neuron = (F2Neuron)contextNeurons[(int)artCluster[0, 1]];
                f2Neuron.getMapFieldConnections();
                ArrayList mapFieldConnections = f2Neuron.getMapFieldConnections();
                IEnumerator mapFieldConnEnum = mapFieldConnections.GetEnumerator();
                while (mapFieldConnEnum.MoveNext())
                {
                    MapFieldConnection mapFieldConn = (MapFieldConnection)mapFieldConnEnum.Current;
                    if (mapFieldConn.getWeight() == 1)
                    {
                        MapFieldCategory category = mapFieldConn.getCategory();
                        code = category.getCode();
                        //break;
                    }
                }
            }
               // code = mapField.getAssociatedCategory(contextNeurons, (int)artCluster[0, 1]); 
            return code;
        }
    
    }
}
