using BookOne.BookOne_Domain;
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

        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        

        // GET: Requests
        public ActionResult Requests()
        {
            var loggedInUser = dbOps.GetLoggedInUser(User.Identity.GetUserId());

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
            var loggedInUser = dbOps.GetLoggedInUser(User.Identity.GetUserId());
            var bookRequested = dbOps.GetBook(book.BookId);

            dbOps.InsertRequest(loggedInUser, bookRequested);

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
            var circulation = dbOps.GetBookLatestOnGoingCirculation(bookId);
            if (circulation == null)
            {
                return HttpNotFound();
            }
            return View("BorrowerReturnsBook", circulation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OwnerGetsBackHisBook(BookCirculation circulation)
        {
            dbOps.OwnerReceivedBookBack(circulation);

            return RedirectToAction("Requests");
        }
    }
}