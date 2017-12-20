using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Test
{
	[ExecuteInEditMode]
	public class TestHexagon : MonoBehaviour 
	{
		public float pointRadius = 0.3f;
		public uint count = 0;
		public float gap = 1f;

		public TestHexagonPoint[] others;

		private List<TestHexagonPoint> points = new List<TestHexagonPoint>();

		void Update()
		{
			var pointCount = 6*count;
			if (points.Count > pointCount)
			{
				for (int i = points.Count-1; i >= pointCount; --i)
				{
					GameObject.DestroyImmediate(points[i].gameObject);
					points.RemoveAt(i);
				}
			}
			else if (points.Count < pointCount)
			{
				var increaseCount = pointCount-points.Count;
				for (int i = 0; i < increaseCount; ++i)
				{
					var go = new GameObject("point", typeof(TestHexagonPoint));
					go.transform.parent = transform;
					points.Add(go.GetComponent<TestHexagonPoint>());
				}
			}

			if (0 < points.Count)
			{
				for (int i = 0; i < points.Count;i+=6)
				{
					SetPoint(i, (i/6+1)*gap);
				}
			}
		}

		void SetPoint(int index, float radius)
		{
			var origin = transform.position;
			var rotation = transform.rotation;

			float pieceAngle = 60f;

			var p0 = origin + rotation * (Quaternion.identity * Vector3.forward * radius);
			var point = points[index++];
			point.transform.position = p0;
			point.name = string.Format("{0}_1", (index/6)+1);
			for (int i = 1; i < 6; ++i)
			{
				var r = Quaternion.Euler(0, pieceAngle*i, 0);
				Vector3 p = origin + rotation * (r * Vector3.forward * radius);
				point = points[index];
				point.name = string.Format("{0}_{1}", (index/6)+1, i+1);
				point.transform.position = p;

				++index;
			}
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(transform.position, pointRadius);

			if (null != others && 0 < others.Length)
			{
				for (int i = 0; i < others.Length; ++i)
				{
					if (null !=  others[i])
					{
						Gizmos.DrawLine(transform.position, others[i].transform.position);
					}
				}
			}
		}
	}
} // namespace Ghost.Test
