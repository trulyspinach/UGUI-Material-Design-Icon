using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MaterialDesignIcon
{

	[CanEditMultipleObjects]
	[CustomEditor(typeof(VectorIcon))]
	public class VectorIconInspector : Editor
	{

		private SerializedProperty m_Size;
		private SerializedProperty m_Color;
		private VectorIcon icon;

		void OnEnable()
		{
			m_Size = serializedObject.FindProperty("m_Size");
			m_Color = serializedObject.FindProperty("m_Color");
			icon = target as VectorIcon;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(m_Size);
			EditorGUILayout.PropertyField(m_Color);
			icon.UpdateScale();
			if (GUILayout.Button("Pick an Icon")) IconPickerWindow.Show(icon.text,(t) => icon.text = t);
			if (GUILayout.Button("Pick Material Design Color")) ColorPickerWindow.Show(icon.color, (t) => icon.color = t);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
