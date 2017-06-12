namespace Mapbox.Unity.MeshGeneration.Modifiers
{
    using Mapbox.Unity.MeshGeneration.Data;
	using System;

	public enum ModifierType
    {
        Preprocess,
        Postprocess
    }

	[Serializable]
    public class MeshModifier : ModifierBase
    {
        public virtual ModifierType Type { get { return ModifierType.Preprocess; } }

        public virtual void Run(VectorFeatureUnity feature, MeshData md, UnityTile tile = null)
        {

        }
    }
}