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


        public ActionResult Index()
        {
            ViewBag.allUsers = dbOps.GetAllOtherUsers(User.Identity.GetUserId());
            ViewBag.currentUser = dbOps.GetUser(User.Identity.GetUserId());

            return View("ChatStart");
        }


        public JsonResult ConversationWithContact(ApplicationUser contact)
        {
            var loggedInUserId = User.Identity.GetUserId();

            var conversations = dbOps.GetConversation(loggedInUserId, contact.Id);
            
            return Json(
                new { status = "success", data = conversations },
                JsonRequestBehavior.AllowGet
            );
        }


        [HttpPost]
        public JsonResult SendMessage(ApplicationUser contact)
        {
            var loggedInUser = dbOps.GetUser(User.Identity.GetUserId());

            string socket_id = Request.Form["socket_id"];

            while (contact != loggedInUser)
            {
                Message message = new Message
                {
                    Sender = loggedInUser,
                    Content = Request.Form["message"],
                    Receiver = contact
                };

                dbOps.InsertMessage(message);

                return Json(message);
            };

            return null;
        }
    }
}