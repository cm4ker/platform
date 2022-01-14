using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aquila.Metadata
{
    /// <summary>
    /// Provider semantic in metadata
    /// </summary>
    public class EntityMetadataCollection
    {
        private List<EntityMetadata> _metadata;
        private List<SecPolicyMetadata> _secMetadata;
        private List<SMEntity> _semanticMetadata;
        private List<SMSecPolicy> _secPolicies;
        private SMCache _cache;
        private bool _needUpdate;


        /// <summary>
        /// Constructor
        /// </summary>
        public EntityMetadataCollection()
        {
            _metadata = new List<EntityMetadata>();
        }

        public EntityMetadataCollection(IEnumerable<EntityMetadata> metadata) : this()
        {
            _metadata = metadata.ToList();
        }

        public IEnumerable<EntityMetadata> Metadata => _metadata;

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

            _semanticMetadata = _metadata.Select(x =>
            {
                var t = new SMEntity(x, _cache);
                _cache.AddType(t);
                return t;
            }).ToList();

            _secPolicies = _secMetadata.Select(x => new SMSecPolicy(x, _cache)).ToList();

            _needUpdate = false;
        }

        public SMEntity GetSemanticByName(string name) => GetSemanticMetadata().FirstOrDefault(x => x.Name == name);

        public SMEntity GetSemantic(Func<SMEntity, bool> criteria) => GetSemanticMetadata().FirstOrDefault(criteria);

        public SMSecPolicy GetSecPolicy(Func<SMSecPolicy, bool> criteria) => GetSecPolicies().FirstOrDefault(criteria);

        public IEnumerable<SMSecPolicy> GetSecPolicies(Func<SMSecPolicy, bool> criteria) =>
            GetSecPolicies().Where(criteria);

        public IEnumerator<EntityMetadata> GetEnumerator() => _metadata.GetEnumerator();

        public void AddMetadata(EntityMetadata metadata)
        {
            _metadata.Add(metadata);
            _needUpdate = true;
        }

        public void AddMetadataRange(IEnumerable<EntityMetadata> metadatas)
        {
            foreach (var md in metadatas)
            {
                _metadata.Add(md);
            }

            _needUpdate = true;
        }

        public bool IsEmpty()
        {
            return !_metadata.Any();
        }

        public void Clear()
        {
            _metadata.Clear();
            _semanticMetadata = null;
        }
    }
}