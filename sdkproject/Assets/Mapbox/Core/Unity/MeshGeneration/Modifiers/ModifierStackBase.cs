using UnityEngine;
using System.Collections;
using Mapbox.Map;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Filters;

namespace Mapbox.Unity.MeshGeneration.Modifiers
{
    public abstract class ModifierStackBase : ScriptableObject
    {
        public FilterBase Filter;

        public abstract GameObject Execute(UnityTile tile, VectorFeatureUnity feature, MeshData meshData, GameObject parent = null, string type = "");
    }
}