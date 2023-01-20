using System;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace UnityECSUtility
{
    public static class EntityManagerUtility
    {
        public static Entity GetOrCreateSingletonEntity<T>(this EntityManager em)
        {
            using var query = em.CreateEntityQuery(ComponentType.ReadOnly<T>());
            var needCreate = query.IsEmptyIgnoreFilter;
            var e = needCreate ? em.CreateEntity(typeof(T)) : query.GetSingletonEntity();
            #if UNITY_EDITOR
            if (needCreate)
            {
                var type = typeof(T);
                var name = new StringBuilder();
                name.Append(type.Name);
                if (type.IsGenericType)
                {
                    name.Remove(name.Length - 2, 2);
                    name.Append(" | ");
                    foreach (var otherType in type.GetGenericArguments())
                    {
                        name.Append(otherType.Name);
                    }
                }

                em.SetName(e, name.ToString());
            }
            #endif
            return e;
        }

        public static EntityArchetype GetArchetype(this EntityManager em, params ComponentType[] types)
        {
            var e = em.CreateEntity(types);
            var archetype = em.GetChunk(e).Archetype;
            em.DestroyEntity(e);
            return archetype;
        }

        public static EntityArchetype GetArchetype(this EntityManager em, in Entity e)
        {
            return em.GetChunk(e).Archetype;
        }

        /// <summary>
        /// Adds or removes given component from entity.
        /// </summary>
        /// <param name="em"></param>
        /// <param name="stateEntity"></param>
        /// <returns>Returns true if PlaySimulation was added and returns false if it was removed.</returns>
        [GenerateTestsForBurstCompatibility]
        public static bool ToggleState<T>(this EntityManager em, in Entity stateEntity)
            where T : unmanaged, IComponentData
        {
            if (em.RemoveComponent<T>(stateEntity))
            {
                return false;
            }

            em.AddComponent<T>(stateEntity);
            return true;
        }

        [GenerateTestsForBurstCompatibility]
        public static void ToggleState<T>(this EntityManager em, in Entity stateEntity, in bool state)
            where T : unmanaged, IComponentData
        {
            if (state)
            {
                em.AddComponent<T>(stateEntity);
            }
            else
            {
                em.RemoveComponent<T>(stateEntity);
            }
        }


        /// <summary>
        /// Provides an unsafe ref to component data directly. Gets invalidated on any structural change.
        /// </summary>
        /// <param name="em"></param>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [GenerateTestsForBurstCompatibility]
        public static unsafe ref T GetComponentReference<T>(this EntityManager em, in Entity entity)
            where T : struct, IComponentData
        {
            var access = em.GetCheckedEntityDataAccess();
            var typeIndex = TypeManager.GetTypeIndex<T>();

            access->EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);
            access->EntityComponentStore->AssertNotZeroSizedComponent(typeIndex);
            if (!access->IsInExclusiveTransaction)
                access->DependencyManager->CompleteWriteDependency(typeIndex);

            // var ptr = access->EntityComponentStore->GetComponentDataRawRW(entity, typeIndex);
            var ptr = access->EntityComponentStore->GetComponentDataWithTypeRW(entity,
                typeIndex,
                em.GlobalSystemVersion);

            return ref UnsafeUtility.AsRef<T>(ptr);
        }
    }
}