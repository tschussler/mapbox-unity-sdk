using System;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.MeshGeneration.Data;

namespace Mapbox.Unity.MeshGeneration.Modifiers
{
	[Serializable]
    public class SmoothLineModifier : MeshModifier
    {
		[Serializable]
		public struct ModifierSettings
		{
			public int MaxEdgeSectionCount;
			public int PreferredEdgeSectionLength;

			public static ModifierSettings DefaultSettings
			{
				get
				{
					return new ModifierSettings
					{
						MaxEdgeSectionCount = 40,
						PreferredEdgeSectionLength = 10,
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
            for (int i = 0; i < feature.Points.Count; i++)
            {
                var nl = new List<Vector3>();
                for (int j = 1; j < feature.Points[i].Count; j++)
                {
                    nl.Add(feature.Points[i][j - 1]);
                    var dist = Vector3.Distance(feature.Points[i][j - 1], feature.Points[i][j]);
                    var step = Math.Min(Settings.MaxEdgeSectionCount, dist / Settings.PreferredEdgeSectionLength);
                    if (step > 1)
                    {
                        var counter = 1;
                        while (counter < step)
                        {
                            var nv = Vector3.Lerp(feature.Points[i][j - 1], feature.Points[i][j], Mathf.Min(1, counter / step));
                            nl.Add(nv);
                            counter++;
                        }
                    }
                    nl.Add(feature.Points[i][j]);
                }
                feature.Points[i] = nl;
            }
        }
    }
}
