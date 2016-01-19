using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Ghost.Extension;

namespace Ghost.Utility
{
	public static class String
	{
		public static string Join(params object[] objs)
		{
			if (objs.IsNullOrEmpty ()) 
			{
				return string.Empty;
			}
			var builder = new StringBuilder ();
			foreach (var obj in objs) 
			{
				builder.Append (obj);
			}
			return builder.ToString ();
		}
	}
} // namespace Ghost.Utility
