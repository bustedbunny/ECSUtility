using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityECSUtility
{
    public static class ArrayUtility
    {
        public static unsafe NativeArray<T> ReinterpretAsNativeArray<T>(this Array array, out GCHandle handle)
            where T : unmanaged
        {
            handle = GCHandle.Alloc(array, GCHandleType.Pinned);
            var ptr = handle.AddrOfPinnedObject();

            var nativeArray =
                NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)ptr,
                    array.Length,
                    Allocator.None);
            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref nativeArray, AtomicSafetyHandle.Create());
            #endif

            return nativeArray;
        }
    }
}