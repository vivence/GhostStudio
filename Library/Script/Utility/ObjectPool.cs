using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ghost.Extension;

namespace Ghost.Utility
{
	// for ObjectPool 
	public interface IReuseableObject
	{
		void Construct(params object[] args);
		void Destruct();
		bool reused{get;set;}
	}

	public class ReuseableList<T> : IReuseableObject
	{
		public List<T> list{get;private set;}

		public ReuseableList()
		{
			list = new List<T>();
		}

		#region IReuseableObject
		public void Construct(params object[] args)
		{
		}
		public void Destruct()
		{
			list.Clear();
		}
		public bool reused{get;set;}
		#endregion IReuseableObject
	}

	public class ObjectPool<T> where T:IReuseableObject, new()
	{
		public static readonly ObjectPool<T> Singleton = new ObjectPool<T>();

		private Stack<T> pool = new Stack<T>();

		public T Create(params object[] args)
		{
			T obj;
			if (0 == pool.Count)
			{
				obj = new T();
			}
			else
			{
				obj = pool.Pop();
			}

			obj.Construct(obj);
			obj.reused = true;
			return obj;
		}

		public void Destroy(T obj)
		{
			if (obj.reused)
			{
				return;
			}
			pool.Push(obj);

			obj.Destruct();
			obj.reused = false;
		}

	}
} // namespace Ghost.Utility
