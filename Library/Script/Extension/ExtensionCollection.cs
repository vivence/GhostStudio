using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Ghost.Extension
{
	public static class S_Collection
	{
		#region array
		public static string ToStringWithSeparator<T>(this T[] array, object separator)
		{
			if (array.IsNullOrEmpty ()) 
			{
				return string.Empty;
			}
			var builder = new StringBuilder ();
			foreach (var e in array) 
			{
				builder.Append (separator).Append (e);
			}
			return builder.ToString ();
		}

		public static bool IsNullOrEmpty<T>(this T[] array)
		{
			return null == array || 0 >= array.Length;
		}

		public static string DumpToString<T>(this T[] array)
		{
			if (array.IsNullOrEmpty())
			{
				return "[]";
			}
			return Utility.String.Join('[', array.ToStringWithSeparator(','), ']');
		}

		public static bool CheckIndex<T>(this T[] array, int index)
		{
			return 0 <= index && array.Length > index;
		}
		#endregion array

		#region ICollection
		public static bool IsNullOrEmpty(this ICollection collection)
		{
			return null == collection || 0 >= collection.Count;
		}
		public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
		{
			return null == collection || 0 >= collection.Count;
		}
		#endregion ICollection

		#region IEnumerable
		public static object[] ToArray(this IEnumerable enumerable)
		{
			if (null == enumerable)
			{
				return null;
			}
			var list = new List<object>();
			var enumerator = enumerable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				list.Add(enumerator.Current);
			}
			return list.ToArray();
		}
		public static T[] ToArray<T>(this IEnumerable<T> enumerable)
		{
			if (null == enumerable)
			{
				return null;
			}
			var list = new List<T>();
			var enumerator = enumerable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				list.Add(enumerator.Current);
			}
			return list.ToArray();
		}
		#endregion IEnumerable

		#region List
		private static List<T> ToUnique<T>(List<T> list, System.Action<List<T>> sortDelegate)
		{
			if (1 > list.Count) 
			{
				return list;
			}
			list.ToArray();
			sortDelegate(list);
			List<T> uniqueList = new List<T> ();
			foreach (T obj in list) 
			{
				if (0 >= uniqueList.Count || !object.Equals (uniqueList[uniqueList.Count-1], obj)) 
				{
					uniqueList.Add (obj);
				}
			}
			return uniqueList;
		}
		public static List<T> ToUnique<T>(this List<T> list)
		{
			return ToUnique(list, delegate(List<T> obj) {
				obj.Sort();
			});
		}
		public static List<T> ToUnique<T>(this List<T> list, IComparer<T> comparer)
		{
			return ToUnique(list, delegate(List<T> obj) {
				obj.Sort(comparer);
			});
		}
		public static List<T> ToUnique<T>(this List<T> list, Comparison<T> comparison)
		{
			return ToUnique(list, delegate(List<T> obj) {
				obj.Sort(comparison);
			});
		}

		private static void MakeUnique<T>(List<T> list, System.Func<List<T>, List<T>> toUniqueDelegate)
		{
			var uniqueList = toUniqueDelegate(list);
			if (null != uniqueList)
			{
				list.Clear();
				list.AddRange(uniqueList);
			} 
		}
		public static void MakeUnique<T>(this List<T> list)
		{
			MakeUnique(list, delegate(List<T> arg) {
				return arg.ToUnique();
			});
		}
		public static void MakeUnique<T>(this List<T> list, IComparer<T> comparer)
		{
			MakeUnique(list, delegate(List<T> arg) {
				return arg.ToUnique(comparer);
			});
		}
		public static void MakeUnique<T>(this List<T> list, Comparison<T> comparison)
		{
			MakeUnique(list, delegate(List<T> arg) {
				return arg.ToUnique(comparison);
			});
		}
		#endregion List
	}
} // namespace Ghost.Extension
