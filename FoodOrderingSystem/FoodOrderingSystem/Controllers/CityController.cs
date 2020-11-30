using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using yasemin.Models;

namespace yasemin.Controllers
{
    public class CityController : Controller
    {
        private DbFoodOrderingSystemEntities db = new DbFoodOrderingSystemEntities();

        // GET: CITies
        public ActionResult Index()
        {
            return View(db.CITY.ToList());
        }

        // GET: CITies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CITY cITY = db.CITY.Find(id);
            if (cITY == null)
            {
                return HttpNotFound();
            }
            return View(cITY);
        }

        // GET: CITies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CITies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CityId,CityName")] CITY cITY)
        {
            if (ModelState.IsValid)
            {
                db.CITY.Add(cITY);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cITY);
        }

        // GET: CITies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CITY cITY = db.CITY.Find(id);
            if (cITY == null)
            {
                return HttpNotFound();
            }
            return View(cITY);
        }

        // POST: CITies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CityId,CityName")] CITY cITY)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cITY).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cITY);
        }

        // GET: CITies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CITY cITY = db.CITY.Find(id);
            if (cITY == null)
            {
                return HttpNotFound();
            }
            return View(cITY);
        }

        // POST: CITies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CITY cITY = db.CITY.Find(id);
            db.CITY.Remove(cITY);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
