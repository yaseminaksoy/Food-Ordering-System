using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace yasemin.Models
{
    public static class BASKET
    {
        static List<FOOD> basket = null;

        static BASKET()
        {
            basket = new List<FOOD>();
        }
        public static List<FOOD> Foods
        {
            get
            {
                return basket;
            }
        }

        public static void AddFood(FOOD entity)
        {
            basket.Add(entity);
        }

        public static void DeleteFood(FOOD entity)
        {
            basket.Remove(entity);
        }

        public static void DeleteAll()
        {
            basket.Clear();
        }

    }
}