using UnityEngine;
using System.IO;

namespace AssetsNamespace
{
    public class AssetSystem : SingletonBehaviour<AssetSystem>
    {
        const string gameassetsdata = "gameassetsdata.asset";
        public GameAssetsDatabaseScriptableObject assetDatabase;
        AssetReferences assetReferences;
        AssetLoader assetLoader;
        AssetInstantiator assetInstantiator;
        AssetPool assetPool;

        public bool editorMode = true;
        string assetBundlesFolder = @"/AssetBundles/";

        public static AssetLoader Loader { get { return Instance.assetLoader; } }

        protected override void Awake()
        {
            base.Awake();
            assetLoader = AssetLoader.GetAssetLoader(editorMode, assetBundlesFolder);
            assetReferences = new AssetReferences();
            assetPool = AssetPool.Instance;
            assetPool.SetAssetReferences(assetReferences);
            assetInstantiator = new AssetInstantiator();
            LoadGameAssetsDatabase();
        }

        void LoadGameAssetsDatabase()
        {
            if (assetDatabase != null)
            {
                assetDatabase.CreateAssetTable();
            }
        }

        // для предварительного создания пулов
        public void InstantiateToPool(string assetId, int count)
        {
            GameObject prefabReference = assetReferences.PrefabReference(assetId);
            if (prefabReference == null)
            {
                AssetInfo assetInfo = assetDatabase.GetAssetInfo(assetId);
                if (assetInfo == null)
                {
                    Debug.LogError("Нет информации об ассете в базе " + assetId);
                    return;
                }

                prefabReference = assetLoader.LoadAsset<GameObject>(assetInfo);
                if (prefabReference == null)
                    return;

                assetReferences.AddPrefabReference(assetId, prefabReference);
            }
            for (int i = 0; i < count; i++)
            {
                GameObject instantiated = assetInstantiator.InstantiatedDeactivatedGameObject(assetId, prefabReference, assetPool.transform);
                assetPool.AddToPool(assetId, instantiated);
            }
        }

        public static T GetAsset<T>(string assetId) where T : Component
        {
            return Instance.GetAssetInactive<T>(assetId);
        }

        public static GameObject GetAsset(string assetId)
        {
            return Instance.GetAssetInactive(assetId);
        }

        GameObject GetAssetInactive(string assetId)
        {
            if (assetPool.HasPoolOfAssets(assetId))
            {
                // уже есть такой пул объектов - пул сам разберется, если необходимо добавить еще один объект, потому что ссылки на префабы общие
                return assetPool.GetAssetInactive(assetId);
            }
            // нет такого пула объектов - поищем среди ссылок
            GameObject prefabReference = assetReferences.PrefabReference(assetId);
            if (prefabReference != null)
            {
                return InstantiatePrefabReference(assetId, prefabReference);
            }

            // нет ссылок - надо искать ассет в базе
            AssetInfo assetInfo = assetDatabase.GetAssetInfo(assetId);
            if (assetInfo == null)
            {
                return null;
            }

            prefabReference = assetLoader.LoadAsset<GameObject>(assetInfo);
            if (prefabReference == null)
                return null;

            assetReferences.AddPrefabReference(assetId, prefabReference);
            return InstantiatePrefabReference(assetId, prefabReference);
        }

        T GetAssetInactive<T>(string assetId) where T : Component
        {
            if (assetPool.HasPoolOfAssets(assetId))
            {
                // уже есть такой пул объектов - пул сам разберется, если необходимо добавить еще один объект, потому что ссылки на префабы общие
                return assetPool.GetAssetInactive<T>(assetId);
            }
            // нет такого пула объектов - поищем среди ссылок
            GameObject prefabReference = assetReferences.PrefabReference(assetId);
            if (prefabReference != null)
            {
                return InstantiatePrefabReference<T>(assetId, prefabReference);
            }

            // нет ссылок - надо искать ассет в базе
            AssetInfo assetInfo = assetDatabase.GetAssetInfo(assetId);
            if (assetInfo == null)
            {
                return null;
            }

            prefabReference = assetLoader.LoadAsset<GameObject>(assetInfo);
            if (prefabReference == null)
                return null;

            assetReferences.AddPrefabReference(assetId, prefabReference);
            return InstantiatePrefabReference<T>(assetId, prefabReference);
        }

