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
    public class PlayerController : Controller
    {
        private TournamentContext db = new TournamentContext();

        // GET: Player
        public ActionResult Index(string sortOrder)
        {
            var players = from s in db.Players select s;
            switch (sortOrder)
            {
                default:
                    players = players.OrderByDescending(s => s.Score);
                    break;
            }
            Enrollment user = null;
            user = db.Enrollments.SingleOrDefault(c => c.PlayerID == db.Enrollments.Min(e => e.PlayerID));
            if (user is null)
            {
                ModelState.AddModelError("", "Заявки на турниры отсутствуют");
            }
            else
            {
                int? min_id = db.Enrollments.Min(e => e.PlayerID);
                int? max_id = db.Enrollments.Max(e => e.PlayerID);
                for (; min_id <= max_id; min_id++)
                {
                    Player player = db.Players.Find(min_id);
                    var sum = db.Enrollments.Where(e => e.PlayerID == min_id).Sum(x => x.Grade);
                    db.Players.Find(min_id).Score = sum;
                    db.Entry(player).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return View(players.Include(e=>e.Role).ToList());
        }

        // GET: Player/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", player.RoleId);
            return View(player);
        }

        // GET: Player/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Player/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,LastName,Firstname")] Player player)
        {
            if (ModelState.IsValid)
            {
                db.Players.Add(player);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(player);
        }

        // GET: Player/Edit/5
        public ActionResult Edit(int? id)
        {
            string name = db.Players.Single(x => x.ID == id).NickName;
            if (name != User.Identity.GetUserName())
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", player.RoleId);
            ViewBag.AvatarId = new SelectList(db.Avatars, "Id", "Name", player.AvatarId);
            return View(player);
        }

        // POST: Player/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,LastName,Firstname,AvatarId,RoleId,NickName,Team,Password,salt")] Player player)
        {
            if (ModelState.IsValid)
            {
                db.Entry(player).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name", player.RoleId);
            ViewBag.AvatarId = new SelectList(db.Avatars, "Id", "Name", player.AvatarId);
            return View(player);
        }

        // GET: Player/Delete/5
        public ActionResult Delete(int? id)
        {
            string name = db.Players.Single(x => x.ID == id).NickName;
            if (name != User.Identity.GetUserName())
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Player player = db.Players.Find(id);
            db.Players.Remove(player);
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
