using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using Ghost.Extension;

namespace Ghost
{
	public abstract class TCPSessionMulti : MonoBehaviour 
	{
		private Async.Producer asyncReceive = new Async.Producer();
		private Async.Consumer asyncOperate = new Async.Consumer();

		public TCPSessionInfo.Setting setting;

		private Dictionary<int, TCPSessionInfo> sessions = new Dictionary<int, TCPSessionInfo>();

		public bool AddTCPClient(int ID, TcpClient client)
		{
			#if DEBUG
			Debug.Assert(null != client);
			#endif // DEBUG
			if (sessions.ContainsKey(ID))
			{
				return false;
			}

			var session = new TCPSessionInfo(client, setting);
			sessions[ID] = session;

			session.DoBkgSend = DoBkgSend;
			session.DoBkgReceive = DoBkgReceive;
			return true;
		}

		public bool Disconnect(int ID)
		{
			TCPSessionInfo session;
			if (!sessions.TryGetValue(ID, out session))
			{
				return false;
			}
			return session.Disconnect(asyncOperate);
		}

		public bool Send(int ID, params object[] args)
		{
			TCPSessionInfo session;
			if (!sessions.TryGetValue(ID, out session))
			{
				return false;
			}
			return session.Send(asyncOperate, args);
		}

		#region background
		// TODO
		private Dictionary<int, TCPSessionInfo> bkgSessions = new Dictionary<int, TCPSessionInfo>();

		#region abstract
		protected abstract void DoBkgSend(Socket tcp, object[] args);
		protected abstract object DoBkgReceive(Socket tcp);
		#endregion abstract

		private void BkgOperate(object arg)
		{
			var optData = (TCPSessionInfo.OperateData)arg;
			optData.owner.BkgOperate(optData);
		}

		private object BkgReceive()
		{
			var receives = new Dictionary<int, object>();
			foreach (var key_value in bkgSessions)
			{
				var receive = key_value.Value.BkgReceive();
				if (null != receive)
				{
					receives[key_value.Key] = receive;
				}
			}
			return receives;
		}
		#endregion background

		#region abstract
		protected abstract void HandleReceive(int ID, object p);
		protected virtual void HandleReceives(object p)
		{
			var receives = p as Dictionary<int, object>;
			if (!receives.IsNullOrEmpty())
			{
				foreach (var key_value in receives)
				{
					HandleReceive(key_value.Key, key_value.Value);
				}
			}
		}
		#endregion abstract

		#region behaviour
		protected virtual void Start()
		{
			asyncOperate.StartWork(BkgOperate);
		}

		protected virtual void FixedUpdate()
		{
			if (0 < sessions.Count)
			{
				if (!asyncReceive.working)
				{
					asyncReceive.StartWork(BkgReceive);
				}
				else
				{
					asyncReceive.ConsumeProducts(HandleReceives);
				}
			}
			else
			{
				asyncReceive.EndWork();
			}
		}

		protected virtual void OnDestroy()
		{
			foreach (var session in sessions.Values)
			{
				session.Dispose();
			}
			asyncReceive.EndWork();
			asyncOperate.EndWork();
		}
		#endregion behaviour
	}
} // namespace Ghost
