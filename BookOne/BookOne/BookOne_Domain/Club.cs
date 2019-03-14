using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookOne.BookOne_Domain
{
    public class Club
    {
        public Club()
        {
            CreatedOn = DateTime.Now;
        }

        [Key]
        public int ClubId { get; set; }

        public string ClubName { get; set; }

        public string ClubDescription { get; set; }

        public int ClubLocation { get; set; }

        public DateTime CreatedOn { get; set; }

        public ICollection<ClubMember> ClubMembers { get; set; }

        public ICollection<Book> ClubBooks { get; set; }
    }
}
