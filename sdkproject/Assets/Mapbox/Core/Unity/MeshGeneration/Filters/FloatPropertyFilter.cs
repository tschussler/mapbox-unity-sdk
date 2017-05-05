namespace Mapbox.Unity.MeshGeneration.Filters
{
    using UnityEngine;
    using Mapbox.Unity.MeshGeneration.Data;

    [CreateAssetMenu(menuName = "Mapbox/Filters/Float Comparator Filter")]
    public class FloatPropertyFilter : FilterBase
    {
        public enum FloatPropertyFilterOptions
        {
            Above,
            Equals,
            Below
        }

        public string PropertyName = "Height";
        public float Value;
        public FloatPropertyFilterOptions Type;

        public override bool Try(VectorFeatureUnity feature)
        {
            var hg = System.Convert.ToSingle(feature.Properties[PropertyName]);
            if (Type == FloatPropertyFilterOptions.Above && hg > Value)
                return true;
            else if (Type == FloatPropertyFilterOptions.Below && hg < Value)
                return true;
            else if (Type == FloatPropertyFilterOptions.Equals && hg == Value)
                return true;

            return false;
        }
    }
}
