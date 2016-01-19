using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ghost.Task
{
	public enum DriverMode
	{
		Serial,
		Parallel
	}

	public class DriverUpdateParams
	{
		public float time = 0f;
		public float deltaTime = 0f;
	}

	public abstract class TaskDriver : MonoBehaviour
	{
		public DriverMode mode = DriverMode.Parallel;

		private Driver driver = null;

		protected void UpdateDriver(DriverUpdateParams param)
		{
			driver.Update(param);
		}

		#region behaviour
		void Awake()
		{
			driver = Driver.Create(mode);
		}
		#endregion behaviour
	}

	internal abstract class Driver 
	{
		#region static
		public static Driver Create(DriverMode mode)
		{
			switch (mode)
			{
			case DriverMode.Serial:
				return new DriverSerial();
			case DriverMode.Parallel:
				return new DriverParallel();
			}
			throw new System.NotImplementedException (mode.ToString());
		}
		#endregion static

		protected DriverUpdateParams updateParam = null;

		public void PostTask(Predicate<DriverUpdateParams> task)
		{
			#if DEBUG
			Debug.Assert(null != task);
			#endif // DEBUG
			DoPostTask(task);
		}

		public void Update(DriverUpdateParams param)
		{
			#if DEBUG
			Debug.Assert(null != param);
			#endif // DEBUG
			updateParam = param;
			DoUpdate();
		}

		#region abstract
		protected abstract void DoPostTask(Predicate<DriverUpdateParams> task);
		protected abstract void DoUpdate();
		#endregion abstract
	}
} // namespace Ghost.Task
