using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Ghost.Extension;

namespace Ghost.Task.IO
{
	public class SyncStream : TaskStream
	{
		public int partLength = 1024;

		private int accessLength = 0;
		private Func<byte[], int, int, int> accessFunc = null;

		private int WriteAccessFunc(byte[] buffer, int offset, int count)
		{
			runningTaskParam.stream.Write(buffer, offset, count);
			return accessLength;
		}

		#region override
		protected override bool DoUpdate (DriverUpdateParams param)
		{
			try
			{
				accessLength = Mathf.Min(partLength, runningTaskParam.length-result.completedLength);
				var oldProgress = result.completedLength/runningTaskParam.length;
				result.completedLength += accessFunc(
					runningTaskParam.buffer, 
					runningTaskParam.bufferOffset+result.completedLength, 
					accessLength);
				var newProgress = result.completedLength/runningTaskParam.length;
				OnProgressChanged(oldProgress, newProgress);
			}
			catch (IOException e)
			{
				result.exception = e;
				return false;
			}
			return result.completedLength < runningTaskParam.length;
		}

		public override void EnterState (TaskState state)
		{
			base.EnterState (state);
			switch (state)
			{
			case TaskState.Running:
				switch (runningTaskParam.access)
				{
				case Access.Read:
					accessFunc = runningTaskParam.stream.Read;
					break;
				case Access.Write:
					accessFunc = WriteAccessFunc;
					break;
				}
				break;
			}
		}
		#endregion override
	}
} // namespace Ghost.Task.IO
