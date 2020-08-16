using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkDB.Models;
using System.IO;

namespace WorkDB.Controllers
{
    public class AvatarController : Controller
    {
        DAL.TournamentContext db = new DAL.TournamentContext();
        // GET: Avatar
        public ActionResult Index()
        {
            
            return View(db.Avatars);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Avatar pic, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid && uploadImage != null)
            {
                byte[] imageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
                // установка массива байтов
                pic.Image = imageData;

                db.Avatars.Add(pic);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(pic);
        }
    }
}