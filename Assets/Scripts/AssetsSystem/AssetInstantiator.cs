using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetsNamespace
{
    public class AssetInstantiator
    {
        public GameObject InstantiatedDeactivatedGameObject(string assetId, GameObject prefab, Transform rootTransform)
        {
            prefab.SetActive(false);
            GameObject instantiatedPrefab = GameObject.Instantiate(prefab, rootTransform);
            instantiatedPrefab.name = assetId;
            return instantiatedPrefab;
        }
    }
}
