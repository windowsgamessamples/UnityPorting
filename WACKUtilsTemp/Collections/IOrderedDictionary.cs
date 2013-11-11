#if UNITY_METRO && NETFX_CORE && !UNITY_EDITOR
using System.Collections;

namespace System.Collections.Specialized {

	public interface IOrderedDictionary : IDictionary, ICollection, IEnumerable {
		
		Object this[int index] { get; set; }
		
		IDictionaryEnumerator GetEnumerator();
		void Insert(int index, Object key, Object value);
		void RemoveAt(int index);
	}
}
#endif