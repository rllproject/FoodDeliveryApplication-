using FoodOrderSystem.Database;
using FoodOrderSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace FoodOrderSystem.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        FoodOrderSystemEntities db = new FoodOrderSystemEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                return RedirectToAction("Index", "ProductDashbord");
            }
            else
            {
                return View();

            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(SignupLogin model)
        {
            var data = db.Users.Where(s => s.Email.Equals(model.Email) && s.Password.Equals(model.Password)).ToList();
            if (data.Count() > 0)
            {
                Session["uid"] = data.FirstOrDefault().ID;
                HttpCookie cookies = new HttpCookie("UserInfo");
                cookies.Values["idUser"] = Convert.ToString(data.FirstOrDefault().ID);
                cookies.Values["FullName"] = Convert.ToString(data.FirstOrDefault().Name);
                cookies.Values["Email"] = Convert.ToString(data.FirstOrDefault().Email);
                cookies.Values["IsAdmin"] = Convert.ToString(data.FirstOrDefault().IsAdmin);
                cookies.Expires = DateTime.Now.AddMonths(1);
                Response.Cookies.Add(cookies);
                if (data.FirstOrDefault().IsAdmin)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "ProductDashbord");
                }
            }
            else
            {
                ViewBag.Message = "Login failed";
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout()
        {

            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("UserInfo"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["UserInfo"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult ForgetPassword()
        {

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPassword(SignupLogin signup)
        {

            var isEmailAlreadyExists = db.Users.FirstOrDefault(x => x.Email == signup.Email);
            if (isEmailAlreadyExists == null)
            {
                ViewBag.Message = "User Not Registerd";
                return View();
            }
            else
            {
                SendMail(isEmailAlreadyExists.Email, isEmailAlreadyExists.Password);
                ViewBag.Message = "Email Sent to Register Email Address";
                return RedirectToAction("Login", "Login");
            }

            return View();
        }
        void SendMail(string toemail, string password)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("nemipes96@gmail.com");
                message.To.Add(new MailAddress(toemail));
                message.Subject = "Food Order System Login Password";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = "your Password Is : " + password;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new NetworkCredential("nemipes96@gmail.com", "kpkwpegosfttvoxv");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex)
            {

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(SignupLogin signup)
        {
            if (ModelState.IsValid)
            {
                var isEmailAlreadyExists = db.Users.Any(x => x.Email == signup.Email);
                if (isEmailAlreadyExists)
                {
                    ViewBag.Message = "Email Already Registered. Please Try Again With Another Email";
                    return View();
                }
                else
                {
                    db.Users.Add(new Database.User
                    {
                        Email = signup.Email,
                        IsAdmin = false,
                        MobileNo = signup.MobileNo,
                        Name = signup.Name,
                        Password = signup.Password,
                    });
                    db.SaveChanges();
                    return RedirectToAction("Index", "ProductDashbord");
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult Signup()
        {
            var userInCookie = Request.Cookies["UserInfo"];
            if (userInCookie != null)
            {
                return RedirectToAction("Index", "ProductDashbord");
            }
            else
            {
                return View();
            }
        }
    }
}