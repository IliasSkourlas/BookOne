using BookOne.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookOne.BookOne_Domain
{
    public class Message
    {
        public Message()
        {
            SentOn = DateTime.Now;
        }


        public int MessageId { get; set; }

        public ApplicationUser Sender { get; set; }

        public ApplicationUser Receiver { get; set; }

        [Required(ErrorMessage = "A message can not be empty")]
        public string Content { get; set; }

        public DateTime SentOn { get; set; }
    }
}
