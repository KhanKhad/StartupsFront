using System;
using System.Collections.Generic;
using System.Text;

namespace StartupsFront.Models
{
    public class MessageModel
    {
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;

        public int SenderForeignKey { get; set; }
        public UserModel Sender { get; set; }

        public int RecipientForeignKey { get; set; }
        public UserModel Recipient { get; set; }

        public DateTime MessageSended { get; set; }
        public DateTime Messagereaded { get; set; }

        public bool IsGetted { get; set; }
        public bool IsReaded { get; set; }
    }
}
