using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.TypeSystem;
using ZenPlatform.SimpleIde.ViewModels;
using ZenPlatform.SimpleIde.Views;

namespace ZenPlatform.SimpleIde.Items
{
    [View(typeof(CodeEditorView))]
    class MetadataDocumentConfigurationItem : DocumentConfigurationItemBase
    {

        public MetadataDocumentConfigurationItem(Guid metadataId, TypeManager typeManager)
        {
            _metadataId = metadataId;
            Document = new TextDocument();
        }

        private object _metadataId;

        public TextDocument Document { get; }

        public IHighlightingDefinition Highlighting { get; }

    }
}
