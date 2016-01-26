using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Ghost.Extension;

namespace Ghost.Task.IO
{
	public abstract class TaskStream : Entity
	{
		public enum Access
		{
			Read,
			Write
		}

		public class Param
		{
			public Access access = Access.Read;

			public Stream stream = null;
			public byte[] buffer = null;
			public int bufferOffset = 0;
			public int length = 0;

			public bool streamCanAccess
			{
				get
				{
					switch (access)
					{
					case Access.Read:
						return stream.CanRead;
					case Access.Write:
						return stream.CanWrite;
					}
					return false;
				}
			}

			public bool valid
			{
				get
				{
					return null != stream && streamCanAccess
						&& !buffer.IsNullOrEmpty()
						&& buffer.CheckIndex(bufferOffset)
						&& buffer.Length >= (bufferOffset+length);
				}
			}
		}
		public class Result
		{
			public int completedLength = 0;
			public IOException exception = null;

			public void Reset()
			{
				completedLength = 0;
				exception = null;
			}
		}

		public Param taskParam = null;

		public Param runningTaskParam{get; private set;}
		public Result result{get;private set;}

		public TaskStream()
		{
			result = new Result();
		}

		#region override
		public override bool AllowSwitchState (TaskState currentState, TaskState nextState)
		{
			switch (currentState)
			{
			case TaskState.Idle:
				if (TaskState.Pending != nextState)
				{
					return false;
				}
				if (null == taskParam || !taskParam.valid)
				{
					return false;
				}
				return true;
			}
			return base.AllowSwitchState(currentState, nextState);
		}

		public override void EnterState (TaskState state)
		{
			base.EnterState(state);
			switch (state)
			{
			case TaskState.Pending:
				runningTaskParam = taskParam;
				taskParam = null;
				result.Reset();
				break;
			}
		}
		#endregion override
	
	}
} // namespace Ghost.Task.IO
