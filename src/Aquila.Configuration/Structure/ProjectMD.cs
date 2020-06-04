using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Contracts.Configuration;

namespace Aquila.Configuration.Structure
{
    public class ProjectMD : IEquatable<ProjectMD>
    {
        public ProjectMD()
        {
            ComponentReferences = new List<IComponentRef>();
            ProjectId = Guid.NewGuid();
        }

        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectVersion { get; set; }

        public List<IComponentRef> ComponentReferences { get; set; }

        public bool Equals(ProjectMD other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ProjectId.Equals(other.ProjectId) && ProjectName == other.ProjectName &&
                   ProjectVersion == other.ProjectVersion &&
                   ComponentReferences.SequenceEqual(other.ComponentReferences);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProjectMD) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProjectId, ProjectName, ProjectVersion, ComponentReferences);
        }
    }
}