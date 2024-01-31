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
    public class RentsController : Controller
    {
        private ReadRentEntities db = new ReadRentEntities();

        // GET: Rents
        public ActionResult Index(string sortOrder, string sortColumn, string searchString)
        {
            if (Session["user_id"] != null)
            {
                var rents = db.Rents.Include(r => r.Assortment).Include(r => r.User);

                bool ascending = sortOrder == "asc";
                if (!String.IsNullOrEmpty(searchString))
                {
                    /* TO-DO: Добавить ещё поля для сортировки */
                    rents = rents.Where(s => s.User.realname.Contains(searchString)
                                          || s.User.username.Contains(searchString));
                }
                
                switch (sortColumn)
                {
                    case "date_begin":
                        rents = ascending ? rents.OrderBy(b => b.date_begin) : rents.OrderByDescending(b => b.date_begin);
                        break;
                    case "date_end":
                        rents = ascending ? rents.OrderBy(b => b.date_end) : rents.OrderByDescending(b => b.date_end);
                        break;
                    case "summ":
                        rents = ascending ? rents.OrderBy(b => b.summ) : rents.OrderByDescending(b => b.summ);
                        break;
                    case "payment_status":
                        rents = ascending ? rents.OrderBy(b => b.payment_status) : rents.OrderByDescending(b => b.payment_status);
                        break;
                    case "title":
                        rents = ascending ? rents.OrderBy(b => b.Assortment.Book.title) : rents.OrderByDescending(b => b.Assortment.Book.title);
                        break;
                    case "realname":
                        rents = ascending ? rents.OrderBy(b => b.User.realname) : rents.OrderByDescending(b => b.User.realname);
                        break;
                    case "username":
                        rents = ascending ? rents.OrderBy(b => b.User.username) : rents.OrderByDescending(b => b.User.username);
                        break;
                    default:
                        rents = rents.OrderByDescending(b => b.date_begin);
                        break;
                }

                ViewData["SortOrder"] = sortOrder;
                ViewData["SortColumn"] = sortColumn;

                return View(rents.ToList());
            } else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Rents/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Rent rent = db.Rents.Find(id);
                if (rent == null)
                {
                    return HttpNotFound();
                }
                return View(rent);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Rents/Create
        public ActionResult Create()
        {
            if (Session["user_id"] != null)
            {
                ViewBag.assortment_id = new SelectList(db.Assortments, "id", "id");
                ViewBag.user_id = new SelectList(db.Users, "id", "realname");
                return View();
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Rents/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,date_begin,date_end,summ,payment_status,user_id,assortment_id")] Rent rent)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Rents.Add(rent);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.assortment_id = new SelectList(db.Assortments, "id", "id", rent.assortment_id);
                ViewBag.user_id = new SelectList(db.Users, "id", "realname", rent.user_id);
                return View(rent);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Rents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Rent rent = db.Rents.Find(id);
                if (rent == null)
                {
                    return HttpNotFound();
                }
                ViewBag.assortment_id = new SelectList(db.Assortments, "id", "id", rent.assortment_id);
                ViewBag.user_id = new SelectList(db.Users, "id", "realname", rent.user_id);
                return View(rent);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Rents/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,date_begin,date_end,summ,payment_status,user_id,assortment_id")] Rent rent)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(rent).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.assortment_id = new SelectList(db.Assortments, "id", "id", rent.assortment_id);
                ViewBag.user_id = new SelectList(db.Users, "id", "realname", rent.user_id);
                return View(rent);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Rents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Rent rent = db.Rents.Find(id);
                if (rent == null)
                {
                    return HttpNotFound();
                }
                return View(rent);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Rents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["user_id"] != null)
            {
                Rent rent = db.Rents.Find(id);
                db.Rents.Remove(rent);
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
