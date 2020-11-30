using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using yasemin.Models;

namespace yasemin.Controllers
{
    public class SecurityController : Controller
    {
        // GET: Security
        DbFoodOrderingSystemEntities FoodOrder = new DbFoodOrderingSystemEntities();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(MEMBER member)
        {
            var memberInDb = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberMail == member.MemberMail && x.MemberPassword == member.MemberPassword);
            if (memberInDb != null)
            {
                FormsAuthentication.SetAuthCookie(memberInDb.MemberMail,false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.message = "Unvalid mail or password";
                return View();
            }
        }
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(MEMBER member)
        {
            var memberInDb = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberMail == member.MemberMail && x.MemberPassword == member.MemberPassword);
            if (memberInDb == null)
            {
                FoodOrder.MEMBER.Add(member);
                FoodOrder.SaveChanges();
                if (member.MemberRole == "U")
                {
                    MEMBER m = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberMail == member.MemberMail);
                    USERS user = new USERS();
                    user.UserId = m.MemberId;
                    user.UserMail = m.MemberMail;
                    user.UserPassword = m.MemberPassword;
                    user.UserName = m.MemberMail;
                    user.UserSurname = " ";
                    FoodOrder.USERS.Add(user);
                    FoodOrder.SaveChanges();
                    memberInDb = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberMail == member.MemberMail);
                    FormsAuthentication.SetAuthCookie(memberInDb.MemberMail, false);
                    return RedirectToAction("MyProfile", "User");

                }else if (member.MemberRole == "R")
                {
                    MEMBER m = FoodOrder.MEMBER.FirstOrDefault(x=>x.MemberMail==member.MemberMail);
                    RESTAURANT restaurant = new RESTAURANT();
                    restaurant.RestaurantId = m.MemberId;
                    restaurant.RestaurantMail = m.MemberMail;
                    restaurant.RestaurantPassword = m.MemberPassword;
                    restaurant.RestaurantName = m.MemberMail;
                    FoodOrder.RESTAURANT.Add(restaurant);
                    FoodOrder.SaveChanges();
                    memberInDb = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberMail == member.MemberMail);
                    FormsAuthentication.SetAuthCookie(memberInDb.MemberMail, false);
                    return RedirectToAction("MyRestaurant", "Restaurant");
                }
                else
                {
                    memberInDb = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberMail == member.MemberMail);
                    FormsAuthentication.SetAuthCookie(memberInDb.MemberMail, false);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View();
            }

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Home");
        }
    }
}