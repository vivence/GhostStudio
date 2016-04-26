using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace Ghost.Utility
{
	public class ConditionVariable
	{
		private readonly object locker = new object();
		private bool condition = false;

		public void Wait()
		{
			Monitor.Enter(locker);
			while (!condition)
			{
				Monitor.Wait(locker);
			}
			Monitor.Exit(locker);
		}
		public void Sign()
		{
			Monitor.Enter(locker);
			condition = true;
			Monitor.Pulse(locker);
			Monitor.Exit(locker);
		}
		public void SignAll()
		{
			Monitor.Enter(locker);
			condition = true;
			Monitor.PulseAll(locker);
			Monitor.Exit(locker);
		}
	}
} // namespace Ghost.Utility
