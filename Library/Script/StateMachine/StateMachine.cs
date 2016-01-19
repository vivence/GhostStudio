using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ghost
{
	public interface IStateMachineTraits<_State>
	{
		bool AllowSwitchState(_State currentState, _State nextState);
		void ExitState(_State state);
		void EnterState(_State state);
	}

	public class StateMachine<_State, _Operation>
	{
		public IStateMachineTraits<_State> traits = null;

		private Dictionary<_State, Dictionary<_Operation, Action>> states = new Dictionary<_State, Dictionary<_Operation, Action>>();

		private Dictionary<_Operation, Action> currentOpts = null;
		private _State currentState_ = default(_State);
		private _State currentState
		{
			get
			{
				return currentState_;
			}
			set
			{
				currentState_ = value;
				currentOpts = GetOperations(value);
			}
		}

		public void Register(_State state, Dictionary<_Operation, Action> opts)
		{
			#if DEBUG
			Debug.Assert(null != opts && !states.ContainsKey(state));
			#endif // DEBUG
			states.Add(state, opts);
		}

		public void Unregister(_State state)
		{
			states.Remove(state);
		}

		public Dictionary<_Operation, Action> GetOperations(_State state)
		{
			Dictionary<_Operation, Action> s;
			if (!states.TryGetValue(state, out s))
			{
				return null;
			}
			return s;
		}

		public bool InvokeCurrentState(_Operation opt)
		{
			if (null == currentOpts)
			{
				return false;
			}
			Action action;
			if (!currentOpts.TryGetValue(opt, out action))
			{
				return false;
			}
			action();
			return true;
		}

		public bool TrySwitchState(_State nextState)
		{
			if (null != traits)
			{
				if (!!traits.AllowSwitchState(currentState, nextState))
				{
					return false;
				}
				traits.ExitState(currentState);
				traits.EnterState(nextState);
			}
			currentState = nextState;
			return true;
		}
	}
} // namespace Ghost
