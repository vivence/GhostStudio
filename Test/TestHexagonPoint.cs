using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Test
{
	public class TestHexagonPoint : MonoBehaviour 
	{
		public float pointRadius = 0.3f;
		public TestHexagonPoint[] others;

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
