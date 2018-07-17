using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace EditorExtensions
{
    public sealed class EditorExtend
    {
        #region Text AutoComplete
        private const string m_AutoCompleteField = "AutoCompleteField";
        private static List<string> m_CacheCheckList = null;
        private static string m_AutoCompleteLastInput;
        private static string m_EditorFocusAutoComplete;
        static float width = 180f;
        /// <summary>A textField to popup a matching popup, based on developers input values.</summary>
        /// <param name="input">string input.</param>
        /// <param name="source">the data of all possible values (string).</param>
        /// <param name="maxShownCount">the amount to display result.</param>
        /// <param name="levenshteinDistance">
        /// value between 0f ~ 1f,
        /// - more then 0f will enable the fuzzy matching
        /// - 1f = anything thing is okay.
        /// - 0f = require full match to the reference
        /// - recommend 0.4f ~ 0.7f
        /// </param>
        /// <returns>output string.</returns>
        public static string TextFieldAutoComplete(Rect rect, string input, string[] source, Direction direction = Direction.Right, int maxShownCount = 5, float levenshteinDistance = 0.5f)
        {
            string tag = m_AutoCompleteField + GUIUtility.GetControlID(FocusType.Passive);
            int uiDepth = GUI.depth;
            GUI.SetNextControlName(tag);
            GUIStyle cancelButtonStyle = MyGUIStyles.searchTextCancelButton;
            GUIStyle emptyCancelButtonStyle = MyGUIStyles.searchTextCancelButtonEmpty;
            float cancelButtonWidth = cancelButtonStyle.fixedWidth;

            // поле поиска
            Rect searchRect = rect;
            searchRect.width -= cancelButtonWidth;
            string rst = EditorGUI.TextField(searchRect, input, MyGUIStyles.searchTextField);

            // закрывашка
            Rect buttonRect = rect;
            buttonRect.x += rect.width - cancelButtonWidth;
            buttonRect.width = cancelButtonWidth;

            GUI.Label(buttonRect, GUIContent.none, emptyCancelButtonStyle);
            //if (GUI.Button(buttonRect, GUIContent.none, rst != "" ? cancelButtonStyle : emptyCancelButtonStyle) && rst != "")
            //{
            //    rst = "";
            //    rst = EditorGUI.TextField(searchRect, "", MyGUIStyles.searchTextField);

            //}

            GUI.backgroundColor = Color.white;

            if (input.Length > 0 && GUI.GetNameOfFocusedControl() == tag)
            {
                if (m_AutoCompleteLastInput != input || // input changed
                    m_EditorFocusAutoComplete != tag) // another field.
                {
                    // Update cache
                    m_EditorFocusAutoComplete = tag;
                    m_AutoCompleteLastInput = input;

                    List<string> uniqueSrc = new List<string>(new HashSet<string>(source)); // remove duplicate
                    int srcCnt = uniqueSrc.Count;
                    m_CacheCheckList = new List<string>(System.Math.Min(maxShownCount, srcCnt)); // optimize memory alloc

                    // Start with - slow
                    for (int i = 0; i < srcCnt && m_CacheCheckList.Count < maxShownCount; i++)
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
                        for (int i = 0; i < srcCnt && m_CacheCheckList.Count < maxShownCount; i++)
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

                    //// Levenshtein Distance - very very slow.
                    //if (levenshteinDistance > 0f && // only developer request
                    //    input.Length > 3 && // 3 characters on input, hidden value to avoid doing too early.
                    //    m_CacheCheckList.Count < maxShownCount) // have some empty space for matching.
                    //{
                    //    levenshteinDistance = Mathf.Clamp01(levenshteinDistance);
                    //    string keywords = input.ToLower();
                    //    for (int i = 0; i < srcCnt && m_CacheCheckList.Count < maxShownCount; i++)
                    //    {
                    //        int distance = Kit.Extend.StringExtend.LevenshteinDistance(uniqueSrc[i], keywords, caseSensitive: false);
                    //        bool closeEnough = (int)(levenshteinDistance * uniqueSrc[i].Length) > distance;
                    //        if (closeEnough)
                    //        {
                    //            m_CacheCheckList.Add(uniqueSrc[i]);
                    //            uniqueSrc.RemoveAt(i);
                    //            srcCnt--;
                    //            i--;
                    //        }
                    //    }
                    //}
                }

                // Draw recommend keyward(s)
                if (m_CacheCheckList.Count > 0)
                {
                    int cnt = m_CacheCheckList.Count;
                    float height = cnt * EditorGUIUtility.singleLineHeight;
                    Rect area = GetArea(rect, height, direction);
                    //int currentDepth = GUI.depth;
                    GUI.depth = 0;
                    // GUI.BeginGroup(area);
                    // area.position = Vector2.zero;
                    GUI.BeginClip(area);
                    Rect line = new Rect(0, 0, area.width, EditorGUIUtility.singleLineHeight);

                    for (int i = 0; i < cnt; i++)
                    {
                        if (GUI.Button(line, m_CacheCheckList[i]))//, EditorStyles.toolbarDropDown))
                        {
                            rst = m_CacheCheckList[i];
                            GUI.changed = true;
                            GUI.FocusControl(""); // force update
                        }
                        line.y += line.height;
                    }
                    GUI.EndClip();
                    //GUI.EndGroup();
                    //GUI.depth += 10;
                }
            }
            return rst;
        }

        static Rect GetArea(Rect area, float height, Direction direction)
        {
            switch (direction)
            {
                case Direction.Top:
                    return new Rect(area.x, area.y - height, area.width, height);
                case Direction.Down:
                    return new Rect(area.x, area.yMax, area.width, height);
                case Direction.Left:
                    return new Rect(area.x - width, area.yMin, width, height);
                case Direction.Right:
                    return new Rect(area.xMax, area.yMin, width, height);
                default:
                    break;
            }
            return new Rect(area.x, area.y - height, area.width, height);

            //switch (direction)
            //{
            //    case Direction.Top:
            //        return new Rect(area.x, area.y - height, area.width, height);
            //    case Direction.Down:
            //        return new Rect(area.x, area.yMax, area.width, height);
            //    case Direction.Left:
            //        return new Rect(area.x - area.width, area.yMin, area.width, height);
            //    case Direction.Right:
            //        return new Rect(area.xMax, area.yMin, area.width, height);
            //    default:
            //        break;
            //}
            //return new Rect(area.x, area.y - height, area.width, height);
        }

        public static string TextFieldAutoComplete(string input, string[] source, Direction direction = Direction.Right, int maxShownCount = 5, float levenshteinDistance = 0.5f)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            return TextFieldAutoComplete(rect, input, source, direction, maxShownCount, levenshteinDistance);
        }
        #endregion
    }

    public enum Direction
    {
        Top,
        Down,
        Left,
        Right
    }
}