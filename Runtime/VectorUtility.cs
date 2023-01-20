using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityECSUtility
{
    public static class VectorUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Magnitude(this float3 v) { return math.sqrt(v.x * v.x + v.y * v.y + v.z * v.z); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Magnitude(this float2 v) { return math.sqrt(v.x * v.x + v.y * v.y); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(quaternion targetDir, quaternion currDir)
        {
            return targetDir.value.w * currDir.value.w + targetDir.value.x * currDir.value.x +
                   targetDir.value.y * currDir.value.y + targetDir.value.z * currDir.value.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 Normalized(this float3 original)
        {
            return original / (math.sqrt(original.x * original.x + original.y * original.y + original.z * original.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 Normalized(this float2 original)
        {
            return original / (math.sqrt(original.x * original.x + original.y * original.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Normalize(this ref float3 original)
        {
            original /= (math.sqrt(original.x * original.x + original.y * original.y + original.z * original.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Normalize(this ref float2 original)
        {
            original /= (math.sqrt(original.x * original.x + original.y * original.y));
        }
    }
}