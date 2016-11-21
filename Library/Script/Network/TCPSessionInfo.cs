using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using Ghost.Extension;
using Ghost.Utility;

namespace Ghost
{
	[System.Serializable]
	public class TCPSessionInfo : IDisposable, Async.IProductOwner
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
		public class OperateData : Async.ProductBase<TCPSessionInfo, Operate>
		{
		}
		private ObjectPool<OperateData> optDataPool = new ObjectPool<OperateData>();

		#region IProductOwner
		public void DestroyProduct(IDisposable p)
		{
			var optData = p as OperateData;
			#if DEBUG
			Debug.Assert(null != optData);
			#endif // DEBUG
			if (null != optDataPool)
			{
				optDataPool.Destroy(optData);
			}
			else
			{
				optData.Destroy();
			}
		}
		#endregion IProductOwner

		#region helper
		public static void LoopSend(
			Socket socket, 
			byte[] data, 
			int offset, 
			int size, 
			SocketFlags flags = SocketFlags.None)
		{
			while (0 < size)
			{
				var ret = socket.Send(data, offset, size, flags);
				if (0 >= ret)
				{
					throw new System.IO.IOException(string.Format("Send return {0}", ret));
				}
				offset += ret;
				size -= ret;
			}
		}
		public static void LoopReceive(
			Socket socket, 
			byte[] buffer, 
			int offset, 
			int size, 
			SocketFlags flags = SocketFlags.None)
		{
			while (0 < size)
			{
				var ret = socket.Receive(buffer, offset, size, flags);
				if (0 >= ret)
				{
					throw new System.IO.IOException(string.Format("Receive return {0}", ret));
				}
				offset += ret;
				size -= ret;
			}
		}
		#endregion helper

		public TcpClient tcp{get;private set;}

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
		public class Setting : ICloneable
		{
			public int sendTimeout = 0;
			public int receiveTimeout = 0;
			public int sendBufferSize = 0;
			public int receiveBufferSize = 0;
			public bool blocking = true;

			public object Clone()
			{
				return MemberwiseClone();
			}

			public Setting CloneSelf()
			{
				return Clone() as Setting;
			}
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

		#region IDispose
		public void Dispose()
		{
			optDataPool.Dispose();
			optDataPool = null;
			tcp.Close();
			phase = Phase.None;
		}
		#endregion IDispose

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

			asyncOperate.PostProduct(optDataPool.Create(this, Operate.Connect, host, port));
			return true;
		}

		public bool Disconnect(Async.Consumer asyncOperate)
		{
			if (!AllowDisconnect())
			{
				return false;
			}
			phase = Phase.ClosePost;
			asyncOperate.PostProduct(optDataPool.Create(this, Operate.Disconnect));
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
			asyncOperate.PostProduct(optDataPool.Create(this, Operate.Send, args));
			return true;
		}

		#region background
		public System.Action<Socket, object[]> DoBkgSend;
		public System.Func<Socket, object> DoBkgReceive;

		public bool BkgConnect(OperateData optData)
		{
			#if DEBUG
			Debug.Assert(this == optData.owner);
			#endif // DEBUG
			if (!AllowDoConnect())
			{
				return false;
			}
			var host = (string)optData.args[0];
			var port = (int)optData.args[1];
			tcp.Client.Connect(host, port);
			phase = Phase.Connected;
			return true;
		}

		public bool BkgDisconnect(OperateData optData)
		{
			#if DEBUG
			Debug.Assert(this == optData.owner);
			#endif // DEBUG
			if (!AllowDoDisconnect())
			{
				return false;
			}
			tcp.Client.Disconnect(true);
			phase = Phase.Closed;
			return true;
		}

		public bool BkgSend(OperateData optData)
		{
			#if DEBUG
			Debug.Assert(this == optData.owner);
			#endif // DEBUG
			if (!AllowSend())
			{
				return false;
			}

			#if DEBUG
			Debug.Assert(null != DoBkgSend);
			#endif // DEBUG
			DoBkgSend(tcp.Client, optData.args);
			return true;
		}

		public void BkgOperate(OperateData optData)
		{
			#if DEBUG
			Debug.Assert(this == optData.owner);
			#endif // DEBUG
			try
			{
				switch (optData.opt)
				{
				case Operate.Connect:
					BkgConnect(optData);
					break;
				case Operate.Disconnect:
					BkgDisconnect(optData);
					break;
				case Operate.Send:
					BkgSend(optData);
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
