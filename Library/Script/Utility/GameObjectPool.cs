using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Utility
{
	public class GameObjectPool : MonoBehaviourSingleton<GameObjectPool>
	{
		private Dictionary<string, Pool> map = new Dictionary<string, Pool> ();

		#region static
		private static T DoCreate<T>(string name, Transform parent = null) where T:Component
		{
			return new GameObject(name, typeof(T)).GetComponent<T>();
		}

		private static void DoDestroy(GameObject obj)
		{
			GameObject.Destroy(obj);
		}

		public static T Create<T>(string name, string poolName, Transform parent = null) where T:Component
		{
			if (null != GameObjectPool.Singleton)
			{
				return GameObjectPool.Singleton.Create_<T>(name, poolName, parent);
			}
			return DoCreate<T>(name, parent);
		}

		public static void Destroy (GameObject obj, string name, string poolName)
		{
			if (null != GameObjectPool.Singleton)
			{
				GameObjectPool.Singleton.Destroy_(obj, name, poolName);
			}
			else
			{
				DoDestroy(obj);
			}
		}

		public static void ClearPool (string poolName)
		{
			if (null != GameObjectPool.Singleton)
			{
				GameObjectPool.Singleton.ClearPool_(poolName);
			}
		}

		public static void ClearAll ()
		{
			if (null != GameObjectPool.Singleton)
			{
				GameObjectPool.Singleton.ClearAll_();
			}
		}
		#endregion static

		private T Create_<T>(string name, string poolName, Transform parent = null) where T:Component
		{
			Pool pool;
			if (map.TryGetValue(poolName, out pool))
			{
				var obj = pool.Get<T>(name, parent);
				if (null != obj)
				{
					return obj;
				}
			}
			return DoCreate<T>(name, parent);
		}

		private void Destroy_ (GameObject obj, string name, string poolName)
		{
			if (null == obj)
			{
				return;
			}
			Pool pool;
			if (!map.TryGetValue(poolName, out pool))
			{
				pool = new Pool(poolName, transform);
				map.Add(poolName, pool);
			}
			pool.Add(name, obj);
		}

		private void ClearPool_ (string poolName)
		{
			Pool pool;
			if (map.TryGetValue(poolName, out pool))
			{
				pool.Destroy();
				map.Remove(poolName);
			}
		}

		private void ClearAll_ ()
		{
			foreach (var key_value in map)
			{
				key_value.Value.Destroy();
			}
			map.Clear();
		}
	
	}

	class Pool
	{
		public string name;
		public GameObject root;
		private Dictionary<string, Stack<GameObject>> map = new Dictionary<string, Stack<GameObject>> ();

		public Pool (string n, Transform parent)
		{
			name = n;
			root = new GameObject (string.Format("Pool_{0}", name));
			root.transform.SetParent(parent);
		}

		public void Add (string name, GameObject obj)
		{
			if (null == root || null == obj)
			{
				return;
			}
			Stack<GameObject> objs;
			if (!map.TryGetValue(name, out objs))
			{
				objs = new Stack<GameObject>();
				map.Add(name, objs);
				objs.Push(obj);
			}
			else
			{
				if (!objs.Contains(obj))
				{
					objs.Push(obj);
				}
			}
			obj.transform.parent = root.transform;
		}

		public GameObject Get (string name, Transform parent = null)
		{
			if (null == root)
			{
				return null;
			}
			Stack<GameObject> objs;
			if (!map.TryGetValue(name, out objs))
			{
				return null;
			}
			if (0 >= objs.Count)
			{
				return null;
			}
			var obj = objs.Pop();
			if (null == obj)
			{
				return null;
			}
			obj.transform.SetParent(parent);

			return obj;
		}

		public T Get<T> (string name, Transform parent = null) where T:Component
		{
			var obj = Get (name, parent);
			if (null != obj)
			{
				return obj.GetComponent<T>();
			}
			return null;
		}

		public void Destroy ()
		{
			if (null != root)
			{
				GameObject.Destroy(root);
				root = null;
			}
			map.Clear();
		}
	}
} // namespace Ghost.Utility
