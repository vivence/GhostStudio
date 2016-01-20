using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Ghost.Sample.EditorTool
{
	[CustomEditor(typeof(SampleTaskTextFileStream)), CanEditMultipleObjects]
	public class E_SampleTaskTextFileStream : Editor
	{
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
			if (!Application.isPlaying)
			{
				return;
			}
			EditorGUILayout.Separator();
			if (GUILayout.Button("SyncReadStart"))
			{
				int count = 0;
				foreach (var t in targets)
				{
					var task = t as SampleTaskTextFileStream;
					if (task.StartSyncRead())
					{
						++count;
					}
				}
				Debug.LogFormat("Start task(sync read) count: {0}", count);
			}
			if (GUILayout.Button("SyncReadEnd"))
			{
				int count = 0;
				foreach (var t in targets)
				{
					var task = t as SampleTaskTextFileStream;
					if (task.EndSyncReadTask())
					{
						++count;
					}
				}
				Debug.LogFormat("End task(sync read) count: {0}", count);
			}
		}
	
	}
} // namespace Ghost.Sample.EditorTool
