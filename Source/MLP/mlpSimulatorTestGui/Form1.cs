using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Neurotic;
namespace NeuroticGUI
{
    public partial class GUI : Form
    {
        NeuralNetExe exe;
        int[] structure;
        int location = 60;
        int outputLocation = 60;
        int tabIndex = 1;
        ArrayList inputBoxes = new ArrayList();
        ArrayList outputBoxes = new ArrayList();
        public GUI()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            location = 60;
            outputLocation = 60;
            tabIndex = 1;
            for (int count = 0; count < inputBoxes.Count; count++) {
                TextBox tb = (TextBox)inputBoxes[count];
                this.Controls.Remove(tb);
            }
            for (int count = 0; count < outputBoxes.Count; count++)
            {
                TextBox tb = (TextBox)outputBoxes[count];
                this.Controls.Remove(tb);
            }
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Select Net configuration file";
            fdlg.InitialDirectory = @"D:\";
            fdlg.Filter = "Net Config Files (*.net)|*.net|Net Fonfig Files (*.net)|*.net";
            //fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
           
            string path="";
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                path = fdlg.FileName;
            }
            exe= new NeuralNetExe(fdlg.FileName);   
            try
            {
                exe.loadNeuralNet(path);
            }
            catch (ConfigFileException er)
            {
                MessageBox.Show("This is not a valid .net file, Please load a valid file." + er.getMessage());
                Dispose();
            }
            
            //exe.
            structure = exe.getNNStructure();
            int inputSize = structure[0];
            Label lbl = new Label();
            lbl.Text = "NEURAL NET INPUTS";
            lbl.Location = new System.Drawing.Point(40, 40);
            lbl.Size = new System.Drawing.Size(160, 20);
            this.Controls.Add(lbl);            
            for (int count = 0; count < inputSize; count++) {
                AddTextBox(count+1+"");    
            }
            button2.Enabled = true;
        }
      
        private void AddTextBox(string title){
            TextBox textBox1 = new System.Windows.Forms.TextBox();
            inputBoxes.Add(textBox1);
            textBox1.Text = "Input "+ title;
            textBox1.Location = new System.Drawing.Point(60, location);            
            textBox1.Name = "btnInput"+title;
            textBox1.Size = new System.Drawing.Size(50, 20);
            location += 30;
            textBox1.TabIndex = tabIndex++;
            this.Controls.Add(textBox1);
        }
        private void AddOutputTextBox(string title)
        {
            TextBox textBox1 = new System.Windows.Forms.TextBox();
            outputBoxes.Add(textBox1);
            textBox1.Text = title;            
            textBox1.Location = new System.Drawing.Point(200, outputLocation);
            textBox1.Name = "btnOutput" + title;
            textBox1.Size = new System.Drawing.Size(60, 20);
            outputLocation += 30;
            textBox1.TabIndex = tabIndex++;
            this.Controls.Add(textBox1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            location = 60;
            outputLocation = 60;
            //tabIndex = 1;
            for (int count = 0; count < outputBoxes.Count; count++)
            {
                TextBox tb = (TextBox)outputBoxes[count];
                this.Controls.Remove(tb);
            }
            
            int inputSize = structure[0];
            float[,] input = new float[1, inputSize];
            try
            {
                for (int count = 0; count < inputBoxes.Count; count++)
                {
                    TextBox tb = (TextBox)inputBoxes[count];
                    input[0, count] = (float)Convert.ToDecimal(tb.Text);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Input format is not correct..." + ex.Message);
                Dispose();
            }
            exe.feedInput(input);
            exe.runNeuralNet();
            float[] outputs = exe.getNNOutput(radioButton1.Checked);

            int outputSize = structure[structure.Length - 1];
            Label lbl = new Label();
            lbl.Text = " OUTPUTS";            
            lbl.Location = new System.Drawing.Point(200, 40);
            lbl.Size = new System.Drawing.Size(160, 20);
            this.Controls.Add(lbl);
            for (int count = 0; count < outputSize; count++)
            {
                AddOutputTextBox(outputs[count] + "");
            }
        }
        
    }
}