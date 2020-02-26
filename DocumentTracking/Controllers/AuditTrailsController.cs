using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DocumentTracking.Models;
using DocumentTracking.Models.DataModels;

namespace DocumentTracking.Controllers
{
    [Authorize(Roles ="Super Admin")]
    public class AuditTrailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AuditTrails
        public ActionResult Display()
        {
            return View(db.AuditTrails.ToList());
        }
        //public ActionResult Index()
        //{
        //    return View(db.AuditTrails.ToList());
        //}

        //// GET: AuditTrails/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AuditTrail auditTrail = db.AuditTrails.Find(id);
        //    if (auditTrail == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(auditTrail);
        //}

        //// GET: AuditTrails/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: AuditTrails/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,UserID,TDate,Action,MainID,RecordID,Module")] AuditTrail auditTrail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.AuditTrails.Add(auditTrail);
        //        //db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(auditTrail);
        //}

        //// GET: AuditTrails/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AuditTrail auditTrail = db.AuditTrails.Find(id);
        //    if (auditTrail == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(auditTrail);
        //}

        //// POST: AuditTrails/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,UserID,TDate,Action,MainID,RecordID,Module")] AuditTrail auditTrail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(auditTrail).State = EntityState.Modified;
        //        //db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(auditTrail);
        //}

        //// GET: AuditTrails/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    AuditTrail auditTrail = db.AuditTrails.Find(id);
        //    if (auditTrail == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(auditTrail);
        //}

        //// POST: AuditTrails/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    AuditTrail auditTrail = db.AuditTrails.Find(id);
        //    db.AuditTrails.Remove(auditTrail);
        //    //db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
