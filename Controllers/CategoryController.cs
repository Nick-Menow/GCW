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
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace WorkDB.Controllers
{
    public class CategoryController : Controller
    {
        private TournamentContext db = new TournamentContext();

        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Post,Vocabulary")] Category category)

        {
            var name_cat = db.Categories.SingleOrDefault(x => x.Name == category.Name);
            if (name_cat is null)
            {
                //Question question = null;
                int counter_category;
                int? maximum = db.Categories.Max(e => e.Id);
                if (maximum is null)
                {
                    counter_category = 1;
                }
                else
                {
                    counter_category = db.Categories.Max(e => e.Id) + 1;
                }
                {
                    string voc = category.Vocabulary;
                    string[] words = voc.Split(',');
                    string que = null;
                    string ans = null;
                    string comment = null;
                    foreach (string s in words)
                    {
                        var html = @"https://db.chgk.info/search/questions/" + s + "/limit15";
                        HtmlWeb web = new HtmlWeb();
                        var htmlDoc = web.Load(html);
                        int i = 1;
                        string node = "";
                        var txt = new HtmlDocument();
                        int counter = 0;
                        while (counter != 5)
                        {
                            try
                            {
                                node = htmlDoc.DocumentNode.SelectSingleNode("//dd[" + i + "]/div[@class='question']/p[1]").InnerText;
                                if (node != null)
                                {
                                    Regex rgx = new Regex("&nbsp;");
                                    string rep_node = rgx.Replace(node, "");
                                    rgx = new Regex("&mdash;");
                                    rep_node = rgx.Replace(rep_node, "");
                                    que = rep_node;
                                    rep_node = null;
                                    string check_repitable_question = db.Questions.FirstOrDefault(x => x.Questions == que)?.Questions;
                                    if (check_repitable_question != null)
                                    {
                                        i++;
                                    }
                                    else
                                    {
                                        node = htmlDoc.DocumentNode.SelectSingleNode("//dd[" + i + "]/div[@class='question']/p[2]").InnerText;
                                        if (node != null)
                                        {
                                            rgx = new Regex("&nbsp;");
                                            rep_node = rgx.Replace(node, "");
                                            rgx = new Regex("&mdash;");
                                            rep_node = rgx.Replace(rep_node, "");
                                            ans = rep_node;
                                            rep_node = null;
                                        }
                                        node = htmlDoc.DocumentNode.SelectSingleNode("//dd[" + i + "]/div[@class='question']/p[3]").InnerText;
                                        if (node != null)
                                        {
                                            rgx = new Regex("&nbsp;");
                                            rep_node = rgx.Replace(node, "");
                                            rgx = new Regex("&mdash;");
                                            rep_node = rgx.Replace(rep_node, "");
                                            ans = ans + "  " + rep_node;
                                            rep_node = null;
                                        }
                                        node = htmlDoc.DocumentNode.SelectSingleNode("//dd[" + i + "]/div[@class='question']/p[4]").InnerText;
                                        if (node != null)
                                        {
                                            rgx = new Regex("&nbsp;");
                                            rep_node = rgx.Replace(node, "");
                                            rgx = new Regex("&mdash;");
                                            rep_node = rgx.Replace(rep_node, "");
                                            comment = rep_node;
                                            rep_node = null;
                                            i++;
                                            counter++;
                                        }
                                        db.Questions.Add(new Question { CategoryID = counter_category, Questions = que, Answer = ans, Comment = comment });
                                        db.SaveChanges();
                                    }
                                }

                            }
                            catch
                            {
                                break;
                            }

                        }
                    }
                    db.Categories.Add(entity: new Category { Id = counter_category, Name = category.Name, Post = category.Post, Vocabulary = category.Vocabulary });
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "Такое имя уже существует");
                return View(category);
            }



        }




        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Post,Vocabulary")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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
