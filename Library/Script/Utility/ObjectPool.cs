using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Utility
{
	// for ObjectPool 
	public interface IReuseableObject
	{
		void Construct(params object[] args);
		void Destruct();
		bool reused{get;set;}
	}

	public class ObjectPool<T> where T:IReuseableObject, new()
	{
		public static readonly ObjectPool<T> Singleton = new ObjectPool<T>();

		private Stack<T> pool = new Stack<T>();
	
		private ObjectPool()
		{
		}

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
