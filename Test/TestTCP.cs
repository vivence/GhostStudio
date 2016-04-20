using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Ghost.Test
{
	public class TestTCP : MonoBehaviour 
	{
		public string host = "stdtime.gov.hk";
		public int port = 13;
		public string state;

		private TcpClient tcp;

		#region Interlocked
		private int connectCallback = 0;
		#endregion Interlocked

		#region callback
		private static void ConnectCallback(IAsyncResult result)
		{
			Debug.LogFormat("current thread({0}): {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name);
			var self = result.AsyncState as TestTCP;
			if (null != self)
			{
				Interlocked.Exchange(ref self.connectCallback, 1);
			}
		}
		#endregion callback

		#region behavoir
		void Start()
		{
			Debug.LogFormat("current thread({0}): {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name);
			try
			{
				tcp = new TcpClient();
				Interlocked.Exchange(ref connectCallback, 0);
				var result = tcp.BeginConnect(host, port, ConnectCallback, this);
				if (result.IsCompleted)
				{
					state = tcp.Connected ? "connected" : "connect failed";
				}
				else
				{
					state = "connecting";
				}
			}
			catch (Exception e)
			{
				state = string.Format("excption: {0}", e.Message);
				if (null != tcp)
				{
					tcp.Close();
					tcp = null;
				}
			}
		}

		void Destroy()
		{
			if (null != tcp)
			{
				tcp.Close();
				tcp = null;
			}
		}

		void FixedUpdate()
		{
			if (1 == Interlocked.CompareExchange(ref connectCallback, 0, 1))
			{
				if (null != tcp)
				{
					state = tcp.Connected ? "connected" : "connect failed";
				}
			}
		}
		#endregion behavoir
	}
} // namespace Ghost.Test
