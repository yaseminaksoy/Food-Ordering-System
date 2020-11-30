using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using yasemin.Models;

namespace yasemin.Controllers
{
    //[Authorize(Roles ="A,R,U")]
    public class RestaurantController : Controller
    {

        // GET: Restaurant
        DbFoodOrderingSystemEntities FoodOrder = new DbFoodOrderingSystemEntities();
        [Authorize(Roles = "A,R")]
        public ActionResult Index()
        {
            List<RESTAURANT> restaurants = FoodOrder.RESTAURANT.ToList();

            foreach (var item in restaurants)
            {
                if (FoodOrder.SCORE.Where(x => x.RestaurantId == item.RestaurantId).ToList().Count == 0)
                {
                    item.RestaurantScore = 0;
                }
                else
                {
                    item.RestaurantScore = item.RestaurantScore / FoodOrder.SCORE.Where(x => x.RestaurantId == item.RestaurantId).ToList().Count;
                    item.RestaurantScore = Double.Parse(item.RestaurantScore.ToString("0.##"));
                }
            }
            List<CITY> cities = FoodOrder.CITY.ToList();
            ViewBag.cities = cities;
            List<CATEGORY> categories = FoodOrder.CATEGORY.ToList();
            ViewBag.categories = categories;

            return View(restaurants);
        }
        [Authorize(Roles = "A,R")]
        [HttpPost]
        public ActionResult Index(int CityId,string searchText)
        {
            if (CityId == 0 & searchText == "")
            {
                return RedirectToAction("Index");
            }
            List<RESTAURANT> restaurants = new List<RESTAURANT>();
            List<CITY_RESTAURANT> city_restaurant = FoodOrder.CITY_RESTAURANT.Where(x => x.CityId == CityId).ToList();
            List<CITY> cities = FoodOrder.CITY.ToList();
            ViewBag.cities = cities;
            

            if(searchText != "" & searchText.Trim().Length >= 0 & CityId == 0)
            {
                foreach (RESTAURANT item in FoodOrder.RESTAURANT)
                {
                    if (item.RestaurantName.Contains(searchText))
                    {
                        restaurants.Add(item);
                    }
                }
            }else if(CityId!=0 & searchText == "" & searchText.Trim().Length <= 0)
            {
                foreach (RESTAURANT item in FoodOrder.RESTAURANT)
                {
                    foreach (CITY_RESTAURANT cr in city_restaurant)
                    {
                        if (cr.RestaurantId == item.RestaurantId & !restaurants.Contains(item))
                        {
                            restaurants.Add(item);
                        }
                    }
                }
            }else if(CityId != 0 & searchText != "" & searchText.Trim().Length >= 0)
            {
                foreach (RESTAURANT item in FoodOrder.RESTAURANT)
                {
                    foreach (CITY_RESTAURANT cir in city_restaurant)
                    {
                        if (cir.RestaurantId == item.RestaurantId)
                        {
                            if (item.RestaurantName.Contains(searchText) & !restaurants.Contains(item))
                            {
                                restaurants.Add(item);
                            }
                            
                        }
                    }
                }
            }
            return View(restaurants);
        }

        public ActionResult List(int id)
        {
            RESTAURANT restaurant = FoodOrder.RESTAURANT.FirstOrDefault(x => x.RestaurantId == id);
            ViewBag.restaurant = restaurant;
            List<FOOD> foods = FoodOrder.FOOD.Where(x=>x.RestaurantId==restaurant.RestaurantId).ToList();
            List<CATEGORY> categories = new List<CATEGORY>();
            foreach (CATEGORY item in FoodOrder.CATEGORY)
            {
                foreach (FOOD food in foods)
                {
                    if (food.CategoryId == item.CategoryId && !categories.Contains(item))
                    {
                        categories.Add(item);
                    }
                }
            }
            ViewBag.categories = categories;
            return View(foods);
        }
        
