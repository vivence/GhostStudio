using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Ghost.EditorTool
{
	[CustomEditor(typeof(TCPSession), true)]
	public class E_TCPSession : Editor
	{

		public override void OnInspectorGUI ()
		{
			var tcp = target as TCPSession;

			EditorGUI.BeginDisabledGroup(!tcp.AllowConnect());
			tcp.host = EditorGUILayout.TextField("Host", tcp.host);
			tcp.port = EditorGUILayout.IntField("Port", tcp.port);
			tcp.sendTimeout = EditorGUILayout.IntField("Send Timeout", tcp.sendTimeout);
			tcp.receiveTimeout = EditorGUILayout.IntField("Receive Timeout", tcp.receiveTimeout);
			tcp.sendBufferSize = EditorGUILayout.IntField("Send Buffer Size", tcp.sendBufferSize);
			tcp.receiveBufferSize = EditorGUILayout.IntField("Receive Buffer Size", tcp.receiveBufferSize);
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(false);
			EditorGUILayout.EnumPopup("Phase", tcp.phase);
			EditorGUI.EndDisabledGroup();

			if (Application.isPlaying)
			{
				EditorGUILayout.Separator();
				if (tcp.AllowConnect())
				{
					if (GUILayout.Button("Connect"))
					{
						tcp.Connect();
					}
				}
				else if (tcp.AllowDisconnect())
				{
					if (GUILayout.Button("Disconnect"))
					{
						tcp.Disconnect();
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
