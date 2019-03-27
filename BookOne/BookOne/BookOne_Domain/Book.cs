using BookOne.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BookOne.BookOne_Domain
{
    public class Book
    {
        public Book()
        {
            RegisteredOn = DateTime.Now;
            BookStatus = BookStatuses.Public;
            AvailabilityStatus = true;
            CarrierUsername = Owner.ActualUsername;
        }

        
        public int BookId { get; set; }

        public ApplicationUser Owner { get; set; }

        public string CarrierUsername { get; set; }

        public Club AssociatedClub { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; }

        public DateTime RegisteredOn { get; set; }

        public BookStatuses BookStatus { get; set; }

        public bool AvailabilityStatus { get; set; }
    }

    public enum BookStatuses
    {
        Public,
        Hidden
    }
}