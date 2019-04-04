using System;
using System.Net;
using System.Web.Mvc;
using BookOne.BookOne_Domain;
using BookOne.Models;
using BookOne.ViewModels;
using Microsoft.AspNet.Identity;

namespace BookOne.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private DatabaseOperations dbOps = new DatabaseOperations();


        // GET: Books
        public ActionResult Index()
        {
            //Displays all Books not owned by the logged in user
            var loggedInUserId = User.Identity.GetUserId();

            return View(dbOps.AllBooks(loggedInUserId));
        }


        // GET: MyBooks
        public ActionResult MyBooks()
        {
            //Displays all Books owned by the logged in user
            var loggedInUserId = User.Identity.GetUserId();

            var model = new BooksViewModel()
            {
                Books = dbOps.MyBooks(loggedInUserId),
                BookCirculations = dbOps.MyBooksCirculations(loggedInUserId)
            };

            foreach (var circulation in model.BookCirculations)
                circulation.DaysRemaining = DaysRemainingCounter(circulation);

            return View(model);
        }


        // GET: MyHand
        public ActionResult MyHand()
        {
            //Displays all Books that the logged in user currently holds
            var loggedInUserId = User.Identity.GetUserId();
            
            var model = new BooksViewModel()
            {
                Books = dbOps.MyHand(loggedInUserId),
                BookCirculations = dbOps.BooksInMyHandCirculations(loggedInUserId)
            };

            foreach (var circulation in model.BookCirculations)
                circulation.DaysRemaining = DaysRemainingCounter(circulation);

            return View(model);
        }
        

        // GET: Books/Create
        public ActionResult Create()
        {
            var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());
            var userRole = dbOps.GetUserRole(loggedInUser);

            //Check if the loggedInUser is a Player. If he is not, he is redirected to enter additional information needed in order to become one.
            if (!(userRole == "Player" || userRole == "Administrator"))
            {
                return View("PlayerForm", loggedInUser);
            }

            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookId,Title,Author,RegisteredOn,BookStatus")] Book book)
        {
            if (ModelState.IsValid)
            {
                var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());

                book.Owner = loggedInUser;
                book.Carrier = loggedInUser;
                dbOps.InsertBook(book);
                return RedirectToAction("MyBooks");
            }

            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = dbOps.GetBook(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookId,Title,Author,RegisteredOn,BookStatus")] Book book)
        {
            if (ModelState.IsValid)
            {
                dbOps.UpdateBook(book);
                return RedirectToAction("MyBooks");
            }
            return View(book);
        }

        // GET: Books/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Book book = dbOps.GetBook(id);
        //    if (book == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(book);
        //}

        // POST: Books/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = dbOps.GetBook(id);
            dbOps.DeleteBook(book);
            return RedirectToAction("MyBooks");
        }


        //DaysRemaining counter for borrowed book
        public int DaysRemainingCounter(BookCirculation circulation)
        {
            //For testing purposes, borrowing time is set to 14 days(2 weeks)
            int weeksRemaining = circulation.BorrowedForXWeeks;
            int daysInTheseWeeks = weeksRemaining * 7;

            DateTime today = DateTime.Today;
            DateTime returnBookDate = circulation.BorrowedOn.AddDays(daysInTheseWeeks);

            return (returnBookDate - today).Days;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public ActionResult History()
        {
            var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());
            var userRole = dbOps.GetUserRole(loggedInUser);

            //Check if the loggedInUser is a Player. If he is not, he is redirected to enter additional information needed in order to become one.
            if (!(userRole == "Player" || userRole == "Administrator"))
            {
                return View("~/Views/Player/PlayerForm.cshtml", loggedInUser);
            }

            var model = new UserExchangeHistoryViewModel()
            {
                BookRequests = dbOps.GetAllRequests(loggedInUser),
                BookCirculations = dbOps.GetAllCirculations(loggedInUser)
            };

            return View(model);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
