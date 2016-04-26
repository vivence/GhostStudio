using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Ghost.Sample
{
	public class SampleTCPSession : TCPSession 
	{
		private byte[] buffer = new byte[1024];
		private MemoryStream stream = new MemoryStream();

		#region override
		protected override void DoBkgSend (Socket tcp, object[] args)
		{
			
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

		#endregion behaviour
	}
} // namespace Ghost.Sample
