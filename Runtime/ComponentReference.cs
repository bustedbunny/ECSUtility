using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace UnityECSUtility
{
    // public unsafe struct ComponentReference<T> where T : struct, IComponentData
    // {
    //     internal IntPtr ptr;
    //     internal uint orderVersion;
    //     internal Chunk* chunk;
    //     internal int typeIndex;
    //
    //     public T Read
    //     {
    //         get
    //         {
    //             if (chunk->GetOrderVersion() != orderVersion)
    //             {
    //                 Debug.LogError("Order changed, component is invalid");
    //             }
    //
    //             UnsafeUtility.CopyPtrToStructure((void*)ptr, out T output);
    //             return output;
    //         }
    //     }
    //
    //     public void Write(T value, uint globalSystemVersion)
    //     {
    //         if (chunk->GetOrderVersion() != orderVersion)
    //         {
    //             Debug.LogError("Order changed, component is invalid");
    //             return;
    //         }
    //
    //         var indexInTypeArray = ChunkDataUtility.GetIndexInTypeArray(chunk->Archetype, typeIndex);
    //         chunk->SetChangeVersion(indexInTypeArray, globalSystemVersion);
    //         UnsafeUtility.CopyStructureToPtr(ref value, (void*)ptr);
    //     }
    //
    //     public static ComponentReference<T> Get<T>(EntityManager em, Entity entity) where T : struct, IComponentData
    //     {
    //         var access = em.GetCheckedEntityDataAccess();
    //         var typeIndex = TypeManager.GetTypeIndex<T>();
    //
    //         access->EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);
    //         access->EntityComponentStore->AssertNotZeroSizedComponent(typeIndex);
    //         if (!access->IsInExclusiveTransaction)
    //             access->DependencyManager->CompleteWriteDependency(typeIndex);
    //
    //
    //         var ptr = access->EntityComponentStore->GetComponentDataWithTypeRO(entity,
    //             typeIndex);
    //
    //         var chunk = em.GetChunk(entity);
    //         var orderVersion = chunk.GetOrderVersion();
    //         return new ComponentReference<T>
    //         {
    //             ptr = (IntPtr)ptr, orderVersion = orderVersion, typeIndex = typeIndex, chunk = chunk.m_Chunk
    //         };
    //     }
    // }
}