using BookOne.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookOne.BookOne_Domain
{
    public class Book
    {
        public Book()
        {
            RegisteredOn = DateTime.Now;
            BookStatus = BookStatuses.Public;
        }

        
        public int BookId { get; set; }

        public ApplicationUser Owner { get; set; }

        public Club AssociatedClub { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; }

        public DateTime RegisteredOn { get; set; }

        public BookStatuses BookStatus { get; set; }
    }

    public enum BookStatuses
    {
        Public,
        Hidden
    }
}