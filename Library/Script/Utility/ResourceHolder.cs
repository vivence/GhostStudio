using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ghost.Utility
{
	public abstract class ResourceHolder : IDisposable
	{
		private bool disposed = false;

		~ResourceHolder()
		{
			Release (false);
		}

		private void Release(bool fromDisposing)
		{
			if (disposed)
			{
				return;
			}
			if (fromDisposing) 
			{ 
				ReleaseManagedResource();
			}
			ReleaseUnmanagedResource();
			disposed = true;
		}

		#region virtual
		protected virtual void ReleaseManagedResource(){}
		protected virtual void ReleaseUnmanagedResource(){}
		#endregion virtual

		#region IDisposable
		public void Dispose() 
		{
			Release (true);
			GC.SuppressFinalize (this); 
		}
		#endregion IDisposable
	}
} // namespace Ghost.Utility
