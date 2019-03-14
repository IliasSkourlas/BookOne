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
    public class BookCirculation
    {
        [Key]
        public int BookCirculationId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book BookAssociated { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Owner { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Borrower { get; set; }

        public DateTime BorrowedOn { get; set; }

        public int BorrowedForXWeeks { get; set; }

        public bool OwnerGaveBook { get; set; }

        public bool BorrowerReceivedBook { get; set; }

        public CirculationStatuses CirculationStatus { get; set; }
    }


    public enum CirculationStatuses
    {
        Borrowed,
        Completed
    }
}
