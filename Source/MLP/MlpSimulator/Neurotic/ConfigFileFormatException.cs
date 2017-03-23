using System;
using System.Collections.Generic;
using System.Text;

namespace Neurotic
{
    public class ConfigFileException : Exception
    {
        private string error;
        public ConfigFileException(string e) {
            error = e;
        }
        public string getMessage(){
            return error;
        }
    }
}
