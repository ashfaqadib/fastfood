using Project.Core;
using Project.Core.Interfaces;
using Project.Data;
using Project.Entity;
using Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project.Controllers
{
    [Authorize(Roles = "Restaurant")]
    public class RestaurantController : Controller
    {
        //
        // GET: /Restaurant/
        IRestaurantService restService;
        IInvoiceService invService;
        IOrderService ordService;

        public RestaurantController(IRestaurantService restService, IInvoiceService invService, IOrderService ordService)
        {
            this.restService = restService;
            this.invService = invService;
            this.ordService = ordService;
        }
        [HttpGet]
        public ActionResult Index()
        {
            Restaurant thisUser = (Restaurant)Session["loggedInUser"];

            List<Invoice> invs = invService.GetAll().Where(inv => inv.RestaurantId == thisUser.Id).ToList();

            return View(PrepareInvoices(thisUser.Id, invs));
        }
        [HttpGet]
        public ActionResult GetInvoices()
        {
            Restaurant thisUser = (Restaurant)Session["loggedInUser"];

            List<Invoice> invs = invService.GetAll().Where(inv => (inv.RestaurantId == thisUser.Id) && (inv.Status == "Pending")).ToList();

            return Json(PrepareInvoices(thisUser.Id, invs),JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetInvoiceDetails(int invId)
        {
            return Json(PrepareInvoiceDetails(invId), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Account()
        {
            return View(Session["loggedInUser"]);
        }

        [HttpGet]
        public ActionResult Start(int region)
        {
            IEnumerable<Restaurant> restaurants = (IEnumerable<Restaurant>)Session["restaurants"];
            //restaurants = restaurants.Where(restaurant => restaurant.Region == region);
            return View(restaurants);
        }

        [HttpGet]
        public ActionResult Menu(int id)
        {
            Restaurant thisUser = (Restaurant)Session["loggedInUser"];
            List<Item> items = (List<Item>)Session["items"];
            items = items.Where(item => item.Id == id).ToList();
            List<List<Item>> selectedItems = new List<List<Item>>();
            foreach (var groups in items.GroupBy(group => group.Type))
            {
                selectedItems.Add(groups.ToList());
            }
            return View(selectedItems);
        }


        [HttpGet]
        public ActionResult OrderHistory()
        {
            Restaurant thisUser = (Restaurant)Session["loggedInUser"];

            List<Invoice> invs = invService.GetAll().Where(inv => (inv.RestaurantId == thisUser.Id)).ToList();

            return View(PrepareInvoices(thisUser.Id,invs));
        }


        [HttpGet]
        public ActionResult EditAccount()
        {
            Restaurant thisUser = (Restaurant)Session["loggedInUser"];
            return View(Session["loggedInUser"]);
        }
        [HttpPost]
        public ActionResult EditAccount(Restaurant editedRest, HttpPostedFileBase image)
        {
            Restaurant thisUser = (Restaurant)Session["loggedInUser"];
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string inputFileName = "image/restaurants/" + thisUser.Id.ToString() + Path.GetExtension(image.FileName).ToString();
                    //restService.UpdateLogo(thisUser.Id,inputFileName);
                    var serverSavePath = Path.Combine(Server.MapPath("~/") + inputFileName);
                    //Save file to server folder  
                    image.SaveAs(serverSavePath);
                    editedRest.ImageLocation = inputFileName;
                    //assigning file uploaded status to ViewBag for showing message to user.  
                }
                else editedRest.ImageLocation = thisUser.ImageLocation;

                editedRest.Id = thisUser.Id;
                editedRest.Password = thisUser.Password;
                editedRest.LastOnline = DateTime.Now;
                editedRest.AddressId = thisUser.AddressId;
                editedRest.Rating = thisUser.Rating;
                restService.Update(editedRest, thisUser.Id);
                Session["loggedInUser"] = restService.Get(thisUser.Id);
                return RedirectToAction("Index", "Restaurant");
            }
            return View(editedRest);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View(Session["loggedInUser"]);
        }
/*
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            User thisUser = (User)Session["loggedInUser"];
            if (ModelState.IsValid)
            {
                if (changePasswordModel.CurrentPassword == thisUser.Password)
                {
                    List<User> users = (List<User>)Session["users"];
                    foreach (User user in users)
                    {
                        if (user.Email == thisUser.Email)
                        {
                            user.Password = changePasswordModel.NewPassword;
                            Session["loggedInUser"] = user;
                            return RedirectToAction("Index", thisUser.Role);
                        }
                    }
                }
                ModelState.AddModelError("Password", "Wrong password.");
            }
            return View(Session["loggedInUser"]);
        }
*/
        [HttpGet]
        public ActionResult AddItem()
        {
            return View(new AddItemModel());
        }
        [HttpGet]
        public ActionResult TrackOrder(int invId)
        {
            return Json(invService.Get(invId).Status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddItem(AddItemModel addItemModel)
        {
            Restaurant thisRestaurant = (Restaurant)Session["loggedInUser"];
            if (ModelState.IsValid)
            {
                addItemModel.RestaurantId = thisRestaurant.Id;
                List<Item> items = (List<Item>)Session["items"];

                Item item = new Item();
                item.Name = addItemModel.Name;
                item.RestaurantId = thisRestaurant.Id;
                item.Proportion = addItemModel.Proportion;
                item.Price = addItemModel.Price;
                item.Type = addItemModel.Type;
                item.Description = addItemModel.Description;

                new Repository<Item>().Insert(item);
                return RedirectToAction("Index", "Restaurant");
            }
            return View(addItemModel);
        }

        [HttpGet]
        public ActionResult ManageItems()
        {
            Restaurant thisRestaurant = (Restaurant)Session["loggedInUser"];
            List<Item> items = new Repository<Item>().GetAll() as List<Item>;
            items = items.Where(item => item.RestaurantId == thisRestaurant.Id).ToList();
            //ViewBag.items = items;
            ViewBag.items = items;
            return View();
        }

        [HttpGet]
        public ActionResult InvoiceDetails(int id)
        {
            Restaurant thisRestaurant = (Restaurant)Session["loggedInUser"];
            return View(PrepareInvoiceDetails(id));
        }
/*
        [HttpGet]
        public ActionResult RemoveItem(int id)
        {
            User thisRestaurant = (User)Session["loggedInUser"];
            List<Item> items = (List<Item>)Session["items"];
            Item deleteItem = items.Where(item => item.Id == id).SingleOrDefault();
            items.Remove(deleteItem);
            return RedirectToAction("ManageItems", thisRestaurant.Role);
        }

        [HttpGet]
        public ActionResult EditRestaurantAccount()
        {
            return View();
        }
*/
        [HttpGet]
        public ActionResult EditRestaurantAddress()
        {
            return View("Temp");
        }
        [HttpPost]
        public ActionResult EditRestaurantAddress(RestaurantAddress address)
        {
            Restaurant thisRestaurant = (Restaurant)Session["loggedInUser"];
            restService.UpdateAddress(thisRestaurant.Id, address);
            return RedirectToAction("Index", "Restaurant");
        }

        [HttpGet]
        public ActionResult ChangeOrderStatus(int invId,string status)
        {
            invService.ChangeStatus(invId, status);

            Restaurant thisUser = (Restaurant)Session["loggedInUser"];

            List<Invoice> invs = invService.GetAll().Where(inv => (inv.RestaurantId == thisUser.Id) && (inv.Status == "Pending")).ToList();

            return Json(PrepareInvoices(thisUser.Id, invs), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["loggedInUser"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Public");
        }

        public List<InvoiceModel> PrepareInvoices(int restaurantId,List<Invoice> invoices)
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
                invMod.Status = inv.Status;
                invModels.Add(invMod);
            }

            return invModels;
        }

        public CartModel PrepareInvoiceDetails(int invId)
        {
            double bill = 0;
            Repository<Item> items = new Repository<Item>();
            CartModel cart = new CartModel();
            Item item = new Item();

            foreach (Order ord in ordService.GetOrdersByInvoiceId(invId))
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
    }
}
