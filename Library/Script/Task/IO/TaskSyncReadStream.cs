using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Ghost.Extension;

namespace Ghost.Task.IO
{
	public class SyncReadStream : TaskReadStream
	{
		#region override
		protected override bool DoUpdate (DriverUpdateParams param)
		{
			try
			{
				result.readLength += runningTaskParam.stream.Read(
					runningTaskParam.buffer, 
					runningTaskParam.bufferOffset+result.readLength, 
					runningTaskParam.length-result.readLength);
			}
			catch (IOException e)
			{
				result.exception = e;
				return false;
			}
			return result.readLength < runningTaskParam.length;
		}
		#endregion override
	}
} // namespace Ghost.Task.IO
