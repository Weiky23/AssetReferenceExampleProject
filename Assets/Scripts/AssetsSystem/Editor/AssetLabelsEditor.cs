using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorExtensions;

namespace AssetsNamespace
{
    [CustomEditor(typeof(AssetLabels))]
    public class AssetLabelsEditor : SingleReorderableListEditor
    {
        const string description = "Необязательный и неиспользуемый конфиг - просто для справки.\nПеречень ассет лейблов, которыми можно помечать ассеты перед добавлением в базу.\nЧтобы внести новый ассет лейбл в этот список, нужно добавить конст строку в класс AssetLabelNames и нажать Refresh";       

        void OnEnable()
        {
            displayAddButton = false;
            displayRemoveButton = false;
            refreshButton = true;
            orderButton = true;
            header = "Asset Labels";
            InitList(AssetLabels.AssetLabelsName);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(description, EditorStyles.textArea);
            base.OnInspectorGUI();
        }

        protected override void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 2;
            rect.width = Mathf.Min(rect.width, 180);
            SerializedProperty element = elements.GetArrayElementAtIndex(index);
            EditorGUI.LabelField(rect, element.stringValue, MyGUIStyles.AssetLabel);
        }

        protected override void RefreshElements()
        {
            elements.RefreshElements(typeof(AssetLabelNames));
            serializedObject.ApplyModifiedProperties();
        }

        protected override void OrderElements()
        {
            elements.OrderElements();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