        GameObject InstantiatePrefabReference(string assetId, GameObject prefabReference)
        {
            // в пул добавлять не нужно, добавится сам при реюзе
            GameObject instantiated = assetInstantiator.InstantiatedDeactivatedGameObject(assetId, prefabReference, transform);
            return instantiated;
        }

        T InstantiatePrefabReference<T>(string assetId, GameObject prefabReference) where T : Component
        {
            if (prefabReference.GetComponent<T>() != null)
            {
                // в пул добавлять не нужно, добавится сам при реюзе
                GameObject instantiated = assetInstantiator.InstantiatedDeactivatedGameObject(assetId, prefabReference, transform);
                return instantiated.GetComponent<T>();
            }
            else
            {
                Debug.LogError("Нет компонента " + typeof(T) + " на префабе " + assetId);
            }
            return null;
        }

        public static void Release(GameObject releasedGameObject)
        {
            Instance.assetPool.Release(releasedGameObject);
        }

        // для уникальных объектов, присутствующих в базе ассетов в единственном числе для этого типа
        public static T GetObject<T>() where T : UnityEngine.Object
        {
            string assetId = Instance.assetDatabase.GetUniqueAssetId(typeof(T));
            return GetObject<T>(assetId);
        }

        public static T GetObject<T>(string assetId) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetId))
                return null;

            // сначала проверяем в кэше ссылок
            UnityEngine.Object objectReference = Instance.assetReferences.ObjectReference(assetId);
            T typeReference = null;
            if (objectReference != null)
            {
                typeReference = objectReference as T;
                if (typeReference == null)
                {
                    Debug.LogError("Ассет " + assetId + " не является типом " + typeof(T));
                    return null;
                }
                return typeReference;
            }

            // нет в кэше - находим информацию об ассете
            AssetInfo assetInfo = Instance.assetDatabase.GetAssetInfo(assetId);
            if (assetInfo == null)
            {
                Debug.LogError("Нет ассета " + assetId + " в базе ассетов");
                return null;
            }

            // информация есть - грузим ассет
            typeReference = Instance.assetLoader.LoadAsset<T>(assetInfo);
            if (typeReference != null)
            {
                // ассет загрузился - добавляем к кэшу
                Instance.assetReferences.AddObjectReference(assetId, typeReference);
                return typeReference;
            }
            return null;
        }

        public void CashAsset(string assetId)
        {
            if (assetReferences.HasPrefabReference(assetId) || assetReferences.HasObjectReference(assetId))
                return;

            AssetInfo assetInfo = assetDatabase.GetAssetInfo(assetId);
            if (assetInfo == null)
                return;

            UnityEngine.Object objectReference = assetLoader.LoadAsset<UnityEngine.Object>(assetInfo);
            if (objectReference == null)
                return;

            GameObject prefabReference = objectReference as GameObject;
            if (prefabReference != null)
            {
                assetReferences.AddPrefabReference(assetId, prefabReference);
            }
            else
            {
                assetReferences.AddObjectReference(assetId, objectReference);
            }
        }
    }
}


//using UnityEngine;
//using System.Collections;
//using VFX;
//using Sounds;
//using System;
//using System.Collections.Generic;
//using System.Runtime.Serialization;
//using System.IO;

//namespace AssetsNamespace
//{
//    public class AssetSystem : SingletonBehaviour<AssetSystem>
//    {
//        const string gameassetsdata = "gameassetsdata.asset";
//        public GameAssetsDatabaseScriptableObject assetDatabase;
//        AssetReferences assetReferences;
//        AssetLoader assetLoader;
//        AssetInstantiator assetInstantiator;
//        AssetPool assetPool;
//        SoundSystem soundSystem;
//        //VisualEffectsSystem visualEffectsSystem;

//        void Awake()
//        {
//            assetLoader = new RuntimeAssetLoader();
//            assetReferences = new AssetReferences();
//            assetPool = AssetPool.Instance;
//            assetPool.SetAssetReferences(assetReferences);
//            assetInstantiator = new AssetInstantiator();
//            LoadGameAssetsDatabase();
//        }

//        void LoadGameAssetsDatabase()
//        {
//            // надо загружать саму базу а не по ссылке


