using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace UnityECSUtility
{
    public static class InstantiateUtility
    {
        public static unsafe void InstantiateInto(this EntityManager em, Entity source, Entity target)
        {
            var sourceChunk = em.GetChunk(source);
            var targetArchetype = sourceChunk.Archetype;


            em.SetArchetype(target, targetArchetype);

            using var targetTypes = targetArchetype.GetComponentTypes();


            // Settings shared components
            foreach (var targetType in targetTypes)
            {
                if (!targetType.IsSharedComponent) continue;
                var handle = em.GetDynamicSharedComponentTypeHandle(targetType);

                var sharedComponentIndex = sourceChunk.GetSharedComponentIndex(ref handle);


                var access = em.GetCheckedEntityDataAccess();
                var changes = access->BeginStructuralChanges();

                var store = access->EntityComponentStore;
                store->SetSharedComponentDataIndex(target, targetType, sharedComponentIndex);

                access->EndStructuralChanges(ref changes);
            }

            sourceChunk = em.GetChunk(source);
            var targetChunk = em.GetChunk(target);

            var entityTypeHandle = em.GetEntityTypeHandle();

            var sourceChunkEntities = sourceChunk.GetNativeArray(entityTypeHandle);
            var targetChunkEntities = targetChunk.GetNativeArray(entityTypeHandle);

            var sourceEntityIndex = GetEntityIndexInArray(sourceChunkEntities, source);
            var targetEntityIndex = GetEntityIndexInArray(targetChunkEntities, target);


            foreach (var targetType in targetTypes)
            {
                if (targetType.IsZeroSized) continue;
                var typeInfo = TypeManager.GetTypeInfo(targetType.TypeIndex);
                var typeSize = typeInfo.SizeInChunk;


                var dynamicTypeHandle = em.GetDynamicComponentTypeHandle(targetType);

                var sourcePtr = (byte*)sourceChunk
                    .GetDynamicComponentDataArrayReinterpret<byte>(ref dynamicTypeHandle, typeSize)
                    .GetUnsafeReadOnlyPtr();
                var targetPtr = (byte*)targetChunk
                    .GetDynamicComponentDataArrayReinterpret<byte>(ref dynamicTypeHandle, typeSize)
                    .GetUnsafePtr();

                UnsafeUtility.MemCpy(
                    targetPtr + targetEntityIndex * typeSize,
                    sourcePtr + sourceEntityIndex * typeSize,
                    typeSize);
            }
        }

        private static int GetEntityIndexInArray(in NativeArray<Entity> array, in Entity e)
        {
            for (var index = 0; index < array.Length; index++)
            {
                var entity = array[index];
                if (entity == e)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}