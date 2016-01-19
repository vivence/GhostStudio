using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Task
{
	public class TaskDriverFixedTime : TaskDriver 
	{
		protected DriverUpdateParams updateParam = new DriverUpdateParams();

		#region behaviour
		void FixedUpdate()
		{
			updateParam.time = Time.fixedTime;
			updateParam.deltaTime = Time.fixedDeltaTime;
			UpdateDriver(updateParam);
		}
		#endregion behaviour
	}
} // namespace Ghost.Task
