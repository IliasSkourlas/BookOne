using Microsoft.Owin.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Net;

namespace BookOne.Controllers
{
    public class ErrorController : Controller
    {
        protected override void OnException(ExceptionContext ex)
        {
            ex.ExceptionHandled = true;

            if (ex.Exception is SqlException)
            {
                ex.Result = ViewBag("An error occurred while communicating with the database.");
            }

            else if (ex.Exception is WebException)
            {
                ex.Result = ViewBag("A network error occurred.");
            }

            else
            {
                ex.Result = ViewBag("An error occurred.");
            }

            ex.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
        }
    }


}