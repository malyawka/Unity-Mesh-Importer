using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MeshExtensions.ModelPostProcessor
{
    public class MeshPostProcessor : AssetPostprocessor
    {
        ModelPostProcessorSettings postProcessorSettings;

        void OnPreprocessModel()
        {
            postProcessorSettings = ModelPostProcessorSettings.GetOrCreateSettings();

            ModelImporter modelImporter = assetImporter as ModelImporter;

            if (modelImporter != null && postProcessorSettings != null)
            {
                if (postProcessorSettings.AssetAutoConfigEnabled)
                {
                    if (modelImporter.importSettingsMissing)
                    {
                        SetupModelData(modelImporter);
                    }
                }
            }
        }

        void SetupModelData(ModelImporter modelImporter)
        {
            //Scene
            modelImporter.globalScale = postProcessorSettings.ScaleFactor;
            modelImporter.useFileScale = postProcessorSettings.ConvertUnits;
            modelImporter.bakeAxisConversion = postProcessorSettings.BakeAxisConversion;
            modelImporter.importBlendShapes = postProcessorSettings.ImportBlendShapes;
            modelImporter.importVisibility = postProcessorSettings.ImportVisibility;
            modelImporter.importCameras = postProcessorSettings.ImportCameras;
            modelImporter.importLights = postProcessorSettings.ImportLights;
            modelImporter.preserveHierarchy = postProcessorSettings.PreserveHierarchy;
            modelImporter.sortHierarchyByName = postProcessorSettings.SortHierarchyByName;

            //Meshes
            modelImporter.meshCompression = postProcessorSettings.MeshCompression;
            modelImporter.isReadable = postProcessorSettings.ReadWriteEnabled;
            modelImporter.meshOptimizationFlags = postProcessorSettings.meshOptimizationFlags;
            modelImporter.addCollider = postProcessorSettings.GenerateColliders;

            //Geometry;
            modelImporter.keepQuads = postProcessorSettings.KeepQuads;
            modelImporter.weldVertices = postProcessorSettings.WeldVertices;
            modelImporter.indexFormat = postProcessorSettings.IndexFormat;
            modelImporter.importNormals = postProcessorSettings.Normals;
            modelImporter.importBlendShapeNormals = postProcessorSettings.BlendShapeNormals;
            modelImporter.normalCalculationMode = postProcessorSettings.NormalCalculationMode;
            modelImporter.normalSmoothingSource = postProcessorSettings.NormalSmoothingSource;
            modelImporter.normalSmoothingAngle = postProcessorSettings.NormalSmoothingAngle;
            modelImporter.importTangents = postProcessorSettings.Tangents;

            //LightMap
            modelImporter.generateSecondaryUV = postProcessorSettings.GenerateLightMapUV;
            modelImporter.secondaryUVHardAngle = postProcessorSettings.HardAngle;
            modelImporter.secondaryUVAngleDistortion = postProcessorSettings.AngleError; //Под вопросом
            modelImporter.secondaryUVAreaDistortion = postProcessorSettings.AreaError;
            modelImporter.secondaryUVMarginMethod = postProcessorSettings.marginMethod;
            modelImporter.secondaryUVMinLightmapResolution = postProcessorSettings.MinLightmapResolution;
            modelImporter.secondaryUVMinObjectScale = postProcessorSettings.MinObjectScale;

            //Rig
            modelImporter.animationType = postProcessorSettings.animationType;

            //Animation
            modelImporter.importConstraints = postProcessorSettings.ImportConstraints;
            modelImporter.importAnimation = postProcessorSettings.ImportAnimations;
            modelImporter.animationCompression = postProcessorSettings.AnimationCompression;

            //Material
            modelImporter.materialImportMode = postProcessorSettings.MaterialCreationMode;
        }
    }
}