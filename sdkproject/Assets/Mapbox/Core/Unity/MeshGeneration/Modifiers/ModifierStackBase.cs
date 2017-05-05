using UnityEngine;
using System.Collections;
using Mapbox.Map;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Filters;
using System.Collections.Generic;

namespace Mapbox.Unity.MeshGeneration.Modifiers
{
    public abstract class ModifierStackBase : ScriptableObject
    {
        public List<MeshModifier> MeshModifiers;
        public List<GameObjectModifier> GoModifiers;

        public abstract GameObject Execute(UnityTile tile, VectorFeatureUnity feature, MeshData meshData, GameObject parent = null, string type = "");
    }
}