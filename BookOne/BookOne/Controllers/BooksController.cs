using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using BookOne.BookOne_Domain;
using BookOne.ViewModels;
using Microsoft.AspNet.Identity;

namespace BookOne.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private DatabaseOperations dbOps = new DatabaseOperations();


        //Displays all Books
        public ActionResult Index()
        {
            var loggedInUserId = User.Identity.GetUserId();

            return View(dbOps.AllBooks(loggedInUserId));
        }


        //Displays all Books owned by the logged in user
        public ActionResult MyBooks()
        {
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


        //Displays all Books that the logged in user currently holds
        public ActionResult MyHand()
        {
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
                return View("~/Views/Player/PlayerForm.cshtml", loggedInUser);
            }

            return View();
        }

        // POST: Books/Create
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

        
        //Displays all Book Requests and Circulations that the loggedIn user was a part of.
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

        
        //Loads the model to be loaded in the History View
        public string PrepareHistoryForDownload()
        {
            var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());
            
            var model = new UserExchangeHistoryViewModel()
            {
                BookRequests = dbOps.GetAllRequests(loggedInUser),
                BookCirculations = dbOps.GetAllCirculations(loggedInUser)
            };

            return RenderViewToString("~/Views/Books/DownloadHistory.cshtml", model);
        }


        //Returns a given view as a string
        public string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


        //Download function for a user's complete history of book exchanges. Creates a BookOne_History.html file.
        [HttpPost]
        public FileStreamResult SaveHistory()
        {
            var viewAsString = PrepareHistoryForDownload();

            Byte[] viewAsAnArrayOfBytes = Encoding.ASCII.GetBytes(viewAsString);

            var stream = new MemoryStream(viewAsAnArrayOfBytes);

            return File(stream, "text/html", "BookOne_History.html");
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbOps.DisposeDB();
            }
            base.Dispose(disposing);
        }

        
    }
}
