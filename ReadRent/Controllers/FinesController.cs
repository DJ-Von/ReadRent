using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ReadRent.Models;

namespace ReadRent.Controllers
{
    public class FinesController : Controller
    {
        private ReadRentEntities db = new ReadRentEntities();

        // GET: Fines
        public ActionResult Index(string sortOrder, string sortColumn, string searchString)
        {
            if (Session["user_id"] != null)
            {
                var fines = db.Fines.Include(f => f.User);
                bool ascending = sortOrder == "asc";
                if (!String.IsNullOrEmpty(searchString))
                {
                    fines = fines.Where(s => s.User.realname.Contains(searchString)
                                          || s.User.username.Contains(searchString)
                                          || s.summ == Convert.ToInt32(searchString));
                }

                switch (sortColumn)
                {
                    case "realname":
                        fines = ascending ? fines.OrderBy(b => b.User.realname) : fines.OrderByDescending(b => b.User.realname);
                        break;
                    case "username":
                        fines = ascending ? fines.OrderBy(b => b.User.username) : fines.OrderByDescending(b => b.User.username);
                        break;
                    case "summ":
                        fines = ascending ? fines.OrderBy(b => b.summ) : fines.OrderByDescending(b => b.summ);
                        break;
                    default:
                        fines = fines.OrderBy(b => b.User.realname);
                        break;
                }

                ViewData["SortOrder"] = sortOrder;
                ViewData["SortColumn"] = sortColumn;

                return View(fines.ToList());
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Fines/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Fine fine = db.Fines.Find(id);
                if (fine == null)
                {
                    return HttpNotFound();
                }
                return View(fine);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Fines/Create
        public ActionResult Create()
        {
            if (Session["user_id"] != null)
            {
                ViewBag.user_id = new SelectList(db.Users, "id", "realname");
                return View();
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Fines/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,user_id,summ")] Fine fine)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Fines.Add(fine);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.user_id = new SelectList(db.Users, "id", "realname", fine.user_id);
                return View(fine);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Fines/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Fine fine = db.Fines.Find(id);
                if (fine == null)
                {
                    return HttpNotFound();
                }
                ViewBag.user_id = new SelectList(db.Users, "id", "realname", fine.user_id);
                return View(fine);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Fines/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,user_id,summ")] Fine fine)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(fine).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.user_id = new SelectList(db.Users, "id", "realname", fine.user_id);
                return View(fine);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Fines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Fine fine = db.Fines.Find(id);
                if (fine == null)
                {
                    return HttpNotFound();
                }
                return View(fine);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Fines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["user_id"] != null)
            {
                Fine fine = db.Fines.Find(id);
                db.Fines.Remove(fine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
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
