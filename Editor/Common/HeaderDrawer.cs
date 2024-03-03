using UnityEngine;

namespace Yusuke57.CommonPackage.Editor.TextAssetCustom
{
    public class HeaderDrawer
    {
        private static HeaderInfo _selectedHeaderInfo = new()
        {
            IsCustom = true,
            IsReadonly = true
        };

        private readonly string[] _tabNames = new[] { "Custom", "Original" };

        public HeaderInfo DrawHeader(bool isCustomTabVisible)
        {
            var isCustomPrevious = _selectedHeaderInfo.IsCustom;
            var isReadonlyPrevious = _selectedHeaderInfo.IsReadonly;

            using var horizontalScope = new GUILayout.HorizontalScope();

            // Custom Tab
            if (isCustomTabVisible)
            {
                var tabIndex = GUILayout.Toolbar(_selectedHeaderInfo.IsCustom ? 0 : 1, _tabNames, GUILayout.MinWidth(200));
                _selectedHeaderInfo.IsCustom = tabIndex == 0;
            }

            // Space
            GUILayout.FlexibleSpace();

            // Readonly Toggle
            var isReadOnly = GUILayout.Toggle(_selectedHeaderInfo.IsReadonly, "ReadOnly");
            _selectedHeaderInfo.IsReadonly = isReadOnly;
            
            var isChanged = isCustomPrevious != _selectedHeaderInfo.IsCustom || isReadonlyPrevious != _selectedHeaderInfo.IsReadonly;
            if (isChanged)
            {
                GUI.FocusControl(null);
            }

            return _selectedHeaderInfo;
        }
    }

    public struct HeaderInfo
    {
        public bool IsCustom;
        public bool IsReadonly;
    }
}