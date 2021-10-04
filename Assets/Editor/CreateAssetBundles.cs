﻿using System.IO;
using UnityEditor;

namespace Editor
{
    public class CreateAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
        static void BuildAssetBundles()
        {
            var assetBundleDirectory = "Assets/AssetBundles";
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows);
        }
    }
}