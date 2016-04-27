using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Ghost.Sample;

namespace Ghost.EditorTool
{
	[CustomEditor(typeof(SampleTCPSession))]
	public class E_SampleTCPSession : Editor
	{
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			if (Application.isPlaying)
			{
				var session = target as TCPSession;

				if (TCPSession.Phase.Connected == session.phase)
				{
					EditorGUILayout.Separator();


				}
			}
		}
	
	}
} // namespace Ghost.EditorTool
