using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Ghost.Utility;

namespace Ghost.Async
{
	public abstract class ProducerConsumerBase
	{
		protected class ProductContainer : IReuseableObject
		{
			public List<object> products{get;private set;}

			public ProductContainer()
			{
				products = new List<object>();
			}

			#region IReuseableObject
			public void Construct(params object[] args)
			{

			}
			public void Destruct()
			{
				products.Clear();
			}
			public bool reused{get;set;}
			#endregion IReuseableObject
		}

		protected class Context
		{
			public bool closed{get; private set;}
			private ProductContainer productContainer;

			public Context()
			{
				closed = false;
				productContainer = ObjectPool<ProductContainer>.Singleton.Create();
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
				productContainer.products.Add(p);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public ProductContainer GetProductContainer()
			{
				if (0 >= productContainer.products.Count)
				{
					return null;
				}
				var ret = productContainer;
				productContainer = ObjectPool<ProductContainer>.Singleton.Create();
				return ret;
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public void ReuseProductContainer(ProductContainer p)
			{
				if (p != productContainer)
				{
					ObjectPool<ProductContainer>.Singleton.Destroy(p);
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
