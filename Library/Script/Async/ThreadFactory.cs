using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace Ghost.Utility
{
	public static class ThreadFactory
	{
		public static Thread Create(ParameterizedThreadStart start, int maxStackSize)
		{
			var t = new Thread(start, maxStackSize);
			return t;
		}

		public static Thread Create(ParameterizedThreadStart start)
		{
			var t = new Thread(start);
			return t;
		}

		public static Thread Create(ThreadStart start, int maxStackSize)
		{
			var t = new Thread(start, maxStackSize);
			return t;
		}

		public static Thread Create(ThreadStart start)
		{
			var t = new Thread(start);
			return t;
		}

		public static void Destroy(Thread t)
		{
			
		}
	
	}
} // namespace Ghost.Utility
