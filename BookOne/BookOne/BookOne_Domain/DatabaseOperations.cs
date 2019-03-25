using BookOne.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BookOne.BookOne_Domain
{
    public class DatabaseOperations
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        //Get loggedInUser by his Id
        public ApplicationUser GetLoggedInUser(string loggedInUserId)
        {
            return db.Users.Where(u => u.Id == loggedInUserId).SingleOrDefault();
        }


        //Returns All books inserted to the application except logged in user's books
        public IEnumerable<Book> AllBooksExceptOwners(string loggedInUserId)
        {
            return db.Books.Where(b => b.Owner.Id != loggedInUserId && b.BookStatus == BookStatuses.Public).Include(b => b.Owner).ToList();
        }


        //Returns All books owned by the logged in user
        public IEnumerable<Book> MyBooks(string loggedInUserId)
        {
            return db.Books.Where(b => b.Owner.Id == loggedInUserId).ToList();
        }


        //Returns All books that the user currently holds
        public IEnumerable<Book> MyHand(string loggedInUserId)
        {
            // my books not currently borrowed by anyone
            var ownedBooksNotCurrentlyBorrowed =
                db.Books.Where(b => b.Owner.Id == loggedInUserId).Except(
                    db.BookCirculations.Where(c => c.BookAssociated.Owner.Id == loggedInUserId && c.CirculationStatus == CirculationStatuses.Borrowed)
                    .Select(c => c.BookAssociated)).Include(b => b.Owner);

            // books currently borrowed by the logged in user
            var borrowedBooks =
                db.BookCirculations.Where(c => c.Borrower.Id == loggedInUserId && c.CirculationStatus == CirculationStatuses.Borrowed)
                .Select(c => c.BookAssociated).Include(b => b.Owner);
            
            return borrowedBooks.Union(ownedBooksNotCurrentlyBorrowed).ToList();
        }


        //Reads a book by its id from the database
        public Book GetBook(int? id)
        {
            return db.Books.Find(id);
        }


        //Returns a user by his id
        public ApplicationUser LoggedInUser(string id)
        {
            return db.Users.Where(u => u.Id == id).SingleOrDefault();
        }


        //Inserts a book to the database
        public void InsertBook(Book book)
        {
            db.Books.Add(book);
            db.SaveChanges();
        }


        //Updates a book to the database
        public void UpdateBook(Book book)
        {
            db.Entry(book).State = EntityState.Modified;
            db.SaveChanges();
        }
        

        //Removes a book from the database
        public void DeleteBook(Book book)
        {
            db.Books.Remove(book);
            db.SaveChanges();
        }


        public void UpdateUserDetails(ApplicationUser loggedInUser)
        {
            db.Entry(loggedInUser).State = EntityState.Modified;
            db.SaveChanges();
        }


        //Promote User to Player (change of his role)
        public void ChangeUserRoleToPlayer(ApplicationUser user)
        {
            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.RemoveFromRole(user.Id, "User");
            userManager.AddToRole(user.Id, "Player");
        }

        
        public bool UserIsAPlayer(ApplicationUser user)
        {
            int meh = user.Roles.Where(r => r.RoleId == "2").Count();
            if (meh > 0)
                return true;
            else
                return false;
        }


        public BookRequest GetBookRequest(int? id)
        {
            return db.BookRequests
                .Include(r => r.BookRequested)
                .Include(r => r.RequestedBy)
                .Where(r => r.BookRequestId == id).SingleOrDefault();
        }


        //Inserts Request to the Database
        public void InsertRequest(ApplicationUser userAskingForBook, Book book)
        {
            BookRequest request = new BookRequest();
            request.BookRequested = book;
            request.RequestedBy = userAskingForBook;
            db.BookRequests.Add(request);
            db.SaveChanges();
        }


        //Owner declines Request to give a book
        public void DeclineRequest(BookRequest request)
        {
            request.OwnerDeclined = true;
            request.RequestStatus = RequestStatuses.Declined;
            db.SaveChanges();
        }


        //Counts Requests of user's books
        public int RequestsReceivedCounter(ApplicationUser user)
        {
            return db.BookRequests.Where(r => r.BookRequested.Owner.Id == user.Id && r.RequestStatus == RequestStatuses.Unanswered).Count();
        }


        //Gets all user's book requests (user is the owner of this book)
        public IEnumerable<BookRequest> GetRequests(ApplicationUser user)
        {
             var booksRequestedFromTheLoggedInUser= db.BookRequests
                .Where(r => r.BookRequested.Owner.Id == user.Id && r.RequestStatus == RequestStatuses.Unanswered)
                .Include(r => r.BookRequested)
                .Include(r => r.RequestedBy)
                .Include(r => r.BookRequested.Owner);

            // pending BookRequests made by the logged in user
            var bookRequests_OwnerHasNotAnswered = 
                db.BookRequests
                .Where(r => r.RequestedBy.Id == user.Id && r.RequestStatus == RequestStatuses.Unanswered)
                .Include(r => r.BookRequested.Owner);

            // pending BookRequests to be approved by the logged in user
            var bookRequests_OwnerApproved =
                db.BookRequests.Where(r => r.RequestedBy.Id == user.Id && r.RequestStatus == RequestStatuses.Accepted)
                .Include(r => r.BookRequested.Owner);

            // pending BookRequests to be approved by the logged in user
            var bookRequests_OwnerDeclined =
                db.BookRequests.Where(r => r.RequestedBy.Id == user.Id && r.RequestStatus == RequestStatuses.Declined)
                .Include(r => r.BookRequested.Owner);

            return booksRequestedFromTheLoggedInUser
                .Union(bookRequests_OwnerHasNotAnswered)
                .Union(bookRequests_OwnerApproved)
                .Union(bookRequests_OwnerDeclined)
                .ToList();
        }


        //Adds BookCirculation for a book
        public BookCirculation InsertBookCirculation(BookRequest request)
        {
            var book = request.BookRequested;
            var userAskingForBook = request.RequestedBy;

            BookCirculation circulation = new BookCirculation();
            circulation.BookAssociated = book;
            circulation.Borrower = userAskingForBook;
            circulation.RequestForThisCirculation = request;
            db.BookCirculations.Add(circulation);

            return circulation;
        }
        //Borrow a book to someone (Owner gave book)
        public void OwnerGaveBook(BookCirculation circulation)
        {
            circulation.OwnerGaveBook = true;
            circulation.BookAssociated = circulation.RequestForThisCirculation.BookRequested;

            circulation.RequestForThisCirculation.RequestStatus = RequestStatuses.Accepted;

            db.SaveChanges();
        }
        //Borrow a book from someone (Borrower received book)
        public void BorrowerReceivedBook(BookRequest request)
        {
            var circulation = db.BookCirculations.Where(c => c.RequestForThisCirculation.BookRequestId == request.BookRequestId).SingleOrDefault();

            circulation.BorrowerReceivedBook = true;
            circulation.CirculationStatus = CirculationStatuses.Borrowed;
            circulation.BookAssociated.AvailabilityStatus = false;
            db.SaveChanges();
        }

        public BookCirculation GetBookLatestOnGoingCirculation(int? bookId)
        {
            var book = db.Books.Find(bookId);

            var circulation = db.BookCirculations.Where(c => c.BookAssociated.BookId == book.BookId && c.CirculationStatus != CirculationStatuses.Borrowed).Include(c => c.Borrower).Last();

            return circulation;
        }


        //public BookRequest BookRequestMadeForACirculation(BookCirculation circulation)
        //{
        //    return db.BookRequests
        //        .Where(r => r.BookRequested.BookId == circulation.BookAssociated.BookId &&
        //        r.RequestedBy.Id == circulation.Borrower.Id &&
        //        r.RequestStatus == RequestStatuses.Unanswered)
        //        .SingleOrDefault();
        //}

        //public BookCirculation BookRequestMadeForACirculation(BookRequest request)
        //{
        //    return db.BookCirculations
        //        .Where(c => c.BookAssociated.BookId == request.BookRequested.BookId &&
        //        c.Borrower.Id == request.RequestedBy.Id && 
        //        c.BorrowerReceivedBook == false)
        //        .LastOrDefault();
        //}


        //DaysRemaining counter for borrowed book
        public int DaysRemainingCounter(BookCirculation circulation)
        {
            //For testing purposes, borrowing time is set to 14 days(2 weeks)
            int weeksRemaining = circulation.BorrowedForXWeeks;
            int daysInTheseWeeks = weeksRemaining * 7;

            DateTime today = DateTime.Today;
            DateTime returnBookDate = circulation.BorrowedOn.AddDays(daysInTheseWeeks);

            return (returnBookDate - today).Days;
        }




        //Book returns to the owner
        public void OwnerReceivedBookBack(BookCirculation circulation)
        {
            circulation.CirculationStatus = CirculationStatuses.Completed;
            var book = db.Books.Where(b => b.BookId == circulation.BookAssociated.BookId).SingleOrDefault();
            book.AvailabilityStatus = true;
            db.SaveChanges();
        }
    }
}