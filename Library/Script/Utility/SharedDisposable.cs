using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ghost.Utility
{
	public class SharedDisposable : ResourceHolder
	{
		class RefCount
		{
			public int count = 0;
		}

		private IDisposable obj;
		private RefCount refCount;

		public SharedDisposable (IDisposable disposable)
		{
			obj = disposable;
			refCount = new RefCount();
			++refCount.count;
		}

		public SharedDisposable (SharedDisposable other)
		{
			obj = other.obj;
			refCount = other.refCount;
			++refCount.count;
		}

		public SharedDisposable Share()
		{
			return new SharedDisposable(this);
		}

		private void TryRelease()
		{
			if (0 == --refCount.count)
			{
				obj.Dispose();
			}
		}

		#region override
		protected override void ReleaseManagedResource ()
		{
			base.ReleaseManagedResource();
			TryRelease();
		}
		#endregion override
	}
} // namespace Ghost.Utility
