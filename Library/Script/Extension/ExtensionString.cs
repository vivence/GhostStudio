using UnityEngine;
using System.Collections.Generic;

namespace Ghost.Extension
{
	public static class ExtensionString
	{
		public static string[] PickUpDigits(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}

			var parts = new List<string>();

			bool findingDigit = true;
			int startIndex = 0;
			var strLen = str.Length;
			for (int i = 0; i < strLen; ++i)
			{
				if (findingDigit)
				{
					if (char.IsDigit(str[i]))
					{
						startIndex = i;
						findingDigit = false;
					}
				}
				else
				{
					if (!char.IsDigit(str[i]))
					{
						parts.Add(str.Substring(startIndex, i-startIndex));
						findingDigit = true;
					}
				}
			}
			if (!findingDigit && startIndex < strLen)
			{
				parts.Add(str.Substring(startIndex, strLen-startIndex));
			}

			return parts.ToArray();
		}
	}
} // namespace Ghost.Extension
