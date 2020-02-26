
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
    public class MsgSetupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MsgSetups
        public ActionResult Index()
        {
            return View(db.MsgSetups.ToList());
        }

        // GET: MsgSetups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MsgSetup msgSetup = db.MsgSetups.Find(id);
            if (msgSetup == null)
            {
                return HttpNotFound();
            }
            return View(msgSetup);
        }

        // GET: MsgSetups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MsgSetups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,Password,Port,Host,Backlog")] MsgSetup msgSetup)
        {
            if (ModelState.IsValid)
            {
                db.MsgSetups.Add(msgSetup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(msgSetup);
        }

        // GET: MsgSetups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MsgSetup msgSetup = db.MsgSetups.Find(id);
            if (msgSetup == null)
            {
                return HttpNotFound();
            }
            return View(msgSetup);
        }

        // POST: MsgSetups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Password,Port,Host,Backlog")] MsgSetup msgSetup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(msgSetup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(msgSetup);
        }

        // GET: MsgSetups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MsgSetup msgSetup = db.MsgSetups.Find(id);
            if (msgSetup == null)
            {
                return HttpNotFound();
            }
            return View(msgSetup);
        }

        // POST: MsgSetups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MsgSetup msgSetup = db.MsgSetups.Find(id);
            db.MsgSetups.Remove(msgSetup);
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
