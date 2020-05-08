using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquila.Shared.Tree;

namespace Aquila.Core.Querying.Model
{
    /// <summary>
    /// Элемент логических связей в запросе. LT - Logical tree
    /// </summary>
    public abstract partial class QItem : OneWayNode
    {
        protected QItem()
        {
            AttachedPropery = new Dictionary<string, object>();
        }

        public static bool SequenceEqual<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }

            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }

            return cnt.Values.All(c => c == 0);
        }

        public bool Compare(QItem node1, QItem node2)
        {
            if (node1 == null && node2 == null) return true;
            if (node1 == null || node2 == null) return false;
            return node1.Equals(node2);
        }

        public bool Compare<T1>(T1 node1, T1 node2)
        {
            if (node1 == null && node2 == null) return true;
            if (node1 == null || node2 == null) return false;
            return node1.Equals(node2);
        }

        public int Xor(IEnumerable<QItem> list, Func<QItem, int> func)
        {
            var result = 0;

            foreach (var item in list)
            {
                result ^= func(item);
            }

            return result;
        }


        public Dictionary<string, object> AttachedPropery { get; }
        
        
        public abstract T Accept<T>(QLangVisitorBase<T> visitor);
        
        public abstract void Accept(QLangVisitorBase visitor);
    }
}