using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using Ghost.Extension;

namespace Ghost
{
	public abstract class TCPSession : MonoBehaviour 
	{
		[SerializeField]
		protected TCPSessionInfo info_ = new TCPSessionInfo(new TcpClient());
		public TCPSessionInfo info
		{
			get
			{
				return info_;
			}
		}

		private Async.Producer asyncReceive = new Async.Producer();
		private Async.Consumer asyncOperate = new Async.Consumer();

		public bool Connect()
		{
			return info.Connect(asyncOperate);
		}

		public bool Disconnect()
		{
			return info.Disconnect(asyncOperate);
		}

		public bool Send(params object[] args)
		{
			return info.Send(asyncOperate, args);
		}

		#region background

		#region abstract
		protected abstract void DoBkgSend(Socket tcp, object[] args);
		protected abstract object DoBkgReceive(Socket tcp);
		#endregion abstract

		private void BkgOperate(object arg)
		{
			info.BkgOperate((TCPSessionInfo.OperateData)arg);
		}

		private object BkgReceive()
		{
			return info.BkgReceive();
		}
		#endregion background

		#region abstract
		protected abstract void HandleReceive(object p);
		#endregion abstract

		#region behaviour
		protected virtual void Start()
		{
			info.DoBkgSend = DoBkgSend;
			info.DoBkgReceive = DoBkgReceive;

			asyncOperate.StartWork(BkgOperate);
		}

		protected virtual void FixedUpdate()
		{
			var p = info.phase;
			switch (p)
			{
			case TCPSessionInfo.Phase.Closed:
				GameObject.Destroy(gameObject);
				break;
			case TCPSessionInfo.Phase.Exception:
				asyncReceive.EndWork();
				asyncOperate.EndWork();
				break;
			case TCPSessionInfo.Phase.ClosePost:
				asyncReceive.EndWork();
				break;
			case TCPSessionInfo.Phase.Connected:
				if (!asyncReceive.working)
				{
					asyncReceive.StartWork(BkgReceive);
				}
				else
				{
					asyncReceive.ConsumeProducts(HandleReceive);
				}
				break;
			}
		}

		protected virtual void OnDestroy()
		{
			info.Dispose();
			asyncReceive.EndWork();
			asyncOperate.EndWork();
		}
		#endregion behaviour
	}
} // namespace Ghost
