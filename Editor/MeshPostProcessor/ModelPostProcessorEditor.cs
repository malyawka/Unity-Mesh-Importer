using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MeshExtensions.ModelPostProcessor
{
	public class MeshPostProcessorEditor : UnityEditor.Editor
	{

		static SerializedObject s;

		#region SerializedProperties

		static SerializedProperty AssetAutoConfigEnabled;
		static SerializedProperty ScaleFactor;
		static SerializedProperty ConvertUnits;
		static SerializedProperty BakeAxisConversion;
		static SerializedProperty ImportBlendShapes;
		static SerializedProperty ImportVisibility;
		static SerializedProperty ImportCameras;
		static SerializedProperty ImportLights;
		static SerializedProperty PreserveHierarchy;
		static SerializedProperty SortHierarchyByName;
		static SerializedProperty MeshCompression;
		static SerializedProperty ReadWriteEnabled;
		static SerializedProperty meshOptimizationFlags;
		static SerializedProperty GenerateColliders;
		static SerializedProperty KeepQuads;
		static SerializedProperty WeldVertices;
		static SerializedProperty IndexFormat;
		static SerializedProperty Normals;
		static SerializedProperty BlendShapeNormals;
		static SerializedProperty NormalCalculationMode;
		static SerializedProperty NormalSmoothingSource;
		static SerializedProperty NormalSmoothingAngle;
		static SerializedProperty Tangents;
		static SerializedProperty SwapUV;
		static SerializedProperty GenerateLightMapUV;
		static SerializedProperty HardAngle;
		static SerializedProperty AngleError;
		static SerializedProperty AreaError;
		static SerializedProperty marginMethod;
		static SerializedProperty MinLightmapResolution;
		static SerializedProperty MinObjectScale;
		static SerializedProperty animationType;
		static SerializedProperty ImportConstraints;
		static SerializedProperty ImportAnimations;
		static SerializedProperty AnimationCompression;
		static SerializedProperty MaterialCreationMode;

		#endregion

		private static void loadProperties()
		{
			AssetAutoConfigEnabled = s.FindProperty("AssetAutoConfigEnabled");
			ScaleFactor = s.FindProperty("ScaleFactor");
			ConvertUnits = s.FindProperty("ConvertUnits");
			BakeAxisConversion = s.FindProperty("BakeAxisConversion");
			ImportBlendShapes = s.FindProperty("ImportBlendShapes");
			ImportVisibility = s.FindProperty("ImportVisibility");
			ImportCameras = s.FindProperty("ImportCameras");
			ImportLights = s.FindProperty("ImportLights");
			PreserveHierarchy = s.FindProperty("PreserveHierarchy");
			SortHierarchyByName = s.FindProperty("SortHierarchyByName");
			MeshCompression = s.FindProperty("MeshCompression");
			ReadWriteEnabled = s.FindProperty("ReadWriteEnabled");
			meshOptimizationFlags = s.FindProperty("meshOptimizationFlags");
			GenerateColliders = s.FindProperty("GenerateColliders");
			KeepQuads = s.FindProperty("KeepQuads");
			WeldVertices = s.FindProperty("WeldVertices");
			IndexFormat = s.FindProperty("IndexFormat");
			Normals = s.FindProperty("Normals");
			BlendShapeNormals = s.FindProperty("BlendShapeNormals");
			NormalCalculationMode = s.FindProperty("NormalCalculationMode");
			NormalSmoothingSource = s.FindProperty("NormalSmoothingSource");
			NormalSmoothingAngle = s.FindProperty("NormalSmoothingAngle");
			Tangents = s.FindProperty("Tangents");
			SwapUV = s.FindProperty("SwapUV");
			GenerateLightMapUV = s.FindProperty("GenerateLightMapUV");
			HardAngle = s.FindProperty("HardAngle");
			AngleError = s.FindProperty("AngleError");
			AreaError = s.FindProperty("AreaError");
			marginMethod = s.FindProperty("marginMethod");
			MinLightmapResolution = s.FindProperty("MinLightmapResolution");
			MinObjectScale = s.FindProperty("MinObjectScale");
			animationType = s.FindProperty("animationType");
			ImportConstraints = s.FindProperty("ImportConstraints");
			ImportAnimations = s.FindProperty("ImportAnimations");
			AnimationCompression = s.FindProperty("AnimationCompression");
			MaterialCreationMode = s.FindProperty("MaterialCreationMode");
		}

		[SettingsProvider]
		public static SettingsProvider CreateSettingsProvider()
		{
			SettingsProvider provider = new SettingsProvider("Project/Mesh Importer/Mesh Import Default Settings", SettingsScope.Project)
			{
				guiHandler = searchContext =>
				{
					s = ModelPostProcessorSettings.GetSerializedSettings();
					loadProperties();
					EditorGUILayout.LabelField("Model mesh import default settings", EditorStyles.boldLabel);
					EditorGUILayout.HelpBox(Styles.ProjectInfo.text, MessageType.None);
					GUILayout.Space(30);
					EditorGUILayout.PropertyField(s.FindProperty("AssetAutoConfigEnabled"), new GUIContent("Enable"));
					EditorGUILayout.Space();
					//EditorGUILayout.HelpBox(Styles.ProjectTip.text, MessageType.Info);
					
					if(AssetAutoConfigEnabled.boolValue)
					{
						GUILayout.Space(10);
						GUILayout.Label(EditorGUIUtility.TrTextContent("Model"), EditorStyles.boldLabel);
						EditorGUILayout.PropertyField(ScaleFactor);
						EditorGUILayout.PropertyField(ConvertUnits);
						EditorGUILayout.PropertyField(BakeAxisConversion);
						EditorGUILayout.PropertyField(ImportBlendShapes);
						EditorGUILayout.PropertyField(ImportVisibility);
						EditorGUILayout.PropertyField(ImportCameras);
						EditorGUILayout.PropertyField(ImportLights);
						EditorGUILayout.PropertyField(PreserveHierarchy);
						EditorGUILayout.PropertyField(SortHierarchyByName);
						EditorGUILayout.PropertyField(MeshCompression);
						EditorGUILayout.PropertyField(ReadWriteEnabled);
						EditorGUILayout.PropertyField(meshOptimizationFlags);
						EditorGUILayout.PropertyField(GenerateColliders);
						EditorGUILayout.PropertyField(KeepQuads);
						EditorGUILayout.PropertyField(WeldVertices);
						EditorGUILayout.PropertyField(IndexFormat);
						EditorGUILayout.PropertyField(Normals);
						EditorGUILayout.PropertyField(BlendShapeNormals);
						EditorGUILayout.PropertyField(NormalCalculationMode);
						EditorGUILayout.PropertyField(NormalSmoothingSource);
						EditorGUILayout.PropertyField(NormalSmoothingAngle);
						EditorGUILayout.PropertyField(Tangents);
						EditorGUILayout.PropertyField(SwapUV);

						EditorGUILayout.PropertyField(GenerateLightMapUV);
						if(GenerateLightMapUV.boolValue)
						{
							EditorGUILayout.PropertyField(HardAngle);
							EditorGUILayout.PropertyField(AngleError);
							EditorGUILayout.PropertyField(AreaError);
							EditorGUILayout.PropertyField(marginMethod);
							EditorGUILayout.PropertyField(MinLightmapResolution);
							EditorGUILayout.PropertyField(MinObjectScale);
						}

						GUILayout.Space(10);
						GUILayout.Label(EditorGUIUtility.TrTextContent("Rig"), EditorStyles.boldLabel);
						EditorGUILayout.PropertyField(animationType);
						GUILayout.Space(10);
						GUILayout.Label(EditorGUIUtility.TrTextContent("Animation"), EditorStyles.boldLabel);
						EditorGUILayout.PropertyField(ImportConstraints);
						EditorGUILayout.PropertyField(ImportAnimations);
						EditorGUILayout.PropertyField(AnimationCompression);
						GUILayout.Space(10);
						GUILayout.Label(EditorGUIUtility.TrTextContent("Material"), EditorStyles.boldLabel);
						EditorGUILayout.PropertyField(MaterialCreationMode);
					}
					s.ApplyModifiedProperties();
				},
				keywords = new HashSet<string>(new[] { "mesh import", "model import" })
			};
			return provider;
		}


		#region Styles
		private static class Styles
		{
			public static GUIContent ProjectInfo = EditorGUIUtility.TrTextContent("These defaults are used when importing a new model asset in the model post-processor.");
			//public static GUIContent ProjectTip = EditorGUIUtility.TrTextContent("Tip: To apply the import settings to your models, re-import them into the project.");
			public static GUIContent ProjectCollapse = EditorGUIUtility.TrTextContent("Enable custom import settings", "");
		}
		
		private static class PropertyStyles
		{
			public static GUIContent Example = EditorGUIUtility.TrTextContent("Example text,","Example discription");
		}
		#endregion
	}
}
