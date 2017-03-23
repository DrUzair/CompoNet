/* 190706
 * Uzair Ahmad
 * OSLAB, Kyung Hee University
 * South Korea 
 */

using System;
using System.Collections;
using System.Text;

namespace ConSelFAM.NET
{
    public class MapFieldCategory
    {
        string name;
        int code;        
        ArrayList connections = new ArrayList();
        public MapFieldCategory(string name,int code){
            this.name = name;
            this.code = code;
        }
        public int getCode() { return code; }
        public void addConnection(ConSelFAM.NET.F2Neuron f2Neuron)
        {            
            connections.Add(new MapFieldConnection(this, f2Neuron, 1)); 
        }
        public void addConnection(MapFieldConnection conn)
        {
            connections.Add(conn);         
        }
        public ArrayList getConnections() { return connections; }
        public MapFieldConnection getConnection(int i) { return (MapFieldConnection)connections[i]; }
        public string getName(){return name;}
        public void setName(string n) { name = n; }        
    }
}
