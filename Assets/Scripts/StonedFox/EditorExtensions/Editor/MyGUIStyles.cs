using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EditorExtensions
{
    public class MyGUIStyles
    {
        static GUIStyle centerHeader;
        static GUIStyle centerBoldHeader;
        static GUIStyle noMargin;
        static GUIStyle assetLabel;
        public static GUIStyle searchTextField;
        public static GUIStyle searchTextCancelButtonEmpty;
        public static GUIStyle searchTextCancelButton;
        public static GUIStyle CenterBoldHeader { get { return centerBoldHeader; } }
        public static GUIStyle CenterHeader { get { return centerHeader; } }
        public static GUIStyle NoMargin { get { return noMargin; } }
        public static GUIStyle AssetLabel { get { return assetLabel; } }
        public static GUIStyle SearchTextField { get { return searchTextField; } }

        static MyGUIStyles()
        {
            centerHeader = new GUIStyle(EditorStyles.largeLabel) { alignment = TextAnchor.MiddleCenter };
            centerBoldHeader = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
            noMargin = new GUIStyle(EditorStyles.miniLabel) { margin = new RectOffset(0, 0, 0, 0) };
            assetLabel = GetStyle("AssetLabel");
            searchTextField = GetStyle("SearchTextField");
            searchTextCancelButtonEmpty = GetStyle("SearchCancelButtonEmpty");
            searchTextCancelButton = GetStyle("SearchCancelButton");
        }

        public static GUIStyle GetStyle(string styleName)
        {
            GUIStyle s = GUI.skin.FindStyle(styleName);
            if (s == null)
                s = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            if (s == null)
            {
                Debug.LogError("Missing built-in guistyle " + styleName);
            }
            return s;
        }
    }

    public static class MyGUI
    {
        public static void DrawSeparatorLine()
        {
            EditorGUILayout.LabelField(GUIContent.none, GUI.skin.horizontalSlider);
        }
    }

    public class MyGUIContent
    {
        public static GUIContent playButtonIcon;
        public static GUIContent pauseButtonIcon;

        static MyGUIContent()
        {
            playButtonIcon = GetContent("PlayButton");
            pauseButtonIcon = GetContent("PauseButton");
        }

        public static GUIContent GetContent(string contentName)
        {
            return EditorGUIUtility.IconContent(contentName);
        }
    }

    public class MyColors
    {
        public static Color reorderableListColor;
        public static Color errorAutoCompleteColor;

        static MyColors()
        {
            UnityEngine.ColorUtility.TryParseHtmlString("#E4E5E4FF", out reorderableListColor);
            errorAutoCompleteColor = new Color(1f, 0f, 0f, 0.28f);
        }
    }
}
