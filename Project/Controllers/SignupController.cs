using Project.Core.Interfaces;
using Project.Data;
using Project.Entity;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project.Controllers
{
    public class SignupController : Controller
    {
        IUserService userService;
        ICustomerService custService;

        public SignupController(IUserService userService, ICustomerService custService)
        {
            this.userService = userService;
            this.custService = custService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                string email = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                User user = userService.Get(email);
                return RedirectToAction("Index", user.Role);
            }
            else return View(new SignupModel());
        }

        [HttpPost]
        public ActionResult Index(SignupModel signupModel)
        {
            if (ModelState.IsValid)
            {
                User user = userService.Get(signupModel.Email);

                if (user == null)
                {
                    user = new User();
                    user.Password = signupModel.Password;
                    user.Email = signupModel.Email;
                    user.Role = "Customer";
                    user.Id = userService.GetAll().Count() + 1;
                    userService.Insert(user);
                    //repo.dbContext.Dispose();

                    Customer cust = new Customer();
                    cust.Id = user.Id;
                    cust.Name = signupModel.Name;
                    cust.Password = signupModel.Password;
                    cust.DateOfBirth = signupModel.DateofBirth;
                    cust.Gender = signupModel.Gender;
                    cust.Email = signupModel.Email;
                    cust.LastOnline = DateTime.Now;
                    custService.Insert(cust);

                    Session["loggedInUser"] = cust;
                    return RedirectToAction("Index", "Customer");
                }
                else ModelState.AddModelError("UserExists", "A user with this e-mail address already exists!");
            }
            return View(signupModel);
        }

    }
}
