using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DocumentTracking.Models;

namespace DocumentTracking.Controllers
{
    [Authorize]
    public class FileTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FileTypes
        public ActionResult Index()
        {
            return View(db.FileTypes.ToList());
        }

        // GET: FileTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileType fileType = db.FileTypes.Find(id);
            if (fileType == null)
            {
                return HttpNotFound();
            }
            return View(fileType);
        }

        // GET: FileTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FileTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FileTypeId,Description")] FileType fileType)
        {
            if (ModelState.IsValid)
            {
                db.FileTypes.Add(fileType);
                db.SaveChanges();
                Audit.Trail(fileType.Description, eAction.Create, fileType.FileTypeId.ToString());
                return RedirectToAction("Index");
            }

            return View(fileType);
        }

        // GET: FileTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileType fileType = db.FileTypes.Find(id);
            if (fileType == null)
            {
                return HttpNotFound();
            }
            return View(fileType);
        }

        // POST: FileTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FileTypeId,Description")] FileType fileType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fileType).State = EntityState.Modified;
                db.SaveChanges();
                Audit.Trail(fileType.Description, eAction.Update, fileType.FileTypeId.ToString());
                return RedirectToAction("Index");
            }
            return View(fileType);
        }

        // GET: FileTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileType fileType = db.FileTypes.Find(id);
            if (fileType == null)
            {
                return HttpNotFound();
            }
            return View(fileType);
        }

        // POST: FileTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FileType fileType = db.FileTypes.Find(id);
            db.FileTypes.Remove(fileType);
            db.SaveChanges();
            Audit.Trail(fileType.Description, eAction.Delete, fileType.FileTypeId.ToString());
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
