using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityECSUtility
{
    public static class NativeUnsafeUtility
    {
        public static unsafe void AddBatchUnsafe<TKey, TValue>(
            [NoAlias] this NativeParallelHashMap<TKey, TValue>.ParallelWriter hashMap,
            [NoAlias] TKey* keys,
            [NoAlias] TValue* values,
            int length)
            where TKey : unmanaged, IEquatable<TKey>
            where TValue : unmanaged
        {
            var writer = hashMap.m_Writer;
            var data = writer.m_Buffer;

            var newLength = Interlocked.Add(ref data->allocatedIndexLength, length);
            var oldLength = newLength - length;

            var keyPtr = ((TKey*)data->keys) + oldLength;
            var valuePtr = ((TValue*)data->values) + oldLength;

            UnsafeUtility.MemCpy(keyPtr, keys, length * UnsafeUtility.SizeOf<TKey>());
            UnsafeUtility.MemCpy(valuePtr, values, length * UnsafeUtility.SizeOf<TValue>());

            var buckets = (int*)data->buckets;
            var nextPtrs = ((int*)data->next) + oldLength;

            for (var idx = 0; idx < length; idx++)
            {
                var hash = keys[idx].GetHashCode() & data->bucketCapacityMask;
                var index = oldLength + idx;
                var next = Interlocked.Exchange(ref UnsafeUtility.ArrayElementAsRef<int>(buckets, hash), index);
                nextPtrs[idx] = next;
            }
        }

        public static unsafe void AddRangeNative<T>(this List<T> list, void* arrayBuffer, int length)
            where T : struct
        {
            if (length == 0)
            {
                return;
            }

            var index = list.Count;
            var newLength = index + length;

            // Resize our list if we require
            if (list.Capacity < newLength)
            {
                list.Capacity = newLength;
            }

            var items = NoAllocHelpers.ExtractArrayFromListT(list);
            var size = UnsafeUtility.SizeOf<T>();

            // Get the pointer to the end of the list
            var bufferStart = (IntPtr)UnsafeUtility.AddressOf(ref items[0]);
            var buffer = (byte*)(bufferStart + (size * index));

            UnsafeUtility.MemCpy(buffer, arrayBuffer, length * (long)size);

            NoAllocHelpers.ResizeList(list, newLength);
        }
    }
}