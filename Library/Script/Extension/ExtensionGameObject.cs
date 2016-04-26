using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Extension
{
	public static class ExtensionGameObject
	{
		public static void FindGameObjectsInChildren(this GameObject obj, System.Predicate<GameObject> pred, ref List<GameObject> list)
		{
			if (pred(obj))
			{
				list.Add(obj);
			}

			var parentTransform = obj.transform;
			var childCount = parentTransform.childCount;
			if (0 < childCount)
			{
				for (int i = 0; i < childCount; ++i)
				{
					var childObj = parentTransform.GetChild(i).gameObject;
					childObj.FindGameObjectsInChildren(pred, ref list);
				}
			}
		}

		public static void FindGameObjectsInChildrenWithName(this GameObject obj, string name, ref List<GameObject> list)
		{
			FindGameObjectsInChildren(obj, delegate(GameObject go) {
				return string.Equals(go.name, name);
			}, ref list);
		}

		public static void FindGameObjectsInChildrenWithTag(this GameObject obj, string tag, ref List<GameObject> list)
		{
			FindGameObjectsInChildren(obj, delegate(GameObject go) {
				return go.tag == tag;
			}, ref list);
		}

		public static GameObject[] FindGameObjectsInChildren(this GameObject obj, System.Predicate<GameObject> pred)
		{
			var objs = new List<GameObject>();
			obj.FindGameObjectsInChildren(pred, ref objs);
			return objs.ToArray();
		}

		public static GameObject[] FindGameObjectsInChildrenWithName(this GameObject obj, string name)
		{
			return obj.FindGameObjectsInChildren(delegate(GameObject go) {
				return string.Equals(go.name, name);
			});
		}

		public static GameObject[] FindGameObjectsInChildrenWithTag(this GameObject obj, string tag)
		{
			return obj.FindGameObjectsInChildren(delegate(GameObject go) {
				return go.tag == tag;
			});
		}

		public static GameObject FindGameObjectInChildren(this GameObject obj, System.Predicate<GameObject> pred)
		{
			if (pred(obj))
			{
				return obj;
			}

			var parentTransform = obj.transform;
			var childCount = parentTransform.childCount;
			if (0 < childCount)
			{
				for (int i = 0; i < childCount; ++i)
				{
					var childObj = parentTransform.GetChild(i).gameObject;
					var ret = childObj.FindGameObjectInChildren(pred);
					if (null != ret)
					{
						return ret;
					}
				}
			}
			return null;
		}

		public static GameObject FindGameObjectInChildrenWithName(this GameObject obj, string name)
		{
			return obj.FindGameObjectInChildren(delegate(GameObject go) {
				return string.Equals(go.name, name);
			});
		}

		public static GameObject FindGameObjectInChildrenWithTag(this GameObject obj, string tag)
		{
			return obj.FindGameObjectInChildren(delegate(GameObject go) {
				return go.tag == tag;
			});
		}
	}
} // namespace Ghost.Extension
