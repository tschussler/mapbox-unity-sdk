namespace Mapbox.Unity.MeshGeneration.Filters
{
    using UnityEngine;
    using Mapbox.Unity.MeshGeneration.Data;
    using System;

    [Serializable]
    [CreateAssetMenu(menuName = "Mapbox/Filters/Type Filter")]
    public class StringPropertyFilter : FilterBase
    {
        public string PropertyName = "type";
        public string Value;
        public TypeFilterType Behaviour;

        public override bool Try(VectorFeatureUnity feature)
        {
            var check = Value.ToLowerInvariant().Contains(feature.Properties[PropertyName].ToString().ToLowerInvariant());
            return Behaviour == TypeFilterType.Include ? check : !check;
        }

        public enum TypeFilterType
        {
            Include,
            Exclude
        }
    }
}