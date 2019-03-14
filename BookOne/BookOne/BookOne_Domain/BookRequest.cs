using BookOne.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookOne.BookOne_Domain

{
    public class BookRequest
    {
        public BookRequest()
        {
            RequestedOn = DateTime.Now;
        }

        [Key]
        public int BookRequestId { get; set; }

        //[ForeignKey("BookId")]
        public virtual Book BookRequested { get; set; }

        //[ForeignKey("UserId")]
        public virtual ApplicationUser BookOwner { get; set; }

        //[ForeignKey("UserId")]
        public virtual ApplicationUser RequestedBy { get; set; }

        public DateTime RequestedOn { get; set; }

        public bool ApprovedByOwner { get; set; }
    }
}