//            if (assetDatabase == null)
//            {
//                string filePath = Path.Combine(Application.streamingAssetsPath, gameassetsdata);
//                if (File.Exists(filePath))
//                {
//                    string dataAsJson = File.ReadAllText(filePath);
//                    assetDatabase = JsonUtility.FromJson<GameAssetsDatabaseScriptableObject>(dataAsJson);
//                }
//                else
//                {
//                    Debug.LogError("Нет файла с базой ассетов пути " + filePath);
//                    assetDatabase = ScriptableObject.CreateInstance<GameAssetsDatabaseScriptableObject>();
//                }
//            }
//            if (assetDatabase != null)
//            {
//                assetDatabase.CreateAssetTable();
//            }
//        }

//        // для предварительного создания пулов
//        public void InstantiateToPool(string assetId, int count)
//        {
//            AssetInfo assetInfo = assetDatabase.GetAssetInfo(assetId);
//            if (assetInfo == null)
//            {
//                Debug.LogError("Нет информации об ассете в базе " + assetId);
//                return;
//            }

//            GameObject prefabReference = assetLoader.LoadAsset(assetInfo);
//            if (prefabReference == null)
//                return;

//            assetReferences.AddPrefabReference(assetId, prefabReference);
//            for (int i = 0; i < count; i++)
//            {
//                GameObject instantiated = assetInstantiator.InstantiatedDeactivatedGameObject(assetId, prefabReference, assetPool.transform);
//                assetPool.AddToPool(assetId, instantiated);
//            }
//        }

//        public static T GetAsset<T>(string assetId) where T : Component
//        {
//            return Instance.GetAssetInactive<T>(assetId);
//        }

//        public T GetAssetInactive<T>(string assetId) where T : Component
//        {
//            if (assetPool.HasPoolOfAssets(assetId))
//            {
//                // уже есть такой пул объектов - пул сам разберется, если необходимо добавить еще один объект, потому что ссылки на префабы общие
//                return assetPool.GetAssetInactive<T>(assetId);
//            }
//            // нет такого пула объектов - поищем среди ссылок
//            GameObject prefabReference = assetReferences.PrefabReference(assetId);
//            if (prefabReference != null)
//            {
//                return InstantiatePrefabReference<T>(assetId, prefabReference);
//            }

//            // нет ссылок - надо искать ассет в базе
//            AssetInfo assetInfo = assetDatabase.GetAssetInfo(assetId);
//            if (assetInfo == null)
//            {
//                Debug.LogError("Нет информации об ассете в базе " + assetId);
//                return null;
//            }

//            prefabReference = assetLoader.LoadAsset(assetInfo);
//            if (prefabReference == null)
//                return null;

//            assetReferences.AddPrefabReference(assetId, prefabReference);
//            return InstantiatePrefabReference<T>(assetId, prefabReference);
//        }

//        T InstantiatePrefabReference<T>(string assetId, GameObject prefabReference) where T : Component
//        {
//            if (prefabReference.GetComponent<T>() != null)
//            {
//                // в пул добавлять не нужно, добавится сам при реюзе
//                GameObject instantiated = assetInstantiator.InstantiatedDeactivatedGameObject(assetId, prefabReference, transform);
//                return instantiated.GetComponent<T>();
//            }
//            else
//            {
//                Debug.LogError("Нет компонента " + typeof(T) + " на префабе " + assetId);
//            }
//            return null;
//        }

//        public static void Release(GameObject releasedGameObject)
//        {
//            Instance.assetPool.Release(releasedGameObject);
//        }

//        public static void NewSoundSystem(SoundSystem soundSystem)
//        {
//            Instance.soundSystem = soundSystem;
//        }

//        public static T GetObject<T>(string assetId, AssetCollectionType assetCollectionType) where T : UnityEngine.Object
//        {
//            if (assetCollectionType == AssetCollectionType.Sound)
//            {
//                return SoundSystem.GetAudioClip(assetId) as T;
//            }
//            //else if (assetCollectionType == AssetCollectionType.VFX)
//            //{
//            //    return AssetPoolSystem.AssetPoolSystem.Instance.GetAssetInactive<T>(assetId);
//            //}
//            return null;
//        }

//        //public static T GetAsset<T>(string assetId, AssetCollectionType assetCollectionType) where T : Component
//        //{
//        //    if (assetCollectionType == AssetCollectionType.VFX)
//        //    {
//        //        return Instance.assetPool.GetAssetInactive<T>(assetId);
//        //    }
//        //    return null;
//        //}

