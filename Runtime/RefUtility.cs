using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace UnityECSUtility
{
    public static class RefUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void CheckIndexInRange(int index, int length)
        {
            if (index < 0 || index > length)
                throw new ArgumentException($"Index {index} is out of range of length {length}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref T GetAsRef<T>(this NativeArray<T> array, int index) where T : struct
        {
            CheckIndexInRange(index, array.Length);

            return ref UnsafeUtility.ArrayElementAsRef<T>(array.GetUnsafePtr(), index);
        }

        public static unsafe ref T GetAsRef<T>(this DynamicBuffer<T> buffer, int index)
            where T : unmanaged, IBufferElementData
        {
            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(buffer.m_Safety0);
            AtomicSafetyHandle.CheckWriteAndThrow(buffer.m_Safety1);
            #endif

            CheckIndexInRange(index, buffer.Length);
            return ref UnsafeUtility.ArrayElementAsRef<T>(buffer.GetUnsafePtr(), index);
        }
    }
}