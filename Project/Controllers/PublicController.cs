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

namespace Project.Controllers
{
    [Authorize(Roles="Public")]
    public class PublicController : Controller
    {
        List<Order> Orders;
        IRestaurantService restService;
        IRestaurantAddressService restAddService;
        IPublicService publicService;
        IItemService itemService;

        public PublicController(IPublicService publicService, IRestaurantService restService, IRestaurantAddressService restAddService, IItemService itemService)
        {
            this.restService = restService;
            this.publicService = publicService;
            this.restAddService = restAddService;
            this.itemService = itemService;
            Orders = new List<Order>();
        }

        public double toRadians(double degree)
        {
            return (Math.PI / 180) * degree;
        }
        //
        // GET: /Default1/
        [HttpGet]
        public ActionResult HomeSuggestions()
        {
            LocationModel locationModel = new LocationModel();
            List<RestaurantAddress> restaurantAddresses = restAddService.GetAll().ToList();
            List<Restaurant> selectedRestaurants = new List<Restaurant>();
            Restaurant rest;

            locationModel.Latitude = System.Convert.ToDouble(Request.QueryString["Latitude"]);
            locationModel.Longitude = System.Convert.ToDouble(Request.QueryString["Longitude"]);

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
        public ActionResult AutoComplete(string val)
        {
            StringComparison comp = StringComparison.OrdinalIgnoreCase;
            List<Restaurant> rests = restService.GetAll().Where(rest => rest.Name.IndexOf(val, StringComparison.OrdinalIgnoreCase)>=0).ToList();
            return Json(rests,JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ShowReviews()
        {
            List<Review> reviews = publicService.GetAllReviews();
            List<ReviewModel> revModels = new List<ReviewModel>();
            foreach (Review rev in reviews)
            {
                ReviewModel revModel = new ReviewModel();
                revModel.ReviewId = rev.Id;
                revModel.Reviewer = publicService.GetReviewerName(rev.CustomerId);
                revModel.Description = rev.Description;
                revModel.RestaurantName = restService.Get(rev.RestaurantId).Name;
                revModel.Rating = rev.Rating;
                revModel.Time = rev.Time.ToString("dd/MM/yyyy hh:mm tt");
                revModel.Comments = publicService.GetReviewComments(rev.Id);
                revModel.CommentModels = PrepareCommentModel(revModel.Comments);

                if (publicService.GetReviewImageLocation(rev.Id) != null) revModel.Images = publicService.GetReviewImageLocation(rev.Id);
                revModels.Add(revModel);
            }
            return View(revModels);
        }
        [HttpGet]
        public ActionResult ShowRestaurantReviews(int restId)
        {
            List<Review> reviews = publicService.GetAllReviews().Where(rev => rev.RestaurantId == restId).ToList();
            List<ReviewModel> revModels = new List<ReviewModel>();
            foreach (Review rev in reviews)
            {
                ReviewModel revModel = new ReviewModel();
                revModel.ReviewId = rev.Id;
                revModel.Reviewer = publicService.GetReviewerName(rev.CustomerId);
                revModel.Description = rev.Description;
                revModel.RestaurantName = restService.Get(rev.RestaurantId).Name;
                revModel.Rating = rev.Rating;
                revModel.Time = rev.Time.ToString("dd/MM/yyyy hh:mm tt");
                revModel.Comments = publicService.GetReviewComments(rev.Id);
                revModel.CommentModels = PrepareCommentModel(revModel.Comments);

                if (publicService.GetReviewImageLocation(rev.Id) != null) revModel.Images = publicService.GetReviewImageLocation(rev.Id);
                revModels.Add(revModel);
            }
            return View("ShowReviews", revModels);
        }

        [HttpGet]
        public ActionResult ShowAllComments(int reviewId)
        {
            List<Comment> comments = publicService.GetReviewComments(reviewId);
            return Json(PrepareCommentModel(comments), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Menu(int id)
        {
            List<Item> items = new Repository<Item>().GetAll() as List<Item>;
            Restaurant rest = restService.Get(id);
            items = items.Where(item => item.RestaurantId == id).ToList();
            MenuModel menuModel = new MenuModel();
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

            double bill = 0;

            foreach (Order ord in Orders)
            {
                bill += ord.Total;
            }

            ViewBag.Bill = bill.ToString();


            return View(menuModel);
        }

        [HttpGet]
        public ActionResult SearchResult()
        {
            List<Restaurant> restaurants = (List<Restaurant>)Session["restaurants"];
            return View(restaurants);
        }
        [HttpGet]
        public ActionResult DiscardCart()
        {
            Orders = new List<Order>();
            Session["Cart"] = Orders;
            Session["CartRestaurant"] = null;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddToCart()
        {
            Orders = (List<Order>)Session["Cart"];

            string id = Request.QueryString["id"];
            Item item = new Repository<Item>().Get(System.Convert.ToInt32(id));

            Order existingOrder = Orders.FirstOrDefault();

            if (existingOrder != null)
            {
                Restaurant rest = (Restaurant)Session["CartRestaurant"];
                if (rest.Id == item.RestaurantId)
                {
                    Order order = Orders.Where(order1 => order1.ItemId == item.Id).SingleOrDefault();
                    if (order == null)
                    {
                        order = new Order();
                        order.ItemId = item.Id;
                        order.ItemName = item.Name;
                        order.Total = item.Price;
                        order.Quantity = 1;
                        Orders.Add(order);
                        Session["CartRestaurant"] = restService.Get(item.RestaurantId);
                    }
                    else
                    {
                        order.Quantity++;
                        order.Total = order.Quantity * item.Price;
                    }
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
                Order order = new Order();
                order.ItemId = item.Id;
                order.ItemName = item.Name;
                order.Total = item.Price;
                order.Quantity = 1;
                Orders.Add(order);
                Session["CartRestaurant"] = restService.Get(item.RestaurantId);
                Session["Cart"] = Orders;
                return Json(Orders, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult RemoveFromCart()
        {
            Orders = (List<Order>)Session["Cart"];

            string id = Request.QueryString["id"];
            Order order = Orders.Where(order1 => order1.ItemId == System.Convert.ToInt32(id)).SingleOrDefault();

            if (order.Quantity > 1)
            {
                order.Total = order.Total / order.Quantity;
                order.Quantity--;
                order.Total = order.Total * order.Quantity;
            }
            else
            {
                Orders.Remove(order);
            }

            Session["Cart"] = Orders;
            return Json(Orders, JsonRequestBehavior.AllowGet); 
        }

        [HttpGet]
        public ActionResult Cart()
        {
            Orders = (List<Order>)Session["Cart"];
            double bill = 0,discount=0,vat=0;
            if (Orders.Count!=0)
            {
                List<Item> items = new Repository<Item>().GetAll() as List<Item>;
                Order anOrder = Orders.First();
                Item anItem = items.Where(item => item.Id == anOrder.ItemId).SingleOrDefault();

                Restaurant rest = new Repository<Restaurant>().Get(anItem.RestaurantId);

                CartModel cartModel = new CartModel();
                cartModel.RestaurantName = rest.Name;

                foreach (Order order in Orders)
                {
                    cartModel.Item.Add(items.Where(item => item.Id == order.ItemId).SingleOrDefault());
                    cartModel.Quantity.Add(order.Quantity);
                    cartModel.Price.Add(order.Total);
                    bill += order.Total;
                }
                discount = ((rest.DiscountinPercentage * bill) / 100);
                vat = ((rest.VATinPercentage * bill) / 100);

                cartModel.Total = bill.ToString();
                cartModel.VATinPercentage = rest.VATinPercentage.ToString();
                cartModel.DiscountinPercentage = rest.DiscountinPercentage.ToString();
                cartModel.DeliveryCharge = rest.DeliveryCharge.ToString();
                cartModel.Discount = discount.ToString();
                cartModel.VAT = vat.ToString();

                bill -= discount;
                bill += vat;
                bill += rest.DeliveryCharge;
                bill = Math.Ceiling(bill);

                ViewBag.Bill = bill;
                return View(cartModel);
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
        public ActionResult Checkout()
        {
            return View();
        }

        public List<CommentModel> PrepareCommentModel(List<Comment> comments)
        {
            List<CommentModel> commentModels = new List<CommentModel>();
            foreach (Comment comment in comments)
            {
                CommentModel cmt = new CommentModel();
                cmt.Commenter = (publicService.GetCommenterName(comment.CustomerId));
                cmt.CommentText = (comment.Text);
                cmt.Time = comment.Time.ToString("dd/MM/yyyy hh:mm tt");
                commentModels.Add(cmt);
            }
            return commentModels;
        }
    }
}
