namespace Mapbox.Unity.MeshGeneration.Filters
{
    using UnityEngine;
    using Mapbox.Unity.MeshGeneration.Data;
    using System;

    [Serializable]
    public abstract class FilterBase : ScriptableObject
    {
        public abstract void Reset();

        public virtual void OnValidate()
        { }

        public virtual bool Try(VectorFeatureUnity feature)
        {
            return true;
        }
    }
}