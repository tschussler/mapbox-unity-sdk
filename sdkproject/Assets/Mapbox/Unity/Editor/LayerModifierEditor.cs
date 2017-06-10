using UnityEngine;
using System.Collections;
using UnityEditor;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.MeshGeneration.Modifiers;

[CustomEditor(typeof(LayerModifier))]
public class LayerModifierEditor : Editor
{
    public SerializedProperty layerId_Prop;

    void OnEnable()
    {
        layerId_Prop = serializedObject.FindProperty("_layerId");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        layerId_Prop.intValue = EditorGUILayout.LayerField("Layer", layerId_Prop.intValue);

        serializedObject.ApplyModifiedProperties();
    }
}
