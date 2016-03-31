using UnityEngine;
using System.Collections.Generic;
using Ghost.Task;
using Ghost.Task.IO;

namespace Ghost.Sample
{
	public class SampleTaskTextFileStreamAsync : SampleTaskTextFileStream 
	{
		#region override
		protected override bool fileAsync
		{
			get
			{
				return true;
			}
		}

		protected override TaskStream CreateTask ()
		{
			return Factory.Create<AsyncStream>(driver);
		}
		#endregion override

		#region behaviour

		#endregion behaviour
	}
} // namespace Ghost.Sample
