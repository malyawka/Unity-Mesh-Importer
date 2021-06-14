using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MeshExtensions.Editor
{
    public class ProjectSettings : ScriptableObject
    {
        [HideInInspector] [SerializeField] public bool autoCollapsePostProcessor;

        private static string _path = string.Empty;
        
        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        public static ProjectSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<ProjectSettings>(SettingsPath());
            if (settings == null)
            {
                if (File.Exists(SettingsPath()))
                {
                    return null; // Not yet imported, will happen in a 'Reimport All'
                }
                Debug.Log($"Creating new project settings asset for Mesh Importer at '{SettingsPath()}'");
                settings = CreateInstance<ProjectSettings>();
                settings.autoCollapsePostProcessor = false;
                AssetDatabase.CreateAsset(settings, SettingsPath());
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        private static string SettingsPath([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (string.IsNullOrEmpty(_path))
            {
                List<string> s = sourceFilePath.Split('/').ToList();
                int idx = s.FindIndex(x => x == "Assets");
                List<string> c = new List<string>();

                for (int i = 0; i < s.Count - 1; i++)
                {
                    if (i >= idx) c.Add(s[i]);
                }
            
                c.Add("ProjectSettings.asset");
                _path = Path.Combine(c.ToArray());
            }

            return _path;
        }
    }

}