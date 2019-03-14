using BookOne.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookOne.BookOne_Domain
{
    public class Book
    {
        public Book()
        {
            RegisteredOn = DateTime.Now;
        }

        [Key]
        public int BookId { get; set; }

        //[ForeignKey("UserId")]
        public virtual ApplicationUser Owner { get; set; }

        //[ForeignKey("ClubId")]
        public virtual Club AssociatedClub { get; set; }

        [Required(ErrorMessage = "Title field can not be empty.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author field can not be empty.")]
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