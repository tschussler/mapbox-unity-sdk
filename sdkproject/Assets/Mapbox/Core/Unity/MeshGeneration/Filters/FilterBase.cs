namespace Mapbox.Unity.MeshGeneration.Filters
{
    using UnityEngine;
    using Mapbox.Unity.MeshGeneration.Data;
    using System;

    [Serializable]
    public class FilterBase : ScriptableObject
    {
        public virtual bool Try(VectorFeatureUnity feature) { return true; }
    }
}