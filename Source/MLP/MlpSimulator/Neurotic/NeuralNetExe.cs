using System;
using System.Threading;
using System.Collections;
using System.Text;


namespace Neurotic
{
    public class NeuralNetExe
    {
        
        class WeightsMatrix
        {
            float[] weights;

            public WeightsMatrix(int totalWeights)
            {
                weights = new float[totalWeights];
            }

            public void addWeight(object val, int index)
            {
                weights.SetValue(val, index);
            }
            public float getWeight(int index)
            {
                return (float)weights.GetValue(index);
            }
        }
        // path where neural data and inputs are located
        
        string targetspath;
        private float []minTgt;
        private float []maxTgt;
        private float []minTrg;
        private float []maxTrg;
        public int[] getNNStructure(){
            int[] structure = new int[net.layers.Count];
            for (int count = 0; count < structure.Length; count++)
            {
                structure[count] = ((NeuralNet.Layer)net.layers[count]).getNeuronCount();
            }
            return structure;
        }
        public string getTargetsPath()
        {
            return targetspath;
        }
        public void setTargetsPath(string p)
        {
            this.targetspath = p;
        }
        NeuralNet net;
        Neurotic.NeuralReader neuralReader;
        public NeuralNetExe(string name)
        {
            net = new NeuralNet(name);            
            neuralReader = new NeuralReader();            
        }

        /* loadNerualNet protocol
         * xyz.net file extension
         * contents should be as following
         * network structure ; netwok weights ; network biases ; input preprocessing componenets ; output postprocessing components
         * network structure should be as follws
         * neurons at input layer , activation function code, neurons at hidden layer 1 , activation function code ... and so on
         * network weights should be as follows
         * weights of neuron one of layer one and then next neuron and next layer and so on, must be comma separated
         * network biases should be as follows
         * bias of neuron one of layer one and then next neuron and next layer and so on, must be comma separated
         * input preprocessing components
         * comma separated minTrg, maxTrg of first neuron of input layer and then second and so on ...
         * output postprocessing components
         * comma separated minTgt,maxTgt of first neuron of output laer and then next and so on ....
         
         */
        public void loadNeuralNet(string fileName)  {
            try
            {
                if (neuralReader.netConfigFound(fileName))
                {
                    ArrayList config = neuralReader.readConfiguration(fileName);
                    if (config == null)
                    {
                        throw new ConfigFileException("Config file format is not recognized");
                    }

                    IEnumerator configEnum = config.GetEnumerator();
                    configEnum.MoveNext();
                    ArrayList structure = (ArrayList)configEnum.Current;

                    configEnum.MoveNext();
                    ArrayList weights = (ArrayList)configEnum.Current;
                    float[] w = new float[weights.Count];
                    IEnumerator eWeights = weights.GetEnumerator();
                    int count = 0;
                    while (eWeights.MoveNext())
                        w[count++] = (float)Math.Round(Convert.ToDecimal(eWeights.Current), 4);

                    configEnum.MoveNext();
                    ArrayList biases = (ArrayList)configEnum.Current;
                    float[] b = new float[biases.Count];
                    IEnumerator eBiases = biases.GetEnumerator();
                    count = 0;
                    while (eBiases.MoveNext())
                        b[count++] = (float)Math.Round(Convert.ToDecimal(eBiases.Current), 4);

                    populateNeuralNet(structure, w, b);

                    configEnum.MoveNext();
                    ArrayList normComps = (ArrayList)configEnum.Current;
                    IEnumerator normEnum = normComps.GetEnumerator();
                    minTrg = new float[normComps.Count / 2];
                    maxTrg = new float[normComps.Count / 2];
                    int x = 0;
                    while (normEnum.MoveNext())
                    {
                        minTrg[x] = (float)Convert.ToDecimal(normEnum.Current.ToString());
                        normEnum.MoveNext();
                        maxTrg[x] = (float)Convert.ToDecimal(normEnum.Current.ToString());
                        x++;
                    }

                    configEnum.MoveNext();
                    ArrayList denormComps = (ArrayList)configEnum.Current;
                    IEnumerator denormEnum = denormComps.GetEnumerator();
                    minTgt = new float[denormComps.Count / 2];
                    maxTgt = new float[denormComps.Count / 2];
                    x = 0;
                    while (denormEnum.MoveNext())
                    {
                        minTgt[x] = (float)Convert.ToDecimal(denormEnum.Current.ToString());
                        denormEnum.MoveNext();
                        maxTgt[x] = (float)Convert.ToDecimal(denormEnum.Current.ToString());
                        x++;
                    }
                }
            }
            catch (Exception fnf) {
                throw new ConfigFileException("Config file is missing.");
            }
        }
        private void populateNeuralNet(ArrayList Structure,float []Weights,float []Biases)
        {
            //short[] neurons = structure;         
            
            net.initNeuralNet(Structure);
            float[] w = Weights;

            // create links
            int totalLinks = 0;

            // calculate total number of links
            for (int i = 0; i < Structure.Count - 1; i++)
            {
                totalLinks += ((short[,])(Structure[i]))[0, 0] * ((short[,])(Structure[i+1]))[0, 0];
            }

            // initialize WeightsCollection
            WeightsMatrix weights = new WeightsMatrix(totalLinks);
            for (int i = 0; i < totalLinks; i++)
            {
                weights.addWeight(w[i], i);
            }

            // initialize Links
            int linkcount = 0;
            for (short layerCount = 0; layerCount + 1 < Structure.Count; layerCount++)
            {
                // get adjacent layers
                NeuralNet.Layer layerOne = net.getLayer(layerCount);
                NeuralNet.Layer layerTwo = net.getLayer(layerCount + 1);

                // traverse nerons of second layer
                for (int toNeuronCount = 0; toNeuronCount < layerTwo.getNeuronCount(); toNeuronCount++)
                {
                    // traverse all neurons of first layer
                    for (int fromNeuronCount = 0; fromNeuronCount < layerOne.getNeuronCount(); fromNeuronCount++)
                    {
                        // create a link
                        NeuralNet.Link link = new NeuralNet.Link(layerOne.getNeuron(fromNeuronCount), layerTwo.getNeuron(toNeuronCount));
                        link.setWeight(weights.getWeight(linkcount++));
                    }
                }

            }

            // biases
            float[] biases = Biases;
            // initialize neurons with biases
            int count = 0;
            for (short layerCount = 1; layerCount < net.layers.Count; layerCount++)
            {

                for (int neuronCount = 0; neuronCount < net.getLayer(layerCount).getNeuronCount(); neuronCount++)
                {
                    net.getLayer(layerCount).getNeuron(neuronCount).setBias(biases[count++]);
                }
            }

        }
        //    to read net configuration(layers/neurons/biases and weights from file
        //	float [,] inputData;


