﻿using BookOne.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookOne.BookOne_Domain
{
    public class Reaction
    {
        public Reaction()
        {
            ReceivedOn = DateTime.Now;
        }


        public int ReactionId { get; set; }

        public string ActionGiverId { get; set; }

        public string ActionReceiverId { get; set; }

        public BookCirculation CirculationForThisReaction { get; set; }

        public ReactionChoices Choice { get; set; }

        public DateTime ReceivedOn { get; set; }
    }


    public enum ReactionChoices
    {
        Nothing,
        Clap,
        Shovel,
        Treat,
        PoisonousCookie,
        ConnectorBonus
    }
}
