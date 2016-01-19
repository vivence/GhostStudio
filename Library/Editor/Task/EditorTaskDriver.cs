using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Ghost.EditorTool
{
	[CustomEditor(typeof(Task.TaskDriver), true), CanEditMultipleObjects]
	public class E_TaskDriver : Editor
	{
		public override void OnInspectorGUI ()
		{
			if (Application.isPlaying)
			{
				return;
			}
			base.OnInspectorGUI();
		}
	}
} // namespace Ghost.EditorTool
