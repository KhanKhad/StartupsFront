using StartupsFront.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartupsFront.ViewModels
{
    public class MessageViewModel:BaseViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Owner { get; set; }

        public MessageViewModel() 
        {

        }

        public MessageViewModel(MessageModel message, string owner)
        {
            Id = message.Id;
            Message = message.Message;
            Owner = owner;
        }
    }
}
