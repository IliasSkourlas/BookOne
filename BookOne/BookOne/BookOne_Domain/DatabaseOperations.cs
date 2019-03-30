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


        //Get a user by his Id
        public ApplicationUser GetUser(string userId)
        {
            return db.Users
                .Where(u => u.Id == userId)
                .SingleOrDefault();
        }


        //Get Reactions for a user
        public IEnumerable<UserReaction> GetUserReactions(string userId)
        {
            return db.UserReactions.Where(r => r.ActionReceiverId == userId);
        }
        

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


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


        //Reads a book by its id from the database
        public Book GetBook(int? id)
        {
            return db.Books.Where(b => b.BookId == id).Include(b => b.Owner)
                .SingleOrDefault();
        }


        //Returns All books inserted to the application except logged in user's books
        public IEnumerable<Book> AllBooksExceptOwners(string loggedInUserId)
        {
            return db.Books.Where(b => b.Owner.Id != loggedInUserId && b.BookStatus == BookStatuses.Public)
                .Include(b => b.Owner)
                .ToList();
        }


        //Returns All books owned by the logged in user
        public IEnumerable<Book> MyBooks(string loggedInUserId)
        {
            return db.Books.Where(b => b.Owner.Id == loggedInUserId).Include(b => b.Carrier).ToList();
        }

        //Returns all circulations for the books owned by the logged in user
        public IEnumerable<BookCirculation> MyBooksCirculations(string loggedInUserId)
        {
            return db.BookCirculations.Where(c => c.BookAssociated.Owner.Id == loggedInUserId && c.CirculationStatus == CirculationStatuses.Borrowed).ToList();
        }

        //Returns all circulations for the books borrowed by the logged in user
        public IEnumerable<BookCirculation> BooksInMyHandCirculations(string loggedInUserId)
        {
            return db.BookCirculations.Where(c => c.Borrower.Id == loggedInUserId && c.CirculationStatus == CirculationStatuses.Borrowed).ToList();
        }


        //Returns All books that the user currently holds
        public IEnumerable<Book> MyHand(string loggedInUserId)
        {
            //This method could just work by checking which books have the loggedInUser's Id in their Carrier field.

            // my books not currently borrowed by anyone
            var ownedBooksNotCurrentlyBorrowed =
                db.Books.Where(b => b.Owner.Id == loggedInUserId).Except(
                    db.BookCirculations.Where(c => c.BookAssociated.Owner.Id == loggedInUserId && c.CirculationStatus == CirculationStatuses.Borrowed)
                    .Select(c => c.BookAssociated))
                    .Include(b => b.Owner);

            // books currently borrowed by the logged in user
            var borrowedBooks =
                db.BookCirculations.Where(c => c.Borrower.Id == loggedInUserId && c.CirculationStatus == CirculationStatuses.Borrowed)
                .Select(c => c.BookAssociated)
                .Include(b => b.Owner);
            
            return borrowedBooks.Union(ownedBooksNotCurrentlyBorrowed).ToList();
        }
        

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        

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
            int userRole = user.Roles.Where(r => r.RoleId == "2").Count();
            if (userRole > 0)
                return true;
            else
                return false;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


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
                db.BookRequests.Where(r => r.RequestedBy.Id == user.Id && r.RequestStatus == RequestStatuses.Unanswered)
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


        //Borrower cancels a Book Request
        public void CancelRequest(BookRequest request)
        {
            var requestToBeChanged = db.BookRequests.Find(request.BookRequestId);

            requestToBeChanged.RequestStatus = RequestStatuses.Closed;
            db.SaveChanges();
        }


        //Borrower wants to return a book
        public void ReturnBookRequest(BookRequest request)
        {
            var requestToBeChanged = db.BookRequests.Find(request.BookRequestId);

            requestToBeChanged.RequestStatus = RequestStatuses.Returning;
            request.BookRequested.BorrowerAskedToReturnThisBook = true;
            db.SaveChanges();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //Adds BookCirculation for a book / Owner is giving his book
        public BookCirculation InsertBookCirculation(BookRequest request)
        {
            var bookToBeBorrowed = db.Books.Find(request.BookRequested.BookId);
            var borrower = db.Users.Find(request.RequestedBy.Id);

            BookCirculation circulation = new BookCirculation()
            {
                RequestIdForThisCirculation = request.BookRequestId,
                BookAssociated = bookToBeBorrowed,
                Borrower = borrower,
                OwnerGaveBook = true,
            };
            
            db.BookCirculations.Add(circulation);

            var requestForThisCirculation = db.BookRequests.Find(request.BookRequestId);
            requestForThisCirculation.RequestStatus = RequestStatuses.Accepted;

            db.SaveChanges();

            return circulation;
        }
        //Borrow a book from someone (Borrower received book)
        public void BorrowerReceivedBook(BookRequest request)
        {
            var circulation = db.BookCirculations.Where(c => c.RequestIdForThisCirculation == request.BookRequestId)
                .SingleOrDefault();

            var bookToBeBorrowed = db.Books.Where(b => b.BookId == request.BookRequested.BookId)
                .SingleOrDefault();

            circulation.BorrowerReceivedBook = true;
            circulation.CirculationStatus = CirculationStatuses.Borrowed;
            bookToBeBorrowed.AvailabilityStatus = false;
            bookToBeBorrowed.Carrier = db.Users.Find(request.RequestedBy.Id);
            db.SaveChanges();
        }


        public BookCirculation GetBookLatestOnGoingCirculation(int? bookId)
        {
            var book = db.Books.Find(bookId);

            var circulation = db.BookCirculations.OrderByDescending(c => c.BorrowedOn)
                .Where(c => c.BookAssociated.BookId == book.BookId && c.CirculationStatus == CirculationStatuses.Borrowed)
                .Include(c => c.Borrower)
                .FirstOrDefault();

            return circulation;
        }


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
            var book = db.Books.Include(b => b.Owner).Where(b => b.BookId == circulation.BookAssociated.BookId).SingleOrDefault();
            var requestForThisCirculation = db.BookRequests.Find(circulation.RequestIdForThisCirculation);

            var borrower = requestForThisCirculation.RequestedBy;

            var circulationForThisBook = db.BookCirculations.Find(circulation.BookCirculationId);
            requestForThisCirculation.RequestStatus = RequestStatuses.Closed;

            book.AvailabilityStatus = true;
            book.Carrier = db.Users.Find(book.Owner.Id);
            circulationForThisBook.CirculationStatus = CirculationStatuses.Completed;
            db.SaveChanges();
        }


        //User gives a reaction(rating) to the borrower of one of his books.
        public void InsertReaction(UserReaction reaction)
        {
            reaction.CirculationForThisReaction = db.BookCirculations.Find(reaction.CirculationForThisReaction.BookCirculationId);
            db.UserReactions.Add(reaction);
            db.SaveChanges();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //Returns the number of times a book was in a succesful circulation
        public int BookCirculationsCounter(int? bookId)
        {
            return db.BookCirculations.Where(c => c.BookAssociated.BookId == bookId && c.CirculationStatus == CirculationStatuses.Completed).Count();
        }

        //Reactions a user has received after succesful circulations
        public IEnumerable<UserReaction> GetReactionsAUserReceived(string userId)
        {
            return db.UserReactions.Where(r => r.ActionReceiverId == userId).ToList();
        }

        //Returns the number of circulations a user has completed as a borrower
        public int CompletedUserBookCirculationsCounter(string userId)
        {
            return db.BookCirculations.Where(c => c.Borrower.Id == userId && c.CirculationStatus == CirculationStatuses.Completed).Count();
        }

        //Returns the number of books a user is currently borrowing
        public int OnGoingUserBookCirculationsCounter(string userId)
        {
            return db.BookCirculations.Where(c => c.Borrower.Id == userId && c.CirculationStatus == CirculationStatuses.Borrowed).Count();
        }
    }
}