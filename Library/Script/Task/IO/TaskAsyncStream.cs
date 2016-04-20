using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using Ghost.Extension;

namespace Ghost.Task.IO
{
	public class AsyncStream : TaskStream
	{
		private IAsyncResult asyncResult = null;

		private int accessLength = 0;
		private Func<byte[], int, int, AsyncCallback, object, IAsyncResult> accessBeginFunc = null;
		private Func<IAsyncResult, int> accessEndFunc = null;

		private void BeginAccess()
		{
			try
			{
				accessLength = runningTaskParam.length-result.completedLength;
				var ar = accessBeginFunc(
					runningTaskParam.buffer, 
					runningTaskParam.bufferOffset+result.completedLength,
					accessLength,
					AccessCallback,
					this);
				if (!ar.IsCompleted)
				{
					// set API token
					asyncResult = ar;
				}
			}
			catch (IOException e)
			{
				result.exception = e;
			}
		}

		private void EndAccess(IAsyncResult ar = null)
		{
			if (null == ar)
			{
				if (null == asyncResult)
				{
					// no API token
					return;
				}
				// use stored API token
				ar = asyncResult;
			}
			try
			{
				var oldProgress = result.completedLength/runningTaskParam.length;

				result.completedLength += accessEndFunc(ar);

				var newProgress = result.completedLength/runningTaskParam.length;
				OnProgressChanged(oldProgress, newProgress);

				if (ar.IsCompleted)
				{
					if(result.completedLength < runningTaskParam.length)
					{
						result.exception = new IOException("No more data could be read!");
					}
					else
					{
						// clear API token because completed
						asyncResult = null;
					}
				}
			}
			catch (IOException e)
			{
				result.exception = e;
			}
		}

		private IAsyncResult accessCallbackResult;
		private int accessCallbacked = 0;
		private static void AccessCallback(IAsyncResult ar)
		{
//			Debug.LogFormat("current thread({0}): {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name);
//			Debug.LogFormat("CompletedSynchronously={0}", ar.CompletedSynchronously);
			// use callback API token
			var self = ar.AsyncState as AsyncStream;
			if (null != self)
			{
				self.accessCallbackResult = ar;
				Interlocked.Exchange(ref self.accessCallbacked, 1);
			}
		}
		private void DoAccessCallback(IAsyncResult ar)
		{
			EndAccess(ar);
			if (null == result.exception && !ar.IsCompleted)
			{
				BeginAccess();
			}
		}

		#region override
		protected override bool DoUpdate (DriverUpdateParams param)
		{
			if (1 == Interlocked.CompareExchange(ref accessCallbacked, 0, 1))
			{
				DoAccessCallback(accessCallbackResult);
			}
			return null == result.exception 
				&& result.completedLength < runningTaskParam.length;
		}

		public override void EnterState (TaskState state)
		{
			base.EnterState (state);
			switch (state)
			{
			case TaskState.Pending:
				// clear API token
				asyncResult = null;
				break;
			case TaskState.Running:
				switch (runningTaskParam.access)
				{
				case Access.Read:
					accessBeginFunc = runningTaskParam.stream.BeginRead;
					accessEndFunc = runningTaskParam.stream.EndRead;
					break;
				case Access.Write:
					accessBeginFunc = runningTaskParam.stream.BeginWrite;
					accessEndFunc = delegate(IAsyncResult arg) {
						runningTaskParam.stream.EndWrite(arg);
						if (arg.IsCompleted)
						{
							return accessLength;
						}
						return 0;
					};
					break;
				}
				BeginAccess();
				break;
			case TaskState.Idle:
				EndAccess();
				break;
			}
		}
		#endregion override
	}
} // namespace Ghost.Task.IO
