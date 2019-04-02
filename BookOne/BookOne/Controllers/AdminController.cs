using BookOne.BookOne_Domain;
using BookOne.Models;
using BookOne.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BookOne.Controllers
{
    public class AdminController : Controller
    {
        DatabaseOperations dbOps = new DatabaseOperations();


        // GET: AllUsers
        public ActionResult Index()
        {
            return View(dbOps.GetAllUsers());
        }

        // GET: AllBooks
        public ActionResult Books()
        {
            return View(dbOps.GetAllBooks());
        }

        // GET: AllBookCirculations
        public ActionResult BookCirculations()
        {
            return View(dbOps.GetAllBookCirculations());
        }



        // GET: Books/Edit/5
        public ActionResult EditBook(int? id)
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
        public ActionResult EditBook([Bind(Include = "BookId,Title,Author,RegisteredOn,BookStatus")] Book book)
        {
            if (ModelState.IsValid)
            {
                dbOps.UpdateBook(book);
                return RedirectToAction("Books");
            }
            return View(book);
        }


        // GET: Users/Edit/5
        public ActionResult EditUser(string userId)
        {
            var user = dbOps.GetUser(userId);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                dbOps.UpdateUser(user);
                return RedirectToAction("Users");
            }
            return View(user);
        }



        // GET: Books/Delete/5
        public ActionResult DeleteBook(int? id)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBookConfirmed(int id)
        {
            Book book = dbOps.GetBook(id);
            dbOps.DeleteBook(book);
            return RedirectToAction("Books");
        }


        // GET: Users/Delete/5
        public ActionResult DeleteUser(string userId)
        {
            var user = dbOps.GetUser(userId);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Books/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(string userId)
        {
            var user = dbOps.GetUser(userId);
            dbOps.DeleteUser(user);
            return RedirectToAction("Users");
        }
    }
}