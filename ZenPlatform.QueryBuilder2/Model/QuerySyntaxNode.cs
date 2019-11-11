using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.QueryBuilder.Model
{
    public partial class QuerySyntaxNode
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

        public bool Compare(QuerySyntaxNode node1, QuerySyntaxNode node2) 
        {
            if (node1 == null && node2 == null) return true; 
            if (node1 == null || node2 == null) return false; 
            return node1.Equals(node2);
        }

        public int Xor(IEnumerable<QuerySyntaxNode> list, Func<QuerySyntaxNode, int> func)
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
