using BookOne.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookOne.BookOne_Domain
{
    public class Reaction
    {
        public Reaction()
        {
            ReceivedOn = DateTime.Now;
        }


        public string ReactionId { get; set; }

        public ApplicationUser ActionGiver { get; set; }

        public ApplicationUser ActionReceiver { get; set; }

        public Book ForBook { get; set; }

        public ReactionChoices Choice { get; set; }

        public DateTime ReceivedOn { get; set; }
    }


    public enum ReactionChoices
    {
        Clap,
        Shovel,
        Treat,
        PoisonousCookie,
        ConnectorBonus,
        Nothing
    }
}
