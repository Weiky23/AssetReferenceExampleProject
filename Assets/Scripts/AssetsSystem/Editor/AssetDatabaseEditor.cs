using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetsNamespace
{
    [Serializable]
    public class AssetDatabaseEditor : EditorWindow
    {
        const string message = "В базе ассетов содержится информация об ассетах. Каждый ассет в базе должен обладать уникальным id (названием). Помимо названия, в базе содержится информация о способе загрузки ассета – расположен ли он просто в проекте, находится ли в папке Resources или упакован в AssetBundle. При этом в клиентском коде сериализовано лишь id ассета. Таким образом, изменяя базу, можно обновлять и изменять ассеты, не трогая клиентский код.\nНапример, нужно заменить ассет, ранее просто лежавший в одной из папок проекта, на новый. Для этого достаточно поместить новый ассет с тем же названием в AssetBundle, обновить базу, и доставить игроку обновление в виде обновленной базы и самого AssetBundle'а с ассетом.\n\nДля занесения ассета в базу его необходимо пометить AssetLabel'ом " + AssetLabelNames.DatabaseAsset + " и произвольным количеством других AssetLabel'ов, которые являются фильтрами для выборки ассетов из базы.\nДля того, чтобы помеченный ассет добавился в базу, необходимо обновить базу в этом окне. После этого ассет станет доступен для выбора во всех компонентах системы.\n\nПримеры уже добавленных в базу ассетов находятся в папках Prefabs и Resources.\n\nКомпоненты, работающие с базой, находятся в сцене на объектах Cache and Pool Creator (Optional) и ClientCodeExample. Для удобства выбора ассетов используются Popup'ы и автодополнение названия.";

        const string defaultAssetPath = "Assets/Configurations/AssetsDatabase/gameassetsdata.asset";

        static GameAssetsDatabaseScriptableObject assetsDatabase;
        static AssetLoader assetLoader;

        const string resources = "Resources";
        const int resourcesLength = 10;

        [InitializeOnLoadMethod]
        static void LoadAssetDatabase()
        {
            assetLoader = AssetLoader.GetAssetLoader(true, "AssetBundles/");
            assetsDatabase = AssetDatabase.LoadAssetAtPath<GameAssetsDatabaseScriptableObject>(defaultAssetPath);
            if (assetsDatabase == null)
            {
                Debug.LogError("Нет файла с базой ассетов " + defaultAssetPath);
                return;
            }
            assetsDatabase.Initialize();
        }

        [MenuItem("Window/Asset Database")]
        static void Init()
        {
            EditorWindow window = EditorWindow.GetWindow<AssetDatabaseEditor>();
            window.titleContent = new GUIContent("Asset Database");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create Game Assets Database"))
            {
                CreateGameAssetsDatabase();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(message, EditorStyles.textArea);
        }

        void CreateGameAssetsDatabase()
        {
            string[] guids = AssetDatabase.FindAssets("l: " + AssetLabelNames.DatabaseAsset);//, foldersNames);
            List<AssetInfo> assetInfos = new List<AssetInfo>();

            // для кэша путей зарегистрированных ассетов чтобы отличать дочерние ассеты от ассетов с одинаковыми id
            HashSet<string> cachedPathes = new HashSet<string>();

            // для кэша уже зарегистрированных ассетов
            HashSet<string> cachedIds = new HashSet<string>();

            // для кэша уникальных ассетов и проверки что не пытаемся зарегистрировать несколько уникальных ассетов одинакового типа
            HashSet<string> cachedUniqueAssetTypes = new HashSet<string>();

            bool createNewDatabase = (assetsDatabase == null);

            GameAssetsDatabaseScriptableObject gameAssetsDatabase = createNewDatabase ? CreateInstance<GameAssetsDatabaseScriptableObject>() : assetsDatabase;
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                if (cachedPathes.Contains(path))
                {
                    // возможно это дочерний ассет. Ассет лейблы вешаются на них автоматически. В базу они попадать не должны
                    continue;
                }
                cachedPathes.Add(path);

                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                string assetId = asset.name;

                if (cachedIds.Contains(assetId))
                {
                    Debug.LogError("Существует несколько ассетов с названиями " + assetId + " В базу добавлен только один из них. Нужно исправить. Названия ассетов должны быть уникальными");
                    continue;
                }
                cachedIds.Add(assetId);

                string[] assetLabels = AssetDatabase.GetLabels(asset);

                string assetBundleName = AssetDatabase.GetImplicitAssetBundleName(path);
                AssetInfo assetInfo = new AssetInfo();
                assetInfo.assetId = assetId;

                if (assetLabels.Contains(AssetLabelNames.Unique))
                {
                    // ассет уникальный и может потребоваться доступ по его типу
                    string assetType = asset.GetType().ToString();
                    assetInfo.type = assetType;
                    if (cachedUniqueAssetTypes.Contains(assetType))
                    {
                        AssetInfo firstRegisteredAsset = assetInfos.Where(a => a.type == assetType).FirstOrDefault();
                        Debug.LogError("Ассеты " + assetId + " и " + firstRegisteredAsset.assetId + " помечены как уникальные ассет лейблом " + AssetLabelNames.Unique + " Ассет " + assetId + " не добавлен в базу");
                        continue;
                    }
                    cachedUniqueAssetTypes.Add(assetType);
                }
                if (!string.IsNullOrEmpty(assetBundleName))
                {
                    // ассет из бандла
                    assetInfo.assetLoadType = AssetLoadType.AssetBundle;
                    assetInfo.location = assetBundleName;
                }
                else
                {
                    int resourcesStartIndex = path.IndexOf(resources);
                    if (resourcesStartIndex != -1)
                    {
                        // ассет из ресурсов
                        assetInfo.assetLoadType = AssetLoadType.Resources;

                        string resourcesLoadPathWithExtension = path.Substring(resourcesStartIndex + resourcesLength);
                        int extensionStartIndex = resourcesLoadPathWithExtension.LastIndexOf(Char.Parse("."));
                        string resourcesLoadPath = resourcesLoadPathWithExtension;
                        if (extensionStartIndex != -1)
                        {
                            resourcesLoadPath = resourcesLoadPathWithExtension.Substring(0, extensionStartIndex);
                        }
                        assetInfo.location = resourcesLoadPath;
                    }
                    else
                    {
                        // ассет из проекта
                        assetInfo.assetLoadType = AssetLoadType.Project;
                        assetInfo.asset = asset;
                    }
                }

                // для эдитора - добавить лучше вариант с генерацией базы для эдитора и для билда, где не будет этой лишней информации
                // грузить через бандлы в эдиторе неудобно, потому что они могут быть не сбилжены поэтому там сразу обращение к ассету происходит
                assetInfo.asset = asset;

                if (assetLabels.Length > 1)
                {
                    List<string> labels = new List<string>();
                    for (int k = 0; k < assetLabels.Length; k++)
                    {
                        if (assetLabels[k] != AssetLabelNames.DatabaseAsset)
                        {
                            labels.Add(assetLabels[k]);
                        }
                    }
                    assetInfo.assetLabels = labels.ToArray();
                }
                assetInfos.Add(assetInfo);

            }

            gameAssetsDatabase.assetInfos = assetInfos.ToArray();

            if (createNewDatabase)
            {
                AssetDatabase.CreateAsset(gameAssetsDatabase, defaultAssetPath);
            }
            else
            {
                EditorUtility.SetDirty(gameAssetsDatabase);
                AssetDatabase.SaveAssets();
            }
            LoadAssetDatabase();
        }

        public static string[] GetAssetIdsByLabel(string assetLabel)
        {
            if (assetsDatabase != null)
            {
                return assetsDatabase.GetAssetIdsByLabel(assetLabel);


            }
            return new string[0];
        }

        public static string[] GetAssetLabels()
        {
            if (assetsDatabase != null)
            {
                return assetsDatabase.GetAssetLabels();


            }
            return new string[0];
        }

        public static T GetObject<T>() where T : UnityEngine.Object
        {
            AssetInfo assetInfo = assetsDatabase.GetAssetInfo(typeof(T));
            return assetLoader.LoadAsset<T>(assetInfo);
        }

        public static T GetObject<T>(string assetId) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetId))
                return null;
            AssetInfo assetInfo = assetsDatabase.GetAssetInfo(assetId);
            return assetLoader.LoadAsset<T>(assetInfo);
        }

        [MenuItem("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        }
    }
}
