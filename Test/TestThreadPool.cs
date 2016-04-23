using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Ghost.Test
{
	public class TestThreadPool : MonoBehaviour 
	{
		#region callback
		private static void TaskProc(object param)
		{
			Debug.LogFormat("current thread({0}): task={1}", Thread.CurrentThread.ManagedThreadId, System.Convert.ToInt32(param));
		}
		#endregion callback

		#region behavoir
		void Start()
		{
			Debug.LogFormat("current thread({0}): {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name);
			for (int i = 0; i < 10; ++i)
			{
				ThreadPool.QueueUserWorkItem(TaskProc, i);
			}
		}

		void Destroy()
		{

		}
		#endregion behavoir
	}
} // namespace Ghost.Test
