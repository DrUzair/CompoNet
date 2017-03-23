/* 190706
 * Uzair Ahmad
 * OSLAB, Kyung Hee University
 * South Korea 
 */
using System;
using System.Collections;
using System.Text;
using System.IO;
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
        public FuzzyARTMAP(string path)
        {
            string data = "";
            if (netConfigFound(path))
            {
                try
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(path);
                    data = reader.ReadToEnd();
                    reader.Close();
                }
                catch (Exception ioerror)
                {
                    Console.Out.WriteLine(ioerror.ToString());
                    return;
                } // cath ends
            }
            //if (data.Contains("Rho")) POCKET PC .NET DO NOT SUPPORT String.Contains() Method
            {
                float rho = float.Parse(data.Substring(4, data.IndexOf("\r") - (4)));
                this.rho = rho;
                data = data.Substring(data.IndexOf("\r") + 2);
            }
            {
                float rhoIncrement = float.Parse(data.Substring(7, data.IndexOf("\r") - 7));
                this.rhoIncrement = rhoIncrement;
                data = data.Substring(data.IndexOf("\r") + 2);
            }
            //if (data.Contains("Alpha"))
            {
                float alpha = float.Parse(data.Substring(5, data.IndexOf("\r") - 5));
                this.alpha = alpha;
                data = data.Substring(data.IndexOf("\r") + 2);
            }
            //if (data.Contains("Beta"))
            {
                float beta = float.Parse(data.Substring(4, data.IndexOf("\r") - 4));
                this.beta = beta;
                data = data.Substring(data.IndexOf("\r") + 2);
            }
            //if (data.Contains("CC"))
            {
                bool CC = bool.Parse(data.Substring(2, data.IndexOf("\r") - 2));
                this.complementCoding = CC;
                data = data.Substring(data.IndexOf("\r") + 2);
            }
            //if (data.Contains("F1"))

            {
                int F1Count = int.Parse(data.Substring(2, data.IndexOf("\r") - 2));
                this.ARTModule = new ContextAwareFuzzyART.NET.ContextAwareFuzzyART(F1Count, -1, this.rho, this.alpha, this.beta, this.complementCoding);
                data = data.Substring(data.IndexOf("\r") + 2);
            }
            {                
                int MFCategoryCount = Int32.Parse(data.Substring(16, data.IndexOf("\r") - 16)); //16 characters in 'MFCategoryCount '
                data = data.Substring(data.IndexOf("\r") + 2);  
                this.mapField = new MAPField();
                for(int i =0;i < MFCategoryCount;i++){
                    int catCode = Int32.Parse(data.Substring(0, data.IndexOf("\r")));
                    data = data.Substring(data.IndexOf("\r") + 2);  
                    string catName = data.Substring(0, data.IndexOf("\r"));
                    data = data.Substring(data.IndexOf("\r") + 2);  
                    this.mapField.addNewCategory(catName,catCode);
                }
                int contextCount = Int32.Parse(data.Substring(13, data.IndexOf("\r") - 13)); //13 characters in 'ContextCount '
                data = data.Substring(data.IndexOf("\r") + 2);  
                for (int contextIndex = 0; contextIndex < contextCount; contextIndex++)
                {                       
                    int contextCode = Int32.Parse(data.Substring(8/*7 characters in 'Context '*/, data.IndexOf("\r") - 8));
                    LayerF2 F2Layer = new LayerF2();
                    this.ARTModule.contextField.Add(contextCode, F2Layer);
                    data = data.Substring(data.IndexOf("\r") + 2);
                    int F2Count = 0;
                    //if (data.Contains("F2"))
                    F2Count = int.Parse(data.Substring(8/*8 characters in 'F2Count '*/, data.IndexOf("\r") - 8));
                    data = data.Substring(data.IndexOf("\r") + 2);
                    for (int i = 0; i < F2Count; i++)
                    {
                    // Parse and set F2 Neuron Prototype 
                        double[] prototype = new double[this.ARTModule.F1.Count];                        
                        data = data.Substring(data.IndexOf("\r") + 2);
                        string pt = data.Substring(0, data.IndexOf("\r"));
                        for (int j = 0; j < this.ARTModule.F1.Count; j++)
                        {
                            prototype[j] = double.Parse(pt.Substring(0, pt.IndexOf("\t")).Trim());
                            if (j < (this.ARTModule.F1.Count - 1))
                                pt = pt.Substring(pt.IndexOf("\t") + 1);
                        }
                        data = data.Substring(data.IndexOf("\r") + 2);
                        data = data.Substring(data.IndexOf("\r") + 2);//discard tdConnWs word from data string
                    // Parse and set tdConnection Weights
                        double[] weights = new double[this.ARTModule.F1.Count];
                        string ws = data.Substring(0, data.IndexOf("\r"));
                        for (int j = 0; j < this.ARTModule.F1.Count; j++)
                        {
                            weights[j] = double.Parse(ws.Substring(0, ws.IndexOf("\t")).Trim());
                            if (j < (this.ARTModule.F1.Count - 1))
                                ws = ws.Substring(ws.IndexOf("\t") + 1);
                        }
                        data = data.Substring(data.IndexOf("\r") + 2);
                    
                        int f2NeuronIndex = F2Layer.AddF2Neuron((LayerF1)this.ARTModule.F1, weights,prototype);
                    // Parse and set map field connection weights                        
                        data = data.Substring(data.IndexOf("\r") + 2);                    
                        string mfConnWs = data.Substring(0, data.IndexOf("\r"));
                        for (int catIndex = 0; catIndex < this.mapField.getCategoryCount(); catIndex++)
                        {
                            int weight = int.Parse(mfConnWs.Substring(0, mfConnWs.IndexOf("\t")).Trim());
                            MapFieldConnection mfConn = new MapFieldConnection(this.mapField.getCategory(catIndex), (F2Neuron)F2Layer[f2NeuronIndex], weight);
                            this.mapField.getCategory(catIndex).addConnection(mfConn);
                            mfConnWs = mfConnWs.Substring(mfConnWs.IndexOf("\t") + 1);
                            //((F2Neuron)F2Layer[f2NeuronIndex]).addMapFieldConnection((object)mfConn);
                        }
                        data = data.Substring(data.IndexOf("\r") + 2);                                            
                    }                    
                }
            }
        }
        public bool netConfigFound(string filepath)
        {
            bool found = false;
            try
            {
                found = System.IO.File.Exists(filepath);
            }
            catch (Exception ioerror)
            {
                throw new System.IO.FileNotFoundException("Config File is not found");
            } // cath ends
            return found;
        }
        public void saveNetworkOnFile(string path) {
            // Delete the file if it exists.
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream fs = File.Create(path);
            string netConfig = "";
            netConfig += "Rho " + this.rho + "\r\n";
            netConfig += "RhoInc " + this.rhoIncrement + "\r\n";
            netConfig += "Alpha " + this.alpha + "\r\n";
            netConfig += "Beta " + this.beta + "\r\n";
            netConfig += "CC " + this.complementCoding + "\r\n";
            netConfig += "F1 " + this.ARTModule.F1.Count + "\r\n";
            netConfig += "MFCategoryCount " + this.mapField.getCategoryCount() + "\r\n";
            for (int catIndex = 0; catIndex < this.mapField.getCategoryCount(); catIndex++) {
                MapFieldCategory cat = this.mapField.getCategory(catIndex);
                netConfig += cat.getCode() + "\r\n";
                netConfig += cat.getName() + "\r\n";                
            }
            netConfig += "ContextCount " + this.ARTModule.contextField.Count + "\r\n";
            ICollection contextKeys = this.ARTModule.contextField.Keys;            
            IEnumerator contextKeysEnum = contextKeys.GetEnumerator();            
            while(contextKeysEnum.MoveNext()){
                int contextKey = Int32.Parse(contextKeysEnum.Current.ToString());            
                netConfig += "Context " + contextKey + "\r\n";
                LayerF2 F2 = ((LayerF2)this.ARTModule.contextField[contextKey]);
                netConfig += "F2Count " + F2.Count + "\r\n";
                for (int f2Index = 0; f2Index < F2.Count; f2Index++)
                {
                    netConfig += "Prototype\r\n";
                    double []prototype = ((F2Neuron)F2[f2Index]).getProtoTypeCluster();
                    for (int i = 0; i < prototype.Length; i++) {
                        netConfig += prototype[i] + "\t";
                    }
                    netConfig += "\r\n";
                    netConfig += "tdConnWs\r\n";
                    SynapticConnection[] conns = ((F2Neuron)F2[f2Index]).getConnections();
                    for (int connIndex = 0; connIndex < conns.Length; connIndex++)
                    {
                        netConfig += ((SynapticConnection)conns[connIndex]).getWeight() + "\t";
                    }
                    netConfig += "\r\n";
                    netConfig += "MFConns \r\n";
                    //for (int f2Index = 0; f2Index < F2.Count; f2Index++)
                    {
                        ArrayList mapFieldConns = ((F2Neuron)F2[f2Index]).getMapFieldConnections();
                        for (int mapFieldConnIndex =  0; mapFieldConnIndex < mapFieldConns.Count; mapFieldConnIndex++)
                        {                     
                            netConfig += ((MapFieldConnection)mapFieldConns[mapFieldConnIndex]).getWeight() + "\t";
                        }
                        netConfig += "\r\n";
                    }
                }                
                
            }
            AddText(fs, netConfig);
            fs.Close();
        }
        private void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
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
