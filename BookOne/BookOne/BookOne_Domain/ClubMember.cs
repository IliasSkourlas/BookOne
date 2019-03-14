using BookOne.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookOne.BookOne_Domain
{
    public class ClubMember
    {
        public ClubMember()
        {
            JoinedOn = DateTime.Now;
        }

        public int ClubMemberId { get; set; }

        public Club AssociatedClub { get; set; }

        public ApplicationUser Member { get; set; }

        public DateTime JoinedOn { get; set; }

        public bool MemberIsConnector { get; set; }
    }
}
