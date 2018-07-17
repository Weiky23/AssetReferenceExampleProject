using CSharpExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorExtensions
{
    public static class SerializedPropertyExtentions
    {
        public static string DrawPopup(this SerializedProperty stringValueProperty, Rect rect, string[] values)
        {
            EditorGUI.BeginChangeCheck();

            int arrayIndex = Array.IndexOf(values, stringValueProperty.stringValue);

            if (arrayIndex == -1)// || arrayIndex == 0)
            {
                // текущее сериализованное значение не содержится в списке текущих эвентов. Нужно нарисовать ошибку
                EditorGUI.DrawRect(rect, Color.red);
                arrayIndex = 0;
            }

            int choice = EditorGUI.Popup(rect, arrayIndex, values);

            if (EditorGUI.EndChangeCheck())
            {
                stringValueProperty.stringValue = values[choice];
            }

            return stringValueProperty.stringValue;
        }

        public static string AutoComplete(this SerializedProperty stringValueProperty, Rect rect, string[] values, Direction direction = Direction.Right)
        {
            string currentValue = stringValueProperty.stringValue;

            if (values != null && !values.Contains(currentValue))
            {
                GUI.backgroundColor = MyColors.errorAutoCompleteColor;

                //EditorGUI.DrawRect(rect, Color.red);
            }
            string autoComplete = EditorExtend.TextFieldAutoComplete(rect, currentValue, values, direction);
            GUI.backgroundColor = Color.white;
            stringValueProperty.stringValue = autoComplete;
            //EditorGUI.TextField(rect, autoComplete);//, MyGUIStyles.GetStyle("SearchTextField"));
            return autoComplete;
        }

        public static List<string> AutoCompleteValues(this SerializedProperty stringValueProperty, string[] values)
        {
            string input = stringValueProperty.stringValue;

            if (input.Length > 0)
            {

                List<string> uniqueSrc = new List<string>(new HashSet<string>(values)); // remove duplicate
                int srcCnt = uniqueSrc.Count;
                List<string> m_CacheCheckList = new List<string>(srcCnt); // optimize memory alloc

                // Start with - slow
                for (int i = 0; i < srcCnt && m_CacheCheckList.Count < srcCnt; i++)
                {
                    if (uniqueSrc[i].ToLower().StartsWith(input.ToLower()))
                    {
                        m_CacheCheckList.Add(uniqueSrc[i]);
                        uniqueSrc.RemoveAt(i);
                        srcCnt--;
                        i--;
                    }
                }

                // Contains - very slow
                if (m_CacheCheckList.Count == 0)
                {
                    for (int i = 0; i < srcCnt && m_CacheCheckList.Count < srcCnt; i++)
                    {
                        if (uniqueSrc[i].ToLower().Contains(input.ToLower()))
                        {
                            m_CacheCheckList.Add(uniqueSrc[i]);
                            uniqueSrc.RemoveAt(i);
                            srcCnt--;
                            i--;
                        }
                    }
                }

                return m_CacheCheckList;
            }

            return values.ToList();
        }

        public static void RefreshElements(this SerializedProperty elements, Type sourceConstNamesType)
        {
            List<string> types = sourceConstNamesType.GetAllPublicConstantValues<string>();
            elements.ClearArray();
            for (int i = 0; i < types.Count; i++)
            {
                elements.InsertArrayElementAtIndex(i);
                elements.GetArrayElementAtIndex(i).stringValue = types[i];
            }
        }

        public static void OrderElements(this SerializedProperty elements)
        {
            List<string> currentTypes = new List<string>();
            int arraySize = elements.arraySize;
            for (int i = 0; i < arraySize; i++)
            {
                currentTypes.Add(elements.GetArrayElementAtIndex(i).stringValue);
            }
            currentTypes.Sort();
            elements.ClearArray();
            for (int i = 0; i < currentTypes.Count; i++)
            {
                elements.InsertArrayElementAtIndex(i);
                elements.GetArrayElementAtIndex(i).stringValue = currentTypes[i];
            }
        }

        public static bool SwapArrayObjects(this SerializedProperty arrayOfObjects, int first, int second)
        {
            int arrayLength = arrayOfObjects.arraySize;
            if (first < 0 || second < 0 || first == second || first >= arrayLength || second >= arrayLength)
            {
                return false;
            }

            SerializedProperty propertyFirst = arrayOfObjects.GetArrayElementAtIndex(first);
            UnityEngine.Object objectFirst = propertyFirst.objectReferenceValue;

            SerializedProperty propertySecond = arrayOfObjects.GetArrayElementAtIndex(second);  
            UnityEngine.Object objectSecond = propertySecond.objectReferenceValue;

            propertyFirst.objectReferenceValue = objectSecond;
            propertySecond.objectReferenceValue = objectFirst;

            return true;
        }
    }
}
