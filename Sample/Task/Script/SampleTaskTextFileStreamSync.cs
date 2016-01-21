using UnityEngine;
using System.Collections.Generic;
using Ghost.Task;
using Ghost.Task.IO;

namespace Ghost.Sample
{
	public class SampleTaskTextFileStreamSync : SampleTaskTextFileStream 
	{
		public int readPartMaxSize = 1024;

		#region override
		protected override TaskReadStream CreateTask ()
		{
			var task = Factory.Create<SyncReadStream>(driver);
			if (0 >= readPartMaxSize)
			{
				readPartMaxSize = 1;
			}
			task.partLength = readPartMaxSize;
			return task;
		}
		#endregion override

		#region behaviour

		#endregion behaviour
	}
} // namespace Ghost.Sample
