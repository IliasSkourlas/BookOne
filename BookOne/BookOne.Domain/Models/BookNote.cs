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
    public class BookNote
    {
        public BookNote()
        {
            ReceivedOn = DateTime.Now;
        }


        [Key]
        public int BookNoteId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book BookAssociated { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Giver { get; set; }

        public string Content { get; set; }

        public DateTime ReceivedOn { get; set; }
    }
}
