using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AssetsNamespace;
using EditorExtensions;

[CustomPropertyDrawer(typeof(AssetReference))]
public class AssetReferencePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.DrawRect(rect, MyColors.reorderableListColor);

        rect.y += 2;
        rect.height -= 2;

        string[] assetLabels = AssetDatabaseEditor.GetAssetLabels();
        if (assetLabels == null || assetLabels.Length == 0)
        {
            EditorGUI.LabelField(rect, "Нет ни одного Asset Label");
            return;
        }

        Rect labelRect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label);

        SerializedProperty assetLabelProperty = property.FindPropertyRelative(AssetReference.Label);
        Rect assetLabelRect = new Rect(rect.x + 100, rect.y, 120, EditorGUIUtility.singleLineHeight);

        string assetLabel = assetLabelProperty.DrawPopup(assetLabelRect, assetLabels);

        Rect assetIdRect = new Rect(rect.x + 220, rect.y, 200, EditorGUIUtility.singleLineHeight);

        string[] assetIds = AssetDatabaseEditor.GetAssetIdsByLabel(assetLabel);
        if (assetIds == null || assetIds.Length == 0)
        {
            EditorGUI.LabelField(assetIdRect, "Нет ассетов с тегом " + assetLabel + " в базе ассетов!");
            return;
        }

        SerializedProperty assetIdProperty = property.FindPropertyRelative(AssetReference.AssetId);
        string assetId = assetIdProperty.DrawPopup(assetIdRect, assetIds);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight + 4f;
    }
}
