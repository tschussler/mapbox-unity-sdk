using System;

namespace UnityEditor.PostProcessing
{
    public class ModifierBaseEditorAttribute : Attribute
    {
        public readonly Type type;
        public readonly bool alwaysEnabled;

        public ModifierBaseEditorAttribute(Type type, bool alwaysEnabled = false)
        {
            this.type = type;
            this.alwaysEnabled = alwaysEnabled;
        }
    }
}
