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
			var info = tcp.info;

			EditorGUI.BeginDisabledGroup(!info.AllowConnect());
			base.OnInspectorGUI();
			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(false);
			EditorGUILayout.EnumPopup("Phase", info.phase);
			EditorGUI.EndDisabledGroup();

			if (Application.isPlaying)
			{
				EditorGUILayout.Separator();
				if (info.AllowConnect())
				{
					if (GUILayout.Button("Connect"))
					{
						tcp.Connect();
					}
				}
				else if (info.AllowDisconnect())
				{
					if (GUILayout.Button("Disconnect"))
					{
						tcp.Disconnect();
					}
				}

				var exception = info.exception;
				if (null != exception)
				{
					EditorGUILayout.Separator();
					EditorGUILayout.LabelField("Exception", exception.Message);
				}
			}
		}
	}
} // namespace Ghost.EditorTool
