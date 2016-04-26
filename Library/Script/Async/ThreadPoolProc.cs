using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace Ghost.Async
{
	public class ThreadPoolProc
	{
		public bool running{get;private set;}
		public System.Exception exception{get;private set;}

		private System.Action proc = null;
		private Thread thread = null;

		public bool Start(System.Action p)
		{
			#if DEBUG
			Debug.Assert(null != p);
			#endif // DEBUG
			if (running)
			{
				return false;
			}
			running = true;

			proc = p;
			exception = null;
			thread = null;
			ThreadPool.QueueUserWorkItem(BkgProc, this);
			return true;
		}

		public void End()
		{
			lock(this)
			{
				if (!running)
				{
					return;
				}
				running = false;
				if (null != thread)
				{
					thread.Interrupt();
					thread = null;
				}
			}
		}

		#region background
		private void BkgRun()
		{
			lock(this)
			{
				if (!running)
				{
					return;
				}
				thread = Thread.CurrentThread;
			}
			// if canceled, CurrentThread must be interrupted.
			proc();
		}

		private void BkgEnd(System.Exception e = null)
		{
			lock(this)
			{
				// if canceled and run again, maybe thread is not CurrentThread but running is true
				if (Thread.CurrentThread == thread)
				{
					// 1.
					thread = null;
					if (null != e)
					{
						exception = e;
					}
					// 2.
					running = false;
				}
			}
		}

		private static void BkgProc(object param)
		{
			var self = param as ThreadPoolProc;
			try
			{
				self.BkgRun();
			}
			catch (System.Exception e)
			{
				self.BkgEnd(e);
			}
			finally
			{
				self.BkgEnd();
			}
		}
		#endregion background
	}
} // namespace Ghost.Async
