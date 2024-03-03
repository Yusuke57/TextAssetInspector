using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Yusuke57.CommonPackage.Editor.TextAssetCustom
{
    [CustomEditor(typeof(TextAsset))]
    public class TextAssetCustomEditor : UnityEditor.Editor
    {
        private readonly List<IPartialTextAssetCustomEditor> _partialTextAssetCustomEditors = new()
        {
            new JsonTextAssetCustomEditor(),
            new CsvTextAssetCustomEditor(),
        };
        
        private readonly IPartialTextAssetCustomEditor _defaultTextAssetCustomEditor = new DefaultTextAssetCustomEditor();
        private readonly HeaderDrawer _headerDrawer = new();

        public override void OnInspectorGUI()
        {
            var previousEnabled = GUI.enabled;
            GUI.enabled = true;

            var textAsset = (TextAsset) target;
            foreach (var partialTextAssetCustomEditor in _partialTextAssetCustomEditors)
            {
                if (partialTextAssetCustomEditor.IsTargetFile(textAsset))
                {
                    DrawInspector(partialTextAssetCustomEditor, textAsset, true);
                    return;
                }
            }
            
            DrawInspector(_defaultTextAssetCustomEditor, textAsset, false);
            
            GUI.enabled = previousEnabled;
        }

        private void DrawInspector(IPartialTextAssetCustomEditor partialTextAssetCustomEditor, TextAsset textAsset, bool isCustomTabVisible)
        {
            var headerInfo = _headerDrawer.DrawHeader(isCustomTabVisible);

            EditorGUILayout.Space(20);

            EditorGUI.BeginDisabledGroup(headerInfo.IsReadonly);
            EditorGUI.BeginChangeCheck();

            string text;
            if (headerInfo.IsCustom)
            {
                try
                {
                    text = partialTextAssetCustomEditor.DrawTextAssetContent(textAsset);
                }
                catch
                {
                    EditorGUILayout.HelpBox("Invalid format", MessageType.Error);
                    text = textAsset.text;
                }
            }
            else
            {
                text = _defaultTextAssetCustomEditor.DrawTextAssetContent(textAsset);
            }
                    
            if (EditorGUI.EndChangeCheck())
            {
                var path = AssetDatabase.GetAssetPath(textAsset);
                File.WriteAllText(path, text);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            EditorGUI.EndDisabledGroup();
            
            if (headerInfo.IsCustom)
            {
                EditorGUILayout.Space(20);
                partialTextAssetCustomEditor.DrawPagination();
            }
        }
    }
}