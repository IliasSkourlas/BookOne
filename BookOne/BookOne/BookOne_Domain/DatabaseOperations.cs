using BookOne.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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


        public IEnumerable<ApplicationUser> GetAllOtherUsers(string userId)
        {
            return db.Users.Where(u => u.Id != userId).ToList();
        }


        //Get Reactions for a user
        public IEnumerable<UserReaction> GetUserReactions(string userId)
        {
            return db.UserReactions.Where(r => r.ActionReceiverId == userId);
        }


        //Check a user's status during Login.
        public bool AccountIsDisabled(string email)
        {
            var user = db.Users.Where(u => u.Email == email).SingleOrDefault();

            if (user.UserStatus == UserStatuses.Deleted)
                return true;
            else
                return false;
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
        //public IEnumerable<Book> AllBooksExceptOwners(string loggedInUserId)
        //{
        //    return db.Books.Where(b => b.Owner.Id != loggedInUserId && b.BookStatus == BookStatuses.Public)
        //        .Include(b => b.Owner)
        //        .ToList();
        //}

        //Returns All books inserted to the application except logged in user's books
        public IEnumerable<Book> AllBooks(string loggedInUserId)
        {
            return db.Books.Where(b => b.BookStatus == BookStatuses.Public)
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
        

        public void UpdateUserDetails(ApplicationUser user)
        {
            db.Entry(user).State = EntityState.Modified;
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

            // pending BookRequests to be approved by the logged in user for books returning(user is the owner of these books)
            var bookRequests_BorrowerWantsToReturnBook =
                db.BookRequests.Where(r => r.BookRequested.Owner.Id == user.Id && r.RequestStatus == RequestStatuses.Returning)
                .Include(r => r.BookRequested.Owner);

            // pending BookRequests to be approved by the logged in user for books returning(user is the borrower of these books)
            //var bookRequests_BorrowerAskedToReturnBook =
            //    db.BookRequests.Where(r => r.RequestedBy.Id == user.Id && r.RequestStatus == RequestStatuses.Returning)
            //    .Include(r => r.BookRequested.Owner);

            return booksRequestedFromTheLoggedInUser
                .Union(bookRequests_OwnerHasNotAnswered)
                .Union(bookRequests_OwnerApproved)
                .Union(bookRequests_OwnerDeclined)
                .Union(bookRequests_BorrowerWantsToReturnBook)
                .ToList();
        }


        //Gets all user's requests (Unfiltered)
        public IEnumerable<BookRequest> GetAllRequests(ApplicationUser user)
        {
            var usersRequestsAsOwner = db.BookRequests
                .Where(r => r.BookRequested.Owner.Id == user.Id)
                .Include(r => r.BookRequested.Owner)
                .Include(r => r.RequestedBy);

            var usersRequestsAsBorrower = db.BookRequests
                .Where(r => r.RequestedBy.Id == user.Id)
                .Include(r => r.BookRequested.Owner)
                .Include(r => r.RequestedBy);

            return usersRequestsAsOwner
                .Union(usersRequestsAsBorrower)
                .ToList();
        }


        public void CloseRequest(BookRequest request)
        {
            var requestToBeChanged = db.BookRequests.Find(request.BookRequestId);

            requestToBeChanged.RequestStatus = RequestStatuses.Closed;
            db.SaveChanges();
        }


        //Borrower cancels a Book Request
        public void CancelRequest(BookRequest request)
        {
            var requestToBeChanged = db.BookRequests.Find(request.BookRequestId);

            requestToBeChanged.RequestStatus = RequestStatuses.Canceled;
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

            var thisRequest = db.BookRequests.Find(request.BookRequestId);

            thisRequest.RequestStatus = RequestStatuses.BorrowedBook;
            circulation.BorrowerReceivedBook = true;
            circulation.CirculationStatus = CirculationStatuses.Borrowed;
            bookToBeBorrowed.AvailabilityStatus = false;
            bookToBeBorrowed.Carrier = db.Users.Find(request.RequestedBy.Id);
            db.SaveChanges();
        }

        public BookCirculation GetBookCirculation(int? CirculationId)
        {
            return db.BookCirculations.Find(CirculationId);
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


        //Gets all user's Circulations (Unfiltered)
        public IEnumerable<BookCirculation> GetAllCirculations(ApplicationUser user)
        {
            var usersCirculationsAsOwner = db.BookCirculations
                .Where(c => c.BookAssociated.Owner.Id == user.Id)
                .Include(c => c.BookAssociated.Owner)
                .Include(c => c.Borrower);

            var usersCirculationsAsBorrower = db.BookCirculations
                .Where(c => c.Borrower.Id == user.Id && c.CirculationStatus == CirculationStatuses.Borrowed)
                .Include(c => c.BookAssociated.Owner)
                .Include(c => c.Borrower);

            return usersCirculationsAsOwner
                .Union(usersCirculationsAsBorrower)
                .ToList();
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
            book.BorrowerAskedToReturnThisBook = false;
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



        //Returns user's number of unanswered requests
        public int NewRequestsCounter(string loggedInUserId)
        {
            //Requests made by other others for loggedInUser's books
            var unansweredRequests = db.BookRequests
                .Where(r => r.BookRequested.Owner.Id == loggedInUserId && r.RequestStatus == RequestStatuses.Unanswered)
                .Count();

            //LoggedInUser's Requests - Owner accepted to borrow his book
            var acceptedRequests = db.BookRequests
                .Where(r => r.RequestedBy.Id == loggedInUserId && r.RequestStatus == RequestStatuses.Accepted)
                .Count();

            //LoggedInUser's Requests from other users to return his books
            var returningRequests = db.BookRequests
                .Where(r => r.BookRequested.Owner.Id == loggedInUserId && r.RequestStatus == RequestStatuses.Returning && r.BookRequested.BorrowerAskedToReturnThisBook == true)
                .Count();

            return unansweredRequests + acceptedRequests + returningRequests;
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public void InsertMessage(Message message)
        {
            db.Messages.Add(message);
            db.SaveChanges();
        }


        //Returns all messages between the logged in user and another user.
        public IEnumerable<Message> GetConversation(string loggedInUserId, string contactId)
        {
            return db.Messages
                .Where(c => (c.Receiver.Id == loggedInUserId && c.Sender.Id == contactId) || 
                (c.Receiver.Id == contactId && c.Sender.Id == loggedInUserId))
                .OrderBy(c => c.SentOn)
                .ToList();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        
        //Inserts a visitor's Email Address to the database in order to notify him when the application is ready
        public void InsertEmailNotification(EmailNotification notification)
        {
            db.EmailNotifications.Add(notification);
            db.SaveChanges();
        }


        //Returns the number of users that want to be notified when the application is ready
        public int EmailNotificationsCounter()
        {
            return db.EmailNotifications.Count();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //ADMIN OPERATIONS


        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            return db.Users
                .OrderBy(u => u.RegisteredOn)
                .ToList();
        }


        public IEnumerable<Book> GetAllBooks()
        {
            return db.Books
                .Include(b => b.Owner)
                .OrderBy(b => b.RegisteredOn)
                .ToList();
        }


        public IEnumerable<BookCirculation> GetAllBookCirculations()
        {
            return db.BookCirculations
                .Where(c => c.CirculationStatus != CirculationStatuses.Fresh)
                .Include(c => c.BookAssociated)
                .Include(c => c.Borrower)
                .OrderBy(c => c.BorrowedOn)
                .ToList();
        }


        //Removes a user from the database
        public void DeleteUser(ApplicationUser user)
        {
            user.UserStatus = UserStatuses.Deleted;
            db.SaveChanges();
        }


        public string GetUserRole(ApplicationUser user)
        {
            if (user.Roles.Where(r => r.RoleId == "3").Count() > 0)
                return "Administrator";
            if (user.Roles.Where(r => r.RoleId == "2").Count() > 0)
                return "Player";
            if (user.Roles.Where(r => r.RoleId == "1").Count() > 0)
                return "User";
            return null;
        }


        //public void ChangeUserRole(ApplicationUser user)
        //{
        //    var savedUser = db.Users.Find(user.Id);
        //    var oldRole = savedUser.Roles.SingleOrDefault().RoleId;

        //    var roleStore = new RoleStore<IdentityRole>(db);
        //    var roleManager = new RoleManager<IdentityRole>(roleStore);
        //    var userStore = new UserStore<ApplicationUser>(db);
        //    var userManager = new UserManager<ApplicationUser>(userStore);
        //    userManager.RemoveFromRole(user.Id, oldRole);
        //    userManager.AddToRole(user.Id, user.userRole.ToString());
        //}
    }
}