        [HttpPost]
        public ActionResult List(int RestaurantId, int CategoryId)
        {
            RESTAURANT restaurant = FoodOrder.RESTAURANT.FirstOrDefault(x => x.RestaurantId == RestaurantId);
            ViewBag.restaurant = restaurant;
            List<FOOD> foods = FoodOrder.FOOD.Where(x => x.RestaurantId == restaurant.RestaurantId).ToList();
            List<CATEGORY> categories = new List<CATEGORY>();
            foreach (CATEGORY item in FoodOrder.CATEGORY)
            {
                foreach (FOOD food in foods)
                {
                    if (food.CategoryId == item.CategoryId && !categories.Contains(item))
                    {
                        categories.Add(item);
                    }
                }
            }
            ViewBag.categories = categories;

            if (CategoryId == 0)
            {
                return View(foods);
            }

            List<FOOD> filter_foods = new List<FOOD>();
            try
            {
            
                foreach (FOOD item in foods)
                {
                    if (item.CategoryId == CategoryId)
                    {
                        filter_foods.Add(item);
                    }
                }
            }catch(Exception e)
            {

            }
            

            return View(filter_foods);
        }

        [Authorize(Roles = "A,R")]
        public ActionResult Add()
        {
            RESTAURANT r = new RESTAURANT();
            return View(r);
        }

        [Authorize(Roles = "A,R")]
        [HttpPost]
        public ActionResult Add(RESTAURANT restaurant)
        {
            MEMBER m = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberId == restaurant.RestaurantId);
            if (m == null)
            {
                m = new MEMBER();
                m.MemberMail = restaurant.RestaurantMail;
                m.MemberPassword = restaurant.RestaurantPassword;
                m.MemberRole = "R";
                FoodOrder.MEMBER.Add(m);
                FoodOrder.RESTAURANT.AddOrUpdate(restaurant);
                FoodOrder.SaveChanges();

            }
            else
            {
                ViewBag.city_restaurant = FoodOrder.CITY_RESTAURANT.Where(x => x.RestaurantId == restaurant.RestaurantId);
                m.MemberMail = restaurant.RestaurantMail;
                m.MemberPassword = restaurant.RestaurantPassword;
                m.MemberRole = "R";
                FoodOrder.MEMBER.AddOrUpdate(m);
                FoodOrder.SaveChanges();
                FoodOrder.RESTAURANT.AddOrUpdate(restaurant);
                FoodOrder.SaveChanges();
                FormsAuthentication.SetAuthCookie(m.MemberMail, false);
            }
            if (User.IsInRole("R"))
            {
                return RedirectToAction("MyRestaurant");
            }
            else
            {
                return RedirectToAction("Index");
            }
            
            
            
            
        }
        [Authorize(Roles = "A,R")]
        public ActionResult Details(int id)
        {
            RESTAURANT restaurant = FoodOrder.RESTAURANT.FirstOrDefault(x => x.RestaurantId == id);
            List<CITY> cities = FoodOrder.CITY.ToList();
            ViewBag.cities = cities;
            List<CONCEPT> concepts = FoodOrder.CONCEPT.ToList();
            ViewBag.concepts = concepts;
            List<CATEGORY> categories = FoodOrder.CATEGORY.ToList();
            ViewBag.categories = categories;

            return View(restaurant);
        }
        [Authorize(Roles = "A,R")]
        public ActionResult Update(int id)
        {
            RESTAURANT restaurant = FoodOrder.RESTAURANT.FirstOrDefault(x => x.RestaurantId == id);
            MEMBER m= FoodOrder.MEMBER.FirstOrDefault(x => x.MemberId == id);
            if (m != null)
            {
                restaurant.RestaurantMail = m.MemberMail;
                restaurant.RestaurantPassword = m.MemberPassword;
            }
            ViewBag.cities = FoodOrder.CITY_RESTAURANT.Where(x => x.RestaurantId == restaurant.RestaurantId).ToList();
            ViewBag.concepts = FoodOrder.CONCEPT_RESTAURANT.Where(x => x.RestaurantId == restaurant.RestaurantId).ToList();
            return View("Add",restaurant);
        }

