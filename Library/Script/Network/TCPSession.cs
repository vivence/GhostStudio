using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using Ghost.Extension;

namespace Ghost
{
	public abstract class TCPSession : MonoBehaviour 
	{
		public enum Phase
		{
			None,
			ConnectPost,
			Connected,
			ClosePost,
			Closed,
			Exception
		}

		public enum Operate
		{
			Connect,
			Disconnect,
			Send,
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

		private Async.Producer asyncReceive = new Async.Producer();
		private Async.Consumer asyncOperate = new Async.Consumer();

		private TcpClient tcp = new TcpClient();

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

		public bool AllowConnect()
		{
			var p = phase;
			switch (p)
			{
			case Phase.None:
				return true;
			}
			return false;
		}
		public bool AllowDoConnect()
		{
			var p = phase;
			switch (p)
			{
			case Phase.ConnectPost:
				return true;
			}
			return false;
		}

		public bool AllowDisconnect()
		{
			var p = phase;
			switch (p)
			{
			case Phase.Connected:
				return true;
			}
			return false;
		}
		public bool AllowDoDisconnect()
		{
			var p = phase;
			switch (p)
			{
			case Phase.ClosePost:
				return true;
			}
			return false;
		}

		public bool AllowSend()
		{
			var p = phase;
			switch (p)
			{
			case Phase.Connected:
				return true;
			}
			return false;
		}

		public bool AllowReceive()
		{
			var p = phase;
			switch (p)
			{
			case Phase.Connected:
				return true;
			}
			return false;
		}
		#endregion sync

		public string host;
		public int port;
		public int sendTimeout;
		public int receiveTimeout;
		public int sendBufferSize;
		public int receiveBufferSize;
		public bool blocking = true;

		public bool Connect()
		{
			if (!AllowConnect())
			{
				return false;
			}
			if (string.IsNullOrEmpty(host))
			{
				return false;
			}
			if (0 >= port)
			{
				return false;
			}

			tcp.SendTimeout = sendTimeout;
			tcp.ReceiveTimeout = receiveTimeout;
			tcp.SendBufferSize = sendBufferSize;
			tcp.ReceiveBufferSize = receiveBufferSize;
			tcp.Client.Blocking = blocking;

			if (!asyncOperate.working)
			{
				asyncOperate.StartWork(BkgOperate);
			}
			phase = Phase.ConnectPost;
			asyncOperate.PostProduct(OperateData.Create(Operate.Connect, host, port));
			return true;
		}

		public bool Disconnect()
		{
			if (!AllowDisconnect())
			{
				return false;
			}
			phase = Phase.ClosePost;
			asyncOperate.PostProduct(OperateData.Create(Operate.Disconnect));
			return true;
		}

		public bool Send(uint id1, uint id2, byte[] data)
		{
			if (!AllowSend())
			{
				return false;
			}
			if (data.IsNullOrEmpty())
			{
				return false;
			}
			asyncOperate.PostProduct(OperateData.Create(Operate.Send, id1, id2, data));
			return true;
		}

		#region background

		#region abstract
		protected abstract void DoBkgSend(Socket tcp, object[] args);
		protected abstract object DoBkgReceive(Socket tcp);
		#endregion abstract

		private void BkgOperate(object arg)
		{
			var optData = (OperateData)arg;
			try
			{
				switch (optData.opt)
				{
				case Operate.Connect:
					if (AllowDoConnect())
					{
						var host = (string)optData.args[0];
						var port = (int)optData.args[1];
						tcp.Client.Connect(host, port);
						phase = Phase.Connected;
					}
					break;
				case Operate.Disconnect:
					if (AllowDoDisconnect())
					{
						tcp.Client.Disconnect(true);
						phase = Phase.Closed;
					}
					break;
				case Operate.Send:
					if (AllowSend())
					{
						DoBkgSend(tcp.Client, optData.args);
					}
					break;
				}
			}
			catch (System.Exception e)
			{
				exception = e;
			}
		}

		private object BkgReceive()
		{
			if (AllowReceive())
			{
				try
				{
					return DoBkgReceive(tcp.Client);
				}
				catch (System.Exception e)
				{
					exception = e;
				}
			}
			return null;
		}
		private static void BkgLoopReceive(Socket socket, byte[] buffer, int offset, int size)
		{
			while (0 < size)
			{
				var ret = socket.Receive(buffer, offset, size, SocketFlags.None);
				offset += ret;
				size -= ret;
			}
		}
		#endregion background

		#region abstract
		protected abstract void HandleReceive(object p);
		#endregion abstract

		#region behaviour
		protected virtual void FixedUpdate()
		{
			var p = phase;
			switch (p)
			{
			case Phase.Closed:
				GameObject.Destroy(gameObject);
				break;
			case Phase.Exception:
				asyncReceive.EndWork();
				asyncOperate.EndWork();
				break;
			case Phase.ClosePost:
				asyncReceive.EndWork();
				break;
			case Phase.Connected:
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
			tcp.Close();
			asyncReceive.EndWork();
			asyncOperate.EndWork();
		}
		#endregion behaviour
	}
} // namespace Ghost
