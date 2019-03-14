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
    public class ClubMember
    {
        public ClubMember()
        {
            JoinedOn = DateTime.Now;
        }

        [Key]
        public int ClubMemberId { get; set; }

        [ForeignKey("ClubId")]
        public virtual Club AssociatedClub { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Member { get; set; }

        public DateTime JoinedOn { get; set; }

        public bool MemberIsConnector { get; set; }
    }
}
