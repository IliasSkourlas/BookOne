using BookOne.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookOne.Domain.Models
{
    public class Message
    {
        public Message()
        {
            SentOn = DateTime.Now;
        }


        [Key]
        public int MessageId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Sender { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Receiver { get; set; }

        public DateTime SentOn { get; set; }
    }
}
