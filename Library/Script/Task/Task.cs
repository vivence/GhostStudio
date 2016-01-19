using UnityEngine;
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
		private Driver driver;
		private StateMachine<TaskState, TaskOperation> stateMachine = new StateMachine<TaskState, TaskOperation>();

		public TaskState currentState
		{
			get
			{
				return stateMachine.currentState;
			}
		}

		internal Entity(Driver d)
		{
			#if DEBUG
			Debug.Assert(null != d);
			#endif // DEBUG

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
		}

		public bool Operate(TaskOperation opt, object param = null)
		{
			return stateMachine.InvokeCurrentState(opt, param);
		}

		private bool Update(DriverUpdateParams param)
		{
			return Operate(TaskOperation.Update, param);
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
		#endregion virtual IStateMachineTraits
	}
} // namespace Ghost.Task
