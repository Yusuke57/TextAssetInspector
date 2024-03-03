using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Yusuke57.CommonPackage.Editor.TextAssetCustom
{
    public class CsvTextAssetCustomEditor : IPartialTextAssetCustomEditor
    {
        private readonly PaginationDrawer _paginationDrawer = new();
        
        private static readonly Color _evenRowBackgroundColor = Color.white;
        private static readonly Color _oddRowBackgroundColor = new Color(0.8f, 0.8f, 0.8f);

        private const string Delimiter = ",";
        private const int MaxRowCountInPage = 25;

        public bool IsTargetFile(TextAsset textAsset)
        {
            var path = AssetDatabase.GetAssetPath(textAsset);
            return !string.IsNullOrEmpty(path) && path.EndsWith(".csv");
        }

        public string DrawTextAssetContent(TextAsset textAsset)
        {
            var csvParser = new CsvParser(textAsset.text);
            var stringBuilder = new StringBuilder();
            var previousBackgroundColor = GUI.backgroundColor;
            
            var rowOffset = _paginationDrawer.CurrentPage * MaxRowCountInPage;
            stringBuilder.Append(csvParser.GetRawText(0, rowOffset));
            
            for (var row = rowOffset; row < rowOffset + MaxRowCountInPage; row++)
            {
                GUI.backgroundColor = row % 2 == 0 ? _evenRowBackgroundColor : _oddRowBackgroundColor;

                if (row < csvParser.RowCount)
                {
                    DrawRow(csvParser.Fields[row], stringBuilder);
                }
            }
            
            GUI.backgroundColor = previousBackgroundColor;
            
            stringBuilder.Append(csvParser.GetRawText(rowOffset + MaxRowCountInPage, csvParser.RowCount - rowOffset - MaxRowCountInPage));
            
            _paginationDrawer.SetMaxPage((csvParser.RowCount - 1) / MaxRowCountInPage + 1);

            return stringBuilder.ToString();
        }

        private void DrawRow(List<string> fields, StringBuilder stringBuilder)
        {
            var rowStringBuilder = new StringBuilder();

            using var horizontalScope = new EditorGUILayout.HorizontalScope();
            for (var column = 0; column < fields.Count; column++)
            {
                if (column > 0)
                {
                    rowStringBuilder.Append(Delimiter);
                }

                var field = fields[column];
                var text = EditorGUILayout.TextField(field);
                rowStringBuilder.Append(text);
            }

            DrawRowButton("+", () =>
            {
                var newFields = new string[fields.Count];
                rowStringBuilder.Append("\n");
                rowStringBuilder.Append(string.Join(Delimiter, newFields));
            });

            DrawRowButton("-", null, () =>
            {
                stringBuilder.AppendLine(rowStringBuilder.ToString());
            });
        }

        private void DrawRowButton(string label, Action onClick, Action onNotClick = null)
        {
            var height = EditorGUIUtility.singleLineHeight;
            var style = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = height,
                fixedHeight = height,
                margin = new RectOffset(2, 2, 1, 1),
                padding = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleCenter
            };

            if (GUILayout.Button(label, style))
            {
                GUI.FocusControl(null);
                onClick?.Invoke();
            }
            else
            {
                onNotClick?.Invoke();
            }
        }

        public void DrawPagination()
        {
            _paginationDrawer.DrawPagination();
        }
    }
}