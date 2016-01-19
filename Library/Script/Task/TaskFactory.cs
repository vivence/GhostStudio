using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Task
{
	public static class Factory
	{
		public static T Create<T>(TaskDriver taskDriver) where T : Entity,new()
		{
			var task = new T();
			task.Init(taskDriver.driver);
			return task;
		}
	}
} // namespace Ghost.Task
