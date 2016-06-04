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
	public class SampleTCPSessionListener : TCPSessionListener 
	{
		private Phase prevPhase = Phase.None;

		#region override
		protected override void HandleSession (object p)
		{
			var client = p as TcpClient;
			if (null == client)
			{
				return;
			}
			Debug.LogFormat("<color=green>New Client: </color>{0}", client.Client.RemoteEndPoint);
			client.Close();
		}
		#endregion override

		#region behaviour
		protected override void FixedUpdate ()
		{
			base.FixedUpdate ();
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
