using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

namespace Ghost.EditorTool
{
	[CustomEditor(typeof(ThreadMonitor))]
	public class E_ThreadMonitor : Editor
	{
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			var sb = new StringBuilder();

			var monitor = target as ThreadMonitor;
			var activeThreads = monitor.activeThreads;
			if (null != activeThreads && 0 < activeThreads.Count)
			{
				sb.Append("active threads:\n");
				foreach (var t in activeThreads)
				{
					sb.AppendFormat("  {0}", t.ManagedThreadId);
				}
				sb.AppendLine();
			}
			var abortThreads = monitor.abortThreads;
			if (null != abortThreads && 0 < abortThreads.Count)
			{
				var fixedTime = Time.fixedTime;
				sb.Append("abort threads:\n");
				foreach (var key_value in abortThreads)
				{
					sb.AppendFormat("  {0}, {1}s", key_value.Key.ManagedThreadId, key_value.Value-fixedTime);
				}
			}

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.TextArea(sb.ToString());
			EditorGUI.EndDisabledGroup();
		}

	}
} // namespace Ghost.EditorTool
