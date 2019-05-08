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
        private bool _breaked;

        private ItemSwitch(T item)
        {
            _item = item;
            _breaked = false;
        }

        public static ItemSwitch<T> Switch(T item)
        {
            return new ItemSwitch<T>(item);
        }

        public ItemSwitch<T> CaseIs<TIsType>(Action<TIsType> action)
            where TIsType : T
        {
            if (_breaked) return this;

            if (_item is TIsType type)
            {
                _executed = true;
                action(type);
            }

            return this;
        }

        public ItemSwitch<T> BreakIfExecuted()
        {
            if (_executed)
                _breaked = true;
            return this;
        }

        public ItemSwitch<T> Case(Func<T, bool> c, Action action)
        {
            if (_breaked) return this;
            if (c(_item) && !_executed)
            {
                _executed = true;
                action();
            }

            return this;
        }
    }
}