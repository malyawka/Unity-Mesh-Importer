using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MeshExtensions.Editor
{
    [CustomEditor(typeof(Mesh))]
    [CanEditMultipleObjects]
    public class MeshImporterEditor : UnityEditor.Editor
    {
	    #region Variables

        private UnityEditor.Editor _defaultEditor;
        private UserData _userData;
        private bool _modified;

        #endregion

        #region Properties

        private int IndentLevel { get { return EditorGUI.indentLevel; } set { EditorGUI.indentLevel = value; } }
        private Func<Enum, bool> CheckEnabledMod => f =>
        {
	        return (Modifier) f == Modifier.Combine ||
	               (Modifier) f == Modifier.Manual ||
	               (Modifier) f == Modifier.Mesh ||
	               (Modifier) f == Modifier.Bounds ||
	               (Modifier) f == Modifier.None;
        };

        private Func<Enum, bool> CheckEnabledUVs => f =>
        {
	        Mesh mesh = target as Mesh;
	        if (mesh == null) return false;

	        return (UVChannel) f == UVChannel.UV0 && mesh.uv != null && mesh.uv.Length > 0 ||
	               (UVChannel) f == UVChannel.UV1 && mesh.uv2 != null && mesh.uv2.Length > 0 ||
	               (UVChannel) f == UVChannel.UV2 && mesh.uv3 != null && mesh.uv3.Length > 0 ||
	               (UVChannel) f == UVChannel.UV3 && mesh.uv4 != null && mesh.uv4.Length > 0 ||
	               (UVChannel) f == UVChannel.UV4 && mesh.uv5 != null && mesh.uv5.Length > 0 ||
	               (UVChannel) f == UVChannel.UV5 && mesh.uv6 != null && mesh.uv6.Length > 0 ||
	               (UVChannel) f == UVChannel.UV6 && mesh.uv7 != null && mesh.uv7.Length > 0 ||
	               (UVChannel) f == UVChannel.UV7 && mesh.uv8 != null && mesh.uv8.Length > 0;
        };

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            _defaultEditor = CreateEditor(targets, Type.GetType("UnityEditor.ModelInspector, UnityEditor"));
            InitAndReset();
        }

        private void OnDisable()
        {
	        if (_modified)
	        {
		        Mesh mesh = target as Mesh;
		        if (mesh != null )
		        {
			        string assetPath = AssetDatabase.GetAssetPath(mesh);
			        int exit = EditorUtility.DisplayDialogComplex("Unapplied import settings",
				        $"Unapplied import settings for mesh '{mesh.name}' with '{assetPath}'.",
				        "Apply", "Cancel", "Revert");

			        if (exit == 0) ApplyAndReimport();
		        }
	        }
	        
	        MethodInfo disableMethod = _defaultEditor.GetType().GetMethod("OnDisable",
		        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
	        disableMethod?.Invoke(_defaultEditor,null);

	        DestroyImmediate(_defaultEditor);
        }
        
        public override void OnInspectorGUI()
        {
            bool guiEnabled = GUI.enabled;
            GUI.enabled = true;
            
            if (targets.Length == 1 && target != null)
            {
	            DrawFunctions();

                Space(2);

                DrawButtons();
            }

            GUI.enabled = guiEnabled;
            
            _defaultEditor.OnInspectorGUI();
        }

        public override void OnPreviewSettings() => _defaultEditor.OnPreviewSettings();

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background) => _defaultEditor.OnInteractivePreviewGUI(r, background);
        public override void OnPreviewGUI(Rect r, GUIStyle background) => _defaultEditor.OnPreviewGUI(r, background);

        public override bool HasPreviewGUI() => _defaultEditor.HasPreviewGUI();

        #endregion

        #region Drawing Methods

        private void DrawFunctions()
        {
	        EditorGUILayout.BeginHorizontal();
	        DrawBoldLabel("Mesh Import Extension");
	        bool add = GUILayout.Button(Styles.Add, GUILayout.Width(50));
	        EditorGUILayout.EndHorizontal();

	        if (_userData != null && _userData.meshFunctions.Length > 0)
	        {
		        int showedIdx = 0;
	        
		        for (var i = 0; i < _userData.meshFunctions.Length; i++)
		        {
			        Modifier modifier = _userData.meshFunctions[i].modifier;
			        bool isExpanded = BeginFoldoutBox(NameFunction(i), showedIdx, i);

			        if (isExpanded)
			        {
				        IndentLevel++;
				        switch (modifier)
				        {
					        case Modifier.Combine: DrawCombine(i); break;
					        case Modifier.Manual: DrawManual(i); break;
					        case Modifier.Mesh: DrawMesh(i); break;
					        case Modifier.Bounds: DrawBounds(i); break;
					        default: DrawNone(i); break;
				        }
				        IndentLevel--;
			        }

			        EndBox();
                
			        showedIdx++;
		        }
	        }
	        else
	        {
		        EditorGUILayout.HelpBox(Styles.Info.text, MessageType.Info);
	        }
	        
            if (add)
            {
	            AddFunction();
	            _modified = true;
            }
        }

        private void DrawCombine(int idx)
        {
            if (_userData.meshFunctions.Length <= idx) return;
            
            EditorGUILayout.BeginVertical();
            
            EditorGUI.BeginChangeCheck();
            Modifier func = (Modifier) EditorGUILayout.EnumPopup(Styles.Modifier, _userData.meshFunctions[idx].modifier,
	            CheckEnabledMod, true);
            if (EditorGUI.EndChangeCheck())
            {
	            _userData.meshFunctions[idx].modifier =  func;
	            _modified = true;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Styles.UVChanelCombine, EditorStyles.label);
            
            EditorGUI.BeginChangeCheck();
            UVChannel uv0 = (UVChannel) EditorGUILayout.EnumPopup(Styles.UVChanelTo, _userData.meshFunctions[idx].uVs[0],
	            CheckEnabledUVs, true, GUILayout.Width(70));
            if (EditorGUI.EndChangeCheck())
            {
	            _userData.meshFunctions[idx].uVs[0] = uv0;
	            _modified = true;
            }

            EditorGUI.BeginChangeCheck();
            UVChannel uv1 = (UVChannel) EditorGUILayout.EnumPopup(Styles.UVChanelFrom, _userData.meshFunctions[idx].uVs[1],
	            CheckEnabledUVs, true, GUILayout.Width(70));
            if (EditorGUI.EndChangeCheck())
            {
	            _userData.meshFunctions[idx].uVs[1] = uv1;
	            _modified = true;
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawManual(int idx)
        {
	        if (_userData.meshFunctions.Length <= idx) return;

	        EditorGUILayout.BeginVertical();
            
	        EditorGUI.BeginChangeCheck();
	        Modifier func = (Modifier) EditorGUILayout.EnumPopup(Styles.Modifier, _userData.meshFunctions[idx].modifier,
		        CheckEnabledMod, true);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshFunctions[idx].modifier =  func;
		        _modified = true;
	        }
	        
	        EditorGUI.BeginChangeCheck();
	        Entity ent0 = (Entity) EditorGUILayout.EnumPopup(Styles.Array, _userData.meshFunctions[idx].entities[0]);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshFunctions[idx].entities[0] = ent0;
		        _modified = true;
	        }

	        if (_userData.meshFunctions[idx].entities[0] == Entity.UV)
	        {
		        IndentLevel++;
		        EditorGUI.BeginChangeCheck();
		        UVChannel uv0 = (UVChannel) EditorGUILayout.EnumPopup(Styles.UVChanel, _userData.meshFunctions[idx].uVs[0]);
		        if (EditorGUI.EndChangeCheck())
		        {
			        _userData.meshFunctions[idx].uVs[0] = uv0;
			        _modified = true;
		        }
		        IndentLevel--;
	        }

	        if (ent0 == Entity.Position || ent0 == Entity.Normal)
	        {
		        EditorGUI.BeginChangeCheck();
		        Vector3 vec0 = EditorGUILayout.Vector3Field(Styles.Point, _userData.meshFunctions[idx].points[0]);
		        if (EditorGUI.EndChangeCheck())
		        {
			        _userData.meshFunctions[idx].points[0] = vec0;
			        _modified = true;
		        }
	        }
	        else
	        {
		        EditorGUI.BeginChangeCheck();
		        Vector4 vec0 = EditorGUILayout.Vector4Field(Styles.Point, _userData.meshFunctions[idx].points[0]);
		        if (EditorGUI.EndChangeCheck())
		        {
			        _userData.meshFunctions[idx].points[0] = vec0;
			        _modified = true;
		        }
	        }
	        
	        EditorGUILayout.EndVertical();
        }
        
        private void DrawMesh(int idx)
        {
	        if (_userData.meshFunctions.Length <= idx) return;

	        EditorGUILayout.BeginVertical();
            
	        EditorGUI.BeginChangeCheck();
	        Modifier func = (Modifier) EditorGUILayout.EnumPopup(Styles.Modifier, _userData.meshFunctions[idx].modifier,
		        CheckEnabledMod, true);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshFunctions[idx].modifier =  func;
		        _modified = true;
	        }
	        
	        EditorGUI.BeginChangeCheck();
	        Entity ent0 = (Entity) EditorGUILayout.EnumPopup(Styles.Array, _userData.meshFunctions[idx].entities[0]);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshFunctions[idx].entities[0] = ent0;
		        _modified = true;
	        }

	        if (_userData.meshFunctions[idx].entities[0] == Entity.UV)
	        {
		        IndentLevel++;
		        EditorGUI.BeginChangeCheck();
		        UVChannel uv0 = (UVChannel) EditorGUILayout.EnumPopup(Styles.UVChanel, _userData.meshFunctions[idx].uVs[0]);
		        if (EditorGUI.EndChangeCheck())
		        {
			        _userData.meshFunctions[idx].uVs[0] = uv0;
			        _modified = true;
		        }
		        IndentLevel--;
	        }
	        
	        EditorGUI.BeginChangeCheck();
	        Object obj1 = EditorGUILayout.ObjectField(Styles.Mesh,
		        _userData.meshFunctions[idx].objects[0] ? (Mesh) _userData.meshFunctions[idx].objects[0] : null,
		        typeof(Mesh), false);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshFunctions[idx].objects[0] = obj1;
		        _modified = true;
	        }

	        if (obj1)
	        {
		        EditorGUI.BeginChangeCheck();
		        Entity ent1 = (Entity) EditorGUILayout.EnumPopup(Styles.MeshArray, _userData.meshFunctions[idx].entities[1]);
		        if (EditorGUI.EndChangeCheck())
		        {
			        _userData.meshFunctions[idx].entities[1] = ent1;
			        _modified = true;
		        }

		        if (_userData.meshFunctions[idx].entities[1] == Entity.UV)
		        {
			        IndentLevel++;
			        EditorGUI.BeginChangeCheck();
			        UVChannel uv1 = (UVChannel) EditorGUILayout.EnumPopup(Styles.MeshUVChannel, _userData.meshFunctions[idx].uVs[1]);
			        if (EditorGUI.EndChangeCheck())
			        {
				        _userData.meshFunctions[idx].uVs[1] = uv1;
				        _modified = true;
			        }
			        IndentLevel--;
		        }
	        }

	        EditorGUILayout.EndVertical();
        }

        private void DrawBounds(int idx)
        {
	        if (_userData.meshFunctions.Length <= idx) return;

	        EditorGUILayout.BeginVertical();
            
	        EditorGUI.BeginChangeCheck();
	        Modifier func = (Modifier) EditorGUILayout.EnumPopup(Styles.Modifier, _userData.meshFunctions[idx].modifier,
		        CheckEnabledMod, true);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshFunctions[idx].modifier =  func;
		        _modified = true;
	        }
	        
	        EditorGUI.BeginChangeCheck();
	        Vector3 vec0 = EditorGUILayout.Vector3Field(Styles.BoundsCenter, _userData.meshFunctions[idx].points[0]);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshFunctions[idx].points[0] = vec0;
		        _modified = true;
	        }
	        
	        EditorGUI.BeginChangeCheck();
	        Vector3 vec1 = EditorGUILayout.Vector3Field(Styles.BoundsSize, _userData.meshFunctions[idx].points[1]);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshFunctions[idx].points[1] = vec1;
		        _modified = true;
	        }

	        EditorGUILayout.EndVertical();
        }
        
        private void DrawNone(int idx)
        {
	        if (_userData.meshFunctions.Length <= idx) return;

	        EditorGUILayout.BeginVertical();
            
	        EditorGUI.BeginChangeCheck();
	        Modifier func = (Modifier) EditorGUILayout.EnumPopup(Styles.Modifier, _userData.meshFunctions[idx].modifier,
		        CheckEnabledMod, true);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshFunctions[idx].modifier =  func;
		        _modified = true;
	        }

	        EditorGUILayout.EndVertical();
        }

        private void DrawButtons()
        {
	        if (_userData != null && _userData.meshFunctions.Length > 0 || _modified)
	        {
		        Space();
		        
		        EditorGUILayout.BeginHorizontal();
		        GUILayout.FlexibleSpace();
            
		        bool guiEnabled = GUI.enabled;
		        GUI.enabled = _modified;
            
		        if (GUILayout.Button(Styles.Revert, GUILayout.Width(50)))
		        {
			        InitAndReset();
		        }
		        if (GUILayout.Button(Styles.Apply, GUILayout.Width(50)))
		        {
			        ApplyAndReimport();
		        }
            
		        GUI.enabled = guiEnabled;
            
		        EditorGUILayout.EndHorizontal();
	        }
        }

        #endregion

        #region Private Methods

        private string NameFunction(int idx)
        {
	        MeshModifier f = _userData.meshFunctions[idx];
	        switch (f.modifier)
	        {
		        case Modifier.Combine: return $"  Combine {f.uVs[1]} to {f.uVs[0]}";
		        case Modifier.Manual: return $"  Manual {f.entities[0]} to {f.points[0]}";
		        case Modifier.Mesh:
			        return !f.objects[0] ? "  Mesh not selected" : f.entities[0] == Entity.UV ? 
				        $"  Copy {f.uVs[0]} from {f.objects[0].name} {f.uVs[1]}" : 
				        $"  Copy {f.entities[0]} from {f.objects[0].name} {f.entities[1]}";
		        case Modifier.Bounds: return $"  Bounds Center {(Vector3)f.points[0]} Size {(Vector3)f.points[1]}";
		        default: return "  None";
	        }
        }

        private void AddFunction()
        {
	        if (_userData == null)
	        {
		        _userData = new UserData();
	        }
	            
	        MeshModifier f = new MeshModifier(target.name);
	        List<MeshModifier> t = _userData.meshFunctions.ToList();
	        t.Add(f);
	        _userData.meshFunctions = t.ToArray();
        }

        private void DeleteFunction(int idx)
        {
	        List<MeshModifier> t = _userData.meshFunctions.ToList();
	        t.RemoveAt(idx);
	        _userData.meshFunctions = t.ToArray();
        }

        private void InitAndReset()
        {
            Mesh mesh = target as Mesh;
            if (!mesh) return;
            
            AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mesh));
            _userData = JsonUtility.FromJson<UserData>(assetImporter.userData);
            _modified = false;
        }

        private void ApplyAndReimport()
        {
            Mesh mesh = target as Mesh;
            if (!mesh) return;
            
            AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mesh));
            if (_userData != null && _userData.meshFunctions.Length > 0)
            {
	            assetImporter.userData = JsonUtility.ToJson(_userData);
            }
            else
            {
	            assetImporter.userData = string.Empty;
            }
            EditorUtility.SetDirty(assetImporter);
            assetImporter.SaveAndReimport();

            _modified = false;
        }
        
        #endregion

        #region Misc Drawing

        private bool BeginFoldoutBox(string boxTitle, int id, int idx)
        {
            GUIStyle style		= new GUIStyle("HelpBox");
            style.padding.left	= 0;
            style.padding.right	= 0;

            GUILayout.BeginVertical(style);

            if (string.IsNullOrEmpty(boxTitle)) return true;
            
            bool wasExpanded = IsBoxExpanded(id.ToString());
            GUILayout.BeginHorizontal();
                
            bool isExpanded = DrawBoldFoldout(wasExpanded, boxTitle);
                
            Color guiColor = GUI.backgroundColor;
            GUI.backgroundColor *= 0.75f;
            bool isDeleted = GUILayout.Button(Styles.Delete, EditorStyles.miniButton, GUILayout.Width(60));
            GUI.backgroundColor = guiColor;
                
            GUILayout.EndHorizontal();
                
            Space(2);

            if (wasExpanded != isExpanded)
            {
	            if (isExpanded)
	            {
		            SetBoxExpanded(id.ToString());
	            }
	            else
	            {
		            SetBoxCollapsed(id.ToString());
	            }
            }

            if (!isDeleted) return isExpanded;
            
            DeleteFunction(idx);
            _modified = true;

            return isExpanded;
        }
        
        private void EndBox()
		{
			GUILayout.EndVertical();
		}
        
        private void DrawBoldLabel(string text)
		{
			EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
		}
		
		private bool DrawBoldFoldout(bool isExpanded, string text)
		{
			GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);

			foldoutStyle.fontStyle = FontStyle.Bold;

			return EditorGUILayout.Foldout(isExpanded, text, foldoutStyle);
		}

		private bool IsBoxExpanded(string key)
		{
			string[] editorExpandedBoxes = EditorPrefs.GetString("bbb-box-expanded").Split(';');

			for (int i = 0; i < editorExpandedBoxes.Length; i++)
			{
				if (editorExpandedBoxes[i] == key)
				{
					return true;
				}
			}

			return false;
		}
		
		private void SetBoxExpanded(string key)
		{
			string boxExpandedStr = EditorPrefs.GetString("bbb-box-expanded");

			if (!string.IsNullOrEmpty(boxExpandedStr))
			{
				boxExpandedStr += ";";
			}

			boxExpandedStr += key;

			EditorPrefs.SetString("bbb-box-expanded", boxExpandedStr);
		}
		
		private void SetBoxCollapsed(string key)
		{
			string[] editorExpandedBoxes = EditorPrefs.GetString("bbb-box-expanded").Split(';');

			string boxExpandedStr = "";

			for (int i = 0; i < editorExpandedBoxes.Length; i++)
			{
				if (editorExpandedBoxes[i] == key)
				{
					continue;
				}

				if (!string.IsNullOrEmpty(boxExpandedStr))
				{
					boxExpandedStr += ";";
				}

				boxExpandedStr += editorExpandedBoxes[i];
			}

			EditorPrefs.SetString("bbb-box-expanded", boxExpandedStr);
		}

		private void Space()
		{
			EditorGUILayout.Space();
		}

		private void Space(float pixels)
		{
			GUILayout.Space(pixels);
		}
		
        #endregion

        #region Styles

        private static class Styles
        {
	        public static GUIContent Info = EditorGUIUtility.TrTextContent("Modifiers can be added to the import of this mesh. To do this, click on the 'Add' button at the top right.");
	        public static GUIContent Add = EditorGUIUtility.TrTextContent("Add", "Add a new mesh modifier.");
	        public static GUIContent Delete = EditorGUIUtility.TrTextContent("Delete", "Remove the mesh modifier.");
	        public static GUIContent Apply = EditorGUIUtility.TrTextContent("Apply", "Apply modifiers to the mesh.");
	        public static GUIContent Revert = EditorGUIUtility.TrTextContent("Revert", "Revert changes in mesh modifiers.");
	        public static GUIContent Array = EditorGUIUtility.TrTextContent("Array", "Select the array to which the modifier will be applied.");
	        public static GUIContent Point = EditorGUIUtility.TrTextContent("Point", "Set the value to be written to array.");
	        public static GUIContent UVChanel = EditorGUIUtility.TrTextContent("UV Chanel", "Select a UV channel to refine the array.");
	        public static GUIContent Mesh = EditorGUIUtility.TrTextContent("Mesh", "Select the mesh fro" + "m which the array will be copied.");
	        public static GUIContent MeshArray = EditorGUIUtility.TrTextContent("Array", "Select the array " + "from which the data will be copied to the main array.");
	        public static GUIContent MeshUVChannel = EditorGUIUtility.TrTextContent("UV Chanel", "Select a UV channel to refine the array.");
	        public static GUIContent BoundsCenter = EditorGUIUtility.TrTextContent("Bounds Center", "Set values for bounds center.");
	        public static GUIContent BoundsSize = EditorGUIUtility.TrTextContent("Bounds Size", "Set values for bounds size.");
	        public static GUIContent UVChanelCombine = EditorGUIUtility.TrTextContent("UV Chanel", "Select UV channels to combine. The second channel is transferred to the first.");
	        public static GUIContent UVChanelFrom = EditorGUIUtility.TrTextContent("", "This UV channel will be moved to the first one and removed. Its data will be available in the properties of the first UV channel Z and W.");
	        public static GUIContent UVChanelTo = EditorGUIUtility.TrTextContent("", "This UV channel will be the main one.");
	        public static GUIContent Modifier = EditorGUIUtility.TrTextContent("Modifier", "Select the modifier to be applied to the mesh during import.");
        }

        #endregion
    }
}
