namespace Mapbox.Unity.MeshGeneration.Modifiers
{
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.MeshGeneration.Data;
	using System;
	using System.Linq;
	using Mapbox.VectorTile.Geometry.InteralClipperLib;

	/// <summary>
	/// Chamfer modifiers adds an extra vertex and a line segmet at each corner, making corners and line smoother.
	/// Generally used for smoother building meshes and should be used before Polygon Mesh Modifier.
	/// </summary>
	[CreateAssetMenu(menuName = "Mapbox/Modifiers/New Chamfer Modifier 1")]
	public class NewChamferModifier1 : MeshModifier
	{
		[SerializeField]
		private float _offset;

		public override void Run(VectorFeatureUnity feature, MeshData md, UnityTile tile = null)
		{
			if (md.Vertices.Count == 0 || feature.Points.Count > 1)
				return;

			for (int i = 0; i < md.Triangles[0].Count; i++)
			{
				md.Triangles[0][i] *= 3;
			}

			List<Vector3> newVertices = new List<Vector3>();
			int num_points = md.Vertices.Count - 1;
			for (int j = 0; j < num_points; j++)
			{
				// Find the new location for point j.
				// Find the points before and after j.
				int i = (j - 1);
				if (i < 0) i += num_points;
				int k = (j + 1) % num_points;

				// Move the points by the offset.
				Vector3 v1 = new Vector3(
					md.Vertices[j].x - md.Vertices[i].x, 0,
					md.Vertices[j].z - md.Vertices[i].z);
				v1.Normalize();
				v1 *= -_offset;
				Vector3 n1 = new Vector3(-v1.z, 0, v1.x);

				Vector3 pij1 = new Vector3(
					(float)(md.Vertices[i].x + n1.x), 0,
					(float)(md.Vertices[i].z + n1.z));
				Vector3 pij2 = new Vector3(
					(float)(md.Vertices[j].x + n1.x), 0,
					(float)(md.Vertices[j].z + n1.z));

				Vector3 v2 = new Vector3(
					md.Vertices[k].x - md.Vertices[j].x, 0,
					md.Vertices[k].z - md.Vertices[j].z);

				Vector3 poi, close1, close2;

				v2.Normalize();
				v2 *= -_offset;
				Vector3 n2 = new Vector3(-v2.z, 0, v2.x);
				Vector3 pjk1 = new Vector3(
					(float)(md.Vertices[j].x + n2.x), 0,
					(float)(md.Vertices[j].z + n2.z));
				Vector3 pjk2 = new Vector3(
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

				md.Triangles[0].Add(3 * j);
				md.Triangles[0].Add(3 * j + 1);
				md.Triangles[0].Add(3 * j + 2);

				if (j != 0)
				{
					md.Triangles[0].Add(3 * j);
					md.Triangles[0].Add(3 * j - 3);
					md.Triangles[0].Add(3 * j - 1);

					md.Triangles[0].Add(3 * j);
					md.Triangles[0].Add(3 * j - 1);
					md.Triangles[0].Add(3 * j + 1);
				}
				else
				{
					md.Triangles[0].Add(0);
					md.Triangles[0].Add(3 * num_points - 3);
					md.Triangles[0].Add(3 * num_points - 1);

					md.Triangles[0].Add(0);
					md.Triangles[0].Add(3 * num_points - 1);
					md.Triangles[0].Add(1);
				}
			}

			md.Vertices = newVertices;
			md.Edges.Clear();
			for (int i = 0; i < md.Vertices.Count; i+=3)
			{
				md.Edges.Add((i + 2) % md.Vertices.Count);
				md.Edges.Add(i+1);

				md.Edges.Add((i + 4) % md.Vertices.Count);
				md.Edges.Add((i + 2) % md.Vertices.Count);
			}
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
