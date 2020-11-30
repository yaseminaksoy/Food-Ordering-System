using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using yasemin.Models;

namespace yasemin.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        DbFoodOrderingSystemEntities FoodOrder = new DbFoodOrderingSystemEntities();
        List<FOOD> basket = new List<FOOD>();

        //[Authorize]
        [Authorize(Roles = "A")]
        public ActionResult Index()
        {
            List<USERS> users = FoodOrder.USERS.ToList();
            return View(users);
        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public ActionResult Index(string searchText)
        {
            List<USERS> users = FoodOrder.USERS.Where(x => x.UserName.Contains(searchText)).ToList();
            return View(users);
        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public void Delete(int id)
        {
            USERS user = FoodOrder.USERS.FirstOrDefault(x => x.UserId == id);
            List<USER_ADDRESS> user_address = FoodOrder.USER_ADDRESS.Where(x => x.UserId == id).ToList();
            foreach (USER_ADDRESS item in user_address)
            {
                FoodOrder.USER_ADDRESS.Remove(item);
            }
            MEMBER m = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberId == id);
            FoodOrder.MEMBER.Remove(m);
            FoodOrder.USERS.Remove(user);
            FoodOrder.SaveChanges();
        }
        [Authorize(Roles = "A,U")]
        public ActionResult Add()
        {
            USERS u = new USERS();
            return View(u);
        }
        [Authorize(Roles = "A,U")]
        [HttpPost]
        public ActionResult Add(USERS user)
        {
            MEMBER m = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberId == user.UserId);
            if (m == null)
            {
                m = new MEMBER();
                m.MemberMail = user.UserMail;
                m.MemberPassword = user.UserPassword;
                m.MemberRole = "U";
                FoodOrder.MEMBER.Add(m);
                FoodOrder.USERS.AddOrUpdate(user);
                FoodOrder.SaveChanges();

            }
            else
            {
                m.MemberMail = user.UserMail;
                m.MemberPassword = user.UserPassword;
                m.MemberRole = "U";
                FoodOrder.MEMBER.AddOrUpdate(m);
                FoodOrder.SaveChanges();
                FoodOrder.USERS.AddOrUpdate(user);
                FoodOrder.SaveChanges();
                FormsAuthentication.SetAuthCookie(m.MemberMail, false);
            }
            if (User.IsInRole("U"))
            {
                return RedirectToAction("MyProfile");
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "A,U")]
        public ActionResult Update(int id)
        {
            USERS user = FoodOrder.USERS.FirstOrDefault(x => x.UserId == id);
            
            MEMBER m = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberId == id);
            if (m != null)
            {
                user.UserMail = m.MemberMail;
                user.UserPassword = m.MemberPassword;
            }
            return View("Add", user);
        }
        [Authorize(Roles = "A,U")]
        public ActionResult Address(int id)
        {
            List<USER_ADDRESS> user_address = FoodOrder.USER_ADDRESS.Where(x => x.UserId == id).ToList();
            ViewBag.userId = id;
            return View(user_address);
        }
        [Authorize(Roles = "A,U")]
        public ActionResult AddressAdd(int id)
        {
            USER_ADDRESS ua = new USER_ADDRESS();
            ua.UserId = id;
            return View(ua);
        }
        [Authorize(Roles = "A,U")]
        [HttpPost]
        public ActionResult AddressAdd(USER_ADDRESS ua)
        {
            FoodOrder.USER_ADDRESS.AddOrUpdate(ua);
            FoodOrder.SaveChanges();
            if (User.IsInRole("A"))
            {
                return RedirectToAction("Address", new { id = ua.UserId });
            }else if (User.IsInRole("U"))
            {
                return RedirectToAction("MyProfile");
            }
            return View(ua);
        }
        [Authorize(Roles = "A,U")]
        public ActionResult AddressUpdate(int id)
        {
            USER_ADDRESS user_address = FoodOrder.USER_ADDRESS.FirstOrDefault(x => x.AddressId == id);
            
            return View("AddressAdd", user_address);
        }
        [Authorize(Roles = "A,U")]
        [HttpPost]
        public void AddressDelete(int id)
        {
            USER_ADDRESS ua = FoodOrder.USER_ADDRESS.FirstOrDefault(x => x.AddressId == id);
            FoodOrder.USER_ADDRESS.Remove(ua);
            FoodOrder.SaveChanges();
        }
        [Authorize(Roles ="U")]
        public ActionResult MyProfile()
        {
            if (User.Identity.IsAuthenticated)
            {
                string mail = User.Identity.Name;
                MEMBER m = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberMail == mail);
                USERS user = FoodOrder.USERS.FirstOrDefault(x => x.UserId == m.MemberId);
                user.UserMail = m.MemberMail;
                user.UserPassword = m.MemberPassword;
                ViewBag.order_food = FoodOrder.ORDER_FOOD.ToList();
                ViewBag.foods = FoodOrder.FOOD.ToList();
                ViewBag.restaurants = FoodOrder.RESTAURANT.ToList();
                return View(user);
            }

            return HttpNotFound();
        }

        [Authorize(Roles ="U")]
        public ActionResult Buy()
        {
            if (BASKET.Foods.Count > 0)
            {
                MEMBER m = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberMail == User.Identity.Name);
                USERS user = FoodOrder.USERS.FirstOrDefault(x => x.UserId == m.MemberId);

                List<USER_ADDRESS> user_address = FoodOrder.USER_ADDRESS.ToList();
                ViewBag.user_address = user_address;

                int total = 0;
                if (BASKET.Foods.Count > 0)
                {
                    total = BASKET.Foods.Sum(x => x.TotalPrice);
                }
                ViewBag.total = "   Total price of your basket: " + total + " TL";
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
        [Authorize(Roles = "U")]
        [HttpPost]
        public ActionResult Buy(int AddressId)
        {
            if (BASKET.Foods.Count > 0)
            {
                MEMBER m = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberMail == User.Identity.Name);
                USERS user = FoodOrder.USERS.FirstOrDefault(x => x.UserId == m.MemberId);
                ORDERS order = new ORDERS();
                order.UserId = user.UserId;
                foreach (var item in BASKET.Foods)
                {
                    order.RestaurantId = item.RestaurantId;
                }
                order.GiveScore = "false";
                FoodOrder.ORDERS.Add(order);
                FoodOrder.SaveChanges();
                List<ORDERS> user_orders = FoodOrder.ORDERS.Where(x => x.UserId == user.UserId).ToList();
                bool OrderExist = false;
                int orderId = 0;
                if (FoodOrder.ORDER_FOOD.ToList().Count > 0)
                {
                    foreach (var item in FoodOrder.ORDER_FOOD)
                    {
                        foreach (var uo in user_orders)
                        {
                            if (uo.OrderId == item.OrderId)
                            {
                                OrderExist = true;
                            }
                            else
                            {
                                OrderExist = false;
                                orderId = uo.OrderId;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in FoodOrder.ORDERS)
                    {
                        orderId = item.OrderId;
                    }
                }
                if (OrderExist == false)
                {
                    List<int> food_id = new List<int>();
                    
                    foreach (var item in BASKET.Foods)
                    {
                        ORDER_FOOD of = new ORDER_FOOD();
                        of.OrderId = orderId;
                        of.FoodId = item.FoodId;
                        of.FoodAmount = (int) item.FoodAmount;
                        of.FoodTotalPrice = item.TotalPrice;

                        FoodOrder.ORDER_FOOD.Add(of);
                        FoodOrder.SaveChanges();
                    }
                }
                
                BASKET.DeleteAll();
                return RedirectToAction("MyProfile");
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }
        [Authorize(Roles = "A,U")]
        [HttpPost]
        public ActionResult GiveScore(int Score,int RestaurantId,int OrderId)
        {
            ORDERS order = FoodOrder.ORDERS.FirstOrDefault(x=>x.OrderId==OrderId);
            order.GiveScore = "true";
            FoodOrder.ORDERS.AddOrUpdate(order);
            SCORE score = new SCORE();
            score.RestaurantId = RestaurantId;
            score.Score1 = Score;
            FoodOrder.SCORE.Add(score);
            RESTAURANT restaurant = FoodOrder.RESTAURANT.FirstOrDefault(x=>x.RestaurantId==RestaurantId);
            restaurant.RestaurantScore += Score;
            FoodOrder.SaveChanges();
            return RedirectToAction("MyProfile");
        }
    }
}