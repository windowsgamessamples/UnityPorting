using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System;
using System.Runtime.InteropServices;

namespace LegacySystem.Collections.Specialized {

	public interface IOrderedDictionary : IDictionary, ICollection, IEnumerable {
		
		Object this[int index] { get; set; }
		
		IDictionaryEnumerator GetEnumerator();
		void Insert(int index, Object key, Object value);
		void RemoveAt(int index);
	}
}
