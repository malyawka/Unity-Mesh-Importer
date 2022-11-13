using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MeshExtensions.Editor
{
    public class MeshImporter : AssetPostprocessor
    {
        #region Properties

        public override int GetPostprocessOrder() => -100000;

        #endregion
        
        #region Variables
        
        private UserData _userData;
        private bool _generateSecondaryUV;

        #endregion

        #region Unity Methods

        private void OnPreprocessModel()
        {
            ModelImporter modelImporter = (ModelImporter) assetImporter;
            if (modelImporter.generateSecondaryUV)
            {
                _generateSecondaryUV = true;
                modelImporter.generateSecondaryUV = false;
            }
        }

        private void OnPostprocessModel(GameObject obj)
        {
            ModelImporter modelImporter = (ModelImporter) assetImporter;

            _userData = JsonUtility.FromJson<UserData>(assetImporter.userData);
            _userData ??= new UserData();
            
            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();

            if (_userData.meshModifiers != null && _userData.meshModifiers.Length > 0)
            {
                List<MeshModifier> functions = _userData.meshModifiers.ToList();
            
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    if (functions.FindIndex(f => f.id == meshFilter.sharedMesh.name) == -1) continue;
                
                    List<MeshModifier> currentFunctions = functions.FindAll(f => f.id == meshFilter.sharedMesh.name);
                    foreach (MeshModifier currentFunction in currentFunctions)
                    {
                        switch (currentFunction.modifier)
                        {
                            case Modifier.Combine: Combine(meshFilter, currentFunction); break;
                            case Modifier.Manual: Manual(meshFilter, currentFunction); break;
                            case Modifier.Mesh: FromMesh(meshFilter, currentFunction); break;
                            case Modifier.Collapse: Collapse(meshFilter, currentFunction); break;
                            case Modifier.Bounds: Bounds(meshFilter, currentFunction); break;
                        }
                    }
                }
            }
            else
            {
                if (ProjectSettings.AutoCollapsePostProcessor)
                {
                    List<MeshModifier> functions = new List<MeshModifier>();
                    foreach (MeshFilter meshFilter in meshFilters)
                    {
                        MeshModifier m = new MeshModifier(meshFilter.sharedMesh.name);
                        m.modifier = Modifier.Collapse;
                        m.folds = meshFilter.sharedMesh.GetFolds(modelImporter.generateSecondaryUV);
                        functions.Add(m);
                        
                        Collapse(meshFilter, m);
                        Debug.Log($"UVs for mesh '{meshFilter.sharedMesh.name}' auto collapsed. Asset '{assetPath}.'");
                    }
                    
                    _userData.meshModifiers = functions.ToArray();
                    assetImporter.userData = JsonUtility.ToJson(_userData);
                    
                    EditorUtility.SetDirty(assetImporter);
                }
            }

            if (_generateSecondaryUV)
            {
                _generateSecondaryUV = false;
                modelImporter.generateSecondaryUV = true;
                
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    Unwrapping.GenerateSecondaryUVSet(meshFilter.sharedMesh);
                }
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        #endregion

        #region Private Methods

        private void Combine(MeshFilter m, MeshModifier f)
        {
            if (f.uVs[0] == UVChannel.None || f.uVs[1] == UVChannel.None) return;

            Mesh mesh = m.sharedMesh;
            List<Vector4> mainPoints = new List<Vector4>(), failPoints = new List<Vector4>();

            mesh.GetUVs((int)f.uVs[0], mainPoints);
            mesh.GetUVs((int)f.uVs[1], failPoints);

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                mainPoints[i] = new Vector4(mainPoints[i].x, mainPoints[i].y, failPoints[i].x, failPoints[i].y);
            }

            mesh.SetUVs((int)f.uVs[0], mainPoints);
            //if (f.uVs[1] == UVChannel.UV1 && _generateSecondaryUV) return;
            mesh.SetUVs((int)f.uVs[1], new List<Vector2>());
        }

        private void Manual(MeshFilter m, MeshModifier f)
        {
            Mesh mesh = m.sharedMesh;

            if (f.entities[0] == Entity.Position)
            {
                List<Vector3> points = new List<Vector3>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(f.points[0]);
                }
                mesh.SetVertices(points);
            }
            else if (f.entities[0] == Entity.Normal)
            {
                List<Vector3> points = new List<Vector3>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(f.points[0]);
                }
                mesh.SetNormals(points);
            }
            else if (f.entities[0] == Entity.Tangent)
            {
                List<Vector4> points = new List<Vector4>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(f.points[0]);
                }
                mesh.SetTangents(points);
            }
            else if (f.entities[0] == Entity.Color)
            {
                List<Color> colors = new List<Color>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    colors.Add(new Color(f.points[0].x, f.points[0].y, f.points[0].z, f.points[0].w));
                }
                mesh.SetColors(colors);
            }
            else if (f.entities[0] == Entity.UV && f.uVs[0] != UVChannel.None)
            {
                List<Vector4> points = new List<Vector4>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(f.points[0]);
                }
                mesh.SetUVs((int) f.uVs[0], points);
            }
        }

        private void FromMesh(MeshFilter m, MeshModifier f)
        {
            Mesh mesh = m.sharedMesh;
            if (f.objects[0] == null) return;
            Mesh anotherMesh = (Mesh)f.objects[0];
            if (anotherMesh == null) return;
            
            List<Vector4> anotherPoints = new List<Vector4>();

            if (f.entities[1] == Entity.Position)
            {
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    anotherPoints.Add(anotherMesh.vertices[i]);
                }
            }
            else if (f.entities[1] == Entity.Normal)
            {
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    anotherPoints.Add(anotherMesh.normals[i]);
                }
            }
            else if (f.entities[1] == Entity.Tangent)
            {
                anotherMesh.GetTangents(anotherPoints);
            }
            else if (f.entities[1] == Entity.Color)
            {
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    anotherPoints.Add(new Vector4(
                        anotherMesh.colors[i].r,
                        anotherMesh.colors[i].g,
                        anotherMesh.colors[i].b,
                        anotherMesh.colors[i].a));
                }
            }
            else if (f.entities[1] == Entity.UV && f.uVs[1] != UVChannel.None)
            {
                anotherMesh.GetUVs((int) f.uVs[1], anotherPoints);
            }
            
            if (anotherPoints.Count < mesh.vertexCount) return;

            if (f.entities[0] == Entity.Position)
            {
                List<Vector3> points = new List<Vector3>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(anotherPoints[i]);
                }
                mesh.SetVertices(points);
            }
            else if (f.entities[0] == Entity.Normal)
            {
                List<Vector3> points = new List<Vector3>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(anotherPoints[i]);
                }
                mesh.SetNormals(points);
            }
            else if (f.entities[0] == Entity.Tangent)
            {
                List<Vector4> points = new List<Vector4>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(anotherPoints[i]);
                }
                mesh.SetTangents(points);
            }
            else if (f.entities[0] == Entity.Color)
            {
                List<Color> colors = new List<Color>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    colors.Add(new Color(
                        anotherPoints[i].x, 
                        anotherPoints[i].y, 
                        anotherPoints[i].z, 
                        anotherPoints[i].w));
                }
                mesh.SetColors(colors);
            }
            else if (f.entities[0] == Entity.UV && f.uVs[0] != UVChannel.None)
            {
                List<Vector4> points = new List<Vector4>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(anotherPoints[i]);
                }
                mesh.SetUVs((int) f.uVs[0], points);
            }
        }

        private void Collapse(MeshFilter m, MeshModifier f)
        {
            Mesh mesh = m.sharedMesh;
            UVFold[] folds = f.folds;
            List<Vector4>[] pointsArray = new List<Vector4>[folds.Length];

            for (int a = 0; a < folds.Length; a++)
            {
                pointsArray[a] = new List<Vector4>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    pointsArray[a].Add(new Vector4(
                        mesh.GetUVVector(folds[a].xy, i).x, 
                        mesh.GetUVVector(folds[a].xy, i).y,
                        folds[a].zw != UVChannel.None ? mesh.GetUVVector(folds[a].zw, i).x : 0f, 
                        folds[a].zw != UVChannel.None ? mesh.GetUVVector(folds[a].zw, i).y : 0f));
                }
            }

            for (int u = 0; u < 8; u++)
            {
                //if (u == 1 && _generateSecondaryUV) continue;
                mesh.SetUVs(u, new List<Vector2>());
            }

            for (var a = 0; a < folds.Length; a++)
            {
                if (folds[a].zw == UVChannel.None)
                {
                    List<Vector2> points = new List<Vector2>();
                    for (int i = 0; i < mesh.vertexCount; i++)
                    {
                        points.Add(new Vector2(pointsArray[a][i].x, pointsArray[a][i].y));
                    }
                    mesh.SetUVs((int)folds[a].origin, points);
                }
                else
                {
                    mesh.SetUVs((int)folds[a].origin, pointsArray[a]);    
                }
            }
        }

        private void Bounds(MeshFilter m, MeshModifier f)
        {
            Mesh mesh = m.sharedMesh;

            mesh.bounds = new Bounds(f.points[0], f.points[1]);
        }

        #endregion
    }
}
