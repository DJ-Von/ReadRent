using Microsoft.Win32;
using ReadRent.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ReadRent.Controllers
{
    public class HomeController : Controller
    {
        private ReadRentEntities db = new ReadRentEntities();
        public ActionResult Index()
        {
            if (Session["user_id"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //GET: Register

        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                var check = db.Users.FirstOrDefault(s => s.username == user.username);
                if (check == null)
                {
                    user.password_hash = user.password_hash;//GetMD5(_user.Password);
                    user.category_id = 1;
                    user.role_id = 3;
                    if (upload != null && upload.ContentLength > 0)
                    {
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            user.img = reader.ReadBytes(upload.ContentLength);
                        }
                    }

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Такое имя пользователя уже существует";
                    return View();
                }
            }
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password_hash)
        {
            if (ModelState.IsValid)
            {
                var f_password = password_hash;//GetMD5(password);
                var data = db.Users.Where(s => s.username.Equals(username) && s.password_hash.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["realname"] = data.FirstOrDefault().realname;
                    Session["username"] = data.FirstOrDefault().username;
                    Session["user_id"] = data.FirstOrDefault().id;
                    Session["user_role"] = data.FirstOrDefault().UserRole.role_name;
                    Session["img"] = data.FirstOrDefault().img;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }


        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }



        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}