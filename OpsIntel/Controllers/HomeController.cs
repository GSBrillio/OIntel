using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpsIntel.ViewModel;
using OpsIntel.Models;
using System.Web.Security;
using System.Data.Entity.Validation;
namespace OpsIntel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Home()
        {
            return View();
        }
        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(User user)
        {
            //if (ModelState.IsValid)
            //{
            if (IsValid(user.Username, user.Password))
            {

                TempData["Username"] = user.Username;
                //FormsAuthentication.SetAuthCookie(user.Username, false);

                return RedirectToAction("Index", "Home");

            }
            else
            {
                ModelState.AddModelError("", "Login details are wrong.");
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (OpsIntel1Entities1 context = new OpsIntel1Entities1())
                    {
                        User newUser = new User();
                       // var newUser = context.Users.Create();
                        newUser.Email = user.Email;
                        newUser.Password = user.Password;
                        newUser.Username = user.Username;
                        newUser.FirstName = user.FirstName;
                        newUser.LastName = user.LastName;
                      
                        newUser.CreatedDate = DateTime.Now;
                        
                        
                        context.Users.Add(newUser);
                        context.SaveChanges();
                        return RedirectToAction("LogIn", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct");
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return View();
        }
        private bool IsValid(string username, string password)
        {
            
            bool IsValid = false;

            using (OpsIntel1Entities1 context = new OpsIntel1Entities1())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    if (user.Password ==password)
                    {
                        IsValid = true;
                    }
                }
            }
            return IsValid;
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogIn", "Home");
        }

        [System.Web.Http.HttpGet]
        public JsonResult VMAllDetails(int id)
        {
            ChartController chartController = new ChartController();
            var result = chartController.GetVmThresholdData(id);

            RulesAction rulesAction = new RulesAction();
            foreach (var item in result)
            {
                rulesAction.Id = item.Id;
                rulesAction.Name = item.VMName;
                rulesAction.CPUs = item.CloudServiceName;
                rulesAction.Memory = item.Memory;
                rulesAction.ThresholdCPU = item.Threshold_CPU;
                rulesAction.ThresholdMemory = item.Threshold_Memory;
                rulesAction.DiskRead = item.Threshold_DiskRead;
                rulesAction.DiskWrite = item.Threshold_DiskWrite;
                rulesAction.NetworkIn = item.Threshold_NetworkIn;
                rulesAction.NetworkOut = item.Threshold_NetworkOut;

            }
            //return View("_RulesAction", rulesAction);

            return Json(rulesAction, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult RulesAction(int id)
        //{
        //    ChartController chartController = new ChartController();
        //    var result = chartController.GetVmThresholdData(id);

        //    RulesAction rulesAction = new RulesAction();
        //    foreach (var item in result)
        //    {
        //        rulesAction.Name = item.VMName;
        //        rulesAction.CPUs = item.CloudServiceName;
        //        rulesAction.Memory = item.Memory;
        //        rulesAction.ThresholdCPU = item.Threshold_CPU;
        //        rulesAction.ThresholdMemory = item.Threshold_Memory;
        //        rulesAction.DiskRead = item.Threshold_DiskRead;
        //        rulesAction.DiskWrite = item.Threshold_DiskWrite;
        //        rulesAction.NetworkIn = item.Threshold_NetworkIn;
        //        rulesAction.NetworkOut = item.Threshold_NetworkOut;

        //    }
        //    return View("_RulesAction", rulesAction);
        //}
    }
}
