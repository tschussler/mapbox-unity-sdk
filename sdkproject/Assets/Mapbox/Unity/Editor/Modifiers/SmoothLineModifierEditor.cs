using UnityEngine;
using System.Collections;
using Mapbox.Unity.MeshGeneration.Modifiers;
using UnityEditor.PostProcessing;
using UnityEditor;

[ModifierBaseEditorAttribute(typeof(SmoothLineModifier))]
public class SmoothLineModifierEditor : ModifierBaseEditor
{
	SerializedProperty m_MaxEdgeSectionCount;
	SerializedProperty m_PreferredEdgeSectionLength;

	public override void OnEnable()
	{
		m_MaxEdgeSectionCount = FindSetting((SmoothLineModifier.ModifierSettings x) => x.MaxEdgeSectionCount);
		m_PreferredEdgeSectionLength = FindSetting((SmoothLineModifier.ModifierSettings x) => x.PreferredEdgeSectionLength);
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(m_MaxEdgeSectionCount);
		EditorGUILayout.PropertyField(m_PreferredEdgeSectionLength);
	}
}
