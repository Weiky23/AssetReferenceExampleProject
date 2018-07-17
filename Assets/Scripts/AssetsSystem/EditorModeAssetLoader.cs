using AssetsNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetsNamespace
{
    public class EditorModeAssetLoader : AssetLoader
    {
        public EditorModeAssetLoader(string assetBundlesFolder)
            : base(assetBundlesFolder)
        {
        }

        protected override T LoadFromBundle<T>(AssetsNamespace.AssetInfo assetInfo)
        {
            // бандлы в эдиторе могут быть не собраны - просто грузим ассет по пути
            return assetInfo.asset as T;
            //T asset = AssetDatabase.LoadAssetAtPath<T>(assetInfo.assetProjectPath);
            //if (asset == null)
            //{
            //    Debug.LogError("Не удается загрузить ассет по пути " + assetInfo.assetProjectPath + " либо ассет не является типом " + typeof(T));
            //}
            //return asset;
        }
    }

    //public class EditorModeAssetLoader : AssetLoader
    //{
    //    public override GameObject LoadAsset(AssetsNamespace.AssetInfo assetInfo)
    //    {
    //        if (assetInfo == null)
    //            return null;
    //        switch (assetInfo.assetLoadType)
    //        {
    //            case AssetsNamespace.AssetLoadType.AssetBundle:
    //                return LoadFromBundle(assetInfo);
    //            case AssetsNamespace.AssetLoadType.Resources:
    //                return LoadFromResources(assetInfo);
    //            case AssetsNamespace.AssetLoadType.Project:
    //                return LoadFromProject(assetInfo);
    //            default:
    //                break;
    //        }
    //        return null;
    //    }

    //    GameObject LoadFromProject(AssetsNamespace.AssetInfo assetInfo)
    //    {
    //        return assetInfo.asset as GameObject;
    //    }

    //    GameObject LoadFromResources(AssetsNamespace.AssetInfo assetInfo)
    //    {
    //        GameObject prefabReference = Resources.Load<GameObject>(assetInfo.location);
    //        if (prefabReference == null)
    //        {
    //            Debug.LogError("Не загружается ассет из папки ресурсы " + assetInfo.assetId + " " + assetInfo.location);
    //        }
    //        return prefabReference;
    //    }

    //    GameObject LoadFromBundle(AssetsNamespace.AssetInfo assetInfo)
    //    {
    //        GameObject asset = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(assetInfo.assetProjectPath);
    //        return asset;
    //    }

    //    public override T LoadObject<T>(AssetInfo assetInfo)
    //    {
    //        if (assetInfo == null)
    //            return null;
    //        return assetInfo.asset as T;
    //    }

    //    public override UnityEngine.Object LoadObject(AssetInfo assetInfo)
    //    {
    //        if (assetInfo == null)
    //            return null;
    //        return assetInfo.asset;
    //    }
    //}
}
