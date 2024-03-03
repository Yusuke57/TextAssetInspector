using UnityEditor;
using UnityEngine;

namespace Yusuke57.CommonPackage.Editor.TextAssetCustom
{
    public class DefaultTextAssetCustomEditor : IPartialTextAssetCustomEditor
    {
        public bool IsTargetFile(TextAsset textAsset)
        {
            return true;
        }

        public string DrawTextAssetContent(TextAsset textAsset)
        {
            var style = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true
            };

            return EditorGUILayout.TextArea(textAsset.text, style);
        }

        public void DrawPagination()
        {
            // Do nothing
        }
    }
}