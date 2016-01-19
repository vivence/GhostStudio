using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Extension
{
	public static class S_Vector
	{
		#region vector2
		public static Vector2 Multiply(this Vector2 p, Vector2 other)
		{
			return new Vector2(p.x*other.x, p.y*other.y);
		}

		public static Vector2 Divide(this Vector2 p, Vector2 other)
		{
			return new Vector2(p.x/other.x, p.y/other.y);
		}
		#endregion vector2

		#region vector3
		public static Vector2 XZ(this Vector3 p)
		{
			return new Vector2(p.x, p.z);
		}

		public static Vector3 Multiply(this Vector3 p, Vector3 other)
		{
			return new Vector3(p.x*other.x, p.y*other.y, p.z*other.z);
		}

		public static Vector3 Divide(this Vector3 p, Vector3 other)
		{
			return new Vector3(p.x/other.x, p.y/other.y, p.z/other.z);
		}
		#endregion vector3

		#region vector4
		public static Vector2 XZ(this Vector4 p)
		{
			return new Vector2(p.x, p.z);
		}

		public static Vector4 Multiply(this Vector4 p, Vector4 other)
		{
			return new Vector4(p.x*other.x, p.y*other.y, p.z*other.z, p.w*other.w);
		}

		public static Vector4 Divide(this Vector4 p, Vector4 other)
		{
			return new Vector4(p.x/other.x, p.y/other.y, p.z/other.z, p.w/other.w);
		}
		#endregion vector4
	}
} // namespace Ghost.Extension
