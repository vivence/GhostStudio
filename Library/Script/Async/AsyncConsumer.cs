using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Ghost.Utility;

namespace Ghost.Async
{
	public sealed class Consumer : ProducerConsumerBase
	{
		class ConsumerContext : Context
		{
			public System.Action<object> bkgProc{get;private set;}
			private ConditionVariable condition;

			public ConsumerContext(System.Action<object> bp)
				: base()
			{
				bkgProc = bp;
				condition = new ConditionVariable();
			}

			public void Wait()
			{
				condition.Wait();
			}

			public void Sign()
			{	
				condition.Sign();
			}

			public void SignAll()
			{	
				condition.SignAll();
			}

			#region override
			public override void Close()
			{
				base.Close();
				Sign();
			}
			#endregion override
		}
		private ConsumerContext context;

		public bool StartWork(System.Action<object> backgroundProc)
		{
			if (null == backgroundProc)
			{
				return false;
			}
			context = new ConsumerContext(backgroundProc);
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

		public bool PostProduct(object p)
		{
			if (!working)
			{
				return false;
			}
			context.PostProduct(p);
			context.Sign();
			return true;
		}

		#region background
		private static void BkgProc(object param)
		{
			try
			{
				var context = param as ConsumerContext;
				if (null == context)
				{
					return;
				}
				while (!context.closed)
				{
					var p = context.GetProductContainer();
					if (null != p)
					{
						for (int i = 0; i < p.products.Count; ++i)
						{
							context.bkgProc(p.products[i]);
						}
						context.ReuseProductContainer(p);
					}
					else
					{
						context.Wait();
					}
				}
			}
			catch (ThreadInterruptedException)
			{

			}
		}
		#endregion background

	}
} // namespace Ghost.Async
