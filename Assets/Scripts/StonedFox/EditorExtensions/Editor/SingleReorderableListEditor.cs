using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;

namespace EditorExtensions
{
    // попроще делаем ReorderableList, если он один
    public class SingleReorderableListEditor : Editor
    {
        protected ReorderableList list;

        protected SerializedProperty elements;
        protected bool draggable = true;
        protected bool displayHeader = true;
        protected bool displayAddButton = true;
        protected bool displayRemoveButton = true;
        protected bool drawCustomElement = true;
        protected bool refreshButton = false;
        protected bool orderButton = false;
        protected string header;


        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            serializedObject.Update();
            list.DoLayoutList();
            if (refreshButton || orderButton)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (refreshButton)
                {
                    if (GUILayout.Button("Refresh", EditorStyles.miniButton))
                    {
                        RefreshElements();
                    }
                }
                if (orderButton)
                {
                    if (GUILayout.Button("Order", EditorStyles.miniButton))
                    {
                        OrderElements();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OrderElements()
        {

        }

        protected virtual void RefreshElements()
        {

        }

        protected void InitList(string listSerializedPropertyName)
        {
            elements = serializedObject.FindProperty(listSerializedPropertyName);
            list = new ReorderableList(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton);
            if (displayHeader)
            {
                list.drawHeaderCallback = DrawHeader;
            }
            if (drawCustomElement)
            {
                list.drawElementCallback = DrawElement;
            }
        }

        protected void InitList(ReorderableListBuilder builder)
        {
            list = builder.Build();

            if (displayHeader)
            {
                list.drawHeaderCallback = DrawHeader;
            }
            if (drawCustomElement)
            {
                list.drawElementCallback = DrawElement;
            }
        }

        protected virtual void SetHeader(string header)
        {
            this.header = header;
        }

        protected virtual void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, header, EditorStyles.boldLabel);
        }

        protected virtual void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {

        }
    }
}
