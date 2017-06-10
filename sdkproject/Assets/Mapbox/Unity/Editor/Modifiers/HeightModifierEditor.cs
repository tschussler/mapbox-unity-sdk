using UnityEngine;
using System.Collections;
using Mapbox.Unity.MeshGeneration.Modifiers;
using UnityEditor.PostProcessing;
using UnityEditor;

[ModifierBaseEditorAttribute(typeof(HeightModifier))]
public class HeightModifierEditor : ModifierBaseEditor
{
	SerializedProperty m_FlatTops;
	SerializedProperty m_Height;
	SerializedProperty m_ForceHeight;

	public override void OnEnable()
	{
		m_FlatTops = FindSetting((HeightModifier.ModifierSettings x) => x.FlatTops);
		m_Height = FindSetting((HeightModifier.ModifierSettings x) => x.Height);
		m_ForceHeight = FindSetting((HeightModifier.ModifierSettings x) => x.ForceHeight);
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(m_FlatTops);
		EditorGUILayout.PropertyField(m_Height);
		EditorGUILayout.PropertyField(m_ForceHeight);
	}
}
