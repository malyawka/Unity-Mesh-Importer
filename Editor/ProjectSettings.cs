using UnityEditor;

namespace MeshExtensions.Editor
{
    public static class ProjectSettings
    {
        private const string autoCollapsePref = "MeshImporter-AutoCollapse";

        public static bool AutoCollapsePostProcessor
        {
            get => EditorPrefs.GetBool(autoCollapsePref, false);
            set => EditorPrefs.SetBool(autoCollapsePref, value);
        }
    }

}