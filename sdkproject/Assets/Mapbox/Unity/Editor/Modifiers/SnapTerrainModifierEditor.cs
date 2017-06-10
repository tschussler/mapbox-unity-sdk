using UnityEngine;
using System.Collections;
using Mapbox.Unity.MeshGeneration.Modifiers;
using UnityEditor.PostProcessing;
using UnityEditor;

[ModifierBaseEditorAttribute(typeof(SnapTerrainModifier))]
public class SnapTerrainModifierEditor : ModifierBaseEditor
{
	public override void OnEnable()
	{
	}

	public override void OnInspectorGUI()
	{
		GUILayout.Label("some info here");
	}
}
