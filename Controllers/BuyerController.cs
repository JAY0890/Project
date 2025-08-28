using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class BuyerController : Controller
    {
        private CarEntities2 db = new CarEntities2();

        // GET: Buyer/Index - Show all cars
        public ActionResult Index()
        {
            var cars = db.Cars.ToList();
            return View(cars);
        }

        // GET: Buyer/Details/5 - Show car details
        public ActionResult Details(int id)
        {
            var car = db.Cars
                .Include("CarImages")   // ✅ EF6 string-based Include
                .FirstOrDefault(c => c.Id == id);

            if (car == null)
                return HttpNotFound();

            return View(car);
        }
        // Action to fetch image by Id
        public FileContentResult GetImage(int id)
        {
            var image = db.CarImages.FirstOrDefault(i => i.Id == id);

            if (image != null && image.Image != null)
            {
                return File(image.Image, "image/jpeg"); // you can adjust type based on your uploads
            }

            return null;
        }
    }
}