using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MeshExtensions.Editor
{
    [Serializable]
    public class UserData
    {
        public MeshModifier[] meshFunctions = new MeshModifier[0];
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
        

        public MeshModifier(string id)
        {
            this.id = id;
            modifier = Modifier.None;
            entities = new Entity[2];
            uVs = new UVChannel[2];
            points = new SerializableVector[2];
            objects = new Object[2];
        }
    }
    
    [Serializable]
    public enum Modifier { None = -1, Combine = 0, Manual = 1, Mesh = 2, Json = 3, Math = 4, Bounds = 5 }

    [Serializable]
    public enum Entity { Position = 0, Normal = 1, Tangent = 2, UV = 3, Color = 4 }

    [Serializable]
    public enum UVChannel { UV0 = 0, UV1 = 1, UV2 = 2, UV3 = 3, UV4 = 4, UV5 = 5, UV6 = 6, UV7 = 7 }

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
                default: return mesh.uv[idx];
                case UVChannel.UV1: return mesh.uv2[idx];
                case UVChannel.UV2: return mesh.uv3[idx];
                case UVChannel.UV3: return mesh.uv4[idx];
                case UVChannel.UV4: return mesh.uv5[idx];
                case UVChannel.UV5: return mesh.uv6[idx];
                case UVChannel.UV6: return mesh.uv7[idx];
                case UVChannel.UV7: return mesh.uv8[idx];
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
                default: 
                    Debug.LogError("Can return data from UV entity!");
                    return default;
            }
        }
    }
}
