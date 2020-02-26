using DocumentTracking.Models;
using DocumentTracking.Models.DataModels;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocumentTracking.Controllers
{
    public class UploadController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Upload
        public ActionResult Index()
        {
            ViewBag.Uploaded = 0;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {


                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    //reader.IsFirstRowAsColumnNames = true;

                    DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    reader.Close();

                    DataTable tbl = result.Tables[0];

                    foreach (DataRow item in tbl.Rows)
                    {
                        Document document = new Document();

                        document.UserID = HttpContext.User.Identity.Name;
                        document.Subject = item[2].ToString();
                        document.Description = item[1].ToString();
                        document.DateCreated = Convert.ToDateTime(item[3].ToString());
                        document.UnitId = Convert.ToInt32(item[4].ToString());

                        string fd = item[5].ToString();
                        string prefix = "";
                        string ts = MiscClass.GenerateNumber(5);

                        switch (fd)
                        {
                            case "LETTERS & CORESPONDENCES":
                                prefix = "LTR";
                                document.FileId = prefix + "/" + DateTime.Today.Year.ToString() + "/" + ts;
                                document.FileTypeId = 2;
                                break;
                            case "FILE":
                                document.FileId = item[0].ToString();
                                document.FileTypeId = 1;
                                break;
                            case "PROPOSAL":
                                prefix = "PRP";
                                document.FileId = prefix + "/" + DateTime.Today.Year.ToString() + "/" + ts;
                                document.FileTypeId = 3;
                                break;
                            case "REQUEST":
                                prefix = "TEMP";
                                document.FileId = prefix + "/" + DateTime.Today.Year.ToString() + "/" + ts;
                                document.FileTypeId = 5;
                                break;
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
                    }

                    ViewBag.recCounts = tbl.Rows.Count;
                    ViewBag.Uploaded = 1;

                    return View(result.Tables[0]);
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();
        }
    }
}