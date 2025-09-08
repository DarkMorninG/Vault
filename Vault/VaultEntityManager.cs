using Unity.Entities;

namespace Vault {
    public static class VaultEntityManager {
        public static DynamicBuffer<T> GetSingletonBuffer<T>(this EntityManager entityManager)
            where T : unmanaged, IBufferElementData {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<T>());
            var singletonEntity = query.GetSingletonEntity();
            return entityManager.GetBuffer<T>(singletonEntity);
        }
    }
}