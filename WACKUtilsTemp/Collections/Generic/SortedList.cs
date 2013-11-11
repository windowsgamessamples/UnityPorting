#if UNITY_METRO && NETFX_CORE && !UNITY_EDITOR
namespace System.Collections.Generic
{
    // See http://stackoverflow.com/questions/935621/whats-the-difference-between-sortedlist-and-sorteddictionary
    public class SortedList<TKey, TValue> : SortedDictionary<TKey, TValue> {
        public SortedList(IComparer<TKey> comparer) : base(comparer) {}
    }
}
#endif