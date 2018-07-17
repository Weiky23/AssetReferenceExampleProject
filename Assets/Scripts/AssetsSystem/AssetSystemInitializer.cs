using UnityEngine;
using System.Collections;

namespace AssetsNamespace
{
    // инициализирует систему ассетов в нужном порядке - сначала бандлы, потом кэш, потом ассет пул, потом выгружаем ассет бандлы
    public class AssetSystemInitializer : MonoBehaviour
    {
        AssetBundleCacheCreator assetBundleCacheCreator;
        AssetCacheCreator assetCacheCreator;
        AssetPoolCreator assetPoolCreator;

        void Start()
        {
            assetBundleCacheCreator = GetComponent<AssetBundleCacheCreator>();
            assetCacheCreator = GetComponent<AssetCacheCreator>();
            assetPoolCreator = GetComponent<AssetPoolCreator>();

            if (assetBundleCacheCreator != null)
                assetBundleCacheCreator.CreateAssetBundlesCache();

            if (assetCacheCreator != null)
                assetCacheCreator.CreateAssetCache();

            if (assetPoolCreator != null)
                assetPoolCreator.CreateAssetPool();

            if (assetBundleCacheCreator != null)
                assetBundleCacheCreator.UnloadAssetBundles();
        }
    }
}
