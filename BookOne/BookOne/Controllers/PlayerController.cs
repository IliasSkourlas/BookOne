﻿using BookOne.BookOne_Domain;
using BookOne.Models;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Web.Mvc;

namespace BookOne.Controllers
{
    [Authorize]
    public class PlayerController : Controller
    {
        DatabaseOperations dbOps = new DatabaseOperations();
        
        // GET: Player
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ChangeUserToPlayer(ApplicationUser loggedInUser)
        {
            if (ModelState.IsValid)
            {
                dbOps.UpdateUserDetails(loggedInUser);
                dbOps.ChangeUserRoleToPlayer(loggedInUser);
                return RedirectToAction("Requests");
            }

            return RedirectToAction("Index", "Home");
        }


        // GET: Requests
        public ActionResult Requests()
        {
            var loggedInUserId = User.Identity.GetUserId();
            var loggedInUser = dbOps.GetLoggedInUser(loggedInUserId);
            
            //Check if the loggedInUser is a Player. If he is not, he is redirected to enter additional information needed in order to become one.
            //if (!User.IsInRole("Player"))
            if (!dbOps.UserIsAPlayer(loggedInUser))
            {
                return View("PlayerForm", loggedInUser);
            }

            var requests = dbOps.GetRequests(loggedInUser);

            return View(requests);
        }

        //Create request for a book
        public ActionResult RequestConfirmation(int? bookId)
        {
            if (bookId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = dbOps.GetBook(bookId);
            if (book == null)
            {
                return HttpNotFound();
            }

            return View(book);
        }


        [HttpPost, ActionName("RequestConfirmation")]
        [ValidateAntiForgeryToken]
        public ActionResult RequestConfirmed(Book book)
        {
            var loggedInUserId = User.Identity.GetUserId();
            var loggedInUser = dbOps.GetLoggedInUser(loggedInUserId);

            var bookRequested = dbOps.GetBook(book.BookId);
            dbOps.InsertRequest(loggedInUser, bookRequested);

            return RedirectToAction("Requests");
        }


        
        public ActionResult BorrowerReceivesBook(int? BookRequestId)
        {
            if (BookRequestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var request = dbOps.GetBookRequest(BookRequestId);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View("BorrowingBookConfirmation", request);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BorrowingBookConfirmation(BookRequest request)
        {
            var borrower = request.RequestedBy;
            var bookBeingBorrowed = request.BookRequested;

            var circulation = dbOps.InsertBookCirculation(borrower, bookBeingBorrowed);

            dbOps.OwnerGaveBook(circulation);

            return View("Requests");
        }


        //Owner Declines to borrow his book for this request
        public ActionResult DeclineRequest(int? BookRequestId)
        {
            if (BookRequestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var request = dbOps.GetBookRequest(BookRequestId);
            if (request == null)
            {
                return HttpNotFound();
            }

            dbOps.DeclineRequest(request);
            return View("Requests");
        }
    }
}