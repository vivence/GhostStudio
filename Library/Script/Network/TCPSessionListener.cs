using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using Ghost.Extension;

namespace Ghost
{
	public abstract class TCPSessionListener : MonoBehaviour 
	{
		public enum Phase
		{
			None,
			StartPost,
			Started,
			StopPost,
			Stoped,
			Exception
		}

		public enum Operate
		{
			Start,
			Stop,
		}
		public struct OperateData
		{
			public Operate opt;
			public object[] args;

			public OperateData(Operate o, object[] a = null)
			{
				opt = o;
				args = a;
			}

			public static OperateData Create(Operate opt, params object[] args)
			{
				return new OperateData(opt, args);
			}
		}

		private Async.Producer asyncAccept = new Async.Producer();
		private Async.Consumer asyncOperate = new Async.Consumer();

		private TcpListener tcp_;
		private TcpListener tcp
		{
			get
			{
				return tcp_;
			}
			set
			{
				if (value == tcp)
				{
					return;
				}
				if (null != tcp)
				{
					tcp.Stop();
				}
				tcp_ = value;
			}
		}

		#region sync
		public Phase phase
		{
			get
			{
				return syncInfo.GetPhase();
			}
			set
			{
				syncInfo.SetPhase(value);
			}
		}
		public System.Exception exception
		{
			get
			{
				return syncInfo.GetException();
			}
			set
			{
				syncInfo.SetException(value);
			}
		}
		[System.Runtime.Remoting.Contexts.SynchronizationAttribute]
		public class SyncInfo : System.ContextBoundObject
		{
			private System.Exception excetion = null;
			private Phase phase = Phase.None;

			public System.Exception GetException()
			{
				return excetion;
			}
			public void SetException(System.Exception e)
			{
				if (null != e)
				{
					if (null == excetion)
					{
						excetion = e;
						phase = Phase.Exception;
					}
				}
				else
				{
					excetion = null;
					phase = Phase.None;
				}
			}

			public Phase GetPhase()
			{
				return phase;
			}
			public void SetPhase(Phase p)
			{
				phase = p;
			}
		}
		private SyncInfo syncInfo = new SyncInfo();

		public bool AllowStart()
		{
			var p = phase;
			switch (p)
			{
			case Phase.None:
				return true;
			}
			return false;
		}
		public bool AllowDoStart()
		{
			var p = phase;
			switch (p)
			{
			case Phase.StartPost:
				return true;
			}
			return false;
		}

		public bool AllowStop()
		{
			var p = phase;
			switch (p)
			{
			case Phase.Started:
				return true;
			}
			return false;
		}
		public bool AllowDoStop()
		{
			var p = phase;
			switch (p)
			{
			case Phase.StopPost:
				return true;
			}
			return false;
		}

		public bool AllowAccept()
		{
			var p = phase;
			switch (p)
			{
			case Phase.Started:
				return true;
			}
			return false;
		}
		#endregion sync

		public string ip;
		public int port;
		public bool blocking = true;

		public bool Start()
		{
			if (!AllowStart())
			{
				return false;
			}
			if (0 >= port)
			{
				return false;
			}
			var ipAdress = string.IsNullOrEmpty(ip) ? IPAddress.Any : IPAddress.Parse(ip);
			tcp = new TcpListener(ipAdress, port);
			tcp.Server.Blocking = blocking;

			if (!asyncOperate.working)
			{
				asyncOperate.StartWork(BkgOperate);
			}
			phase = Phase.StartPost;
			asyncOperate.PostProduct(OperateData.Create(Operate.Start));
			return true;
		}

		public bool Stop()
		{
			if (!AllowStop())
			{
				return false;
			}
			phase = Phase.StopPost;
			asyncOperate.PostProduct(OperateData.Create(Operate.Stop));
			return true;
		}

		#region background

		private void BkgOperate(object arg)
		{
			var optData = (OperateData)arg;
			try
			{
				switch (optData.opt)
				{
				case Operate.Start:
					if (AllowDoStart())
					{
						tcp.Start();
						phase = Phase.Started;
					}
					break;
				case Operate.Stop:
					if (AllowDoStop())
					{
						tcp.Stop();
						phase = Phase.Stoped;
					}
					break;
				}
			}
			catch (System.Exception e)
			{
				exception = e;
			}
		}

		private object BkgAccept()
		{
			if (AllowAccept())
			{
				try
				{
					return tcp.AcceptTcpClient();
				}
				catch (System.Exception e)
				{
					exception = e;
				}
			}
			return null;
		}
		#endregion background

		#region abstract
		protected abstract void HandleSession(object p);
		#endregion abstract

		#region behaviour
		protected virtual void FixedUpdate()
		{
			var p = phase;
			switch (p)
			{
			case Phase.Stoped:
				GameObject.Destroy(gameObject);
				break;
			case Phase.Exception:
				asyncAccept.EndWork();
				asyncOperate.EndWork();
				break;
			case Phase.StopPost:
				asyncAccept.EndWork();
				break;
			case Phase.Started:
				if (!asyncAccept.working)
				{
					asyncAccept.StartWork(BkgAccept);
				}
				else
				{
					asyncAccept.ConsumeProducts(HandleSession);
				}
				break;
			}
		}

		protected virtual void OnDestroy()
		{
			tcp = null;
			asyncAccept.EndWork();
			asyncOperate.EndWork();
		}
		#endregion behaviour
	}
} // namespace Ghost
