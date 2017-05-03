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
    private VectorLayerVisualizer _visualizer;
    List<bool> showPosition;
    string header;
    private Type typ;
    private int hash;

    private Dictionary<Type, Type> _filterEditorDict;
    private Dictionary<int, FilterEditor> _filterEditors;
    private Dictionary<int, SerializedObject> _stackObj;


    void OnEnable()
    {
        script = MonoScript.FromScriptableObject((VectorLayerVisualizer)target);
        _filterEditors = new Dictionary<int, FilterEditor>();
        _stackObj = new Dictionary<int, SerializedObject>();
        _filterEditorDict = new Dictionary<Type, Type>()
        {
            {typeof(Mapbox.Unity.MeshGeneration.Filters.TypeFilter), typeof(TypeFilterEditor) }
        };
        _visualizer = target as VectorLayerVisualizer;
        _visualizerList = serializedObject.FindProperty("Stacks");
        showPosition = new List<bool>(_visualizer.Stacks.Count);
    }

    public override void OnInspectorGUI()
    {
        if (_visualizer == null)
            return;

        serializedObject.Update();

        if (showPosition.Count != _visualizer.Stacks.Count)
        {
            showPosition.Clear();
            for (int i = 0; i < _visualizer.Stacks.Count; i++)
            {
                showPosition.Add(false);
            }
        }


        if (_visualizerList != null)
        {
            EditorGUILayout.LabelField("Modifier Stacks");
            for (int i = 0; i < _visualizer.Stacks.Count; i++)
            {
                header = _visualizer.Stacks[i] != null ? _visualizer.Stacks[i].name : "Assign a modifier stack";
                var ele = _visualizer.Stacks[i];

                var ind = i;
                if (Header(header, _visualizerList.GetArrayElementAtIndex(i), null, () =>
                {
                    _visualizerList.DeleteArrayElementAtIndex(ind);
                    _visualizer.Stacks.RemoveAt(ind);
                }))
                {
                    EditorGUI.indentLevel += 2;
                    
                    if (ele != null)
                    {
                        if (!_stackObj.ContainsKey(ele.GetHashCode()))
                            _stackObj.Add(ele.GetHashCode(), new SerializedObject(ele));

                        _visualizer.Stacks[i].Filter = (FilterBase)EditorGUILayout.ObjectField(_visualizer.Stacks[i].Filter, typeof(FilterBase));
                        if (_visualizer.Stacks[i].Filter != null)
                        {
                            var filterType = _visualizer.Stacks[i].Filter.GetType();
                            if (_filterEditorDict.ContainsKey(filterType))
                            {
                                DrawEditorFor(i, _visualizer.Stacks[i].Filter);
                            }
                            EditorUtility.SetDirty(ele);
                            _stackObj[ele.GetHashCode()].ApplyModifiedProperties();
                        }
                    }


                    _visualizer.Stacks[i] = (ModifierStackBase)EditorGUILayout.ObjectField(_visualizer.Stacks[i], typeof(ModifierStackBase));

                    if (ele != null)
                    {
                        EditorGUI.indentLevel += 2;
                        foreach (var item in ele.MeshModifiers)
                        {
                            EditorGUILayout.LabelField(item.name);
                        }
                        EditorGUI.indentLevel -= 2;
                    }

                    EditorGUI.indentLevel -= 2;
                }
            }
        }


        if (GUILayout.Button("Add New Visualizer"))
        {
            _visualizer.Stacks.Add(null);
            showPosition.Add(false);
        }
        EditorUtility.SetDirty(_visualizer);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawEditorFor(int index, FilterBase filter)
    {
        EditorGUI.indentLevel += 2;
        var editorType = _filterEditorDict[filter.GetType()];
        int hash = filter.GetHashCode();
        if (!_filterEditors.ContainsKey(hash))
        {
            var editIns = (TypeFilterEditor)Activator.CreateInstance(editorType);
            editIns.target = filter;
            _filterEditors.Add(filter.GetHashCode(), editIns);
            editIns.serializedObject = new SerializedObject(filter);
            editIns.OnPreEnable();
        }
        else
        {
            _filterEditors[hash].OnInspectorGUI();
            EditorUtility.SetDirty(filter);
            _filterEditors[hash].serializedObject.ApplyModifiedProperties();
        }
        EditorGUI.indentLevel -= 2;
    }


    public bool Header(string title, SerializedProperty group, SerializedProperty enabledField, Action resetAction)
    {
        //var field = ReflectionUtils.GetFieldInfoFromPath(enabledField.serializedObject.targetObject, enabledField.propertyPath);
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
