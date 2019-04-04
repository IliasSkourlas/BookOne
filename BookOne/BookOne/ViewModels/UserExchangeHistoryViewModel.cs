using BookOne.BookOne_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookOne.ViewModels
{
    public class UserExchangeHistoryViewModel
    {
        public IEnumerable<BookRequest> BookRequests { get; set; }

        public IEnumerable<BookCirculation> BookCirculations { get; set; }
    }
}