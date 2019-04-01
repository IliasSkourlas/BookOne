using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using BookOne.BookOne_Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookOne.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            RegisteredOn = DateTime.Now;
            UserStatus = UserStatuses.Active;
            ActualUsername = "...";
        }

        //By default, MVC's username is the user's email address. So we have to create a custom username field.
        public string ActualUsername { get; set; }

        [Required]
        public int PostalCode { get; set; }

        public DateTime RegisteredOn { get; set; }
        
        public UserStatuses UserStatus { get; set; }

        public int XP_Points { get; set; }



        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<ClubInitiation> ClubInitiations { get; set; }
        public DbSet<ClubLimitation> ClubLimitations { get; set; }
        public DbSet<ClubMember> ClubMembers { get; set; }
        public DbSet<BookCirculation> BookCirculations { get; set; }
        public DbSet<BookRequest> BookRequests { get; set; }
        public DbSet<BookNote> BookNotes { get; set; }
        public DbSet<UserReaction> UserReactions { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }
    }


    public enum UserStatuses
    {
        Active,
        Deleted,
        SadFaced,
        Bombed
    }
}