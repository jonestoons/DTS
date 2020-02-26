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
using Microsoft.Ajax.Utilities;

namespace DocumentTracking.Controllers
{
    [Authorize]
    public class DocumentMovementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [HttpPost]
        public ActionResult Confirm(int id)
        {
            DocumentMovement documentMovement = db.DocumentMovements.Find(id);
            if (documentMovement == null)
            {
                return HttpNotFound();
            }

            var Incoming = documentMovement;

            documentMovement.DateStatus = DateTime.Now;
            documentMovement.Status = Status.Confirmed;
            db.Entry(documentMovement).State = EntityState.Modified;
            db.SaveChanges();
            Audit.Trail(documentMovement.PrefID, eAction.Confirmed, documentMovement.Id.ToString());

            Incoming.Type = Movement.Incoming;
            Incoming.Status = Status.Confirmed;
            Incoming.Source = documentMovement.Destination;
            Incoming.Destination = MiscClass.getUserUnit().ToString();
            db.DocumentMovements.Add(Incoming);
            db.SaveChanges();
            Audit.Trail(Incoming.PrefID, eAction.Create, Incoming.Id.ToString());


            return RedirectToAction("Notifications");
        }
        public ActionResult Notifications()
        {
            string emp = HttpContext.User.Identity.Name;
            var user = db.Users.Where(z => z.UserName == emp).FirstOrDefault();
            var pending = db.DocumentMovements.Where(x => x.Status == Status.Pending && x.Destination == user.Unit).OrderBy(x=>x.DateCreated).ToList();
            return View(pending);
        }
        public ActionResult FileMovement(string FileId)
        {
            if (FileId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<DocumentMovement> documentMovement = db.DocumentMovements.Where(z => z.PrefID == FileId).OrderBy(x => x.DateCreated).ToList();
            if (documentMovement == null)
            {
                return HttpNotFound();
            }
            ViewBag.FileId = FileId;
            return View(documentMovement.OrderByDescending(s => s.DateCreated));
        }

        public ActionResult CompanyLists(string Company)
        {
            if (Company == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<DocumentMovement> documentMovement = db.DocumentMovements.Where(z => z.Description == Company)
                .OrderBy(x => x.DateCreated)
                .DistinctBy(x => x.PrefID)
                .ToList();

            

            if (documentMovement == null)
            {
                return HttpNotFound();
            }
            ViewBag.Company = Company;
            return View(documentMovement);
        }

        // GET: DocumentMovements
        public ActionResult Index()
        {
            return View(db.DocumentMovements.ToList());
        }

        // GET: DocumentMovements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentMovement documentMovement = db.DocumentMovements.Find(id);
            if (documentMovement == null)
            {
                return HttpNotFound();
            }
            return View(documentMovement);
        }

        // GET: DocumentMovements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DocumentMovements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PrefID,Description,Source,Destination,DateCreated,DateStatus,Status,Type,Linked,OldId,LinkDate")] DocumentMovement documentMovement)
        {
            if (ModelState.IsValid)
            {
                db.DocumentMovements.Add(documentMovement);
                db.SaveChanges();
                Audit.Trail(documentMovement.PrefID, eAction.Create, documentMovement.Id.ToString());
                return RedirectToAction("Index");
            }

            return View(documentMovement);
        }

        // GET: DocumentMovements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentMovement documentMovement = db.DocumentMovements.Find(id);
            if (documentMovement == null)
            {
                return HttpNotFound();
            }
            return View(documentMovement);
        }

        // POST: DocumentMovements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PrefID,Description,Source,Destination,DateCreated,DateStatus,Status,Type,Linked,OldId,LinkDate")] DocumentMovement documentMovement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documentMovement).State = EntityState.Modified;
                db.SaveChanges();
                Audit.Trail(documentMovement.PrefID, eAction.Update, documentMovement.Id.ToString());
                return RedirectToAction("Index");
            }
            return View(documentMovement);
        }

        // GET: DocumentMovements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentMovement documentMovement = db.DocumentMovements.Find(id);
            if (documentMovement == null)
            {
                return HttpNotFound();
            }
            return View(documentMovement);
        }

        // POST: DocumentMovements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocumentMovement documentMovement = db.DocumentMovements.Find(id);
            db.DocumentMovements.Remove(documentMovement);
            db.SaveChanges();
            Audit.Trail(documentMovement.PrefID, eAction.Delete, documentMovement.Id.ToString());
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
