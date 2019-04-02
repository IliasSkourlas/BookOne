using BookOne.BookOne_Domain;
using BookOne.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookOne.ViewModels
{
    public class UserExchangeHistoryViewModel
    {
        public IEnumerable<Book> Books { get; set; }

        public IEnumerable<BookRequest> BookRequests { get; set; }

        public IEnumerable<BookCirculation> BookCirculations { get; set; }
    }
}