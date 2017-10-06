using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQClient.Messages.Info
{
    public class InfoMessage : Message
    {
        public Guid To { get; set; }
        public int Size { get; set; }
        public string Info { get; set; }

    }
}
