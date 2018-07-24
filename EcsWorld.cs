using System;

namespace SimpleEcs
{
    public interface IEcsWorld
    {
        T GetComponent<T>(int entity) where T : class, new();
    }
    public class EcsWorld : IEcsWorld, IDisposable
    {
        public void Dispose()
        {
            if (this == self)
            {
                self = null;
            }
        }

        public T GetComponent<T>(int entity) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public EcsWorld self;
        private object _delayedUpdatesCount;
        private int _reservedEntitiesCount;
        int[] _reservedEntities = new int[256];
        public EcsWorld()
        {
            self = this;
        }

        public void ProcessDelayedUpdates(int level = 0)
        {
            var iMax = _delayedUpdatesCount;
        }

        public static void RegisterComponentCreator<T>(Func<T> creator) where T : class, new()
        {
            EcsComponentPool<T>.Instance.SetCreator(creator);
        }

        public int CreateEntity()
        {
            return CreateEntityInternal(true);
        }

        private int CreateEntityInternal(bool addSafeRemove)
        {
            int entity;
            if (_reservedEntitiesCount > 0)
            {
                _reservedEntitiesCount--;
                entity = _reservedEntities[_reservedEntitiesCount];
                _entities[entity].IsReserverd = false;
            }
            else
            {
                entity = _entitiesCount;
                if (_entities.Length == _entitiesCount)
                {
                    Array.Resize(ref _entities, _entitiesCount << 1);
                }

                _entities[_entitiesCount++] = new EcsEntity();
            }
            return entity;

        }

        EcsEntity[] _entities = new EcsEntity[1024];
        private int _entitiesCount;
    }
    struct DelayedUpdate
    {
        public enum Op : byte
        {
            RemoveEntiy,
            SateRemoveEntity,
            AddComponent,
            RemoveCompont
        }

        public Op Type;
        public int Entity;

        public int ComponentId { get; private set; }

        public IEcsComponentPool Pool;
        public DelayedUpdate( Op type, int entity, IEcsComponentPool pool, int componentId)
        {
            Type = type;
            Pool = pool;
            Entity = entity;
            ComponentId = componentId;
        }
    }

    sealed class EcsEntity
    {
        /// <summary>
        /// 是否被保留
        /// </summary>
        public bool IsReserverd;
        /// <summary>
        /// ECS个数
        /// </summary>
        public int ComponentCount;

        public ComponentLink[] Components = new ComponentLink[8];
    }

    sealed class ComponentLink
    {
        public IEcsComponentPool Pool;
        public int ItemId;
        public ComponentLink(IEcsComponentPool pool, int itemId)
        {
            Pool = pool;
            ItemId = itemId;
        }
    }
}
