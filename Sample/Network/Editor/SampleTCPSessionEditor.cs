using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Ghost.Sample;

namespace Ghost.EditorTool
{
	[CustomEditor(typeof(SampleTCPSession))]
	public class E_SampleTCPSession : E_TCPSession
	{
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			if (Application.isPlaying)
			{
				var session = target as TCPSession;
				var info = session.info;

				if (TCPSessionInfo.Phase.Connected == info.phase)
				{
					EditorGUILayout.Separator();


				}
			}
		}
	
	}
} // namespace Ghost.EditorTool
