using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace UnityECSUtility
{
    public static class EntityUtility
    {
        [GenerateTestsForBurstCompatibility]
        public static float3 GetEntityPosition(this EntityManager em, in Entity e)
        {
            return em.GetComponentData<LocalToWorld>(e).Position;
        }


        [GenerateTestsForBurstCompatibility]
        public static NativeArray<Entity> GetAllEntitiesInHierarchy(this EntityManager em, in Entity parent,
            Allocator allocator = Allocator.Temp)
        {
            var list = new NativeList<Entity>(1, allocator);
            list.Add(parent);
            if (!em.HasComponent<Child>(parent))
            {
                return list.AsArray();
            }

            var buffer = em.GetBuffer<Child>(parent);
            foreach (var child in buffer)
            {
                var children = GetAllEntitiesInHierarchy(em, child.Value);
                list.AddRange(children);
                children.Dispose();
            }

            return list.AsArray();
        }

        [GenerateTestsForBurstCompatibility]
        public static NativeArray<Entity> GetAllEntitiesInHierarchy(this BufferLookup<Child> lookup, in Entity parent,
            Allocator allocator = Allocator.Temp)
        {
            var list = new NativeList<Entity>(1, allocator);
            list.Add(parent);

            if (!lookup.TryGetBuffer(parent, out var buffer))
            {
                return list.AsArray();
            }

            foreach (var child in buffer)
            {
                var children = GetAllEntitiesInHierarchy(lookup, child.Value);
                list.AddRange(children);
                children.Dispose();
            }

            return list.AsArray();
        }
    }
}