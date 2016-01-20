using UnityEngine;
using System.Collections.Generic;
using Ghost.Task;

namespace Ghost.Sample
{
	public class SampleTask : MonoBehaviour 
	{
		public TaskDriver driver = null;
		public float duration = 1f;

		private TaskEntity task = null;

		public bool StartTask()
		{
			if (0 >= duration)
			{
				return false;
			}
			if (null == driver)
			{
				return false;
			}

			EndTask();

			task = Factory.Create<TaskEntity>(driver);
			var renderer = GetComponent<Renderer>();
			if (null != renderer)
			{
				task.material = renderer.material;
			}
			task.duration = duration;
			return task.Operate(TaskOperation.Start);
		}

		public bool EndTask()
		{
			if (null == task)
			{
				return false;
			}
			if (!task.Operate(TaskOperation.End))
			{
				return false;
			}
			task = null;
			return true;
		}

		#region behaviour
		#endregion behaviour
	}

	class TaskEntity : Entity
	{
		public Material material = null;

		public Color idleColor = Color.grey;
		public Color pendingColor = Color.white;
		public KeyValuePair<Color, Color> runningColor = new KeyValuePair<Color, Color>(Color.green, Color.red);
		public float duration = 1f;
		public float progress{get;private set;}

		#region override
		protected override bool DoUpdate (DriverUpdateParams param)
		{
			progress = Mathf.Clamp01(progress + param.deltaTime/duration);
			if (null != material)
			{
				material.color = Color.Lerp(runningColor.Key, runningColor.Value, progress);
			}
			return 1 > progress;
		}

		public override void EnterState (TaskState state)
		{
			switch (state)
			{
			case TaskState.Idle:
				if (null != material)
				{
					material.color = idleColor;
				}
				break;
			case TaskState.Pending:
				progress = 0f;
				if (null != material)
				{
					material.color = pendingColor;
				}
				break;
			case TaskState.Running:
				progress = 0f;
				if (null != material)
				{
					material.color = runningColor.Key;
				}
				break;
			}
		}
		#endregion override
	}
} // namespace Ghost.Sample
