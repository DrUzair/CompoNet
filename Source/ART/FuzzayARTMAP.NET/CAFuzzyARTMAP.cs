/* 190706
 * Uzair Ahmad
 * OSLAB, Kyung Hee University
 * South Korea 
 */
using System;
using System.Collections;
using System.Text;
using System.IO;
namespace ConSelFAM.NET
{
    public class ConSelfARTMAP
    {
        private ConSelFAM.NET.ConSelfART ARTModule;
        private MAPField mapField;
        private double rho;
        private double rhoIncrement;
        private bool acomplementCoding;
        private double alpha;
        private double beta;
        public ConSelfARTMAP(int inputComponentCount,double initialRho,double rhoInc, double alpha, double beta, bool complementCoding, bool delayUpdate) {
            ARTModule = new ConSelFAM.NET.ConSelfART(inputComponentCount,-1,initialRho, alpha, beta, complementCoding);
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
        public int getCategoryCount(){
            return mapField.getCategoryCount();
        }
        public ConSelfARTMAP(string path)
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
            
                bool CC = bool.Parse(data.Substring(2, data.IndexOf("\r") - 2));
                this.complementCoding = CC;
                data = data.Substring(data.IndexOf("\r") + 2);
            
            //if (data.Contains("F1"))

                {
                    int F1Count = int.Parse(data.Substring(2, data.IndexOf("\r") - 2));
                    if (CC)
                    {
                        this.ARTModule = new ConSelFAM.NET.ConSelfART(F1Count/2, -1, this.rho, this.alpha, this.beta, this.complementCoding);
                    }else{
                        this.ARTModule = new ConSelFAM.NET.ConSelfART(F1Count, -1, this.rho, this.alpha, this.beta, this.complementCoding);
                    }
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
                        int F1count = this.ARTModule.F1.Count;
                        for (int j = 0; j < F1count; j++)
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
                        string catcode = data.Substring(0, data.IndexOf("\r")).Trim();
                        int code = int.Parse(catcode);              
                        mapField.getCategory(code).addConnection((F2Neuron)F2Layer[f2NeuronIndex]);                                                                                 
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
                MapFieldCategory cat = (MapFieldCategory)this.mapField.categories[catIndex];
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
                    netConfig += "CatCode\r\n";
                    //for (int f2Index = 0; f2Index < F2.Count; f2Index++)
                    {
                        MapFieldConnection mapFieldConn = ((F2Neuron)F2[f2Index]).getMapFieldConnection();
                        netConfig += mapFieldConn.getCategory().getCode() + "\r\n";                        
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
        
        public int learn(int contextCode,double[] Pattern, int categoryCode,string categoryName) {
            if(! mapField.categoryExists(categoryCode)){
                mapField.addNewCategory(categoryName, categoryCode);
            }
            // Initialize Rho with System Rho
            ARTModule.reSetRho(rho);

            double rhoInc = 0.0;
        
        Repeater:
            rhoInc += rhoIncrement;
            int f2NeuronCountBefore = 0;            
            ConSelFAM.NET.LayerF2 f2Neurons = (ConSelFAM.NET.LayerF2)ARTModule.contextField[contextCode]; // just to initialize
            //load context
            if (ARTModule.contextField.ContainsKey(contextCode))
            {
                f2Neurons = (ConSelFAM.NET.LayerF2)ARTModule.contextField[contextCode];
                f2NeuronCountBefore = f2Neurons.Count;
            }            
            object [,]artCluster = ARTModule.feedInput(contextCode,Pattern,categoryCode);
            int J = (int)artCluster[0, 1];
            double[] ZJ = (double[])artCluster[0, 0];

            f2Neurons = (ConSelFAM.NET.LayerF2)ARTModule.contextField[contextCode];
            int f2NeuronCountAfter = f2Neurons.Count;


            if (f2NeuronCountBefore != f2NeuronCountAfter)
            {
                // THIS MEANS THAT NEW F2 NEURON IS ADDED TO THIS CONTEXT
                MapFieldCategory cat = mapField.getCategory(categoryCode);
                cat.addConnection(((F2Neuron)f2Neurons[J]));
            }            
                                    
            return mapField.getAssociatedCategory((F2Neuron)f2Neurons[J]);
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

        //public int recall(int contextCode, double[] Pattern) // THIS METHOD ASSUMES THAT EACH F2 NODE HAS CONNECTIONS WITH ALL MAPFIELD NODES
        //{
        //    ARTModule.reSetRho(rho);
        //    int code = -1; // Don't Know   
        //    LayerF2 contextNeurons = (LayerF2)ARTModule.contextField[contextCode];        
        //    object [,]artCluster = ARTModule.getCluster(contextCode, Pattern);
        //    if ((int)artCluster[0, 1] == -1)
        //        code = -1;
        //    else
        //    {
        //        F2Neuron f2Neuron = (F2Neuron)contextNeurons[(int)artCluster[0, 1]];
        //        f2Neuron.getMapFieldConnections();
        //        ArrayList mapFieldConnections = f2Neuron.getMapFieldConnections();
        //        IEnumerator mapFieldConnEnum = mapFieldConnections.GetEnumerator();
        //        while (mapFieldConnEnum.MoveNext())
        //        {
        //            MapFieldConnection mapFieldConn = (MapFieldConnection)mapFieldConnEnum.Current;
        //            if (mapFieldConn.getWeight() == 1)
        //            {
        //                MapFieldCategory category = mapFieldConn.getCategory();
        //                code = category.getCode();
        //                //break;
        //            }
        //        }
        //    }
        //       // code = mapField.getAssociatedCategory(contextNeurons, (int)artCluster[0, 1]); 
        //    return code;
        //}
        public int recall(int contextCode, double[] Pattern) //THIS METHOD ASSUMES THAT EACH F2 NODE IS CONNECTED WITH ONLY ONE MAPFIELD NODE
        {
            ARTModule.reSetRho(rho);
            int code = -1; // Don't Know   
            LayerF2 contextNeurons = (LayerF2)ARTModule.contextField[contextCode];
            object[,] artCluster = ARTModule.getCluster(contextCode, Pattern);
            if ((int)artCluster[0, 1] == -1)
                code = -1;
            else
            {
                F2Neuron f2Neuron = (F2Neuron)contextNeurons[(int)artCluster[0, 1]];
                MapFieldConnection mfConn= f2Neuron.getMapFieldConnection();
                MapFieldCategory category = mfConn.getCategory();
                code = category.getCode();      
            }
            // code = mapField.getAssociatedCategory(contextNeurons, (int)artCluster[0, 1]); 
            return code;
        }
        public MapFieldCategory recallCat(int contextCode, double[] Pattern) //THIS METHOD ASSUMES THAT EACH F2 NODE IS CONNECTED WITH ONLY ONE MAPFIELD NODE
        {
            ARTModule.reSetRho(rho);            
            LayerF2 contextNeurons = (LayerF2)ARTModule.contextField[contextCode];
            object[,] artCluster = ARTModule.getCluster(contextCode, Pattern);
            if ((int)artCluster[0, 1] == -1)
                return null;
            else
            {
                F2Neuron f2Neuron = (F2Neuron)contextNeurons[(int)artCluster[0, 1]];
                MapFieldConnection mfConn = f2Neuron.getMapFieldConnection();
                return mfConn.getCategory();
            }
        }
        public int getCatCode(string name)
        {
            return this.mapField.getCategoryCode(name);
        }
        public MapFieldCategory getCat(int index) {
            return this.mapField.getCategory(index, "");
        }
    }
}
