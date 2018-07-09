using System;

namespace ZenPlatform.Shared
{
    /// <summary>
    /// switch - подобная монада, позволяет инкапсулировать в поведение логику оператора is
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemSwitch<T>
    {
        private T _item;

        private bool _executed;

        private ItemSwitch(T item)
        {
            _item = item;
        }

        public static ItemSwitch<T> Switch(T item)
        {
            return new ItemSwitch<T>(item);
        }

        public ItemSwitch<T> CaseIs<TIsType>(Action<TIsType> action)
            where TIsType : T
        {
            if (_item is TIsType type)
            {
                _executed = true;
                action(type);
            }

            return this;
        }

        public ItemSwitch<T> Case(Func<T, bool> c, Action action)
        {
            if (c(_item) && !_executed)
            {
                _executed = true;
                action();
            }

            return this;
        }
    }
}