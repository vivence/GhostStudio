using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ghost.Utility
{
	public class LRUCache<_K, _V> : ResourceHolder
	{
		public static int CapacityMin = 1;
		public static int DeterminCapacity(int capacity)
		{
			return Mathf.Max(CapacityMin, capacity);
		}

		private List<_K> usedTimeline;
		private Dictionary<_K, _V> cache;

		private int capacity_ = 0;
		public int capacity
		{
			get
			{
				return capacity_;
			}
			set
			{
				if (value == capacity)
				{
					return;
				}
				capacity_ = value;
				if (0 < capacity)
				{
					var removeCount = usedTimeline.Count - capacity;
					if (0 < removeCount)
					{
						for (int i = 0; i < removeCount; ++i)
						{
							RemoveFromCache(usedTimeline[i]);
						}
						usedTimeline.RemoveRange(0, removeCount);
					}
				}
				else
				{
					Clear();
				}
			}
		}

		public int cachedCount
		{
			get
			{
				return cache.Count;
			}
		}

		public LRUCache ()
		{
			cache = new Dictionary<_K, _V>();
			usedTimeline = new List<_K>();
			capacity_ = CapacityMin;
		}
		public LRUCache (int capacity)
		{
			capacity = DeterminCapacity(capacity);
			cache = new Dictionary<_K, _V>(capacity);
			usedTimeline = new List<_K>(capacity);
			capacity_ = capacity;
		}
		public LRUCache (IDictionary<_K, _V> dictionary)
		{
			cache = new Dictionary<_K, _V>(dictionary);
			usedTimeline = new List<_K>(dictionary.Keys);
			capacity_ = DeterminCapacity(dictionary.Count);
		}


		public LRUCache (IEqualityComparer<_K> comparer)
		{
			cache = new Dictionary<_K, _V>(comparer);
			usedTimeline = new List<_K>();
			capacity_ = CapacityMin;
		}
		public LRUCache (int capacity, IEqualityComparer<_K> comparer)
		{
			capacity = DeterminCapacity(capacity);
			cache = new Dictionary<_K, _V>(capacity, comparer);
			usedTimeline = new List<_K>(capacity);
			capacity_ = capacity;
		}
		public LRUCache (IDictionary<_K, _V> dictionary, IEqualityComparer<_K> comparer)
		{
			cache = new Dictionary<_K, _V>(dictionary, comparer);
			usedTimeline = new List<_K>(dictionary.Keys);
			capacity_ = DeterminCapacity(dictionary.Count);
		}

		public void Add (_K key, _V value)
		{
			cache.Add(key, value);
			usedTimeline.Add(key);
			if (cachedCount > capacity)
			{
				Remove(usedTimeline[0]);
			}
		}

		public bool Remove(_K key)
		{
			if (RemoveFromCache(key))
			{
				usedTimeline.Remove(key);
				return true;
			}
			return false;
		}

		public bool TryGetValue(_K key, out _V value)
		{
			if (cache.TryGetValue(key, out value))
			{
				usedTimeline.Remove(key);
				usedTimeline.Add(key);
				return true;
			}
			return false;
		}

		public bool ContainKey(_K key)
		{
			return cache.ContainsKey(key);
		}

		public bool ContainValue(_V value)
		{
			return cache.ContainsValue(value);
		}

		public void Clear()
		{
			ReleaseValues();
			cache.Clear();
			usedTimeline.Clear();
		}

		private bool RemoveFromCache(_K key)
		{
			_V value;
			if (cache.TryGetValue(key, out value))
			{
				ReleaseValue(value);
				cache.Remove(key);
				return true;
			}
			return false;
		}

		private void ReleaseValue(_V value)
		{
			var res = value as IDisposable;
			if (null != res)
			{
				res.Dispose();
			}
		}

		private void ReleaseValues()
		{
			foreach (var key_value in cache)
			{
				ReleaseValue(key_value.Value);
			}
		}

		#region override
		protected override void ReleaseManagedResource ()
		{
			base.ReleaseManagedResource();
			ReleaseValues();
		}
		#endregion override
	}
} // namespace Ghost.Utility
