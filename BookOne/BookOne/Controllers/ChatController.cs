using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookOne.Models;
using BookOne.BookOne_Domain;
using Microsoft.AspNet.Identity;

namespace BookOne.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        DatabaseOperations dbOps = new DatabaseOperations();


        //public ActionResult Index()
        //{
        //    var loggedInUserId = User.Identity.GetUserId();
        //    var otherUsers = dbOps.GetAllOtherUsers(User.Identity.GetUserId());
            

        //    return View();
        //}


        public ActionResult ConversationWithContact(ApplicationUser contact)
        {
            var loggedInUserId = User.Identity.GetUserId();

            var conversation = dbOps.GetConversation(loggedInUserId, contact.Id);

            return View(conversation);
        }


        [HttpPost]
        public ActionResult SendMessage(ApplicationUser contact, string content)
        {
            var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());
            contact = dbOps.GetUser(User.Identity.GetUserId());

            while (contact != loggedInUser)
            {
                Message message = new Message
                {
                    Sender = loggedInUser,
                    Content = content,
                    Receiver = contact
                };

                dbOps.InsertMessage(message);

                return View(message);
            };

            return ViewBag.Message("Please select a user to chat with");
        }
    }
}