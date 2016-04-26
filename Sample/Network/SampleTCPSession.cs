using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Ghost.Sample
{
	public class SampleTCPSession : TCPSession 
	{

		#region override
		protected override void DoBkgSend (Socket tcp, object[] args)
		{
			
		}

		protected override object DoBkgReceive (Socket tcp)
		{
			return null;
		}

		protected override void HandleReceive (object p)
		{
			
		}
		#endregion override

		#region behaviour

		#endregion behaviour
	}
} // namespace Ghost.Sample
