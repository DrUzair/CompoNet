using System;
using System.Collections;
using System.Text;

namespace ConSelFAM.NET
{
    public class ContextField : Hashtable
    {
        override public bool ContainsKey(Object key) {
            bool contains = false;
            if (base.Count == 0) { contains = false; }
            else
            {
                ICollection keyCollection = base.Keys;
                IEnumerator keyEnumerator = keyCollection.GetEnumerator();

                while (keyEnumerator.MoveNext())
                {                    
                    int previousKey = (int)keyEnumerator.Current;
                    int thisKey = (int)key;
                    if (previousKey == thisKey)
                    {
                        contains = true;
                        break;
                    }
                }
            }
            return contains;
        }
    }
}
