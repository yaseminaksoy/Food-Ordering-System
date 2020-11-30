using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yasemin.Models;

namespace yasemin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        DbFoodOrderingSystemEntities FoodOrder = new DbFoodOrderingSystemEntities();
        
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
            
            List<CITY> CITIES = FoodOrder.CITY.ToList();
            ViewBag.cities = CITIES;
            return View(restaurants);
        }
     
        

        [HttpPost]
        public ActionResult RestaurantListele(int CityId, string searchText)
        {
            if(CityId==0 & searchText=="")
            {
                return RedirectToAction("Index");
            }
            
            List<RESTAURANT> restaurants = new List<RESTAURANT>();
            List<CITY_RESTAURANT> city_restaurant = FoodOrder.CITY_RESTAURANT.Where(x => x.CityId == CityId).ToList();
            List<CONCEPT> concepts = FoodOrder.CONCEPT.Where(x => x.ConceptName.Contains(searchText)).ToList();
            if(searchText!="" & searchText.Trim().Length >= 0 & CityId==0)
            {
                foreach (RESTAURANT item in FoodOrder.RESTAURANT)
                {
                    if (item.RestaurantName.Contains(searchText))
                    {
                        restaurants.Add(item);
                    }
                    else if(concepts.Count>0) {
                        foreach (CONCEPT_RESTAURANT cr in FoodOrder.CONCEPT_RESTAURANT)
                    {
                        foreach (CONCEPT concept in concepts)
                        {
                            if(concept.ConceptId == cr.ConceptId)
                            {
                                RESTAURANT restaurant = FoodOrder.RESTAURANT.FirstOrDefault(x => x.RestaurantId == cr.RestaurantId);
                                    if (!restaurants.Contains(restaurant))
                                    {
                                        restaurants.Add(restaurant);
                                    }
                            }
                        }
                    }
                    }
                    
                }
            }else if(CityId!=0 & searchText == "" & searchText.Trim().Length <= 0)
            {
                foreach (RESTAURANT item in FoodOrder.RESTAURANT)
                {
                    foreach (CITY_RESTAURANT cr in city_restaurant)
                    {
                        if(cr.RestaurantId == item.RestaurantId & !restaurants.Contains(item))
                        {
                            restaurants.Add(item);
                        }
                    }
                }
            }else if(CityId!=0 & searchText!="" & searchText.Trim().Length >= 0){
                foreach (RESTAURANT item in FoodOrder.RESTAURANT)
                {
                    foreach (CITY_RESTAURANT cir in city_restaurant)
                    {
                        if(cir.RestaurantId == item.RestaurantId)
                        {
                            if (item.RestaurantName.Contains(searchText) & !restaurants.Contains(item))
                            {
                                restaurants.Add(item);
                            }
                            else
                            {
                                foreach (CONCEPT_RESTAURANT cor in FoodOrder.CONCEPT_RESTAURANT)
                                {
                                    foreach (CONCEPT concept in concepts)
                                    {
                                        if(cor.ConceptId == concept.ConceptId)
                                        {
                                            if (item.RestaurantId==cor.RestaurantId &!restaurants.Contains(item))
                                            {
                                                restaurants.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

                List<CITY> CITIES = FoodOrder.CITY.ToList();
                ViewBag.cities = CITIES;
                return View("Index",restaurants);
        }
    }
}