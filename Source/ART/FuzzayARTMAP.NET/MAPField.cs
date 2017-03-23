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
    public class MAPField
    {       
        public ArrayList categories = new ArrayList();
                       
        public int getCategoryCount(){
            return categories.Count;
        }
       // public MapFieldCategory getCategory(int index) {
       //     return (MapFieldCategory)categories[index];
       // }
        public MapFieldCategory getCategory(int catCode)
        {
            MapFieldCategory cate = null;
            foreach ( MapFieldCategory cat in categories)
            {
                if (catCode == cat.getCode())
                {
                    cate = cat;
                }
            }
            return cate;
        }
        public MapFieldCategory getCategory(int catIndex,string s)
        {                     
            return (MapFieldCategory)categories[catIndex];
        }
        public int getCategoryCode(string name)
        {
            int code = -1;
            foreach (MapFieldCategory cat in categories)
            {         
                if (name == cat.getName())
                {
                    code = cat.getCode();
                }
            }
            return code;
        }
        public MAPField() { }
        public void setCategory(double categoryCode) {
            for (int i = 0; i < ((MapFieldCategory)categories[i]).getConnections().Count; i++)
            {
                double d = ((MapFieldCategory)categories[i]).getConnection(i).getWeight() * categoryCode;
            }
        }
        public int getConnectionWeight(int j, int categoryCode) {
            int categoryWeight = 0;
            for (int i = 0; i < categories.Count; i++)
            {
                if (((MapFieldCategory)categories[i]).getCode() == categoryCode)
                {
                    categoryWeight = (int)((MapFieldCategory)categories[i]).getConnection(j).getWeight();                    
                    break;
                }
            }
            return categoryWeight;
        }

        public bool categoryExists(int CategoryCode) {
            bool exists = false;
            for (int i = 0; i < categories.Count; i++) {
                if (((MapFieldCategory)categories[i]).getCode() == CategoryCode)
                {
                    exists = true;
                    break;
                }
            }
            return exists;
        }
        public void addNewCategory(string categoryName, int code) /*This method is to be  called by FuzzyArtMap(string path) method*/{
            MapFieldCategory categoryNode = new MapFieldCategory(categoryName, code);
            categories.Add(categoryNode);
        }
        public void addNewCategory(F2Neuron f2Neuron, string categoryName, int code)
        {
            MapFieldCategory categoryNode = new MapFieldCategory(categoryName, code);
            categoryNode.addConnection(f2Neuron);            
            categories.Add(categoryNode);
        }
        public void updateCategories(ConSelFAM.NET.F2Neuron f2Neuron)
        {
            for (int i = 0; i < categories.Count; i++) {                 
                ((MapFieldCategory)categories[i]).addConnection(f2Neuron);                                
            }
        }
        public void updateCategories(int exceptThisCategory, ConSelFAM.NET.F2Neuron f2Neuron)
        {
            for (int i = 0; i < categories.Count; i++)
            {
                if(((MapFieldCategory)categories[i]).getCode() != exceptThisCategory)
                    ((MapFieldCategory)categories[i]).addConnection(f2Neuron);
            }
        }
        public void updateCategoryWeights(int []classCode) { }
        public void associate(int k, int j) {
            for (int i = 0; i < categories.Count; i++)
            {
                if (((MapFieldCategory)categories[i]).getCode() == k)
                {
                    ((MapFieldCategory)categories[i]).getConnection(j).setWeight(1.0);
                }
            }
        }
        public void associate(int categoryCode, ConSelFAM.NET.F2Neuron f2Neuron)
        {
            ArrayList mapFieldConnections = f2Neuron.getMapFieldConnections();
            IEnumerator mapFieldConnEnum = mapFieldConnections.GetEnumerator();
            while (mapFieldConnEnum.MoveNext()) {
                MapFieldConnection conn = (MapFieldConnection)mapFieldConnEnum.Current;
                if (conn.getCategory().getCode() == categoryCode) {
                    conn.setWeight(1.0);
                }
            }          
        }
        public int getAssociatedCategory(F2Neuron f2Neuron) {
            int code = -1;
            if (f2Neuron.getMapFieldConnection() != null) {
                code = f2Neuron.getMapFieldConnection().getCategory().getCode();
            }
            return code;
        }        
    }
}
