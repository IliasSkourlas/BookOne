﻿using BookOne.BookOne_Domain;
using BookOne.Models;
using BookOne.ViewModels;
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


        //Displays a user's Profile View along with his received Reactions(Claps, Shovels, etc)
        public ActionResult ShowUserProfile(string userId)
        {
            ApplicationUser user = dbOps.GetUser(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            UserViewModel model = new UserViewModel()
            {
                User = user,
                UserBooks = dbOps.MyBooks(userId),
                UserReactions = dbOps.GetReactionsAUserReceived(userId)
            };

            ViewBag.CompletedCirculations = dbOps.CompletedUserBookCirculationsCounter(userId);
            ViewBag.OnGoingCirculations = dbOps.OnGoingUserBookCirculationsCounter(userId);
            return View(model);
        }


        //Upgrades a user's Role from simple User to Player
        [HttpPost]
        public ActionResult ChangeUserToPlayer(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                dbOps.UpdateUserDetails(user);
                dbOps.ChangeUserRoleToPlayer(user);
                return RedirectToAction("Index", "MyBooks");
            }

            return RedirectToAction("Index", "Home");
        }


        public ActionResult CheckIfUserIsAPlayer(ApplicationUser user)
        {
            var userRole = dbOps.GetUserRole(user);

            //Check if the loggedInUser is a Player. If he is not, he is redirected to enter additional information needed in order to become one.
            if (!(userRole == "Player" || userRole == "Administrator"))
            {
                return View("PlayerForm", user);
            }

            return null;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //Displays all on-going user's Requests
        public ActionResult Requests()
        {
            var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());

            CheckIfUserIsAPlayer(loggedInUser);

            var requests = dbOps.GetRequests(loggedInUser);

            return View(requests);
        }


        //Creates request for a book
        public ActionResult RequestConfirmation(int? bookId)
        {
            var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());

            CheckIfUserIsAPlayer(loggedInUser);

            if (bookId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = dbOps.GetBook(bookId);
            if (book == null)
            {
                return HttpNotFound();
            }
            
            var bookRequested = dbOps.GetBook(book.BookId);

            dbOps.InsertRequest(loggedInUser, bookRequested);

            return RedirectToAction("Requests");
        }


        //Borrower cancels a Book Request
        public ActionResult CancelConfirmation(int? RequestId)
        {
            if (RequestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRequest request = dbOps.GetBookRequest(RequestId);
            if (request == null)
            {
                return HttpNotFound();
            }

            dbOps.CancelRequest(request);

            return RedirectToAction("Requests");
        }


        //Borrower wants to return a book
        public ActionResult ReturnBookRequest(int? CirculationId)
        {
            if (CirculationId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookCirculation circulation = dbOps.GetBookCirculation(CirculationId);

            BookRequest request = dbOps.GetBookRequest(circulation.RequestIdForThisCirculation);
            if (request == null)
            {
                return HttpNotFound();
            }

            dbOps.ReturnBookRequest(request);

            return RedirectToAction("MyHand", "Books");
        }


        //Owner Takes Back His Book
        public ActionResult OwnerTakesBackHisBook(int? RequestId)
        {
            if (RequestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookRequest request = dbOps.GetBookRequest(RequestId);
            if (request == null)
            {
                return HttpNotFound();
            }

            var book = dbOps.GetBook(request.BookRequested.BookId);

            var circulation = dbOps.GetBookLatestOnGoingCirculation(book.BookId);
            if (circulation == null)
            {
                return HttpNotFound();
            }

            var model = new ReturnBookViewModel()
            {
                Circulation = circulation,
                ReactionGiven = new UserReaction()
                {
                    ActionGiverId = request.BookRequested.Owner.Id,
                    ActionReceiverId = circulation.Borrower.Id,
                    CirculationForThisReaction = circulation
                }
            };

            return View("BorrowerReturnsBook", model);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //Owner is giving his book to the borrower
        public ActionResult BorrowerReceivesBook(int? RequestId)
        {
            if (RequestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var request = dbOps.GetBookRequest(RequestId);
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
            var circulation = dbOps.InsertBookCirculation(request);
            
            return RedirectToAction("Requests");
        }


        //Owner Declines to borrow his book for this request
        public ActionResult DeclineRequest(int? RequestId)
        {
            if (RequestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var request = dbOps.GetBookRequest(RequestId);
            if (request == null)
            {
                return HttpNotFound();
            }

            dbOps.DeclineRequest(request);
            return RedirectToAction("Requests");
        }

        //Borrower is receiving a book from its owner
        public ActionResult BorrowerReceivedBook(int? RequestId)
        {
            if (RequestId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var request = dbOps.GetBookRequest(RequestId);
            if (request == null)
            {
                return HttpNotFound();
            }
            var book = dbOps.GetBook(request.BookRequested.BookId);
            if (request == null)
            {
                return HttpNotFound();
            }

            ViewBag.Owner = book.Owner.ActualUsername;
            return View("BorrowedBookConfirmation", request);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BorrowedBookConfirmation(BookRequest request)
        {
            if(request.BookRequested.AvailabilityStatus == false)
            {
                dbOps.CloseRequest(request);

                ViewBag.RequestFailed = "This book is borrowed by someone else. The request you made is closed.";
                return RedirectToAction("Requests");
            }

            dbOps.BorrowerReceivedBook(request);

            return RedirectToAction("Requests");
        }


        //Used to make the Requests tab in the navbar blink when the loggedInUser has unanswered requests.
        public PartialViewResult CheckUnansweredRequests()
        {
            var meh = dbOps.NewRequestsCounter(User.Identity.GetUserId());
            if (meh > 0)
                return PartialView("_UnansweredRequestsFoundPartial");
            else
                return null;
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OwnerGetsBackHisBook(ReturnBookViewModel model)
        {
            dbOps.OwnerReceivedBookBack(model.Circulation);

            dbOps.InsertReaction(model.ReactionGiven);

            return RedirectToAction("Requests", "Player");
        }
    }
}