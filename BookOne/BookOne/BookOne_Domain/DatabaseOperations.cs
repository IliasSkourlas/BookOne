using BookOne.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BookOne.BookOne_Domain
{
    public class DatabaseOperations
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        //Returns All books inserted to the application except logged in user's books
        public IEnumerable<Book> AllBooksExceptOwners(string loggedInUserId)
        {
            return db.Books.Where(b => b.Owner.Id != loggedInUserId && b.BookStatus == BookStatuses.Public).ToList();
        }


        //Returns All books owned by the logged in user
        public IEnumerable<Book> MyBooks(string loggedInUserId)
        {
            return db.Books.Where(b => b.Owner.Id == loggedInUserId).ToList();

        }


        //Returns All books that the user currently holds
        public IEnumerable<Book> MyHand(string loggedInUserId)
        {
            // books currently borrowed by the logged in user
            var borrowedBooks =
                db.BookCirculations.Where(c => c.CirculationStatus == CirculationStatuses.Borrowed && c.Borrower.Id == loggedInUserId)
                .Select(c => c.BookAssociated);

            // my books not currently borrowed by anyone
            var ownedBooksNotCurrentlyBorrowed =
                db.Books.Where(b => b.Owner.Id == loggedInUserId).Except(
                    db.BookCirculations.Where(c => c.Owner.Id == loggedInUserId && c.CirculationStatus == CirculationStatuses.Borrowed)
                    .Select(c => c.BookAssociated));

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
    }
}