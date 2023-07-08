using HRSystem.Entity;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HRSystem.Controllers
{
    public class AdminController : Controller
    {
        private HRSystemDBEntities db = new HRSystemDBEntities();
        public bool hisadmin = UserController.hisadmin;
        public bool logged = UserController.logged;
        public int myid = UserController.myid;

        public ActionResult Dashboard()
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {

                    #region get count for employees, departmrnts and numOfReq

                    ViewBag.employees = db.Users.Where(User => User.Role_Id == 2).Count(); //numbers of employee 
                    ViewBag.department = db.Departments.ToList().Count(); // numbers of debartmnt
                    var Req = db.Users.Where(u => u.Is_Active == 0); // numbers of requste
                    ViewBag.role = db.Roles.ToList();

                    #endregion

                    #region get avareg od sallary
                    var users = db.Users.ToList();
                    if (users.Count > 0)
                    {
                        var sum = 0;
                        foreach (var user in users)
                        {
                            sum += (int)user.Salary;
                        }
                        var average = sum / users.Count;
                        ViewBag.average = average;
                    }
                    #endregion

                    return View(Req);
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }


        }


        #region  Role 
        #region List role
        [HttpGet]
        public ActionResult ListRoles()
        {
            var userId = Request.Cookies["UserID"];
            if (userId != null)
            {
                var role = db.Roles.ToList();
                return View(role);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        #endregion

        #region add role
        [HttpGet]
        public ActionResult CreateRole()
        {
            var userId = Request.Cookies["UserID"];
            if (userId != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        #endregion

        [HttpPost]
        public ActionResult CreateRole(Role role)
        {
            var userId = Request.Cookies["UserID"];
            if (userId != null)
            {
                if (ModelState.IsValid)
                {
                    db.Roles.Add(role);
                    db.SaveChanges();
                    return RedirectToAction("ListRoles");
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        #endregion

        #region Departmint
        #region list departmint
        public ActionResult AllDepartment()
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    var departments = db.Departments.ToList();
                    return View(departments);
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        #endregion

        #region Add Department

        [HttpGet]
        public ActionResult CreateDepartment()
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpPost]
        public ActionResult CreateDepartment(Department dep)
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    var isExist = db.Departments.Where(d => d.Name == dep.Name).FirstOrDefault();
                    if(isExist == null)
                    {
                        db.Departments.Add(dep);
                        db.SaveChanges();
                        return RedirectToAction("AllDepartment", TempData["SuccessMessage"] = true);
                    }
                    else
                    {
                        ViewBag.error = "Department is already exist";
                        return View();
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        #endregion

        #region  Department Details

        public ActionResult DepartmentDetails(int id)
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    var details = db.Departments.Where(c => c.Id == id).FirstOrDefault();
                    var countEmp = ViewBag.employee=db.Users.Where(d => d.Department_id == id).ToList();
                    ViewBag.loginId=db.Users.Where(d => d.Id == myid).ToList();

                    if (countEmp.Count > 0)
                    {
                        var some = 0;
                        foreach (var employee in countEmp)
                        {
                            some += (int)employee.Salary;
                        }
                        some = some / countEmp.Count;
                        ViewBag.employee = countEmp;
                        ViewBag.some = some;
                    }
                    else
                    {
                        ViewBag.some = 0;
                    }
                    return View(details);
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        #endregion

        #region Delete Department

        public ActionResult DeleteDepartment(int id)
        {if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    Department department = db.Departments.Find(id);
                    var users = db.Users.Where(u => u.Department_id == id).ToList();
                    if(users.Count <=0)
                    {
                        if (department == null)
                        {
                            return HttpNotFound();
                        }
                        db.Departments.Remove(department);
                        db.SaveChanges();
                        return RedirectToAction("AllDepartment");
                    }
                    return RedirectToAction("AllDepartment", TempData["DeleteMessage"] = "You cannot delete "+department.Name+ " department Because it has "+ users.Count()+" employees");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        #endregion
        #endregion

        #region Employee


        #region list employee
        public ActionResult AllEmployee(int? pageNum)
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    int pageSize = 6;
                    int page;
                    if (pageNum == null)
                    {
                        page = (pageNum ?? 1);
                    }
                    else
                    {
                        page = (int)pageNum;
                    }
                    var employees = db.Users.Where(u=> u.Id != myid).ToList();
                    return View(employees.OrderBy(x => x.Name).ToPagedList(page, pageSize));
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        #endregion

        #region employee details

        public ActionResult EmployeeDetails(int id)
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    ViewBag.experience = db.Experiences.Where(e => e.Emp_Id == id).ToList();

                    var employee = db.Users.Where(d => d.Id == id).FirstOrDefault();
                    //ViewBag.expernice = db.Users.Where(d => d.Experience_id == id).ToList();
                    ViewBag.experience = db.Experiences.Where(d => d.Emp_Id == employee.Id).ToList();

                    return View(employee);
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        #endregion

        #region create employee
        [HttpGet]
        public ActionResult CreateEmployee()
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    ViewBag.department = db.Departments.ToList();
                    ViewBag.role = db.Roles.ToList();
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpPost]
        public ActionResult CreateEmployee(User employee)
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    ViewBag.department = db.Departments.ToList();
                    ViewBag.role = db.Roles.ToList();
                    var isExist = db.Users.Where(d => d.UserName == employee.UserName).FirstOrDefault();
                    if (isExist == null)
                    {   
                        db.Users.Add(employee);
                        employee.Is_Active = 0;
                        db.SaveChanges();
                        return RedirectToAction("AllEmployee", TempData["SuccessMessage"] = true);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "UserName is already taken. Try a different UserName";
                        return View();
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        #endregion

        #region Edit employee
        public ActionResult EditEmployee(int id)
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    var user = db.Users.Where(u => u.Id == id).FirstOrDefault();
                    ViewBag.department = db.Departments.ToList();
                    ViewBag.role = db.Roles.ToList();
                    return View(user);
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpPost]
        public ActionResult EditEmployee(User newuser)
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    var oldUser = db.Users.Where(u => u.Id == newuser.Id).FirstOrDefault();
                    int id = oldUser.Id;
                    var userNameExist = db.Users.Where(u => u.UserName == newuser.UserName).Count();
                    var compare = db.Users.Where(u => u.UserName == newuser.UserName && u.Id == newuser.Id).Count();

                    if (userNameExist > 0)
                    {
                        if (compare > 0)
                        {
                            oldUser.Name = newuser.Name;
                            oldUser.UserName = newuser.UserName;
                            oldUser.Salary = newuser.Salary;
                            oldUser.Department_id = newuser.Department_id;
                            ViewBag.SuccessMessage = "Updated successfully";
                            db.SaveChanges();
                            return RedirectToAction("EditEmployee", id);
                        }
                        ViewBag.ErrorMessage = "Username is already exist";
                        return RedirectToAction("EditEmployee", id);
                    }
                    oldUser.Name = newuser.Name;
                    oldUser.UserName = newuser.UserName;
                    oldUser.Salary = newuser.Salary;
                    oldUser.Department_id = newuser.Department_id;
                    ViewBag.SuccessMessage = "Updated successfully";
                    TempData["SuccessMessage"] = true;
                    db.SaveChanges();
                    return RedirectToAction("EditEmployee");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        #endregion

        #region delete employee
        public ActionResult DeleteEmployee(int id)
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    User emp = db.Users.Find(id);
                    if (emp == null)
                    {
                        return HttpNotFound();
                    }
                    var ex = db.Experiences.Where(e => e.Emp_Id == emp.Id).ToList();
                    foreach (var exp in ex)
                    {
                        db.Experiences.Remove(exp);
                    }
                    db.Users.Remove(emp);
                    db.SaveChanges();

                    return RedirectToAction("AllEmployee", TempData["DeleteEmployee"] = true);
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        #endregion





        #region Active & un Active
        public ActionResult Active(int id)
        {
            if (logged == true) //if loged?
            {
                if (hisadmin != true) // not Admin?
                {
                    return RedirectToAction("Dashboard", "Employee");
                }
                else
                {
                    var user = db.Users.Where(d => d.Id == id).FirstOrDefault();
                    if(user.Is_Active == 0)
                    {
                        user.Is_Active = 1;
                    }
                    else if (user.Is_Active == 1)
                    {
                        user.Is_Active = 0;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        #endregion
        #endregion

    }
}