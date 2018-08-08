using ZenPlatform.Shared.Tree;
using System.Collections.Generic;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    /// <summary>
    /// Модель представления корня конфигурации конфигурации
    /// </summary>
    public class XCRootViewModel : Node
    {
        public XCRootViewModel()
        {
            Data = new XCDataViewModel();

            Childs.AddRange(Data);
        }

        private XCDataViewModel Data { get; }
    }

    public class XCDataViewModel : Node, IChildItem<XCRootViewModel>
    {
        private XCRootViewModel _parent;

        public XCRootViewModel Parent => _parent;

        XCRootViewModel IChildItem<XCRootViewModel>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }

    public class ConfigurationViewModelNode : Node
    {
        /// <summary>
        /// Состояние ноды: открыта / закрыта
        /// </summary>
        public XCNodeState NodeState { get; set; }

        /// <summary>
        /// Иконка ноды
        /// </summary>
        public byte[] NodeIcon { get; set; }
    }

    public enum XCNodeState
    {
        Opened,
        Collapsed
    }
}