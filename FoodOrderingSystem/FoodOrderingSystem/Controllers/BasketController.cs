using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yasemin.Models;

namespace yasemin.Controllers
{
    public class BasketController : Controller
    {
        // GET: Basket
        DbFoodOrderingSystemEntities FoodOrder = new DbFoodOrderingSystemEntities();
        public ActionResult Index()
        {
            int total = 0;
            int RestaurantId = 0;
            if (BASKET.Foods.Count > 0)
            {
                total = BASKET.Foods.Sum(x => x.TotalPrice);
                foreach (var item in BASKET.Foods)
                {
                    RestaurantId = item.RestaurantId;
                }
            }
            ViewBag.RestaurantId = RestaurantId;
            ViewBag.total = "Total price of your basket: " + total + " TL";
            return View();
        }

        [HttpPost]
        public void BasketAdd(int id)
        {
            FOOD food = FoodOrder.FOOD.FirstOrDefault(x => x.FoodId == id);
            bool FoodExist = false;
            foreach (var item in BASKET.Foods)
            {
                if (item.RestaurantId == food.RestaurantId)
                {
                    if (item.FoodId == id)
                    {
                        FoodExist = true;
                        item.TotalPrice += item.FoodPrice;
                        item.FoodAmount++;
                    }
                }
            }
            int RestaurantId = 0;
            bool BasketHasFoods = false;
            if (BASKET.Foods.Count > 0)
            {
                foreach (var item in BASKET.Foods)
                {
                    RestaurantId = item.RestaurantId;
                    BasketHasFoods = true;
                }
            }
            if (FoodExist == false)
            {
                if (BasketHasFoods == true && food.RestaurantId!=RestaurantId)
                {
                    
                }
                else
                {
                    food.TotalPrice += food.FoodPrice;
                    food.FoodAmount++;
                    BASKET.AddFood(food);
                }
            }

        }

        [HttpPost]
        public void Increase(int id)
        {
            foreach (var item in BASKET.Foods)
            {
                if (item.FoodId == id)
                {
                    item.TotalPrice += item.FoodPrice;
                    item.FoodAmount++;
                }
            }
        }

        [HttpPost]
        public void Decrease(int id)
        {
            foreach (var item in BASKET.Foods)
            {
                if (item.FoodId == id)
                {
                    if (item.FoodAmount > 1)
                    {
                        item.FoodAmount--;
                        item.TotalPrice -= item.FoodPrice;
                    }
                }
            }
        }

        [HttpPost]
        public void Delete(int id)
        {
            FOOD deleteFood = new FOOD();
            foreach (var item in BASKET.Foods)
            {
                if (item.FoodId == id)
                {
                    deleteFood = item;
                }
            }
            BASKET.DeleteFood(deleteFood);
        }

        [HttpPost]
        public void DeleteAll()
        {
            BASKET.DeleteAll();
        }
    }
}