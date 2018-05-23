using System;
using System.Collections.ObjectModel;
using Mono.Cecil;

namespace ZenPlatform.UIGeneration2
{
    [Serializable]
    public class ClassEntity
    {

        public AssemblyDefinition AssemblyDefinition { get; }

        public String AssemblyName => this.AssemblyDefinition.Name.Name;

        public String ClassName => this.TypeDefinition.Name;

        public Boolean IsSelected { get; set; }

        public String NamespaceName => this.TypeDefinition.Namespace;

        public string ProjectFrameworkVersion { get; }

        public ProjectType ProjectType { get; }

        public ObservableCollection<PropertyInformationViewModel> PropertyInformationCollection { get; }

        public TypeDefinition TypeDefinition { get; }

        public ClassEntity(AssemblyDefinition assemblyDefinition, TypeDefinition typeDefinition, ProjectType projectType, String projectFrameworkVersion)
        {
            if (assemblyDefinition == null)
            {
                throw new ArgumentNullException(nameof(assemblyDefinition));
            }
            if (typeDefinition == null)
            {
                throw new ArgumentNullException(nameof(typeDefinition));
            }

            this.AssemblyDefinition = assemblyDefinition;
            this.TypeDefinition = typeDefinition;
            this.ProjectType = projectType;
            this.ProjectFrameworkVersion = projectFrameworkVersion;
            this.PropertyInformationCollection = new ObservableCollection<PropertyInformationViewModel>();
        }

    }
}
