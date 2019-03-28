using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookOne.BookOne_Domain
{
    public class UserReaction
    {
        public UserReaction()
        {
            ReceivedOn = DateTime.Now;
        }


        public int UserReactionId { get; set; }

        public string ActionGiverId { get; set; }

        public string ActionReceiverId { get; set; }

        public BookCirculation CirculationForThisReaction { get; set; }

        public UserReactionChoices Choice { get; set; }

        public DateTime ReceivedOn { get; set; }
    }

    public enum UserReactionChoices
    {
        Nothing,
        Clap,
        Shovel,
        Treat,
        PoisonousCookie,
        ConnectorBonus
    }
}