using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetsNamespace
{
    // класс для хранения ссылок на уже загруженные любым образом ассеты
    public class AssetReferences
    {
        // неистанцируемые объекты - звуки
        Dictionary<string, UnityEngine.Object> objectReferences;
        // инстанцируемые объекты - префабы
        Dictionary<string, GameObject> prefabReferences;

        public AssetReferences()
        {
            objectReferences = new Dictionary<string, UnityEngine.Object>();
            prefabReferences = new Dictionary<string, GameObject>();
        }

        public UnityEngine.Object ObjectReference(string assetId)
        {
            UnityEngine.Object asset;
            objectReferences.TryGetValue(assetId, out asset);
            return asset;
        }

        public void AddObjectReference(string assetId, UnityEngine.Object objectReference)
        {
            if (!objectReferences.ContainsKey(assetId))
            {
                objectReferences.Add(assetId, objectReference);
            }
        }

        public GameObject PrefabReference(string assetId)
        {
            GameObject prefab;
            prefabReferences.TryGetValue(assetId, out prefab);
            return prefab;
        }

        public void AddPrefabReference(string assetId, GameObject prefabReference)
        {
            if (!prefabReferences.ContainsKey(assetId))
            {
                prefabReferences.Add(assetId, prefabReference);
            }
        }

        public bool HasPrefabReference(string assetId)
        {
            return prefabReferences.ContainsKey(assetId);
        }

        public bool HasObjectReference(string assetId)
        {
            return objectReferences.ContainsKey(assetId); ;
        }
    }
}
