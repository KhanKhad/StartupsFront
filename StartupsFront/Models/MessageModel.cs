using StartupsFront.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StartupsFront.Models
{
    public class MessageModel
    {
        [JsonPropertyName(JsonConstants.MessageId)]
        public int Id { get; set; }
        [JsonPropertyName(JsonConstants.MessageText)]

        public string Message { get; set; } = string.Empty;
        [JsonPropertyName(JsonConstants.MessageSender)]

        public int SenderForeignKey { get; set; }
        [JsonPropertyName(JsonConstants.MessageRecipient)]

        public int RecipientForeignKey { get; set; }

        public DateTime MessageSended { get; set; }
        public DateTime Messagereaded { get; set; }

        public bool IsGetted { get; set; }
        public bool IsReaded { get; set; }
    }
}
