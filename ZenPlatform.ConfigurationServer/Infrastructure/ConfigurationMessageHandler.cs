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
                case XCNodeKind.Root:
                {
                    responce.RequestId = treeRequest.RequestId;
                    responce.ParentId = treeRequest.ItemId;

                    var dataItem = new XCItem() {ItemType = XCNodeKind.Data, ItemName = "Data"};

                    responce.Items.Add(dataItem);
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