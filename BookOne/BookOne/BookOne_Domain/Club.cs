using System;
using System.ComponentModel.DataAnnotations;

namespace BookOne.BookOne_Domain
{
    public class Club
    {
        public Club()
        {
            CreatedOn = DateTime.Now;
        }

        public int ClubId { get; set; }

        [Required(ErrorMessage = "A club name is required")]
        public string ClubName { get; set; }

        public string ClubDescription { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public int ClubLocation { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
