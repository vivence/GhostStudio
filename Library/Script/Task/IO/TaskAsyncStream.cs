using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
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
					null);
				if (!ar.IsCompleted)
				{
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
					return;
				}
				ar = asyncResult;
			}
			try
			{
				result.completedLength += accessEndFunc(ar);
				if (ar.IsCompleted)
				{
					if(result.completedLength < runningTaskParam.length)
					{
						result.exception = new IOException("No more data could be read!");
					}
					else
					{
						asyncResult = null;
					}
				}
			}
			catch (IOException e)
			{
				result.exception = e;
			}
		}

		private void AccessCallback(IAsyncResult ar)
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
			return null == result.exception 
				&& result.completedLength < runningTaskParam.length;
		}

		public override void EnterState (TaskState state)
		{
			base.EnterState (state);
			switch (state)
			{
			case TaskState.Pending:
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
