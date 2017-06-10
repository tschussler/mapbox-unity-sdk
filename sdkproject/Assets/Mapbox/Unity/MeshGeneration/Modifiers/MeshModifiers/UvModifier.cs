namespace Mapbox.Unity.MeshGeneration.Modifiers
{
    using System.Collections.Generic;
    using UnityEngine;
    using Mapbox.Unity.MeshGeneration.Data;
	using System;

	/// <summary>
	/// UV Modifier works only with (and right after) Polygon Modifier and not with Line Mesh Modifier.
	/// If UseSatelliteRoof parameter is false, it creates a tiled UV map, otherwise it creates a stretched UV map.
	/// </summary>
	[Serializable]
    public class UvModifier : MeshModifier
    {
		[Serializable]
		public struct ModifierSettings
		{
			public bool UseSatelliteRoof;

			public static ModifierSettings DefaultSettings
			{
				get
				{
					return new ModifierSettings
					{
						UseSatelliteRoof = false
					};
				}
			}
		}

		[SerializeField]
		ModifierSettings m_Settings = ModifierSettings.DefaultSettings;
		public ModifierSettings Settings
		{
			get { return m_Settings; }
			set { m_Settings = value; }
		}

		public override void Reset()
		{
			m_Settings = ModifierSettings.DefaultSettings;
		}

		

        public override void Run(VectorFeatureUnity feature, MeshData md, UnityTile tile = null)
        {
            var uv = new List<Vector2>();
            foreach (var c in md.Vertices)
            {
                if (Settings.UseSatelliteRoof)
                {
                    var fromBottomLeft = new Vector2((float)((c.x + md.TileRect.Size.x / 2) / md.TileRect.Size.x),
                        (float)((c.z + md.TileRect.Size.x / 2) / md.TileRect.Size.x));
                    uv.Add(fromBottomLeft);
                }
                else
                {
                    uv.Add(new Vector2(c.x, c.z));
                }
            }
            md.UV[0].AddRange(uv);
        }
    }
}
