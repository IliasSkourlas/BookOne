using BookOne.BookOne_Domain;
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


        //Display another user's details view (WITHOUT VIEWMODEL)
        //public ActionResult ShowUserProfile(string userId)
        //{
        //    ApplicationUser user = dbOps.GetUser(userId);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View("ShowUserProfile(old)", user);
        //}


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
                UserBooks = dbOps.MyBooks(userId)
                //UserReactions
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult ChangeUserToPlayer(ApplicationUser loggedInUser)
        {
            if (ModelState.IsValid)
            {
                dbOps.UpdateUserDetails(loggedInUser);
                dbOps.ChangeUserRoleToPlayer(loggedInUser);
                return RedirectToAction("Index", "Books");
            }

            return RedirectToAction("Index", "Home");
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        // GET: Requests
        public ActionResult Requests()
        {
            var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());

            //Check if the loggedInUser is a Player. If he is not, he is redirected to enter additional information needed in order to become one.
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

            return RedirectToAction("RequestConfirmed", book);
        }

        public ActionResult RequestConfirmed(Book book)
        {
            var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());
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

            return RedirectToAction("CancelConfirmed", request);
        }
        
        public ActionResult CancelConfirmed(BookRequest request)
        {
            dbOps.CancelRequest(request);

            return RedirectToAction("Requests");
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
            return View("BorrowedBookConfirmation", request);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BorrowedBookConfirmation(BookRequest request)
        {
            dbOps.BorrowerReceivedBook(request);

            return RedirectToAction("Requests");
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //Owner is receiving a book from its borrower
        public ActionResult ReturnBook(int? bookId)
        {
            if (bookId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var book = dbOps.GetBook(bookId);
            if (book == null)
            {
                return HttpNotFound();
            }
            var circulation = dbOps.GetBookLatestOnGoingCirculation(bookId);
            if (circulation == null)
            {
                return HttpNotFound();
            }

            var model = new ReturnBookViewModel()
            {
                Circulation = circulation,
                ReactionGiven = new Reaction()
                {
                    ActionGiverId = book.Owner.Id,
                    ActionReceiverId = circulation.Borrower.Id,
                    CirculationForThisReaction = circulation
                }
            };

            return View("BorrowerReturnsBook", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OwnerGetsBackHisBook(ReturnBookViewModel model)
        {
            dbOps.InsertReaction(model.ReactionGiven);

            dbOps.OwnerReceivedBookBack(model.Circulation);

            return RedirectToAction("MyBooks", "Books");
        }
    }
}