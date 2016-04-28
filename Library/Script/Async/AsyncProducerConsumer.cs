using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Ghost.Utility;

namespace Ghost.Async
{
	public abstract class ProducerConsumerBase
	{
		protected class Context
		{
			public bool closed{get; private set;}

			private ReuseableList<object> productContainer;
			private ObjectPool<ReuseableList<object>> containerPool;

			public Context()
			{
				closed = false;
				containerPool = new ObjectPool<ReuseableList<object>>();
				productContainer = containerPool.Create();
			}

			#region virtual
			public virtual void Close()
			{
				closed = true;
			}
			#endregion virtual

			[MethodImpl(MethodImplOptions.Synchronized)]
			public void PostProduct(object p)
			{
				productContainer.list.Add(p);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public ReuseableList<object> GetProductContainer()
			{
				if (0 >= productContainer.list.Count)
				{
					return null;
				}
				var ret = productContainer;
				productContainer = containerPool.Create();
				return ret;
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public void ReuseProductContainer(ReuseableList<object> p)
			{
				if (p != productContainer)
				{
					containerPool.Destroy(p);
				}
			}
		}
	
		private Thread thread;

		public bool working
		{
			get
			{
				return null != thread;
			}
		}

		public bool StartWork(ParameterizedThreadStart proc, object param)
		{
			if (working)
			{
				return false;
			}
			DoStart();
			thread = ThreadFactory.Create(proc);
			thread.Start(param);
			return true;
		}

		public void EndWork()
		{
			if (!working)
			{
				return;
			}
			DoEnd();
			thread.Interrupt();
			ThreadFactory.Destroy(thread);
			thread = null;
		}

		#region virtual
		protected virtual void DoStart()
		{

		}

		protected virtual void DoEnd()
		{
			
		}
		#endregion virtual
	}
} // namespace Ghost.Async
