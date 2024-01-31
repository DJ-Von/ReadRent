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
    public class UserRolesController : Controller
    {
        private ReadRentEntities db = new ReadRentEntities();

        // GET: UserRoles
        public ActionResult Index(string sortOrder, string sortColumn, string searchString)
        {
            if (Session["user_id"] != null)
            {
                IQueryable<ReadRent.Models.UserRole> roles = db.UserRoles;
                bool ascending = sortOrder == "asc";
                if (!String.IsNullOrEmpty(searchString))
                {
                    roles = roles.Where(s => s.role_name.Contains(searchString));
                }
                switch (sortColumn)
                {
                    case "role_name":
                        roles = ascending ? roles.OrderBy(b => b.role_name) : roles.OrderByDescending(b => b.role_name);
                        break;  
                    default:
                        roles = roles.OrderBy(b => b.role_name);
                        break;
                }

                ViewData["SortOrder"] = sortOrder;
                ViewData["SortColumn"] = sortColumn;
                return View(roles.ToList());
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: UserRoles/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UserRole userRole = db.UserRoles.Find(id);
                if (userRole == null)
                {
                    return HttpNotFound();
                }
                return View(userRole);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: UserRoles/Create
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

        // POST: UserRoles/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,role_name")] UserRole userRole)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.UserRoles.Add(userRole);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(userRole);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: UserRoles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UserRole userRole = db.UserRoles.Find(id);
                if (userRole == null)
                {
                    return HttpNotFound();
                }
                return View(userRole);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: UserRoles/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,role_name")] UserRole userRole)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(userRole).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(userRole);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: UserRoles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UserRole userRole = db.UserRoles.Find(id);
                if (userRole == null)
                {
                    return HttpNotFound();
                }
                return View(userRole);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: UserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["user_id"] != null)
            {
                UserRole userRole = db.UserRoles.Find(id);
                db.UserRoles.Remove(userRole);
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
