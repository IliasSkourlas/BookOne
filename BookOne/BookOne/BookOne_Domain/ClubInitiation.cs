using BookOne.Models;
using System.ComponentModel.DataAnnotations;

namespace BookOne.BookOne_Domain
{
    public class ClubInitiation
    {
        public int ClubInitiationId { get; set; }

        public Club AssociatedClub { get; set; }

        public ApplicationUser InitialMember { get; set; }

        public bool MemberApproval { get; set; }
    }
}
