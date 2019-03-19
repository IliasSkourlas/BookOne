using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
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

        // GET: Books
        public ActionResult Index()
        {
            //Displays all Books not owned by the logged in user
            var loggedInUserId = User.Identity.GetUserId();

            return View(db.Books.Where(b => b.Owner.Id != loggedInUserId).ToList());
        }

        // GET: MyBooks
        public ActionResult MyBooks()
        {
            //Displays all Books owned by the logged in user
            var loggedInUserId = User.Identity.GetUserId();

            return View(db.Books.Where(b => b.Owner.Id == loggedInUserId).ToList());
        }

        // GET: MyHand
        public ActionResult MyHand()
        {
            //Displays all Books that the logged in user currently holds

            var loggedInUserId = User.Identity.GetUserId();
            var userBooks = db.Books.Where(b => b.Owner.Id == loggedInUserId).ToList();
            var booksInUserHand = new List<Book>();

            foreach (var book in userBooks)
            {
                var bookCirculations = book.ThisBookCirculations.LastOrDefault().CirculationStatus != CirculationStatuses.Borrowed;

                if (bookCirculations == false)
                {
                    booksInUserHand.Add(book);
                }
            }
            
            return View(booksInUserHand);
        }


        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
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
                var loggedInUserId = User.Identity.GetUserId();
                var loggedInUser = db.Users.Where(u => u.Id == loggedInUserId).SingleOrDefault();

                book.Owner = loggedInUser;
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
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
            Book book = db.Books.Find(id);
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
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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
            Book book = db.Books.Find(id);
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
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
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
