using BookOne.BookOne_Domain;
using BookOne.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookOne.ViewModels
{
    public class UserViewModel
    {
        public ApplicationUser User { get; set; }

        public IEnumerable<Book> UserBooks { get; set; }

        public IEnumerable<Reaction> UserReactions { get; set; }
    }
}