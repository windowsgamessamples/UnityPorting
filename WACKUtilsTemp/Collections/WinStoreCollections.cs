/**
 * Example code for porting existing Unity projects to Windows Store Apps and Windows Phone 8.
 * 
 * Author:  Jodon Karlik (Coding Jar Studios Inc.)
 * http://www.CodingJar.com/blog/
 */
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System;
using System.Runtime.InteropServices;

// UNITY_WINRT is defined for both Windows Store Apps and Windows Phone 8, which makes it the one we want to use!
#if UNITY_WINRT && !UNITY_EDITOR

// This is defined when compiling to Win8 from the Editor, but not in the .vcproj

#if !ENABLE_SERIALIZATION_BY_CODEGENERATION
/**
 * The first thing we need to do is define System.Serializable.  We need this for WP8 plug-ins but also for compiling Unity code outside
 * of Unity.  You can compile Unity code outside of Unity when doing a Windows 8 project, using my supplied .vcproj files (which in turn were
 * derived from a Metro-Project-Creator available to the Unity Windows Store Apps Beta List).
 */
namespace System
{
	[AttributeUsageAttribute( AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false )]
	[ComVisible( true )]
	public sealed class SerializableAttribute : Attribute
	{

	}

	[AttributeUsageAttribute( AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Field, Inherited = false )]
	[ComVisible( true )]
	public sealed class NonSerializedAttribute : Attribute
	{
	}
}
#endif // !ENABLE_SERIALIZATION_BY_CODEGENERATION

/**
 * An ArrayList is just a dynamic array of generic objects... very close to List<> which is supported in Metro
 */
public class ArrayList : List<object>
{
	public System.Array ToArray( System.Type elementType )
	{
		var array = System.Array.CreateInstance( elementType, Count );
		System.Array.Copy( ToArray(), array, Count );

		return array;
	}

	public ArrayList() {}
	public ArrayList( IEnumerable enumerable ) : base(enumerable.Cast<object>()) {}
}

/**
 * Helpers for missing functions of classes, turned into extensions instead...
 */
public static class MissingExtensions
{
	/**
	 * Helping Metro convert from System.Format(string,Object) to Windows App Store's System.Format(string,Object[]).
	 */
	public static string Format(string format, System.Object oneParam )
	{
		return System.String.Format( format, new System.Object[]{ oneParam } );
	}

	/**
	 * StringBuilder.AppendFormat(arg0,arg1,arg2) isn't implemented on WP8, so use this instead.
	 */
	public static StringBuilder AppendFormatEx( this StringBuilder sb, string format, System.Object arg0, System.Object arg1 = null, System.Object arg2 = null )
	{
		return sb.AppendFormat( format, new object[] { arg0, arg1, arg2 } );
	}
	
	/**
	 * Missing IsSubclassOf, this works well
	 */
	public static bool IsSubclassOf( this System.Type type, System.Type parent )
	{
		return parent.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
	}
	
	/**
	 * Just forward this call to the proper spot
	 */
	public static bool IsAssignableFrom( this System.Type type, System.Type other )
	{
		return type.GetTypeInfo().IsAssignableFrom(other.GetTypeInfo());
	}
}

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
	[System.Serializable]
	public struct HashtableEnumerator : IDictionaryEnumerator, IEnumerator<DictionaryEntry>
	{
		private Dictionary<object, object>.Enumerator	CurrentImpl;

		public HashtableEnumerator( Dictionary<object, object>.Enumerator wrap )
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
		public void Dispose ()
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
			get { return new DictionaryEntry( CurrentImpl.Current.Key, CurrentImpl.Current.Value ); }
		}

		/**
		 * Allow DictionaryEntry from 2.0
		 */
		DictionaryEntry IEnumerator<DictionaryEntry>.Current
		{
			get { return Entry;	}
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
	public Hashtable() : base() {}
	public Hashtable( int capacity ) : base(capacity) {}
	public Hashtable(IDictionary<object, object> dictionary) : base (dictionary) {}

	/**
	 * Do we contain this key?
	 */
	public bool Contains( object key )
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
			base.TryGetValue( key, out retValue );

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
	object IDictionary.this [object key]
	{
		get
		{
			object retValue;
			base.TryGetValue( key, out retValue );

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
		Dictionary<object,object>.Enumerator baseEnumerator = base.GetEnumerator();
		return new HashtableEnumerator( baseEnumerator );
	}

	/**
	 * Implement one of the base class GetEnumerators
	 */
	IDictionaryEnumerator IDictionary.GetEnumerator ()
	{
		Dictionary<object,object>.Enumerator baseEnumerator = base.GetEnumerator();
		return new HashtableEnumerator( baseEnumerator );
	}

	/** The one we actually use when doing a foreach on Hashtable */
	public new IDictionaryEnumerator GetEnumerator ()
	{
		return ((IDictionary)this).GetEnumerator();
	}
}
#endif