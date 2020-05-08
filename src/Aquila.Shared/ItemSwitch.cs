using System;
using System.Threading.Tasks;

namespace Aquila.Shared
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


        public ItemSwitch<T> Case(T c, Action action)
        {
            if (_item.Equals(c) && !_executed)
            {
                _executed = true;
                action();
            }

            return this;
        }
    }

    /// <summary>
    /// switch - подобная монада, позволяет инкапсулировать в поведение логику оператора is
    /// </summary>
    /// <typeparam name="TI">type item</typeparam>
    /// <typeparam name="TR">type result</typeparam>
    public class ItemSwitchWithResult<TI, TR>
    {
        private TI _item;
        private TR _result;

        private bool _executed;
        private bool _breaked;
        private bool _immidiate;

        private ItemSwitchWithResult(TI item)
        {
            _item = item;
            _breaked = false;
        }

        public static ItemSwitchWithResult<TI, TR> Switch(TI item)
        {
            return new ItemSwitchWithResult<TI, TR>(item);
        }

        public ItemSwitchWithResult<TI, TR> CaseIs<TIsType>(Func<TIsType, TR> action)
            where TIsType : TI
        {
            if (_breaked || _executed) return this;

            if (_item is TIsType type)
            {
                _executed = true;
                _result = action(type);

                if (_immidiate)
                    _breaked = true;
            }

            return this;
        }

        public ItemSwitchWithResult<TI, TR> Break()
        {
            _breaked = true;
            return this;
        }

        public ItemSwitchWithResult<TI, TR> Case(Func<TI, bool> c, Func<TR> action)
        {
            if (_breaked || _executed) return this;
            if (c(_item))
            {
                _executed = true;
                _result = action();
            }

            return this;
        }

        public ItemSwitchWithResult<TI, TR> Case(TI c, Func<TR> action)
        {
            if (_breaked || _executed) return this;

            if (_item.Equals(c))
            {
                _executed = true;
                action();
            }

            return this;
        }


        public TR Result()
        {
            return _result;
        }
    }
}