using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FoodOrderSystem.Database;

namespace FoodOrderSystem.Controllers
{
    public class RestorentsController : Controller
    {
        private FoodOrderSystemEntities db = new FoodOrderSystemEntities();

        // GET: Restorents
        public ActionResult Index()
        {
            return View(db.Restorents.ToList());
        }

        // GET: Restorents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restorent restorent = db.Restorents.Find(id);
            if (restorent == null)
            {
                return HttpNotFound();
            }
            return View(restorent);
        }

        // GET: Restorents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Restorents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Address,City,PinCode")] Restorent restorent)
        {
            if (ModelState.IsValid)
            {
                db.Restorents.Add(restorent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(restorent);
        }

        // GET: Restorents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restorent restorent = db.Restorents.Find(id);
            if (restorent == null)
            {
                return HttpNotFound();
            }
            return View(restorent);
        }

        // POST: Restorents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Address,City,PinCode")] Restorent restorent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(restorent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(restorent);
        }

        // GET: Restorents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restorent restorent = db.Restorents.Find(id);
            if (restorent == null)
            {
                return HttpNotFound();
            }
            return View(restorent);
        }

        // POST: Restorents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Restorent restorent = db.Restorents.Find(id);
            db.Restorents.Remove(restorent);
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
