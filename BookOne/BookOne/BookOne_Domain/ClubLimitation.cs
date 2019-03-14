using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookOne.BookOne_Domain
{
    public class ClubLimitation
    {
        [Key]
        public int ClubLimitationId { get; set; }

        //[ForeignKey("ClubId")]
        public virtual Club AssociatedClub { get; set; }

        public int MaxMembers { get; set; }

        public bool ClubIsOpen { get; set; }
    }
}
