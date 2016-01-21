using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Ghost.Extension;

namespace Ghost.Task.IO
{
	public class AsyncReadStream : TaskReadStream
	{
		private IAsyncResult asyncResult = null;

		private void BeginRead()
		{
			try
			{
				var ar = runningTaskParam.stream.BeginRead(
					runningTaskParam.buffer, 
					runningTaskParam.bufferOffset+result.readLength,
					runningTaskParam.length-result.readLength,
					ReadCallback,
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

		private void EndRead(IAsyncResult ar = null)
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
				result.readLength += runningTaskParam.stream.EndRead(ar);
				if (ar.IsCompleted)
				{
					if(result.readLength < runningTaskParam.length)
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

		private void ReadCallback(IAsyncResult ar)
		{
			EndRead(ar);
			if (null == result.exception && !ar.IsCompleted)
			{
				BeginRead();
			}
		}

		#region override
		protected override bool DoUpdate (DriverUpdateParams param)
		{
			return null == result.exception 
				&& result.readLength < runningTaskParam.length;
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
				BeginRead();
				break;
			case TaskState.Idle:
				EndRead();
				break;
			}
		}
		#endregion override
	}
} // namespace Ghost.Task.IO
