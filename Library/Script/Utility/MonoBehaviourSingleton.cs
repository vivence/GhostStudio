using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Utility
{
	public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
	{
		public static T Singleton{get;private set;}

		public virtual bool forceResetSingleton
		{
			get
			{
				return false;
			}
		}

		protected void RegisterMe()
		{
			if (this == Singleton)
			{
				return;
			}
			if (null != Singleton)
			{
				if (!forceResetSingleton)
				{
					GameObject.Destroy(gameObject);
					return;
				}
				GameObject.Destroy(Singleton.gameObject);
			}
			Singleton = this as T;
		}

		protected void UnregisterMe()
		{
			if (this != Singleton)
			{
				return;
			}
			Singleton = null;
		}

		#region behaviour
		protected virtual void Awake ()
		{
			RegisterMe();
		}

		protected virtual void OnDestroy ()
		{
			UnregisterMe();
		}
		#endregion behaviour
	}
} // namespace Ghost.Utility
