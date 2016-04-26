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
			for (int i = 0; i < p.products.Count; ++i)
			{
				consumer(p.products[i]);
			}
			context.ReuseProductContainer(p);
			return true;
		}

		#region background
		private static void BkgProc(object param)
		{
			try
			{
				var context = param as ProductContext;
				if (null == context)
				{
					return;
				}
				while (!context.closed)
				{
					var p = context.bkgProc();
					if (null != p)
					{
						context.PostProduct(p);
					}
				}
			}
			catch (ThreadInterruptedException e)
			{
			}
		}
		#endregion background
	}
} // namespace Ghost.Async
