using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Ghost.Extension;
using Ghost.Utility;

namespace Ghost.Async
{
	public interface IProductOwner
	{
		void DestroyProduct(IDisposable p);
	}

	public class ProductBase<_Owner, _Opt> : IReuseableObject, IDisposable 
		where _Owner:IProductOwner
	{
		public _Owner owner;
		public _Opt opt;
		public object[] args;

		#region IReuseableObject
		public void Construct(params object[] a)
		{
			#if DEBUG
			Debug.Assert(null != a && 2 <= a.Length);
			#endif // DEBUG

			owner = (_Owner)a[0];
			opt = (_Opt)a[1];
			if (2 < a.Length)
			{
				args  = new object[a.Length-2];
				for (int i = 0; i < args.Length; ++i)
				{
					args[i] = a[i+2];
				}
			}
			else
			{
				args = null;
			}
		}
		public void Destruct()
		{
			if (!args.IsNullOrEmpty())
			{
				for (int i = 0; i < args.Length; ++i)
				{
					var obj = args[i];
					var dispose = obj as System.IDisposable;
					if (null != dispose)
					{
						dispose.Dispose();
					}
				}
				args = null;
			}
		}
		public bool reused{get;set;}

		public void Destroy()
		{
			Destruct();
		}
		#endregion IReuseableObject

		public void Dispose()
		{
			owner.DestroyProduct(this);
		}
	}

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
			var context = param as ConsumerContext;
			if (null == context)
			{
				return;
			}
			try
			{
				while (!context.closed)
				{
					var p = context.GetProductContainer();
					if (null != p)
					{
						for (int i = 0; i < p.list.Count; ++i)
						{
							context.bkgProc(p.list[i]);
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
			finally
			{
				context.Dispose();
			}
		}
		#endregion background

	}
} // namespace Ghost.Async
