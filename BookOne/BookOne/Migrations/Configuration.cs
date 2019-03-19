namespace BookOne.Migrations
{
    using BookOne.BookOne_Domain;
    using BookOne.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
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

            if (!context.Roles.Any(r => r.Name == "Visitor"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Id = "0", Name = "Visitor" };

                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "User"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Id = "1", Name = "User" };

                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "Player"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Id = "2", Name = "Player" };

                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "Administrator"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Id = "3", Name = "Administrator" };

                manager.Create(role);
            }


            var user = context.Users.Where(u => u.Email == "moufauser@bookone.com").FirstOrDefault();
            Club firstClub = new Club();

            if (!context.Clubs.Any(r => r.ClubName == "Clubara"))
            {
                firstClub.ClubName = "Clubara";
                firstClub.ClubLocation = 10437;
                firstClub.ClubDescription = "First Club Ever";
                context.Clubs.AddOrUpdate(firstClub);
            }

            //if (!context.ClubMembers.Any(m => m.AssociatedClub.ClubId == firstClub.ClubId && m.Member.Id == user.Id))
            //{
            //    ClubMember member = new ClubMember();
            //    member.AssociatedClub = firstClub;
            //    member.Member = user;
            //    member.MemberIsConnector = false;
            //    context.ClubMembers.AddOrUpdate(member);
            //}

            if (!context.Books.Any(r => r.Title == "The Running Man"))
            {
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
}
