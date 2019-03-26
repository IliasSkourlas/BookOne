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
            RequestStatus = RequestStatuses.Unanswered;
        }

        public int BookRequestId { get; set; }

        public Book BookRequested { get; set; }

        public ApplicationUser RequestedBy { get; set; }

        public DateTime RequestedOn { get; set; }

        public bool OwnerDeclined { get; set; }

        public RequestStatuses RequestStatus { get; set; }
    }

    public enum RequestStatuses
    {
        Unanswered,
        Accepted,
        Declined,
        Closed
    }
}
