using BookOne.BookOne_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookOne.ViewModels
{
    public class ReturnBookViewModel
    {
        public BookCirculation Circulation { get; set; }

        public Reaction ReactionGiven { get; set; }
    }
}