using UnityEngine;
using System.Collections;
using Mapbox.Unity.MeshGeneration.Modifiers;
using UnityEditor.PostProcessing;
using UnityEditor;

[ModifierBaseEditorAttribute(typeof(PolygonMeshModifier))]
public class PolygonMeshModifierEditor : ModifierBaseEditor
{
	public override void OnEnable()
	{
		
	}

	public override void OnInspectorGUI()
	{
		GUILayout.Label("some info here");
	}
}
