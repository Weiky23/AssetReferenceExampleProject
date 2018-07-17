using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetsNamespace
{
    public abstract class AssetLoader
    {
        protected AssetBundlesManager assetBundlesManager;

        public AssetLoader(string assetBundlesFolder)
        {
            assetBundlesManager = new AssetBundlesManager(assetBundlesFolder);
        }

        public T LoadAsset<T>(AssetsNamespace.AssetInfo assetInfo) where T : UnityEngine.Object
        {
            if (assetInfo == null)
                return null;
            switch (assetInfo.assetLoadType)
            {
                case AssetsNamespace.AssetLoadType.AssetBundle:
                    return LoadFromBundle<T>(assetInfo);
                case AssetsNamespace.AssetLoadType.Resources:
                    return LoadFromResources<T>(assetInfo);
                case AssetsNamespace.AssetLoadType.Project:
                    return LoadFromProject<T>(assetInfo);
                default:
                    break;
            }
            return null;
        }

        public static AssetLoader GetAssetLoader(bool editorMode, string assetBundlesFolder)
        {
            if (editorMode)
            {
                return new EditorModeAssetLoader(assetBundlesFolder);
            }
            else
            {
                return new RuntimeAssetLoader(assetBundlesFolder);
            }
        }

        protected virtual T LoadFromProject<T>(AssetInfo assetInfo) where T : UnityEngine.Object
        {
            T asset = assetInfo.asset as T;
            if (asset == null)
            {
                Debug.LogError("Ассет " + assetInfo.assetId + " не является типом " + typeof(T));
            }
            return asset;
        }

        protected virtual T LoadFromResources<T>(AssetInfo assetInfo) where T : UnityEngine.Object
        {
            T prefabReference = Resources.Load<T>(assetInfo.location);
            if (prefabReference == null)
            {
                Debug.LogError("Не загружается ассет из папки ресурсы " + assetInfo.assetId + " " + assetInfo.location + " или ассет не является типом " + typeof(T));
            }
            return prefabReference;
        }

        protected virtual T LoadFromBundle<T>(AssetInfo assetInfo) where T : UnityEngine.Object
        {
            bool cachedBundle = true;
            AssetBundle loadedAssetBundle = assetBundlesManager.GetCachedAssetBundle(assetInfo.location);
            if (loadedAssetBundle == null)
            {
                // нет бандла в кэше - грузим его для одноразового использования
                cachedBundle = false;
                loadedAssetBundle = assetBundlesManager.LoadBundle(assetInfo.location);
            }
            else
            {
                // есть бандл в кэше - его загрузил какой то внешний механизм, не выгружаем его, видимо еще будем грузить из него файлы
                cachedBundle = true;
            }
            T prefab = loadedAssetBundle.LoadAsset<T>(assetInfo.assetId);
            if (prefab == null)
            {
                Debug.LogError("Нет ассета " + assetInfo.assetId + " в бандле " + assetInfo.location + " или ассет не является типом " + typeof(T));
            }
            if (!cachedBundle)
            {
                // выгружаем бандл которого не было в кэше. кэш добавляется только извне, не каждый загруженный бандл в него попадает
                assetBundlesManager.UnloadBundle(assetInfo.location);
            }
            return prefab;
        }

        public void LoadAssetBundleToCach(string assetBundleName)
        {
            assetBundlesManager.LoadBundle(assetBundleName);
        }

        public void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            assetBundlesManager.UnloadBundle(assetBundleName, unloadAllLoadedObjects);
        }

        public void UnloadAllBundles(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            assetBundlesManager.UnloadAllBundles(unloadAllLoadedObjects);
        }
    }
}
