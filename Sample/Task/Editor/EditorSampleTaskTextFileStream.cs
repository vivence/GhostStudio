using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Ghost.Sample.EditorTool
{
	[CustomEditor(typeof(SampleTaskTextFileStream), true), CanEditMultipleObjects]
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
			if (GUILayout.Button("StartRead"))
			{
				int count = 0;
				foreach (var t in targets)
				{
					var task = t as SampleTaskTextFileStream;
					if (task.StartRead())
					{
						++count;
					}
				}
				Debug.LogFormat("Start read task count: {0}", count);
			}
			if (GUILayout.Button("EndRead"))
			{
				int count = 0;
				foreach (var t in targets)
				{
					var task = t as SampleTaskTextFileStream;
					if (task.EndRead())
					{
						++count;
					}
				}
				Debug.LogFormat("End read task count: {0}", count);
			}
		}
	
	}
} // namespace Ghost.Sample.EditorTool
