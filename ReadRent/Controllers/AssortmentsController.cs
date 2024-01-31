using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ReadRent.Models;

namespace ReadRent.Controllers
{
    public class AssortmentsController : Controller
    {
        private ReadRentEntities db = new ReadRentEntities();

        // GET: Assortments
        public ActionResult Index(string sortOrder, string sortColumn, string searchString)
        {
            if (Session["user_id"] != null)
            {
                var assortments = db.Assortments.Include(a => a.Book);
                bool ascending = sortOrder == "asc";
                if (!String.IsNullOrEmpty(searchString))
                {
                    assortments = assortments.Where(s => s.Book.title.Contains(searchString)
                                          || s.Book.author.Contains(searchString));                    
                }
                switch (sortColumn)
                {
                    case "title":
                        assortments = ascending ? assortments.OrderBy(b => b.Book.title) : assortments.OrderByDescending(b => b.Book.title);
                        break;
                    case "author":
                        assortments = ascending ? assortments.OrderBy(b => b.Book.author) : assortments.OrderByDescending(b => b.Book.author);
                        break;
                    default:
                        assortments = assortments.OrderBy(b => b.Book.title);
                        break;
                }

                ViewData["SortOrder"] = sortOrder;
                ViewData["SortColumn"] = sortColumn;

                return View(assortments.ToList());
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Assortments/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["user_id"] != null)
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Assortment assortment = db.Assortments.Find(id);
                if (assortment == null)
                {
                    return HttpNotFound();
                }
                return View(assortment);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Assortments/Create
        public ActionResult Create()
        {
            if (Session["user_id"] != null)
            {
                ViewBag.book_id = new SelectList(db.Books, "id", "title");
                return View();
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Assortments/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,book_id")] Assortment assortment)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Assortments.Add(assortment);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.book_id = new SelectList(db.Books, "id", "title", assortment.book_id);
                return View(assortment);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Assortments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Assortment assortment = db.Assortments.Find(id);
                if (assortment == null)
                {
                    return HttpNotFound();
                }
                ViewBag.book_id = new SelectList(db.Books, "id", "title", assortment.book_id);
                return View(assortment);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Assortments/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,book_id")] Assortment assortment)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(assortment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.book_id = new SelectList(db.Books, "id", "title", assortment.book_id);
                return View(assortment);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Assortments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Assortment assortment = db.Assortments.Find(id);
                if (assortment == null)
                {
                    return HttpNotFound();
                }
                return View(assortment);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Assortments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["user_id"] != null)
            {
                Assortment assortment = db.Assortments.Find(id);
                db.Assortments.Remove(assortment);
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
