using UnityEditor;
using UnityEngine;

namespace Yusuke57.CommonPackage.Editor.TextAssetCustom
{
    public class PaginationDrawer
    {
        private int _maxPage;

        public int CurrentPage { get; private set; }
        
        public void SetMaxPage(int maxPage)
        {
            _maxPage = maxPage;
        }

        public void DrawPagination()
        {
            using var _ = new GUILayout.HorizontalScope();

            using (new EditorGUI.DisabledGroupScope(CurrentPage <= 0))
            {
                if (GUILayout.Button("<"))
                {
                    CurrentPage--;
                    GUI.FocusControl(null);
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label($"{CurrentPage + 1} / {_maxPage}");
            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledGroupScope(CurrentPage >= _maxPage - 1))
            {
                if (GUILayout.Button(">"))
                {
                    CurrentPage++;
                    GUI.FocusControl(null);
                }
            }
        }
    }
}