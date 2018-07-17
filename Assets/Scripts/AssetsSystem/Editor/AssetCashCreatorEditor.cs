using EditorExtensions;
using UnityEditor;
using UnityEngine;

namespace AssetsNamespace
{
    [CustomEditor(typeof(AssetCacheCreator))]
    class AssetCashCreatorEditor : SingleReorderableListEditor
    {
        string[] assetLabels;
        string[] assetIds;

        void OnEnable()
        {
            InitList(ReorderableListBuilder.GetDefaultBuilder(serializedObject, serializedObject.FindProperty(AssetCacheCreator.AssetsToLoad)));
        }

        public override void OnInspectorGUI()
        {
            assetLabels = AssetDatabaseEditor.GetAssetLabels();
            if (assetLabels == null || assetLabels.Length == 0)
            {
                EditorGUILayout.LabelField("Нет ни одного Asset Label");
                return;
            }
            base.OnInspectorGUI();
        }

        protected override void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "AssetLabel — AssetId", EditorStyles.boldLabel);
        }

        protected override void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 2;

            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

            SerializedProperty assetLabelProperty = element.FindPropertyRelative(AssetReference.Label);
            Rect assetLabelRect = new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight);

            string assetLabel = assetLabelProperty.DrawPopup(assetLabelRect, assetLabels);

            assetIds = AssetDatabaseEditor.GetAssetIdsByLabel(assetLabel);
            if (assetIds == null || assetIds.Length == 0)
            {
                EditorGUILayout.LabelField("Нет ассетов с тегом " + assetLabel + " в базе ассетов!");
                return;
            }
            Rect assetIdRect = new Rect(rect.x + 120, rect.y, 200, EditorGUIUtility.singleLineHeight);
            SerializedProperty assetIdProperty = element.FindPropertyRelative(AssetReference.AssetId);
            string assetId = assetIdProperty.AutoComplete(assetIdRect, assetIds);
        }
    }
}