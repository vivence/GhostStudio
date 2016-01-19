using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Task
{
	public class TaskDriverScaledTime : TaskDriver 
	{
		protected DriverUpdateParams updateParam = new DriverUpdateParams();

		#region behaviour
		void Update()
		{
			updateParam.time = Time.time;
			updateParam.deltaTime = Time.deltaTime;
			UpdateDriver(updateParam);
		}
		#endregion behaviour
	}
} // namespace Ghost.Task
