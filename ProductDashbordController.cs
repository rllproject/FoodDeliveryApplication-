using FoodOrderSystem.Database;
using FoodOrderSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace FoodOrderSystem.Controllers
{
    public class ProductDashbordController : Controller
    {
        private FoodOrderSystemEntities db = new FoodOrderSystemEntities();
        // GET: ProductDashbord
        public ActionResult Index()
        {
            var products = db.Products;

            return View(products.ToList());
        }

        public ActionResult Search(string str)
        {
            var products = db.Products.Include(o => o.Restorent).Where(x => x.Restorent.City == str);
            return View("Index", products.ToList());
        }

        public ActionResult addToCart(int? Id)
        {

            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                // List<Products> lst = new List<Products>();
                Product products = db.Products.FirstOrDefault(x => x.ID == Id);
                //foreach (var item in products)
                //{
                //    lst.Add(new Products
                //    {
                //        id=item.ID,
                //        ProductName=item.ProductName,
                //        ProductPicture=item.ProductPicture,
                //        ProductPrissssce=double.Parse(item.ProductPrice.ToString())
                //    });
                //}
                return View(products);

            }
            else
            {
                return RedirectToAction("Login", "User");
            }


        }

        public ActionResult PreviousOrderHistery()
        {
            var userInCookie = Request.Cookies["UserInfo"];
            int iduser = Convert.ToInt32(userInCookie["idUser"]);
            var orders = db.Orders.Include(o => o.Restorent).Include(o => o.User).Where(x => x.UserID == iduser);
            return View("PreviousOrderHistery", orders.ToList());
        }
        public ActionResult Rating(int id, int rate)
        {
            var userInCookie = Request.Cookies["UserInfo"];
            int iduser = Convert.ToInt32(userInCookie["idUser"]);
            var orders = db.Orders.FirstOrDefault(x => x.ID == id);
            orders.Rating = rate;
            db.SaveChanges();
            var orders1 = db.Orders.Include(o => o.Restorent).Include(o => o.User).Where(x => x.UserID == iduser);
            return View("PreviousOrderHistery", orders1.ToList());
        }
        List<Cart> li = new List<Cart>();

        [HttpPost]
        public ActionResult addToCart(int Id, string number)
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                FoodOrderSystem.Database.Product products = db.Products.FirstOrDefault(x => x.ID == Id);
                Cart cart = new Cart();
                cart.productId = products.ID;
                cart.productName = products.ProductName;
                cart.productPic = products.ProductPicture;
                cart.price = products.ProductPrice;
                cart.qty = Convert.ToInt32(number);
                cart.bill = cart.price * cart.qty;
                if (TempData["cart"] == null)
                {
                    li.Add(cart);
                    TempData["cart"] = li;
                }
                else
                {
                    //List<Cart> li2 = TempData["cart"] as List<Cart>;
                    //li2.Add(cart);
                    //TempData["cart"] = li2;
                    List<Cart> li2 = TempData["cart"] as List<Cart>;
                    int flag = 0;
                    foreach (var item in li2)
                    {
                        if (item.productId == cart.productId)
                        {
                            item.qty += cart.qty;
                            item.bill += cart.bill;
                            flag = 1;
                        }
                    }
                    if (flag == 0)
                    {
                        li2.Add(cart);
                    }
                    TempData["cart"] = li2;
                }

                TempData.Keep();
                return RedirectToAction("Index");

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult Checkout()
        {

            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                TempData.Keep();
                if (TempData["cart"] != null)
                {
                    decimal x = 0;
                    List<Cart> li2 = TempData["cart"] as List<Cart>;
                    foreach (var item in li2)
                    {
                        x += item.bill;
                    }
                    TempData["total"] = x;
                }
                TempData.Keep();
                return View();

            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }



        [HttpPost]
        public ActionResult Checkout(Order order)
        {

            var userInCookie = Request.Cookies["UserInfo"];
            int iduser = Convert.ToInt32(userInCookie["idUser"]);

            List<Cart> li = TempData["cart"] as List<Cart>;
            double discount = ((double)li.FirstOrDefault().bill / 100) * 10;
            var Isdiscount = db.Orders.Any(x => x.UserID == iduser);
            var user = db.Users.FirstOrDefault(x => x.ID == iduser);
            var resto = db.Restorents.FirstOrDefault(x => x.ID == order.ResotoId);
            
            if (Isdiscount)
            {
                discount = 0;
            }
            Invoice invoice = new Invoice();
            invoice.FKUserID = iduser;
            invoice.DateInvoice = System.DateTime.Now;
            invoice.Total_Bill = (double)li.FirstOrDefault().bill - discount;
            db.Invoices.Add(invoice);
            db.SaveChanges();
            foreach (var item in li)
            {
                var res = db.Products.FirstOrDefault(x => x.ID == item.productId);
                Order odr = new Order();
                odr.ProductId = item.productId;
                // odr. = invoice.ID;
                odr.OrderDate = System.DateTime.Now;
                odr.Item = item.qty;
                odr.Price = (int)item.price;
                odr.TotalPrice = (item.bill - (decimal)discount);
                odr.ResotoId = res.RestoID;
                odr.OrderStatus = "Ordered";
                odr.Rating = 0;
                var bill = (item.bill - (decimal)discount);
                order.DeliveryCharge = bill > 200 ? bill : bill + 30;
                odr.UserID = iduser;
                odr.UserID = db.Users.FirstOrDefault(x => x.ID == iduser).ID;
                odr.User = db.Users.FirstOrDefault(x => x.ID == iduser);
                odr.DeliverdDate = DateTime.Now.AddMinutes(30);
                odr.Discount = (decimal)discount;
                db.Orders.Add(odr);
                db.SaveChanges();
            }
            TempData.Remove("total");
            TempData.Remove("cart");
            TempData.Keep();
            return RedirectToAction("Index");
        }
        public ActionResult Remove(int? id)
        {
            List<Cart> li2 = TempData["cart"] as List<Cart>;
            Cart c = li2.Where(x => x.productId == id).SingleOrDefault();
            li2.Remove(c);
            decimal h = 0;
            foreach (var item in li2)
            {
                h += item.bill;
            }
            TempData["total"] = h;
            return RedirectToAction("Checkout");
        }
    }
}