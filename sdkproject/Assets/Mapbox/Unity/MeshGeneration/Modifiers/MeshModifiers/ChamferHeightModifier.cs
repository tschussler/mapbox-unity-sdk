namespace Mapbox.Unity.MeshGeneration.Modifiers
{
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.MeshGeneration.Data;

	[CreateAssetMenu(menuName = "Mapbox/Modifiers/Smooth Height for Buildings Modifier")]
	public class ChamferHeightModifier : MeshModifier
	{
		[SerializeField]
		private bool _flatTops;
		[SerializeField]
		private float _height;
		[SerializeField]
		private bool _forceHeight;
		[SerializeField]
		private float _offset;

		public override ModifierType Type { get { return ModifierType.Preprocess; } }

		public override void Run(VectorFeatureUnity feature, MeshData md, UnityTile tile = null)
		{
			if (md.Vertices.Count == 0 || feature == null || feature.Points.Count < 1)
				return;

			var originalVertexCount = md.Vertices.Count;
			Chamfer(feature, md, tile);

			Sides(feature, md, originalVertexCount);
		}

		private void Sides(VectorFeatureUnity feature, MeshData meshData, int originalVertexCount)
		{
			var minHeight = 0f;
			float hf = _height;

			if (!_forceHeight)
			{
				GetHeightData(feature, ref minHeight, ref hf);
			}

			var max = meshData.Vertices[0].y;
			var min = meshData.Vertices[0].y;
			if (_flatTops)
			{
				FlattenTops(meshData, minHeight, ref hf, ref max, ref min);
			}
			else
			{
				for (int i = 0; i < meshData.Vertices.Count; i++)
				{
					meshData.Vertices[i] = new Vector3(meshData.Vertices[i].x, meshData.Vertices[i].y + minHeight + hf, meshData.Vertices[i].z);
				}
			}

			float d = 0f;
			Vector3 v1;
			Vector3 v2 = Vector3.zero;
			int ind = 0;

			var wallTri = new List<int>();
			var wallUv = new List<Vector2>();
			meshData.Vertices.Add(new Vector3(meshData.Vertices[originalVertexCount - 1].x, meshData.Vertices[originalVertexCount - 1].y - hf, meshData.Vertices[originalVertexCount - 1].z));
			wallUv.Add(new Vector2(0, -hf));
			meshData.Normals.Add(meshData.Normals[originalVertexCount - 1]);

			for (int i = 0; i < meshData.Edges.Count; i += 2)
			{
				v1 = meshData.Vertices[meshData.Edges[i]];
				v2 = meshData.Vertices[meshData.Edges[i + 1]];
				ind = meshData.Vertices.Count;
				meshData.Vertices.Add(v1);
				meshData.Vertices.Add(v2);
				meshData.Vertices.Add(new Vector3(v1.x, v1.y - hf, v1.z));
				meshData.Vertices.Add(new Vector3(v2.x, v2.y - hf, v2.z));

				meshData.Normals.Add(meshData.Normals[meshData.Edges[i]]);
				meshData.Normals.Add(meshData.Normals[meshData.Edges[i + 1]]);
				meshData.Normals.Add(meshData.Normals[meshData.Edges[i]]);
				meshData.Normals.Add(meshData.Normals[meshData.Edges[i + 1]]);

				d = (v2 - v1).magnitude;

				wallUv.Add(new Vector2(0, 0));
				wallUv.Add(new Vector2(d, 0));
				wallUv.Add(new Vector2(0, -hf));
				wallUv.Add(new Vector2(d, -hf));

				wallTri.Add(ind);
				wallTri.Add(ind + 1);
				wallTri.Add(ind + 2);

				wallTri.Add(ind + 1);
				wallTri.Add(ind + 3);
				wallTri.Add(ind + 2);
			}

			meshData.Triangles.Add(wallTri);
			meshData.UV[0].AddRange(wallUv);
		}

		private static void FlattenTops(MeshData meshData, float minHeight, ref float hf, ref float max, ref float min)
		{
			for (int i = 0; i < meshData.Vertices.Count; i++)
			{
				if (meshData.Vertices[i].y > max)
					max = meshData.Vertices[i].y;
				else if (meshData.Vertices[i].y < min)
					min = meshData.Vertices[i].y;
			}
			for (int i = 0; i < meshData.Vertices.Count; i++)
			{
				meshData.Vertices[i] = new Vector3(meshData.Vertices[i].x, max + minHeight + hf, meshData.Vertices[i].z);
			}
			hf += max - min;
		}

		private static void GetHeightData(VectorFeatureUnity feature, ref float minHeight, ref float hf)
		{
			if (feature.Properties.ContainsKey("height"))
			{
				if (float.TryParse(feature.Properties["height"].ToString(), out hf))
				{
					if (feature.Properties.ContainsKey("min_height"))
					{
						minHeight = float.Parse(feature.Properties["min_height"].ToString());
						hf -= minHeight;
					}
				}
			}
			if (feature.Properties.ContainsKey("ele"))
			{
				if (float.TryParse(feature.Properties["ele"].ToString(), out hf))
				{
				}
			}
		}

		public void Chamfer(VectorFeatureUnity feature, MeshData md, UnityTile tile = null)
		{
			if (md.Vertices.Count == 0 || feature.Points.Count < 1)
				return;

			List<Vector3> newVertices = new List<Vector3>();
			List<Vector2> newUV = new List<Vector2>();
			md.Normals.Clear();
			md.Edges.Clear();

			for (int t = 0; t < md.Triangles[0].Count; t++)
			{
				md.Triangles[0][t] *= 3;
			}

			var i = 0; var j = 0; var k = 0;
			Vector3 v1, v2, n1, n2, pij1, pij2, pjk1, pjk2;
			Vector3 poi, close1, close2;
			for (int e = 0; e < md.PointEdges.Count; e++)
			{
				if (md.PointEdges[e][0] == -1)
				{
					newVertices.Add(Vector3.zero);
					newVertices.Add(Vector3.zero);
					newVertices.Add(Vector3.zero);
					newUV.Add(new Vector2(0, 0));
					newUV.Add(new Vector2(0, 0));
					newUV.Add(new Vector2(0, 0));
					md.Normals.Add(Constants.Math.Vector3Up);
					md.Normals.Add(Constants.Math.Vector3Up);
					md.Normals.Add(Constants.Math.Vector3Up);
					continue;
				}

				j = e;
				k = md.PointEdges[e][0];
				i = md.PointEdges[e][1];

				v1 = new Vector3(
						md.Vertices[j].x - md.Vertices[i].x, 0,
						md.Vertices[j].z - md.Vertices[i].z);
				v1.Normalize();
				v1 *= -_offset;
				n1 = new Vector3(-v1.z, 0, v1.x);

				pij1 = new Vector3(
					(float)(md.Vertices[i].x + n1.x), 0,
					(float)(md.Vertices[i].z + n1.z));
				pij2 = new Vector3(
					(float)(md.Vertices[j].x + n1.x), 0,
					(float)(md.Vertices[j].z + n1.z));

				v2 = new Vector3(
					md.Vertices[k].x - md.Vertices[j].x, 0,
					md.Vertices[k].z - md.Vertices[j].z);
				
				v2.Normalize();
				v2 *= -_offset;
				n2 = new Vector3(-v2.z, 0, v2.x);
				pjk1 = new Vector3(
					(float)(md.Vertices[j].x + n2.x), 0,
					(float)(md.Vertices[j].z + n2.z));
				pjk2 = new Vector3(
					(float)(md.Vertices[k].x + n2.x), 0,
					(float)(md.Vertices[k].z + n2.z));

				// See where the shifted lines ij and jk intersect.
				bool lines_intersect, segments_intersect;

				FindIntersection(pij1, pij2, pjk1, pjk2,
					out lines_intersect, out segments_intersect,
					out poi, out close1, out close2);

				newVertices.Add(poi + new Vector3(0, _offset / 2, 0));
				newVertices.Add(md.Vertices[j] + v1);
				newVertices.Add(md.Vertices[j] - v2);
				md.Normals.Add(Constants.Math.Vector3Up);
				md.Normals.Add(-n1);
				md.Normals.Add(-n2);
				newUV.Add(md.UV[0][j]);
				newUV.Add(md.UV[0][j]);
				newUV.Add(md.UV[0][j]);

				md.Triangles[0].Add(3 * j);
				md.Triangles[0].Add(3 * j + 1);
				md.Triangles[0].Add(3 * j + 2);

				md.Edges.Add(3 * j + 2);
				md.Edges.Add(3 * j + 1);

				md.Triangles[0].Add(3 * k);
				md.Triangles[0].Add(3 * j + 2);
				md.Triangles[0].Add(3 * k + 1);

				md.Triangles[0].Add(3 * j);
				md.Triangles[0].Add(3 * j + 2);
				md.Triangles[0].Add(3 * k);
				//Debug.Log(i + " - " + j + " - " + k);
				md.Edges.Add(3 * k + 1);
				md.Edges.Add(3 * j + 2);
			}

			md.Vertices = newVertices;
			md.UV[0] = newUV;
		}

		private List<Vector3> GetEnlargedPolygon(List<Vector3> old_points, float offset)
		{
			List<Vector3> enlarged_points = new List<Vector3>();
			int num_points = old_points.Count;
			for (int j = 0; j < num_points; j++)
			{
				// Find the new location for point j.
				// Find the points before and after j.
				int i = (j - 1);
				if (i < 0) i += num_points;
				int k = (j + 1) % num_points;

				// Move the points by the offset.
				Vector3 v1 = new Vector3(
					old_points[j].x - old_points[i].x, 0,
					old_points[j].z - old_points[i].z);
				v1.Normalize();
				v1 *= offset;
				Vector3 n1 = new Vector3(-v1.z, 0, v1.x);

				Vector3 pij1 = new Vector3(
					(float)(old_points[i].x + n1.x), 0,
					(float)(old_points[i].z + n1.z));
				Vector3 pij2 = new Vector3(
					(float)(old_points[j].x + n1.x), 0,
					(float)(old_points[j].z + n1.z));

				Vector3 v2 = new Vector3(
					old_points[k].x - old_points[j].x, 0,
					old_points[k].z - old_points[j].z);
				v2.Normalize();
				v2 *= offset;
				Vector3 n2 = new Vector3(-v2.z, 0, v2.x);

				Vector3 pjk1 = new Vector3(
					(float)(old_points[j].x + n2.x), 0,
					(float)(old_points[j].z + n2.z));
				Vector3 pjk2 = new Vector3(
					(float)(old_points[k].x + n2.x), 0,
					(float)(old_points[k].z + n2.z));

				// See where the shifted lines ij and jk intersect.
				bool lines_intersect, segments_intersect;
				Vector3 poi, close1, close2;
				FindIntersection(pij1, pij2, pjk1, pjk2,
					out lines_intersect, out segments_intersect,
					out poi, out close1, out close2);
				Debug.Assert(lines_intersect,
					"Edges " + i + "-->" + j + " and " +
					j + "-->" + k + " are parallel");

				enlarged_points.Add(poi);
			}

			return enlarged_points;
		}

		// Find the point of intersection between
		// the lines p1 --> p2 and p3 --> p4.
		private void FindIntersection(
			Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4,
			out bool lines_intersect, out bool segments_intersect,
			out Vector3 intersection,
			out Vector3 close_p1, out Vector3 close_p2)
		{
			// Get the segments' parameters.
			float dx12 = p2.x - p1.x;
			float dy12 = p2.z - p1.z;
			float dx34 = p4.x - p3.x;
			float dy34 = p4.z - p3.z;

			// Solve for t1 and t2
			float denominator = (dy12 * dx34 - dx12 * dy34);

			float t1 =
				((p1.x - p3.x) * dy34 + (p3.z - p1.z) * dx34)
					/ denominator;
			if (float.IsInfinity(t1))
			{
				// The lines are parallel (or close enough to it).
				lines_intersect = false;
				segments_intersect = false;
				intersection = new Vector3(float.NaN, 0, float.NaN);
				close_p1 = new Vector3(float.NaN, 0, float.NaN);
				close_p2 = new Vector3(float.NaN, 0, float.NaN);
				return;
			}
			lines_intersect = true;

			float t2 =
				((p3.x - p1.x) * dy12 + (p1.z - p3.z) * dx12)
					/ -denominator;

			// Find the point of intersection.
			intersection = new Vector3(p1.x + dx12 * t1, 0, p1.z + dy12 * t1);

			// The segments intersect if t1 and t2 are between 0 and 1.
			segments_intersect =
				((t1 >= 0) && (t1 <= 1) &&
				 (t2 >= 0) && (t2 <= 1));

			// Find the closest points on the segments.
			if (t1 < 0)
			{
				t1 = 0;
			}
			else if (t1 > 1)
			{
				t1 = 1;
			}

			if (t2 < 0)
			{
				t2 = 0;
			}
			else if (t2 > 1)
			{
				t2 = 1;
			}

			close_p1 = new Vector3(p1.x + dx12 * t1, 0, p1.z + dy12 * t1);
			close_p2 = new Vector3(p3.x + dx34 * t2, 0, p3.z + dy34 * t2);
		}
	}
}
