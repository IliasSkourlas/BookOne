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
            //if (Session["user"] == null)
            //{
            //    return Redirect("/");
            //}

            ViewBag.allUsers = dbOps.GetAllOtherUsers(User.Identity.GetUserId());
            ViewBag.currentUser = dbOps.GetUser(User.Identity.GetUserId());

            return View("ChatStart");
        }

        public JsonResult ConversationWithContact(ApplicationUser contact)
        {
            if (Session["user"] == null)
            {
                return Json(new { status = "error", message = "User is not logged in" });
            }

            var currentUser = (ApplicationUser)Session["user"];

            var conversations = new List<Message>();

            using (var db = new ApplicationDbContext())
            {
                conversations = db.Messages.
                                  Where(c => (c.Receiver == currentUser.Claims
                                      && c.Sender == contact) ||
                                      (c.Receiver == contact
                                      && c.Sender == currentUser.Claims))
                                  .OrderBy(c => c.SentOn)
                                  .ToList();
            }

            return Json(
                new { status = "success", data = conversations },
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpPost]
        public JsonResult SendMessage(ApplicationUser contact)
        {
            if (Session["user"] == null)
            {
                return Json(new { status = "error", message = "User is not logged in" });
            }

            var currentUser = (ApplicationUser)Session["user"];

            string socket_id = Request.Form["socket_id"];

            while (contact != currentUser)
            {
                Message message = new Message
                {
                    Sender = currentUser,
                    Content = Request.Form["message"],
                    Receiver = contact
                };

                using (var db = new ApplicationDbContext())
                {
                    db.Messages.Add(message);
                    db.SaveChanges();
                }

                return Json(message);
            };

            return null;
        }
    }
}