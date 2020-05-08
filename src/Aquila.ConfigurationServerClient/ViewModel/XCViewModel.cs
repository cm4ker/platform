using Aquila.Shared.ParenChildCollection;
using Aquila.Shared.Tree;

namespace Aquila.IdeIntegration.Client.ViewModel
{
    /// <summary>
    /// Модель представления корня конфигурации конфигурации
    /// </summary>
    public class XCRootViewModel : ConfigurationViewModelNode
    {
        public XCRootViewModel()
        {
            Data = new XCDataViewModel();

            Attach(Data);
        }

        public XCDataViewModel Data { get; }
    }

    public class XCDataViewModel : ConfigurationViewModelNode, IChildItem<XCRootViewModel>
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