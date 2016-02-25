using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ghost.Utility
{
	public class MinHeap<_T>
	{
		private IComparer<_T> comparer;
		private List<_T> array;

		public int capacity 
		{
			get
			{
				return array.Capacity;
			}
			set
			{
				array.Capacity = value;
			}
		}

		public int size
		{
			get
			{
				return array.Count;
			}
		}

		public bool empty
		{
			get
			{
				return 0 >= size;
			}
		}

		public MinHeap (IComparer<_T> cmp = null)
		{
			comparer = null != cmp ? cmp : Comparer<_T>.Default;
			array = new List<_T>();
		}

		public MinHeap (int c, IComparer<_T> cmp = null)
			: this(cmp)
		{
			array = new List<_T>(c);
		}

		public MinHeap(IEnumerable<_T> collection, IComparer<_T> cmp = null)
			: this(cmp)
		{
			array = new List<_T>(collection);
			Reset();
		}

		public void Push(_T v)
		{
			array.Add(v);
			ShiftUp(size);
		}

		public void Push(IEnumerable<_T> collection)
		{
			foreach (var v in collection)
			{
				Push(v);
			}
		}

		public _T Top()
		{
			if (empty)
			{
				return default(_T);
			}
			return array[0];
		}

		public _T Pop()
		{
			if (empty)
			{
				return default(_T);
			}
			var v = array[0];
			if (1 == size)
			{
				array.Clear();
				return v;
			}
			
			array[0] = array[size-1];
			array.RemoveAt(size-1);
			ShiftDown(1);
			return v;
		}

		private void Reset()
		{
			if (1 < size)
			{
				for (var n = size/2; n > 0; --n)
				{
					ShiftDown(n);
				}
			}
		}

		private void ShiftUp(int n)
		{
			#if DEBUG
			Debug.Assert(0 < size && 0 < n && n <= size);
			#endif // DEBUG

			if (1 == n)
			{
				// root node
				return;
			}

			var temp = array[n-1];
			var holeN = n;

			while (true)
			{
				var pn = holeN / 2; // parent number
				if (0 == pn)
				{
					break;
				}
				if (0 < comparer.Compare(array[pn-1], temp))
				{
					// shift up
					array[holeN-1] = array[pn-1];
					holeN = pn;
				}
				else
				{
					// found the correct number
					break;
				}
			}
			if (holeN != n)
			{
				array[holeN-1] = temp;
			}
		}
			
		void ShiftDown(int n)
		{
			#if DEBUG
			Debug.Assert(0 < size && 0 < n && n <= size);
			#endif // DEBUG

			if (size == n)
			{
				// last node
				return;
			}

			var temp = array[n-1];
			var holeN = n;

			while (true)
			{
				var parentN = holeN;
				var min = temp;

				// test left child
				var ln = 2 * parentN;
				if (size < ln)
				{
					break;
				}
				if (0 > comparer.Compare(array[ln-1], min))
				{
					// shift down
					holeN = ln;
					min = array[ln-1];
				}

				// test right child
				var rn = ln + 1;
				if (size >= rn)
				{
					if (0 > comparer.Compare(array[rn-1], min))
					{
						// shift down
						holeN = rn;
						min = array[rn-1];
					}
				}

				if (holeN == parentN)
				{
					// found the correct number
					break;
				}
				else
				{
					// min up
					array[parentN-1] = min;
		        }
		    }
		    if (holeN != n)
		    {
		        array[holeN-1] = temp;
		    }
		}
	}
} // namespace Ghost.Utility

