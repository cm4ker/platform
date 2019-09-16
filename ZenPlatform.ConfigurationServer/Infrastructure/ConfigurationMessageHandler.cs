using System;
using System.Diagnostics;
using System.Linq;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.IdeIntegration.Shared.Infrastructure;
using ZenPlatform.IdeIntegration.Shared.Messages;
using ZenPlatform.IdeIntegration.Shared.Models;

namespace ZenPlatform.IdeIntegration.Server.Infrastructure
{
    public class ConfigurationMessageHandler : IMessageHandler
    {
        private readonly XCRoot _conf;

        public ConfigurationMessageHandler(XCRoot conf)
        {
            _conf = conf;
        }

        public bool TryHandle(MessageEventArgs args)
        {
            if ((args.Message is XCTreeRequestMessage treeRequestMessage))
            {
                Handle(treeRequestMessage);
            }

            return true;
        }

        private void Handle(XCTreeRequestMessage treeRequest)
        {
            var responce = new XCTreeResponceMessage();

            switch (treeRequest.ItemType)
            {
                case XCNodeKind.Nil:
                {
                    responce.RequestId = treeRequest.RequestId;

                    var rootItem = new XCItem()
                    {
                        ItemType = XCNodeKind.Root, ItemId = _conf.ProjectId, ItemName = _conf.ProjectName
                    };

                    responce.Items.Add(rootItem);
                    break;
                }
                case XCNodeKind.Root:
                {
                    responce.RequestId = treeRequest.RequestId;
                    responce.ParentId = treeRequest.ItemId;

                    var dataItem = new XCItem() {ItemType = XCNodeKind.Data, ItemName = "Data"};
                    var rolesItem = new XCItem() {ItemType = XCNodeKind.Roles, ItemName = "Roles"};
                    var modulesItem = new XCItem() {ItemType = XCNodeKind.Modules, ItemName = "Modules"};
                    var languagesItem = new XCItem() {ItemType = XCNodeKind.Languages, ItemName = "Languages"};
                    var interfaceItem = new XCItem() {ItemType = XCNodeKind.Interface, ItemName = "Interfaces"};

                    responce.Items.AddRange(new[] {dataItem, rolesItem, modulesItem, languagesItem, interfaceItem});

                    break;
                }
                case XCNodeKind.Data:
                {
                    responce.RequestId = treeRequest.RequestId;
                    responce.ParentId = treeRequest.ItemId;

                    var items = _conf.Data.Components.Select(x => new XCItem()
                    {
                        ItemType = XCNodeKind.Component,
                        ItemId = x.Info.ComponentId,
                        ItemName = x.Info.ComponentName
                    }).ToList();

                    responce.Items.AddRange(items);
                    break;
                }
                case XCNodeKind.Component:
                {
                    responce.RequestId = treeRequest.RequestId;
                    responce.ParentId = treeRequest.ItemId;

                    var items = _conf.Data
                        .Components
                        .FirstOrDefault(x => x.Info.ComponentId == treeRequest.ItemId)
                        ?.Types.Select(x => new XCItem()
                        {
                            ItemType = XCNodeKind.Type,
                            ItemId = x.Guid,
                            ItemName = x.Name
                        }).ToList();

                    responce.Items.AddRange(items);
                    break;
                }
                case XCNodeKind.Type:
                {
                    responce.RequestId = treeRequest.RequestId;
                    responce.ParentId = treeRequest.ItemId;

                    var type = _conf.Data.ComponentTypes.FirstOrDefault(x => x.Guid == treeRequest.ItemId);
                    var attachedComponents = type.Parent.AttachedComponents;

                    if (type.HasProperties)
                    {
                        responce.Items.Add(new XCItem()
                        {
                            ItemType = XCNodeKind.PropertyRoot,
                            ItemName = "Properties",
                            ParentId = treeRequest.ItemId
                        });
                    }

                    //TODO: Обработать присоединённые компоненты

                    break;
                }
            }

            OnSendMessage(new MessageEventArgs(responce));
        }

        public event EventHandler<MessageEventArgs> SendMessage;

        protected virtual void OnSendMessage(MessageEventArgs e)
        {
            SendMessage?.Invoke(this, e);
        }
    }
}