using HRSystem.Entity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HRSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private HRSystemDBEntities db = new HRSystemDBEntities();
        public bool hisadmin = UserController.hisadmin;
        public bool logged = UserController.logged;
        public int myid = UserController.myid;

        // GET: Employee


        #region list emp details

        public ActionResult Dashboard()
        {
            
            if (logged==true)
            {
                var empIndSssion = db.Users.Where(d => d.Id == myid).FirstOrDefault();
                ViewBag.employee = db.Users.Where(d => d.Department_id == empIndSssion.Department_id && d.Role_Id != 1 && d.Id != myid).ToList();
                ViewBag.experience = db.Experiences.Where(e => e.Emp_Id == empIndSssion.Id).ToList();
                var details = db.Users.Where(c => c.Id == myid).FirstOrDefault();

                return View(details);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        #endregion


        #region employee details

        public ActionResult Details(int id)
        {
            var employee = db.Users.Where(d => d.Id == id).FirstOrDefault();
            //ViewBag.employee = db.Users.Where(d => d.Department_id == employee.Department_id).ToList();
            ViewBag.expernice = db.Users.Where(d => d.Experience_id == id).ToList();
            return View(employee);
        }

        #endregion


        #region Add Experience

        [HttpGet]
        public ActionResult CreateExperience()
        {
            if (logged == true)
            {
                var empIndSssion = db.Users.Where(d => d.Id == myid).FirstOrDefault();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        public ActionResult CreateExperience(Experience experience)
        {
            if (logged == true)
            {
                var empIndSssion = db.Users.Where(d => d.Id == myid).FirstOrDefault();
                Experience experience1 = new Experience()
                {
                    Title = experience.Title,
                    Description = experience.Description,
                    Emp_Id = empIndSssion.Id,
                };

                db.Experiences.Add(experience1);
                db.SaveChanges();

                return RedirectToAction("Dashboard", TempData["SuccessMessage"] = true);
            }
            else
            {
                return RedirectToAction("Login", "User");

            }

        }
        #endregion

        #region Delete Experience
        public ActionResult DeleteExperience(int id)
        {
            var empIndSssion = db.Users.Where(d => d.Id == myid).FirstOrDefault();
            var experiences = db.Experiences.FirstOrDefault();
            if (logged == true && experiences.Emp_Id == empIndSssion.Id)
            {
                Experience emp = db.Experiences.Find(id);
                if (emp == null)
                {
                    return HttpNotFound();
                }
                db.Experiences.Remove(emp);
                db.SaveChanges();

                return RedirectToAction("Dashboard", TempData["DeleteEmployee"] = true);
            }
            else
            {
                return RedirectToAction("Login", "User");

            }
        }
        #endregion

        #region Edit employee
        //public ActionResult Edit()
        //{
        //    var user = db.Users.Where(u => u.Id == myid).FirstOrDefault();
        //    ViewBag.department = db.Departments.ToList();
        //    ViewBag.role = db.Roles.ToList();
        //    return View(user);
        //}

        //[HttpPost]
        //public ActionResult Edit(User newuser)
        //{
        //    var oldUser = db.Users.Where(u => u.Id == newuser.Id).FirstOrDefault();
        //    int id = oldUser.Id;
        //    var userNameExist = db.Users.Where(u => u.UserName == newuser.UserName).Count();
        //    var compare = db.Users.Where(u => u.UserName == newuser.UserName && u.Id == newuser.Id).Count();

        //    if (userNameExist > 0)
        //    {
        //        if (compare > 0)
        //        {
        //            oldUser.Name = newuser.Name;
        //            oldUser.Password = newuser.Password;
        //            oldUser.UserName = newuser.UserName;
        //            ViewBag.SuccessMessage = "Updated successfully";
        //            db.SaveChanges();
        //            return RedirectToAction("EditEmployee", id);
        //        }
        //        ViewBag.ErrorMessage = "Username is already exist";
        //        return RedirectToAction("EditEmployee", id);
        //    }
        //    oldUser.Name = newuser.Name;
        //    oldUser.UserName = newuser.UserName;
        //    oldUser.Password = newuser.Password;
        //    ViewBag.SuccessMessage = "Updated successfully";
        //    db.SaveChanges();
        //    return RedirectToAction("EditEmployee", id);
        //}
        #endregion

    }
}