using UnityEngine;
using System.Collections.Generic;

namespace AssetsNamespace
{
    public class AssetPool : SingletonBehaviour<AssetPool>
    {
        AssetReferences assetReferences;
        AssetInstantiator assetInstantiator;
        Dictionary<string, Queue<GameObject>> pooledAssets = new Dictionary<string, Queue<GameObject>>();
        //Dictionary<string, GameObject> prefabReferences = new Dictionary<string, GameObject>();
        //Dictionary<Type, AssetPool> assetPools;

        public void SetAssetReferences(AssetReferences assetReferences)
        {
            this.assetReferences = assetReferences;
            assetInstantiator = new AssetInstantiator();
        }

        public GameObject GetAssetInactive(string assetId)
        {
            Queue<GameObject> pooledObjects;
            if (pooledAssets.TryGetValue(assetId, out pooledObjects))
            {
                while (pooledObjects.Count > 0)
                {
                    GameObject dequeue = pooledObjects.Dequeue();
                    if (dequeue != null)
                    {
                        return dequeue;
                    }
                }
                GameObject prefabReference = assetReferences.PrefabReference(assetId);
                if (prefabReference != null)
                {
                    GameObject instantiatedGameObject = assetInstantiator.InstantiatedDeactivatedGameObject(assetId, prefabReference, transform);
                    return instantiatedGameObject;
                }
            }
            return null;
        }

        public T GetAssetInactive<T>(string assetId) where T : Component
        {
            Queue<GameObject> pooledObjects;
            if (pooledAssets.TryGetValue(assetId, out pooledObjects))
            {
                while (pooledObjects.Count > 0)
                {
                    GameObject dequeue = pooledObjects.Dequeue();
                    if (dequeue != null)
                    {
                        T component = dequeue.GetComponent<T>();
                        if (component != null)
                        {
                            return component;
                        }
                        else
                        {
                            Debug.LogError("Нет компонента " + typeof(T) + " на ассете " + assetId);
                            pooledObjects.Enqueue(dequeue);
                            return null;
                        }
                    }
                }
                GameObject prefabReference = assetReferences.PrefabReference(assetId);
                if (prefabReference != null)
                {
                    if (prefabReference.GetComponent<T>() != null)
                    {
                        GameObject instantiatedGameObject = assetInstantiator.InstantiatedDeactivatedGameObject(assetId, prefabReference, transform);
                        return instantiatedGameObject.GetComponent<T>();
                    }
                    else
                    {
                        Debug.LogError("Нет компонента " + typeof(T) + " на ассете " + assetId);
                    }
                }
            }
            return null;
        }

        public bool HasPoolOfAssets(string assetId)
        {
            return pooledAssets.ContainsKey(assetId);
        }

        //public void InstantiateToPool(GameObject prefab)
        //{
        //    string assetId = prefab.name;
        //    GameObject pooledGameObject = assetInstantiator.InstantiatedDeactivatedGameObject(assetId, prefab);
        //    AddToPool(assetId, pooledGameObject);
        //}

        //public void InstantiateToPool(GameObject prefab, int count)
        //{
        //    string assetId = prefab.name;
        //    for (int i = 0; i < count; i++)
        //    {
        //        GameObject pooledGameObject = InstantiatedDeactivatedGameObject(assetId, prefab);
        //        AddToPool(assetId, pooledGameObject);
        //    }
        //}

        //void AddToPool(string assetId, GameObject pooledGameObject)
        //{
        //    PooledObject pooledObject = GetPooledObject(pooledGameObject);
        //    AddToPool(assetId, pooledObject);
        //}

        public void AddToPool(string assetId, GameObject pooledObject)
        {
            Queue<GameObject> pooledObjects;
            if (!pooledAssets.TryGetValue(assetId, out pooledObjects))
            {
                pooledObjects = new Queue<GameObject>();
                pooledAssets.Add(assetId, pooledObjects);
            }

            pooledObjects.Enqueue(pooledObject);
            //pooledObject.addedToPool = true;
            PlaceToPoolHierachy(pooledObject);
        }

        //public void Release(GameObject pooledGameObject)
        //{
        //    GameObject pooledObject = GetPooledObject(pooledGameObject);
        //    Release(pooledObject);
        //}

        public void Release(GameObject pooledObject)
        {
            AddToPool(pooledObject.name, pooledObject);
        }

        void PlaceToPoolHierachy(GameObject pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
            pooledObject.transform.parent = transform;
            pooledObject.transform.localPosition = Vector3.zero;
            //pooledObject.ready = true;
        }

        //PooledObject GetPooledObject(GameObject pooledGameObject)
        //{
        //    PooledObject pooledObject = pooledGameObject.GetComponent<PooledObject>();
        //    if (pooledObject == null)
        //    {
        //        pooledObject = pooledGameObject.AddComponent<PooledObject>();
        //    }
        //    return pooledObject;
        //}
    }
}