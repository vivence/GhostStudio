using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using Ghost.Extension;
using Ghost.Utility;

namespace Ghost.Sample
{
	public class SampleTCPSession : TCPSession 
	{
		private byte[] buffer = new byte[1024];
		private MemoryStream stream = new MemoryStream();

		private Phase prevPhase = Phase.None;

		#region sync
		private ReuseableList<int> sendedIDs;

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void NotifySended(int sendID)
		{
			sendedIDs.list.Add(sendID);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public ReuseableList<int> GetSendIDs()
		{
			if (0 >= sendedIDs.list.Count)
			{
				return null;
			}
			var ret = sendedIDs;
			sendedIDs = ObjectPool<ReuseableList<int>>.Singleton.Create();
			return ret;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ReuseSendIDs(ReuseableList<int> p)
		{
			if (p != sendedIDs)
			{
				ObjectPool<ReuseableList<int>>.Singleton.Destroy(p);
			}
		}
		#endregion sync

		#region override
		protected override void DoBkgSend (Socket tcp, object[] args)
		{
			#if DEBUG
			Debug.Assert(null != args && 1 < args.Length);
			#endif

			var ID = System.Convert.ToInt32(args[0]);
			var data = args[1] as byte[];

			#if DEBUG
			Debug.Assert(null != data);
			#endif

			var len = tcp.Send(data);

			if (data.Length == len)
			{
				NotifySended(ID);
			}
		}

		protected override object DoBkgReceive (Socket tcp)
		{
			var len = tcp.Receive(buffer);
			while (buffer.Length == len)
			{
				stream.Write(buffer, 0, len);
			}
			if (0 < len)
			{
				stream.Write(buffer, 0, len);
			}
			if (0 < stream.Length)
			{
				var str = Encoding.UTF8.GetString(stream.GetBuffer(), 0, System.Convert.ToInt32(stream.Length));
				stream.SetLength(0);
				return str;
			}
			return null;
		}

		protected override void HandleReceive (object p)
		{
			Debug.LogFormat("<color=green>Receive: </color>\n{0}", System.Convert.ToString(p));
		}
		#endregion override

		#region behaviour
		protected virtual void Awake()
		{
			sendedIDs = ObjectPool<ReuseableList<int>>.Singleton.Create();
		}

		protected override void FixedUpdate ()
		{
			base.FixedUpdate ();
			var p = GetSendIDs();
			if (null != p)
			{
				for (int i = 0; i < p.list.Count; ++i)
				{
					Debug.LogFormat("<color=green>Sended: </color>\n{0}", p.list[i]);
				}
				ReuseSendIDs(p);
			}

			if (prevPhase != phase)
			{
				Debug.LogFormat("<color=green>Phase Changed: </color>\n{0} -> {1}", prevPhase, phase);
				var e = exception;
				if (null != e)
				{
					Debug.LogFormat("<color=green>Exception: </color>\n{0}", e);
				}

				prevPhase = phase;
			}
		}
		#endregion behaviour
	}
} // namespace Ghost.Sample
