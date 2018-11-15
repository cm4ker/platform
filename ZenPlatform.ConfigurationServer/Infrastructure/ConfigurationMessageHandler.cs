using System;
using System.Diagnostics;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.IdeIntegration.Messages.Messages;
using ZenPlatform.IdeIntegration.Messages.Models;

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


                    var dataItem = new XCItem() {NodeType = XCNodeKind.Data, ItemName = "Data"};

                    responce.Items.Add(dataItem);
                    break;
                }
                case XCNodeKind.Data:
                {
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