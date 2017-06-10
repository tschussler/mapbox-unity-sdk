namespace Mapbox.Unity.MeshGeneration.Modifiers
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using Mapbox.Unity.MeshGeneration.Data;
    using Mapbox.Unity.MeshGeneration.Components;

    /// <summary>
    /// Modifier Stack creates a game object from a feature using given modifiers.
    /// It runs mesh modifiers, creates the game object and then run the game object modifiers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mapbox/Modifiers/FeatureStyle")]
    public class FeatureStyle : ModifierStackBase
    {
		public List<ModifierBase> MeshModifiers = new List<ModifierBase>();

		public SmoothLineModifier SmoothLineModifier = new SmoothLineModifier();
		public SnapTerrainModifier SnapTerrainModifier = new SnapTerrainModifier();
		public PolygonMeshModifier PolygonMeshModifier = new PolygonMeshModifier();
		public UvModifier UvModifier = new UvModifier();
		public HeightModifier HeightModifier = new HeightModifier();

		public void OnEnable()
		{
			//MeshModifiers = new List<ModifierBase>();
			//MeshModifiers.Add(new SmoothLineModifier());
			//MeshModifiers.Add(new SnapTerrainModifier());
			//MeshModifiers.Add(new PolygonMeshModifier());
			//MeshModifiers.Add(new UvModifier());
			//MeshModifiers.Add(new HeightModifier());
			//MeshModifiers.Add(new TextureModifier());
			//MeshModifiers.Add(new ColliderModifier());
			//MeshModifiers.Add(new LayerModifier());
		}

		public override GameObject Execute(UnityTile tile, VectorFeatureUnity feature, MeshData meshData, GameObject parent = null, string type = "")
        {
            foreach (MeshModifier mod in MeshModifiers.Where(x => x is MeshModifier && x.Active))
            {
                mod.Run(feature, meshData, tile);
            }

            var go = CreateGameObject(meshData, parent);
            go.name = type + " - " + feature.Data.Id;
            var bd = go.AddComponent<FeatureBehaviour>();
            bd.Init(feature);

            foreach (GameObjectModifier mod in MeshModifiers.Where(x =>  x is GameObjectModifier && x.Active))
            {
                mod.Run(bd);
            }

            return go;
        }

        private GameObject CreateGameObject(MeshData data, GameObject main)
        {
            var go = new GameObject();
            var mesh = go.AddComponent<MeshFilter>().mesh;
            mesh.subMeshCount = data.Triangles.Count;

            mesh.SetVertices(data.Vertices);
            for (int i = 0; i < data.Triangles.Count; i++)
            {
                var triangle = data.Triangles[i];
                mesh.SetTriangles(triangle, i);
            }

            for (int i = 0; i < data.UV.Count; i++)
            {
                var uv = data.UV[i];
                mesh.SetUVs(i, uv);
            }

            mesh.RecalculateNormals();
            go.transform.SetParent(main.transform, false);

            return go;
        }
    }
}
