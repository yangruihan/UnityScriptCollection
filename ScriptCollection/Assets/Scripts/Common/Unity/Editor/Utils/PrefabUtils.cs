using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Common.Unity.Editor
{
    public static class PrefabUtils
    {
        /// <summary>
        /// 加载某一个目录下的所有Prefab
        /// </summary>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        public static List<GameObject> LoadDirectoryPrefab(string rootPath)
        {
            var directoryInfo = new DirectoryInfo(rootPath);
            if (!directoryInfo.Exists) return null;

            var ret = new List<GameObject>();
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories);

            EditorUtility.DisplayProgressBar("加载Prefab中...", "开始加载", 0f);

            int i = 0;
            foreach (FileInfo files in fileInfos)
            {
                string path = files.FullName;
#if UNITY_STANDALONE_WIN
                string assetPath = path.Substring(path.IndexOf("Assets\\", StringComparison.Ordinal));
#else
        string assetPath = path.Substring(path.IndexOf("Assets/"));
#endif

                EditorUtility.DisplayProgressBar("加载Prefab中...", assetPath, ++i * 1.0f / fileInfos.Length);

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                ret.Add(prefab);
            }

            EditorUtility.ClearProgressBar();
            return ret;
        }
    }
}