using BookOne.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookOne.BookOne_Domain

{
    public class BookRequest
    {
        public BookRequest()
        {
            RequestedOn = DateTime.Now;
        }

        public int BookRequestId { get; set; }

        public Book BookRequested { get; set; }

        public ApplicationUser BookOwner { get; set; }

        public ApplicationUser RequestedBy { get; set; }

        public DateTime RequestedOn { get; set; }

        public bool ApprovedByOwner { get; set; }
    }
}
