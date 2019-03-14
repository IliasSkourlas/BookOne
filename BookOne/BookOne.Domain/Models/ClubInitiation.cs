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
    public class ClubInitiation
    {
        [Key]
        public int ClubInitiationId { get; set; }

        [ForeignKey("ClubId")]
        public virtual Club AssociatedClub { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser InitialMember { get; set; }

        public bool MemberApproval { get; set; }
    }
}
