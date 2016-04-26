using UnityEngine;
using System.Collections.Generic;
using Ghost.Utility;
using Ghost.Extension;

namespace Ghost.Async
{
	public class Task : IReuseableObject
	{
		#region factory
		public static Task Create(System.Func<object, object> bkgProc, object param = null)
		{
			return ObjectPool<Task>.Singleton.Create(bkgProc, param);
		}
		public static void Destroy(Task task)
		{
			ObjectPool<Task>.Singleton.Destroy(task);
		}
		#endregion factory

		public object param{get;private set;}
		public object result{get;private set;}
		public System.Exception exception
		{
			get
			{
				return worker.exception;
			}
		}

		private System.Func<object, object> bkgProc = null;
		private ThreadPoolProc worker = new ThreadPoolProc();

		public void Post()
		{
			#if DEBUG
			Debug.Assert(null != bkgProc);
			#endif // DEBUG
			worker.Start(BkgProc);
		}

		public void Cancel()
		{
			worker.End();
		}

		#region background
		private void BkgProc()
		{
			result = bkgProc(param);
		}
		#endregion background

		#region IReuseable
		public void Construct(params object[] args)
		{
			#if DEBUG
			Debug.Assert(!args.IsNullOrEmpty());
			#endif // DEBUG

			bkgProc = args[0] as System.Func<object, object>;
			param = 1 < args.Length ? args[1] : null;
			result = null;

			#if DEBUG
			Debug.Assert(null != bkgProc);
			#endif // DEBUG
		}
		public void Destruct()
		{
			Cancel();
		}
		public bool reused{get;set;}
		#endregion IReuseable
	}
} // namespace Ghost.Async
