

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
     * Implement Hashtable like it exists in .NET 2.0... or at least the part I'm using
     * 
     * This will trick the compiler into compiling any existing plug-ins that use System.Collections.Hashtable
     * as long as the plug-in is not a DLL.  This is another reason to prefer plug-ins that give you source code
     * over plug-ins that give you DLLs.
     *
     * The reason this code looks so complex is I'm [trying to deal] with the enumerators properly (foreach loops over a Hashtable object).
     */
    public class Hashtable : System.Collections.Generic.Dictionary<object, object>, IEnumerable, IDictionary
    {
        /**
         * This is an advanced example of writing an enumerator to be compatible with .NET 2.0.
         * I'm mostly doing this to show it CAN be done, rather than encouraging it -- it's probably easier
         * to rewrite the code that uses the enumerator
         */
#if !NETFX_CORE
        [System.Serializable]
#endif
        public struct HashtableEnumerator : IDictionaryEnumerator, IEnumerator<DictionaryEntry>
        {
            private Dictionary<object, object>.Enumerator CurrentImpl;

            public HashtableEnumerator(Dictionary<object, object>.Enumerator wrap)
            {
                CurrentImpl = wrap;
            }

            /** Current from IDictionaryEnumerator */
            public object Current
            {
                get { return Entry; }
            }

            /** Reset from IDictionaryEnumerator */
            public void Reset()
            {
                CurrentImpl.Dispose();
            }

            /** MoveNext from IDictionaryEnumerator */
            public bool MoveNext()
            {
                return CurrentImpl.MoveNext();
            }

            /** IDisposable from IEnumerator<T> */
            public void Dispose()
            {
                CurrentImpl.Dispose();
            }

            /**
             * 2.0 uses Enumerator.Key.  Windows Store Apps uses Enumerator.Current.Key... so convert that
             */
            public object Key
            {
                get { return CurrentImpl.Current.Key; }
            }

            /**
             * 2.0 uses Enumerator.Value.  Windows Store Apps uses Enumerator.Current.Value... so convert that
             */
            public object Value
            {
                get { return CurrentImpl.Current.Value; }
            }

            /**
             * This is a dictionary entry for 2.0
             */
            public DictionaryEntry Entry
            {
                get { return new DictionaryEntry(CurrentImpl.Current.Key, CurrentImpl.Current.Value); }
            }

            /**
             * Allow DictionaryEntry from 2.0
             */
            DictionaryEntry IEnumerator<DictionaryEntry>.Current
            {
                get { return Entry; }
            }

            /**
             * Implicit conversion operator for enumerating old types
             */
            public static implicit operator DictionaryEntry(HashtableEnumerator he)
            {
                return he.Entry;
            }
        }

        /** Constructors */
        public Hashtable() : base() { }
        public Hashtable(int capacity) : base(capacity) { }
        public Hashtable(IDictionary<object, object> dictionary) : base(dictionary) { }

        /**
         * Do we contain this key?
         */
        public bool Contains(object key)
        {
            return ContainsKey(key);
        }

        /**
         * Return a copy of ourselves
         */
        public object Clone()
        {
            return new Hashtable(this);
        }

        /**
         * Hashtable expects an index operation to allow a key to not be found...
         */
        public new object this[object key]
        {
            get
            {
                object retValue = null;
                base.TryGetValue(key, out retValue);

                return retValue;
            }
            set
            {
                base[key] = value;
            }
        }

        /**
         * This helps with conversions, or at least with the error messages
         */
        object IDictionary.this[object key]
        {
            get
            {
                object retValue;
                base.TryGetValue(key, out retValue);

                return retValue;
            }

            set
            {
                base[key] = value;
            }
        }

        /**
         * Implement one of the base class GetEnumerators
         */
        IEnumerator IEnumerable.GetEnumerator()
        {
            Dictionary<object, object>.Enumerator baseEnumerator = base.GetEnumerator();
            return new HashtableEnumerator(baseEnumerator);
        }

        /**
         * Implement one of the base class GetEnumerators
         */
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            Dictionary<object, object>.Enumerator baseEnumerator = base.GetEnumerator();
            return new HashtableEnumerator(baseEnumerator);
        }

        /** The one we actually use when doing a foreach on Hashtable */
        public new IDictionaryEnumerator GetEnumerator()
        {
            return ((IDictionary)this).GetEnumerator();
        }
    }
}