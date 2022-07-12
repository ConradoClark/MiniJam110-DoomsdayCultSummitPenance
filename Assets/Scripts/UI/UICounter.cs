using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UICounter : BaseUIObject
    {
        public ScriptPrefab CounterItem;
        private UICounterItemPool _itemPool;

        private UICounterItem[] _items;

        public float DistanceBetweenItems;
        public int Initial;
        public int Maximum;
        private int _current;

        protected override void OnAwake()
        {
            base.OnAwake();
            _current = Initial;
            _itemPool = SceneObject<UICounterItemManager>.Instance().GetEffect(CounterItem);

            if (_itemPool.TryGetManyFromPool(Maximum, out var objects))
            {
                _items = objects;
                for (var index = 0; index < _items.Length; index++)
                {
                    var item = _items[index];
                    item.transform.position =
                        new Vector3(transform.position.x + index * DistanceBetweenItems, transform.position.y);
                }
            }
            AdjustCounter();
        }

        public void Set(int amount)
        {
            _current = amount;
            if (_current > Maximum) _current = Maximum;
            if (_current < 0) _current = 0;
            AdjustCounter();
        }

        public void Increase(int amount)
        {
            _current += amount;
            if (_current > Maximum) _current = Maximum;
            AdjustCounter();
        }

        public void Decrease(int amount)
        {
            _current -= amount;
            if (_current < 0) _current = 0;
            AdjustCounter();
        }

        private void AdjustCounter()
        {
            if (_items == null) return;
            for (var index = 0; index < _items.Length; index++)
            {
                var item = _items[index];
                if (_current <= index)
                {
                    item.SetEmpty();
                }
                else
                {
                    item.SetFull();
                }
            }
        }
    }
}
