using System.ComponentModel.DataAnnotations;

namespace BookOne.BookOne_Domain
{
    public class ClubLimitation
    {
        public int ClubLimitationId { get; set; }

        public Club AssociatedClub { get; set; }

        public int MaxMembers { get; set; }

        public bool ClubIsOpen { get; set; }
    }
}
