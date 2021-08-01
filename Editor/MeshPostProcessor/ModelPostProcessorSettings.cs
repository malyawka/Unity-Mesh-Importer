using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace MeshExtensions.ModelPostProcessor
{
	public class ModelPostProcessorSettings : ScriptableObject
	{
		#region Variables

		#region General_Settings

		[HideInInspector] [SerializeField] public bool AssetAutoConfigEnabled = false;

		#endregion

		#region MeshTab

		[HideInInspector] [SerializeField] public float ScaleFactor = 1f;
		[HideInInspector] [SerializeField] public bool ConvertUnits = true;
		[HideInInspector] [SerializeField] public bool BakeAxisConversion = false;
		[HideInInspector] [SerializeField] public bool ImportBlendShapes = true;
		[HideInInspector] [SerializeField] public bool ImportVisibility = true;
		[HideInInspector] [SerializeField] public bool ImportCameras = true;
		[HideInInspector] [SerializeField] public bool ImportLights = true;
		[HideInInspector] [SerializeField] public bool PreserveHierarchy = false;
		[HideInInspector] [SerializeField] public bool SortHierarchyByName = true;

		[HideInInspector] [SerializeField] public ModelImporterMeshCompression MeshCompression = ModelImporterMeshCompression.Off;
		[HideInInspector] [SerializeField] public bool ReadWriteEnabled = false;
		[HideInInspector] [SerializeField] public MeshOptimizationFlags meshOptimizationFlags = MeshOptimizationFlags.Everything;
		[HideInInspector] [SerializeField] public bool GenerateColliders = false;

		[HideInInspector] [SerializeField] public bool KeepQuads = false;
		[HideInInspector] [SerializeField] public bool WeldVertices = false;
		[HideInInspector] [SerializeField] public ModelImporterIndexFormat IndexFormat = ModelImporterIndexFormat.Auto;
		[HideInInspector] [SerializeField] public ModelImporterNormals Normals = ModelImporterNormals.Import;
		[HideInInspector] [SerializeField] public ModelImporterNormals BlendShapeNormals = ModelImporterNormals.Calculate;
		[HideInInspector] [SerializeField] public ModelImporterNormalCalculationMode NormalCalculationMode = ModelImporterNormalCalculationMode.AreaAndAngleWeighted;
		[HideInInspector] [SerializeField] public ModelImporterNormalSmoothingSource NormalSmoothingSource = ModelImporterNormalSmoothingSource.PreferSmoothingGroups;
		[HideInInspector] [SerializeField] public float NormalSmoothingAngle = 60f;
		[HideInInspector] [SerializeField] public ModelImporterTangents Tangents = ModelImporterTangents.CalculateMikk;
		[HideInInspector] [SerializeField] public bool SwapUV = false;

		[HideInInspector] [SerializeField] public bool GenerateLightMapUV = false;
		[HideInInspector] [SerializeField] public float HardAngle = 88;
		[HideInInspector] [SerializeField] public float AngleError = 8;
		[HideInInspector] [SerializeField] public float AreaError = 15;
		[HideInInspector] [SerializeField] public ModelImporterSecondaryUVMarginMethod marginMethod = ModelImporterSecondaryUVMarginMethod.Calculate;
		[HideInInspector] [SerializeField] public float MinLightmapResolution = 40;
		[HideInInspector] [SerializeField] public float MinObjectScale = 1f;

		#endregion

		#region Rig

		[HideInInspector] [SerializeField] public ModelImporterAnimationType animationType = ModelImporterAnimationType.Generic;

		#endregion

		#region Animation

		[HideInInspector] [SerializeField] public bool ImportConstraints = false;
		[HideInInspector] [SerializeField] public bool ImportAnimations = true;
		[HideInInspector] [SerializeField] public ModelImporterAnimationCompression AnimationCompression = ModelImporterAnimationCompression.Off;

		#endregion

		#region Material

		[HideInInspector] [SerializeField] public ModelImporterMaterialImportMode MaterialCreationMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;

		#endregion

		#endregion

		private static string _path = string.Empty;

		public static SerializedObject GetSerializedSettings()
		{
			return new SerializedObject(GetOrCreateSettings());
		}

		public static ModelPostProcessorSettings GetOrCreateSettings()
		{
			var settings = AssetDatabase.LoadAssetAtPath<ModelPostProcessorSettings>(SettingsPath());
			if (settings == null)
			{
				if (File.Exists(SettingsPath()))
				{
					return null; // Not yet imported, will happen in a 'Reimport All'
				}
				Debug.Log($"Creating new project settings asset for Mesh Importer at '{SettingsPath()}'");
				settings = CreateInstance<ModelPostProcessorSettings>();
				settings.AssetAutoConfigEnabled = false;
				AssetDatabase.CreateAsset(settings, SettingsPath());
				AssetDatabase.SaveAssets();
			}
			return settings;
		}

		private static string SettingsPath([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
		{
			if (string.IsNullOrEmpty(_path))
			{
				List<string> s = new List<string>();
				switch (Application.platform)
				{
					case RuntimePlatform.OSXEditor: s = sourceFilePath.Split('/').ToList(); break;
					case RuntimePlatform.WindowsEditor: s = sourceFilePath.Split('\\').ToList(); break;
					default: s = sourceFilePath.Split('/').ToList(); break;
				}
				int idx = s.FindIndex(x => x == "Assets");
				List<string> c = new List<string>();

				for (int i = 0; i < s.Count - 1; i++)
				{
					if (i >= idx) c.Add(s[i]);
				}

				c.Add("ModelPostProcessorSettings.asset");
				_path = Path.Combine(c.ToArray());
			}

			return _path;
		}

	}
}

