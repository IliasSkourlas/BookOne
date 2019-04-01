using BookOne.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
            BorrowerAskedToReturnThisBook = false;
        }

        
        public int BookId { get; set; }

        public ApplicationUser Owner { get; set; }

        public ApplicationUser Carrier { get; set; }

        public Club AssociatedClub { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; }

        public DateTime RegisteredOn { get; set; }

        public BookStatuses BookStatus { get; set; }

        public bool AvailabilityStatus { get; set; }

        [NotMapped]
        public int CompletedCirculationsForThisBook { get; set; }
        
        public bool BorrowerAskedToReturnThisBook { get; set; }
    }

    public enum BookStatuses
    {
        Public,
        Hidden
    }
}