using Unity.Mathematics;

namespace UnityECSUtility
{
    public static class MathUtility
    {
        public static readonly float3 Forward = new(0f, 0f, 1f);
        public static readonly float3 Up = new(0f, 1f, 0f);


        public static float3 ToFloat3(this float2 a) => new float3(a, 0f);
    }
}