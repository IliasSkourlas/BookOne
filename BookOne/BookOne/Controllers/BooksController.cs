using System.Net;
using System.Web.Mvc;
using BookOne.BookOne_Domain;
using BookOne.Models;
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

            return View(dbOps.AllBooksExceptOwners(loggedInUserId));
        }


        // GET: MyBooks
        public ActionResult MyBooks()
        {
            //Displays all Books owned by the logged in user
            var loggedInUserId = User.Identity.GetUserId();

            return View(dbOps.MyBooks(loggedInUserId));
        }


        // GET: MyHand
        public ActionResult MyHand()
        {
            //Displays all Books that the logged in user currently holds
            var loggedInUserId = User.Identity.GetUserId();

            var booksInUserHand = dbOps.MyHand(loggedInUserId);

            return View(booksInUserHand);
        }



        // GET: Books/Details/5
        public ActionResult Details(int? id)
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

        // GET: Books/Create
        public ActionResult Create()
        {
            var loggedInUser = dbOps.GetLoggedInUser(User.Identity.GetUserId());

            //Check if the loggedInUser is a Player. If he is not, he is redirected to enter additional information needed in order to become one.
            if (!dbOps.UserIsAPlayer(loggedInUser))
            {
                return View("~/Views/Player/PlayerForm.cshtml", loggedInUser);
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
                var loggedInUser = dbOps.GetLoggedInUser(User.Identity.GetUserId());

                book.Owner = loggedInUser;
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
        public ActionResult Delete(int? id)
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

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = dbOps.GetBook(id);
            dbOps.DeleteBook(book);
            return RedirectToAction("MyBooks");
        }



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
