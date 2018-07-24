using System;

namespace SimpleEcs
{

    interface IEcsComponentPool
    {
        object GetItemById(int idx);
        void RecycleById(int id);
        int GetComponentTypeIndex();
    }

    sealed class EcsComponentPool<T> : IEcsComponentPool where T : class, new()
    {
        const int MinSize = 8;
        public static readonly EcsComponentPool<T> Instance = new EcsComponentPool<T>();
        public T[] Items = new T[MinSize];

        int _typeIndex;
        int _reservedItemsCount;
        int[] _reservedItems = new int[MinSize];
        int _itemsCount;

        EcsComponentPool()
        {
            _typeIndex = EcsHelpers.ComponentsCount;
        }
        Func<T> _creator;

        public int RequestNewId()
        {
            int id;
            if (_reservedItemsCount > 0)
            {
                id = _reservedItems[--_reservedItemsCount];
            }
            else
            {
                id = _itemsCount;
                if (_itemsCount == Items.Length)
                {
                    Array.Resize(ref Items, _itemsCount << 1);
                }
                Items[_itemsCount++] = _creator != null ? _creator() : (T)Activator.CreateInstance(typeof(T));
            }

            return id;

        }

        public int GetComponentTypeIndex()
        {
            return _typeIndex;
        }

        public object GetItemById(int id)
        {
            return Items[id];
        }

        public void RecycleById(int id)
        {
            if (_reservedItemsCount == _reservedItems.Length)
            {
                Array.Resize(ref _reservedItems, _reservedItemsCount << 1);
            }

            _reservedItems[_reservedItemsCount++] = id;
        }

        public void SetCreator(Func<T> creator)
        {
            _creator = creator;
        }

        public void Shrink()
        {
            var newSize = EcsHelpers.GetPowerOfTwoSize(_itemsCount < MinSize ? MinSize : _itemsCount);
            if (newSize < Items.Length)
            {
                Array.Resize(ref Items, newSize);
                if (_reservedItems.Length > MinSize)
                {
                    _reservedItems = new int[MinSize];
                    _reservedItemsCount = 0;
                }
            }
        }


    }
}
