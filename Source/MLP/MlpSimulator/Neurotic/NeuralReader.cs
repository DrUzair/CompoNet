using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Neurotic
{
    public class NeuralReader
    {
        string data;
        float[,] inputs;


        public NeuralReader()
        {

        }

        public void readNeuralNet(string path)
        {

        }

        //	public float[] loadNeuralNetBiases(){

        //}
        public ArrayList readTargets(string path)
        {
            try
            {

                System.IO.StreamReader reader = new System.IO.StreamReader(path);
                data = reader.ReadToEnd();
                reader.Close();
                System.IO.File.Delete(path);
            }
            catch (Exception ioerror)
            {
                Console.Out.WriteLine(ioerror.ToString());
            }

            ArrayList targetValues = new ArrayList();

            int targetValSize;
            int commaIndex = 0;
            string targetVal;
            while (true)
            {
                targetValSize = data.IndexOf(",", commaIndex);
                if (targetValSize == -1)
                {
                    targetVal = data.Substring(commaIndex, 1);
                    targetValues.Add(System.Single.Parse(targetVal));
                    break;
                }
                targetVal = data.Substring(commaIndex, targetValSize - commaIndex);
                targetValues.Add(System.Single.Parse(targetVal));
                commaIndex = targetValSize + 1;

            } // while ends
            return targetValues;

        }

        public bool targetsFound(string targetspath)
        {
            bool found = false;
            try
            {
                found = System.IO.File.Exists(targetspath);
            }
            catch (Exception ioerror)
            {
                Console.Out.WriteLine(ioerror.ToString());
            } // cath ends
            return found;
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
        public ArrayList readConfiguration(string path)
        {
            string data = "";
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(path);
                data = reader.ReadToEnd();
                reader.Close();




                float[] weightsRow = new float[1];

                ArrayList netConfigMatrix = new ArrayList();

                string strctr = data.Substring(0, data.IndexOf(";", 0));
                ArrayList structure = new ArrayList();
                int commaindex = 0;
                string s = "";
                short[,] lf = new short[1, 2];
                while (true)
                {
                    commaindex = strctr.IndexOf(",", 0);
                    s = strctr.Substring(0, commaindex);
                    lf[0, 0] = Convert.ToInt16(s);
                    int nextCommaIndex;
                    nextCommaIndex = strctr.IndexOf(",", commaindex + 1);
                    s = strctr.Substring(commaindex + 1, 1);
                    lf[0, 1] = Convert.ToInt16(s);
                    structure.Add(lf);
                    lf = new short[1, 2];
                    strctr = strctr.Substring(nextCommaIndex + 1, strctr.Length - (nextCommaIndex + 1));
                    if (nextCommaIndex == -1)
                        break;
                }
                //s = strctr.Substring(0);
                //structure.Add(Convert.ToInt16(s));
                netConfigMatrix.Add(structure);
                data = data.Substring(data.IndexOf(";") + 1);
                // get weights
                strctr = data.Substring(0, data.IndexOf(";", 0));
                ArrayList sss = new ArrayList();
                commaindex = 0;
                s = "";
                while (strctr.IndexOf(",", 0) != -1)
                {
                    commaindex = strctr.IndexOf(",", 0);
                    s = strctr.Substring(0, commaindex);
                    sss.Add(Convert.ToDouble(s));
                    strctr = strctr.Substring(commaindex + 1, strctr.Length - (commaindex + 1));
                }
                s = strctr.Substring(0);
                sss.Add(Convert.ToDouble(s));
                netConfigMatrix.Add(sss);
                data = data.Substring(data.IndexOf(";") + 1);

                // get biases
                strctr = data.Substring(0, data.IndexOf(";", 0));
                ArrayList biases = new ArrayList();
                commaindex = 0;
                s = "";
                while (strctr.IndexOf(",", 0) != -1)
                {
                    commaindex = strctr.IndexOf(",", 0);
                    s = strctr.Substring(0, commaindex);
                    biases.Add(Convert.ToDouble(s));
                    strctr = strctr.Substring(commaindex + 1, strctr.Length - (commaindex + 1));
                }
                s = strctr.Substring(0);
                biases.Add(Convert.ToDouble(s));
                netConfigMatrix.Add(biases);

                data = data.Substring(data.IndexOf(";") + 1);
                // get preprocessing components
                strctr = data.Substring(0, data.IndexOf(";", 0));
                ArrayList normComps = new ArrayList();
                commaindex = 0;
                s = "";
                while (strctr.IndexOf(",", 0) != -1)
                {
                    commaindex = strctr.IndexOf(",", 0);
                    s = strctr.Substring(0, commaindex);
                    normComps.Add(Convert.ToDouble(s));
                    strctr = strctr.Substring(commaindex + 1, strctr.Length - (commaindex + 1));
                }
                s = strctr.Substring(0);
                normComps.Add(Convert.ToDouble(s));
                netConfigMatrix.Add(normComps);

                data = data.Substring(data.IndexOf(";") + 1);
                // get preprocessing components
                strctr = data.Substring(0, data.IndexOf(";", 0));
                ArrayList denormComps = new ArrayList();
                commaindex = 0;
                s = "";
                while (strctr.IndexOf(",", 0) != -1)
                {
                    commaindex = strctr.IndexOf(",", 0);
                    s = strctr.Substring(0, commaindex);
                    denormComps.Add(Convert.ToDouble(s));
                    strctr = strctr.Substring(commaindex + 1, strctr.Length - (commaindex + 1));
                }
                s = strctr.Substring(0);
                denormComps.Add(Convert.ToDouble(s));
                netConfigMatrix.Add(denormComps);

                return netConfigMatrix;
            }catch (Exception ioerror)
            {
                return null;
            }
        }
        public float[,] readInputs(string path, int inputLayerNeuronCount)
        {
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(path);
                data = reader.ReadToEnd();
                reader.Close();
                //    System.IO.File.Delete(path);
            }
            catch (Exception ioerror)
            {
                Console.Out.WriteLine(ioerror.ToString());
            } // cath ends
            int inputsSize = 0;
            int rowSize;
            string inputRow;
            int semiColonIndex = 0;
            ArrayList rows = new ArrayList();

            while (true)
            {

                rowSize = data.IndexOf(";", semiColonIndex);
                if (rowSize == -1)
                {
                    inputs = new float[inputsSize, inputLayerNeuronCount];
                    break;
                }
                inputRow = data.Substring(semiColonIndex, rowSize - semiColonIndex);
                rows.Add(inputRow);
                semiColonIndex = rowSize + 1;
                inputsSize++;
            } // while ends


            object[] obj = rows.ToArray();
            for (int rowCount = 0; rowCount < rows.Count; rowCount++)
            {
                inputRow = (string)obj[rowCount];
                int commaIndex = 0;
                int valSize;
                int count = 0;
                string inputVal;
                float[] iVal = new float[inputLayerNeuronCount];
                while (true)
                {
                    valSize = inputRow.IndexOf(",", commaIndex);
                    if (valSize == -1)
                    {
                        inputVal = inputRow.Substring(commaIndex, (inputRow.Length) - commaIndex);
                        iVal[count++] = System.Single.Parse(inputVal);
                        for (int i = 0; i < iVal.Length; i++)
                        {
                            inputs[rowCount, i] = iVal[i];
                        }

                        break;
                    }
                    inputVal = inputRow.Substring(commaIndex, valSize - commaIndex);
                    iVal[count++] = System.Single.Parse(inputVal);
                    commaIndex = valSize + 1;


                } // while ends

            } // for ends				
            return inputs;
        } // readInputs end
    } // class ends
}
