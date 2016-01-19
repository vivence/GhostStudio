using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Task
{
	internal class DriverParallel : Driver
	{
		protected List<System.Predicate<DriverUpdateParams>> tasks = new List<System.Predicate<DriverUpdateParams>>();

		protected bool TaskUpdateAndRemovePredicate(System.Predicate<DriverUpdateParams> task)
		{
			return !task(updateParam);
		}

		#region override
		protected override void DoPostTask (System.Predicate<DriverUpdateParams> task)
		{
			#if DEBUG
			Debug.Assert(!tasks.Contains(task));
			#endif // DEBUG
			tasks.Add(task);
		}

		protected override void DoUpdate ()
		{
			if (0 >= tasks.Count)
			{
				return;
			}
			tasks.RemoveAll(TaskUpdateAndRemovePredicate);
		}
		#endregion override
	}
} // namespace Ghost.Task
