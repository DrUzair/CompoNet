using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace LVQ.NET
{
    public class LVQExe
    {
        NeuralReader neuralReader;
        LVQNet lvqNet;
        public LVQExe() {         
            neuralReader = new NeuralReader();
        }
        public OutputPattern feedInput(InputPattern input){                                    
            return lvqNet.execute(input);
        }

        
        public LVQNet getLVQ() {
            return lvqNet;
        }
        public void loadLVQNet(string path){
            if (neuralReader.netConfigFound(path)) {
                try
                {
                    WeightsMatrix wm = neuralReader.readConfiguration(path);
                    lvqNet = new LVQNet(wm);                    
                }
                catch (Exception e) {
                    throw e;
                }
                
            }
        
        }

    }
}
