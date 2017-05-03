namespace Mapbox.Unity.MeshGeneration.Filters
{
    using UnityEngine;
    using Mapbox.Unity.MeshGeneration.Data;
    using System;

    [Serializable]
    [CreateAssetMenu(menuName = "Mapbox/Filters/Type Filter")]
    public class TypeFilter : FilterBase
    {
        [Serializable]
        public struct TypeSettings
        {
            public string Key;
            public string Type;
            public TypeFilterType Behaviour;

            public static TypeSettings DefaultSettings
            {
                get
                {
                    return new TypeSettings
                    {
                        Key = "type",
                        Type = "",
                        Behaviour = TypeFilterType.Exclude
                    };
                }
            }
        }

        [SerializeField]
        TypeSettings m_Settings = TypeSettings.DefaultSettings;
        public TypeSettings Settings
        {
            get { return m_Settings; }
            set { m_Settings = value; }
        }

        public override void Reset()
        {
            m_Settings = TypeSettings.DefaultSettings;
        }

        //public override string Key { get { return "type"; } }
        //[SerializeField]
        //private string _type;
        //[SerializeField]
        //private TypeFilterType _behaviour;

        public override bool Try(VectorFeatureUnity feature)
        {
            var check = Settings.Type.ToLowerInvariant().Contains(feature.Properties["type"].ToString().ToLowerInvariant());
            return Settings.Behaviour == TypeFilterType.Include ? check : !check;
        }

        public enum TypeFilterType
        {
            Include,
            Exclude
        }
    }
}