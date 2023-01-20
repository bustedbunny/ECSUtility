using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace UnityECSUtility
{
    public unsafe struct NativeReferenceUntyped
    {
        public void* _data;

        #if ENABLE_UNITY_COLLECTIONS_CHECKS
        public AtomicSafetyHandle m_Safety;
        #endif
        public AllocatorManager.AllocatorHandle m_AllocatorLabel;
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public struct SingletonNativeReferenceStore : ISystem
    {
        public NativeHashMap<long, NativeReferenceUntyped> ptrMap;

        public void OnCreate(ref SystemState state)
        {
            state.Enabled = false;

            Init();
        }

        public void Init()
        {
            if (!ptrMap.IsCreated)
                ptrMap = new NativeHashMap<long, NativeReferenceUntyped>(10, Allocator.Persistent);
        }


        public void OnDestroy(ref SystemState state)
        {
            foreach (var value in ptrMap)
            {
                var copy = value;
                ref var val = ref UnsafeUtility.As<NativeReferenceUntyped, NativeReference<int>>(ref copy.Value);

                #if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (val.IsCreated && val.m_AllocatorLabel.IsValid)
                    #endif
                    val.Dispose();
            }
        }

        public void OnUpdate(ref SystemState state)
        {
        }
    }

    public static class NativeReferenceUtility
    {
        [GenerateTestsForBurstCompatibility]
        public static NativeReference<T> GetSingletonStructReference<T>(this EntityManager em)
            where T : unmanaged
        {
            var hash = BurstRuntime.GetHashCode64<NativeReference<T>>();

            var handle = em.WorldUnmanaged.GetOrCreateUnmanagedSystem<SingletonNativeReferenceStore>();
            ref var store = ref em.WorldUnmanaged.GetUnsafeSystemRef<SingletonNativeReferenceStore>(handle);
            store.Init();

            if (store.ptrMap.TryGetValue(hash, out var untyped))
            {
                return UnsafeUtility.As<NativeReferenceUntyped, NativeReference<T>>(ref untyped);
            }

            var value = new NativeReference<T>(Allocator.Persistent);
            store.ptrMap.Add(hash, UnsafeUtility.As<NativeReference<T>, NativeReferenceUntyped>(ref value));

            return value;
        }

        public static unsafe ref T GetRef<T>(this NativeReference<T> reference) where T : unmanaged
        {
            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(reference.m_Safety);
            #endif
            return ref UnsafeUtility.AsRef<T>(reference.m_Data);
        }
    }
}