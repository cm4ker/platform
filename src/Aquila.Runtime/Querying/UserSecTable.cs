﻿using System.Collections.Generic;
using System.Linq;
using Aquila.Metadata;

namespace Aquila.Runtime.Querying
{
/*
        
            // 1. We can throw the SecException on construct query if user haven't access to the table
            // 2. We need create SecTable
        
            UserSecTable
            Object\Permission        | Create | Update | Read | Delete |
                                     +--------+--------+------+--------+
                            Store    |   x    |        |  x   |   x*   | 
                                     |________+________+______+________+
                            Invoice  |        |   x*   |      |        |
                                     |________+________+______+________+
                            Document |   x    |   x    |  x   |   x    |
                                     |________+________+______+________+
            
            Where x* - is permission with lookup query (more complex case) NOTE: Lookup queries can be more than one
         */

    public class UserSecPermission
    {
        public UserSecPermission()
        {
            Criteria = new Dictionary<SecPermission, List<string>>();
        }

        public SecPermission Permission { get; set; }

        public Dictionary<SecPermission, List<string>> Criteria { get; set; }
    }

    public class UserSecTable
    {
        private Dictionary<SMEntity, UserSecPermission> _rows;

        public UserSecTable()
        {
        }

        public void Init(List<SMSecPolicy> policies)
        {
            var subjects = policies.SelectMany(x => x.Subjects);
            var criteria = policies.SelectMany(x => x.Criteria);

            var result = new Dictionary<SMEntity, UserSecPermission>();

            var a = (
                from s in subjects
                join c in criteria
                    on s.Subject equals c.Subject
                    into tmp
                from t in tmp.DefaultIfEmpty()
                group new { Sbuject = s.Subject, Permission = s.Permission, Criterion = t } by s.Subject
                into g
                select new { g.Key, Items = g.ToList() }
            );

            foreach (var t in a)
            {
                var up = new UserSecPermission();

                foreach (var values in t.Items)
                {
                    up.Permission |= values.Permission;

                    var criterion = values.Criterion;
                    if (criterion != null)
                    {
                        if (up.Criteria.TryGetValue(criterion.Permission, out var pList))
                            pList.Add(criterion.Query);
                        else
                            up.Criteria[criterion.Permission] = new() { criterion.Query };
                    }
                }

                result[t.Key] = up;
            }

            // TODO: this is primitive algo for creating user sec permissions dictionary
            // we need more complex algo for this
            // 1. Check for the same criterion query
            // 2. Merge UserSecPermission then we have many secs;

            _rows = result;
        }

        public UserSecPermission this[SMEntity md]
        {
            get { return _rows[md]; }
        }

        public bool TryClaimSec(SMEntity md, out UserSecPermission permission)
        {
            return _rows.TryGetValue(md, out permission);
        }

        public bool TryClaimPermission(SMEntity md, SecPermission permission, out UserSecPermission claim)
        {
            if (TryClaimSec(md, out var result))
            {
                claim = result;
                if (permission.HasFlag(SecPermission.Create) && !result.Permission.HasFlag(SecPermission.Create))
                {
                    claim = null;
                    return false;
                }

                if (permission.HasFlag(SecPermission.Read) && !result.Permission.HasFlag(SecPermission.Read))
                {
                    claim = null;
                    return false;
                }

                if (permission.HasFlag(SecPermission.Update) && !result.Permission.HasFlag(SecPermission.Update))
                {
                    claim = null;
                    return false;
                }

                if (permission.HasFlag(SecPermission.Delete) && !result.Permission.HasFlag(SecPermission.Delete))
                {
                    claim = null;
                    return false;
                }
            }
            else
            {
                claim = null;
                return false;
            }

            return true;
        }
    }
}