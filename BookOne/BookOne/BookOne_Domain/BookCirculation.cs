using BookOne.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookOne.BookOne_Domain
{
    public class BookCirculation
    {
        public BookCirculation()
        {
            BorrowedOn = DateTime.Now;
            CirculationStatus = CirculationStatuses.Fresh;
            BorrowedForXWeeks = 2;      //As default value until it is implemented in the UI
        }


        public int BookCirculationId { get; set; }

        public Book BookAssociated { get; set; }

        public ApplicationUser Borrower { get; set; }

        public DateTime BorrowedOn { get; set; }

        [Required(ErrorMessage = "A number of weeks is required")]
        public int BorrowedForXWeeks { get; set; }
        
        public bool OwnerGaveBook { get; set; }
        
        public bool BorrowerReceivedBook { get; set; }

        public BookRequest RequestForThisCirculation { get; set; }

        public CirculationStatuses CirculationStatus { get; set; }
    }


    public enum CirculationStatuses
    {
        Fresh,
        Borrowed,
        Completed
    }
}