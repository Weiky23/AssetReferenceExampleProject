using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ClientCodeExample))]
public class ClientCodeExampleEditor : Editor
{
    const string message = "Левый Popup – выбор AssetLabel'а, правый Popup – выбор из списка ассетов с этим AssetLabel'ом.\nPrefab – объект инстанцируется\nSample Config – выведется информация о конфиге\nSample Audio – при старте игры проиграется выбранный звук";

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField(message, EditorStyles.textArea);
        base.OnInspectorGUI();
    }
}
