using UnityEngine;
using System.Collections;
using UnityEditor;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.MeshGeneration.Filters;
using UnityEditor.PostProcessing;
using System.Linq.Expressions;
using System;

public class FilterEditor
{
    public FilterBase target { get; internal set; }
    public SerializedObject serializedObject { get; internal set; }

    protected SerializedProperty m_SettingsProperty;

    internal bool alwaysEnabled = false;
    //internal PostProcessingProfile profile;
    //internal PostProcessingInspector inspector;

    internal void OnPreEnable()
    {
        m_SettingsProperty = serializedObject.FindProperty("m_Settings");

        OnEnable();
    }

    public virtual void OnEnable()
    { }

    public virtual void OnDisable()
    { }

    internal void OnGUI()
    {
        GUILayout.Space(5);

        //var display = alwaysEnabled
        //    ? EditorGUIHelper.Header(serializedProperty.displayName, m_SettingsProperty)
        //    : EditorGUIHelper.Header(serializedProperty.displayName, m_SettingsProperty, m_EnabledProperty, Reset);

        EditorGUI.indentLevel++;
        OnInspectorGUI();
        EditorGUI.indentLevel--;
    }

    void Reset()
    {
        var obj = serializedObject;
        Undo.RecordObject(obj.targetObject, "Reset");
        target.Reset();
        EditorUtility.SetDirty(obj.targetObject);
    }

    public virtual void OnInspectorGUI()
    { }

    public void Repaint()
    {
        //inspector.Repaint();
    }

    protected SerializedProperty FindSetting<T, TValue>(Expression<Func<T, TValue>> expr)
    {
        return m_SettingsProperty.FindPropertyRelative(ReflectionUtils.GetFieldPath(expr));
    }

    protected SerializedProperty FindSetting<T, TValue>(SerializedProperty prop, Expression<Func<T, TValue>> expr)
    {
        return prop.FindPropertyRelative(ReflectionUtils.GetFieldPath(expr));
    }
}
