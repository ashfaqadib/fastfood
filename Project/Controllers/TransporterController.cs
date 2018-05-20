using Project.Core;
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
    [Authorize(Roles = "Transporter")]
    public class TransporterController : Controller
    {
        IUserService userService;
        IRestaurantService restService;
        IInvoiceService invService;
        IOrderService orderService;
        IRestaurantAddressService restAddService;
        ITransporterService transporterService;

        public TransporterController(IRestaurantService restService, IInvoiceService invService, IOrderService orderService, IRestaurantAddressService restAddService, ITransporterService transporterService, IUserService userService)
        {
            this.restAddService = restAddService;
            this.restService = restService;
            this.invService = invService;
            this.orderService = orderService;
            this.transporterService = transporterService;
            this.userService = userService;
        }

        public double toRadians(double degree)
        {
            return (Math.PI / 180) * degree;
        }

        [HttpGet]
        public ActionResult HomeSuggestions()
        {
            LocationModel locationModel = new LocationModel();
            List<RestaurantAddress> restaurantAddresses = restAddService.GetAll() as List<RestaurantAddress>;
            List<Restaurant> restaurants = restService.GetAll() as List<Restaurant>;
            List<Restaurant> selectedRestaurants = new List<Restaurant>();
            List<Invoice> selectedInvoices = new List<Invoice>();

            locationModel.Latitude = System.Convert.ToDouble(Request.QueryString["Latitude"]);
            locationModel.Longitude = System.Convert.ToDouble(Request.QueryString["Longitude"]);
            int radius = System.Convert.ToInt32(Request.QueryString["Radius"]);
            foreach (RestaurantAddress restaurantAddress in restaurantAddresses)
            {
                if ((Math.Acos(Math.Sin(toRadians(restaurantAddress.Latitude)) * Math.Sin(toRadians(locationModel.Latitude)) + Math.Cos(toRadians(restaurantAddress.Latitude))
                 * Math.Cos(toRadians(locationModel.Latitude)) * Math.Cos(toRadians(restaurantAddress.Longitude) - toRadians(locationModel.Longitude)))) * 6380 < radius)
                {
                    selectedRestaurants.Add(restService.Get(restaurantAddress.RestaurantId));
                }
            }
            foreach (Restaurant rest in selectedRestaurants)
            {
                foreach (Invoice inv in invService.GetAllByRestaurantId(rest.Id))
                {
                    if(inv.Status=="Received") selectedInvoices.Add(inv);
                }
            }
            return Json(PrepareInvoices(selectedInvoices), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetInvoices()
        {
            Restaurant thisUser = (Restaurant)Session["loggedInUser"];

            List<Invoice> invs = invService.GetAll().Where(inv => inv.RestaurantId == thisUser.Id).ToList();

            return Json(PrepareInvoices(invs), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeliverOrder(int invId)
        {
            Transporter thisUser = (Transporter)Session["loggedInUser"];
            return Json(transporterService.DeliverOrder(thisUser.Id, invId, "On The Way"),JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetTasks()
        {
            Transporter thisUser = (Transporter)Session["loggedInUser"];
            List<TransporterTaskModel> tasks = new List<TransporterTaskModel>();
            foreach (TransporterTask task in transporterService.GetTasks(thisUser.Id).Where(t=>t.Status=="On The Way"))
            {
                Invoice inv = invService.Get(task.InvoiceId);
                TransporterTaskModel taskModel = new TransporterTaskModel();
                taskModel.Id = task.Id;
                taskModel.CustomerName = inv.Recipient;
                taskModel.RestaurantName = inv.RestaurantName;
                taskModel.DeliveryAddress = inv.DeliveryAddress;
                taskModel.InvoiceNo = inv.Id;
                taskModel.CustomerContactNo = inv.RecipientContactNo;
                taskModel.StartingTime = task.StartTime.ToString("MM/dd/yyyy hh:mm tt");
                taskModel.EndTime = task.EndTime.ToString();
                taskModel.Status = task.Status;
                tasks.Add(taskModel);
            }
            return Json(tasks, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void CancelTask(int invId)
        {
            Transporter thisUser = (Transporter)Session["loggedInUser"];
            transporterService.CancelTask(thisUser.Id, invId, "Received");
        }

        [HttpGet]
        public ActionResult ConfirmDelivery(int invId, int token)
        {
            Transporter thisUser = (Transporter)Session["loggedInUser"];
            return Json(transporterService.ConfirmDelivery(thisUser.Id, invId, token), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetInvoiceDetails(int invId)
        {
            Transporter thisUser = (Transporter)Session["loggedInUser"];

            return Json(PrepareInvoiceDetails(invId), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult TaskHistory()
        {
            Transporter thisUser = (Transporter)Session["loggedInUser"];

            List<TransporterTaskModel> tasks = new List<TransporterTaskModel>();
            foreach (TransporterTask task in transporterService.GetTasks(thisUser.Id))
            {
                Invoice inv = invService.Get(task.InvoiceId);
                TransporterTaskModel taskModel = new TransporterTaskModel();
                taskModel.Id = task.Id;
                taskModel.CustomerName = inv.Recipient;
                taskModel.RestaurantName = inv.RestaurantName;
                taskModel.DeliveryAddress = inv.DeliveryAddress;
                taskModel.InvoiceNo = inv.Id;
                taskModel.CustomerContactNo = inv.RecipientContactNo;
                taskModel.StartingTime = task.StartTime.ToString("MM/dd/yyyy hh:mm tt");
                taskModel.EndTime = task.EndTime.ToString();
                taskModel.Status = task.Status;
                tasks.Add(taskModel);
            }

            return View(tasks);

        }

        public List<InvoiceModel> PrepareInvoices(List<Invoice> invoices)
        {
            Repository<Customer> repoUser = new Repository<Customer>();
            List<InvoiceModel> invModels = new List<InvoiceModel>();
            InvoiceModel invMod;

            foreach (Invoice inv in invoices)
            {
                invMod = new InvoiceModel();
                invMod.InvoiceNo = inv.Id;
                invMod.OrderOwner = repoUser.Get(inv.CustomerId).Name;
                invMod.OrderTime = inv.CheckOutTime.ToString("dd/MM/yyyy hh:mm tt");
                invMod.Bill = inv.Bill;
                invMod.RestaurantName = inv.RestaurantName;
                invMod.Status = inv.Status;
                invModels.Add(invMod);
            }

            return invModels;
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["loggedInUser"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Public");
        }

        public CartModel PrepareInvoiceDetails(int invId)
        {
            double bill = 0;
            Repository<Item> items = new Repository<Item>();
            CartModel cart = new CartModel();
            Item item = new Item();

            foreach (Order ord in orderService.GetOrdersByInvoiceId(invId))
            {
                item = new Item();
                item = items.Get(ord.ItemId);
                cart.Item.Add(item);
                cart.Quantity.Add(ord.Quantity);
                cart.Price.Add(ord.Total);
                bill += ord.Total;
            }

            Invoice thisInv = new Repository<Invoice>().Get(invId);

            cart.RestaurantName = restService.GetByInvoice(thisInv).Name;
            cart.Total = bill.ToString();
            cart.Bill = thisInv.Bill.ToString();
            cart.VATinPercentage = thisInv.VATinPercentage.ToString();
            cart.DiscountinPercentage = thisInv.DiscountinPercentage.ToString();
            cart.DeliveryCharge = thisInv.DeliveryCharge.ToString();
            ViewBag.GrandTotal = (thisInv.Bill - thisInv.VAT).ToString();
            cart.Discount = thisInv.Discount.ToString();
            cart.VAT = thisInv.VAT.ToString();
            cart.InvoiceNo = invId.ToString();

            cart.RestaurantName = thisInv.RestaurantName;
            cart.RestaurantAddress = restService.GetRestaurantAddress(thisInv.RestaurantId).FormattedAddress;
            cart.Recipient = thisInv.Recipient;
            cart.RecipientContactNo = thisInv.RecipientContactNo;
            cart.DeliveryAddress = thisInv.DeliveryAddress;

            return (cart);
        }

        [HttpGet]
        public ActionResult Account()
        {
            return View(Session["loggedInUser"]);
        }

        [HttpGet]
        public ActionResult EditAccount()
        {
            Transporter thisUser = (Transporter)Session["loggedInUser"];
            ViewBag.dobDay = Convert.ToInt32(thisUser.DateOfBirth.Split('/')[0]);
            ViewBag.dobMonth = Convert.ToInt32(thisUser.DateOfBirth.Split('/')[1]);
            ViewBag.dobYear = Convert.ToInt32(thisUser.DateOfBirth.Split('/')[2]);
            return View(Session["loggedInUser"]);
        }

        [HttpPost]
        public ActionResult EditAccount(EditProfileModel editProfileModel)
        {
            Transporter thisUser = (Transporter)Session["loggedInUser"];
            if (ModelState.IsValid)
            {
                Transporter user = new Transporter();
                user.Name = editProfileModel.Name;
                user.DateOfBirth = editProfileModel.DateofBirth;
                user.Gender = editProfileModel.Gender;
                user.Password = thisUser.Password;
                user.Id = thisUser.Id;
                user.Email = thisUser.Email;
                user.Status = thisUser.Status;
                user.Points = thisUser.Points;
                user.LastOnline = DateTime.Now;
                transporterService.Update(user, user.Id);
                Session["loggedInUser"] = user;

                return RedirectToAction("Index", "Transporter");
            }
            ViewBag.dobDay = Convert.ToInt32(thisUser.DateOfBirth.Split('/')[0]);
            ViewBag.dobMonth = Convert.ToInt32(thisUser.DateOfBirth.Split('/')[1]);
            ViewBag.dobYear = Convert.ToInt32(thisUser.DateOfBirth.Split('/')[2]);
            return View(editProfileModel);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View(Session["loggedInUser"]);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (ModelState.IsValid)
            {
                Transporter transporter = (Transporter)Session["loggedInUser"];
                Transporter thisUser = transporterService.Get(transporter.Id);
                User user = new User();
                user = userService.Get(thisUser.Email);
                if (thisUser.Password == changePasswordModel.CurrentPassword)
                {
                    thisUser.Password = changePasswordModel.NewPassword;
                    user.Password = changePasswordModel.NewPassword;
                    transporterService.Update(thisUser, thisUser.Id);
                    userService.Update(user,user.Email);
                    ViewBag.success = "Password changed successfully";
                }
                else
                {
                    ModelState.AddModelError("Password", "Wrong password.");
                }
            }
            return View();
        }
    }
}
