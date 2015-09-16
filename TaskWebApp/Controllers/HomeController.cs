using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskWebApp.Policies;

namespace TaskWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [PolicyAuthorize(Policy = "{Enter the name of your sign in policy, e.g. b2c_1_my_sign_in}")]
        public ActionResult Claims()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Error(string message)
        {
            ViewBag.Message = message;

            return View("Error");
        }
    }
}