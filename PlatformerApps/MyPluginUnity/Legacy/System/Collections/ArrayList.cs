
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System;
using System.Runtime.InteropServices;

namespace LegacySystem.Collections
{
    /**
     * An ArrayList is just a dynamic array of generic objects... very close to List<> which is supported in Metro
     */
    public class ArrayList : List<object>
    {
        public System.Array ToArray(System.Type elementType)
        {
            var array = System.Array.CreateInstance(elementType, Count);
            System.Array.Copy(ToArray(), array, Count);

            return array;
        }

        public ArrayList() { }
        public ArrayList(IEnumerable enumerable) : base(enumerable.Cast<object>()) { }
    }
}


