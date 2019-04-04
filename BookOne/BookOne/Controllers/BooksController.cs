using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using BookOne.BookOne_Domain;
using BookOne.Models;
using BookOne.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;

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
            try
            {
                var loggedInUserId = User.Identity.GetUserId();

                return View(dbOps.AllBooks(loggedInUserId));
            }
            catch (SqlException)
            {

                throw;
            }
        }


        // GET: MyBooks
        public ActionResult MyBooks()
        {
            //Displays all Books owned by the logged in user
            try
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
            catch (SqlException)
            {

                throw;
            }
        }


        // GET: MyHand
        public ActionResult MyHand()
        {
            //Displays all Books that the logged in user currently holds
            try
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
            catch (SqlException)
            {

                throw;
            }
        }
        

        // GET: Books/Create
        public ActionResult Create()
        {
            try
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
            catch (SqlException)
            {

                throw;
            }
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
                try
                {
                    var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());

                    book.Owner = loggedInUser;
                    book.Carrier = loggedInUser;
                    dbOps.InsertBook(book);
                    return RedirectToAction("MyBooks");
                    
                }
                catch (SqlException)
                {

                    throw;
                }

                catch (WebException)
                {

                    throw;
                }
            }

            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            try
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
            catch (SqlException)
            {

                throw;
            }
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookId,Title,Author,RegisteredOn,BookStatus")] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dbOps.UpdateBook(book);
                    return RedirectToAction("MyBooks");
                }
                return View(book);
            }
            catch (SqlException)
            {

                throw;
            }
            catch (WebException)
            {

                throw;
            }
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
            try
            {
                Book book = dbOps.GetBook(id);
                dbOps.DeleteBook(book);
                return RedirectToAction("MyBooks");
            }
            catch (SqlException)
            {

                throw;
            }
            catch (WebException)
            {

                throw;
            }
        }


        //DaysRemaining counter for borrowed book
        public int DaysRemainingCounter(BookCirculation circulation)
        {
            //For testing purposes, borrowing time is set to 14 days(2 weeks)
            try
            {
                int weeksRemaining = circulation.BorrowedForXWeeks;
                int daysInTheseWeeks = weeksRemaining * 7;

                DateTime today = DateTime.Today;
                DateTime returnBookDate = circulation.BorrowedOn.AddDays(daysInTheseWeeks);

                return (returnBookDate - today).Days;
            }
            catch (SqlException)
            {

                throw;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public ActionResult History()
        {
            try
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
            catch (SqlException)
            {

                throw;
            }
        }

        
        public string PrepareHistoryForDownload()
        {
            try
            {
                var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());


                var model = new UserExchangeHistoryViewModel()
                {
                    BookRequests = dbOps.GetAllRequests(loggedInUser),
                    BookCirculations = dbOps.GetAllCirculations(loggedInUser)
                };

                return RenderViewToString("~/Views/Books/DownloadHistory.cshtml", model);
            }
            catch (SqlException)
            {

                throw;
            }
        }

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


        //Download function for a user's complete history of book exchanges. Creates a BookOne_History.txt file.
        [HttpPost]
        public FileStreamResult SaveHistory()
        {
            try
            {
                var meh = PrepareHistoryForDownload();

                Byte[] stream2 = Encoding.ASCII.GetBytes(meh);

                var stream = new MemoryStream(stream2);

                return File(stream, "text/html", "BookOne_History.html");
            }
            catch (WebException)
            {

                throw;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
            catch (SqlException)
            {

                throw;
            }
        }
    }
}
