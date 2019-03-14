namespace BookOne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookCirculations",
                c => new
                    {
                        BookCirculationId = c.Int(nullable: false, identity: true),
                        BorrowedOn = c.DateTime(nullable: false),
                        BorrowedForXWeeks = c.Int(nullable: false),
                        OwnerGaveBook = c.Boolean(nullable: false),
                        BorrowerReceivedBook = c.Boolean(nullable: false),
                        CirculationStatus = c.Int(nullable: false),
                        BookAssociated_BookId = c.Int(),
                        Borrower_Id = c.String(maxLength: 128),
                        Owner_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.BookCirculationId)
                .ForeignKey("dbo.Books", t => t.BookAssociated_BookId)
                .ForeignKey("dbo.AspNetUsers", t => t.Borrower_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Owner_Id)
                .Index(t => t.BookAssociated_BookId)
                .Index(t => t.Borrower_Id)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        BookId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Author = c.String(nullable: false),
                        RegisteredOn = c.DateTime(nullable: false),
                        BookStatus = c.Int(nullable: false),
                        AssociatedClub_ClubId = c.Int(),
                        Owner_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.BookId)
                .ForeignKey("dbo.Clubs", t => t.AssociatedClub_ClubId)
                .ForeignKey("dbo.AspNetUsers", t => t.Owner_Id)
                .Index(t => t.AssociatedClub_ClubId)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.Clubs",
                c => new
                    {
                        ClubId = c.Int(nullable: false, identity: true),
                        ClubName = c.String(nullable: false),
                        ClubDescription = c.String(),
                        ClubLocation = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClubId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PostalCode = c.Int(nullable: false),
                        RegisteredOn = c.DateTime(nullable: false),
                        UserStatus = c.Int(nullable: false),
                        XP_Points = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.BookNotes",
                c => new
                    {
                        BookNoteId = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false),
                        ReceivedOn = c.DateTime(nullable: false),
                        BookAssociated_BookId = c.Int(),
                        Giver_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.BookNoteId)
                .ForeignKey("dbo.Books", t => t.BookAssociated_BookId)
                .ForeignKey("dbo.AspNetUsers", t => t.Giver_Id)
                .Index(t => t.BookAssociated_BookId)
                .Index(t => t.Giver_Id);
            
            CreateTable(
                "dbo.BookRequests",
                c => new
                    {
                        BookRequestId = c.Int(nullable: false, identity: true),
                        RequestedOn = c.DateTime(nullable: false),
                        ApprovedByOwner = c.Boolean(nullable: false),
                        BookOwner_Id = c.String(maxLength: 128),
                        BookRequested_BookId = c.Int(),
                        RequestedBy_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.BookRequestId)
                .ForeignKey("dbo.AspNetUsers", t => t.BookOwner_Id)
                .ForeignKey("dbo.Books", t => t.BookRequested_BookId)
                .ForeignKey("dbo.AspNetUsers", t => t.RequestedBy_Id)
                .Index(t => t.BookOwner_Id)
                .Index(t => t.BookRequested_BookId)
                .Index(t => t.RequestedBy_Id);
            
            CreateTable(
                "dbo.ClubInitiations",
                c => new
                    {
                        ClubInitiationId = c.Int(nullable: false, identity: true),
                        MemberApproval = c.Boolean(nullable: false),
                        AssociatedClub_ClubId = c.Int(),
                        InitialMember_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ClubInitiationId)
                .ForeignKey("dbo.Clubs", t => t.AssociatedClub_ClubId)
                .ForeignKey("dbo.AspNetUsers", t => t.InitialMember_Id)
                .Index(t => t.AssociatedClub_ClubId)
                .Index(t => t.InitialMember_Id);
            
            CreateTable(
                "dbo.ClubLimitations",
                c => new
                    {
                        ClubLimitationId = c.Int(nullable: false, identity: true),
                        MaxMembers = c.Int(nullable: false),
                        ClubIsOpen = c.Boolean(nullable: false),
                        AssociatedClub_ClubId = c.Int(),
                    })
                .PrimaryKey(t => t.ClubLimitationId)
                .ForeignKey("dbo.Clubs", t => t.AssociatedClub_ClubId)
                .Index(t => t.AssociatedClub_ClubId);
            
            CreateTable(
                "dbo.ClubMembers",
                c => new
                    {
                        ClubMemberId = c.Int(nullable: false, identity: true),
                        JoinedOn = c.DateTime(nullable: false),
                        MemberIsConnector = c.Boolean(nullable: false),
                        AssociatedClub_ClubId = c.Int(),
                        Member_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ClubMemberId)
                .ForeignKey("dbo.Clubs", t => t.AssociatedClub_ClubId)
                .ForeignKey("dbo.AspNetUsers", t => t.Member_Id)
                .Index(t => t.AssociatedClub_ClubId)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false),
                        SentOn = c.DateTime(nullable: false),
                        Receiver_Id = c.String(maxLength: 128),
                        Sender_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.AspNetUsers", t => t.Receiver_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Sender_Id)
                .Index(t => t.Receiver_Id)
                .Index(t => t.Sender_Id);
            
            CreateTable(
                "dbo.Reactions",
                c => new
                    {
                        ReactionId = c.String(nullable: false, maxLength: 128),
                        Choice = c.Int(nullable: false),
                        ReceivedOn = c.DateTime(nullable: false),
                        ActionGiver_Id = c.String(maxLength: 128),
                        ActionReceiver_Id = c.String(maxLength: 128),
                        ForBook_BookId = c.Int(),
                    })
                .PrimaryKey(t => t.ReactionId)
                .ForeignKey("dbo.AspNetUsers", t => t.ActionGiver_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ActionReceiver_Id)
                .ForeignKey("dbo.Books", t => t.ForBook_BookId)
                .Index(t => t.ActionGiver_Id)
                .Index(t => t.ActionReceiver_Id)
                .Index(t => t.ForBook_BookId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Reactions", "ForBook_BookId", "dbo.Books");
            DropForeignKey("dbo.Reactions", "ActionReceiver_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reactions", "ActionGiver_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "Sender_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "Receiver_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClubMembers", "Member_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClubMembers", "AssociatedClub_ClubId", "dbo.Clubs");
            DropForeignKey("dbo.ClubLimitations", "AssociatedClub_ClubId", "dbo.Clubs");
            DropForeignKey("dbo.ClubInitiations", "InitialMember_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClubInitiations", "AssociatedClub_ClubId", "dbo.Clubs");
            DropForeignKey("dbo.BookRequests", "RequestedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.BookRequests", "BookRequested_BookId", "dbo.Books");
            DropForeignKey("dbo.BookRequests", "BookOwner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.BookNotes", "Giver_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.BookNotes", "BookAssociated_BookId", "dbo.Books");
            DropForeignKey("dbo.BookCirculations", "Owner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.BookCirculations", "Borrower_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.BookCirculations", "BookAssociated_BookId", "dbo.Books");
            DropForeignKey("dbo.Books", "Owner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Books", "AssociatedClub_ClubId", "dbo.Clubs");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Reactions", new[] { "ForBook_BookId" });
            DropIndex("dbo.Reactions", new[] { "ActionReceiver_Id" });
            DropIndex("dbo.Reactions", new[] { "ActionGiver_Id" });
            DropIndex("dbo.Messages", new[] { "Sender_Id" });
            DropIndex("dbo.Messages", new[] { "Receiver_Id" });
            DropIndex("dbo.ClubMembers", new[] { "Member_Id" });
            DropIndex("dbo.ClubMembers", new[] { "AssociatedClub_ClubId" });
            DropIndex("dbo.ClubLimitations", new[] { "AssociatedClub_ClubId" });
            DropIndex("dbo.ClubInitiations", new[] { "InitialMember_Id" });
            DropIndex("dbo.ClubInitiations", new[] { "AssociatedClub_ClubId" });
            DropIndex("dbo.BookRequests", new[] { "RequestedBy_Id" });
            DropIndex("dbo.BookRequests", new[] { "BookRequested_BookId" });
            DropIndex("dbo.BookRequests", new[] { "BookOwner_Id" });
            DropIndex("dbo.BookNotes", new[] { "Giver_Id" });
            DropIndex("dbo.BookNotes", new[] { "BookAssociated_BookId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Books", new[] { "Owner_Id" });
            DropIndex("dbo.Books", new[] { "AssociatedClub_ClubId" });
            DropIndex("dbo.BookCirculations", new[] { "Owner_Id" });
            DropIndex("dbo.BookCirculations", new[] { "Borrower_Id" });
            DropIndex("dbo.BookCirculations", new[] { "BookAssociated_BookId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Reactions");
            DropTable("dbo.Messages");
            DropTable("dbo.ClubMembers");
            DropTable("dbo.ClubLimitations");
            DropTable("dbo.ClubInitiations");
            DropTable("dbo.BookRequests");
            DropTable("dbo.BookNotes");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Clubs");
            DropTable("dbo.Books");
            DropTable("dbo.BookCirculations");
        }
    }
}
