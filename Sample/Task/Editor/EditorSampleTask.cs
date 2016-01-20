using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Ghost.Sample.EditorTool
{
	[CustomEditor(typeof(SampleTask)), CanEditMultipleObjects]
	public class EditorSampleTask : Editor
	{
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
			if (!Application.isPlaying)
			{
				return;
			}
			EditorGUILayout.Separator();
			if (GUILayout.Button("Start"))
			{
				int count = 0;
				foreach (var t in targets)
				{
					var task = t as SampleTask;
					if (task.StartTask())
					{
						++count;
					}
				}
				Debug.LogFormat("Start task count: {0}", count);
			}
			if (GUILayout.Button("End"))
			{
				int count = 0;
				foreach (var t in targets)
				{
					var task = t as SampleTask;
					if (task.EndTask())
					{
						++count;
					}
				}
				Debug.LogFormat("End task count: {0}", count);
			}
		}
	
	}
} // namespace Ghost.Sample.EditorTool
