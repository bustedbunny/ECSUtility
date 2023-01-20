using Unity.Entities;

namespace UnityECSUtility
{
    public struct Singleton<T> : IComponentData where T : unmanaged, IComponentData
    {
    }
}