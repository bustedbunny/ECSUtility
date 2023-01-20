using Unity.Mathematics;
using UnityEngine;

namespace UnityECSUtility
{
    public static class MatrixUtility
    {
        public static float3 GetPosition(this Matrix4x4 matrix)
        {
            return new float3(matrix[0, 3], matrix[1, 3], matrix[2, 3]);
        }

        public static void SetPosition(this Matrix4x4 matrix, float3 pos)
        {
            matrix[0, 3] = pos.x;
            matrix[1, 3] = pos.y;
            matrix[2, 3] = pos.z;
        }

        public static void SetPosition(this float4x4 matrix, float3 pos)
        {
            matrix.c0.z = pos.x;
            matrix.c1.z = pos.y;
            matrix.c2.z = pos.z;
        }

        public static float3 Scale(this float4x4 matrix) { return new float3(matrix.c0.x, matrix.c1.y, matrix.c2.z); }
    }
}