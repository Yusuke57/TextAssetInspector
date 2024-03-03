using UnityEngine;

namespace Yusuke57.CommonPackage.Editor.TextAssetCustom
{
    public interface IPartialTextAssetCustomEditor
    {
        bool IsTargetFile(TextAsset textAsset);
        string DrawTextAssetContent(TextAsset textAsset);
        void DrawPagination();
    }
}