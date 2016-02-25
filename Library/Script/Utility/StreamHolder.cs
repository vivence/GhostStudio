using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace Ghost.Utility
{
	public class StreamHolder : ResourceHolder
	{
		private Stream stream;

		public StreamHolder(Stream s)
		{
			#if DEBUG
			Debug.Assert(null != s);
			#endif // DEBUG
			stream = s;
		}

		private void ReleaseStream()
		{
			stream.Close();
			stream.Dispose();
		}

		#region override
		protected override void ReleaseManagedResource ()
		{
			base.ReleaseManagedResource();
			ReleaseStream();
		}
		#endregion override
	}
} // namespace Ghost.Utility
