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
    [Authorize]
    public class CompaniesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Companies
        public ActionResult Index()
        {
            return View(db.Companys.ToList());
        }

        // GET: Companies/Details/5
        [Authorize(Roles = "Admin, Super Admin")]

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companys.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

    [Authorize(Roles = "Admin, Super Admin")]
        // GET: Companies/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CreateJSON([Bind(Include = "Id,Description")]Company company)
        {
            if (ModelState.IsValid)
            {
                db.Companys.Add(company);
                db.SaveChanges();
                Audit.Trail(company.Description, eAction.Create, company.Id.ToString());
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
    [Authorize(Roles = "Admin, Super Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Companys.Add(company);
                db.SaveChanges();
                Audit.Trail(company.Description, eAction.Create, company.Id.ToString());
                return RedirectToAction("Index");
            }

            return View(company);
        }

    [Authorize(Roles = "Admin, Super Admin")]
        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companys.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
    [Authorize(Roles = "Admin, Super Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                Audit.Trail(company.Description, eAction.Update, company.Id.ToString());
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Companies/Delete/5
    [Authorize(Roles = "Admin, Super Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companys.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

    [Authorize(Roles = "Admin, Super Admin")]
        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companys.Find(id);
            db.Companys.Remove(company);
            db.SaveChanges();
            Audit.Trail(company.Description, eAction.Delete, company.Id.ToString());
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
