using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Ghost.Test
{
	[ExecuteInEditMode]
	public class TestLineRenderer : MonoBehaviour 
	{
		public LineRenderer lineRenderer;
		public Transform target;

		public int pCount = 10;
		public float A1 = 1;

		private void SetPoint(int i, Vector3 p1, Vector3 p2, float progress)
		{
			var x = (progress*2-1);
			var y = (x*x-1)*A1;

			var p = Vector3.Lerp(p1, p2, progress);
			p.y += y;
			lineRenderer.SetPosition(i, p);
		}

		#region behavoir
		void Update()
		{
			if (null == lineRenderer)
			{
				return;
			}

			if (null == target || 2 > pCount)
			{
				lineRenderer.SetVertexCount(0);
				return;
			}

			var p1 = Vector3.zero;
			var p2 = target.position;
			if (lineRenderer.useWorldSpace)
			{
				p1 = lineRenderer.transform.position;
			}
			else
			{
				p2 = lineRenderer.transform.InverseTransformPoint(p2);
			}

			lineRenderer.SetVertexCount(pCount);

			var progressPart = 1f / pCount;

			var lastIndex = pCount-1;
			for (int i = 0; i < lastIndex; ++i)
			{
				SetPoint(i, p1, p2, progressPart*i);
			}

			SetPoint(lastIndex, p1, p2, 1f);
		}
		#endregion behavoir
	}
} // namespace Ghost.Test
