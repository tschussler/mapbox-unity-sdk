using UnityEngine;
using System.Collections;
using UnityEditor;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.MeshGeneration.Interfaces;
using Mapbox.Unity.MeshGeneration.Modifiers;
using Mapbox.Unity.MeshGeneration.Filters;
using System.Collections.Generic;
using System;
using UnityEditor.PostProcessing;
using System.Reflection;

[CustomEditor(typeof(VectorLayerVisualizer))]
public class VectorLayerVisualizerEditor : Editor
{
    private MonoScript script;
    private SerializedProperty _visualizerList;
    private SerializedProperty _filterList;
    private SerializedProperty _key;
    private VectorLayerVisualizer _visualizer;
    string header;
    private Type typ;
    private int hash;

    void OnEnable()
    {
        script = MonoScript.FromScriptableObject((VectorLayerVisualizer)target);
        _visualizer = target as VectorLayerVisualizer;
        _visualizerList = serializedObject.FindProperty("Stacks");
        _filterList = serializedObject.FindProperty("Filters");
        _key = serializedObject.FindProperty("_key");
    }

    public override void OnInspectorGUI()
    {
        if (_visualizer == null)
            return;

        serializedObject.Update();

        GUI.enabled = false;
        script = EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false) as MonoScript;
        GUI.enabled = true;
        EditorGUILayout.PropertyField(_key, new GUIContent("Layer Name"));

        EditorGUILayout.Space();
        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filters");
        if (GUILayout.Button("+"))
        {
            _visualizer.Filters.Add(null);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        if (_visualizer.Filters != null)
        {
            for (int i = 0; i < _visualizer.Filters.Count; i++)
            {
                var ind = i;
                var filt = _visualizer.Filters[ind];
                header = filt != null ? filt.name : "Assign a filter";
                if (Header(header, _filterList.GetArrayElementAtIndex(ind), null, () =>
                {
                    _filterList.DeleteArrayElementAtIndex(ind);
                    _visualizer.Filters.RemoveAt(ind);
                }))
                {
                    EditorGUI.indentLevel++;
                    if (filt != null)
                    {
                        var ed = CreateEditor(filt);
                        ed.OnInspectorGUI();
                    }
                    else
                    {
                        _visualizer.Filters[ind] = (FilterBase)EditorGUILayout.ObjectField(_visualizer.Filters[i], typeof(FilterBase));
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        if (_visualizerList != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Modifier Stacks");
            if (GUILayout.Button("+"))
            {
                _visualizer.Stacks.Add(null);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            for (int i = 0; i < _visualizer.Stacks.Count; i++)
            {
                header = (_visualizer.Stacks[i] != null && _visualizer.Stacks[i].Stack != null) ? _visualizer.Stacks[i].Stack.name : "Assign a modifier stack";
                var ele = _visualizer.Stacks[i].Stack;
                
                var ind = i;
                if (Header(header, _visualizerList.GetArrayElementAtIndex(i), null, () =>
                {
                    _visualizerList.DeleteArrayElementAtIndex(ind);
                    _visualizer.Stacks.RemoveAt(ind);
                }))
                {
                    EditorGUI.indentLevel++;
                    if (ele != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Filter", GUILayout.Width(40));
                        _visualizer.Stacks[i].Filter = (FilterBase)EditorGUILayout.ObjectField(_visualizer.Stacks[i].Filter, typeof(FilterBase));
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel ++;
                        if (_visualizer.Stacks[i].Filter != null)
                        {
                            var filterType = _visualizer.Stacks[i].Filter.GetType();
                            EditorGUILayout.Space();
                            var ed = CreateEditor(_visualizer.Stacks[i].Filter);
                            ed.OnInspectorGUI();
                            EditorUtility.SetDirty(ele);

                            if (GUILayout.Button("Remove Filter"))
                            {
                                _visualizer.Stacks[i].Filter = null;
                            }

                        }
                        EditorGUI.indentLevel --;
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stack", GUILayout.Width(40));
                    _visualizer.Stacks[i].Stack = (ModifierStackBase)EditorGUILayout.ObjectField(_visualizer.Stacks[i].Stack, typeof(ModifierStackBase));
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel++;
                    if (ele != null)
                    {
                        EditorGUILayout.Space();
                        var editor = CreateEditor(ele);
                        editor.OnInspectorGUI();
                    }
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
            }
        }

        EditorGUILayout.Space();
        
        EditorUtility.SetDirty(_visualizer);
        serializedObject.ApplyModifiedProperties();
    }

    public bool Header(string title, SerializedProperty group, SerializedProperty enabledField, Action resetAction)
    {
        object parent = null;
        PropertyInfo prop = null;

        var display = group == null || group.isExpanded;

        var rect = GUILayoutUtility.GetRect(16f, 22f, FxStyles.header);
        GUI.Box(rect, title, FxStyles.header);

        var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);
        var e = Event.current;

        var popupRect = new Rect(rect.x + rect.width - FxStyles.paneOptionsIcon.width - 5f, rect.y + FxStyles.paneOptionsIcon.height / 2f + 1f, FxStyles.paneOptionsIcon.width, FxStyles.paneOptionsIcon.height);
        GUI.DrawTexture(popupRect, FxStyles.paneOptionsIcon);

        if (e.type == EventType.Repaint)
            FxStyles.headerFoldout.Draw(toggleRect, false, false, display, false);

        if (e.type == EventType.MouseDown)
        {
            const float kOffset = 2f;
            toggleRect.x -= kOffset;
            toggleRect.y -= kOffset;
            toggleRect.width += kOffset * 2f;
            toggleRect.height += kOffset * 2f;

            if (toggleRect.Contains(e.mousePosition))
            {
                if (prop != null)
                    prop.SetValue(parent, enabledField.boolValue, null);

                e.Use();
            }
            else if (popupRect.Contains(e.mousePosition))
            {
                var popup = new GenericMenu();

                popup.AddItem(new GUIContent("Delete"), false, () => resetAction());
                popup.ShowAsContext();
            }
            else if (rect.Contains(e.mousePosition) && group != null)
            {
                display = !display;
                group.isExpanded = !group.isExpanded;
                e.Use();
            }
        }

        return display;
    }
}
