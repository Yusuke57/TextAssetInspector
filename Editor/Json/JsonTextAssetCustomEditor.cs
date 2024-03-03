using System;
using System.Collections.Generic;
using System.Linq;
using Codeplex.Data;
using UnityEditor;
using UnityEngine;

namespace Yusuke57.CommonPackage.Editor.TextAssetCustom
{
    public class JsonTextAssetCustomEditor : IPartialTextAssetCustomEditor
    {
        private readonly Dictionary<string, bool> _foldoutStates = new();

        public bool IsTargetFile(TextAsset textAsset)
        {
            var path = AssetDatabase.GetAssetPath(textAsset);
            return !string.IsNullOrEmpty(path) && path.EndsWith(".json");
        }

        public string DrawTextAssetContent(TextAsset textAsset)
        {
            var dynamicJson = DynamicJson.Parse(textAsset.text);
            DrawJsonField(dynamicJson, "root");
            return dynamicJson.ToString();
        }

        public void DrawPagination()
        {
            // Do nothing
        }

        private void DrawJsonField(dynamic dynamicJson, string path)
        {
            var memberNames = dynamicJson.GetDynamicMemberNames();

            foreach (var memberName in memberNames)
            {
                var isArrayElement = dynamicJson.IsArray;
                var memberKey = isArrayElement ? int.Parse(memberName) : memberName;
                var value = dynamicJson[memberKey];
                var updatedPath = $"{path}.{memberName}";
                var label = isArrayElement ? string.Empty : memberName;

                switch (value)
                {
                    case string:
                        using (CreateHorizontalScope(EditorGUI.indentLevel))
                        {
                            dynamicJson[memberKey] = EditorGUILayout.TextField(label, value);
                            DrawDeleteButton(() => dynamicJson.Delete(isArrayElement ? int.Parse(memberName) : memberName));
                        }
                        break;
                    case double:
                        using (CreateHorizontalScope(EditorGUI.indentLevel))
                        {
                            dynamicJson[memberKey] = EditorGUILayout.DoubleField(label, value);
                            DrawDeleteButton(() => dynamicJson.Delete(isArrayElement ? int.Parse(memberName) : memberName));
                        }
                        break;
                    case bool:
                        using (CreateHorizontalScope(EditorGUI.indentLevel))
                        {
                            dynamicJson[memberKey] = EditorGUILayout.Toggle(label, value);
                            DrawDeleteButton(() => dynamicJson.Delete(isArrayElement ? int.Parse(memberName) : memberName));
                        }
                        break;
                    case DynamicJson:
                    {
                        _foldoutStates.TryAdd(updatedPath, true);
                        
                        using (CreateHorizontalScope(EditorGUI.indentLevel + 1))
                        {
                            _foldoutStates[updatedPath] = EditorGUILayout.Foldout(_foldoutStates[updatedPath], label, true);
                            DrawDeleteButton(() => dynamicJson.Delete(isArrayElement ? int.Parse(memberName) : memberName));
                        }

                        if (_foldoutStates[updatedPath])
                        {
                            EditorGUI.indentLevel++;
                            
                            DrawJsonField(value, updatedPath);

                            if (value.IsArray)
                            {
                                DrawArrayFooter(value);
                            }
                            
                            EditorGUI.indentLevel--;
                        }
                        
                        break;
                    }
                    default:
                        using (CreateHorizontalScope(EditorGUI.indentLevel))
                        {
                            EditorGUILayout.LabelField(memberName, value.ToString());
                            DrawDeleteButton(() => dynamicJson.Delete(isArrayElement ? int.Parse(memberName) : memberName));
                        }
                        break;
                }
            }
        }

        private EditorGUILayout.HorizontalScope CreateHorizontalScope(int backgroundColorLevel)
        {
            var style = new GUIStyle
            {
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(16, 8, 1, 1)
            };

            var horizontalScope = new EditorGUILayout.HorizontalScope(style);
            var backgroundAlpha = Mathf.Clamp01(backgroundColorLevel * 0.05f);
            var indentRect = EditorGUI.IndentedRect(horizontalScope.rect);
            EditorGUI.DrawRect(indentRect, new Color(1, 1, 1, backgroundAlpha));
            return horizontalScope;
        }

        private void DrawArrayFooter(dynamic value)
        {
            using var _ = CreateHorizontalScope(EditorGUI.indentLevel);
            GUILayout.FlexibleSpace();
            DrawAddButton(() =>
            {
                var arrayLength = Enumerable.Count(value.GetDynamicMemberNames());
                var indexes = new object[] { (int) arrayLength - 1 };
                if (value.TryGetIndex(null, indexes, out dynamic lastElement))
                {
                    indexes[0] = (int) arrayLength;
                    value.TrySetIndex(null, indexes, lastElement);
                }
            });
        }

        private void DrawDeleteButton(Action deleteAction)
        {
            GUILayout.Space(6);

            var height = EditorGUIUtility.singleLineHeight;
            var style = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = height,
                fixedHeight = height,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleCenter
            };

            if (GUILayout.Button("-", style))
            {
                GUI.FocusControl(null);
                deleteAction?.Invoke();
            }
        }

        private void DrawAddButton(Action addAction)
        {
            var height = EditorGUIUtility.singleLineHeight;
            var style = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = height * 2,
                fixedHeight = height,
                margin = new RectOffset(0, 35, 0, 5),
                padding = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleCenter
            };

            if (GUILayout.Button("+", style))
            {
                GUI.FocusControl(null);
                addAction?.Invoke();
            }
        }
    }
}