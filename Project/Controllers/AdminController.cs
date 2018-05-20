using Project.Core.Interfaces;
using Project.Data;
using Project.Entity;
using Project.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        IAdminService adminrepo;
        ICustomerService custrepo;
        IRestaurantService restrepo;
        IUserService userRepo;
        ITransporterService trepo;
        IInvoiceService irepo;
        IOrderService orepo;
        IItemService itemrepo;
        IRestaurantAddressService restAddService;

        public AdminController(IAdminService adminrepo,ICustomerService custrepo,IRestaurantService restrepo,IUserService userRepo,
            ITransporterService trepo, IInvoiceService irepo, IOrderService orepo, IItemService itemrepo, IRestaurantAddressService restAddService)
        {
            this.adminrepo = adminrepo;
            this.custrepo = custrepo;
            this.restrepo = restrepo;
            this.userRepo = userRepo;
            this.trepo = trepo;
            this.irepo = irepo;
            this.orepo = orepo;
            this.itemrepo = itemrepo;
            this.restAddService = restAddService;
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

            List<Invoice> invs = irepo.GetAll().Where(inv => inv.Status != "Delivered" && inv.Status != "Cancelled").ToList();

            return View(PrepareInvoices(invs));
        }

        [HttpGet]
        public ActionResult GetInvoices()
        {
            List<Invoice> invs = irepo.GetAll().Where(inv => inv.Status != "Delivered" && inv.Status != "Cancelled").ToList();
            return Json(PrepareInvoices(invs), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetInvoiceDetails(int invId)
        {
            return Json(PrepareInvoiceDetails(invId), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ChangeOrderStatus(int invId, string status)
        {
            irepo.ChangeStatus(invId, status);
            List<Invoice> invs = irepo.GetAll().Where(inv => inv.Status != "Delivered" && inv.Status != "Cancelled").ToList();
            return Json(PrepareInvoices(invs), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetTasks()
        {
            List<Invoice> invs = irepo.GetAll().Where(inv => inv.Status == "On The Way").ToList();
            return Json(invs, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ViewAccount()
        {
            Admin thisUser = (Admin)Session["loggedInUser"];
            Admin adminToShow = adminrepo.Get(thisUser.Id);

            return View("Account", adminToShow);
        }

        [HttpGet]
        public ActionResult EditAccount()
        {
            Admin thisUser = (Admin)Session["loggedInUser"];
            Admin adminToShow = adminrepo.Get(thisUser.Id);

            ViewBag.dobDay = Convert.ToInt32(adminToShow.DateOfBirth.Split('/')[0]);
            ViewBag.dobMonth = Convert.ToInt32(adminToShow.DateOfBirth.Split('/')[1]);
            ViewBag.dobYear = Convert.ToInt32(adminToShow.DateOfBirth.Split('/')[2]);

            EditProfileModel editProfileModel = new EditProfileModel();
            editProfileModel.Name = adminToShow.Name;
            editProfileModel.Gender = adminToShow.Gender;
            //editProfileModel.Email = adminToShow.Email;
            return View("EditAccount", editProfileModel);
        }

        [HttpPost]
        public ActionResult EditAccount(EditProfileModel editProfileModel, FormCollection form)
        {
            ModelState.Remove("Email");
            ModelState.Remove("Role");
            ModelState.Remove("Status");
            if (ModelState.IsValid)
            {
                Admin thisUser = (Admin)Session["loggedInUser"];
                Admin adminToEdit = adminrepo.Get(thisUser.Id);

                adminToEdit.Name = editProfileModel.Name;
                adminToEdit.Gender = editProfileModel.Gender;

                string year = form["dobYear"];
                string month = form["dobMonth"];
                string day = form["dobDay"];

                adminToEdit.DateOfBirth = day + "/" + month + "/" + year;

                adminrepo.Update(adminToEdit, adminToEdit.Id);

                return RedirectToAction("ViewAccount");
            }
            return View(editProfileModel);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View("ChangePassword");
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (ModelState.IsValid)
            {
                Admin admin = (Admin)Session["loggedInUser"];
                Admin thisUser = adminrepo.Get(admin.Id);

                if (thisUser.Password == changePasswordModel.CurrentPassword)
                {
                    thisUser.Password = changePasswordModel.NewPassword;
                    adminrepo.Update(thisUser, thisUser.Id);
                    ViewBag.success = "Password changed successfully";
                }
                else
                {
                    ModelState.AddModelError("Password", "Wrong password.");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult SalesReport()
        {

            List<Sales> rlist = new List<Sales>();
            foreach (var rest in restrepo.GetAll())
            {
                Sales salesmodel = new Sales();
                salesmodel.Name = rest.Name;
                salesmodel.totalOrder = irepo.GetAllByRestaurantId(rest.Id).Where(order=>order.Status=="Delivered").Count();
                salesmodel.Id = rest.Id;
                rlist.Add(salesmodel);
            }
            ViewBag.Restaurants = rlist;
            Session["sales"] = rlist;
            return View("SalesReport");
        }

        [HttpGet]
        public ActionResult SalesDetails(int id)
        {
            List<Invoice> invoicelist = irepo.GetAllByRestaurantId(id).Where(order => order.Status == "Delivered").ToList();
            List<List<Order>> orderlist = new List<List<Order>>();
            List<Order> olist = new List<Order>();
            List<Item> itemlist = itemrepo.GetAllByRestaurantId(id);
            List<int> qlist = new List<int>();
            foreach (var invoices in invoicelist)
            {
                olist = orepo.GetOrdersByInvoiceId(invoices.Id);
                orderlist.Add(olist);
            }

            qlist = orderlist.SelectMany(list => list).GroupBy(item => item.ItemName).Select(y => y.Sum(i => i.Quantity)).ToList() as List<int>;

            ViewBag.total = orderlist.SelectMany(list => list).Sum(y => y.Quantity);
            ViewBag.itemlist = itemlist;
            ViewBag.quantitylist = qlist;
            ViewBag.id = id;

            Session["orderlist"] = orderlist;

            return View();
        }

        [HttpPost]
        public ActionResult SalesDetails(string id,string month)
        {
            int restid = Convert.ToInt32(id);
            List<Invoice> invoicelist = new List<Invoice>();
            if (month == "")
            {
                 invoicelist = irepo.GetAllByRestaurantId(restid);
            }
            else
            {
                string m = month.Substring(month.LastIndexOf("-") + 1);
                int mon = Convert.ToInt32(m);
                List<Invoice> ilist = irepo.GetAllByRestaurantId(restid);
                invoicelist = ilist.Where(i => i.CheckOutTime.Month == mon).ToList() as List<Invoice>;
            }
            List<List<Order>> orderlist = new List<List<Order>>();
            List<Order> olist = new List<Order>();
            List<Item> itemlist = itemrepo.GetAllByRestaurantId(restid);
            List<int> qlist = new List<int>();
            foreach (var invoices in invoicelist)
            {
                olist = orepo.GetOrdersByInvoiceId(invoices.Id);
                orderlist.Add(olist);
            }

            qlist = orderlist.SelectMany(list => list).GroupBy(item => item.ItemName).Select(y => y.Sum(i => i.Quantity)).ToList() as List<int>;

            ViewBag.total = orderlist.SelectMany(list => list).Sum(y => y.Quantity);
            ViewBag.itemlist = itemlist;
            ViewBag.quantitylist = qlist;
            ViewBag.id = id;
 
            return View();
        }

         public ActionResult SalesChart()
         {
             string mytheme = @"<Chart BackColor=""Transparent"">
                                        <ChartAreas>
                                            <ChartArea Name=""Default"" BackColor=""Transparent"" ></ChartArea> 
                                        </ChartAreas>   
                                      </Chart>";
             List<List<Order>> orderlist = (List<List<Order>>)Session["orderlist"];

             ArrayList x = new ArrayList();
             ArrayList y = new ArrayList();

             var itemlist = orderlist.SelectMany(list => list).GroupBy(item => item.ItemName).Select(
                 z => new
                 {
                     totalQuantity = z.Sum(i => i.Quantity),
                     ItemName = z.First().ItemName
                 });


             itemlist.ToList().ForEach(item => x.Add(item.ItemName));
             itemlist.ToList().ForEach(q => y.Add(q.totalQuantity));

             new Chart(width: 250, height: 250, theme: mytheme)
             .AddSeries(
             chartType: "pie",
             xValue: x,
             yValues: y
             ).Write("png");

             return null;
         }

         public ActionResult totalSalesChart()
         {
             string mytheme = @"<Chart BackColor=""Transparent"">
                                        <ChartAreas>
                                            <ChartArea Name=""Default"" BackColor=""Transparent"" ></ChartArea> 
                                        </ChartAreas>   
                                      </Chart>";
             ArrayList x = new ArrayList();
             ArrayList y = new ArrayList();

             List<Sales> saleslist = (List<Sales>)Session["sales"];

             saleslist.Where(item => item.totalOrder > 0).ToList().ForEach(item => x.Add(item.Name));
             saleslist.Where(item => item.totalOrder > 0).ToList().ForEach(q => y.Add(q.totalOrder));

             new Chart(width: 250, height: 250, theme: mytheme)
             .AddSeries(
             chartType: "pie",
             xValue: x,
             yValues: y
             ).Write("png");

             return null;
         }

         private bool isValidContentType(string contentType)
         {
             return contentType.Equals("image/png") || contentType.Equals("image/jpg") || contentType.Equals("image/jpeg") || contentType.Equals("image/gif");
         }

         private bool isValidContentLength(int contentLength)
         {
             return ((contentLength / 1024) / 1024) < 1;
         }

         [HttpPost]
         public ActionResult UploadImage(HttpPostedFileBase[] photo)
         {

             foreach (HttpPostedFileBase file in photo)
             {
                 //Checking file is available to save.  
                 if (file != null)
                 {
                     var fileName = Path.GetFileName(file.FileName);
                     var path = Path.Combine(Server.MapPath("~/image/advertisement"), fileName);
                     file.SaveAs(path);

                 }
             }

             string[] filePaths = Directory.GetFiles(Server.MapPath("/image/advertisement/"));
             List<string> files = new List<string>();
             foreach (string filePath in filePaths)
             {
                 string fileName = Path.GetFileName(filePath);
                 string src = "/image/advertisement/" + fileName;
                 files.Add(src);

             }
             ViewBag.files = files;
             return View("Index");
         }


        [HttpGet]
        public ActionResult SearchUser(int id = 0)
        {
            ViewBag.success = TempData["success"];
            if (id == 1)
            {
                string filter = TempData["filter"].ToString();
                if (filter == "name")
                {
                    string searchname = TempData["search"].ToString();
                    ViewBag.custlist = adminrepo.GetByName<Customer>(searchname);
                }
                else if (filter == "email")
                {
                    string searchemail = TempData["search"].ToString();
                    ViewBag.custlist = adminrepo.GetByEmail<Customer>(searchemail);
                }
                else if (filter == "status")
                {
                    string searchstatus = TempData["search"].ToString();
                    ViewBag.custlist = adminrepo.GetByStatus<Customer>(searchstatus);
                }
                else
                {
                    ViewBag.custlist = custrepo.GetAll();
                }
            }
            else
            {
                ViewBag.custlist = custrepo.GetAll();
            }
            return View();
        }

        [HttpPost]
        public ActionResult SearchUser(string search, string filter)
        {
            TempData["search"] = search;
            TempData["filter"] = filter;
            return RedirectToAction("SearchUser", new { id = 1 });
        }

        [HttpGet]
        public ActionResult DetailsOfUser(string email)
        {
            int custid = adminrepo.GetIdFromEmail<Customer>(email);
            Customer detailOfUser = custrepo.Get(custid);
            ViewBag.success = TempData["success"];
            return View(detailOfUser);
        }

        [HttpGet]
        public ActionResult EditUser(string email)
        {
            int custid = adminrepo.GetIdFromEmail<Customer>(email);
            Customer detailOfUser = custrepo.Get(custid);

            ViewBag.dobDay = Convert.ToInt32(detailOfUser.DateOfBirth.Split('/')[0]);
            ViewBag.dobMonth = Convert.ToInt32(detailOfUser.DateOfBirth.Split('/')[1]);
            ViewBag.dobYear = Convert.ToInt32(detailOfUser.DateOfBirth.Split('/')[2]);

            EditProfileModel edituser = new EditProfileModel();
            edituser.Email = detailOfUser.Email;
            edituser.Name = detailOfUser.Name;
            edituser.Status = detailOfUser.Status;
            edituser.Gender = detailOfUser.Gender;

            return View(edituser);
        }

        [HttpPost]
        public ActionResult EditUser(EditProfileModel user, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                int custid = adminrepo.GetIdFromEmail<Customer>(user.Email);
                Customer userToEdit = custrepo.Get(custid);
                userToEdit.Name = user.Name;
                userToEdit.Gender = user.Gender;
                userToEdit.Status = user.Status;

                string year = form["dobYear"];
                string month = form["dobMonth"];
                string day = form["dobDay"];

                userToEdit.DateOfBirth = day + "/" + month + "/" + year;

                custrepo.Update(userToEdit, userToEdit.Id);

                TempData["success"] = "User Edited Successfully";

                return RedirectToAction("SearchUser", "Admin");
            }

            return View(user);

        }

        [HttpGet]
        public ActionResult DeleteUser(string email)
        {
            int custid = adminrepo.GetIdFromEmail<Customer>(email);
            Customer detailOfUser = custrepo.Get(custid);
            ViewBag.success = TempData["success"];
            return View(detailOfUser);
        }

        [HttpPost]
        [ActionName("DeleteUser")]
        public ActionResult DeleteUserPost(string email)
        {
            int custid = adminrepo.GetIdFromEmail<Customer>(email);
            Customer cust = custrepo.Get(custid);
            custrepo.Delete(cust);
            TempData["success"] = "User deleted successfully";
            return RedirectToAction("SearchUser", new { id = 0 });
        }

        [HttpGet]
        public ActionResult SearchRestaurant(int id = 0)
        {
            ViewBag.success = TempData["success"];
            if (id == 1)
            {
                string filter = TempData["filter"].ToString();
                if (filter == "name")
                {
                    string searchname = TempData["search"].ToString();
                    ViewBag.restlist = adminrepo.GetByName<Restaurant>(searchname);
                }
                else if (filter == "email")
                {
                    string searchemail = TempData["search"].ToString();
                    ViewBag.restlist = adminrepo.GetByEmail<Restaurant>(searchemail);
                }
                else if (filter == "status")
                {
                    string searchstatus = TempData["search"].ToString();
                    ViewBag.restlist = adminrepo.GetByStatus<Restaurant>(searchstatus);
                }
                else
                {
                    ViewBag.restlist = restrepo.GetAll();
                }
            }
            else
            {
                ViewBag.restlist = restrepo.GetAll();
            }
            return View();

        }


        [HttpPost]
        public ActionResult SearchRestaurant(string search, string filter)
        {
            TempData["search"] = search;
            TempData["filter"] = filter;
            return RedirectToAction("SearchRestaurant", new { id = 1 });
        }

        [HttpGet]
        public ActionResult DetailsOfRestaurant(string email)
        {
            int restid = adminrepo.GetIdFromEmail<Restaurant>(email);
            Restaurant detailOfRestaurent = restrepo.Get(restid);
            ViewBag.success = TempData["success"];
            return View(detailOfRestaurent);
        }

        [HttpGet]
        public ActionResult EditRestaurant(string email)
        {
            int restid = adminrepo.GetIdFromEmail<Restaurant>(email);
            Restaurant detailOfRestaurant = restrepo.Get(restid);

            RestaurantModel resmodel = new RestaurantModel();

            resmodel.Name = detailOfRestaurant.Name;
            resmodel.Status = detailOfRestaurant.Status;
            resmodel.Email = detailOfRestaurant.Email;

            return View(resmodel);
        }

        [HttpPost]
        public ActionResult EditRestaurant(RestaurantModel restaurant)
        {
            if (ModelState.IsValid)
            {
                int restid = adminrepo.GetIdFromEmail<Restaurant>(restaurant.Email);
                Restaurant restToEdit = restrepo.Get(restid);

                restToEdit.Name = restaurant.Name;
                restToEdit.Status = restaurant.Status;
                restrepo.Update(restToEdit, restToEdit.Id);

                TempData["success"] = "Restaurant Edited Successfully";

                return RedirectToAction("SearchRestaurant", "Admin");
            }
            return View(restaurant);
        }

        [HttpGet]
        public ActionResult DeleteRestaurant(string email)
        {
            int restid = adminrepo.GetIdFromEmail<Restaurant>(email);
            Restaurant restaurantToDelete = restrepo.Get(restid);
            ViewBag.success = TempData["success"];
            return View(restaurantToDelete);
        }

        [HttpPost]
        [ActionName("DeleteRestaurant")]
        public ActionResult DeleteRestaurantPost(string email)
        {
            int restid = adminrepo.GetIdFromEmail<Restaurant>(email);
            Restaurant rest = restrepo.Get(restid);
            restrepo.Delete(rest);


            TempData["success"] = "Restaurant deleted successfully";

            return RedirectToAction("SearchRestaurant", new { id = 0 });
        }

        [HttpPost]
        public ActionResult SearchTransporter(string search, string filter)
        {
            TempData["search"] = search;
            TempData["filter"] = filter;
            return RedirectToAction("SearchTransporter", new { id = 1 });
        }

        [HttpGet]
        public ActionResult TaskHistoryOfTransporter(int id)
        {
            Transporter transporter = trepo.Get(id);
            List<TransporterTaskModel> tasks = new List<TransporterTaskModel>();
            foreach (TransporterTask task in trepo.GetTasks(transporter.Id))
            {
                Invoice inv = irepo.Get(task.InvoiceId);
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

        public CartModel PrepareInvoiceDetails(int invId)
        {
            double bill = 0;
            Repository<Item> items = new Repository<Item>();
            CartModel cart = new CartModel();
            Item item = new Item();

            foreach (Order ord in orepo.GetOrdersByInvoiceId(invId))
            {
                item = new Item();
                item = items.Get(ord.ItemId);
                cart.Item.Add(item);
                cart.Quantity.Add(ord.Quantity);
                cart.Price.Add(ord.Total);
                bill += ord.Total;
            }

            Invoice thisInv = new Repository<Invoice>().Get(invId);

            cart.RestaurantName = restrepo.GetByInvoice(thisInv).Name;
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
            cart.RestaurantAddress = restrepo.GetRestaurantAddress(thisInv.RestaurantId).FormattedAddress;
            cart.Recipient = thisInv.Recipient;
            cart.RecipientContactNo = thisInv.RecipientContactNo;
            cart.DeliveryAddress = thisInv.DeliveryAddress;

            return (cart);
        }

        [HttpGet]
        public ActionResult DetailsOfTransporter(string email)
        {
            int tid = adminrepo.GetIdFromEmail<Transporter>(email);
            Transporter detailOfTransporter = trepo.Get(tid);
            ViewBag.success = TempData["success"];
            return View(detailOfTransporter);
        }

        [HttpGet]
        public ActionResult EditTransporter(string email)
        {
            int tid = adminrepo.GetIdFromEmail<Transporter>(email);
            Transporter detailOfTransporter = trepo.Get(tid);

            ViewBag.dobDay = Convert.ToInt32(detailOfTransporter.DateOfBirth.Split('/')[0]);
            ViewBag.dobMonth = Convert.ToInt32(detailOfTransporter.DateOfBirth.Split('/')[1]);
            ViewBag.dobYear = Convert.ToInt32(detailOfTransporter.DateOfBirth.Split('/')[2]);

            EditProfileModel edituser = new EditProfileModel();
            edituser.Email = detailOfTransporter.Email;
            edituser.Name = detailOfTransporter.Name;
            edituser.Status = detailOfTransporter.Status;
            edituser.Gender = detailOfTransporter.Gender;

            return View(edituser);
        }

        [HttpGet]
        public ActionResult DeleteTransporter(string email)
        {
            int tid = adminrepo.GetIdFromEmail<Transporter>(email);
            Transporter detailOfTransporter = trepo.Get(tid);
            ViewBag.success = TempData["success"];
            return View(detailOfTransporter);
        }

        [HttpPost]
        [ActionName("DeleteTransporter")]
        public ActionResult DeleteTransporterPost(string email)
        {
            int tid = adminrepo.GetIdFromEmail<Transporter>(email);
            Transporter transporter = trepo.Get(tid);
            trepo.Delete(transporter);
            TempData["success"] = "User deleted successfully";
            return RedirectToAction("SearchTransporter", new { id = 0 });
        }


        [HttpGet]
        public ActionResult SearchTransporter(int id=0)
        {
            ViewBag.success = TempData["success"];
            if (id == 1)
            {
                string filter = TempData["filter"].ToString();
                if (filter == "name")
                {
                    string searchname = TempData["search"].ToString();
                    ViewBag.tlist = adminrepo.GetByName<Transporter>(searchname);
                }
                else if (filter == "email")
                {
                    string searchemail = TempData["search"].ToString();
                    ViewBag.tlist = adminrepo.GetByEmail<Transporter>(searchemail);
                }
                else if (filter == "status")
                {
                    string searchstatus = TempData["search"].ToString();
                    ViewBag.tlist = adminrepo.GetByStatus<Transporter>(searchstatus);
                }
                else
                {
                    ViewBag.tlist = trepo.GetAll();
                }
            }
            else
            {
                ViewBag.tlist = trepo.GetAll();

            }
            return View();
        }


        [HttpPost]
        public ActionResult EditTransporter(EditProfileModel user, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                int tid = adminrepo.GetIdFromEmail<Transporter>(user.Email);
                Transporter transporterToEdit = trepo.Get(tid);
                transporterToEdit.Name = user.Name;
                transporterToEdit.Gender = user.Gender;
                transporterToEdit.Status = user.Status;

                string year = form["dobYear"];
                string month = form["dobMonth"];
                string day = form["dobDay"];

                transporterToEdit.DateOfBirth = day + "/" + month + "/" + year;

                trepo.Update(transporterToEdit, transporterToEdit.Id);

                TempData["success"] = "User Edited Successfully";

                return RedirectToAction("SearchTransporter", "Admin");
            }

            return View(user);

        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(EditProfileModel createuser, FormCollection form)
        {
            if (ModelState.IsValid)
            {

                Repository<User> repo = new Repository<User>();
                User user = repo.Get(createuser.Email);
                if (user == null)
                {
                    user = new User();
                    user.Password = "123";
                    user.Email = createuser.Email;
                    user.Role = "Customer";
                    user.Id = repo.dbContext.Users.Count() + 1;
                    repo.Insert(user);
                    //repo.dbContext.Dispose();

                    Customer cust = new Customer();
                    cust.LastOnline = DateTime.Now;
                    cust.Name = createuser.Name;
                    cust.Email = createuser.Email;
                    cust.Id = user.Id;
                    cust.Password = "123";
                    cust.Gender = createuser.Gender;
                    string year = form["dobYear"];
                    string month = form["dobMonth"];
                    string day = form["dobDay"];
                    cust.DateOfBirth = day + "/" + month + "/" + year;
                    cust.Status = createuser.Status;
                    custrepo.Insert(cust);

                    TempData["success"] = "User created successfully";
                    return RedirectToAction("SearchUser", new { id = 0 });
                }
                else ModelState.AddModelError("UserExists", "A user with this e-mail address already exists!");
                return View();
            }
            return View(createuser);
        }

        [HttpGet]
        public ActionResult CreateRestaurant()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRestaurant(RestaurantModel restaurant)
        {
            if (ModelState.IsValid)
            {
                Repository<User> repo = new Repository<User>();
                User user = repo.Get(restaurant.Email);
                if (user == null)
                {
                    user = new User();
                    user.Password = "123";
                    user.Email = restaurant.Email;
                    user.Role = "Restaurant";
                    user.Id = repo.dbContext.Users.Count() + 1;
                    repo.Insert(user);
                    //repo.dbContext.Dispose();
                    Restaurant res = new Restaurant();
                    res.LastOnline = DateTime.Now;
                    res.Email = restaurant.Email;
                    res.Id = user.Id;
                    res.Name = restaurant.Name;
                    res.Password = "123";
                    res.Status = restaurant.Status;
                    restrepo.Insert(res);
                    RestaurantAddress restAdd = new RestaurantAddress();
                    restAdd.RestaurantId = user.Id;
                    restAddService.Insert(restAdd);
                    TempData["success"] = "Restaurant created successfully";
                    return RedirectToAction("SearchRestaurant", new { id = 0 });
                }
                else ModelState.AddModelError("UserExists", "A user with this e-mail address already exists!");
                return View();
            }
            return View(restaurant);
        }

        [HttpGet]
        public ActionResult CreateTransporter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateTransporter(EditProfileModel createtransporter, FormCollection form)
        {
            if (ModelState.IsValid)
            {

                Repository<User> repo = new Repository<User>();
                User user = repo.Get(createtransporter.Email);
                if (user == null)
                {
                    user = new User();
                    user.Password = "123";
                    user.Email = createtransporter.Email;
                    user.Role = "Transporter";
                    user.Id = repo.dbContext.Users.Count() + 1;
                    repo.Insert(user);
                    //repo.dbContext.Dispose();

                    Transporter transporter = new Transporter();
                    transporter.LastOnline = DateTime.Now;
                    transporter.Name = createtransporter.Name;
                    transporter.Email = createtransporter.Email;
                    transporter.Id = user.Id;
                    transporter.Password = "123";
                    transporter.Gender = createtransporter.Gender;
                    string year = form["dobYear"];
                    string month = form["dobMonth"];
                    string day = form["dobDay"];
                    transporter.DateOfBirth = day + "/" + month + "/" + year;
                    transporter.Status = createtransporter.Status;
                    trepo.Insert(transporter);

                    TempData["success"] = "User created successfully";
                    return RedirectToAction("SearchTransporter", new { id = 0 });
                }
                else ModelState.AddModelError("UserExists", "A user with this e-mail address already exists!");
                return View();
            }
            return View(createtransporter);
        }

        [HttpGet]
        public ActionResult SearchAdmin(int id = 0)
        {
            ViewBag.success = TempData["success"];
            if (id == 1)
            {
                string filter = TempData["filter"].ToString();
                if (filter == "name")
                {
                    string searchname = TempData["search"].ToString();
                    ViewBag.adminlist = adminrepo.GetByName<Admin>(searchname);
                }
                else if (filter == "email")
                {
                    string searchemail = TempData["search"].ToString();
                    ViewBag.adminlist = adminrepo.GetByEmail<Admin>(searchemail);
                }
                else if (filter == "status")
                {
                    string searchstatus = TempData["search"].ToString();
                    ViewBag.adminlist = adminrepo.GetByStatus<Admin>(searchstatus);
                }
                else
                {
                    ViewBag.adminlist = adminrepo.GetAll();
                }
            }
            else
            {
                ViewBag.adminlist = adminrepo.GetAll();
            }
            return View();
        }

        [HttpPost]
        public ActionResult SearchAdmin(string search, string filter)
        {
            TempData["search"] = search;
            TempData["filter"] = filter;
            return RedirectToAction("SearchAdmin", new { id = 1 });
        }

        [HttpGet]
        public ActionResult DetailsOfAdmin(string email)
        {
            int adminid = adminrepo.GetIdFromEmail<Admin>(email);
            Admin detailOfAdmin = adminrepo.Get(adminid);
            ViewBag.success = TempData["success"];
            return View(detailOfAdmin);
        }

        [HttpGet]
        public ActionResult EditAdmin(string email)
        {
            int adminid = adminrepo.GetIdFromEmail<Admin>(email);
            Admin detailOfAdmin = adminrepo.Get(adminid);

            ViewBag.dobDay = Convert.ToInt32(detailOfAdmin.DateOfBirth.Split('/')[0]);
            ViewBag.dobMonth = Convert.ToInt32(detailOfAdmin.DateOfBirth.Split('/')[1]);
            ViewBag.dobYear = Convert.ToInt32(detailOfAdmin.DateOfBirth.Split('/')[2]);

            EditProfileModel edituser = new EditProfileModel();
            edituser.Email = detailOfAdmin.Email;
            edituser.Name = detailOfAdmin.Name;
            edituser.Status = detailOfAdmin.Status;
            edituser.Gender = detailOfAdmin.Gender;

            return View(edituser);
        }

        [HttpPost]
        public ActionResult EditAdmin(EditProfileModel user, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                int adminid = adminrepo.GetIdFromEmail<Admin>(user.Email);
                Admin userToEdit = adminrepo.Get(adminid);
                userToEdit.Name = user.Name;
                userToEdit.Gender = user.Gender;
                userToEdit.Status = user.Status;

                string year = form["dobYear"];
                string month = form["dobMonth"];
                string day = form["dobDay"];

                userToEdit.DateOfBirth = day + "/" + month + "/" + year;

                adminrepo.Update(userToEdit, userToEdit.Id);

                TempData["success"] = "User Edited Successfully";

                return RedirectToAction("SearchAdmin", "Admin");
            }

            return View(user);

        }

        [HttpGet]
        public ActionResult DeleteAdmin(string email)
        {
            int adminid = adminrepo.GetIdFromEmail<Admin>(email);
            Admin detailOfAdmin = adminrepo.Get(adminid);
            ViewBag.success = TempData["success"];
            return View(detailOfAdmin);
        }

        [HttpPost]
        [ActionName("DeleteAdmin")]
        public ActionResult DeleteAdminPost(string email)
        {
            int adminid = adminrepo.GetIdFromEmail<Admin>(email);
            Admin admin = adminrepo.Get(adminid);
            adminrepo.Delete(admin);
            TempData["success"] = "User deleted successfully";
            return RedirectToAction("SearchAdmin", new { id = 0 });
        }

        [HttpGet]
        public ActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAdmin(EditProfileModel admin, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                Repository<User> repo = new Repository<User>();
                User user = repo.Get(admin.Email);
                if (user == null)
                {
                    user = new User();
                    user.Password = "123";
                    user.Email = admin.Email;
                    user.Role = "Admin";
                    user.Id = repo.dbContext.Users.Count() + 1;
                    repo.Insert(user);
                    //repo.dbContext.Dispose();
                    Admin res = new Admin();
                    res.LastOnline = DateTime.Now;
                    res.Name = admin.Name;
                    res.Email = admin.Email;
                    res.Id = user.Id;
                    res.Password = "123";
                    res.Gender = admin.Gender;
                    string year = form["dobYear"];
                    string month = form["dobMonth"];
                    string day = form["dobDay"];
                    res.DateOfBirth = day + "/" + month + "/" + year;
                    res.Status = admin.Status;
                    adminrepo.Insert(res);
                    TempData["success"] = "Admin created successfully";
                    return RedirectToAction("SearchAdmin", new { id = 0 });
                }
                else ModelState.AddModelError("UserExists", "A user with this e-mail address already exists!");
                return View();
            }
            return View(admin);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["loggedInUser"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Public");
        }

        [HttpGet]
        public ActionResult StatisticsReport()
        {
            int adminNumber = 0;
            int customerNumber = 0;
            int restaurantNumber = 0;
            int transporterNumber = 0;

            Repository<User> userRepo = new Repository<Entity.User>();

            List<User> users = userRepo.GetAll() as List<User>;

            //var adminNo = userRepo.dbContext.Users.Where(admin => admin.Role=="Admin");

            foreach (var userItem in users)
            {
                if (userItem.Role == "Admin") adminNumber++;
                else if (userItem.Role == "Customer") customerNumber++;
                else if (userItem.Role == "Restaurant") restaurantNumber++;
                else if (userItem.Role == "Transporter") transporterNumber++;
            }
            ViewBag.adminNo = adminNumber;
            ViewBag.customerNo = customerNumber;
            ViewBag.transporterNo = transporterNumber;
            ViewBag.restuarantNo = restaurantNumber;
            ViewBag.userNo = adminNumber + customerNumber + transporterNumber;

            Session["adminNumber"] = adminNumber;
            Session["customerNumber"] = customerNumber;
            Session["transporterNumber"] = transporterNumber;
            Session["restaurantNumber"] = restaurantNumber;

            return View("StatisticsReport");
        }

        public ActionResult StatisticsChart()
        {
            new Chart(width: 800, height: 200).AddSeries(
            chartType: "column",
            xValue: new[] { "Admin", "Customer", "Transporter", "Restaurant" },
            yValues: new[] { Session["adminNumber"], Session["customerNumber"], Session["transporterNumber"], Session["restaurantNumber"] }).Write("png");
            return null;
        }

        [HttpGet]
        public ActionResult MembershipReport()
        {
            return View("MembershipReport");
        }


        [HttpGet]
        public ActionResult ActivityReport()
        {
            int inactiveUser3 = 0;
            int inactiveUser6 = 0;
            int inactiveAdmin3 = 0;
            int inactiveAdmin6 = 0;
            int inactiveRest3 = 0;
            int inactiveRest6 = 0;
            int inactiveTrans3 = 0;
            int inactiveTrans6 = 0;
            //Inactive User
            int count = 0;

            List<Customer> show = custrepo.GetAll() as List<Customer>;

            var currentTime = DateTime.Now;
            Customer user = new Customer();

            var inactiveUser3Months = from showuser in show
                                      where (currentTime - showuser.LastOnline).Days > 90 && (currentTime - showuser.LastOnline).Days < 180
                                      select showuser;

            ViewBag.show3Months = inactiveUser3Months;
            ViewBag.lastOnline3Months = (currentTime - user.LastOnline).Days;

            foreach (var total in inactiveUser3Months)
            {
                count++;
                ViewBag.inactiveUser3Months = count;
                inactiveUser3 = count;
            }

            var inactiveUser6Months = from showuser in show
                                      where (currentTime - showuser.LastOnline).Days > 180
                                      select showuser;

            ViewBag.show6Months = inactiveUser6Months;
            ViewBag.lastOnline6Months = (currentTime - user.LastOnline).Days;

            foreach (var total in inactiveUser6Months)
            {
                count++;
                ViewBag.inactiveUser6Months = count;
                inactiveUser6 = count;
            }
            Session["InactiveUser"] = inactiveUser3 + inactiveUser6;
            //Inactive admin
            int countforadmin = 0;

            List<Admin> showadm = adminrepo.GetAll() as List<Admin>;

            Admin admin = new Admin();

            var inactiveAdmin3Months = from showadmin in showadm
                                       where (currentTime - showadmin.LastOnline).Days > 90 && (currentTime - showadmin.LastOnline).Days < 180
                                       select showadmin;

            ViewBag.show3MonthsInactiveAdmin = inactiveAdmin3Months;
            ViewBag.lastOnline3MonthsInactiveAdmin = (currentTime - admin.LastOnline).Days;

            foreach (var total in inactiveAdmin3Months)
            {
                countforadmin++;
                ViewBag.inactiveAdmin3Months = countforadmin;
                inactiveAdmin3 = countforadmin;
            }

            var inactiveAdmin6Months = from showadmin in showadm
                                       where (currentTime - showadmin.LastOnline).Days > 180
                                       select showadmin;

            ViewBag.show6MonthsInactiveAdmin = inactiveAdmin6Months;
            ViewBag.lastOnline6MonthsInactiveAdmin = (currentTime - admin.LastOnline).Days;

            foreach (var total in inactiveAdmin6Months)
            {
                countforadmin++;
                ViewBag.inactiveAdmin6Months = countforadmin;
                inactiveAdmin6 = countforadmin;
            }
            Session["InactiveAdmin"] = inactiveAdmin3 + inactiveAdmin6;
            //Inactive admin
            int countfortransporter = 0;

            List<Transporter> showtrans = trepo.GetAll() as List<Transporter>;

            Transporter transporter = new Transporter();

            var inactiveTransporter3Months = from showtransporter in showtrans
                                             where (currentTime - showtransporter.LastOnline).Days > 90 && (currentTime - showtransporter.LastOnline).Days < 180
                                             select showtransporter;

            ViewBag.show3MonthsInactiveTransporter = inactiveTransporter3Months;
            ViewBag.lastOnline3MonthsInactiveTransporter = (currentTime - transporter.LastOnline).Days;

            foreach (var total in inactiveTransporter3Months)
            {
                countfortransporter++;
                ViewBag.inactiveTransporter3Months = countfortransporter;
                inactiveTrans3 = countfortransporter;
            }

            var inactiveTransporter6Months = from showtransporter in showtrans
                                             where (currentTime - showtransporter.LastOnline).Days > 180
                                             select showtransporter;

            ViewBag.show6MonthsInactiveTransporter = inactiveTransporter6Months;
            ViewBag.lastOnline6MonthsInactiveTransporter = (currentTime - transporter.LastOnline).Days;

            foreach (var total in inactiveTransporter6Months)
            {
                countfortransporter++;
                ViewBag.inactiveTransporter6Months = countfortransporter;
                inactiveTrans6 = countfortransporter;
            }
            Session["inactiveTrans"] = inactiveTrans3 + inactiveTrans6;
            //Inactive Restuarant
            int countforrestuarant = 0;

            List<Restaurant> showrest = restrepo.GetAll() as List<Restaurant>;

            Restaurant restuarant = new Restaurant();

            var inactiveRestuarant3Months = from showrestuarant in showrest
                                            where (currentTime - showrestuarant.LastOnline).Days > 90 && (currentTime - showrestuarant.LastOnline).Days < 180
                                            select showrestuarant;

            ViewBag.show3MonthsInactiveRestuarant = inactiveRestuarant3Months;
            ViewBag.lastOnline3MonthsInactiveResttuarant = (currentTime - restuarant.LastOnline).Days;

            foreach (var total in inactiveRestuarant3Months)
            {
                countforrestuarant++;
                ViewBag.inactiveRestuarant3Months = countforrestuarant;
                inactiveRest3 = countforrestuarant;
            }

            var inactiveRestuarant6Months = from showrestuarant in showrest
                                            where (currentTime - showrestuarant.LastOnline).Days > 180
                                            select showrestuarant;

            ViewBag.show6MonthsInactiveRestuarant = inactiveRestuarant6Months;
            ViewBag.lastOnline6MonthsInactiveRestuarant = (currentTime - restuarant.LastOnline).Days;

            foreach (var total in inactiveRestuarant6Months)
            {
                countforrestuarant++;
                ViewBag.inactiveRestuarant6Months = countforrestuarant;
                inactiveRest6 = countforrestuarant;
            }
            Session["inactiveRest"] = inactiveRest3 + inactiveRest6;
            return View("ActivityReport");
        }

        public ActionResult ActivityChart()
        {
            new Chart(width: 800, height: 200)
            .AddTitle("Inactive Users")
            .AddSeries(
            chartType: "Bar",
            xValue: new[] { "Admin", "Customer", "Transporter", "Restaurant" },
            yValues: new[] { Session["inactiveAdmin"], Session["inactiveUser"], Session["inactiveTrans"], Session["inactiveRest"] }).Write("png");

            return null;
        }

        public ActionResult Home()
        {
            return RedirectToAction("Index");
        }

        public List<InvoiceModel> PrepareInvoices(List<Invoice> invoices)
        {
            List<InvoiceModel> invModels = new List<InvoiceModel>();
            InvoiceModel invMod;

            foreach (Invoice inv in invoices)
            {
                invMod = new InvoiceModel();
                invMod.InvoiceNo = inv.Id;
                invMod.OrderOwner = custrepo.Get(inv.CustomerId).Name;
                invMod.OrderTime = inv.CheckOutTime.ToString("dd/MM/yyyy hh:mm tt");
                invMod.Bill = inv.Bill;
                invMod.Status = inv.Status;
                invModels.Add(invMod);
            }

            return invModels;
        }

    }


}
