using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security;
using System.Web.Mvc;
using WorkDB.DAL;
using WorkDB.Models;
using Microsoft.AspNet.Identity;


namespace WorkDB.Controllers
{
    public class TournamentsController : Controller
    {

        private TournamentContext db = new TournamentContext();

        // GET: Tournaments
        public ActionResult Index()
        {
            var tournaments = db.Tournaments.Include(e => e.Category);
            return View(tournaments.ToList());
        }
        public ActionResult Game(int? id, int id_tour)
        {
            int check = db.Tournaments.Single(x => x.TournamentID == id_tour).played;
            if (check == 1)
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.ID_tour = id_tour;
            int id_cat = db.Categories.Find(id).Id;
            int id_que = db.Questions.FirstOrDefault(x => x.CategoryID == id_cat).Id;

            Question question = db.Questions.Find(id_que);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }
        public ActionResult Game_2(int? id, int id_tour)
        {
            ViewBag.ID_tour = id_tour;
            int tournament = id_tour;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return RedirectToAction("End", new { id = tournament });
            }            
            int id_check = db.Questions.Find(id).CategoryID;
            int id_prev = db.Questions.Find(id - 1).CategoryID;            
            if (id_check != id_prev)
            {
                return RedirectToAction("End", new { id = tournament });
            }
            return View(question);
        }

        public ActionResult End(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            db.Tournaments.Find(id).played = 1;
            db.SaveChanges();
            return View(tournament);
        }
        public ActionResult Edit_End(int? id)
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
        public ActionResult Edit_End([Bind(Include = "EnrollmentID,TournamentID,PlayerID,Grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enrollment).State = EntityState.Modified;
                db.SaveChanges();
                int counter = db.Enrollments.FirstOrDefault(x => x.TournamentID == enrollment.TournamentID && x.PlayerID == enrollment.PlayerID).TournamentID;
                return RedirectToAction("End", new { id = counter });
            }
            ViewBag.PlayerID = new SelectList(db.Players, "ID", "LastName", enrollment.PlayerID);
            ViewBag.TournamentID = new SelectList(db.Tournaments, "TournamentID", "Title", enrollment.TournamentID);
            return View(enrollment);
        }

        // GET: Tournaments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }



        // GET: Tournaments/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Tournaments/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TournamentID,Title,Country,CategoryID,Date,Post")] Tournament tournament)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    db.Tournaments.Add(tournament);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "Name", tournament.CategoryID);
            return View(tournament);
        }

        // GET: Tournaments/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // POST: Tournaments/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tournamentToUpdate = db.Tournaments.Find(id);
            if (TryUpdateModel(tournamentToUpdate, "", new string[] { "Title", "Country" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }

            }
            return View(tournamentToUpdate);
        }

        // GET: Tournaments/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Tournament tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // POST: Tournaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var item = db.Tournaments.Single(x => x.TournamentID == id);
                db.Tournaments.Remove(item);
                db.SaveChanges();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

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
