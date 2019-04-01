using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookOne.BookOne_Domain
{
    public class EmailNotification
    {
        public EmailNotification()
        {
            ReceivedOn = DateTime.Now;
        }

        public int EmailNotificationId { get; set; }

        [Required(ErrorMessage = "An Email address is required.")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        public DateTime ReceivedOn { get; set; }
    }
}