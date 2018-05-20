using Project.Entity;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Data;
using Project.Core.Interfaces;
using System.Web.Security;

namespace Project.Controllers
{
    public class LoginController : Controller
    {
        IUserService userService;
        ICustomerService custService;
        IRestaurantService restService;
        ITransporterService transporterService;
        //
        // GET: /Login/
        public LoginController(IUserService userService,ICustomerService custService,IRestaurantService restService,ITransporterService transporterService)
        {
            this.userService = userService;
            this.custService = custService;
            this.restService = restService;
            this.transporterService = transporterService;
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
            else return View(new LoginModel());
        }
        [HttpPost]
        public ActionResult Index(LoginModel loginModel,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                {
                    User user = userService.Get(loginModel.Email);
                    if (user != null)
                    {
                        if (user.Password == loginModel.Password)
                        {
                            if (user.Role == "Customer")
                            {
                                Customer cust = custService.Get(user.Id);
                                cust.LastOnline = DateTime.Now;
                                custService.Update(cust, cust.Id);
                                Session["loggedInUser"] = cust;
                                //return RedirectToAction("Index", "Customer");
                            }
                            else if (user.Role == "Restaurant")
                            {
                                Restaurant rest = restService.Get(user.Id);
                                rest.LastOnline = DateTime.Now;
                                restService.Update(rest, rest.Id);
                                Session["loggedInUser"] = rest;
                            }
                            else if (user.Role == "Admin")
                            {
                                AdminRepository custRepo = new AdminRepository();
                                Admin admin = custRepo.Get(user.Id);
                                admin.LastOnline = DateTime.Now;
                                custRepo.Update(admin, admin.Id);
                                Session["loggedInUser"] = admin;
                            }
                            else if (user.Role == "Transporter")
                            {
                                Transporter trn = transporterService.Get(user.Id);
                                trn.LastOnline = DateTime.Now;
                                transporterService.Update(trn, trn.Id);
                                Session["loggedInUser"] = trn;
                            }

                            FormsAuthentication.SetAuthCookie(loginModel.Email, false);
                            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", user.Role);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Password", "Wrong password.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "No such user found.");
                    }
                }
            }
            return View(loginModel);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View(new LoginModel());
        }
        //[HttpPost]
        //public ActionResult ForgotPassword(string email)
        //{
        //    List<User> users = (List<User>)Session["users"];
        //    foreach (User user in users)
        //    {
        //        if (user.Email == email)
        //        {
        //            ViewBag.Password = user.Password;
        //            return View();
        //        }
        //    }
        //    ModelState.AddModelError("", "No such user found.");
        //    return View();
        //}
    }
}
