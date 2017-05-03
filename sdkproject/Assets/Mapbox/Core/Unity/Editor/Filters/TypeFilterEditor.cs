using UnityEngine;
using System.Collections;
using UnityEditor;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.MeshGeneration.Filters;
using Settings = Mapbox.Unity.MeshGeneration.Filters.TypeFilter.TypeSettings;

[CustomEditor(typeof(TypeFilter))]
public class TypeFilterEditor : FilterEditor
{
    SerializedProperty _key;
    SerializedProperty _type;
    SerializedProperty _behaviour;

    public override void OnEnable()
    {
        _key = FindSetting((Settings x) => x.Key);
        _type = FindSetting((Settings x) => x.Type);
        _behaviour = FindSetting((Settings x) => x.Behaviour);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(_key);
        EditorGUILayout.PropertyField(_type);
        EditorGUILayout.PropertyField(_behaviour);
        //EditorGUILayout.PropertyField(m_Downsampling);
        //EditorGUILayout.PropertyField(m_ForceForwardCompatibility);
        //EditorGUILayout.PropertyField(m_HighPrecision, EditorGUIHelper.GetContent("High Precision (Forward)"));

        //using (new EditorGUI.DisabledGroupScope(m_ForceForwardCompatibility.boolValue))
        //    EditorGUILayout.PropertyField(m_AmbientOnly, EditorGUIHelper.GetContent("Ambient Only (Deferred + HDR)"));
    }
}
