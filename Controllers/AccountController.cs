using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WorkDB.Models;
using System.Security.Cryptography;


namespace WorkDB.Controllers
{
    public class AccountController : Controller
    {
        private DAL.TournamentContext db = new DAL.TournamentContext();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Данные введены неккоректно");
                return View(model);
            }

            using (DAL.TournamentContext db = new DAL.TournamentContext())
            {
                var player = db.Players.SingleOrDefault(u => u.NickName == model.Name);
                if (player is null)
                {
                    ModelState.AddModelError("", "Пользователь с таким логином не найден");
                    return View(model);
                }
                var salt1 = player.salt;
                var hmac = ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(model.Password), salt1);
                var pas = Convert.ToBase64String(hmac);
                var password = db.Players.SingleOrDefault(u => u.Password == pas);
                if (password is null)
                {
                    ModelState.AddModelError("", "Неправильный пароль");
                    return View(model);
                }
                var user = db.Players.FirstOrDefault(u => u.NickName == model.Name && u.Password == pas);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }
        private const int SaltSize = 16;

        public static byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[SaltSize];

                rng.GetBytes(randomNumber);

                return randomNumber;

            }
        }

        public static byte[] ComputeHMAC_SHA256(byte[] data, byte[] salt)
        {
            using (var hmac = new HMACSHA256(salt))
            {
                return hmac.ComputeHash(data);
            }
        }



        // GET: Account
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            var player = db.Players.SingleOrDefault(u => u.NickName == model.Name);
            if (player is null)
            {
                Player user = null;
                using (DAL.TournamentContext db = new DAL.TournamentContext())
                {
                    user = db.Players.FirstOrDefault(u => u.NickName == model.Name);
                }
                var salt1 = GenerateSalt();
                var hmac2 = ComputeHMAC_SHA256(Encoding.UTF8.GetBytes(model.Password), salt1);
                var sal1 = Convert.ToBase64String(salt1);
                var pas = Convert.ToBase64String(hmac2);
                if (user == null)
                {
                    using (DAL.TournamentContext db = new DAL.TournamentContext())
                    {
                        db.Players.Add(new Player
                        {
                            NickName = model.Name,
                            Password = pas,
                            salt = salt1,
                            Firstname = model.Firstname,
                            LastName = model.LastName,
                            Team = model.Team,
                            RoleId = 2,
                            AvatarId = 1
                        });
                        db.SaveChanges();
                        user = db.Players.Where(u => u.NickName == model.Name && u.Password == pas).FirstOrDefault();
                    }
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Name, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                return View(model);
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
    
}