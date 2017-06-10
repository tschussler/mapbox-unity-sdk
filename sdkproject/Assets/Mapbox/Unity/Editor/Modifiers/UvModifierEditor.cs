using UnityEngine;
using System.Collections;
using Mapbox.Unity.MeshGeneration.Modifiers;
using UnityEditor.PostProcessing;
using UnityEditor;

[ModifierBaseEditorAttribute(typeof(UvModifier))]
public class UvModifierEditor : ModifierBaseEditor
{
	SerializedProperty m_UseSatelliteUV;

	public override void OnEnable()
	{
		m_UseSatelliteUV = FindSetting((UvModifier.ModifierSettings x) => x.UseSatelliteRoof);
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(m_UseSatelliteUV);
	}
}
