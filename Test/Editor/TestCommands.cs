using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Ghost.EditorTool
{
	public static class E_TestCommands 
	{
		[MenuItem("Test/AssetBundle")]
		static void TestAssetBundle()
		{
			BuildPipeline.BuildAssetBundles (
				Application.streamingAssetsPath, 
				BuildAssetBundleOptions.UncompressedAssetBundle, 
				EditorUserBuildSettings.activeBuildTarget);
		}
	}
} // namespace Ghost.EditorTool
