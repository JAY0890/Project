using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
namespace Project.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        //private ProjectEntities db = new ProjectEntities();
        private CarEntities2 db=new CarEntities2();

        public ActionResult Registration(int id=0)
        {
            User userModel = new User();

            return View(userModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(User userModel)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = db.Users.FirstOrDefault(u => u.Email == userModel.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(userModel);
                }

                // Save to database
                db.Users.Add(userModel);
                db.SaveChanges();

                // Redirect or show success message
                return RedirectToAction("Login");
            }

            // If validation fails, return the same view with errors
            return View(userModel);
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string Password)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ViewBag.ErrorMessage = "Please enter both email and password.";
                return View();
            }

            var user = db.Users
                         .FirstOrDefault(u => u.Email == Email && u.Password == Password);

            if (user != null)
            {
                // Store session values
                Session["UserId"] = user.Id;
                Session["UserName"] = user.Name;
                Session["UserType"] = user.Type;

                // Redirect based on Type
                if (user.Type == true) // Buyer
                {
                    return RedirectToAction("Index","Buyer");
                }
                else // Seller
                {
                    return RedirectToAction("Index","Seller");
                }
            }

            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();
        }

        public ActionResult Buyer()
        {
            return View();
        }

        public ActionResult Seller()
        {
            return View();
        }
    }
}