using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Ghost.EditorTool
{
	[CustomEditor(typeof(TCPSessionListener), true)]
	public class E_TCPSessionListener : Editor
	{

		public override void OnInspectorGUI ()
		{
			var tcp = target as TCPSessionListener;

			EditorGUI.BeginDisabledGroup(!tcp.AllowStart());
			tcp.ip = EditorGUILayout.TextField((string.IsNullOrEmpty(tcp.ip) ? "IP(Any)" : "IP"), tcp.ip);
			tcp.port = EditorGUILayout.IntField("Port", tcp.port);
			tcp.blocking = EditorGUILayout.ToggleLeft("Blocking", tcp.blocking);
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(false);
			EditorGUILayout.EnumPopup("Phase", tcp.phase);
			EditorGUI.EndDisabledGroup();

			if (Application.isPlaying)
			{
				EditorGUILayout.Separator();
				if (tcp.AllowStart())
				{
					if (GUILayout.Button("Start"))
					{
						tcp.Start();
					}
				}
				else if (tcp.AllowStop())
				{
					if (GUILayout.Button("Stop"))
					{
						tcp.Stop();
					}
				}

				var exception = tcp.exception;
				if (null != exception)
				{
					EditorGUILayout.Separator();
					EditorGUILayout.LabelField("Exception", exception.Message);
				}
			}
		}
	}
} // namespace Ghost.EditorTool
