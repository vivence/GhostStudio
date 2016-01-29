using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Ghost.EditorTool
{
	[CustomEditor(typeof(Transform)), CanEditMultipleObjects]
	public class E_Transform : Editor
	{
		interface IVector3Property
		{
			Vector3 GetValue(SerializedProperty property);
			void SetValue(SerializedProperty property, Vector3 value);
		}
		class Vector3Property : IVector3Property
		{
			public static IVector3Property Global = new Vector3Property();

			public Vector3 GetValue(SerializedProperty property)
			{
				return property.vector3Value;
			}
			public void SetValue(SerializedProperty property, Vector3 value)
			{
				property.vector3Value = value;
			}
		}
		class Vector3PropertyByQuaternion : IVector3Property
		{
			public static IVector3Property Global = new Vector3PropertyByQuaternion();
				
			public Vector3 GetValue(SerializedProperty property)
			{
				return property.quaternionValue.eulerAngles;
			}
			public void SetValue(SerializedProperty property, Vector3 value)
			{
				property.quaternionValue = Quaternion.Euler(value);
			}
		}
		static IVector3Property GetVector3PropertyDelegate(SerializedProperty property)
		{
			return typeof(Quaternion).Name == property.type ? Vector3PropertyByQuaternion.Global : Vector3Property.Global;
		}

		static Rect GetControlRectOneLine()
		{
			return EditorGUILayout.GetControlRect();
		}

		static void OnInspectorGUI_Vector3 (string btn, SerializedProperty property, Vector3 defaultValue, IVector3Property propertyDelegate)
		{
			bool changed;
			Vector3 newValue;

			var rect = GetControlRectOneLine();

			var btnRect = rect;
			btnRect.width = btnRect.height+3;
			if (GUI.Button(btnRect, btn))
			{
				newValue = defaultValue;
				changed = true;
			}
			else
			{
				var vectorRect = rect;
				vectorRect.x = btnRect.xMax+3;
				vectorRect.width -= btnRect.width+3;

				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
				newValue = EditorGUI.Vector3Field (vectorRect, "", propertyDelegate.GetValue(property));
				EditorGUI.showMixedValue = false;
				changed = EditorGUI.EndChangeCheck ();
			}

			if (changed)
			{
				propertyDelegate.SetValue(property, newValue);
			}
		}

		#region override
		public override void OnInspectorGUI ()
		{
			var spLocalPosition = serializedObject.FindProperty("m_LocalPosition");
			var spLocalRotation = serializedObject.FindProperty("m_LocalRotation");
			var spLocalScale = serializedObject.FindProperty("m_LocalScale");
			OnInspectorGUI_Vector3("P", spLocalPosition, Vector3.zero, Vector3Property.Global);
			OnInspectorGUI_Vector3("R", spLocalRotation, Vector3.zero, GetVector3PropertyDelegate(spLocalRotation));
			OnInspectorGUI_Vector3("S", spLocalScale, Vector3.one, Vector3Property.Global);
			serializedObject.ApplyModifiedProperties ();
//			if (GUILayout.Button("Test"))
//			{
//				var property = serializedObject.GetIterator();
//				property.Next(true);
//				do
//				{
//					Debug.LogFormat(property.name);
//				}while(property.NextVisible(false));
//			}
		}
		#endregion override
	}
} // namespace Ghost.EditorTool
