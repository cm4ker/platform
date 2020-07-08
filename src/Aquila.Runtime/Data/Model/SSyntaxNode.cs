using System;
using System.Collections.Generic;
using System.Linq;

namespace Aquila.QueryBuilder.Model
{
    public partial class SSyntaxNode
    {
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

        public bool Compare(SSyntaxNode node1, SSyntaxNode node2) 
        {
            if (node1 == null && node2 == null) return true; 
            if (node1 == null || node2 == null) return false; 
            return node1.Equals(node2);
        }

        public int Xor(IEnumerable<SSyntaxNode> list, Func<SSyntaxNode, int> func)
        {
            var result = 0;

            foreach (var item in list)
            {
                result ^= func(item);


            }

            return result;
        }
    }
}
