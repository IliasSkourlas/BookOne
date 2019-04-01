﻿using BookOne.BookOne_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookOne.Controllers
{
    public class HomeController : Controller
    {
        DatabaseOperations dbOps = new DatabaseOperations();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Welcome, we are aiming on rediscovering the world of your books.";
                //"This is an app for searing your books with your friends. Not digital books, but your physical books. " +
                // "It helps you to keep track with their whereabouts, as they are seared from a pool of carriers that you choose. " +
                // "Read and write reviews and decide to give or not a Clap, when they are returned back to you in perfect condition. " +
                // "So...explore and have fun...and don't hesitate searing!";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Uncertified Play Team One";

            return View();
        }



        [HttpPost]
        public ActionResult EmailNotifier(string userEmail)
        {
            var notification = new EmailNotification()
            {
                EmailAddress = userEmail
            };

            return RedirectToAction("EmailIsValidToBeNotified", notification);
        }

        public ActionResult EmailIsValidToBeNotified(EmailNotification notification)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Success = "Invalid Email Address provided.";
                return View("Contact");
            }

            dbOps.InsertEmailNotification(notification);

            ViewBag.Success = "You'll be notified soon..";
            return View("Contact");
        }
    }
}