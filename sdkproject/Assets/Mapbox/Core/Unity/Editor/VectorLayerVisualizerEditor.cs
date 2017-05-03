using UnityEngine;
using System.Collections;
using UnityEditor;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.MeshGeneration.Interfaces;
using Mapbox.Unity.MeshGeneration.Modifiers;
using Mapbox.Unity.MeshGeneration.Filters;
using System.Collections.Generic;
using System;

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

    void OnEnable()
    {
        script = MonoScript.FromScriptableObject((VectorLayerVisualizer)target);
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
                showPosition[i] = EditorGUILayout.Foldout(showPosition[i], header);
                if (showPosition[i])
                {
                    var ele = _visualizer.Stacks[i];
                    if (ele != null)
                    {
                        _visualizer.Stacks[i].Filter = (FilterBase)EditorGUILayout.ObjectField(_visualizer.Stacks[i].Filter, typeof(FilterBase));
                        if (_visualizer.Stacks[i].Filter != null)
                        {
                            EditorGUI.indentLevel += 2;
                            EditorGUILayout.LabelField("Filter details");
                            EditorGUI.indentLevel -= 2;
                        }
                    }
                    _visualizer.Stacks[i] = (ModifierStackBase)EditorGUILayout.ObjectField(_visualizer.Stacks[i], typeof(ModifierStackBase));
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Stack details");
                    EditorGUI.indentLevel--;

                    if (GUILayout.Button("Delete"))
                    {
                        _visualizerList.DeleteArrayElementAtIndex(i);
                        showPosition.RemoveAt(i);
                        break;
                    }
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
}
