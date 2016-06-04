using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using Ghost.Extension;

namespace Ghost
{
	[System.Serializable]
	public class TCPSessionInfo : IDisposable
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
			public TCPSessionInfo session;
			public Operate opt;
			public object[] args;

			public OperateData(TCPSessionInfo s, Operate o, object[] a = null)
			{
				session = s;
				opt = o;
				args = a;
			}

			public static OperateData Create(TCPSessionInfo s, Operate opt, params object[] args)
			{
				return new OperateData(s, opt, args);
			}
		}

		public static void LoopReceive(Socket socket, byte[] buffer, int offset, int size)
		{
			while (0 < size)
			{
				var ret = socket.Receive(buffer, offset, size, SocketFlags.None);
				offset += ret;
				size -= ret;
			}
		}

		private TcpClient tcp = null;

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

		[System.Serializable]
		public class Setting
		{
			public int sendTimeout = 0;
			public int receiveTimeout = 0;
			public int sendBufferSize = 0;
			public int receiveBufferSize = 0;
			public bool blocking = true;
		}
		public string host;
		public int port;
		public Setting setting;

		private void TCPSetting()
		{
			tcp.SendTimeout = setting.sendTimeout;
			tcp.ReceiveTimeout = setting.receiveTimeout;
			tcp.SendBufferSize = setting.sendBufferSize;
			tcp.ReceiveBufferSize = setting.receiveBufferSize;
			tcp.Client.Blocking = setting.blocking;
		}

		public TCPSessionInfo(TcpClient client, Setting s = null)
		{
			#if DEBUG
			Debug.Assert(null != client);
			#endif // DEBUG
			tcp = client;
			if (tcp.Connected)
			{
				phase = Phase.Connected;
				host = tcp.Client.RemoteEndPoint.ToString();
				port = 0;
			}
			if (null == s)
			{
				s = new Setting();
			}
			setting = s;
			TCPSetting();
		}

		public void Dispose()
		{
			tcp.Close();
			phase = Phase.None;
		}

		public bool Connect(Async.Consumer asyncOperate)
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

			TCPSetting();

			phase = Phase.ConnectPost;
			asyncOperate.PostProduct(OperateData.Create(this, Operate.Connect, host, port));
			return true;
		}

		public bool Disconnect(Async.Consumer asyncOperate)
		{
			if (!AllowDisconnect())
			{
				return false;
			}
			phase = Phase.ClosePost;
			asyncOperate.PostProduct(OperateData.Create(this, Operate.Disconnect));
			return true;
		}

		public bool Send(Async.Consumer asyncOperate, params object[] args)
		{
			if (!AllowSend())
			{
				return false;
			}
			if (args.IsNullOrEmpty())
			{
				return false;
			}
			asyncOperate.PostProduct(OperateData.Create(this, Operate.Send, args));
			return true;
		}

		#region background
		public System.Action<Socket, object[]> DoBkgSend;
		public System.Func<Socket, object> DoBkgReceive;

		public void BkgOperate(OperateData optData)
		{
			#if DEBUG
			Debug.Assert(this == optData.session);
			#endif // DEBUG
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
						#if DEBUG
						Debug.Assert(null != DoBkgSend);
						#endif // DEBUG
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

		public object BkgReceive()
		{
			if (AllowReceive())
			{
				try
				{
					#if DEBUG
					Debug.Assert(null != DoBkgReceive);
					#endif // DEBUG
					return DoBkgReceive(tcp.Client);
				}
				catch (System.Exception e)
				{
					exception = e;
				}
			}
			return null;
		}
		#endregion background
	}
} // namespace Ghost
