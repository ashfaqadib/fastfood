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
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        Customer thisUser;
        List<Order> Orders;

        IRestaurantAddressService restAddService;
        IRestaurantService restService;
        IOrderService orderService;
        ICustomerService custService;
        IInvoiceService invService;
        IItemService itemService;

        public CustomerController(IRestaurantService restService,IRestaurantAddressService restAddService,IOrderService ordService,IInvoiceService invService,ICustomerService custService,IItemService itemService)
        {
            this.Orders = new List<Order>();
            this.custService = custService;
            this.restService = restService;
            this.orderService = ordService;
            this.restAddService = restAddService;
            this.itemService = itemService;
            this.invService = invService;

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
            List<Restaurant> selectedRestaurants = new List<Restaurant>();

            locationModel.Latitude = System.Convert.ToDouble(Request.QueryString["Latitude"]);
            locationModel.Longitude = System.Convert.ToDouble(Request.QueryString["Longitude"]);

            Restaurant rest;
            foreach (RestaurantAddress restaurantAddress in restaurantAddresses)
            {
                if ((Math.Acos(Math.Sin(toRadians(restaurantAddress.Latitude)) * Math.Sin(toRadians(locationModel.Latitude)) + Math.Cos(toRadians(restaurantAddress.Latitude))
                 * Math.Cos(toRadians(locationModel.Latitude)) * Math.Cos(toRadians(restaurantAddress.Longitude) - toRadians(locationModel.Longitude)))) * 6380 < 10)
                {
                    rest = restService.Get(restaurantAddress.RestaurantId);
                    if (rest.Rating > 3) selectedRestaurants.Add(rest);
                }
            }
            return Json(selectedRestaurants, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Index()
        {
            string[] filePaths = Directory.GetFiles(Server.MapPath("/image/advertisement/"));
            List<string> files = new List<string>();
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);
                string src = "/image/advertisement/" + fileName;
                files.Add(src);

            }
            ViewBag.files = files;
                return View();
        }

        [HttpPost]
        public ActionResult Index(LocationModel locationModel)
        {
            //long = 90.4125181
            //lat = 23.810332
            //add = 532/4 Road No 11, Dhaka 1212, Bangladesh

            List<Restaurant> restaurants = restService.GetAll() as List<Restaurant>;
            List<RestaurantAddress> restaurantAddresses = restAddService.GetAll() as List<RestaurantAddress>;
            StartModel startModel = new StartModel();

            foreach (RestaurantAddress restaurantAddress in restaurantAddresses)
            {
                if ((Math.Acos(Math.Sin(toRadians(restaurantAddress.Latitude)) * Math.Sin(toRadians(locationModel.Latitude)) + Math.Cos(toRadians(restaurantAddress.Latitude))
                 * Math.Cos(toRadians(locationModel.Latitude)) * Math.Cos(toRadians(restaurantAddress.Longitude) - toRadians(locationModel.Longitude)))) * 6380 < 10)
                {
                    Restaurant rest = restService.Get(restaurantAddress.RestaurantId);
                    if (((DateTime.Now - rest.LastOnline).Hours) < 1) startModel.openRestaurants.Add(rest);
                    else startModel.closedRestaurants.Add(rest);
                }
            }
            return View("Start", startModel);
        }

        [HttpGet]
        public ActionResult SearchResult()
        {
            List<Restaurant> restaurants = (List<Restaurant>)Session["restaurants"];
            return View(restaurants);
        }

        [HttpGet]
        public ActionResult AutoComplete(string val)
        {
            StringComparison comp = StringComparison.OrdinalIgnoreCase;
            List<Restaurant> rests = restService.GetAll().Where(rest => rest.Name.IndexOf(val, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            return Json(rests, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApplyFilter(FormCollection filters)
        {
            StartModel startModel = new StartModel();
            foreach (Restaurant rest in restService.GetAll())
            {
                foreach (var filter in filters[0].Split(','))
                {
                    if (itemService.GetAllByRestaurantId(rest.Id).Any(it => it.Type == filter.ToString()))
                    {
                        if (((DateTime.Now - rest.LastOnline).Hours) < 1) startModel.openRestaurants.Add(rest);
                        else startModel.closedRestaurants.Add(rest);
                        break;
                    }
                }
            }
            return Json(startModel, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Start(string val)
        {
            StringComparison comp = StringComparison.OrdinalIgnoreCase;
            List<Restaurant> rests = restService.GetAll().Where(rest => rest.Name.IndexOf(val, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            StartModel startModel = new StartModel();
            foreach (Restaurant rest in rests)
            {
                if (((DateTime.Now - rest.LastOnline).Hours) < 1) startModel.openRestaurants.Add(rest);
                else startModel.closedRestaurants.Add(rest);
            }
            return View(startModel);
        }

        [HttpGet]
        public ActionResult Menu(int id)
        {
            List<Item> items = itemService.GetAllByRestaurantId(id);
            MenuModel menuModel = new MenuModel();
            Restaurant rest = restService.Get(id);
            menuModel.Items = new List<List<Item>>();
            menuModel.Orders = Session["Cart"] as List<Order>;
            menuModel.ImageLocation = rest.ImageLocation;
            menuModel.RestaurantName = rest.Name;
            menuModel.RestaurantId = rest.Id;
            menuModel.Address = restService.GetRestaurantAddress(id).FormattedAddress;
            List<List<Item>> selectedItems = new List<List<Item>>();
            foreach (var groups in items.GroupBy(group => group.Type))
            {
                menuModel.Items.Add(groups.ToList());
            }
            ViewBag.Bill = custService.CartBill(Orders).ToString();
            return View(menuModel);
        }

        [HttpGet]
        public ActionResult Account()
        {
            return View(Session["loggedInUser"]);
        }

        [HttpGet]
        public ActionResult EditAccount()
        {
            thisUser = (Customer)Session["loggedInUser"];
            ViewBag.dobDay = Convert.ToInt32(thisUser.DateOfBirth.Split('/')[0]);
            ViewBag.dobMonth = Convert.ToInt32(thisUser.DateOfBirth.Split('/')[1]);
            ViewBag.dobYear = Convert.ToInt32(thisUser.DateOfBirth.Split('/')[2]);
            return View(Session["loggedInUser"]);
        }
        [HttpPost]
        public ActionResult EditAccount(EditProfileModel editProfileModel)
        {
            thisUser = (Customer)Session["loggedInUser"];
            if (ModelState.IsValid)
            {
                Customer user = new Customer();
                user.Name = editProfileModel.Name;
                user.DateOfBirth = editProfileModel.DateofBirth;
                user.Gender = editProfileModel.Gender;
                user.Password = thisUser.Password;
                user.Id = thisUser.Id;
                user.Email = thisUser.Email;
                user.Status = thisUser.Status;
                user.DeliveryAddress = thisUser.DeliveryAddress;
                user.Points = thisUser.Points;
                user.LastOnline = DateTime.Now;
                custService.Update(user,user.Id);
                Session["loggedInUser"] = user;

                return RedirectToAction("Index", "Customer");
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

        [HttpGet]
        public ActionResult DiscardCart()
        {
            Orders = new List<Order>();
            Session["Cart"] = Orders;
            Session["CartRestaurant"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (ModelState.IsValid)
            {
                if (changePasswordModel.CurrentPassword == thisUser.Password)
                {
                    List<Customer> users = (List<Customer>)Session["users"];
                    foreach (Customer user in users)
                    {
                        if (user.Email == thisUser.Email)
                        {
                            user.Password = changePasswordModel.NewPassword;
                            Session["loggedInUser"] = user;
                            return RedirectToAction("Index", "Customer");
                        }
                    }
                }
                ModelState.AddModelError("Password", "Wrong password.");
            }
            return View(Session["loggedInUser"]);
        }

        [HttpGet]
        public ActionResult OrderHistory()
        {
            //Repository<Customer> repo = new Repository<Customer>();
            Customer thisUser = (Customer)Session["loggedInUser"];

            List<Invoice> invs = invService.GetAllByCustomerId(thisUser.Id);
            List<InvoiceModel> invModels = new List<InvoiceModel>();
            InvoiceModel invMod;

            foreach (Invoice inv in invs)
            {
                invMod = new InvoiceModel();
                invMod.InvoiceNo = inv.Id;
                invMod.OrderOwner = restService.Get(inv.RestaurantId).Name;
                invMod.OrderTime = inv.CheckOutTime.ToString("dd/MM/yyyy hh:mm tt");
                invMod.Bill = inv.Bill;
                invMod.TokenNo = inv.TokenNo;
                invModels.Add(invMod);
            }

            return View(invModels);
        }

        [HttpGet]
        public ActionResult AddToCart()
        {
            Orders = (List<Order>)Session["Cart"];
            string id = Request.QueryString["id"];
            Orders = (List<Order>)Session["Cart"];
            Item item = new Repository<Item>().Get(System.Convert.ToInt32(id));
            Restaurant rest = restService.Get(item.RestaurantId);
            Order existingOrder = Orders.FirstOrDefault();

            if ((DateTime.Now - rest.LastOnline).Hours <= 1)
            {
                if (existingOrder != null)
                {
                    rest = (Restaurant)Session["CartRestaurant"];
                    if (rest.Id == item.RestaurantId)
                    {
                        Session["Cart"] = Orders = custService.AddToCart(System.Convert.ToInt32(id), Orders);
                        Session["CartRestaurant"] = restService.Get(item.RestaurantId);
                        Session["Cart"] = Orders;
                        return Json(Orders, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    Session["Cart"] = Orders = custService.AddToCart(System.Convert.ToInt32(id), Orders);
                    Session["CartRestaurant"] = rest;
                    Session["Cart"] = Orders;
                    return Json(Orders, JsonRequestBehavior.AllowGet);
                }
            }
            else return Json("Closed", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult RemoveFromCart()
        {
            Orders = (List<Order>)Session["Cart"];
            string id = Request.QueryString["id"];
            Session["Cart"] = Orders = custService.RemoveFromCart(System.Convert.ToInt32(id), Orders);
            return Json(Orders, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Cart()
        {
            Orders = (List<Order>)Session["Cart"];
            double bill = 0, discount = 0, vat = 0;
            if (Orders.Count != 0)
            {
               return View(PrepareCart(-1, Orders));
            }
            else
            {
                ViewBag.Bill = bill.ToString();
                ViewBag.Total = bill;
                ViewBag.VATinPercentage = "0";
                ViewBag.DiscountinPercentage = "0";
                ViewBag.DeliveryCharge = "0";
                ViewBag.Discount = "0";
                ViewBag.VAT = "0";
                return View(new CartModel());
            }
        }

        [HttpGet]
        public ActionResult CheckOut()
        {
            thisUser = (Customer)Session["loggedInUser"];
            if (thisUser.DeliveryAddress != null)
            {
                AddressModel address = new AddressModel();
                address.Flat = (thisUser.DeliveryAddress.Split(',')[0]);
                address.House = (thisUser.DeliveryAddress.Split(',')[1]);
                address.Road = (thisUser.DeliveryAddress.Split(',')[2]);
                address.Area = (thisUser.DeliveryAddress.Split(',')[3]);
                address.District = (thisUser.DeliveryAddress.Split(',')[4]);
                return View(address);
            }
            else return View(new AddressModel());
        }

        [HttpPost]
        public ActionResult CheckOut(AddressModel addressModel)
        {
            thisUser = (Customer)Session["loggedInUser"];
            if (ModelState.IsValid)
            {
                Customer cust = new Repository<Customer>().Get(thisUser.Id);
                string newAddress = addressModel.Flat + "," + addressModel.House + "," + addressModel.Road + "," + addressModel.Area + "," + addressModel.District;
                string contactNo = addressModel.Contact;
                Session["loggedInUser"] = cust = custService.UpdateDeliveryInfo(cust,newAddress,contactNo);

                Orders = (List<Order>)Session["Cart"];
                custService.CheckOut(cust, Orders);
                
                Orders = new List<Order>();
                Session["Cart"] = new List<Order>();

                return RedirectToAction("Index", "Customer");
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditAddress()
        {
            thisUser = (Customer)Session["loggedInUser"];
            AddressModel address = new AddressModel();
            if (thisUser.DeliveryAddress != null)
            {
                address.Flat = (thisUser.DeliveryAddress.Split(',')[0]);
                address.House = (thisUser.DeliveryAddress.Split(',')[1]);
                address.Road = (thisUser.DeliveryAddress.Split(',')[2]);
                address.Area = (thisUser.DeliveryAddress.Split(',')[3]);
                address.District = (thisUser.DeliveryAddress.Split(',')[4]);
            }
            return View(address);
        }
        [HttpPost]
        public ActionResult EditAddress(AddressModel addressModel)
        {
            thisUser = (Customer)Session["loggedInUser"];
            if (ModelState.IsValid)
            {
                Customer cust = custService.Get(thisUser.Id);
                cust.DeliveryAddress= addressModel.Flat + "," + addressModel.House + "," + addressModel.Road + "," + addressModel.Area + "," + addressModel.District;
                Session["loggedInUser"] = cust;
                custService.Update(cust, thisUser.Id);
                return RedirectToAction("Index", "Customer");
            }
            return View();
        }

        [HttpGet]
        public ActionResult InvoiceDetails(int id)
        {
            List<Order> orders = orderService.GetOrdersByInvoiceId(id);
            return View(PrepareCart(id,orders));
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["loggedInUser"] = null;
            Orders = new List<Order>();
            Session["Cart"] = Orders;
            Session["CartRestaurant"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Public");
        }

        [HttpGet]
        public ActionResult Home()
        {
            return RedirectToAction("Index", "Customer");
        }

        [HttpGet]
        public ActionResult TrackOrder(int invId)
        {
            return Json(invService.Get(invId).Status,JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult WriteReview(int restaurantId)
        {
            return View(restaurantId);
        }

        [HttpPost]
        public ActionResult WriteReview(Review review, HttpPostedFileBase[] images)
        {
            Customer thisUser = (Customer)Session["loggedInUser"];
            if(custService.PostReview(review,thisUser.Id)>=1)
            foreach (HttpPostedFileBase file in images)
            {
                //Checking file is available to save.  
                if (file != null)
                {
                    string inputFileName = "image/reviews/" + custService.GetImageId().ToString() + Path.GetExtension(file.FileName).ToString();
                    custService.AddReviewImage(inputFileName);
                    var serverSavePath = Path.Combine(Server.MapPath("~/") + inputFileName);
                    //Save file to server folder  
                    file.SaveAs(serverSavePath);
                    //assigning file uploaded status to ViewBag for showing message to user.  
                    ViewBag.UploadStatus = images.Count().ToString() + " files uploaded successfully.";
                }
            }  
            return View();
        }

        [HttpGet]
        public ActionResult ShowReviews()
        {
            List<Review> reviews = custService.GetAllReviews();
            List<ReviewModel> revModels = new List<ReviewModel>();
            foreach (Review rev in reviews)
            {
                ReviewModel revModel = new ReviewModel();
                revModel.ReviewId = rev.Id;
                revModel.Reviewer = custService.Get(rev.CustomerId).Name;
                revModel.Description = rev.Description;
                revModel.RestaurantName = restService.Get(rev.RestaurantId).Name;
                revModel.Rating = rev.Rating;
                revModel.Time = rev.Time.ToString("dd/MM/yyyy hh:mm tt");
                revModel.Comments = custService.GetAllComments(rev.Id);
                revModel.CommentModels = PrepareCommentModel(revModel.Comments);

                foreach (Comment comment in revModel.Comments)
                {
                    revModel.Commenters.Add(custService.Get(comment.CustomerId).Name);
                    revModel.Texts.Add(comment.Text);
                    revModel.CommentTimes.Add(comment.Time.ToString("dd/MM/yyyy hh:mm tt"));
                }

                if (custService.GetReviewImageLocation(rev.Id) != null) revModel.Images = custService.GetReviewImageLocation(rev.Id);
                revModels.Add(revModel);
            }
            return View(revModels);
        }

        [HttpGet]
        public ActionResult ShowRestaurantReviews(int restId)
        {
            List<Review> reviews = custService.GetAllReviews().Where(rev=> rev.RestaurantId==restId).ToList();
            List<ReviewModel> revModels = new List<ReviewModel>();
            foreach (Review rev in reviews)
            {
                ReviewModel revModel = new ReviewModel();
                revModel.ReviewId = rev.Id;
                revModel.Reviewer = custService.Get(rev.CustomerId).Name;
                revModel.Description = rev.Description;
                revModel.RestaurantName = restService.Get(rev.RestaurantId).Name;
                revModel.Rating = rev.Rating;
                revModel.RestaurantId = rev.RestaurantId;
                revModel.Time = rev.Time.ToString("dd/MM/yyyy hh:mm tt");
                revModel.Comments = custService.GetAllComments(rev.Id);
                revModel.CommentModels = PrepareCommentModel(revModel.Comments);

                foreach (Comment comment in revModel.Comments)
                {
                    revModel.Commenters.Add(custService.Get(comment.CustomerId).Name);
                    revModel.Texts.Add(comment.Text);
                    revModel.CommentTimes.Add(comment.Time.ToString("dd/MM/yyyy hh:mm tt"));
                }

                if (custService.GetReviewImageLocation(rev.Id) != null) revModel.Images = custService.GetReviewImageLocation(rev.Id);
                revModels.Add(revModel);
            }
            return View("ShowReviews",revModels);
        }

        [HttpPost]
        public ActionResult AddComment(CommentModel comment)
        {
            Customer thisUser = (Customer)Session["loggedInUser"];
            Comment postComment = new Comment();
            postComment.Text = comment.CommentText;
            postComment.ReviewId = comment.ReviewId;
            postComment.CustomerId = thisUser.Id;
            postComment.Time = DateTime.Now;

            custService.AddComment(postComment);
            List<Comment> comments = custService.GetAllComments(comment.ReviewId);
            return Json(PrepareCommentModel(comments), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ShowAllComments(int reviewId)
        {
            List<Comment> comments = custService.GetAllComments(reviewId);
            return Json(PrepareCommentModel(comments), JsonRequestBehavior.AllowGet);
        }

        public List<CommentModel> PrepareCommentModel(List<Comment> comments)
        {
            List<CommentModel> commentModels = new List<CommentModel>();
            foreach (Comment comment in comments)
            {
                CommentModel cmt = new CommentModel();
                cmt.Commenter = (custService.Get(comment.CustomerId).Name);
                cmt.CommentText= (comment.Text);
                cmt.Time = comment.Time.ToString("dd/MM/yyyy hh:mm tt");
                commentModels.Add(cmt);
            }
            return commentModels;
        }

        public CartModel PrepareCart(int id,List<Order> orders)
        {
            Customer thisUser = (Customer)Session["loggedInUser"];
            Invoice thisInv = invService.Get(id);
            double bill = 0,discount=0,vat=0;
            Item item;
            CartModel cart = new CartModel();
            ViewBag.InvoiceNo = id;

            foreach (Order ord in orders)
            {
                item = new Item();
                item = itemService.Get(ord.ItemId);
                cart.Item.Add(item);
                cart.Quantity.Add(ord.Quantity);
                cart.Price.Add(ord.Total);
                bill += ord.Total;
            }

            if (thisInv != null)
            {
                cart.RestaurantName = restService.GetByInvoice(thisInv).Name;
                cart.Total = bill.ToString();
                cart.Bill = thisInv.Bill.ToString();
                cart.VATinPercentage = thisInv.VATinPercentage.ToString();
                cart.DiscountinPercentage = thisInv.DiscountinPercentage.ToString();
                cart.DeliveryCharge = thisInv.DeliveryCharge.ToString();
                ViewBag.GrandTotal = (thisInv.Bill - thisInv.VAT).ToString();
                cart.Discount = thisInv.Discount.ToString();
                cart.VAT = thisInv.VAT.ToString();
                cart.InvoiceNo = id.ToString();

                ViewBag.RestaurantName = thisInv.RestaurantName;
                ViewBag.RestaurantAddress = restService.GetRestaurantAddress(thisInv.RestaurantId).FormattedAddress;
                ViewBag.Recipient = thisInv.Recipient;
                ViewBag.RecipientContactNo = thisInv.RecipientContactNo;
                ViewBag.DeliveryAddress = thisInv.DeliveryAddress;
            }
            else
            {
                Order anOrder = Orders.First();
                Restaurant rest = restService.GetByOrder(anOrder);
                discount = ((rest.DiscountinPercentage * bill) / 100);
                vat = ((rest.VATinPercentage * bill) / 100);

                cart.Total = bill.ToString();
                bill -= discount;
                bill += vat;
                bill += rest.DeliveryCharge;

                bill = Math.Ceiling(bill);

                cart.RestaurantName = rest.Name;
                cart.Bill = bill.ToString();
                cart.VATinPercentage = rest.VATinPercentage.ToString();
                cart.DiscountinPercentage = rest.DiscountinPercentage.ToString();
                cart.DeliveryCharge = rest.DeliveryCharge.ToString();
                cart.Discount = discount.ToString();
                cart.VAT = vat.ToString();
            }

            return cart;
        }
    }
}
