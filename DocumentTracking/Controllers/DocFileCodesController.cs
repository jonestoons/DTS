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
    public class DocFileCodesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DocFileCodes
        public ActionResult Index()
        {
            var docFileCodes = db.DocFileCodes.Include(d => d.Unit);
            return View(docFileCodes.ToList());
        }

        // GET: DocFileCodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocFileCode docFileCode = db.DocFileCodes.Find(id);
            if (docFileCode == null)
            {
                return HttpNotFound();
            }
            return View(docFileCode);
        }

        // GET: DocFileCodes/Create
        public ActionResult Create()
        {
            ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description");
            return View();
        }

        // POST: DocFileCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UnitId,Description")] DocFileCode docFileCode)
        {
            if (ModelState.IsValid)
            {
                db.DocFileCodes.Add(docFileCode);
                db.SaveChanges();
                Audit.Trail(docFileCode.Description, eAction.Create, docFileCode.Id.ToString());
                return RedirectToAction("Index");
            }

            ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", docFileCode.UnitId);
            return View(docFileCode);
        }

        // GET: DocFileCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocFileCode docFileCode = db.DocFileCodes.Find(id);
            if (docFileCode == null)
            {
                return HttpNotFound();
            }
            ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", docFileCode.UnitId);
            return View(docFileCode);
        }

        // POST: DocFileCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UnitId,Description")] DocFileCode docFileCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(docFileCode).State = EntityState.Modified;
                db.SaveChanges();
                Audit.Trail(docFileCode.Description, eAction.Update, docFileCode.Id.ToString());
                return RedirectToAction("Index");
            }
            ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", docFileCode.UnitId);
            return View(docFileCode);
        }

        // GET: DocFileCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocFileCode docFileCode = db.DocFileCodes.Find(id);
            if (docFileCode == null)
            {
                return HttpNotFound();
            }
            return View(docFileCode);
        }

        // POST: DocFileCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocFileCode docFileCode = db.DocFileCodes.Find(id);
            db.DocFileCodes.Remove(docFileCode);
            db.SaveChanges();
            Audit.Trail(docFileCode.Description, eAction.Delete, docFileCode.Id.ToString());
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
