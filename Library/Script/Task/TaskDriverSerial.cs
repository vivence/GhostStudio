using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Task
{
	internal class DriverSerial : Driver
	{
		protected Queue<System.Predicate<DriverUpdateParams>> tasks = new Queue<System.Predicate<DriverUpdateParams>>();

		#region override
		protected override void DoPostTask (System.Predicate<DriverUpdateParams> task)
		{
			#if DEBUG
			Debug.Assert(!tasks.Contains(task));
			#endif // DEBUG
			tasks.Enqueue(task);
		}

		protected override void DoUpdate ()
		{
			if (0 >= tasks.Count)
			{
				return;
			}
			var task = tasks.Peek();
			if (!task(updateParam))
			{
				tasks.Dequeue();
			}
		}
		#endregion override
	}
} // namespace Ghost.Task
