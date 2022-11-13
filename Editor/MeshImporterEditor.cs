using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MeshExtensions.Editor
{
    [CustomEditor(typeof(Mesh))]
    [CanEditMultipleObjects]
    [DefaultExecutionOrder(100000)]
    public class MeshImporterEditor : UnityEditor.Editor
    {
	    #region Variables

        private UnityEditor.Editor _defaultEditor;
        private UserData _userData;
        private bool _modified;
        private Mesh _mesh;
        private ModelImporter _modelImporter;

        #endregion

        #region Properties

        private int IndentLevel { get { return EditorGUI.indentLevel; } set { EditorGUI.indentLevel = value; } }

        private Mesh GetMesh { get { if (!_mesh) _mesh = target as Mesh; return _mesh; } }

        private Func<Enum, bool> CheckEnabledMod => f =>
        {
	        return (Modifier) f == Modifier.Combine && !_userData.meshModifiers.Any(m => m.id == GetMesh.name && m.modifier == Modifier.Collapse)||
	               (Modifier) f == Modifier.Manual ||
	               (Modifier) f == Modifier.Mesh ||
	               (Modifier) f == Modifier.Collapse && !_userData.meshModifiers.Any(m => m.id == GetMesh.name && m.modifier == Modifier.Collapse) ||
	               (Modifier) f == Modifier.Bounds ||
	               (Modifier) f == Modifier.None;
        };

        private Func<Enum, bool> CheckEnabled0UVs => f =>
        {
	        if (GetMesh == null) return false;
	        
	        if (_userData == null || _userData.meshModifiers.Length == 0) return true;
	        List<MeshModifier> modifiers = _userData.meshModifiers.ToList();
	        
	        bool isUVs = modifiers.FindIndex(m => m.id == GetMesh.name && m.modifier == Modifier.Collapse) == -1;
	        if (!isUVs) return false;
	        
	        bool isUV0 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV0 || m.uVs[1] == UVChannel.UV0)) == -1;
	        bool isUV1 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV1 || m.uVs[1] == UVChannel.UV1)) == -1;
	        bool isUV2 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV2 || m.uVs[1] == UVChannel.UV2)) == -1;
	        bool isUV3 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV3 || m.uVs[1] == UVChannel.UV3)) == -1;
	        bool isUV4 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV4 || m.uVs[1] == UVChannel.UV4)) == -1;
	        bool isUV5 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV5 || m.uVs[1] == UVChannel.UV5)) == -1;
	        bool isUV6 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV6 || m.uVs[1] == UVChannel.UV6)) == -1;
	        bool isUV7 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV7 || m.uVs[1] == UVChannel.UV7)) == -1;

	        return (UVChannel) f == UVChannel.UV0 && isUV0 && GetMesh.uv != null && GetMesh.uv.Length > 0 ||
	               (UVChannel) f == UVChannel.UV1 && !_modelImporter.generateSecondaryUV && isUV1 && GetMesh.uv2 != null && GetMesh.uv2.Length > 0 ||
	               (UVChannel) f == UVChannel.UV2 && isUV2 && GetMesh.uv3 != null && GetMesh.uv3.Length > 0 ||
	               (UVChannel) f == UVChannel.UV3 && isUV3 && GetMesh.uv4 != null && GetMesh.uv4.Length > 0 ||
	               (UVChannel) f == UVChannel.UV4 && isUV4 && GetMesh.uv5 != null && GetMesh.uv5.Length > 0 ||
	               (UVChannel) f == UVChannel.UV5 && isUV5 && GetMesh.uv6 != null && GetMesh.uv6.Length > 0 ||
	               (UVChannel) f == UVChannel.UV6 && isUV6 && GetMesh.uv7 != null && GetMesh.uv7.Length > 0 ||
	               (UVChannel) f == UVChannel.UV7 && isUV7 && GetMesh.uv8 != null && GetMesh.uv8.Length > 0 ||
	               (UVChannel) f == UVChannel.None;
        };
        
        private Func<Enum, bool> CheckEnabled1UVs => f =>
        {
	        if (GetMesh == null) return false;
	        
	        if (_userData == null || _userData.meshModifiers.Length == 0) return true;
	        List<MeshModifier> modifiers = _userData.meshModifiers.ToList();
	        
	        bool isUVs = modifiers.FindIndex(m => m.id == GetMesh.name && m.modifier == Modifier.Collapse) == -1;
	        if (!isUVs) return false;
	        
	        bool isUV0 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV0 || m.uVs[1] == UVChannel.UV0)) == -1;
	        bool isUV1 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV1 || m.uVs[1] == UVChannel.UV1)) == -1;
	        bool isUV2 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV2 || m.uVs[1] == UVChannel.UV2)) == -1;
	        bool isUV3 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV3 || m.uVs[1] == UVChannel.UV3)) == -1;
	        bool isUV4 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV4 || m.uVs[1] == UVChannel.UV4)) == -1;
	        bool isUV5 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV5 || m.uVs[1] == UVChannel.UV5)) == -1;
	        bool isUV6 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV6 || m.uVs[1] == UVChannel.UV6)) == -1;
	        bool isUV7 = modifiers.FindIndex(m => m.id == GetMesh.name && (m.uVs[0] == UVChannel.UV7 || m.uVs[1] == UVChannel.UV7)) == -1;

	        return (UVChannel) f == UVChannel.UV0 && isUV0 && GetMesh.uv != null && GetMesh.uv.Length > 0 ||
	               (UVChannel) f == UVChannel.UV1 && isUV1 && GetMesh.uv2 != null && GetMesh.uv2.Length > 0 ||
	               (UVChannel) f == UVChannel.UV2 && isUV2 && GetMesh.uv3 != null && GetMesh.uv3.Length > 0 ||
	               (UVChannel) f == UVChannel.UV3 && isUV3 && GetMesh.uv4 != null && GetMesh.uv4.Length > 0 ||
	               (UVChannel) f == UVChannel.UV4 && isUV4 && GetMesh.uv5 != null && GetMesh.uv5.Length > 0 ||
	               (UVChannel) f == UVChannel.UV5 && isUV5 && GetMesh.uv6 != null && GetMesh.uv6.Length > 0 ||
	               (UVChannel) f == UVChannel.UV6 && isUV6 && GetMesh.uv7 != null && GetMesh.uv7.Length > 0 ||
	               (UVChannel) f == UVChannel.UV7 && isUV7 && GetMesh.uv8 != null && GetMesh.uv8.Length > 0 ||
	               (UVChannel) f == UVChannel.None;
        };
        
        private Func<Enum, bool> CheckEnabledEntities => f =>
        {
	        if (_userData == null || _userData.meshModifiers.Length == 0) return true;
	        List<MeshModifier> modifiers = _userData.meshModifiers.ToList();
	        
	        bool isUVs = modifiers.FindIndex(m => m.modifier == Modifier.Collapse) == -1;
	        bool isPosition = modifiers.FindIndex(m => m.entities[0] == Entity.Position) == -1;
	        bool isNormal = modifiers.FindIndex(m => m.entities[0] == Entity.Normal) == -1;
	        bool isTangent = modifiers.FindIndex(m => m.entities[0] == Entity.Tangent) == -1;
	        bool isColor = modifiers.FindIndex(m => m.entities[0] == Entity.Color) == -1;

	        return (Entity) f == Entity.UV && isUVs ||
	               (Entity) f == Entity.Position && isPosition ||
	               (Entity) f == Entity.Normal && isNormal ||
	               (Entity) f == Entity.Tangent && isTangent ||
	               (Entity) f == Entity.Color && isColor ||
	               (Entity) f == Entity.None;
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
		        if (GetMesh != null )
		        {
			        string assetPath = AssetDatabase.GetAssetPath(GetMesh);
			        int exit = EditorUtility.DisplayDialogComplex("Unapplied import settings",
				        $"Unapplied import settings for mesh '{GetMesh.name}' with '{assetPath}'.",
				        "Apply", "Cancel", "Revert");

			        if (exit == 0) ApplyAndReimport();
		        }
	        }

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

	        if (_userData != null && _userData.meshModifiers.ToList().FindIndex(m => m.id == GetMesh.name) != -1)
	        {
		        int showedIdx = 0;
		        
		        for (var i = 0; i < _userData.meshModifiers.Length; i++)
		        {
			        if (_userData.meshModifiers[i].id != target.name)
			        {
				        continue;
			        }
			        
			        Modifier modifier = _userData.meshModifiers[i].modifier;
			        bool isExpanded = BeginFoldoutBox(NameFunction(i), showedIdx, i);

			        if (isExpanded)
			        {
				        IndentLevel++;
				        switch (modifier)
				        {
					        case Modifier.Combine: DrawCombine(i); break;
					        case Modifier.Manual: DrawManual(i); break;
					        case Modifier.Mesh: DrawMesh(i); break;
					        case Modifier.Collapse: DrawCollapse(i); break;
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
            if (_userData.meshModifiers.Length <= idx) return;
            
            EditorGUILayout.BeginVertical();
            
            EditorGUI.BeginChangeCheck();
            Modifier func = (Modifier) EditorGUILayout.EnumPopup(Styles.Modifier, _userData.meshModifiers[idx].modifier,
	            CheckEnabledMod, true);
            if (EditorGUI.EndChangeCheck())
            {
	            _userData.meshModifiers[idx].modifier = func;
	            _modified = true;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Styles.UVChanelCombine, EditorStyles.label);
            
            EditorGUI.BeginChangeCheck();
            UVChannel uv0 = (UVChannel) EditorGUILayout.EnumPopup(Styles.UVChanelTo, _userData.meshModifiers[idx].uVs[0],
	            CheckEnabled0UVs, true, GUILayout.Width(70));
            if (EditorGUI.EndChangeCheck())
            {
	            _userData.meshModifiers[idx].uVs[0] = uv0;
	            _modified = true;
            }
            
            EditorGUILayout.LabelField(new GUIContent("<<"), GUILayout.Width(35));

            EditorGUI.BeginChangeCheck();
            UVChannel uv1 = (UVChannel) EditorGUILayout.EnumPopup(Styles.UVChanelFrom, _userData.meshModifiers[idx].uVs[1],
	            CheckEnabled1UVs, true, GUILayout.Width(70));
            if (EditorGUI.EndChangeCheck())
            {
	            _userData.meshModifiers[idx].uVs[1] = uv1;
	            _modified = true;
            }
            
            EditorGUILayout.EndHorizontal();

            if (uv0 != UVChannel.None && uv1 != UVChannel.None)
            {
	            var icon = EditorGUIUtility.IconContent("d_ToggleUVOverlay"); //d_ToggleUVOverlay@2x;
	            GUIStyle style = new GUIStyle(GUI.skin.label) { richText = true };
	            EditorGUILayout.LabelField(new GUIContent($"  {uv0}.<b><color=#EC7063>x</color><color=#3498DB>y</color></b> + " +
	                                                      $"{uv1}.<b><color=#2ECC71>x</color><color=#F5B041>y</color></b> = " +
	                                                      $"{uv0}.<b><color=#EC7063>x</color><color=#3498DB>y</color>" +
	                                                      $"<color=#2ECC71>z</color><color=#F5B041>w</color></b>", icon.image), style); 
            }
            
            EditorGUILayout.EndVertical();
            
            Space(2);
        }
        
        private void DrawManual(int idx)
        {
	        if (_userData.meshModifiers.Length <= idx) return;

	        EditorGUILayout.BeginVertical();
            
	        EditorGUI.BeginChangeCheck();
	        Modifier func = (Modifier) EditorGUILayout.EnumPopup(Styles.Modifier, _userData.meshModifiers[idx].modifier,
		        CheckEnabledMod, true);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshModifiers[idx].modifier = func;
		        _modified = true;
	        }
	        
	        EditorGUI.BeginChangeCheck();
	        Entity ent0 = (Entity) EditorGUILayout.EnumPopup(Styles.Array, _userData.meshModifiers[idx].entities[0],
		        CheckEnabledEntities, true);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshModifiers[idx].entities[0] = ent0;
		        _modified = true;
	        }

	        if (_userData.meshModifiers[idx].entities[0] == Entity.UV)
	        {
		        IndentLevel++;
		        EditorGUI.BeginChangeCheck();
		        UVChannel uv0 = (UVChannel) EditorGUILayout.EnumPopup(Styles.UVChanel,
			        _userData.meshModifiers[idx].uVs[0], CheckEnabled0UVs, true);
		        if (EditorGUI.EndChangeCheck())
		        {
			        _userData.meshModifiers[idx].uVs[0] = uv0;
			        _modified = true;
		        }
		        IndentLevel--;
	        }

	        if (ent0 != Entity.None)
	        {
		        if (ent0 == Entity.Position || ent0 == Entity.Normal)
		        {
			        EditorGUI.BeginChangeCheck();
			        Vector3 vec0 = EditorGUILayout.Vector3Field(Styles.Point, _userData.meshModifiers[idx].points[0]);
			        if (EditorGUI.EndChangeCheck())
			        {
				        _userData.meshModifiers[idx].points[0] = vec0;
				        _modified = true;
			        }
		        }
		        else
		        {
			        EditorGUI.BeginChangeCheck();
			        Vector4 vec0 = EditorGUILayout.Vector4Field(Styles.Point, _userData.meshModifiers[idx].points[0]);
			        if (EditorGUI.EndChangeCheck())
			        {
				        _userData.meshModifiers[idx].points[0] = vec0;
				        _modified = true;
			        }
		        }
	        }
	        
	        EditorGUILayout.EndVertical();
	        
	        Space(2);
        }
        
        private void DrawMesh(int idx)
        {
	        if (_userData.meshModifiers.Length <= idx) return;

	        EditorGUILayout.BeginVertical();
            
	        EditorGUI.BeginChangeCheck();
	        Modifier func = (Modifier) EditorGUILayout.EnumPopup(Styles.Modifier, _userData.meshModifiers[idx].modifier,
		        CheckEnabledMod, true);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshModifiers[idx].modifier = func;
		        _modified = true;
	        }
	        
	        EditorGUI.BeginChangeCheck();
	        Entity ent0 = (Entity) EditorGUILayout.EnumPopup(Styles.Array, _userData.meshModifiers[idx].entities[0],
		        CheckEnabledEntities, true);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshModifiers[idx].entities[0] = ent0;
		        _modified = true;
	        }

	        if (_userData.meshModifiers[idx].entities[0] == Entity.UV)
	        {
		        IndentLevel++;
		        EditorGUI.BeginChangeCheck();
		        UVChannel uv0 = (UVChannel) EditorGUILayout.EnumPopup(Styles.UVChanel,
			        _userData.meshModifiers[idx].uVs[0], CheckEnabled0UVs, true);
		        if (EditorGUI.EndChangeCheck())
		        {
			        _userData.meshModifiers[idx].uVs[0] = uv0;
			        _modified = true;
		        }
		        IndentLevel--;
	        }
	        
	        EditorGUI.BeginChangeCheck();
	        Object obj1 = EditorGUILayout.ObjectField(Styles.Mesh,
		        _userData.meshModifiers[idx].objects[0] ? (Mesh) _userData.meshModifiers[idx].objects[0] : null,
		        typeof(Mesh), false);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshModifiers[idx].objects[0] = obj1;
		        _modified = true;
	        }

	        if (obj1)
	        {
		        EditorGUI.BeginChangeCheck();
		        Entity ent1 = (Entity) EditorGUILayout.EnumPopup(Styles.MeshArray, _userData.meshModifiers[idx].entities[1]);
		        if (EditorGUI.EndChangeCheck())
		        {
			        _userData.meshModifiers[idx].entities[1] = ent1;
			        _modified = true;
		        }

		        if (_userData.meshModifiers[idx].entities[1] == Entity.UV)
		        {
			        IndentLevel++;
			        EditorGUI.BeginChangeCheck();
			        UVChannel uv1 = (UVChannel) EditorGUILayout.EnumPopup(Styles.MeshUVChannel, _userData.meshModifiers[idx].uVs[1]);
			        if (EditorGUI.EndChangeCheck())
			        {
				        _userData.meshModifiers[idx].uVs[1] = uv1;
				        _modified = true;
			        }
			        IndentLevel--;
		        }
	        }

	        EditorGUILayout.EndVertical();
	        
	        Space(2);
        }

        private void DrawBounds(int idx)
        {
	        if (_userData.meshModifiers.Length <= idx) return;

	        EditorGUILayout.BeginVertical();
            
	        EditorGUI.BeginChangeCheck();
	        Modifier func = (Modifier) EditorGUILayout.EnumPopup(Styles.Modifier, _userData.meshModifiers[idx].modifier,
		        CheckEnabledMod, true);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshModifiers[idx].modifier = func;
		        _modified = true;
	        }
	        
	        EditorGUI.BeginChangeCheck();
	        Vector3 vec0 = EditorGUILayout.Vector3Field(Styles.BoundsCenter, _userData.meshModifiers[idx].points[0]);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshModifiers[idx].points[0] = vec0;
		        _modified = true;
	        }
	        
	        EditorGUI.BeginChangeCheck();
	        Vector3 vec1 = EditorGUILayout.Vector3Field(Styles.BoundsSize, _userData.meshModifiers[idx].points[1]);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshModifiers[idx].points[1] = vec1;
		        _modified = true;
	        }

	        EditorGUILayout.EndVertical();
	        
	        Space(2);
        }
        
        private void DrawCollapse(int idx)
        {
	        if (_userData.meshModifiers.Length <= idx) return;
	        
	        if (!GetMesh) return;

	        EditorGUILayout.BeginVertical();
	        
	        EditorGUI.BeginChangeCheck();
	        Modifier func = (Modifier) EditorGUILayout.EnumPopup(Styles.Modifier, _userData.meshModifiers[idx].modifier,
		        CheckEnabledMod, true);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshModifiers[idx].modifier = func;
		        _modified = true;
	        }
	        
	        if (_userData.meshModifiers[idx].folds == null)
	        {
		        _userData.meshModifiers[idx].folds = GetMesh.GetFolds(_modelImporter.generateSecondaryUV);
	        }

	        var icon = EditorGUIUtility.IconContent("d_ToggleUVOverlay"); //d_ToggleUVOverlay@2x;
	        if (_modelImporter.generateSecondaryUV)
	        {
		        EditorGUILayout.LabelField(new GUIContent($"  Lightmap = UV1", icon.image));  
	        }
	        foreach (UVFold fold in _userData.meshModifiers[idx].folds)
	        {
		        GUIStyle style = new GUIStyle(GUI.skin.label) { richText = true };
		        EditorGUILayout.LabelField(new GUIContent($"  {fold.xy}.<b><color=#EC7063>x</color><color=#3498DB>y</color></b> + " +
		                                                  $"{fold.zw}.<b><color=#2ECC71>x</color><color=#F5B041>y</color></b> = " +
		                                                  $"{fold.origin}.<b><color=#EC7063>x</color><color=#3498DB>y</color>" +
		                                                  $"<color=#2ECC71>z</color><color=#F5B041>w</color></b>", icon.image), style);    
	        }

	        EditorGUILayout.EndVertical();
	        
	        Space(2);
        }
        
        private void DrawNone(int idx)
        {
	        if (_userData.meshModifiers.Length <= idx) return;

	        EditorGUILayout.BeginVertical();
            
	        EditorGUI.BeginChangeCheck();
	        Modifier func = (Modifier) EditorGUILayout.EnumPopup(Styles.Modifier, _userData.meshModifiers[idx].modifier,
		        CheckEnabledMod, true);
	        if (EditorGUI.EndChangeCheck())
	        {
		        _userData.meshModifiers[idx].modifier = func;
		        _modified = true;
	        }

	        EditorGUILayout.EndVertical();
	        
	        Space(2);
        }

        private void DrawButtons()
        {
	        Space(2);
	        
	        EditorGUILayout.BeginHorizontal();
	        
	        if (GUILayout.Button(Styles.Reset, GUILayout.Width(50)))
	        {
		        ResetAndReimport();
	        }
	        
	        if (_userData != null && _userData.meshModifiers.ToList().FindIndex(m => m.id == GetMesh.name) != -1 || _modified)
	        {
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
	        }
	        
	        EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Settings Provider Methods

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
	        SettingsProvider provider = new SettingsProvider("Project/Mesh Importer", SettingsScope.Project)
	        {
		        guiHandler = searchContext =>
		        {
			        bool autoCollapse = ProjectSettings.AutoCollapsePostProcessor;
			        
			        EditorGUILayout.LabelField("Model mesh import defaults", EditorStyles.boldLabel);
			        EditorGUILayout.HelpBox(Styles.ProjectInfo.text, MessageType.None);
			        
			        EditorGUI.BeginChangeCheck();
			        autoCollapse = EditorGUILayout.Toggle(Styles.ProjectCollapse, autoCollapse);
			        if (EditorGUI.EndChangeCheck())
			        {
				        ProjectSettings.AutoCollapsePostProcessor = autoCollapse;
			        }
			        
			        EditorGUILayout.Space();
			        EditorGUILayout.HelpBox(Styles.ProjectTip.text, MessageType.Info);
		        },
		        keywords = new HashSet<string>(new [] { "auto collapse post processor", "mesh import", "model import" })
	        };
	        return provider;
        }

        #endregion

        #region Private Methods

        private string NameFunction(int idx)
        {
	        MeshModifier f = _userData.meshModifiers[idx];
	        switch (f.modifier)
	        {
		        case Modifier.Combine: return $"  Combine {f.uVs[1]} to {f.uVs[0]}";
		        case Modifier.Manual: return $"  Manual {f.entities[0]} to {f.points[0]}";
		        case Modifier.Mesh:
			        return !f.objects[0] ? "  Mesh not selected" : f.entities[0] == Entity.UV ? 
				        $"  Copy {f.uVs[0]} from {f.objects[0].name} {f.uVs[1]}" : 
				        $"  Copy {f.entities[0]} from {f.objects[0].name} {f.entities[1]}";
		        case Modifier.Bounds: return $"  Bounds Center {(Vector3)f.points[0]} Size {(Vector3)f.points[1]}";
		        case Modifier.Collapse: return "  Collapse UVs";
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
	        List<MeshModifier> t = _userData.meshModifiers.ToList();
	        t.Add(f);
	        _userData.meshModifiers = t.ToArray();
        }

        private void DeleteFunction(int idx)
        {
	        List<MeshModifier> t = _userData.meshModifiers.ToList();
	        t.RemoveAt(idx);
	        _userData.meshModifiers = t.ToArray();
        }

        private void InitAndReset()
        {
	        if (!GetMesh) return;
            
            AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(GetMesh));
            _userData = JsonUtility.FromJson<UserData>(assetImporter.userData);
            _modelImporter = (ModelImporter) assetImporter;
            _modified = false;
        }

        private void ApplyAndReimport()
        {
	        if (!GetMesh) return;
            
            AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(GetMesh));
            if (_userData != null && _userData.meshModifiers.Length > 0)
            {
	            ClearCollapseUVs();
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
        
        private void ResetAndReimport()
        {
	        if (!GetMesh) return;
	        
	        string assetPath = AssetDatabase.GetAssetPath(GetMesh);
	        bool reset = EditorUtility.DisplayDialog("Reset Modifiers",
		        $"Are you sure you want to remove all modifiers from the fbx object: '{assetPath}'?.",
		        "Reset", "Cancel");

	        if (reset)
	        {
		        _userData = new UserData();
		        AssetImporter assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(GetMesh));
		        assetImporter.userData = string.Empty;
		        
		        EditorUtility.SetDirty(assetImporter);
		        assetImporter.SaveAndReimport();
		        
		        _modified = false;
	        }
        }
        
        private bool ClearTargetModifiers()
        {
	        if (_userData == null || _userData.meshModifiers.Length == 0) return false;
	        List<MeshModifier> modifiers = _userData.meshModifiers.ToList();

	        bool cleared = false;
	        for (int i = 0; i < modifiers.Count; i++)
	        {
		        if (modifiers[i].id == GetMesh.name)
		        {
			        cleared = true;
			        modifiers.RemoveAt(i);
			        i--;
		        }
	        }

	        if (cleared)
	        {
		        _userData.meshModifiers = modifiers.ToArray();
	        }
	        
	        return cleared;
        }

        private bool ClearCollapseUVs()
        {
	        if (_userData == null || _userData.meshModifiers.Length == 0) return false;
	        List<MeshModifier> modifiers = _userData.meshModifiers.ToList();

	        bool cleared = false;
	        bool isCollapse = modifiers.FindIndex(m => m.id == GetMesh.name && m.modifier == Modifier.Collapse) != -1;
	        if (isCollapse)
	        {
		        for (int i = 0; i < modifiers.Count; i++)
		        {
			        if (modifiers[i].id == GetMesh.name && modifiers[i].modifier == Modifier.Combine ||
			            modifiers[i].id == GetMesh.name && modifiers[i].modifier == Modifier.Manual && modifiers[i].entities[0] == Entity.UV ||
			            modifiers[i].id == GetMesh.name && modifiers[i].modifier == Modifier.Mesh && modifiers[i].entities[0] == Entity.UV)
			        {
				        cleared = true;
				        modifiers.RemoveAt(i);
				        i--;
			        }
		        }
	        }

	        if (cleared)
	        {
		        _userData.meshModifiers = modifiers.ToArray();
	        }
	        
	        return cleared;
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
	        public static GUIContent ProjectInfo = EditorGUIUtility.TrTextContent("These defaults are used when importing a new model asset in the model post-processor. Note that it is always possibe to change the initial values manually on mesh asset basis by selecting the asset in the inspector window.");
	        public static GUIContent ProjectTip = EditorGUIUtility.TrTextContent("Tip: To apply the import settings to your models, re-import them into the project.");
	        public static GUIContent ProjectCollapse = EditorGUIUtility.TrTextContent("Auto Collapse UVs", "For all meshes that are not manually configured, all UVs will automatically collapse.");
	        public static GUIContent Add = EditorGUIUtility.TrTextContent("Add", "Add a new mesh modifier.");
	        public static GUIContent Delete = EditorGUIUtility.TrTextContent("Delete", "Remove the mesh modifier.");
	        public static GUIContent Reset = EditorGUIUtility.TrTextContent("Reset", "Delete all modifiers fr0m the mesh.");
	        public static GUIContent Apply = EditorGUIUtility.TrTextContent("Apply", "Apply modifiers to the mesh.");
	        public static GUIContent Revert = EditorGUIUtility.TrTextContent("Revert", "Revert changes in mesh modifiers.");
	        public static GUIContent Array = EditorGUIUtility.TrTextContent("Array", "Select the array to which the modifier will be applied.");
	        public static GUIContent Point = EditorGUIUtility.TrTextContent("Point", "Set the value to be written to array.");
	        public static GUIContent UVChanel = EditorGUIUtility.TrTextContent("UV Chanel", "Select a UV channel to refine the array.");
	        public static GUIContent Mesh = EditorGUIUtility.TrTextContent("Mesh", $"Select the mesh fr0m which the array will be copied.");
	        public static GUIContent MeshArray = EditorGUIUtility.TrTextContent("Array", "Select the array fr0m which the data will be copied to the main array.");
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
