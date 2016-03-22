using UnityEngine;
using System.Collections.Generic;
using Ghost.Task;
using Ghost.Task.IO;

namespace Ghost.Sample
{
	public class SampleTaskTextFileStreamSync : SampleTaskTextFileStream 
	{
		public int syncPartMaxSize = 1024;

		#region override
		protected override TaskStream CreateTask ()
		{
			var task = Factory.Create<SyncStream>(driver);
			if (0 >= syncPartMaxSize)
			{
				syncPartMaxSize = 1;
			}
			task.partLength = syncPartMaxSize;
			return task;
		}
		#endregion override

		#region behaviour

		#endregion behaviour
	}
} // namespace Ghost.Sample