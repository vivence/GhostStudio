using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Ghost.Utility;

namespace Ghost.Async
{
	public sealed class Producer : ProducerConsumerBase
	{
		class ProductContext : Context
		{
			public System.Func<object> bkgProc{get;private set;}

			public ProductContext(System.Func<object> bp)
				: base()
			{
				bkgProc = bp;
			}
		}

		private ProductContext context;

		public bool StartWork(System.Func<object> backgroundProc)
		{
			if (null == backgroundProc)
			{
				return false;
			}
			context = new ProductContext(backgroundProc);
			if (!base.StartWork(BkgProc, context))
			{
				context = null;
				return false;
			}
			return true;
		}

		#region override
		protected override void DoEnd ()
		{
			base.DoEnd ();
			ClearProducts();
			context.Close();
			context = null;
		}
		#endregion override

		public bool ConsumeProducts(System.Action<object> consumer)
		{
			if (!working)
			{
				return false;
			}
			var p = context.GetProductContainer();
			if (null == p)
			{
				return false;
			}
			for (int i = 0; i < p.list.Count; ++i)
			{
				consumer(p.list[i]);
			}
			context.ReuseProductContainer(p);
			return true;
		}

		public void ClearProducts()
		{
			var p = context.GetProductContainer();
			if (null != p)
			{
				context.ReuseProductContainer(p);
			}
		}

		#region background
		private static void BkgProc(object param)
		{
			var context = param as ProductContext;
			if (null == context)
			{
				return;
			}
			try
			{
				while (!context.closed)
				{
					var p = context.bkgProc();
					if (null != p)
					{
						context.PostProduct(p);
					}
				}
			}
			catch (ThreadInterruptedException)
			{
			}
			finally
			{
				context.Dispose();
			}
		}
		#endregion background
	}
} // namespace Ghost.Async
