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

        [JsonPropertyName(JsonConstants.MessageSenderId)]
        public int SenderForeignKey { get; set; }

        [JsonPropertyName(JsonConstants.MessageRecipientId)]
        public int RecipientForeignKey { get; set; }

        [JsonPropertyName(JsonConstants.MessageSended)]
        public DateTime MessageSended { get; set; }
        [JsonPropertyName(JsonConstants.MessageReaded)]
        public DateTime MessageReaded { get; set; }

        [JsonPropertyName(JsonConstants.MessageIsGetted)]
        public bool IsGetted { get; set; }
        [JsonPropertyName(JsonConstants.MessageIsReaded)]
        public bool IsReaded { get; set; }

        [JsonPropertyName(JsonConstants.MessageRecipientDelta)]
        public int RecipientDelta { get; set; }
        [JsonPropertyName(JsonConstants.MessageSenderDelta)]
        public int SenderDelta { get; set; }
    }
}
