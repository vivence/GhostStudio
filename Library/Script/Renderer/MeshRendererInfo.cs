using UnityEngine;
using System.Collections.Generic;
using Ghost.Extension;

namespace Ghost
{
	[RequireComponent(typeof(MeshRenderer))]
	public class MeshRendererInfo : MonoBehaviour 
	{ 
		#region behaviour
		public bool showVertices = true;
		public Color verticesColor = Color.green;
		public float verticesSize = 0.01f;

		public bool showNormals = true;
		public Color normalsColor = Color.blue;
		public float normalsLength = 0.1f;

		public bool showTangents = true;
		public Color tangentsColor = Color.red;
		public float tangentsLength = 0.1f;
		private void DebugDraw()
		{
			var f = GetComponent<MeshFilter>();
			if (null == f)
			{
				return;
			}
			var mesh = f.sharedMesh;
			if (null == mesh)
			{
				return;
			}

			// draw vertices
			var vertices = mesh.vertices;
			if (!vertices.IsNullOrEmpty())
			{
				var colors = mesh.colors;
				var normals = mesh.normals;
				var tangents = mesh.tangents;
				for (int i = 0; i < vertices.Length; ++i)
				{
					var v = transform.TransformPoint(vertices[i]);
					if (showVertices)
					{
						if (colors.IsNullOrEmpty())
						{
							Gizmos.color = verticesColor;
						}
						else
						{
							Gizmos.color = colors[i];
						}
						Gizmos.DrawSphere(v, verticesSize);
					}
					if (showNormals && !normals.IsNullOrEmpty())
					{
						Gizmos.color = normalsColor;
						Gizmos.DrawLine(v, v+normals[i]*normalsLength);
					}
					if (showTangents && !tangents.IsNullOrEmpty())
					{
						Gizmos.color = tangentsColor;
						var tangent = new Vector3(tangents[i].x, tangents[i].y, tangents[i].z);
						Gizmos.DrawLine(v, v+tangent*tangentsLength);
					}
				}
			}
		}

		void OnDrawGizmos()
		{
			DebugDraw();
		}

		void OnDrawGizmosSelected()
		{
			DebugDraw();
		}
		#endregion behaviour
	}
} // namespace Ghost
