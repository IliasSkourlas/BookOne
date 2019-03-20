using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookOne.Controllers
{
    [Authorize]
    public class PlayerController : Controller
    {
        // GET: Player
        public ActionResult Index()
        {
            return View();
        }


        //User has to complete a form with additional information needed to begin exchanging books
        public ActionResult ChangeRole_UserToPlayer()
        {
            var loggedInUserId = User.Identity.GetUserId();

            return View();
        }


        // GET: Requests
        public ActionResult Requests()
        {
            var loggedInUserId = User.Identity.GetUserId();

            return View();
        }


        public ActionResult RequestConfirmation()
        {
            var loggedInUserId = User.Identity.GetUserId();

            return View();
        }
    }
}