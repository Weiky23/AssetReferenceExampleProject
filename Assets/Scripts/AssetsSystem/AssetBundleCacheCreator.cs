using UnityEngine;
using System.Collections;
using System;

namespace AssetsNamespace
{
    [RequireComponent(typeof(AssetSystemInitializer))]
    public class AssetBundleCacheCreator : MonoBehaviour
    {
        public string[] cachedAssetBundles;
        public string[] unloadAssetBundles;

        public void CreateAssetBundlesCache()
        {
            for (int i = 0; i < cachedAssetBundles.Length; i++)
            {
                AssetSystem.Loader.LoadAssetBundleToCach(cachedAssetBundles[i]);
            }
        }

        public void UnloadAssetBundles()
        {
            for (int i = 0; i < unloadAssetBundles.Length; i++)
            {
                AssetSystem.Loader.UnloadAssetBundle(unloadAssetBundles[i]);
            }
        }
    }
}
