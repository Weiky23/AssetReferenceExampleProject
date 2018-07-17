using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EditorExtensions
{
    // можно сконфигурировать ReorderableList
    public class ReorderableListBuilder
    {
        public SerializedObject serializedObject;
        public SerializedProperty elements;
        public bool draggable;
        public bool displayHeader;
        public bool displayAddButton;
        public bool displayRemoveButton;
        public bool drawCustomElement;

        public ReorderableListBuilder Object(SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;
            return this;
        }

        public ReorderableListBuilder Elements(SerializedProperty elements)
        {
            this.elements = elements;
            return this;
        }

        public ReorderableList Build()
        {
            return new ReorderableList(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton);
        }

        public static ReorderableListBuilder GetDefaultBuilder(SerializedObject serializedObject, SerializedProperty elements)
        {
            ReorderableListBuilder builder = new ReorderableListBuilder()
            {
                serializedObject = serializedObject,
                elements = elements,
                draggable = true,
                displayHeader = true,
                displayAddButton = true,
                displayRemoveButton = true,
                drawCustomElement = true
            };
            return builder;
        }
    }
}
