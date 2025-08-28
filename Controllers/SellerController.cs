using Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class SellerController : Controller
    {
        private CarEntities2 db = new CarEntities2();

        // Helper method to get logged-in user id
        private int GetCurrentUserId()
        {
            //store user id in session after login
            return (int)Session["UserId"];
        }

        // GET: Seller/Index -> Show cars owned by seller
        public ActionResult Index()
        {
            int userId = GetCurrentUserId();
            var myCars = db.Cars.Where(c => c.User_id == userId).ToList();
            return View(myCars);
        }

        // GET: Seller/Details/5
        public ActionResult Details(int id)
        {
            int userId = GetCurrentUserId();
            var car = db.Cars.FirstOrDefault(c => c.Id == id && c.User_id == userId);

            if (car == null)
                return HttpNotFound();

            return View(car);
        }

        // GET: Seller/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Seller/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Car car, IEnumerable<HttpPostedFileBase> files)
        {
            // Validate uploaded files first (before inserting car)
            var validFiles = files?.Where(f => f != null && f.ContentLength > 0).ToList() ?? new List<HttpPostedFileBase>();
            if (validFiles.Count < 1 || validFiles.Count > 5)
            {
                ModelState.AddModelError("files", "Please upload between 1 and 5 images.");
                return View(car);
            }

            if (ModelState.IsValid)
            {
                car.User_id = GetCurrentUserId();
                db.Cars.Add(car);
                db.SaveChanges();  // save car to generate CarId

                // Save images
                foreach (var file in validFiles)
                {
                    using (var br = new System.IO.BinaryReader(file.InputStream))
                    {
                        var carImg = new CarImage
                        {
                            CarId = car.Id,
                            Filename = Path.GetFileName(file.FileName),
                            Image = br.ReadBytes(file.ContentLength)
                        };
                        db.CarImages.Add(carImg);
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(car);
        }



        // GET: Seller/Edit/5
        public ActionResult Edit(int id)
        {
            int userId = GetCurrentUserId();
            var car = db.Cars.FirstOrDefault(c => c.Id == id && c.User_id == userId);

            if (car == null)
                return HttpNotFound();

            return View(car);
        }

        // POST: Seller/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Car car)
        {
            int userId = GetCurrentUserId();
            var existingCar = db.Cars.FirstOrDefault(c => c.Id == car.Id && c.User_id == userId);

            if (existingCar == null)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                // Update only allowed fields
                existingCar.Name = car.Name;
                existingCar.Model = car.Model;
                existingCar.Brand = car.Brand;
                existingCar.Year = car.Year;
                existingCar.Price = car.Price;
                existingCar.Mileage = car.Mileage;
                existingCar.Fuel_type = car.Fuel_type;
                existingCar.Transmission = car.Transmission;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(car);
        }

        // GET: Seller/Delete/5
        // GET: Seller/Delete/5
        public ActionResult Delete(int id)
        {
            var car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Seller/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int userId = GetCurrentUserId();

            var car = db.Cars.FirstOrDefault(c => c.Id == id && c.User_id == userId);

            if (car == null)
                return HttpNotFound();

            // Remove related images first
            var images = db.CarImages.Where(i => i.CarId == car.Id).ToList();
            foreach (var img in images)
            {
                db.CarImages.Remove(img);
            }

            // Now remove the car
            db.Cars.Remove(car);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}