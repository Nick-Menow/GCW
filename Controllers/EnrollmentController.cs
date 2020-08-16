using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WorkDB.DAL;
using WorkDB.Models;

namespace WorkDB.Controllers
{
    public class EnrollmentController : Controller
    {

        private TournamentContext db = new TournamentContext();

        // GET: Enrollment
        public ActionResult Index()
        {
            var enrollments = db.Enrollments.Include(e => e.Player).Include(e => e.Tournament);
            return View(enrollments.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        public ActionResult Create(int? id)
        {

            ViewBag.PlayerID = new SelectList(db.Players, "ID", "LastName");
            ViewBag.TournamentID = new SelectList(db.Tournaments, "TournamentID", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EnrollmentID,TournamentID,PlayerID,Grade")] Enrollment model)
        {
            if (ModelState.IsValid)
            {
                string name = User.Identity.Name;
                var id_player = db.Players.FirstOrDefault(x => x.NickName == name).ID;
                Enrollment user = null;
                using (TournamentContext db = new TournamentContext())
                {
                    user = db.Enrollments.FirstOrDefault(u => u.PlayerID == id_player && u.TournamentID == model.TournamentID);
                }
                if (user == null)
                {
                    db.Enrollments.Add(entity: new Enrollment { TournamentID = model.TournamentID, PlayerID = id_player, Grade = 0 });
                    db.SaveChanges();
                    return RedirectToAction("Index", "Tournaments");
                }
                else
                {
                    ModelState.AddModelError("", "Вы уже заявились на турнир");                    
                    return View(model);
                }
            }
            ViewBag.PlayerID = new SelectList(db.Players, "ID", "LastName", model.PlayerID);
            ViewBag.TournamentID = new SelectList(db.Tournaments, "TournamentID", "Title", model.TournamentID);
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlayerID = new SelectList(db.Players, "ID", "LastName", enrollment.PlayerID);
            ViewBag.TournamentID = new SelectList(db.Tournaments, "TournamentID", "Title", enrollment.TournamentID);
            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EnrollmentID,TournamentID,PlayerID,Grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enrollment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlayerID = new SelectList(db.Players, "ID", "LastName", enrollment.PlayerID);
            ViewBag.TournamentID = new SelectList(db.Tournaments, "TournamentID", "Title", enrollment.TournamentID);
            return View(enrollment);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enrollment enrollment = db.Enrollments.Find(id);
            db.Enrollments.Remove(enrollment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
