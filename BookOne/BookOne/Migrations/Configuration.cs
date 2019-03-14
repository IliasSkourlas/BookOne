namespace BookOne.Migrations
{
    using BookOne.BookOne_Domain;
    using BookOne.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BookOne.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BookOne.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            

            //ApplicationUser firstUser = new ApplicationUser();
            //firstUser.Email = "sotiros@meh.com";
            //firstUser.Id = "meh";
            //firstUser.PasswordHash = "ksdjfh";



            var user = context.Users.Where(u => u.Email == "moufauser@bookone.com").FirstOrDefault();

            Club firstClub = new Club();
            firstClub.ClubName = "Clubara";
            firstClub.ClubLocation = 10437;
            firstClub.ClubDescription = "First Club Ever";
            context.Clubs.AddOrUpdate(firstClub);
            

            Book firstBook = new Book();
            firstBook.Title = "The Running Man";
            firstBook.Author = "Stephen King";
            firstBook.BookStatus = BookStatuses.Public;
            firstBook.AssociatedClub = firstClub;
            firstBook.Owner = user;
            context.Books.AddOrUpdate(firstBook);
            
        }
    }
}
