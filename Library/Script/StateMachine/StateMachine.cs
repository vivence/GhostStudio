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

		void OnStateChanged(_State oldState, _State newState);
	}

	public class StateMachine<_State, _Operation>
	{
		public IStateMachineTraits<_State> traits = null;

		private Dictionary<_State, Dictionary<_Operation, Predicate<object>>> states = new Dictionary<_State, Dictionary<_Operation, Predicate<object>>>();

		private Dictionary<_Operation, Predicate<object>> currentOpts = null;
		private _State currentState_ = default(_State);
		public _State currentState
		{
			get
			{
				return currentState_;
			}
			private set
			{
				var oldState = currentState;
				currentState_ = value;
				currentOpts = GetOperations(value);
				if (null != traits)
				{
					traits.OnStateChanged(oldState, currentState);
				}
			}
		}

		public void Register(_State state, Dictionary<_Operation, Predicate<object>> opts)
		{
			#if DEBUG
			Debug.Assert(null != opts && !states.ContainsKey(state));
			#endif // DEBUG
			states.Add(state, opts);
		}

		public Dictionary<_Operation, Predicate<object>> Register(_State state)
		{
			#if DEBUG
			Debug.Assert(!states.ContainsKey(state));
			#endif // DEBUG
			var opts = new Dictionary<_Operation, Predicate<object>>();
			states.Add(state, opts);
			return opts;
		}

		public void Unregister(_State state)
		{
			states.Remove(state);
		}

		public Dictionary<_Operation, Predicate<object>> GetOperations(_State state)
		{
			Dictionary<_Operation, Predicate<object>> s;
			if (!states.TryGetValue(state, out s))
			{
				return null;
			}
			return s;
		}

		public bool InvokeCurrentState(_Operation opt, object param = null)
		{
			if (null == currentOpts)
			{
				return false;
			}
			Predicate<object> action;
			if (!currentOpts.TryGetValue(opt, out action))
			{
				return false;
			}
			return action(param);
		}

		public bool TrySwitchState(_State nextState)
		{
			if (null != traits)
			{
				if (!traits.AllowSwitchState(currentState, nextState))
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
