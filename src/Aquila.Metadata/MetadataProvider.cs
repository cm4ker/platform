using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aquila.Metadata
{
    /// <summary>
    /// Provider semantic in metadata
    /// </summary>
    public class MetadataProvider
    {
        private List<EntityMetadata> _entityMetadata;
        private List<SecPolicyMetadata> _secMetadata;
        private List<SMEntity> _semanticMetadata;
        private List<SMSecPolicy> _secPolicies;
        private SMCache _cache;
        private bool _needUpdate;


        /// <summary>
        /// Constructor
        /// </summary>
        public MetadataProvider()
        {
            _entityMetadata = new List<EntityMetadata>();
            _secMetadata = new List<SecPolicyMetadata>();
        }

        public MetadataProvider(IEnumerable<EntityMetadata> metadata,
            IEnumerable<SecPolicyMetadata> secMetadata) : this()
        {
            _entityMetadata = metadata.ToList();
            _secMetadata = secMetadata.ToList();
        }

        public IEnumerable<EntityMetadata> EntityMetadata => _entityMetadata;
        public IEnumerable<SecPolicyMetadata> SecMetadata => _secMetadata;

        public IEnumerable<SMEntity> GetSemanticMetadata()
        {
            if (_semanticMetadata == null || _needUpdate)
                CoreLazySemanticAnalyze();

            return _semanticMetadata;
        }

        public IEnumerable<SMSecPolicy> GetSecPolicies()
        {
            if (_secPolicies == null || _needUpdate)
                CoreLazySemanticAnalyze();

            return _secPolicies;
        }

        private void CoreLazySemanticAnalyze()
        {
            _cache = new SMCache();

            _semanticMetadata = _entityMetadata.Select(x =>
            {
                var t = new SMEntity(x, _cache);
                _cache.AddType(t, t.ReferenceName, SMTypeKind.Reference);
                _cache.AddType(t, t.Name, SMTypeKind.Object);

                return t;
            }).ToList();

            _secPolicies = _secMetadata.Select(x => new SMSecPolicy(x, _cache)).ToList();

            _needUpdate = false;
        }

        public SMEntity GetSemanticByName(string fullName) =>
            GetSemanticMetadata().FirstOrDefault(x => x.FullName == fullName);

        public SMEntity GetSemantic(Func<SMEntity, bool> criteria) => GetSemanticMetadata().FirstOrDefault(criteria);

        public SMSecPolicy GetSecPolicy(Func<SMSecPolicy, bool> criteria) => GetSecPolicies().FirstOrDefault(criteria);

        public IEnumerable<SMSecPolicy> GetSecPolicies(Func<SMSecPolicy, bool> criteria) =>
            GetSecPolicies().Where(criteria);

        public IEnumerable<SMSecPolicy> GetSecPoliciesFromRoles(IEnumerable<string> roles) =>
            GetSecPolicies((x => roles.Contains(x.Name)));

        public void AddMetadata(EntityMetadata metadata)
        {
            _entityMetadata.Add(metadata);
            _needUpdate = true;
        }

        public void AddMetadataRange(IEnumerable<EntityMetadata> metadatas)
        {
            foreach (var md in metadatas)
            {
                _entityMetadata.Add(md);
            }

            _needUpdate = true;
        }

        public bool IsEmpty()
        {
            return !_entityMetadata.Any();
        }

        public void Clear()
        {
            _entityMetadata.Clear();
            _semanticMetadata = null;
        }
    }
}