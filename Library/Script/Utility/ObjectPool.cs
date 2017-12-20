using UnityEngine;
using System;
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

		void Destroy();
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
			for (int i = 0;i < list.Count; ++i)
			{
				var obj = list[i];
				var dispose = obj as IDisposable;
				if (null != dispose)
				{
					dispose.Dispose();
				}
			}
			list.Clear();
		}
		public bool reused{get;set;}

		public void Destroy()
		{
			Destruct();
		}
		#endregion IReuseableObject
	}

	public class ObjectPool<T> : IDisposable where T:IReuseableObject, new()
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

			obj.Construct(args);
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

		public void Clear(int restCount = 0)
		{
			var destroyCount = pool.Count-restCount;
			for (int i = 0; i < destroyCount; ++i)
			{
				var obj = pool.Pop();
				obj.Destroy();
			}
		}

		#region IDisposable
		public void Dispose() 
		{
			Clear(0);
		}
		#endregion IDisposable

	}
} // namespace Ghost.Utility
