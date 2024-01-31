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
    public class UserCategoriesController : Controller
    {
        private ReadRentEntities db = new ReadRentEntities();

        // GET: UserCategories
        public ActionResult Index(string sortOrder, string sortColumn, string searchString)
        {
            if (Session["user_id"] != null)
            {
                IQueryable<ReadRent.Models.UserCategory> categories = db.UserCategories;
                bool ascending = sortOrder == "asc";
                /*TO-DO: починить сортировку и поиск*/
                if (!String.IsNullOrEmpty(searchString))
                {
                    categories = categories.Where(s => s.title.Contains(searchString));
                }

                switch (sortColumn)
                {
                    case "title":
                        categories = ascending ? categories.OrderBy(b => b.title) : categories.OrderByDescending(b => b.title);
                        break;
                    case "discount":
                        categories = ascending ? categories.OrderBy(b => b.discount) : categories.OrderByDescending(b => b.discount);
                        break;
                    default:
                        categories = categories.OrderBy(b => b.title);
                        break;
                }

                ViewData["SortOrder"] = sortOrder;
                ViewData["SortColumn"] = sortColumn;

                return View(categories.ToList());
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: UserCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UserCategory userCategory = db.UserCategories.Find(id);
                if (userCategory == null)
                {
                    return HttpNotFound();
                }
                return View(userCategory);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: UserCategories/Create
        public ActionResult Create()
        {
            if (Session["user_id"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: UserCategories/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,title,discount")] UserCategory userCategory)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.UserCategories.Add(userCategory);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(userCategory);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: UserCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UserCategory userCategory = db.UserCategories.Find(id);
                if (userCategory == null)
                {
                    return HttpNotFound();
                }
                return View(userCategory);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: UserCategories/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,discount")] UserCategory userCategory)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(userCategory).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(userCategory);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: UserCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UserCategory userCategory = db.UserCategories.Find(id);
                if (userCategory == null)
                {
                    return HttpNotFound();
                }
                return View(userCategory);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: UserCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["user_id"] != null)
            {
                UserCategory userCategory = db.UserCategories.Find(id);
                db.UserCategories.Remove(userCategory);
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
