using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityECSUtility
{
    /// <summary> Provides access to the internal UnityEngine.NoAllocHelpers methods. </summary>
    public static class NoAllocHelpers
    {
        private static Func<object, Array> _extractArrayFromListTDelegates;
        private static Action<object, int> _resizeListDelegates;

        /// <summary> Extract the internal array from a list. </summary>
        /// <typeparam name="T"><see cref="List{T}" />.</typeparam>
        /// <param name="list">The <see cref="List{T}" /> to extract from.</param>
        /// <returns>The internal array of the list.</returns>
        public static T[] ExtractArrayFromListT<T>(List<T> list)
        {
            if (_extractArrayFromListTDelegates == null)
            {
                var ass = Assembly.GetAssembly(typeof(Mesh)); // any class in UnityEngine
                var type = ass.GetType("UnityEngine.NoAllocHelpers");

                var methodInfo = type.GetMethod("ExtractArrayFromList", BindingFlags.Static | BindingFlags.Public);

                if (methodInfo == null)
                {
                    throw new Exception("ExtractArrayFromList signature changed.");
                }

                _extractArrayFromListTDelegates =
                    (Func<object, Array>)methodInfo.CreateDelegate(typeof(Func<object, Array>));
            }

            return (T[])_extractArrayFromListTDelegates.Invoke(list);
        }

        /// <summary> Resize a list.  </summary>
        /// <typeparam name="T"><see cref="List{T}" />.</typeparam>
        /// <param name="list">The <see cref="List{T}" /> to resize.</param>
        /// <param name="size">The new length of the <see cref="List{T}" />.</param>
        public static void ResizeList<T>(List<T> list, int size)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (size < 0 || size > list.Capacity)
            {
                throw new ArgumentException("Invalid size to resize.", nameof(list));
            }

            if (size == list.Count)
            {
                return;
            }

            if (_resizeListDelegates == null)
            {
                var ass = Assembly.GetAssembly(typeof(Mesh)); // any class in UnityEngine
                var type = ass.GetType("UnityEngine.NoAllocHelpers");

                var methodInfo = type.GetMethod("Internal_ResizeList", BindingFlags.Static | BindingFlags.NonPublic);

                if (methodInfo == null)
                {
                    throw new Exception("Internal_ResizeList signature changed.");
                }

                _resizeListDelegates = (Action<object, int>)methodInfo.CreateDelegate(typeof(Action<object, int>));
            }

            _resizeListDelegates.Invoke(list, size);
        }
    }
}