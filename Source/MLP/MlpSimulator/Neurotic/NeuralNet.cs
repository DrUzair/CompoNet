using System;
using System.Collections;
using System.Text;

namespace Neurotic
{
    class NeuralNet
    {
        static ArrayList pastResults;
        private string name;
        public string getName()
        {
            return name;
        }
        public ArrayList layers;
        public Layer getLayer(int layerNum)
        {
            object[] o = layers.ToArray();
            return (Layer)o[layerNum];
        }

        public NeuralNet(string name)
        {
            this.name = name;
            pastResults = new ArrayList();
        }

        public void setResult(int errorCount)
        {
            pastResults.Add(errorCount);
        }

        public object[] getResult()
        {
            object[] errors = pastResults.ToArray();
            return errors;

        }

        public void initNeuralNet(ArrayList structure)
        {
            short[,] s = new short[structure.Count, 2];
            IEnumerator eStruct = structure.GetEnumerator();
            int count = 0;
            while (eStruct.MoveNext())
            {
                short[,] t = new short[1, 2];
                t = (short[,])(eStruct.Current);
                s[count, 0] = t[0, 0];
                s[count++, 1] = t[0, 1];
            }

            layers = new ArrayList();
            for (int i = 0; i < structure.Count; i++)
                if (i == 0)
                    layers.Add(new Layer("input", 0, s[i,0]));
                else if (i + 1 == structure.Count)
                    layers.Add(new Layer("output", s[i, 1], s[i, 0]));
                else
                    layers.Add(new Layer("hidden", s[i, 1], s[i, 0]));

        }

        // inner class
        public class Link
        {

            //Each link has an input from source that becomes input to destination
            double input;
            public void setInputValue(double input)
            {
                this.input = input;
            }

            public double getInputValue()
            {
                return input;
            }

            double weight;
            public double getWeight()
            {
                return this.weight;
            }

            Neuron source, destination;

            public NeuralNet.Neuron getSourceNeuron()
            {
                return source;
            }

            public NeuralNet.Neuron getDestinationNeuron()
            {
                return destination;
            }

            public Link(Neuron source, Neuron destination)
            {
                this.source = source;
                source.addToLink(this);
                this.destination = destination;
                destination.addFromLink(this);
            }

            public void setWeight(double weight)
            {
                this.weight = weight;
            }
        }

        public class Layer
        {            
            ArrayList neurons;
            public Layer(string layerType, short TransferFunction,int neuronNum)
            {
                neurons = new ArrayList(neuronNum);                
                for (int i = 0; i < neuronNum; i++)
                {
                    if (layerType == "input")
                        neurons.Add(new InputNeuron());
                    else if (layerType == "output")
                    {
                        OutputNeuron n = new OutputNeuron();
                        n.setTransfetFuntion(TransferFunction);
                        neurons.Add(n);
                    }
                    else if (layerType == "hidden")
                    {
                        Neuron n = new Neuron();
                        n.setTransfetFuntion(TransferFunction);
                        neurons.Add(n);
                    }
                }
            }
            public void addNeuron(Neuron n)
            {
                neurons.Add(n);
            }
            public Neuron getNeuron(int i)
            {
                object[] o = neurons.ToArray();
                return (Neuron)o[i];
            }
            public int getNeuronCount()
            {
                return neurons.Count;
            }
        }
        // inner class
        public class Neuron
        {
            short transferFunction;
            ArrayList toLinks; // going out
            public void addToLink(NeuralNet.Link tolink)
            {
                toLinks.Add(tolink);
            }
            ArrayList fromLinks; // coming in
            public void addFromLink(NeuralNet.Link fromlink)
            {
                fromLinks.Add(fromlink);
            }
            public ArrayList getToLinks()
            {
                return this.toLinks;
            }
            public ArrayList getFromLinks()
            {
                return this.fromLinks;
            }
            protected double bias;

            public void setBias(float bias)
            {
                this.bias = bias;
            }
            public void setTransfetFuntion(short tf) {
                transferFunction = tf;
            }
            public Neuron()
            {
                toLinks = new ArrayList();
                fromLinks = new ArrayList();
            }
            
            public void execute()
            {
                double sum = 0.0;
                double output = 0.0;

                object[] fromLinks = this.getFromLinks().ToArray();

                for (int inputLinksCount = 0; inputLinksCount < fromLinks.Length; inputLinksCount++)
                {
                    sum += ((Link)fromLinks[inputLinksCount]).getWeight() * ((Link)fromLinks[inputLinksCount]).getInputValue();
                }
                sum = sum + this.bias;

                if (transferFunction == 1)
                {
                    output = (1 + Math.Exp(-sum));
                    output = 1 / output;
                }
                if (transferFunction == 2) {
                    //n = 2/(1+exp(-2*n))-1 TANSIG
                    output = 2/(1+Math.Exp(-2*sum))-1;
                }


                object[] toLinks = this.getToLinks().ToArray();

                for (int outputLinksCount = 0; outputLinksCount < toLinks.Length; outputLinksCount++)
                {
                    ((Link)toLinks[outputLinksCount]).setInputValue(output);
                }

                if (this.GetType().ToString() == "Neurotic.NeuralNet+OutputNeuron")
                {
                    NeuralNet.OutputNeuron n = (NeuralNet.OutputNeuron)this;
                    n.setOutputValue(output);
                }
            }


        }// Neuron class ends
        public class InputNeuron : Neuron
        {
            
        }// InputNeuron ends
        public class OutputNeuron : Neuron
        {
            private ArrayList outputs;
            bool available = false;

            public bool isOutputAvailable()
            {
                return available;
            }

            public OutputNeuron()
            {
                outputs = new ArrayList();
                available = false;
            }

            public void setOutputValue(double output)
            {
                if (available == false)
                {
                    // old outputs are consumed
                    outputs = new ArrayList();
                }
                this.outputs.Add(output);
                available = true;
            }

            public ArrayList getOutputValues()
            {
                available = false;
                return outputs;
            }
            //public double getOutput(double input, Link link)
            //{
            //    double sum = 0.0;
            //    double output = 0.0;
            //    sum = input * link.getWeight();
            //    sum = sum - bias;
            //    output = 1 / (1 + Math.Pow(2.718282, -sum));
            //    return output;
            //}

            //new public void addToLink(NeuralNet.Link fromlink)
            //{
            //    // do nothing as this neuron has no To neuron
            //}
        }
    
    
    }
}
