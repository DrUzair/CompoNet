using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace LVQ.NET
{
    public class NeuralReader
    {
        string data;
        float[,] inputs;


        public NeuralReader()
        {

        }     

        //	public float[] loadNeuralNetBiases(){

        //}
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
        public WeightsMatrix readConfiguration(string path)
        {
            WeightsMatrix wm;
            string data = "";
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(path);
                data = reader.ReadToEnd();
                reader.Close();

                float[] weightsRow = new float[1];

                ArrayList netConfigMatrix = new ArrayList();

                string strctr = data.Substring(1, data.IndexOf("]", 0) - 1);
                data = data.Substring(strctr.Length + 2);
                ArrayList structure = new ArrayList();
                int spaceindex = 0;
                string s = "";
                while (true)
                {
                    spaceindex = strctr.IndexOf(" ", 0);
                    if (spaceindex == -1)
                    {

                        structure.Add(Convert.ToInt16(strctr));
                        break;
                    }
                    s = strctr.Substring(0, spaceindex);
                    structure.Add(Convert.ToInt16(s));
                    strctr = strctr.Substring(spaceindex, strctr.Length - spaceindex);
                    strctr = strctr.Trim();
                }
                netConfigMatrix.Add(structure);

                string iw = data.Substring(1, data.IndexOf("]", 0) - 1);
                data = data.Substring(iw.Length + 2);
                double[,] IW = new double[Int32.Parse(structure[1].ToString()), Int32.Parse(structure[0].ToString())];
                spaceindex = 0;
                int semicolonindex = 0;
                int rowCount = 0;
                int columnCount = 0;
                s = "";
                int inputNeruronCount = int.Parse(structure[structure.Count - 2].ToString());
                int inputNeruronIndex = 0;
                
                while (inputNeruronIndex < inputNeruronCount)
                {
                    semicolonindex = iw.IndexOf(";");
                    string w = "";
                    if (semicolonindex == -1)
                    {
                        w = iw.Substring(0).Trim();
                    }
                    else
                    {
                        w = iw.Substring(0, semicolonindex).Trim();
                        iw = iw.Substring(semicolonindex + 1);
                    }
                    for (int colCount = 0; columnCount < IW.GetLength(1); colCount++)
                    {
                        spaceindex = w.IndexOf(" ", 0);
                        if (spaceindex == -1)
                        {
                            IW[rowCount, colCount] = double.Parse(w);
                            break;
                        }
                        s = w.Substring(0, spaceindex);
                        IW[rowCount, colCount] = double.Parse(s);
                        w = w.Substring(spaceindex, w.Length - spaceindex).Trim();
                    }
                    rowCount++;
                    inputNeruronIndex++;
                }
                
                string lw = data.Substring(1, data.IndexOf("]", 0) - 1);
                int[,] LW = new int[Int32.Parse(structure[2].ToString()), Int32.Parse(structure[1].ToString())];
                spaceindex = 0;
                semicolonindex = 0;
                rowCount = 0;
                columnCount = 0;
                s = "";
                int outputNeruronCount = int.Parse(structure[structure.Count - 1].ToString());
                int outputNeruronIndex = 0;
                while (outputNeruronIndex < outputNeruronCount)
                {
                    semicolonindex = lw.IndexOf(";");
                    string w = "";
                    if (semicolonindex == -1)
                    {
                        w = lw.Substring(0).Trim();                    
                    }
                    else
                    {
                        w = lw.Substring(0, semicolonindex).Trim();                    
                        lw = lw.Substring(semicolonindex + 1);
                    }
                    for (int colCount = 0; columnCount < LW.GetLength(1); colCount++)
                    {
                        spaceindex = w.IndexOf(" ", 0);
                        if (spaceindex == -1)
                        {
                            LW[rowCount, colCount] = Int32.Parse(w);
                            break;
                        }
                        s = w.Substring(0, spaceindex);
                        LW[rowCount, colCount] = Int32.Parse(s);
                        w = w.Substring(spaceindex, w.Length - spaceindex).Trim();
                    }
                    rowCount++;
                    outputNeruronIndex++;
                }
                wm = new WeightsMatrix(IW, LW);
                return wm;
            }
            catch (Exception ioerror)
            {
                throw ioerror;    
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
