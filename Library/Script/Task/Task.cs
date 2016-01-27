using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ghost.Task
{
	public enum TaskState
	{
		None,
		Idle,
		Pending,
		Running
	}

	public enum TaskOperation
	{
		Start,
		End,
		Update,
	}

	public abstract class Entity : IStateMachineTraits<TaskState>
	{
		private Driver driver = null;
		private StateMachine<TaskState, TaskOperation> stateMachine = new StateMachine<TaskState, TaskOperation>();

		// task, oldState, newState
		public event Action<Entity, TaskState, TaskState> stateChangedListener = null;
		// task, oldProgress, newProgress
		public event Action<Entity, float, float> progressChangedListener = null;

		public TaskState currentState
		{
			get
			{
				return stateMachine.currentState;
			}
		}

		internal bool inited
		{
			get
			{
				return null != driver;
			}
		}

		internal bool Init(Driver d)
		{
			#if DEBUG
			Debug.Assert(null != d);
			#endif // DEBUG

			if (inited)
			{
				return driver == d;
			}

			driver = d;
			stateMachine.traits = this;

			#region idle
			var opts = stateMachine.Register(TaskState.Idle);
			opts[TaskOperation.Start] = delegate {
				if (!stateMachine.TrySwitchState(TaskState.Pending))
				{
					return false;
				}
				driver.PostTask(Update);
				return true;
			};
			#endregion idle

			#region pending
			opts = stateMachine.Register(TaskState.Pending);
			opts[TaskOperation.End] = delegate {
				return stateMachine.TrySwitchState(TaskState.Idle);
			};
			opts[TaskOperation.Update] = delegate(object param) {
				if (!stateMachine.TrySwitchState(TaskState.Running))
				{
					return false;
				}
				return DoUpdate(param as DriverUpdateParams);
			};
			#endregion pending

			#region running
			opts = stateMachine.Register(TaskState.Running);
			opts[TaskOperation.End] = delegate {
				return stateMachine.TrySwitchState(TaskState.Idle);
			};
			opts[TaskOperation.Update] = delegate(object param) {
				return DoUpdate(param as DriverUpdateParams);
			};
			#endregion running

			stateMachine.TrySwitchState(TaskState.Idle);
			return true;
		}

		public bool Operate(TaskOperation opt, object param = null)
		{
			return stateMachine.InvokeCurrentState(opt, param);
		}

		private bool Update(DriverUpdateParams param)
		{
			if (!Operate(TaskOperation.Update, param))
			{
				Operate(TaskOperation.End);
				return false;
			}
			return true;
		}

		#region abstract
		protected abstract bool DoUpdate(DriverUpdateParams param);
		#endregion abstract

		#region virtual IStateMachineTraits
		public virtual bool AllowSwitchState(TaskState currentState, TaskState nextState)
		{
			switch (currentState)
			{
			case TaskState.None:
				return TaskState.Idle == nextState;
			case TaskState.Idle:
				return TaskState.Pending == nextState;
			case TaskState.Pending:
				return TaskState.Running == nextState
					|| TaskState.Idle == nextState;
			case TaskState.Running:
				return TaskState.Idle == nextState;
			}
			return false;
		}
		public virtual void ExitState(TaskState state)
		{

		}
		public virtual void EnterState(TaskState state)
		{

		}
		public virtual void OnStateChanged(TaskState oldState, TaskState newState)
		{
			if (null != stateChangedListener)
			{
				stateChangedListener(this, oldState, newState);
			}
		}
		#endregion virtual IStateMachineTraits

		protected virtual void OnProgressChanged(float oldProgress, float newProgress)
		{
			if (null != progressChangedListener)
			{
				progressChangedListener(this, oldProgress, newProgress);
			}
		}
	}
} // namespace Ghost.Task
