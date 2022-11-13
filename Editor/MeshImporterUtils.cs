using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MeshExtensions.Editor
{
    [Serializable]
    public class UserData
    {
        public MeshModifier[] meshModifiers = new MeshModifier[0];
    }

    [Serializable]
    public struct MeshModifier
    {
        public string id; 
        public Modifier modifier;
        
        public Entity[] entities;
        public UVChannel[] uVs;
        public SerializableVector[] points;
        public Object[] objects;
        public UVFold[] folds;
        

        public MeshModifier(string id)
        {
            this.id = id;
            modifier = Modifier.None;
            entities = new[] { Entity.None, Entity.None };
            uVs = new [] {UVChannel.None, UVChannel.None };
            points = new SerializableVector[2];
            objects = new Object[2];
            folds = null;
        }
    }

    [Serializable]
    public struct UVFold
    {
        public UVChannel origin;
        public UVChannel xy;
        public UVChannel zw;

        public UVFold(UVChannel origin, UVChannel xy, UVChannel zw)
        {
            this.origin = origin;
            this.xy = xy;
            this.zw = zw;
        }
    }
    
    [Serializable]
    public enum Modifier { None = -1, Combine = 0, Manual = 1, Mesh = 2, Json = 3, Collapse = 4, Bounds = 5 }

    [Serializable]
    public enum Entity { None = -1, Position = 0, Normal = 1, Tangent = 2, UV = 3, Color = 4 }

    [Serializable]
    public enum UVChannel { None = -1, UV0 = 0, UV1 = 1, UV2 = 2, UV3 = 3, UV4 = 4, UV5 = 5, UV6 = 6, UV7 = 7 }

    [Serializable]
    public struct SerializableVector
    {
        public float x, y, z, w;

        public SerializableVector(float x, float y, float z = 0.0f, float w = 0.0f)
        {
            this.x = x; this.y = y; this.z = z; this.w = w;
        }
        
        public override string ToString() => $"[{x}, {y}, {z}, {w}]";

        public static implicit operator Vector2(SerializableVector value) =>
            new Vector2(value.x, value.y);

        public static implicit operator Vector3(SerializableVector value) =>
            new Vector3(value.x, value.y, value.z);
        
        public static implicit operator Vector4(SerializableVector value) =>
            new Vector4(value.x, value.y, value.z, value.w);
        
        public static implicit operator SerializableVector(Vector2 value) =>
            new SerializableVector(value.x, value.y);
        
        public static implicit operator SerializableVector(Vector3 value) => 
            new SerializableVector(value.x, value.y, value.z);

        public static implicit operator SerializableVector(Vector4 value) =>
            new SerializableVector(value.x, value.y, value.z, value.w);
    }

    public static class MeshExtension
    {
        public static Vector4 GetUVVector(this Mesh mesh, UVChannel channel, int idx)
        {
            switch (channel)
            {
                case UVChannel.UV0: return mesh.uv[idx];
                case UVChannel.UV1: return mesh.uv2[idx];
                case UVChannel.UV2: return mesh.uv3[idx];
                case UVChannel.UV3: return mesh.uv4[idx];
                case UVChannel.UV4: return mesh.uv5[idx];
                case UVChannel.UV5: return mesh.uv6[idx];
                case UVChannel.UV6: return mesh.uv7[idx];
                case UVChannel.UV7: return mesh.uv8[idx];
                default: return Vector4.zero;
            }
        }

        public static Vector4 GetEntityVector(this Mesh mesh, Entity entity, int idx)
        {
            switch (entity)
            {
                case Entity.Position: return mesh.vertices[idx];
                case Entity.Normal: return mesh.normals[idx];
                case Entity.Tangent: return mesh.tangents[idx];
                case Entity.Color : return mesh.colors[idx];
                default: throw new Exception("Wrong Get Entity Vector");
            }
        }

        public static UVFold[] GetFolds(this Mesh mesh, bool generateSecondaryUV)
        {
            List<UVChannel> uVs = new List<UVChannel>();
            List<UVChannel> freeUVs = new List<UVChannel>();
            if (mesh.uv != null && mesh.uv.Length > 0) uVs.Add(UVChannel.UV0);
            if (mesh.uv2 != null && mesh.uv2.Length > 0) uVs.Add(UVChannel.UV1);
            if (mesh.uv3 != null && mesh.uv3.Length > 0) uVs.Add(UVChannel.UV2);
            if (mesh.uv4 != null && mesh.uv4.Length > 0) uVs.Add(UVChannel.UV3);
            if (mesh.uv5 != null && mesh.uv5.Length > 0) uVs.Add(UVChannel.UV4);
            if (mesh.uv6 != null && mesh.uv6.Length > 0) uVs.Add(UVChannel.UV5);
            if (mesh.uv7 != null && mesh.uv7.Length > 0) uVs.Add(UVChannel.UV6);
            if (mesh.uv8 != null && mesh.uv8.Length > 0) uVs.Add(UVChannel.UV7);
            
            List<UVFold> folds = new List<UVFold>();
            
            while (uVs.Count > 0)
            {
                bool free = freeUVs.Count > 0;

                UVChannel origin = free ? freeUVs[0] : uVs[0];
                UVChannel xy = uVs[0];
                UVChannel zw = uVs.Count == 1 ? UVChannel.None : uVs[1];

                if (generateSecondaryUV && origin == UVChannel.UV1 && zw != UVChannel.None)
                {
                    origin = xy == UVChannel.UV1 ? zw : xy;
                }
                
                folds.Add(new UVFold(origin, xy, zw));
                
                if (free) freeUVs.RemoveAt(0);
                if (origin != xy) freeUVs.Add(xy);
                if (origin != zw && zw != UVChannel.None) freeUVs.Add(zw);

                if (uVs.Count > 0) uVs.RemoveAt(0);
                if (uVs.Count > 0) uVs.RemoveAt(0);
            }

            return folds.ToArray();
        }
    }
}