        //

        float[,] inputData;

        private bool loadInputs(string path)
        {
            string fileName = path + "\\" + this.net.getName() + ".inp";
            bool loaded = false;
            if (neuralReader.netConfigFound(fileName))
            {
                inputData = neuralReader.readInputs(fileName, net.getLayer(0).getNeuronCount());
                loaded = true;
            }
            else
            {
                loaded = false;
            }

            return loaded;
        }
        public void feedInput(float [,]inputVector){
            inputData = Normalize(inputVector);            
        }

        int getInputDataLength()
        {
            return inputData.GetLength(0);
        }

        private float[] getInputRow(int rowNum)
        {
            int rowLength = net.getLayer(0).getNeuronCount();
            float[] row = new float[rowLength];
            for (int valCount = 0; valCount < rowLength; valCount++)
            {
                row[valCount] = inputData[rowNum, valCount];
            }
            return row;
        }

        public void runNeuralNet()
        {
                lock (this)
                {
                    // execute neurons
                    for (int inputCount = 0; inputCount < getInputDataLength(); inputCount++)
                    {
                        float[] inputRow = getInputRow(inputCount); //new float[]{-0.73626F};

                        // input data to input layer neurons
                        NeuralNet.Layer inputLayer = net.getLayer(0);

                        // for each input value
                        short neuronCount;
                        for (short inputValCount = 0; inputValCount < inputRow.Length; inputValCount++)
                        {
                            // for each input follow the following process

                            //set input value in everyToLink from this neuron
                            neuronCount = inputValCount;
                            object[] toLinks = (inputLayer.getNeuron(neuronCount).getToLinks()).ToArray();
                            for (int toLinkCount = 0; toLinkCount < toLinks.Length; toLinkCount++)
                            {

                                ((NeuralNet.Link)(toLinks[toLinkCount])).setInputValue(inputRow[inputValCount]);
                            } // input layer neuron initialized with input

                        } // Inputs Dissimnated

                        // Execute next layer neurons now
                        for (int layerCount = 1; layerCount < net.layers.Count; layerCount++)
                        {
                            for (neuronCount = 0; neuronCount < net.getLayer(layerCount).getNeuronCount(); neuronCount++)
                            {
                                net.getLayer(layerCount).getNeuron(neuronCount).execute();
                            } // One Neuron Executed
                        } // One Layer Executed						
                    } // inputs are finished
                }// lock ends        
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>      

        public float[] getNNOutput(int denormType) // denormType 0 (NO Denorm) OR 1(0~1) OR 2 (-1~1)
        {
            int numOutputs = net.getLayer(net.layers.Count - 1).getNeuronCount();
            float[] f = new float[numOutputs];
            for (int count = 0; count < numOutputs; count++) {
                NeuralNet.Layer layer = net.getLayer(net.layers.Count - 1);
                NeuralNet.OutputNeuron o = (NeuralNet.OutputNeuron)layer.getNeuron(count);
                f[count] = (float)Convert.ToDecimal((o.getOutputValues())[0]);
            }
            if (denormType == 0)
                return f;
            else if (denormType == 1)
                return Denormalize(f, true);
            else
                return Denormalize(f, false);
        }
        private float[] Denormalize(float []i,bool zeroToOne){            
            //p = 0.5(pn+1)*(maxp-minp) + minp;
            float []denormI = new float[i.Length];
            for (int a = 0; a < i.Length; a++)
            {
                if(zeroToOne)
                    denormI[a] = (float)(i[a]*(maxTgt[a] - minTgt[a]) + minTgt[a]);
                else
                    denormI[a] = (float)(0.5*(i[a]+1)*(maxTgt[a]-minTgt[a])+minTgt[a]);
            }
            return denormI;            
        }        
        private float[,] Normalize(float [,]i){
            float[,] nInputVector = new float[1,i.Length];
            // pn = 2*(p-minp)/(maxp-minp) - 1;
            for (int a = 0; a < i.Length; a++)
            {
                nInputVector[0,a] = 2 * (i[0,a] - minTrg[a]) / (maxTrg[a] - minTrg[a]) - 1;
            }
            
            return nInputVector;
        }
    } // NeuralNetExe ends
}
