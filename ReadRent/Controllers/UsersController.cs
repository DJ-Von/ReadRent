using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ReadRent.Models;

namespace ReadRent.Controllers
{
    public class UsersController : Controller
    {
        private ReadRentEntities db = new ReadRentEntities();

        // GET: Users
        public ActionResult Index(string sortOrder, string sortColumn, string searchString)
        {
            if (Session["user_id"] != null)
            {
                var users = db.Users.Include(u => u.UserCategory).Include(u => u.UserRole);
                bool ascending = sortOrder == "asc";
                if (!String.IsNullOrEmpty(searchString))
                {
                    users = users.Where(s => s.realname.Contains(searchString)
                                          || s.username.Contains(searchString)
                                          || s.UserCategory.title.Contains(searchString)
                                          || s.UserRole.role_name.Contains(searchString));
                }

                switch (sortColumn)
                {
                    case "realname":
                        users = ascending ? users.OrderBy(b => b.realname) : users.OrderByDescending(b => b.realname);
                        break;
                    case "username":
                        users = ascending ? users.OrderBy(b => b.username) : users.OrderByDescending(b => b.realname);
                        break;
                    case "category_id":
                        users = ascending ? users.OrderBy(b => b.UserCategory.title) : users.OrderByDescending(b => b.UserCategory.title);
                        break;
                    case "role_id":
                        users = ascending ? users.OrderBy(b => b.UserRole.role_name) : users.OrderByDescending(b => b.UserRole.role_name);
                        break;
                    default:
                        users = users.OrderBy(b => b.realname);
                        break;
                }

                ViewData["SortOrder"] = sortOrder;
                ViewData["SortColumn"] = sortColumn;

                return View(users.ToList());
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                User user = db.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            if (Session["user_id"] != null)
            {
                ViewBag.category_id = new SelectList(db.UserCategories, "id", "title");
                ViewBag.role_id = new SelectList(db.UserRoles, "id", "role_name");
                return View();
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Users/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,category_id,realname,username,password_hash,role_id")] User user, HttpPostedFileBase upload)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            user.img = reader.ReadBytes(upload.ContentLength);
                        }
                    }
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.category_id = new SelectList(db.UserCategories, "id", "title", user.category_id);
                ViewBag.role_id = new SelectList(db.UserRoles, "id", "role_name", user.role_id);
                return View(user);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                User user = db.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                ViewBag.category_id = new SelectList(db.UserCategories, "id", "title", user.category_id);
                ViewBag.role_id = new SelectList(db.UserRoles, "id", "role_name", user.role_id);
                return View(user);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Users/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,category_id,realname,username,password_hash,role_id")] User user, HttpPostedFileBase upload)
        {
            if (Session["user_id"] != null)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.Entry(user).State = EntityState.Modified;
                        if (upload != null && upload.ContentLength > 0)
                        {
                            using (var reader = new System.IO.BinaryReader(upload.InputStream))
                            {
                                user.img = reader.ReadBytes(upload.ContentLength);
                            }
                            db.SaveChanges();
                        }

                        else
                        {
                            db.Entry(user).Property(m => m.img).IsModified = false;
                            db.SaveChanges();
                        }

                        return RedirectToAction("Index");
                    }

                    ViewBag.category_id = new SelectList(db.UserCategories, "id", "title", user.category_id);
                    ViewBag.role_id = new SelectList(db.UserRoles, "id", "role_name", user.role_id);
                    return View(user);
                }
                catch (Exception e) { }

                ViewBag.category_id = new SelectList(db.UserCategories, "id", "title", user.category_id);
                ViewBag.role_id = new SelectList(db.UserRoles, "id", "role_name", user.role_id);
                return View(user);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["user_id"] != null)
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                User user = db.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["user_id"] != null)
            {
                User user = db.Users.Find(id);
                db.Users.Remove(user);
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