        [Authorize(Roles = "A")]
        [HttpPost]
        public void Delete(int id)
        {
            RESTAURANT restaurant = FoodOrder.RESTAURANT.FirstOrDefault(x => x.RestaurantId == id);
            List<CITY_RESTAURANT> city_restaurant = FoodOrder.CITY_RESTAURANT.Where(x => x.RestaurantId == id).ToList();
            foreach (CITY_RESTAURANT item in city_restaurant)
            {
                FoodOrder.CITY_RESTAURANT.Remove(item);
            }
            List<CONCEPT_RESTAURANT> concept_restaurant = FoodOrder.CONCEPT_RESTAURANT.Where(x => x.RestaurantId == id).ToList();
            foreach (CONCEPT_RESTAURANT item in concept_restaurant)
            {
                FoodOrder.CONCEPT_RESTAURANT.Remove(item);
            }
            MEMBER member = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberId == id);
            FoodOrder.MEMBER.Remove(member);
            FoodOrder.RESTAURANT.Remove(restaurant);
            FoodOrder.SaveChanges();
            
        }

        [Authorize(Roles = "A,R")]
        [HttpPost]
        public void CityDelete(int id)
        {
            CITY_RESTAURANT city_restaurant = FoodOrder.CITY_RESTAURANT.FirstOrDefault(x => x.CityRestaurantId == id);
            FoodOrder.CITY_RESTAURANT.Remove(city_restaurant);
            FoodOrder.SaveChanges();
        }

        [Authorize(Roles = "A,R")]
        [HttpPost]
        public void ConceptDelete(int id)
        {
            CONCEPT_RESTAURANT concept_restaurant = FoodOrder.CONCEPT_RESTAURANT.FirstOrDefault(x => x.ConceptId == id);
            FoodOrder.CONCEPT_RESTAURANT.Remove(concept_restaurant);
            FoodOrder.SaveChanges();
        }

        [Authorize(Roles = "A,R")]
        [HttpPost]
        public void FoodDelete(int id)
        {
            FOOD food = FoodOrder.FOOD.FirstOrDefault(x => x.FoodId == id);
            FoodOrder.FOOD.Remove(food);
            FoodOrder.SaveChanges();
        }

        [Authorize(Roles = "A,R")]
        [HttpPost]
        public ActionResult CityAdd(int CityId, int RestaurantId)
        {
            if (CityId != 0)
            {
                CITY_RESTAURANT cr = new CITY_RESTAURANT();
                cr.CityId = CityId;
                cr.RestaurantId = RestaurantId;
                FoodOrder.CITY_RESTAURANT.Add(cr);
                FoodOrder.SaveChanges();
            }
            if (User.IsInRole("R"))
            {
                return RedirectToAction("MyRestaurant");
            }
            return RedirectToAction("Details", new { id = RestaurantId });
        }

        [Authorize(Roles = "A,R")]
        [HttpPost]
        public ActionResult ConceptAdd(int ConceptId, string searchText, int RestaurantId)
        {
            if(ConceptId!=0)
            {
                bool ConceptExist = false;
                foreach (CONCEPT_RESTAURANT item in FoodOrder.CONCEPT_RESTAURANT)
                {
                    if(item.ConceptId==ConceptId && item.RestaurantId == RestaurantId)
                    {
                        ConceptExist = true;
                    }
                }
                if (ConceptExist == false)
                {
                    CONCEPT_RESTAURANT cr = new CONCEPT_RESTAURANT();
                    cr.ConceptId = ConceptId;
                    cr.RestaurantId = RestaurantId;
                    FoodOrder.CONCEPT_RESTAURANT.Add(cr);
                    FoodOrder.SaveChanges();
                }
                
            }else if(ConceptId==0 && searchText != "" & searchText.Trim().Length >= 0)
            {
                bool ConceptExist = false;
                foreach (CONCEPT item in FoodOrder.CONCEPT)
                {
                    if (item.ConceptName == searchText)
                    {
                        ConceptExist = true;
                        ConceptId = item.ConceptId;
                        CONCEPT_RESTAURANT cr = new CONCEPT_RESTAURANT();
                        cr.ConceptId = ConceptId;
                        cr.RestaurantId = RestaurantId;
                        FoodOrder.CONCEPT_RESTAURANT.Add(cr);
                        FoodOrder.SaveChanges();
                    }
                }
                if (ConceptExist == false)
                {
                    CONCEPT concept = new CONCEPT();
                    concept.ConceptName = searchText;
                    FoodOrder.CONCEPT.Add(concept);
                    FoodOrder.SaveChanges();
                    CONCEPT_RESTAURANT cr = new CONCEPT_RESTAURANT();
                    cr.ConceptId = FoodOrder.CONCEPT.FirstOrDefault(x => x.ConceptName == searchText).ConceptId;
                    cr.RestaurantId = RestaurantId;
                    FoodOrder.CONCEPT_RESTAURANT.Add(cr);
                    FoodOrder.SaveChanges();
                }
            }
            if (User.IsInRole("R"))
            {
                return RedirectToAction("MyRestaurant");
            }
            return RedirectToAction("Details", new { id = RestaurantId });
        }
        [Authorize(Roles = "R")]
        public ActionResult MyRestaurant()
        {
            if (User.Identity.IsAuthenticated)
            {
                List<CITY> cities = FoodOrder.CITY.ToList();
                ViewBag.cities = cities;
                List<CONCEPT> concepts = FoodOrder.CONCEPT.ToList();
                ViewBag.concepts = concepts;
                string mail = User.Identity.Name;
                MEMBER m = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberMail == mail);
                RESTAURANT restaurant = FoodOrder.RESTAURANT.FirstOrDefault(x => x.RestaurantId == m.MemberId);
                restaurant.RestaurantMail = m.MemberMail;
                restaurant.RestaurantPassword = m.MemberPassword;
                List<CATEGORY> categories = new List<CATEGORY>();
                foreach (FOOD food in FoodOrder.FOOD)
                {
                    foreach (CATEGORY category in FoodOrder.CATEGORY)
                    {
                        if (category.CategoryId == food.CategoryId && !categories.Contains(category))
                        {
                            categories.Add(category);
                        }
                    }
                }
                ViewBag.categories = categories;
                return View(restaurant);
            }
            return HttpNotFound();
        }


        [Authorize(Roles = "A,R")]
        
        public ActionResult FoodAdd(int id)
        {
            RESTAURANT restaurant = FoodOrder.RESTAURANT.FirstOrDefault(x=>x.RestaurantId==id);
            List<CATEGORY> categories = FoodOrder.CATEGORY.ToList();
            ViewBag.categories = categories;
            FOOD f = new FOOD();
            f.RestaurantId = restaurant.RestaurantId;
            ViewBag.RestaurantId = restaurant.RestaurantId;
            return View(f);
        }

        [Authorize(Roles = "A,R")]
        [HttpPost]
        public ActionResult FoodAdd(FOOD food, string CategoryName,int RestaurantId)
        {
            //string n = User.Identity.Name;
            //int rid = FoodOrder.MEMBER.FirstOrDefault(x => x.MemberMail == n).MemberId;
            food.RestaurantId = RestaurantId;
            List<CATEGORY> categories = FoodOrder.CATEGORY.ToList();
            ViewBag.categories = categories;
            bool CategoryExist = false;
            bool CategoryNameExist = false;
            int cid=0;
            foreach (CATEGORY item in FoodOrder.CATEGORY)
            {
                if (item.CategoryId == food.CategoryId)
                {
                    CategoryExist = true;
                }
                if (item.CategoryName == CategoryName)
                {
                    CategoryNameExist = true;
                    cid = item.CategoryId;
                }
            }
            if (CategoryExist == true)
            {
                FoodOrder.FOOD.AddOrUpdate(food);
                FoodOrder.SaveChanges();
            }
            else
            {
                if(CategoryName!="" && CategoryName.Trim().Length >= 0)
                {
                    if (CategoryNameExist==false)
                    {
                        CATEGORY cat = new CATEGORY();
                        cat.CategoryName = CategoryName;
                        FoodOrder.CATEGORY.Add(cat);
                        FoodOrder.SaveChanges();
                        food.CategoryId = FoodOrder.CATEGORY.FirstOrDefault(x => x.CategoryName == CategoryName).CategoryId;
                        FoodOrder.FOOD.AddOrUpdate(food);
                        FoodOrder.SaveChanges();
                    }
                    else
                    {
                        food.CategoryId = cid;
                        FoodOrder.FOOD.AddOrUpdate(food);
                        FoodOrder.SaveChanges();
                    }
                    
                }
               
                
            }
            //FoodOrder.SaveChanges();    
            
            if (User.IsInRole("R"))
            {
                return RedirectToAction("MyRestaurant");
            }
            return RedirectToAction("Details", new { id = RestaurantId });

        }

        [Authorize(Roles = "A,R")]
        public ActionResult FoodUpdate(int id)
        {
            FOOD food = FoodOrder.FOOD.FirstOrDefault(x => x.FoodId == id);
            List<CATEGORY> categories = FoodOrder.CATEGORY.ToList();
            ViewBag.categories = categories;
            
            return View("FoodAdd", food);
        }
    }
}