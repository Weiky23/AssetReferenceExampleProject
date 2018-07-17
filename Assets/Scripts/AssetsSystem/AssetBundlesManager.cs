using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AssetsNamespace
{
    public class AssetBundlesManager
    {
        Dictionary<string, AssetBundle> loadedBundles;
        string assetBundlesFolder;

        public AssetBundlesManager(string assetBundlesFolder)
        {
            loadedBundles = new Dictionary<string, AssetBundle>();
            this.assetBundlesFolder = assetBundlesFolder;
        }

        public AssetBundle LoadBundle(string assetBundleName)
        {
            AssetBundle loadedBundle = GetCachedAssetBundle(assetBundleName);
            if (loadedBundle != null)
            {
                return loadedBundle;
            }
            string path = Application.dataPath + assetBundlesFolder + assetBundleName;
            loadedBundle = AssetBundle.LoadFromFile(path);
            if (loadedBundle == null)
            {
                Debug.LogError("Не удается загрузить ассет бандл на пути " + path);
                return null;
            }
            loadedBundles.Add(assetBundleName, loadedBundle);
            return loadedBundle;
        }

        public void UnloadBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (!BundleLoaded(assetBundleName))
            {
                return;
            }
            AssetBundle assetBundle = GetCachedAssetBundle(assetBundleName);
            if (assetBundle != null)
            {
                assetBundle.Unload(unloadAllLoadedObjects);
            }
            loadedBundles.Remove(assetBundleName);
        }

        public void UnloadAllBundles(bool unloadAllLoadedObjects = false)
        {
            AssetBundle[] bundles = loadedBundles.Values.ToArray();
            for (int i = 0; i < bundles.Length; i++)
            {
                bundles[i].Unload(unloadAllLoadedObjects);
            }
            loadedBundles.Clear();
        }

        public bool BundleLoaded(string assetBundleName)
        {
            if (loadedBundles.ContainsKey(assetBundleName))
            {
                AssetBundle assetBundle = loadedBundles[assetBundleName];
                if (assetBundle != null)
                {
                    return true;
                }
                loadedBundles.Remove(assetBundleName);
                return false;
            }
            return false;
        }

        public AssetBundle GetCachedAssetBundle(string assetBundleName)
        {
            if (BundleLoaded(assetBundleName))
            {
                return loadedBundles[assetBundleName];
            }
            return null;
        }
    }
}
