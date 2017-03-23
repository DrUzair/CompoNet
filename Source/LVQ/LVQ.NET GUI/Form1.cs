using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using LVQ.NET;
namespace CS_LVQ_GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float[] input = new float[4];
            input[0] = -100.0F;
            input[1] = -40.0F;
            input[2] = -90.0F;
            input[3] = -66.0F;

            InputPattern i = new InputPattern(input);
            LVQ.NET.LVQExe exe = new LVQExe();
            exe.loadLVQNet(@"D:\netConfig\LocationAwareSchedular\309\LVQ_4_8_3_LocationAwareSchedular.LVQ");
            OutputPattern o = exe.feedInput(i);
            MessageBox.Show((o.getOutputPattern()[0] + " " + o.getOutputPattern()[1] + " " + o.getOutputPattern()[2]));
        }

        private void getMaxOut(){
        double[] x = new double[5];
            x[0] = 2.1;
            x[1] = 1.6;
            x[2] = 2.7;
            x[3] = 1.9;
            x[4] = 1.1;
            double max = x[0];
            int maxIndex = 0;
            
                for (int j = x.Length-1; j > 0; j--)
                {
                    if (max < x[j])
                        maxIndex = j;
                    max = Math.Max(max, x[j]);
                }            
            MessageBox.Show(x[maxIndex] + "");
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

                string iw = data.Substring(1, data.IndexOf("]", 0) - 1);
                data = data.Substring(iw.Length + 2);
                double[,] IW = new double[Int32.Parse(structure[1].ToString()), Int32.Parse(structure[0].ToString())];
                spaceindex = 0;
                int semicolonindex = 0;
                int rowCount=0;
                int columnCount = 0;
                s = "";
                while (true)
                {
                    semicolonindex = iw.IndexOf(";");
                    if (semicolonindex == -1)
                        break;
                    string w = iw.Substring(0, semicolonindex).Trim();
                    iw = iw.Substring(semicolonindex+1);
                    for (int colCount = 0; columnCount < IW.GetLength(1); colCount++) {
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
                }
                string lw = data.Substring(1, data.IndexOf("]", 0) - 1);
                int[,] LW = new int[Int32.Parse(structure[2].ToString()), Int32.Parse(structure[1].ToString())];
                spaceindex = 0;
                semicolonindex = 0;
                rowCount = 0;
                columnCount = 0;
                s = "";
                while (true)
                {
                    semicolonindex = lw.IndexOf(";");
                    if (semicolonindex == -1)
                        break;
                    string w = lw.Substring(0, semicolonindex).Trim();
                    lw = lw.Substring(semicolonindex + 1);
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
                }
                return netConfigMatrix;
            }
            catch (Exception ioerror)
            {
                return null;
            }
        }
    }
}