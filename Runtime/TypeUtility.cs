using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityECSUtility
{
    public static class TypeUtility
    {
        public static Assembly[] GetAllAssemblies() => AppDomain.CurrentDomain.GetAssemblies();

        public static IEnumerable<Type> GetAllTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<Type> GetAllAssignableFrom(Type from, bool allowAbstract = false)
        {
            foreach (var type in GetAllTypes())
            {
                if ((!type.IsAbstract || allowAbstract) && from.IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }

        public static object InstantiateOfType(Type type)
        {
            var obj = default(object);
            try
            {
                obj = Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            if (obj != null)
            {
                return obj;
            }

            throw new Exception($"Could not instantiate object of type {type.FullName}");
        }

        public static IEnumerable<object> InstantiateAllOfType(Type objTypes)
        {
            foreach (var type in GetAllAssignableFrom(objTypes))
            {
                var obj = default(object);
                try
                {
                    obj = Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                if (obj != null)
                {
                    yield return obj;
                }
            }
        }

        public static IEnumerable<T> InstantiateAllOfType<T>()
        {
            foreach (var o in InstantiateAllOfType(typeof(T)))
            {
                T obj;
                try
                {
                    obj = (T)o;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    continue;
                }

                yield return obj;
            }
        }
    }
}