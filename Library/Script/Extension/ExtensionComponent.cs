using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Extension
{
	public static class S_Component
	{
		public static T FindComponentInChildren<T>(this Component root) where T:Component
		{
			var c = root.GetComponent<T>();
			if (null != c)
			{
				return c;
			}
			var transform = root.transform;
			var childCount = transform.childCount;
			for (int i = 0; i < childCount; ++i)
			{
				var child = transform.GetChild(i);
				c = child.FindComponentInChildren<T>();
				if (null != c)
				{
					return c;
				}
			}
			return null;
		}

		public static T[] FindComponentsInChildren<T>(this Component root) where T:Component
		{
			var components = new List<T>();

			var parents = new List<Transform>();
			parents.Add(root.transform);
			var nextParents = new List<Transform>();

			while (0 < parents.Count)
			{
				foreach (var parent in parents)
				{
					var component = parent.GetComponent<T>();
					if (null != component)
					{
						components.Add(component);
					}

					var childCount = parent.childCount;
					for (int i = 0; i < childCount; ++i)
					{
						nextParents.Add(parent.GetChild(i));
					}
				}
				parents.Clear();
				var temp = parents;
				parents = nextParents;
				nextParents = temp;
			}

			return components.ToArray();
		}
	}
} // namespace Ghost.Extension
