using BookOne.BookOne_Domain;
using BookOne.Models;
using BookOne.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace BookOne.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        DatabaseOperations dbOps = new DatabaseOperations();


        // GET: AllUsers
        public ActionResult Index()
        {
            try
            {
                var allUsers = dbOps.GetAllUsers();

                foreach (var user in allUsers)
                    user.Role = dbOps.GetUserRole(user);

                return View(allUsers);
            }
            catch (SqlException)
            {

                throw;
            }
        }

        // GET: AllBooks
        public ActionResult Books()
        {
            try
            {
                return View(dbOps.GetAllBooks());
            }
            catch (SqlException)
            {

                throw;
            }
        }

        // GET: AllBookCirculations
        public ActionResult BookCirculations()
        {
            try
            {
                var booksController = new BooksController();

                var circulations = dbOps.GetAllBookCirculations();

                foreach (var circulation in circulations)
                    circulation.DaysRemaining = booksController.DaysRemainingCounter(circulation);

                return View(circulations);
            }
            catch (SqlException)
            {

                throw;
            }
        }



        // GET: Books/Edit/5
        public ActionResult EditBook(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch (WebException)
            {

                throw;
            }
            try
            {
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBook([Bind(Include = "BookId,Title,Author,RegisteredOn,BookStatus")] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dbOps.UpdateBook(book);
                    return RedirectToAction("Books");
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


        // GET: Users/Edit/5
        public ActionResult EditUser(string userId)
        {
            try
            {
                var user = dbOps.GetUser(userId);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            catch (SqlException)
            {

                throw;
            }
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(ApplicationUser user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dbOps.UpdateUserDetails(user);
                    return RedirectToAction("Index");
                }
                return View(user);
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
        public ActionResult DeleteBook(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch (WebException)
            {

                throw;
            }
            try
            {
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

        // POST: Books/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBookConfirmed(int id)
        {
            try
            {
                Book book = dbOps.GetBook(id);
                dbOps.DeleteBook(book);
                return RedirectToAction("Books");
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


        // GET: Users/Delete/5
        public ActionResult DeleteUser(string userId)
        {
            try
            {
                var user = dbOps.GetUser(userId);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            catch (SqlException)
            {

                throw;
            }
        }

        // POST: Books/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(string userId)
        {
            try
            {
                var user = dbOps.GetUser(userId);
                dbOps.DeleteUser(user);
                return RedirectToAction("Index");
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
    }
}