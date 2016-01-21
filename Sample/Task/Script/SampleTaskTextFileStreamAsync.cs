using UnityEngine;
using System.Collections.Generic;
using Ghost.Task;
using Ghost.Task.IO;

namespace Ghost.Sample
{
	public class SampleTaskTextFileStreamAsync : SampleTaskTextFileStream 
	{
		#region override
		protected override TaskReadStream CreateTask ()
		{
			return Factory.Create<AsyncReadStream>(driver);
		}
		#endregion override

		#region behaviour

		#endregion behaviour
	}
} // namespace Ghost.Sample
