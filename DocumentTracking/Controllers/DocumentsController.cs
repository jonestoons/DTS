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
using System.Data.Entity.Validation;
using System.Text;

namespace DocumentTracking.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        [HttpPost]
        public JsonResult getCompanyFiles(int ID)
        {
            var company = db.Companys.Where(c => c.Id == ID).FirstOrDefault();
            List<Document> ds = db.Documents.Where(x => x.Description == company.Description).ToList();

            List<DisplayData> dt = new List<DisplayData>();
            foreach (var item in ds)
            {
                DisplayData ns = new DisplayData();
                ns.PrefID = item.FileId;
                ns.Description = item.Subject;
                ns.FileType = MiscClass.getFileType(item.FileTypeId);
                ns.DateCreated = item.DateCreated.Value.ToShortDateString();

                dt.Add(ns);
            }
            return Json(dt, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getName(string Prefix)
        {

            List<Company> CList = db.Companys.ToList();
            //Searching records from list using LINQ query
            var companyName = (from N in CList
                               where N.Description.StartsWith(Prefix.ToUpper())
                               select new { N.Description });
            return Json(companyName, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentDetails(string FileID)
        {
            DocDetails detail = new DocDetails();
            Document s = db.Documents.Where(n => n.FileId == FileID).FirstOrDefault();
            if (s != null)
            {
                detail.FileId = s.FileId;
                detail.Subject = s.Subject;
                detail.Description = s.Description;
                detail.FileType = db.FileTypes.Where(n => n.FileTypeId == s.FileTypeId).FirstOrDefault().Description;
                detail.DateCreated = s.DateCreated.Value.ToShortDateString();
                detail.UserID = MiscClass.GetFullName(s.UserID);
                return Json(detail, JsonRequestBehavior.AllowGet);

            }
            else
            {
                detail = null;
                return Json("Empty", JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult NotificationCount()
        {
            int count = 0;
            var d = MiscClass.getUserUnit().ToString();
            var t = db.DocumentMovements.Where(k => k.Destination == d && k.Status == Status.Pending).OrderBy(x => x.DateCreated).ToList();
            count = t.Count;
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getData(string FileID)
        {
            var cd = db.DocumentMovements.Where(x => x.PrefID == FileID && x.Type == Movement.Incoming).FirstOrDefault();
            DisplayData ds = new DisplayData();
            ds.PrefID = cd.PrefID;
            ds.Destination = MiscClass.getUnit(Convert.ToInt32(cd.Destination));
            ds.FileType = MiscClass.getFileType(cd.FileType);
            ds.Description = cd.Description;
            ds.DateCreated = Convert.ToDateTime(cd.DateCreated).ToShortDateString();

            return Json(ds, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Merge() {

            ViewBag.CompanyList = new SelectList(db.Companys.ToList(), "Id", "Description");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Merge(Merge merge)
        {
            int Akin = Convert.ToInt32(merge.MainCompany);
            int Akin2 = Convert.ToInt32(merge.OldCompany);

            if (Akin == Akin2)
            {
                ModelState.AddModelError(string.Empty,"Please select a different Company");
                return View();

            }

            var MainCompanyExist = db.Companys.Where(r => r.Id == Akin).FirstOrDefault();

            if (MainCompanyExist != null)
            {

                Company CompanyExist = db.Companys.Where(r => r.Id == Akin2).FirstOrDefault();
                if (CompanyExist == null)
                {
                    ModelState.AddModelError(string.Empty, "Second Company doesn't exist");
                    return View();
                }
                else
                {
                    List<Document> mergeDoc = new List<Document>();
                    mergeDoc = db.Documents.Where(d => d.Description == MainCompanyExist.Description).ToList();
                    //foreach (Document item in mergeDoc)
                    //{
                    //    item.Description = merge.MainCompany;
                    //    item.
                    //   // MiscClass.updateDocuments(item);

                    //}
                    
                    using (var dbs = new ApplicationDbContext())
                    {
                        var some = db.Documents.Where(d => d.Description == CompanyExist.Description).ToList();

                        //foreach (var item in some)
                        //{
                            
                        //}
                        some.ForEach(a =>
                            {
                                a.Description = MainCompanyExist.Description;
                            }
                        );
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var errors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in errors.ValidationErrors)
                                {
                                    // get the error message 
                                    string errorMessage = validationError.ErrorMessage;
                                }
                            }
                        }
                       
                    };

                    List<DocumentMovement> mergeMovement = new List<DocumentMovement>();
                    mergeMovement = db.DocumentMovements.Where(d => d.Description == CompanyExist.Description).ToList();
                    foreach (var item in mergeMovement)
                    {
                        item.Description = MainCompanyExist.Description;
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.SaveChanges();


                    if (merge.Del == true)
                    {
                        Company company = db.Companys.Find(Akin2);
                        db.Companys.Remove(company);
                        db.SaveChanges();
                        //Audit.Trail(merge.OldCompany, eAction.Delete, merge.OldCompany);

                    }

                    //Audit.Trail(merge.OldCompany, eAction.Merge, merge.MainCompany);
                }

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Main Company doesn't exist");
            }

            ViewBag.CompanyList = new SelectList(db.Companys.ToList(), "Id", "Description");
            return View();
        }

        public ActionResult FileSearch()
        {
            ViewBag.Data = 0;
            List<DisplaySearch> ds = new List<DisplaySearch>();

            return View(ds);
        }

        [HttpPost]
        public ActionResult FileSearch(string FileId, string SearchType)
        {
            List<DisplaySearch> ds = new List<DisplaySearch>();
            DisplaySearch d = new DisplaySearch();


            if (SearchType == "Company")
            {
                ViewBag.Data = 1;

                ds = db.Companys.Where(k => k.Description.Contains(FileId))
                    .Select(y =>
                    new DisplaySearch()
                    {
                        CompanyID = y.Id,
                        Description = y.Description
                    }
                ).ToList();

            }
            else if (SearchType == "File")
            {
                ViewBag.Data = 2;

                ds = db.Documents.Where(x => x.FileId.Contains(FileId))
                    .Select(y =>
                   new DisplaySearch()
                   {
                       Description = y.FileId
                   }).ToList();

            }
            else if (SearchType == "Date")
            {
                ViewBag.Data = 2;
                List<string> dt = new List<string>();
                dt.Clear();
                dt = FileId.Split('/').ToList();
                string newDate = dt[1].ToString() + "/" + dt[0].ToString() + "/" + dt[2].ToString();
                DateTime? f = Convert.ToDateTime(newDate);
                ds = db.Documents.Where(x => DbFunctions.TruncateTime(x.DateCreated) == f)
                     .Select(y =>
                     new DisplaySearch()
                     {
                         Description = y.FileId
                     }).ToList();

                //ds = db.Documents.Where(x => x.FileId.Contains(FileId))
                //    .Select(y =>
                //   new DisplaySearch()
                //   {
                //       Description = y.FileId
                //   }).ToList();

            }
            else
            {
                ViewBag.Data = 0;
            }

            return View(ds);
        }

        public ActionResult SendBackLog()
        {
            //ViewBag.Save = 0;
            ViewBag.UnitId = new SelectList(db.Units.OrderBy(c => c.Description), "UnitId", "Description");
            return View();
        }

        [HttpPost]
        public JsonResult SendBackLog(string Unit, string PrefID, DateTime? sDate)
        {
            var outgoing = db.DocumentMovements.Where(x => x.PrefID == PrefID).FirstOrDefault();
            try
            {
                if (outgoing != null)
                {


                    outgoing.DateCreated = sDate;
                    outgoing.Source = outgoing.Destination;
                    outgoing.Destination = Unit;
                    outgoing.Type = Movement.Outgoing;
                    outgoing.Status = Status.Pending;

                    db.DocumentMovements.Add(outgoing);
                    Audit.Trail(outgoing.PrefID, eAction.Create, outgoing.Id.ToString());
                    db.SaveChanges();

                    return Json("Saved", JsonRequestBehavior.AllowGet);


                }
                else
                {
                    return Json("Not Saved", JsonRequestBehavior.AllowGet);

                }
                //ViewBag.UnitId = new SelectList(db.Units.OrderBy(c => c.Description), "UnitId", "Description");
                //ViewBag.Save = 1;


            }
            catch (Exception)
            {

                throw;
                //return Json("Send", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Send()
        {
            //ViewBag.Save = 0;
            ViewBag.UnitId = new SelectList(db.Units.OrderBy(c => c.Description), "UnitId", "Description");
            return View();
        }

        [HttpPost]
        public JsonResult Send(string Unit, string PrefID)
        {
            var outgoing = db.DocumentMovements.Where(x => x.PrefID == PrefID).FirstOrDefault();
            try
            {
                if (outgoing != null)
                {


                    outgoing.DateCreated = DateTime.Now;
                    outgoing.Source = outgoing.Destination;
                    outgoing.Destination = Unit;
                    outgoing.Type = Movement.Outgoing;
                    outgoing.Status = Status.Pending;

                    db.DocumentMovements.Add(outgoing);
                    Audit.Trail(outgoing.PrefID, eAction.Create, outgoing.Id.ToString());
                    db.SaveChanges();

                    return Json("Saved", JsonRequestBehavior.AllowGet);


                }
                else
                {
                    return Json("Not Saved", JsonRequestBehavior.AllowGet);

                }
                //ViewBag.UnitId = new SelectList(db.Units.OrderBy(c => c.Description), "UnitId", "Description");
                //ViewBag.Save = 1;


            }
            catch (Exception)
            {

                throw;
                //return Json("Send", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult CompanySearch(string company)
        {

            List<DocumentMovement> CList = db.DocumentMovements.Where(v => v.Description == company && v.Type == Movement.Incoming).OrderBy(c => c.DateCreated).ToList();
            string us = MiscClass.getUserUnit().ToString();
            List<DocumentMovement> CLists = new List<DocumentMovement>();

            int f = 0;
            foreach (var item in CList)
            {
                List<DocumentMovement> c = db.DocumentMovements.Where(v => v.PrefID == item.PrefID).ToList();

                DocumentMovement df = c.Where(x => x.PrefID == item.PrefID).LastOrDefault();

                if (df.Status == Status.Confirmed && df.Destination == us && df.Type == Movement.Incoming)
                {
                    CLists.Add(item);
                }
                //if (c != null)
                //{
                //    if (c.Destination == us)
                //    {
                //        CLists.Add(item);
                //        //CList.RemoveAt(f);
                //    }
                //}


                f = f + 1;

            }


            return Json(CLists, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getUnit(string DeptId)
        {
            int dept = Convert.ToInt32(DeptId);
            var unitList = db.Units.Where(c => c.DeptID == dept).ToList();
            return Json(unitList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IDSearch(string Id)
        {
            List<DocumentMovement> CList = db.DocumentMovements.Where(v => v.PrefID == Id && v.Destination == "" && v.Type == Movement.Incoming).OrderBy(c => c.DateCreated).ToList();

            return Json(CList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LinkFileID()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LinkFileID(Link link)
        {

            Document doc = new Document();
            doc = db.Documents.Where(x => x.FileId == link.OldID).FirstOrDefault();
            if (doc != null)
            {
                doc.OldFileId = doc.FileId;
                doc.FileId = link.NewID;
                db.Entry(doc).State = EntityState.Modified;

                List<DocumentMovement> docMov = new List<DocumentMovement>();
                docMov = db.DocumentMovements.Where(p => p.PrefID == link.OldID).ToList();
                foreach (DocumentMovement item in docMov)
                {
                    item.OldId = item.PrefID;
                    item.PrefID = link.NewID;
                    db.Entry(item).State = EntityState.Modified;

                }
                db.SaveChanges();
                Audit.Trail(link.OldID, eAction.Link, doc.FileId.ToString());

            }

            return View();
        }

        // GET: Documents
        public ActionResult Index()
        {
            var unit = MiscClass.getUserUnit();
            List<Document> documents = new List<Document>();
            if (User.IsInRole("Super Admin"))
            {
                documents = db.Documents.Include(d => d.FileType).Include(d => d.Unit).ToList();

            }
            else
            {
                documents = db.Documents.Where(x => x.UnitId == unit).Include(d => d.FileType).Include(d => d.Unit).ToList();

            }
            return View(documents);
        }

        // GET: Documents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        [HttpPost]
        public JsonResult getFileID(int unit)
        {
            string FileId = MiscClass.GetUniqueKey(unit);
            return Json(FileId, JsonRequestBehavior.AllowGet);
        }

        // GET: Documents/Create
        public ActionResult Create()
        {
            ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description");
            int s = MiscClass.getUserUnit();
            ViewBag.UnitId = new SelectList(db.Units.Where(x => x.UnitId == s).ToList(), "UnitId", "Description");
            ViewBag.DeptId = new SelectList(db.Departments.OrderBy(x => x.Description).ToList(), "DeptID", "Description");
            return View();
        }

        #region Backlog

        // GET: Documents/Create
        public ActionResult CreateBackLog()
        {
            ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description");
            int s = MiscClass.getUserUnit();
            ViewBag.UnitId = new SelectList(db.Units.Where(x => x.UnitId == s).ToList(), "UnitId", "Description");
            ViewBag.DeptId = new SelectList(db.Departments.OrderBy(x => x.Description).ToList(), "DeptID", "Description");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBackLog([Bind(Include = "Id,FileId,Description,Subject,DateCreated,UnitId,FileTypeId")] Document document)
        {

            document.UserID = HttpContext.User.Identity.Name;
            //document.DateCreated = Convert.ToDateTime(document.DateCreated);
            //document.DateCreated = DateTime.Now;

            if ((document.FileId == null && document.FileTypeId == 1) || document.DateCreated == null)
            {
                ModelState.AddModelError("", "Please Complete all Details File ID or Date");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (document.FileId == null)
                    {
                        var td = db.FileTypes.Where(x => x.FileTypeId == document.FileTypeId).FirstOrDefault();
                        string prefix = "";

                        switch (td.Description.ToUpper())
                        {
                            case "PROPOSALS":
                                prefix = "PRP";
                                break;
                            case "LETTERS":
                                prefix = "LTR";
                                break;
                            case "MEMO":
                                prefix = "MEM";
                                break;
                            default:
                                prefix = "TEMP";
                                break;
                        }
                        string ts = MiscClass.GenerateNumber(5);
                        document.FileId = prefix + "/" + DateTime.Today.Year.ToString() + "/" + ts;
                    }
                    else
                    {
                        var sp = document.FileId.ToArray();
                        StringBuilder sd = new StringBuilder();
                        foreach (var item in sp)
                        {
                            if (item.ToString() == "&")
                            {
                                sd.Append("_");
                            }
                            else
                            {
                                sd.Append(item.ToString());
                            }
                        }
                        document.FileId = sd.ToString();

                    }
                    var isExist = db.Documents.Where(x => x.FileId == document.FileId).FirstOrDefault();

                    if (isExist == null)
                    {
                        DocumentMovement doc = new DocumentMovement();
                        doc.DateCreated = document.DateCreated;
                        doc.Type = Movement.Incoming;
                        doc.Source = MiscClass.getUserUnit().ToString();
                        doc.Destination = document.UnitId.ToString();
                        doc.PrefID = document.FileId;
                        doc.Status = Status.Confirmed;
                        doc.DateStatus = document.DateCreated;
                        doc.Description = document.Description;
                        doc.Subject = document.Subject;
                        doc.FileType = document.FileTypeId;

                        db.DocumentMovements.Add(doc);
                        db.Documents.Add(document);

                        db.SaveChanges();
                        Audit.Trail(doc.PrefID, eAction.Create, doc.Id.ToString());

                    }
                    else
                    {
                        ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description", document.FileTypeId);
                        ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", document.UnitId);
                        ViewBag.DeptId = new SelectList(db.Departments.OrderBy(x => x.Description).ToList(), "DeptID", "Description");
                        ViewBag.Err = 1;
                        return View();
                    }


                    return RedirectToAction("Index");
                }
                ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description", document.FileTypeId);
                ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", document.UnitId);
            }
            ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description", document.FileTypeId);
            ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", document.UnitId);
            ViewBag.DeptId = new SelectList(db.Departments.OrderBy(x => x.Description).ToList(), "DeptID", "Description");

            return View(document);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateSendBackLog([Bind(Include = "Id,FileId,Description,Subject,DateCreated,UnitId,FileTypeId,Destination,DateSend")] CreateSend Cdocument)
        {
            try
            {
                Document document = new Document();
                document.Description = Cdocument.Description;
                document.FileId = Cdocument.FileId;
                document.Subject = Cdocument.Subject;
                document.FileTypeId = Cdocument.FileTypeId;
                document.UnitId = Cdocument.UnitId;

                document.UserID = HttpContext.User.Identity.Name;
                //var s = Convert.ToDateTime(Cdocument.DateCreated);
                document.DateCreated = Cdocument.DateCreated;

                //if (ModelState.IsValid)


                if ((document.FileId == null && document.FileTypeId == 1) || document.DateCreated == null)
                {
                    ModelState.AddModelError("", "Please Complete all Details File ID or Date");
                }
                else
                {
                    if (document.FileId == null)
                    {
                        var td = db.FileTypes.Where(x => x.FileTypeId == document.FileTypeId).FirstOrDefault();
                        string prefix = "";

                        switch (td.Description.ToUpper())
                        {
                            case "PROPOSALS":
                                prefix = "PRP";
                                break;
                            case "LETTERS":
                                prefix = "LTR";
                                break;
                            case "MEMO":
                                prefix = "MEM";
                                break;
                            default:
                                prefix = "TEMP";
                                break;
                        }
                        string ts = MiscClass.GenerateNumber(5);
                        document.FileId = prefix + "/" + DateTime.Today.Year.ToString() + "/" + ts;
                    }
                    else
                    {
                        var sp = document.FileId.ToArray();
                        StringBuilder sd = new StringBuilder();
                        foreach (var item in sp)
                        {
                            if (item.ToString() == "&")
                            {
                                sd.Append("_");
                            }
                            else
                            {
                                sd.Append(item.ToString());
                            }
                        }
                        document.FileId = sd.ToString();

                    }
                    var isExist = db.Documents.Where(x => x.FileId == document.FileId).FirstOrDefault();

                    if (isExist == null)
                    {
                        DocumentMovement doc = new DocumentMovement();
                        doc.DateCreated = document.DateCreated;
                        doc.Type = Movement.Incoming;
                        doc.Source = MiscClass.getUserUnit().ToString();
                        doc.Destination = document.UnitId.ToString();
                        doc.PrefID = document.FileId;
                        doc.Status = Status.Confirmed;
                        doc.DateStatus = document.DateCreated;
                        doc.Description = document.Description;
                        doc.FileType = document.FileTypeId;
                        doc.Subject = document.Subject;

                        db.DocumentMovements.Add(doc);
                        db.Documents.Add(document);

                        db.SaveChanges();
                        Audit.Trail(doc.PrefID, eAction.Create, doc.Id.ToString());


                        var outgoing = db.DocumentMovements.Where(x => x.PrefID == document.FileId).FirstOrDefault();
                        if (outgoing != null)
                        {
                            outgoing.DateCreated = Cdocument.DateSend;
                            outgoing.Source = outgoing.Destination;
                            outgoing.Destination = Cdocument.Destination;
                            outgoing.Type = Movement.Outgoing;
                            outgoing.Status = Status.Pending;

                            db.DocumentMovements.Add(outgoing);
                            Audit.Trail(outgoing.PrefID, eAction.Create, outgoing.Id.ToString());
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description", document.FileTypeId);
                        ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", document.UnitId);
                        return View(document);
                    }


                    return RedirectToAction("Index");
                }

                ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description", document.FileTypeId);
                ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", document.UnitId);
                ViewBag.DeptId = new SelectList(db.Departments.OrderBy(x => x.Description).ToList(), "DeptID", "Description");

                return View(document);

            }
            catch (Exception e)
            {
                var ed = e;
                throw;
            }
        }
        #endregion

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FileId,Description,Subject,DateCreated,UnitId,FileTypeId")] Document document)
        {

            document.UserID = HttpContext.User.Identity.Name;
            document.DateCreated = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (document.FileId == null)
                {
                    var td = db.FileTypes.Where(x => x.FileTypeId == document.FileTypeId).FirstOrDefault();
                    string prefix = "";

                    switch (td.Description.ToUpper())
                    {
                        case "PROPOSALS":
                            prefix = "PRP";
                            break;
                        case "LETTERS":
                            prefix = "LTR";
                            break;
                        case "MEMO":
                            prefix = "MEM";
                            break;
                        default:
                            prefix = "TEMP";
                            break;
                    }
                    string ts = MiscClass.GenerateNumber(5);
                    document.FileId = prefix + "/" + DateTime.Today.Year.ToString() + "/" + ts;
                }
                var isExist = db.Documents.Where(x => x.FileId == document.FileId).FirstOrDefault();

                if (isExist == null)
                {
                    DocumentMovement doc = new DocumentMovement();
                    doc.DateCreated = document.DateCreated;
                    doc.Type = Movement.Incoming;
                    doc.Source = MiscClass.getUserUnit().ToString();
                    doc.Destination = document.UnitId.ToString();
                    doc.PrefID = document.FileId;
                    doc.Subject = document.Subject;
                    doc.Status = Status.Confirmed;
                    doc.DateStatus = document.DateCreated;
                    doc.Description = document.Description;
                    doc.FileType = document.FileTypeId;

                    db.DocumentMovements.Add(doc);
                    db.Documents.Add(document);

                    db.SaveChanges();
                    Audit.Trail(doc.PrefID, eAction.Create, doc.Id.ToString());

                }
                else
                {
                    ModelState.AddModelError(String.Empty, "FileID Already Exists");
                    ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description", document.FileTypeId);
                    ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", document.UnitId);
                    ViewBag.DeptId = new SelectList(db.Departments.OrderBy(x => x.Description).ToList(), "DeptID", "Description");

                    return View(document);
                }


                return RedirectToAction("Index");
            }

            ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description", document.FileTypeId);
            ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", document.UnitId);
            return View(document);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateSend([Bind(Include = "Id,FileId,Description,Subject,DateCreated,UnitId,FileTypeId,Destination,DateSend")] CreateSend Cdocument)
        {
            try
            {
                Document document = new Document();
                document.Description = Cdocument.Description;
                document.FileId = Cdocument.FileId;
                document.Subject = Cdocument.Subject;
                document.FileTypeId = Cdocument.FileTypeId;
                document.UnitId = Cdocument.UnitId;

                document.UserID = HttpContext.User.Identity.Name;
                document.DateCreated = DateTime.Now;
                if (ModelState.IsValid)
                {
                    if (document.FileId == null)
                    {
                        var td = db.FileTypes.Where(x => x.FileTypeId == document.FileTypeId).FirstOrDefault();
                        string prefix = "";

                        switch (td.Description.ToUpper())
                        {
                            case "PROPOSALS":
                                prefix = "PRP";
                                break;
                            case "LETTERS":
                                prefix = "LTR";
                                break;
                            case "MEMO":
                                prefix = "MEM";
                                break;
                            default:
                                prefix = "TEMP";
                                break;
                        }
                        string ts = MiscClass.GenerateNumber(5);
                        document.FileId = prefix + "/" + DateTime.Today.Year.ToString() + "/" + ts;
                    }
                    var isExist = db.Documents.Where(x => x.FileId == document.FileId).FirstOrDefault();

                    if (isExist == null)
                    {
                        DocumentMovement doc = new DocumentMovement();
                        doc.DateCreated = document.DateCreated;
                        doc.Type = Movement.Incoming;
                        doc.Source = MiscClass.getUserUnit().ToString();
                        doc.Destination = document.UnitId.ToString();
                        doc.PrefID = document.FileId;
                        doc.Status = Status.Confirmed;
                        doc.DateStatus = document.DateCreated;
                        doc.Description = document.Description;
                        doc.FileType = document.FileTypeId;
                        doc.Subject = document.Subject;

                        db.DocumentMovements.Add(doc);
                        db.Documents.Add(document);

                        db.SaveChanges();
                        Audit.Trail(doc.PrefID, eAction.Create, doc.Id.ToString());

                        var outgoing = db.DocumentMovements.Where(x => x.PrefID == document.FileId).FirstOrDefault();
                        if (outgoing != null)
                        {
                            var ds = Convert.ToDateTime(Cdocument.DateSend);
                            outgoing.DateCreated = ds;
                            outgoing.Source = outgoing.Destination;
                            outgoing.Destination = Cdocument.Destination;
                            outgoing.Type = Movement.Outgoing;
                            outgoing.Status = Status.Pending;

                            db.DocumentMovements.Add(outgoing);
                            Audit.Trail(outgoing.PrefID, eAction.Create, outgoing.Id.ToString());
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "FileID Already Exists");
                        ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description", document.FileTypeId);
                        ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", document.UnitId);
                        return View(document);
                    }


                    return RedirectToAction("Index");
                }

                ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description", document.FileTypeId);
                ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", document.UnitId);
                return View(document);

            }
            catch (Exception e)
            {
                var ed = e;
                throw;
            }
        }


        // GET: Documents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description", document.FileTypeId);
            ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", document.UnitId);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FileId,Description,Subject,DateCreated,UnitId,FileTypeId")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                Audit.Trail(document.FileId, eAction.Update, document.Id.ToString());
                return RedirectToAction("Index");
            }
            ViewBag.FileTypeId = new SelectList(db.FileTypes, "FileTypeId", "Description", document.FileTypeId);
            ViewBag.UnitId = new SelectList(db.Units, "UnitId", "Description", document.UnitId);
            return View(document);
        }

        // GET: Documents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Document document = db.Documents.Find(id);
            db.Documents.Remove(document);
            db.SaveChanges();
            Audit.Trail(document.FileId, eAction.Delete, document.Id.ToString());
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
