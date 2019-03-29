using BookOne.BookOne_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookOne.ViewModels
{
    public class BooksViewModel
    {
        public IEnumerable<Book> Books { get; set; }

        public IEnumerable<BookCirculation> BookCirculations { get; set; }
    }
}