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

		#region override
		protected override bool DoUpdate (DriverUpdateParams param)
		{
			try
			{
				accessLength = Mathf.Min(partLength, runningTaskParam.length-result.completedLength);
				result.completedLength += accessFunc(
					runningTaskParam.buffer, 
					runningTaskParam.bufferOffset+result.completedLength, 
					accessLength);
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
					accessFunc = delegate(byte[] arg1, int arg2, int arg3) {
						runningTaskParam.stream.Write(arg1, arg2, arg3);
						return accessLength;
					};
					break;
				}
				break;
			}
		}
		#endregion override
	}
} // namespace Ghost.Task.IO
