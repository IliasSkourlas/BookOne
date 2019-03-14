using BookOne.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookOne.BookOne_Domain
{
    public class BookNote
    {
        public BookNote()
        {
            ReceivedOn = DateTime.Now;
        }


        public int BookNoteId { get; set; }

        public Book BookAssociated { get; set; }

        public ApplicationUser Giver { get; set; }

        [Required(ErrorMessage = "This field can not be empty")]
        public string Content { get; set; }

        public DateTime ReceivedOn { get; set; }
    }
}
