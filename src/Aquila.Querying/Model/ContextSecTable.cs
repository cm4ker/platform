using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Querying.Model;
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

    public class ContextSecPermission
    {
        public ContextSecPermission()
        {
            Criteria = new Dictionary<SecPermission, List<(string, QCriterion)>>();
        }

        public SecPermission Permission { get; set; }

        public Dictionary<SecPermission, List<(string cString, QCriterion cModel)>> Criteria { get; internal set; }

        /// <summary>
        /// Returns permission with aspect to certain permission
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public ContextSecPermission WithAspectPermission(SecPermission sp) => new ContextSecPermission
        {
            Permission = this.Permission,
            Criteria = this.Criteria.Where(x => x.Key.HasFlag(sp)).ToDictionary(x => x.Key, x => x.Value)
        };
    }

    /// <summary>
    /// Security table for certain user
    /// in Init method we must pass filtered data
    /// </summary>
    public class ContextSecTable
    {
        private Dictionary<SMEntity, ContextSecPermission> _rows;

        public ContextSecTable()
        {
        }

        public void Init(IEnumerable<SMSecPolicy> policies)
        {
            var smSecPolicies = policies as SMSecPolicy[] ?? policies.ToArray();

            var subjects = smSecPolicies.SelectMany(x => x.Subjects);
            var criteria = smSecPolicies.SelectMany(x => x.Criteria);

            var result = new Dictionary<SMEntity, ContextSecPermission>();

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
                var up = new ContextSecPermission();

                foreach (var values in t.Items)
                {
                    up.Permission |= values.Permission;

                    var criterion = values.Criterion;
                    if (criterion != null)
                    {
                        var subject = criterion.Subject;
                        var sCriterion = criterion.Query;

                        if (up.Criteria.TryGetValue(criterion.Permission, out var pList))
                            pList.Add((criterion.Query, null));
                        else
                            up.Criteria[criterion.Permission] = new() { (sCriterion, null) };
                    }
                }

                result[t.Key] = up;
            }

            // TODO: this is primitive algo for creating user sec permissions dictionary
            // we need more complex algo for this
            // 1. Check for the same criterion query
            // 2. Merge UserSecPermission then we have many secs;
            // 3. Filtering criteria then we claim certain criterion

            _rows = result;
        }

        public ContextSecPermission this[SMEntity md]
        {
            get { return _rows[md]; }
        }

        public bool TryClaimSec(SMEntity md, out ContextSecPermission permission)
        {
            return _rows.TryGetValue(md, out permission);
        }

        public bool TryClaimPermission(SMEntity md, SecPermission permission, out ContextSecPermission claim)
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

            claim = claim.WithAspectPermission(permission);
            return true;
        }
    }
}