//        //public static T GetDefaultAsset<T>(string eventId, AssetCollectionType assetCollectionType) where T : UnityEngine.Object
//        //{
//        //    if (assetCollectionType == AssetCollectionType.Sound)
//        //    {
//        //        return SoundSystem.GetDefaultSound(eventId) as T;
//        //    }
//        //    else if (assetCollectionType == AssetCollectionType.VFX)
//        //    {
//        //        return Instance.visualEffectsSystem.GetDefaultVisualEffect(eventId) as T;
//        //    }
//        //    return null;
//        //}

//        //public static void NewVisualEffectsSystem(VisualEffectsSystem visualEffectsSystem)
//        //{
//        //    Instance.visualEffectsSystem = visualEffectsSystem;
//        //}

//        public static T GetObject<T>(string assetId) where T : UnityEngine.Object
//        {
//            return Instance.GetObject<T>(assetId);
//        }

//        T GetObject<T>(string assetId) where T: UnityEngine.Object
//        {

//        }  
//    }

//    // попытка первой реализации - не работает, потому что поле Object в AssetInfo сериализуется как InstanceID и при перезапуске ссылка ссылается на другой объект
//    // если сохранять базу как ScriptableObject, поле Object в AssetInfo сериализуется как надо
//    [Serializable]
//    public class GameAssetsDatabase
//    {
//        public AssetInfo[] assetInfos;

//        [NonSerialized]
//        Dictionary<string, AssetInfo> assetTable;

//        public GameAssetsDatabase(AssetInfo[] assetInfos)
//        {
//            this.assetInfos = assetInfos;
//        }

//        [OnDeserialized]
//        void CreateAssetTable(StreamingContext context)
//        {
//            CreateAssetTable();
//        }

//        public void CreateAssetTable()
//        {
//            assetTable = new Dictionary<string, AssetInfo>();
//            for (int i = 0; i < assetInfos.Length; i++)
//            {
//                AssetInfo assetInfo = assetInfos[i];
//                assetTable.Add(assetInfo.assetId, assetInfo);
//            }
//        }

//        public AssetInfo GetAssetInfo(string assetId)
//        {
//            AssetInfo assetInfo;
//            assetTable.TryGetValue(assetId, out assetInfo);
//            return assetInfo;
//        }
//    }

//    // описывает местоположение ассета для его нахождения. Создается автоматически в редакторе
//    [Serializable]
//    public class AssetInfo
//    {
//        public string assetId;
//        public AssetLoadType assetLoadType;
//        public string path;
//        public UnityEngine.Object asset;
//        public string[] assetLabels;

//        public const string Asset = "asset";
//    }

//    public enum AssetLoadType
//    {
//        // ассет лежит в бандле
//        AssetBundle,
//        // ассет лежит в папке ресурсы
//        Resources,
//        // ассет лежит в исходном проекте - есть ссылка на него в поле asset
//        Project
//    }

//    // класс для хранения ссылок на уже загруженные любым образом ассеты
//    public class AssetReferences
//    {
//        // неистанцируемые объекты - звуки
//        Dictionary<string, UnityEngine.Object> assetReferences;
//        // инстанцируемые объекты - префабы
//        Dictionary<string, GameObject> prefabReferences;

//        public AssetReferences()
//        {
//            assetReferences = new Dictionary<string, UnityEngine.Object>();
//            prefabReferences = new Dictionary<string, GameObject>();
//        }

//        public UnityEngine.Object AssetReference(string assetId)
//        {
//            UnityEngine.Object asset;
//            assetReferences.TryGetValue(assetId, out asset);
//            return asset;
//        }

//        public void AddAssetReference(string assetId, UnityEngine.Object assetReference)
//        {
//            if (!assetReferences.ContainsKey(assetId))
//            {
//                assetReferences.Add(assetId, assetReference);
//            }
//        }

//        public GameObject PrefabReference(string assetId)
//        {
//            GameObject prefab;
//            prefabReferences.TryGetValue(assetId, out prefab);
//            return prefab;
//        }

//        public void AddPrefabReference(string assetId, GameObject prefabReference)
//        {
//            if (!prefabReferences.ContainsKey(assetId))
//            {
//                prefabReferences.Add(assetId, prefabReference);
//            }
//        }
//    }

//    public class AssetInstantiator
//    {
//        public GameObject InstantiatedDeactivatedGameObject(string assetId, GameObject prefab, Transform rootTransform)
//        {
//            prefab.SetActive(false);
//            GameObject instantiatedPrefab = GameObject.Instantiate(prefab, rootTransform);
//            instantiatedPrefab.name = assetId;
//            return instantiatedPrefab;
//        }
//    }
//}