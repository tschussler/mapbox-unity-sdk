using UnityEngine;
using System.Collections;
using UnityEditor;
using Mapbox.Unity.MeshGeneration.Modifiers;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor.PostProcessing;
using System;

[CustomEditor(typeof(FeatureStyle))]
public class FeatureStyleEditor : Editor
{
	Dictionary<ModifierBaseEditor, ModifierBase> m_CustomEditors = new Dictionary<ModifierBaseEditor, ModifierBase>();

	void OnEnable()
	{
		if (target == null)
			return;

		// Aggregate custom post-fx editors
		var assembly = Assembly.GetAssembly(typeof(FeatureStyleEditor));

		var editorTypes = assembly.GetTypes()
			.Where(x => x.IsDefined(typeof(ModifierBaseEditorAttribute), false));

		var customEditors = new Dictionary<Type, ModifierBaseEditor>();
		foreach (var editor in editorTypes)
		{
			var attr = (ModifierBaseEditorAttribute)editor.GetCustomAttributes(typeof(ModifierBaseEditorAttribute), false)[0];
			var effectType = attr.type;
			var alwaysEnabled = attr.alwaysEnabled;

			var editorInst = (ModifierBaseEditor)Activator.CreateInstance(editor);
			editorInst.alwaysEnabled = alwaysEnabled;
			editorInst.profile = target as FeatureStyle;
			editorInst.inspector = this;
			customEditors.Add(effectType, editorInst);
		}

		var baseType = target.GetType();
		var property = serializedObject.GetIterator();

		while (property.Next(true))
		{
			if (!property.hasChildren)
				continue;

			var type = baseType;
			var srcObject = ReflectionUtils.GetFieldValueFromPath(serializedObject.targetObject, ref type, property.propertyPath);

			if (srcObject == null)
				continue;

			ModifierBaseEditor editor;
			if (customEditors.TryGetValue(type, out editor))
			{
				var effect = (ModifierBase)srcObject;

				if (editor.alwaysEnabled)
					effect.enabled = editor.alwaysEnabled;

				m_CustomEditors.Add(editor, effect);
				editor.target = effect;
				editor.serializedProperty = property.Copy();
				editor.OnPreEnable();
			}
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		// Handles undo/redo events first (before they get used by the editors' widgets)
		var e = Event.current;
		if (e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed")
		{
			foreach (var editor in m_CustomEditors)
				editor.Value.OnValidate();
		}

		foreach (var editor in m_CustomEditors)
		{
			EditorGUI.BeginChangeCheck();

			editor.Key.OnGUI();

			if (EditorGUI.EndChangeCheck())
				editor.Value.OnValidate();
		}
		
		serializedObject.ApplyModifiedProperties();
	}
}