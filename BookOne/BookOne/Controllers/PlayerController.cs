using BookOne.BookOne_Domain;
using BookOne.Models;
using BookOne.ViewModels;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Web.Mvc;
using System.Data.SqlClient;

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


        public ActionResult ShowUserProfile(string userId)
        {
            try
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
            catch (SqlException)
            {

                throw;
            }
        }


        [HttpPost]
        public ActionResult ChangeUserToPlayer(ApplicationUser loggedInUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dbOps.UpdateUserDetails(loggedInUser);
                    dbOps.ChangeUserRoleToPlayer(loggedInUser);
                    return RedirectToAction("Index", "MyBooks");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (SqlException)
            {

                throw;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        // GET: Requests
        public ActionResult Requests()
        {
            try
            {
                var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());
                var userRole = dbOps.GetUserRole(loggedInUser);

                //Check if the loggedInUser is a Player. If he is not, he is redirected to enter additional information needed in order to become one.
                if (!(userRole == "Player" || userRole == "Administrator"))
                {
                    return View("PlayerForm", loggedInUser);
                }

                var requests = dbOps.GetRequests(loggedInUser);

                return View(requests);
            }
            catch (SqlException)
            {

                throw;
            }
        }


        //Create request for a book
        public ActionResult RequestConfirmation(int? bookId)
        {
            try
            {
                var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());
                var userRole = dbOps.GetUserRole(loggedInUser);

                //Check if the loggedInUser is a Player. If he is not, he is redirected to enter additional information needed in order to become one.
                if (!(userRole == "Player" || userRole == "Administrator"))
                {
                    return View("PlayerForm", loggedInUser);
                }

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
            catch (SqlException)
            {

                throw;
            }
        }


        //Borrower cancels a Book Request
        public ActionResult CancelConfirmation(int? RequestId)
        {
            try
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
            catch (SqlException)
            {

                throw;
            }
        }


        //Borrower wants to return a book
        public ActionResult ReturnBookRequest(int? CirculationId)
        {
            try
            {
                if (CirculationId == null)
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
                BookCirculation circulation = dbOps.GetBookCirculation(CirculationId);

                BookRequest request = dbOps.GetBookRequest(circulation.RequestIdForThisCirculation);
                if (request == null)
                {
                    return HttpNotFound();
                }

                dbOps.ReturnBookRequest(request);

                return RedirectToAction("MyHand", "Books");
            }
            catch (SqlException)
            {

                throw;
            }
        }


        //Owner Takes Back His Book
        public ActionResult OwnerTakesBackHisBook(int? RequestId)
        {
            try
            {
                if (RequestId == null)
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
            catch (SqlException)
            {

                throw;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //Owner is giving his book to the borrower
        public ActionResult BorrowerReceivesBook(int? RequestId)
        {
            try
            {
                if (RequestId == null)
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
                var request = dbOps.GetBookRequest(RequestId);
                if (request == null)
                {
                    return HttpNotFound();
                }
                return View("BorrowingBookConfirmation", request);
            }
            catch (SqlException)
            {

                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BorrowingBookConfirmation(BookRequest request)
        {
            try
            {
                var circulation = dbOps.InsertBookCirculation(request);

                return RedirectToAction("Requests");
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


        //Owner Declines to borrow his book for this request
        public ActionResult DeclineRequest(int? RequestId)
        {
            try
            {
                if (RequestId == null)
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
                var request = dbOps.GetBookRequest(RequestId);
                if (request == null)
                {
                    return HttpNotFound();
                }

                dbOps.DeclineRequest(request);
                return RedirectToAction("Requests");
            }
            catch (SqlException)
            {

                throw;
            }
        }

        //Borrower is receiving a book from its owner
        public ActionResult BorrowerReceivedBook(int? RequestId)
        {
            try
            {
                if (RequestId == null)
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
            catch (SqlException)
            {

                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BorrowedBookConfirmation(BookRequest request)
        {
            try
            {
                if (request.BookRequested.AvailabilityStatus == false)
                {
                    dbOps.CloseRequest(request);

                    ViewBag.RequestFailed = "This book is borrowed by someone else. The request you made is closed.";
                    return RedirectToAction("Requests");
                }

                dbOps.BorrowerReceivedBook(request);

                return RedirectToAction("Requests");
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


        //Used to make the Requests tab in the navbar blink when the loggedInUser has unanswered requests.
        public PartialViewResult CheckUnansweredRequests()
        {
            try
            {
                var meh = dbOps.NewRequestsCounter(User.Identity.GetUserId());
                if (meh > 0)
                    return PartialView("_UnansweredRequestsFoundPartial");
                else
                    return null;
            }
            catch (SqlException)
            {

                throw;
            }
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OwnerGetsBackHisBook(ReturnBookViewModel model)
        {
            try
            {
                dbOps.OwnerReceivedBookBack(model.Circulation);

                dbOps.InsertReaction(model.ReactionGiven);

                return RedirectToAction("Requests", "Player");
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