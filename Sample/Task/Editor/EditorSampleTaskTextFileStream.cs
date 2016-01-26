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
			if (GUILayout.Button("Start"))
			{
				int count = 0;
				foreach (var t in targets)
				{
					var task = t as SampleTaskTextFileStream;
					if (task.StartAccess())
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
					var task = t as SampleTaskTextFileStream;
					if (task.EndAccess())
					{
						++count;
					}
				}
				Debug.LogFormat("End task count: {0}", count);
			}
		}
	
	}
} // namespace Ghost.Sample.EditorTool
