using HRSystem.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRSystem.Controllers
{
    public class UserController : Controller
    {
        private HRSystemDBEntities db = new HRSystemDBEntities();

        public static bool hisadmin;
        public static bool logged;
        public static int myid;

        #region Login 
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            if (logged != true)
            {
                var tryLogin = db.Users.Where(u => u.UserName == user.UserName).FirstOrDefault();
                if (tryLogin != null)
                {
                    tryLogin = db.Users.Where(u => u.UserName == user.UserName && u.Password == user.Password).FirstOrDefault();
                    if (tryLogin != null)
                    {
                        if (tryLogin.Is_Active == 1)
                        {
                            if (tryLogin.Role_Id == 1)
                            {
                                hisadmin = true;
                                logged = true;
                                myid = tryLogin.Id;
                                return RedirectToAction("Dashboard", "Admin");
                            }
                            else
                            {
                                hisadmin = false;
                                myid = tryLogin.Id;
                                logged = true;
                                return RedirectToAction("Dashboard", "Employee");
                            }
                        }
                        else
                        {
                            ViewBag.error = "User is not activated";
                        }

                    }
                    else
                    {
                        ViewBag.error = "The username or password is wrong";
                    }
                }
                else
                {
                    ViewBag.error2 = "The user not exist   ";
                }
                return View();
            }

            else
            {
                if (hisadmin != true)
                {
                    return RedirectToAction("Dashboard", "Employee", new { id = myid });

                }
                else
                {

                    return RedirectToAction("Dashboard", "Admin");
                }
            }
        }

        #endregion

        #region Register 
        public ActionResult Register()
        {
            if (logged != true)
            {
                return View();
            }
            else
            {
                if (hisadmin != true)
                {
                    return RedirectToAction("Dashboard", "Employee", new { id = myid } );
                }
                else
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
            }
        }

        [HttpPost]
        public ActionResult Register(User user)
        {

            var compare = db.Users.Where(m => m.UserName == user.UserName).Count();
            while (compare > 0)
            {
                ViewBag.error = "Username is already exist";
                return View();
            }
            if (user.Password == user.RePassword)
            {
                var newUser = db.Users.Add(user);
                newUser.Role_Id = 2;
                newUser.Is_Active = 0;
                db.SaveChanges();
                ViewBag.good = "registered successfully, please wait to be activated by the administrator";
                return RedirectToAction("Login", "User");
            }
            ViewBag.error = "Password must match";
            return View();
        }

        #endregion

        #region logout
        [HttpGet]
        public ActionResult Logout()
        {
            if (logged == true) //if loged?
            {
                hisadmin = false;
                logged = false;
                return RedirectToAction("Login", "User");
            }
            else
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
            }
        }
        #endregion

        #region Profile
        public ActionResult Profile()
        {
            var details = db.Users.Where(c => c.Id == myid).FirstOrDefault();
            var role = db.Roles.Where(r => r.Id == details.Role_Id).FirstOrDefault();
            ViewBag.roleName = role.Name;
            return View(details);
        }

        #endregion

    }
}