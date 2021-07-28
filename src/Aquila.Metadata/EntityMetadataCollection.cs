﻿using System.Collections;
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
        private List<SMEntity> _semanticMetadata;
        private SMCache _cache;
        private bool needUpdate;


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
            if (_semanticMetadata == null || needUpdate)
                CoreLazySemanticAnalyze();

            return _semanticMetadata;
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

            needUpdate = false;
        }

        public SMEntity GetSemanticByName(string name) => GetSemanticMetadata().FirstOrDefault(x => x.Name == name);

        public IEnumerator<EntityMetadata> GetEnumerator() => _metadata.GetEnumerator();

        public void AddMetadata(EntityMetadata metadata)
        {
            _metadata.Add(metadata);
            needUpdate = true;
        }

        public void AddMetadataRange(IEnumerable<EntityMetadata> metadatas)
        {
            foreach (var md in metadatas)
            {
                _metadata.Add(md);
            }

            needUpdate = true;
        }

        public bool IsEmpty()
        {
            return !_metadata.Any();
        }
    }
}