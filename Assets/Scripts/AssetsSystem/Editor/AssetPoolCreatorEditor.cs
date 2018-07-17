using UnityEngine;
using UnityEditor;
using EditorExtensions;

namespace AssetsNamespace
{
    [CustomEditor(typeof(AssetPoolCreator))]
    class AssetPoolCreatorEditor : SingleReorderableListEditor
    {
        string[] assetLabels;
        string[] assetIds;

        void OnEnable()
        {
            InitList(AssetPoolCreator.AssetsToPool);
            SetHeader("AssetLabel — Amount — AssetId");

            //assetCashList = new ReorderableList(serializedObject, serializedObject.FindProperty(AssetPoolCreator.AssetsToPool), true, true, true, true);

            //assetCashList.drawElementCallback = DrawElement;
            //assetCashList.drawHeaderCallback = (r => EditorGUI.LabelField(r, "AssetLabel — Amount — AssetId", EditorStyles.boldLabel));
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

        protected override void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 2;

            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

            SerializedProperty assetLabelProperty = element.FindPropertyRelative(AssetToPoolInfo.Label);
            Rect assetLabelRect = new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight);

            string assetLabel = assetLabelProperty.DrawPopup(assetLabelRect, assetLabels);

            Rect amountRect = new Rect(rect.x + 120, rect.y, 50, EditorGUIUtility.singleLineHeight);
            SerializedProperty amountProperty = element.FindPropertyRelative(AssetToPoolInfo.Amount);
            amountProperty.intValue = EditorGUI.IntField(amountRect, amountProperty.intValue);

            assetIds = AssetDatabaseEditor.GetAssetIdsByLabel(assetLabel);
            if (assetIds == null || assetIds.Length == 0)
            {
                EditorGUILayout.LabelField("Нет ассетов с тегом " + assetLabel + " в базе ассетов!");
                return;
            }
            Rect assetIdRect = new Rect(rect.x + 170, rect.y, 200, EditorGUIUtility.singleLineHeight);
            SerializedProperty assetIdProperty = element.FindPropertyRelative(AssetToPoolInfo.AssetId);
            string assetId = assetIdProperty.AutoComplete(assetIdRect, assetIds);
        }
    }
}
